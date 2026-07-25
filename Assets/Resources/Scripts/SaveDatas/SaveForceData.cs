using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
[System.Serializable]
public class SaveForceData
{
    public int forceId;
    public bool isPlayer;
    public bool isEliminated;
    public float gold;
    public float scipoint;

    [NonSerialized]
    public TurnPhase phase = TurnPhase.None;
    [NonSerialized]
    public List<WarPlanData> warPlans = new List<WarPlanData>();
    [NonSerialized]
    public bool planConfirmed = false;

    [NonSerialized]
    private Dictionary<string, float> posResCache = new Dictionary<string, float>();

    [NonSerialized]
    private Dictionary<string, int> resUsedCache = new Dictionary<string, int>();

    public List<int> unlockedTechIds = new List<int>();

    /// <summary>
    /// 科技研究进度（序列化用），techId → 已积累科技值
    /// </summary>
    public List<TechProgressData> techProgressList = new List<TechProgressData>();

    /// <summary>
    /// 科技研究进度（运行时用），techId → 已积累科技值
    /// </summary>
    [NonSerialized]
    public Dictionary<int, int> techProgressDict = new Dictionary<int, int>();

    public List<KingActionCountData> kingActionCountList = new List<KingActionCountData>();

    [NonSerialized]
    public Dictionary<int, int> kingActionCounts = new Dictionary<int, int>();

    public void AddAttr(string type, float add, string reason = "")
    {
        var attrConfig = CityAttrConfig.GetConfigByname(type.ToLower());
        if (!attrConfig.IsForceAttr)
        {
            GameLog.Error($"AddAttr: {type} is not force attr");
            return;
        }
        
        if (attrConfig.IsPosRes)
        {
            GameLog.Error($"AddAttr: {type} is IsPosRes, use SetAttrVal instead");
            return;
        }
        
        float oldVal = 0;
        switch (type.ToLower())
        {
            case "gold":
                oldVal = gold;
                gold = Math.Min(gold + add, attrConfig.ValMaxForce);
                break;
            case "scipoint":
                oldVal = scipoint;
                scipoint = Math.Min(scipoint + add, attrConfig.ValMaxForce);
                break;
            default:
                break;
        }

        float newVal = type.ToLower() == "scipoint" ? scipoint : gold;
        GameLog.Info($"SaveForceData.AddAttr forceId={forceId} type={type} old={oldVal} add={add} new={newVal} reason={reason}");

        if (isPlayer)
        {
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = type.ToLower(), Value = GetAttr(type.ToLower()), Used = GetResUsed(type.ToLower()) });
        }
    }

    public float GetAttr(string type)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(type.ToLower());
        if (!attrConfig.IsForceAttr)
            return 0;
        
        if (attrConfig.IsPosRes)
        {
            return GetPosResFromCache(type.ToLower());
        }
        
        switch (type.ToLower())
        {
            case "gold":
                return gold;
            case "scipoint":
                return scipoint;
            default:
                return 0;
        }
    }

    private float GetPosResFromCache(string type)
    {
        if (posResCache.ContainsKey(type))
            return posResCache[type];
        return 0;
    }

    public void RecalculatePosRes()
    {
        posResCache.Clear();
        foreach (var attr in CityAttrConfig.ConfigList)
        {
            if (!attr.IsPosRes || !attr.IsForceAttr)
                continue;
            posResCache[attr.name] = 0f;
        }
        
        var cities = GetCityList();
        foreach (var city in cities)
        {
            var assignments = city.GetDevAssignments();
            foreach (var assignment in assignments)
            {
                var devConfig = CityDevConfig.GetConfig(assignment.devId);
                if (string.IsNullOrEmpty(devConfig.DevAttr1))
                    continue;
                
                var attrConfig = CityAttrConfig.GetConfigByname(devConfig.DevAttr1.ToLower());
                if (!attrConfig.IsPosRes || !attrConfig.IsForceAttr)
                    continue;
                
                var heroData = GameManager.Instance.GetHero(assignment.heroId);
                if (heroData == null)
                    continue;
                
                float avgWeightedValue = SysFormula.City.GetHeroWeightedAttrValue(heroData, devConfig.Attrs);
                int tier = SysFormula.City.GetHeroTier(avgWeightedValue);
                
                string resType = devConfig.DevAttr1.ToLower();
                posResCache[resType] += devConfig.DevAttr1Value[tier];
            }
        }

        foreach (var city in cities)
        {
            var worldCfg = WorldConfig.GetConfig(city.cityId);
            if (worldCfg.ResAddon != null)
            {
                foreach (int addonId in worldCfg.ResAddon)
                {
                    var attrCfg = CityAttrConfig.GetConfig(addonId);
                    if (attrCfg.IsPosRes && attrCfg.IsForceAttr)
                    {
                        posResCache[attrCfg.name] += SystemConst.City.RES_ADDON_BONUS;
                    }
                }
            }
        }        
        
        if (isPlayer)
        {
            foreach (var kvp in posResCache)
            {
                PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = kvp.Key, Value = kvp.Value, Used = GetResUsed(kvp.Key) });
            }
        }
    }

    public void RecalculateResUsed()
    {
        resUsedCache.Clear();
        resUsedCache["horse"] = 0;
        resUsedCache["steel"] = 0;
        resUsedCache["wood"] = 0;
        resUsedCache["stone"] = 0;
        
        var cities = GetCityList();
        foreach (var city in cities)
        {
            foreach (var troop in SaveTroopsData.GetTroopsByCity(city.cityId))
            {
                if (troop.armsId <= 0)
                    continue;
                
                var armsConfig = ArmsConfig.GetConfig(troop.armsId);
                resUsedCache["horse"] += armsConfig.HorseCost;
                resUsedCache["steel"] += armsConfig.SteelCost;
                resUsedCache["wood"] += armsConfig.WoodCost;
                resUsedCache["stone"] += armsConfig.StoneCost;
            }
        }
    }

    public int GetResUsed(string resType)
    {
        if (resUsedCache == null)
            return 0;
        resType = resType.ToLower();
        if (resUsedCache.ContainsKey(resType))
            return resUsedCache[resType];
        return 0;
    }

    public bool CanAffordArms(int armsId, SaveTroopsData excludeTroop = null)
    {
        var armsConfig = ArmsConfig.GetConfig(armsId);
        
        float horseAvailable = GetAttr("horse") - GetResUsed("horse");
        float steelAvailable = GetAttr("steel") - GetResUsed("steel");
        float woodAvailable = GetAttr("wood") - GetResUsed("wood");
        float stoneAvailable = GetAttr("stone") - GetResUsed("stone");
        
        if (excludeTroop != null && excludeTroop.armsId > 0)
        {
            var excludeArmsConfig = ArmsConfig.GetConfig(excludeTroop.armsId);
            horseAvailable += excludeArmsConfig.HorseCost;
            steelAvailable += excludeArmsConfig.SteelCost;
            woodAvailable += excludeArmsConfig.WoodCost;
            stoneAvailable += excludeArmsConfig.StoneCost;
        }
        
        if (armsConfig.HorseCost > horseAvailable)
            return false;
        if (armsConfig.SteelCost > steelAvailable)
            return false;
        if (armsConfig.WoodCost > woodAvailable)
            return false;
        if (armsConfig.StoneCost > stoneAvailable)
            return false;
        
        return true;
    }

    public string Name
    {
        get
        {
            var forceCfg = ForceConfig.GetConfig(forceId);
            var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);
            return heroCfg.Name;
        }
    }

    public Color LineColor
    {
        get
        {
            return SysColor.GetForceColor(forceId);
        }
    }

    public string IconPath
    {
        get
        {
            var forceCfg = ForceConfig.GetConfig(forceId);
            var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);
            return "Textures/Skins/" + heroCfg.Icon;
        }
    }

    public void InitRuntimeState()
    {
        phase = TurnPhase.None;
        warPlans = new List<WarPlanData>();
        planConfirmed = false;
        posResCache = new Dictionary<string, float>();
        resUsedCache = new Dictionary<string, int>();
        SyncKingActionCountsFromList();
        SyncTechProgressFromList();
        RecalculatePosRes();
        RecalculateResUsed();
    }

    public void SetPhase(TurnPhase newPhase)
    {
        phase = newPhase;
    }

    public void AddWarPlan(WarPlanData warPlan)
    {
        warPlans.Add(warPlan);
        GameLog.Info($"AddWarPlan");
    }

    public void ResetRoundState()
    {
        warPlans = new List<WarPlanData>();
        planConfirmed = false;
        kingActionCountList.Clear();
        kingActionCounts.Clear();
    }

    /// <summary>
    /// 获取本回合指定 KingAction 已参与的英雄数
    /// </summary>
    public int GetKingActionCount(int devId)
    {
        if (kingActionCounts.TryGetValue(devId, out int count))
            return count;
        return 0;
    }

    /// <summary>
    /// 累加本回合指定 KingAction 的参与英雄数
    /// </summary>
    public void AddKingActionCount(int devId, int count)
    {
        if (kingActionCounts.ContainsKey(devId))
            kingActionCounts[devId] += count;
        else
            kingActionCounts.Add(devId, count);

        SyncKingActionCountsToList();
    }

    /// <summary>
    /// 从 List 同步到 Dictionary（加载后调用）
    /// </summary>
    private void SyncKingActionCountsFromList()
    {
        kingActionCounts = new Dictionary<int, int>();
        foreach (var item in kingActionCountList)
            kingActionCounts[item.devId] = item.count;
    }

    /// <summary>
    /// 从 Dictionary 同步到 List（修改后调用，确保序列化正确）
    /// </summary>
    private void SyncKingActionCountsToList()
    {
        kingActionCountList.Clear();
        foreach (var kv in kingActionCounts)
            kingActionCountList.Add(new KingActionCountData(kv.Key, kv.Value));
    }

    private void SyncTechProgressFromList()
    {
        techProgressDict = new Dictionary<int, int>();
        foreach (var item in techProgressList)
            techProgressDict[item.techId] = item.progress;
    }

    private void SyncTechProgressToList()
    {
        techProgressList.Clear();
        foreach (var kv in techProgressDict)
            techProgressList.Add(new TechProgressData(kv.Key, kv.Value));
    }

    /// <summary>
    /// 获取指定科技的已积累研究值
    /// </summary>
    public int GetTechProgress(int techId)
    {
        if (techProgressDict.TryGetValue(techId, out int progress))
            return progress;
        return 0;
    }

    /// <summary>
    /// 累加指定科技的研究值
    /// </summary>
    public void AddTechProgress(int techId, int value)
    {
        if (techProgressDict.ContainsKey(techId))
            techProgressDict[techId] += value;
        else
            techProgressDict.Add(techId, value);
        SyncTechProgressToList();
    }

    public void StartPlanningPhase()
    {
        phase = TurnPhase.Planning;

        if (isPlayer)
        {
            AIToolHeroDev.AssignHeroesToDev(this);
            PanelManager.Instance.SendSignal(new PhaseChangeSignal { PhaseName = "Planning", ForceId = forceId });
            PanelManager.Instance.SendSignal(new AICheckSignal { ForceId = 0 });
        }
        else
        {
            PanelManager.Instance.SendSignal(new AICheckSignal { AIName = Name, ForceId = forceId });
            GameManager.Instance.StartCoroutine(GameManager.Instance.AIForceTurnCoroutine(this));
        }
    }

    public bool ExecuteCityDev(int cityId, int devId, int[] heroList, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        if(heroList.Length == 0)
        {
            GameLog.Warn($"势力 {Name} 城市 {cityId} 发展任务 {devId} 失败，没有可用英雄");
            attrDatas = null;
            return false;
        }

        attrDatas = new List<PopResultPanelManager.AttrData>();
        var resultTmp = new List<float>();
        
        var devConfig = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        
        if (devConfig.Attrs.Length > 0 && !string.IsNullOrEmpty(devConfig.DevAttr1))
        {
            string mainAttr = devConfig.DevAttr1.ToLower();
            var attrConfig = CityAttrConfig.GetConfigByname(mainAttr);
            float currentVal = attrConfig.IsForceAttr ? GetAttr(mainAttr) : cityData.GetAttr(mainAttr);
            int valMax = attrConfig.IsForceAttr ? attrConfig.ValMaxForce : attrConfig.ValMaxCity;
            if (currentVal >= valMax)
            {
                GameLog.Warn($"势力 {Name} 城市 {cityId} 发展任务 {devId} 失败，{mainAttr} 已达最大值");
                return false;
            }
        }
        
        if (gold < devConfig.GoldCost * heroList.Length)
        {
            GameLog.Warn($"势力 {Name} 城市 {cityId} 发展任务 {devId} 失败，黄金不足");
            return false;
        }
        
        if (devConfig.GoldCost > 0)
        {
            int baseCost = devConfig.GoldCost * heroList.Length;
            float costReduce = ForceTech.GetDevCostReduce(forceId, devId);
            int actualCost = ForceTech.ApplyCostReduce(baseCost, costReduce);
            if (actualCost > 0)
                AddAttr("gold", -actualCost, "城市发展扣除金钱");
        }
        
        for (int i = 0; i < heroList.Length; i++)
        {
            var heroData = GameManager.Instance.GetHero(heroList[i]);
            
            float avgWeightedValue = SysFormula.City.GetHeroWeightedAttrValue(heroData, devConfig.Attrs);
            int tier = SysFormula.City.GetHeroTier(avgWeightedValue);

            resultTmp.Add(0);
            if (!string.IsNullOrEmpty(devConfig.DevAttr1))
            {
                var val = GetValByTier(devConfig.DevAttr1, devConfig.DevAttr1Value[tier], cityData.GetAttr(devConfig.DevAttr1));
                if (val > 0 && SaveCityData.CityHasResAddon(cityId, devConfig.DevAttr1))
                    val += SystemConst.City.RES_ADDON_BONUS;
                resultTmp[0] += val;
            }

            if (!string.IsNullOrEmpty(devConfig.DevAttr2) && devConfig.DevAttr2Value != null && devConfig.DevAttr2Value.Length > tier)
            {
                resultTmp.Add(0);
                var val = GetValByTier(devConfig.DevAttr2, devConfig.DevAttr2Value[tier], cityData.GetAttr(devConfig.DevAttr2));
                if (val > 0 && SaveCityData.CityHasResAddon(cityId, devConfig.DevAttr2))
                    val += SystemConst.City.RES_ADDON_BONUS;
                resultTmp[1] += val;
            }

        }

        List<float> results = new List<float>();
        for (int i = 0; i < resultTmp.Count; i++)
        {
            results.Add(resultTmp[i]);
        }

        ApplyProductionMultiplier(cityData, devConfig, results);
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr1))
        {
            var attr1Config = CityAttrConfig.GetConfigByname(devConfig.DevAttr1.ToLower());
            float attr1Old = attr1Config.IsForceAttr ? GetAttr(devConfig.DevAttr1) : cityData.GetAttr(devConfig.DevAttr1);
            if (!attr1Config.IsPosRes)
            {
                if (attr1Config.IsForceAttr)
                    AddAttr(devConfig.DevAttr1, results[0], "城市发展产出1");
                else
                    cityData.AddAttr(devConfig.DevAttr1, results[0], "城市发展产出1");
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attr = devConfig.DevAttr1,
                    valOld = attr1Old,
                    valAddon = results[0],
                });
            }
        }
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            var attr2Config = CityAttrConfig.GetConfigByname(devConfig.DevAttr2.ToLower());
            if (!attr2Config.IsPosRes)
            {
                float attr2Old = attr2Config.IsForceAttr ? GetAttr(devConfig.DevAttr2) : cityData.GetAttr(devConfig.DevAttr2);
                if (attr2Config.IsForceAttr)
                    AddAttr(devConfig.DevAttr2, results[1], "城市发展产出2");
                else
                    cityData.AddAttr(devConfig.DevAttr2, results[1], "城市发展产出2");
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attr = devConfig.DevAttr2,
                    valOld = attr2Old,
                    valAddon = results[1],
                });
            }
        }

        cityData.AddAction(devId, heroList.Length);

        return true;
    }

    private static void ApplyProductionMultiplier(SaveCityData cityData, CityDevConfig devConfig, List<float> results)
    {
        float multiplier = cityData.GetProductionMultiplier();
        if (multiplier >= 0.999f && multiplier <= 1.001f)
            return;

        int index = 0;
        if (!string.IsNullOrEmpty(devConfig.DevAttr1))
        {
            string attr1 = devConfig.DevAttr1.ToLower();
            if (attr1 == "food" || attr1 == "soldier" || attr1 == "gold")
                results[index] = results[index] * multiplier;
            index++;
        }
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            string attr2 = devConfig.DevAttr2.ToLower();
            if (attr2 == "food" || attr2 == "soldier" || attr2 == "gold")
                results[index] = results[index] * multiplier;
        }
    }

    private static float GetValByTier(string resName, float tierValue, float nowVal)
    {
        var cityAttrConfig = CityAttrConfig.GetConfigByname(resName.ToLower());
        int valMax = cityAttrConfig.IsForceAttr ? cityAttrConfig.ValMaxForce : cityAttrConfig.ValMaxCity;
        
        float addon = tierValue;
        float remaining = valMax - nowVal;
        
        if (addon > remaining)
            addon = Math.Max(0, remaining);
        
        return addon;
    }

    public void ExecuteBattle(List<int> srcCityIds, List<SaveTroopsData> attackTroops, Dictionary<int, int> attackSoldierMap, int targetCityId, bool isAI,
        List<SaveTroopsData> prebuiltDefenceTroops = null, Dictionary<int, int> prebuiltDefenceSoldierMap = null)
    {
        var cityDest = GameManager.Instance.GetCity(targetCityId);

        var destForce = GameManager.Instance.GetForce(cityDest.forceId);
        bool isPlayerInvolved = !isAI || destForce.isPlayer;
        if (isPlayerInvolved)
            BattleManager.Instance.SetMode(false, true);
        else
            BattleManager.Instance.SetMode(true, false);

        // 过滤兵力为0的troops
        var validAttackTroops = new List<SaveTroopsData>();
        var validAttackSoldierMap = new Dictionary<int, int>();
        for (int i = 0; i < attackTroops.Count; i++)
        {
            var t = attackTroops[i];
            if (t.heroId1 > 0 && attackSoldierMap.ContainsKey(t.heroId1) && attackSoldierMap[t.heroId1] > 0)
            {
                validAttackTroops.Add(t);
                validAttackSoldierMap[t.heroId1] = attackSoldierMap[t.heroId1];
            }
        }
        if (validAttackTroops.Count == 0)
        {
            GameLog.Warn($"ExecuteBattle 攻击方无有效部队，跳过战斗 targetCityId={targetCityId}");
            return;
        }

        // 标记攻击方武将本回合已行动
        MarkHeroesActed(validAttackSoldierMap.Keys);

        foreach (var srcCityId in srcCityIds)
        {
            var citySrc = GameManager.Instance.GetCity(srcCityId);
            citySrc.OnBattle();
            int totalSoldiers = validAttackTroops
                .Where(t => t.heroId1 > 0 && GameManager.Instance.GetHero(t.heroId1).cityId == srcCityId)
                .Sum(t => validAttackSoldierMap.ContainsKey(t.heroId1) ? validAttackSoldierMap[t.heroId1] : 0);
            GameLog.Info($"ExecuteBattle 扣除士兵和粮食 cityId={srcCityId} totalSoldiers={totalSoldiers}");
            citySrc.AddAttr("soldier", -totalSoldiers, "出征扣除士兵");
            citySrc.AddAttr("food", -totalSoldiers, "出征扣除粮草");
        }
        cityDest.OnBattle();
        int srcForceId = forceId;
        int destForceId = cityDest.forceId;
        GameManager.Instance.SaveData.forceRelation.RecordBattle(srcForceId, destForceId);

        List<SaveTroopsData> defenceTroops;
        Dictionary<int, int> defenceSoldierMap;
        if (prebuiltDefenceTroops != null && prebuiltDefenceSoldierMap != null)
        {
            defenceTroops = new List<SaveTroopsData>();
            defenceSoldierMap = new Dictionary<int, int>();
            int totalDefenceSoldiers = 0;
            for (int i = 0; i < prebuiltDefenceTroops.Count; i++)
            {
                var t = prebuiltDefenceTroops[i];
                if (t.heroId1 > 0 && prebuiltDefenceSoldierMap.ContainsKey(t.heroId1) && prebuiltDefenceSoldierMap[t.heroId1] > 0)
                {
                    defenceTroops.Add(t);
                    defenceSoldierMap[t.heroId1] = prebuiltDefenceSoldierMap[t.heroId1];
                    totalDefenceSoldiers += prebuiltDefenceSoldierMap[t.heroId1];
                }
            }
            cityDest.AddAttr("soldier", -totalDefenceSoldiers, "防御扣除士兵");
            cityDest.AddAttr("food", -totalDefenceSoldiers, "防御扣除粮草");
            GameLog.Info($"ExecuteBattle 使用预建防御部队 count={defenceTroops.Count} totalSoldiers={totalDefenceSoldiers}");
        }
        else
        {
            (defenceTroops, defenceSoldierMap) = TroopsBuilder.BuildDefenceTroops(cityDest);
            // 过滤兵力为0的防守troops
            var filteredDefenceTroops = new List<SaveTroopsData>();
            var filteredDefenceSoldierMap = new Dictionary<int, int>();
            for (int i = 0; i < defenceTroops.Count; i++)
            {
                var t = defenceTroops[i];
                if (t.heroId1 > 0 && defenceSoldierMap.ContainsKey(t.heroId1) && defenceSoldierMap[t.heroId1] > 0)
                {
                    filteredDefenceTroops.Add(t);
                    filteredDefenceSoldierMap[t.heroId1] = defenceSoldierMap[t.heroId1];
                }
            }
            defenceTroops = filteredDefenceTroops;
            defenceSoldierMap = filteredDefenceSoldierMap;
        }

        if (defenceTroops.Count == 0)
        {
            GameLog.Warn($"ExecuteBattle 防守方无有效部队，跳过战斗 targetCityId={targetCityId}");
            return;
        }

        int battleRound = GameManager.Instance.SaveData.round;
        var attackHeroIdList = validAttackSoldierMap.Keys.ToList();
        var defendHeroIdList = defenceSoldierMap.Keys.ToList();
        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateBattleAttack(battleRound, srcForceId, destForceId, targetCityId, attackHeroIdList));
        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateBattleDefend(battleRound, destForceId, srcForceId, targetCityId, defendHeroIdList));

        BattleManager.Instance.BattleBegin(this, cityDest.GetForce(), validAttackTroops, defenceTroops, validAttackSoldierMap, defenceSoldierMap, targetCityId,
            (result, attackerSoldierCount, defenderSoldierCount, round, gateAvgHp) => OnBattleEnd(result, attackerSoldierCount, defenderSoldierCount, round, gateAvgHp, srcCityIds, targetCityId, srcForceId, destForceId));
    }

    private void OnBattleEnd(BattleResult result, Dictionary<int, int> attackerSoldierCount, Dictionary<int, int> defenderSoldierCount, int round, float gateAvgHp, List<int> srcCityIds, int targetCityId, int srcForceId, int destForceId)
    {
        var destCity = GameManager.Instance.GetCity(targetCityId);
        GameLog.Info($"OnBattleEnd result={result} round={round} gateAvgHp={gateAvgHp} attackerCount={attackerSoldierCount.Count} defenderCount={defenderSoldierCount.Count}");

        int resultRound = GameManager.Instance.SaveData.round;
        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateBattleResult(
            resultRound, srcForceId, destForceId, targetCityId,
            attackerSoldierCount.Keys.ToList(), defenderSoldierCount.Keys.ToList(),
            result == BattleResult.Win));

        if (result == BattleResult.Win)
        {
            var attackHeroList = attackerSoldierCount.Keys.ToList();
            var defenceHeroList = defenderSoldierCount.Keys.ToList();
            
            destCity.Occupy(forceId, attackHeroList, destForceId, defenceHeroList);
        }

        if (result == BattleResult.Win)
        {
            float oldWall = destCity.GetAttr("wall");
            destCity.AddAttr("wall", -oldWall, "攻下城市城墙清零");
            GameLog.Info($"OnBattleEnd 攻方胜利，城墙清零 wall={destCity.GetAttr("wall")}");

            destCity.MultiplyAttr("happy", 0.5f);
            GameLog.Info($"OnBattleEnd 攻方胜利，民心减半50% happy={destCity.GetAttr("happy")}");
            
            destCity.MultiplyAttr("food", 0.5f);
            GameLog.Info($"OnBattleEnd 攻方胜利，粮食减半50% food={destCity.GetAttr("food")}");
        }
        else
        {
            if (gateAvgHp >= 0)
            {
                float oldWall = destCity.GetAttr("wall");
                float newWall = Math.Max(0, gateAvgHp);
                destCity.AddAttr("wall", newWall - oldWall, "战斗结束后城门平均血量设为城墙值");
                GameLog.Info($"OnBattleEnd 城门平均血量设为城墙值 gateAvgHp={gateAvgHp} wall={destCity.GetAttr("wall")}");
            }
            else
            {
                GameLog.Info($"OnBattleEnd 无城门，城墙值不变 wall={destCity.GetAttr("wall")}");
            }

            if (round > SystemConst.City.HAPPY_DECAY_START_ROUND)
            {
                float happyDecay = (round - SystemConst.City.HAPPY_DECAY_START_ROUND) * SystemConst.City.HAPPY_DECAY_PER_ROUND;
                destCity.AddAttr("happy", -happyDecay, "战斗回合过多民心衰减");
                GameLog.Info($"OnBattleEnd 战斗{round}回合，民心衰减 happyDecay={happyDecay} happy={destCity.GetAttr("happy")}");
            }

            if (round > SystemConst.City.DEFENCE_DISCOUNT_START_ROUND)
            {
                float discount = SysFormula.City.GetDefenceDevDiscount(round);
                destCity.defenceDevDiscount = discount;
                GameLog.Info($"OnBattleEnd 战斗{round}回合，防御方dev打折倍率={discount}");
            }
        }

        foreach (var kvp in attackerSoldierCount)
        {
            if (kvp.Value > 0)
            {
                var hero = GameManager.Instance.GetHero(kvp.Key);
                if (hero != null && hero.state == HeroState.Normal)
                {
                    var heroCity = GameManager.Instance.GetCity(hero.cityId);
                    if (heroCity != null)
                    {
                        heroCity.AddAttr("soldier", kvp.Value, "攻击方退回士兵");
                        GameLog.Info($"OnBattleEnd 攻击方退回士兵 heroId={kvp.Key} soldier={kvp.Value} cityId={hero.cityId}");
                    }
                }
            }
        }

        foreach (var kvp in defenderSoldierCount)
        {
            if (kvp.Value > 0)
            {
                var hero = GameManager.Instance.GetHero(kvp.Key);
                if (hero != null && hero.state == HeroState.Normal)
                {
                    var heroCity = GameManager.Instance.GetCity(hero.cityId);
                    if (heroCity != null)
                    {
                        heroCity.AddAttr("soldier", kvp.Value, "防守方退回士兵");
                        GameLog.Info($"OnBattleEnd 防守方退回士兵 heroId={kvp.Key} soldier={kvp.Value} cityId={hero.cityId}");
                    }
                }
            }
        }

        foreach (var srcCityId in srcCityIds)
        {
            var srcCity = GameManager.Instance.GetCity(srcCityId);
            if (srcCity != null)
                srcCity.RecalculateHeros();
        }
        
        destCity.RecalculateHeros();
    }

    public void MoveHeroToCity(int srcCityId, int destCityId, int[] heroIds, bool useDayDistance = false)
    {
        if (destCityId <= 0)
        {
            GameLog.Warn("MoveHeroToCity: destCityId is invalid");
            return;
        }

        if (srcCityId > 0)
        {
            var citySrc = GameManager.Instance.GetCity(srcCityId);
            if (citySrc != null)
            {
                citySrc.MoveHeroTo(heroIds, destCityId);
                citySrc.RecalculateHeros();
            }
        }

        var cityDest = GameManager.Instance.GetCity(destCityId);
        if (cityDest != null)
        {
            cityDest.RecalculateHeros();
        }

        if (useDayDistance)
        {
            int distance = SysFormula.City.CalculateMoveDayDistance(srcCityId, destCityId, forceId);
            MarkHeroesActed(heroIds, distance - 1);
        }
        else
        {
            MarkHeroesActed(heroIds);
        }

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionMove(
            GameManager.Instance.SaveData.round, forceId, srcCityId, destCityId, heroIds));

         PanelManager.Instance.SendSignal(new CityAttrChangeSignal { CityId = destCityId });
    }

    /// <summary>
    /// 标记英雄执行 KingAction 后的占用回合：hero.round = currentRound + dayDiff。
    /// dayDiff=0 表示仅本回合占用；dayDiff>0 表示额外占用多日（远距离等）。
    /// </summary>
    private void MarkHeroesActed(IEnumerable<int> heroIds, int dayDiff = 0)
    {
        int currentRound = GameManager.Instance.SaveData.round;
        foreach (var heroId in heroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero != null)
                hero.round = currentRound + dayDiff;
        }
    }

    /// <summary>
    /// 过滤掉本回合已行动（hero.round >= currentRound）的武将，返回可用武将列表。
    /// KingAction 方法入口的底层防御，配合 UI 层 HeroHeadItem 的选择拦截。
    /// </summary>
    private List<int> FilterAvailableHeroes(IEnumerable<int> heroIds)
    {
        int currentRound = GameManager.Instance.SaveData.round;
        var available = new List<int>();
        int filteredCount = 0;
        foreach (var heroId in heroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero != null && hero.round >= currentRound)
            {
                filteredCount++;
            }
            else
            {
                available.Add(heroId);
            }
        }
        if (filteredCount > 0)
        {
            GameLog.Warn($"FilterAvailableHeroes 过滤已行动武将 {filteredCount} 人");
        }
        return available;
    }

    public List<SaveCityData> GetCityList()
    {
         return GameManager.Instance.GetCitiesByForce(forceId);
    }

    public SaveCityData GetKingCity()
    {
        var kingHeroId = ForceConfig.GetConfig(forceId).HeroId;
        foreach (var city in GetCityList())
        {
            if (city.GetOwner() == kingHeroId)
                return city;
        }
        return null;
    }

    /// <summary>
    /// 交易行动：派遣 heroIds 武将各执行一次交易，每人花 GoldCost 金币兑换 tradeAmount 士兵/粮草
    /// </summary>
    public bool ExecuteCityTrade(int cityId, int devId, int[] heroIds, bool buySoldier, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCityTrade heroIds 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null)
        {
            GameLog.Error($"ExecuteCityTrade city not found cityId={cityId}");
            return false;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        int goldCost = devCfg.GoldCost;
        int heroCount = heroIds.Length;
        int baseTotalCost = heroCount * goldCost;
        float costReduce = ForceTech.GetKingActionCostReduce(forceId, devId);
        int totalCost = ForceTech.ApplyCostReduce(baseTotalCost, costReduce);
        int totalGain = 0;
        float tradeAmountMul = ForceTech.GetKingActionAmountMul(forceId, devId, "rate");
        foreach (var heroId in heroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            int inte = hero != null ? hero.inte : 0;
            int heroGain = SysFormula.Economy.CalculateHeroTradeAmount(goldCost, inte);
            heroGain = (int)ForceTech.ApplyAmountMul(heroGain, tradeAmountMul);
            totalGain += heroGain;
        }

        if (gold < totalCost)
        {
            SystemTip.Instance.ShowTip("黄金不足");
            return false;
        }

        string resType = buySoldier ? "soldier" : "food";
        int resOld = (int)cityData.GetAttr(resType);
        int goldOld = (int)gold;

        if (totalCost > 0)
        {
            AddAttr("gold", -totalCost, "交易扣除金钱");
        }
        cityData.AddAttr(resType, totalGain, "交易增加" + resType);

        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "Gold",
            valOld = goldOld,
            valAddon = -totalCost,
        });
        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = buySoldier ? "Soldier" : "Food",
            valOld = resOld,
            valAddon = totalGain,
        });

        cityData.AddAction(devId, heroCount);
        AddKingActionCount(devId, heroCount);
        MarkHeroesActed(heroIds);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionTrade(
            GameManager.Instance.SaveData.round, forceId, cityId, devId, heroIds, buySoldier, totalGain));

        GameLog.Info($"ExecuteCityTrade cityId={cityId} heroCount={heroCount} buySoldier={buySoldier} totalCost={totalCost} totalGain={totalGain}");
        return true;
    }

    /// <summary>
    /// 走访行动：派遣 heroIds 武将各执行一次走访，每人随机获得 [SEARCH_GOLD_MIN, SEARCH_GOLD_MAX] 金钱
    /// 并根据 CityDevSearchConfig 触发额外发现（资源/武将）
    /// </summary>
    public bool ExecuteCitySearch(int cityId, int devId, int[] heroIds, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCitySearch heroIds 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null)
        {
            GameLog.Error($"ExecuteCitySearch city not found cityId={cityId}");
            return false;
        }

        var devConfig = CityDevConfig.GetConfig(devId);
        int goldCost = devConfig.GoldCost * heroIds.Length;
        if (gold < goldCost)
        {
            SystemTip.Instance.ShowTip("黄金不足");
            GameLog.Warn($"ExecuteCitySearch gold not enough forceId={forceId} gold={gold} cost={goldCost}");
            return false;
        }
        if (devConfig.GoldCost > 0)
        {
            AddAttr("gold", -goldCost, "走访扣除金钱");
        }

        int heroCount = heroIds.Length;
        int searchResultType = 0;
        int totalResourceAmount = 0;
        var discoveredHeroIds = new List<int>();

        foreach (var heroId in heroIds)
        {
            var heroData = GameManager.Instance.GetHero(heroId);
            if (heroData == null) continue;

            var candidates = new List<CityDevSearchConfig>();
            foreach (var searchCfg in CityDevSearchConfig.ConfigList)
            {
                if (!SysFormula.Hero.CheckHeroCondition(searchCfg.Condition, heroData))
                    continue;

                if (searchCfg.ResType == "findhero" && !HasUndiscoveredHero(cityId, false))
                    continue;

                if (searchCfg.ResType == "findherostar" && !HasUndiscoveredHero(cityId, true))
                    continue;

                candidates.Add(searchCfg);
            }

            if (candidates.Count == 0)
                continue;

            float totalWeight = 0;
            foreach (var c in candidates)
                totalWeight += c.Weight;

            float roll = SysRandom.Value * totalWeight;
            float accum = 0;
            CityDevSearchConfig selected = null;
            foreach (var c in candidates)
            {
                accum += c.Weight;
                if (roll < accum)
                {
                    selected = c;
                    break;
                }
            }

            if (selected == null)
                continue;

            if (selected.ResType == "findhero" || selected.ResType == "findherostar")
            {
                bool starHero = selected.ResType == "findherostar";
                if (starHero)
                    searchResultType = System.Math.Max(searchResultType, 4);
                else
                    searchResultType = System.Math.Max(searchResultType, 3);
                var newHero = FindUndiscoveredHero(cityId, starHero);
                if (newHero != null)
                {
                    discoveredHeroIds.Add(newHero.heroId);
                    string heroName = HeroConfig.GetConfig(newHero.heroId).Name;
                    attrDatas.Add(new PopResultPanelManager.AttrData()
                    {
                        attrStr = HeroConfig.GetConfig(heroId).Name,
                        valStr = $"发现<color=green>{heroName}</color>",
                    });
                }
            }
            else if (selected.ResType == "cityattr" || selected.ResType == "forceattr")
            {
                var attrCfg = CityAttrConfig.GetConfig(selected.ResId);
                int amount = SysRandom.Range(selected.AttrValMin, selected.AttrValMax + 1);
                string attrName = attrCfg.name;
                totalResourceAmount += amount;

                if (selected.ResType == "cityattr")
                {
                    searchResultType = System.Math.Max(searchResultType, 1);
                    var heroCity = GameManager.Instance.GetCity(heroData.cityId);
                    if (heroCity == null)
                    {
                        GameLog.Error($"ExecuteCitySearch heroCity not found heroId={heroId} cityId={heroData.cityId}");
                    }
                    else
                    {
                        heroCity.AddAttr(attrName, amount, "走访发现");
                    }
                }
                else
                {
                    searchResultType = System.Math.Max(searchResultType, 2);
                    AddAttr(attrName, amount, "走访发现");
                }

                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = HeroConfig.GetConfig(heroId).Name,
                    valStr = $"<color=green>{attrCfg.Cname}+{amount}</color>",
                });
            }
        }

        cityData.AddAction(devId, heroCount);
        AddKingActionCount(devId, heroCount);
        MarkHeroesActed(heroIds);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionSearch(
            GameManager.Instance.SaveData.round, forceId, cityId, devId, heroIds, searchResultType, totalResourceAmount, discoveredHeroIds));

        GameLog.Info($"ExecuteCitySearch cityId={cityId} heroCount={heroCount}");
        return true;
    }

    /// <summary>
    /// 城市中是否存在未发现的在野武将（BornCity匹配、年龄达标、不在游戏中）
    /// </summary>
    private bool HasUndiscoveredHero(int cityId, bool starHero)
    {
        return FindUndiscoveredHeroConfig(cityId, starHero) != null;
    }

    /// <summary>
    /// 查找城市中未发现在野武将的HeroConfig（BornCity匹配、年龄达标、不在游戏中）
    /// </summary>
    private HeroConfig FindUndiscoveredHeroConfig(int cityId, bool starHero)
    {
        var cityConfig = WorldConfig.GetConfig(cityId);
        if (cityConfig == null)
        {
            GameLog.Error($"FindUndiscoveredHeroConfig city not found cityId={cityId}");
            return null;
        }

        string cityName = cityConfig.Cname;
        float currentYear = GameManager.Instance.GetCurrentYear();

        var existingHeroIds = new HashSet<int>();
        foreach (var h in GameManager.Instance.SaveData.heros)
            existingHeroIds.Add(h.heroId);

        foreach (var heroConfig in HeroConfig.ConfigList)
        {
            if (heroConfig.StarHero != starHero)
                continue;

            if (heroConfig.BornCity != cityName)
                continue;

            if (currentYear - heroConfig.BornYear < SystemConst.Game.BORN_AGE)
                continue;

            if (existingHeroIds.Contains(heroConfig.Id))
                continue;

            return heroConfig;
        }

        return null;
    }

    /// <summary>
    /// 查找城市中未发现的在野武将并创建（BornCity匹配、年龄达标、不在游戏中）
    /// </summary>
    private SaveHeroData FindUndiscoveredHero(int cityId, bool starHero)
    {
        var heroConfig = FindUndiscoveredHeroConfig(cityId, starHero);
        if (heroConfig == null)
        {
            GameLog.Info($"FindUndiscoveredHero 未发现匹配武将 cityId={cityId} starHero={starHero}");
            return null;
        }

        var newHero = SaveHeroData.CreateWildHero(heroConfig.Id, cityId);
        GameManager.Instance.SaveData.heros.Add(newHero);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateWild(
            GameManager.Instance.SaveData.round, forceId, cityId, heroConfig.Id));

        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData != null)
            cityData.RecalculateHeros();

        GameLog.Info($"FindUndiscoveredHero 发现武将 heroId={heroConfig.Id} name={heroConfig.Name} starHero={starHero} cityId={cityId}");
        return newHero;
    }

    private int CalculateRecruitRate(int cityId, int myHeroId, int targetHeroId)
    {
        var cityData = GameManager.Instance.GetCity(cityId);
        var hero = GameManager.Instance.GetHero(targetHeroId);

        // 己方在职武将不可登庸
        if (hero.state == HeroState.Normal && hero.forceId == cityData.forceId)
            return 0;

        if (hero.state == HeroState.Wild)
            return SysFormula.Hero.CalculateRecruitWildRate(cityId, myHeroId, targetHeroId);

        if (hero.state == HeroState.Catched || (hero.state == HeroState.Normal && hero.forceId != cityData.forceId))
            return SysFormula.Hero.CalculateRecruitEnemyRate(cityId, myHeroId, targetHeroId);

        return 0;
    }

    public bool ExecuteCityUseHero(int cityId, int devId, int[] myHeroIds, int[] targetHeroIds, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (myHeroIds == null || myHeroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCityUseHero myHeroIds 为空");
            return false;
        }

        var availableExecutors = FilterAvailableHeroes(myHeroIds);
        if (availableExecutors.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        myHeroIds = availableExecutors.ToArray();

        var cityData = GameManager.Instance.GetCity(cityId);
        List<int> remainingTargets = new List<int>(targetHeroIds);
        bool anySuccess = false;

        foreach (int myHeroId in myHeroIds)
        {
            if (remainingTargets.Count == 0) break;

            int bestTargetId = 0;
            int bestRate = -1;
            foreach (int targetId in remainingTargets)
            {
                int rate = CalculateRecruitRate(cityId, myHeroId, targetId);
                if (rate > bestRate)
                {
                    bestRate = rate;
                    bestTargetId = targetId;
                }
            }

            if (bestTargetId == 0 || bestRate <= 0) continue;

            int randomVal = SysRandom.Range(0, 100);
            bool success = randomVal < bestRate;

            string executorName = HeroConfig.GetConfig(myHeroId).Name;
            string targetName = HeroConfig.GetConfig(bestTargetId).Name;

            GameLog.Info($"{executorName}登庸{targetName} {(success ? "成功" : "失败")} {bestRate}%");

            if (success)
            {
                var hero = GameManager.Instance.GetHero(bestTargetId);
                int oldForceId = hero.forceId;
                hero.state = HeroState.Normal;
                hero.forceId = cityData.forceId;
                hero.loyalty = SystemConst.Hero.RECRUIT_SUCCESS_LOYALTY;
                MoveHeroToCity(hero.cityId, cityId, new int[] { bestTargetId });
                remainingTargets.Remove(bestTargetId);

                GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateRecruitSuccess(
                    GameManager.Instance.SaveData.round, forceId, oldForceId, cityId, bestTargetId));
                anySuccess = true;

                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr =  "登庸" + targetName,
                    valStr = "<color=green>成功</color>"+executorName,
                });
            }
            else
            {
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr =  "登庸" + targetName,
                    valStr = "<color=red>失败</color>"+executorName,
                });
            }
        }

        cityData.AddAction(devId, myHeroIds.Length);
        AddKingActionCount(devId, myHeroIds.Length);

        // 仅标记执行方（去登庸的武将），被登庸武将不标记。
        // dayDiff 按主公所在城市到目标武将所在城市的日程计算：distance - 1
        // 单目标，按目标武将所在城市归属判断本/他国家
        var kingCity = GetKingCity();
        int sourceCityId = kingCity != null ? kingCity.cityId : cityId;
        int targetHeroId = targetHeroIds.Length > 0 ? targetHeroIds[0] : 0;
        var targetHero = GameManager.Instance.GetHero(targetHeroId);
        int targetCityId = targetHero != null ? targetHero.cityId : cityId;
        int distance = SysFormula.City.CalculateRecruitDayDistance(sourceCityId, targetCityId, forceId);
        MarkHeroesActed(myHeroIds, distance - 1);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionRecruit(
            GameManager.Instance.SaveData.round, forceId, cityId, myHeroIds, targetHeroIds, anySuccess));

        return true;
    }

    public bool ExecuteCityPraiseHero(int cityId, int devId, int[] heroList, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroList == null || heroList.Length == 0)
        {
            GameLog.Warn("ExecuteCityPraiseHero heroList 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroList);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroList = availableHeroes.ToArray();

        var devCfg = CityDevConfig.GetConfig(devId);

        // KingAction 人均黄金消耗扣除
        if (devCfg.GoldCost > 0)
        {
            int baseTotalCost = heroList.Length * devCfg.GoldCost;
            float costReduce = ForceTech.GetKingActionCostReduce(forceId, devId);
            int totalCost = ForceTech.ApplyCostReduce(baseTotalCost, costReduce);
            if (gold < totalCost)
            {
                SystemTip.Instance.ShowTip("黄金不足");
                return false;
            }
            int goldOld = (int)gold;
            if (totalCost > 0)
            {
                AddAttr("gold", -totalCost, devCfg.Cname + "扣除金钱");
            }
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = -totalCost,
            });
        }

        // KingAction 每回合参与人数上限
        int effectiveCount = ForceTech.GetEffectiveSlotCount(forceId, devId);
        if (effectiveCount > 0)
        {
            int usedCount = GetKingActionCount(devId);
            if (usedCount + heroList.Length > effectiveCount)
            {
                SystemTip.Instance.ShowTip($"本回合{devCfg.Cname}已达上限");
                return false;
            }
        }

        // 根据 devId 推导 methodId：21605=奖赏(methodId=2)，其余=褒奖(methodId=1)
        int methodId = (devId == CityDevConfig.GetConfigByName("Reward").Id) ? 2 : 1;
        var kingCfg = CityDevKingActionConfig.GetConfig(devId);
        int totalLoyaltyAdd = 0;

        foreach(var heroId in heroList)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            int loyaltyOld = hero.loyalty;
            int loyaltyAdd = SysRandom.Range(kingCfg.EffectMin, kingCfg.EffectMax + 1);
            // 科技加成：褒奖/奖赏效果提升
            float amountMul = ForceTech.GetKingActionAmountMul(forceId, devId, "loyalty");
            loyaltyAdd = (int)ForceTech.ApplyAmountMul(loyaltyAdd, amountMul);

            hero.loyalty = System.Math.Min(SystemConst.Hero.MAX_LOYALTY, hero.loyalty + loyaltyAdd);
            int actualAdd = hero.loyalty - loyaltyOld;
            totalLoyaltyAdd += actualAdd;

            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = HeroConfig.GetConfig(heroId).Name + "忠心",
                valOld = loyaltyOld,
                valAddon = actualAdd,
            });
        }

        AddKingActionCount(devId, heroList.Length);

        MarkHeroesActed(heroList);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionPraise(
            GameManager.Instance.SaveData.round, forceId, cityId, devId, heroList, methodId, totalLoyaltyAdd));

        return true;
    }

    /// <summary>
    /// 执行科技研究 KingAction：消耗研究值(scipoint)，派遣武将提升科技研究进度。
    /// 占用3个时间周期，不消耗黄金，最多派遣1名武将。
    /// 当研究进度达到 TechConfig.SciPointCost 时自动解锁科技。
    /// </summary>
    public bool ExecuteCityTech(int cityId, int devId, int[] heroIds, int techId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCityTech heroIds 为空");
            return false;
        }

        if (techId == 0)
        {
            GameLog.Warn("ExecuteCityTech techId 无效");
            return false;
        }

        // 已解锁的科技无需再研究
        if (HasTech(techId))
        {
            SystemTip.Instance.ShowTip("该科技已解锁");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        // 研究值消耗
        int scipointCost = SystemConst.CityDev.TECH_RESEARCH_SCIPOINT_COST;
        if (scipoint < scipointCost)
        {
            SystemTip.Instance.ShowTip("研究值不足");
            return false;
        }
        float scipointOld = scipoint;
        AddAttr("scipoint", -scipointCost, "科技研究扣除研究值");

        var techCfg = TechConfig.GetConfig(techId);
        var kingCfg = CityDevKingActionConfig.GetConfig(devId);
        int progressOld = GetTechProgress(techId);
        int totalResearchAdd = heroIds.Length * kingCfg.EffectMin;
        AddTechProgress(techId, totalResearchAdd);
        int progressNew = GetTechProgress(techId);

        // 判断是否达到研究阈值
        bool unlocked = progressNew >= techCfg.SciPointCost;
        if (unlocked)
        {
            UnlockTech(techId);
        }

        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "Scipoint",
            valOld = (int)scipointOld,
            valAddon = -scipointCost,
        });

        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attrStr = techCfg.Cname + "研究值",
            valOld = progressOld,
            valAddon = totalResearchAdd,
        });

        if (unlocked)
        {
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = techCfg.Cname + "已解锁",
                valStr = "研究完成",
            });
        }

        AddKingActionCount(devId, heroIds.Length);
        // 占用3个时间周期：dayDiff = TECH_RESEARCH_TIME_PERIODS - 1
        MarkHeroesActed(heroIds, SystemConst.CityDev.TECH_RESEARCH_TIME_PERIODS - 1);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionTech(
            GameManager.Instance.SaveData.round, forceId, cityId, devId, heroIds, techId, totalResearchAdd, unlocked));

        GameLog.Info($"ExecuteCityTech forceId={forceId} techId={techId} heroCount={heroIds.Length} scipointCost={scipointCost} progressOld={progressOld} progressNew={progressNew} unlocked={unlocked}");

        return true;
    }

    /// <summary>
    /// 破坏行动：派遣 heroIds 武将各执行一次破坏，每人降低目标城市 5-10 城防
    /// </summary>
    public bool ExecuteCityDestroy(int cityId, int devId, int[] heroIds, int targetCityId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCityDestroy heroIds 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        if (targetCityId <= 0)
        {
            GameLog.Warn("ExecuteCityDestroy targetCityId 无效");
            return false;
        }

        var targetCity = GameManager.Instance.GetCity(targetCityId);
        if (targetCity == null)
        {
            GameLog.Error($"ExecuteCityDestroy 目标城市不存在 targetCityId={targetCityId}");
            return false;
        }
        if (targetCity.forceId == forceId)
        {
            SystemTip.Instance.ShowTip("不能破坏己方城市");
            return false;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        int goldCost = devCfg.GoldCost;
        int heroCount = heroIds.Length;
        int baseTotalCost = heroCount * goldCost;
        float costReduce = ForceTech.GetKingActionCostReduce(forceId, devId);
        int totalCost = ForceTech.ApplyCostReduce(baseTotalCost, costReduce);
        if (gold < totalCost)
        {
            SystemTip.Instance.ShowTip("黄金不足");
            return false;
        }

        int goldOld = (int)gold;
        if (totalCost > 0)
        {
            AddAttr("gold", -totalCost, devCfg.Cname + "扣除金钱");
        }
        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "Gold",
            valOld = goldOld,
            valAddon = -totalCost,
        });

        int wallOld = (int)targetCity.GetAttr("wall");
        int totalWallReduce = 0;
        int targetForceId = targetCity.forceId;
        var kingCfg = CityDevKingActionConfig.GetConfig(devId);
        // 科技加成：破坏效果提升
        float destroyAmountMul = ForceTech.GetKingActionAmountMul(forceId, devId, "wall");
        foreach (var heroId in heroIds)
        {
            string executorName = HeroConfig.GetConfig(heroId).Name;
            int rate = SysFormula.Hero.CalcKingActionBonus(heroId, targetForceId, devId, null);
            int randomVal = SysRandom.Range(0, 100);
            bool success = randomVal < rate;

            if (success)
            {
                int wallReduce = SysRandom.Range(kingCfg.EffectMin, kingCfg.EffectMax + 1);
                wallReduce = (int)ForceTech.ApplyAmountMul(wallReduce, destroyAmountMul);
                targetCity.AddAttr("wall", -wallReduce, devCfg.Cname + "破坏城防");
                totalWallReduce += wallReduce;
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "破坏",
                    valStr = $"<color=green>成功</color>{rate}%",
                });
            }
            else
            {
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "破坏",
                    valStr = $"<color=red>失败</color>{rate}%",
                });
            }
        }

        string targetCityName = WorldConfig.GetConfig(targetCityId)?.Cname ?? "";
        if (totalWallReduce > 0)
        {
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = targetCityName + "城防",
                valOld = wallOld,
                valAddon = -totalWallReduce,
            });
        }

        AddKingActionCount(devId, heroCount);
        // dayDiff 按主公所在城市到目标城市的日程计算：distance - 1（目标必为敌方，crossCountry=true）
        var kingCity = GetKingCity();
        int sourceCityId = kingCity != null ? kingCity.cityId : cityId;
        int distance = SysFormula.City.CalculateMoveDayDistance(sourceCityId, targetCityId, forceId);
        MarkHeroesActed(heroIds, distance - 1);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionDestroy(
            GameManager.Instance.SaveData.round, forceId, cityId, targetCityId, devId, heroIds, totalWallReduce));

        PanelManager.Instance.SendSignal(new CityAttrChangeSignal { CityId = targetCityId });
        GameLog.Info($"ExecuteCityDestroy forceId={forceId} targetCityId={targetCityId} heroCount={heroCount} totalWallReduce={totalWallReduce}");
        return true;
    }

    /// <summary>
    /// 扰乱行动：派遣 heroIds 武将各执行一次扰乱，每人降低目标城市 3-5 民心，
    /// 并独立随机选择最多 DISTURB_LOYALTY_TARGET_MAX 个敌方武将降低 3-5 忠心
    /// </summary>
    public bool ExecuteCityDisturb(int cityId, int devId, int[] heroIds, int targetCityId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCityDisturb heroIds 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        if (targetCityId <= 0)
        {
            GameLog.Warn("ExecuteCityDisturb targetCityId 无效");
            return false;
        }

        var targetCity = GameManager.Instance.GetCity(targetCityId);
        if (targetCity == null)
        {
            GameLog.Error($"ExecuteCityDisturb 目标城市不存在 targetCityId={targetCityId}");
            return false;
        }
        if (targetCity.forceId == forceId)
        {
            SystemTip.Instance.ShowTip("不能扰乱己方城市");
            return false;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        int goldCost = devCfg.GoldCost;
        int heroCount = heroIds.Length;
        int baseTotalCost = heroCount * goldCost;
        float costReduce = ForceTech.GetKingActionCostReduce(forceId, devId);
        int totalCost = ForceTech.ApplyCostReduce(baseTotalCost, costReduce);
        if (gold < totalCost)
        {
            SystemTip.Instance.ShowTip("黄金不足");
            return false;
        }

        int goldOld = (int)gold;
        if (totalCost > 0)
        {
            AddAttr("gold", -totalCost, devCfg.Cname + "扣除金钱");
        }
        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "Gold",
            valOld = goldOld,
            valAddon = -totalCost,
        });

        int happyOld = (int)targetCity.GetAttr("happy");
        int totalHappyReduce = 0;
        int targetForceId = targetCity.forceId;
        // 记录每个目标武将累计被降低的忠心
        var heroLoyaltyReduceMap = new Dictionary<int, int>();
        int totalLoyaltyReduce = 0;
        var kingCfg = CityDevKingActionConfig.GetConfig(devId);
        // 科技加成：扰乱效果提升
        float disturbAmountMul = ForceTech.GetKingActionAmountMul(forceId, devId, "happy");
        foreach (var heroId in heroIds)
        {
            string executorName = HeroConfig.GetConfig(heroId).Name;
            int rate = SysFormula.Hero.CalcKingActionBonus(heroId, targetForceId, devId, null);
            int randomVal = SysRandom.Range(0, 100);
            bool success = randomVal < rate;

            if (success)
            {
                int happyReduce = SysRandom.Range(kingCfg.EffectMin, kingCfg.EffectMax + 1);
                happyReduce = (int)ForceTech.ApplyAmountMul(happyReduce, disturbAmountMul);
                targetCity.AddAttr("happy", -happyReduce, devCfg.Cname + "扰乱民心");
                totalHappyReduce += happyReduce;

                // 扰乱目标城市武将忠心：成功时随机选择最多 DISTURB_LOYALTY_TARGET_MAX 个武将
                var targetHeroIds = targetCity.GetNormalHeroList();
                int targetKingHeroId = ForceConfig.GetConfig(targetCity.forceId).HeroId;
                targetHeroIds = targetHeroIds.Where(id => id != targetKingHeroId).ToList();
                if (targetHeroIds.Count > 0)
                {
                    List<int> executorTargets = targetHeroIds;
                    if (targetHeroIds.Count > SystemConst.CityDev.DISTURB_LOYALTY_TARGET_MAX)
                    {
                        executorTargets = targetHeroIds
                            .OrderBy(x => SysRandom.Value)
                            .Take(SystemConst.CityDev.DISTURB_LOYALTY_TARGET_MAX)
                            .ToList();
                    }
                    foreach (var targetHeroId in executorTargets)
                    {
                        int reduce = SysRandom.Range(kingCfg.Effect2Min, kingCfg.Effect2Max + 1);
                        if (!heroLoyaltyReduceMap.ContainsKey(targetHeroId))
                            heroLoyaltyReduceMap[targetHeroId] = 0;
                        heroLoyaltyReduceMap[targetHeroId] += reduce;
                        totalLoyaltyReduce += reduce;
                    }
                }

                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "扰乱",
                    valStr = $"<color=green>成功</color>{rate}%",
                });
            }
            else
            {
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "扰乱",
                    valStr = $"<color=red>失败</color>{rate}%",
                });
            }
        }

        string targetCityName = WorldConfig.GetConfig(targetCityId)?.Cname ?? "";
        if (totalHappyReduce > 0)
        {
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = targetCityName + "民心",
                valOld = happyOld,
                valAddon = -totalHappyReduce,
            });
        }

        // 应用忠心变化并在结果中显示每个受影响武将
        if (heroLoyaltyReduceMap.Count > 0)
        {
            foreach (var pair in heroLoyaltyReduceMap)
            {
                var targetHero = GameManager.Instance.GetHero(pair.Key);
                if (targetHero == null) continue;
                int loyaltyOld = targetHero.loyalty;
                targetHero.loyalty = System.Math.Max(0, targetHero.loyalty - pair.Value);
                int actualReduce = loyaltyOld - targetHero.loyalty;

                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = $"<color=yellow>{HeroConfig.GetConfig(pair.Key).Name}</color>忠心",
                    valOld = loyaltyOld,
                    valAddon = -actualReduce,
                });

                // 记录被扰乱武将的忠心变化事件
                GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateLoyaltyChange(
                    GameManager.Instance.SaveData.round, targetHero.forceId, targetCityId, pair.Key, -actualReduce, 0));
            }
        }

        AddKingActionCount(devId, heroCount);
        // dayDiff 按主公所在城市到目标城市的日程计算：distance - 1（目标必为敌方，crossCountry=true）
        var kingCity = GetKingCity();
        int sourceCityId = kingCity != null ? kingCity.cityId : cityId;
        int distance = SysFormula.City.CalculateMoveDayDistance(sourceCityId, targetCityId, forceId);
        MarkHeroesActed(heroIds, distance - 1);

        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateKingActionDisturb(
            GameManager.Instance.SaveData.round, forceId, cityId, targetCityId, devId, heroIds, totalHappyReduce, totalLoyaltyReduce));

        PanelManager.Instance.SendSignal(new CityAttrChangeSignal { CityId = targetCityId });
        GameLog.Info($"ExecuteCityDisturb forceId={forceId} targetCityId={targetCityId} heroCount={heroCount} totalHappyReduce={totalHappyReduce} totalLoyaltyReduce={totalLoyaltyReduce}");
        return true;
    }

    /// <summary>
    /// 亲善行动：提升当前势力与目标势力的友好度
    /// </summary>
    public bool ExecuteCityBefriend(int cityId, int devId, int[] heroIds, int targetForceId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCityBefriend heroIds 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        if (targetForceId <= 0 || targetForceId == forceId)
        {
            SystemTip.Instance.ShowTip("目标势力无效");
            return false;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        int goldCost = devCfg.GoldCost;
        int heroCount = heroIds.Length;
        int totalCost = heroCount * goldCost;
        if (gold < totalCost)
        {
            SystemTip.Instance.ShowTip("黄金不足");
            return false;
        }

        int goldOld = (int)gold;
        AddAttr("gold", -totalCost, devCfg.Cname + "扣除金钱");

        int relationOld = GameManager.Instance.SaveData.forceRelation.GetRelation(forceId, targetForceId);
        int totalRelationChange = 0;
        var kingCfg = CityDevKingActionConfig.GetConfig(devId);
        foreach (var heroId in heroIds)
        {
            string executorName = HeroConfig.GetConfig(heroId).Name;
            int rate = SysFormula.Hero.CalcKingActionBonus(heroId, targetForceId, devId, null);
            int randomVal = SysRandom.Range(0, 100);
            bool success = randomVal < rate;

            if (success)
            {
                int change = SysRandom.Range(kingCfg.EffectMin, kingCfg.EffectMax + 1);
                totalRelationChange += change;
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "亲善",
                    valStr = $"<color=green>成功</color>{rate}%",
                });
            }
            else
            {
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "亲善",
                    valStr = $"<color=red>失败</color>{rate}%",
                });
            }
        }

        if (totalRelationChange > 0)
        {
            GameManager.Instance.SaveData.forceRelation.AddRelation(forceId, targetForceId, totalRelationChange);
        }
        int relationNew = GameManager.Instance.SaveData.forceRelation.GetRelation(forceId, targetForceId);

        string targetForceName = ForceConfig.GetConfig(targetForceId).Cname;
        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "Gold",
            valOld = goldOld,
            valAddon = -totalCost,
        });
        if (totalRelationChange > 0)
        {
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = $"与{targetForceName}友好度",
                valOld = relationOld,
                valAddon = relationNew - relationOld,
            });
        }

        var cityData = GameManager.Instance.GetCity(cityId);
        cityData.AddAction(devId, heroCount);
        AddKingActionCount(devId, heroCount);
        MarkHeroesActed(heroIds);

        GameLog.Info($"ExecuteCityBefriend forceId={forceId} targetForceId={targetForceId} heroCount={heroCount} relationChange={relationNew - relationOld}");
        return true;
    }

    /// <summary>
    /// 调拨行动：降低两个目标势力间的友好度
    /// </summary>
    public bool ExecuteCitySowDiscord(int cityId, int devId, int[] heroIds, int targetForceId1, int targetForceId2, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        if (heroIds == null || heroIds.Length == 0)
        {
            GameLog.Warn("ExecuteCitySowDiscord heroIds 为空");
            return false;
        }

        var availableHeroes = FilterAvailableHeroes(heroIds);
        if (availableHeroes.Count == 0)
        {
            SystemTip.Instance.ShowTip("所选武将本回合已行动");
            return false;
        }
        heroIds = availableHeroes.ToArray();

        if (targetForceId1 <= 0 || targetForceId2 <= 0 || targetForceId1 == targetForceId2)
        {
            SystemTip.Instance.ShowTip("目标势力无效");
            return false;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        int goldCost = devCfg.GoldCost;
        int heroCount = heroIds.Length;
        int totalCost = heroCount * goldCost;
        if (gold < totalCost)
        {
            SystemTip.Instance.ShowTip("黄金不足");
            return false;
        }

        int goldOld = (int)gold;
        AddAttr("gold", -totalCost, devCfg.Cname + "扣除金钱");

        int relationOld = GameManager.Instance.SaveData.forceRelation.GetRelation(targetForceId1, targetForceId2);
        int totalRelationChange = 0;
        var kingCfg = CityDevKingActionConfig.GetConfig(devId);
        foreach (var heroId in heroIds)
        {
            string executorName = HeroConfig.GetConfig(heroId).Name;
            int rate = SysFormula.Hero.CalcKingActionBonus(heroId, targetForceId1, devId, null);
            int randomVal = SysRandom.Range(0, 100);
            bool success = randomVal < rate;

            if (success)
            {
                totalRelationChange += SysRandom.Range(kingCfg.EffectMin, kingCfg.EffectMax + 1);
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "挑拨",
                    valStr = $"<color=green>成功</color>{rate}%",
                });
            }
            else
            {
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = executorName + "挑拨",
                    valStr = $"<color=red>失败</color>{rate}%",
                });
            }
        }

        if (totalRelationChange > 0)
        {
            GameManager.Instance.SaveData.forceRelation.AddRelation(targetForceId1, targetForceId2, -totalRelationChange);
        }
        int relationNew = GameManager.Instance.SaveData.forceRelation.GetRelation(targetForceId1, targetForceId2);

        string targetForce1Name = ForceConfig.GetConfig(targetForceId1).Cname;
        string targetForce2Name = ForceConfig.GetConfig(targetForceId2).Cname;
        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "Gold",
            valOld = goldOld,
            valAddon = -totalCost,
        });
        if (totalRelationChange > 0)
        {
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = $"{targetForce1Name}与{targetForce2Name}友好度",
                valOld = relationOld,
                valAddon = relationNew - relationOld,
            });
        }

        var cityData = GameManager.Instance.GetCity(cityId);
        cityData.AddAction(devId, heroCount);
        AddKingActionCount(devId, heroCount);
        MarkHeroesActed(heroIds);

        GameLog.Info($"ExecuteCitySowDiscord forceId={forceId} target1={targetForceId1} target2={targetForceId2} heroCount={heroCount} relationChange={relationNew - relationOld}");
        return true;
    }

    public Dictionary<string, float> CalculateForceAttrAddons()
    {
        Dictionary<string, float> attrAddons = new Dictionary<string, float>();
        
        var cities = GetCityList();
        GameLog.Debug($"CalculateForceAttrAddons forceId={forceId} cities={cities.Count}");
        
        foreach (var city in cities)
        {
            var assignments = city.GetDevAssignments();
            
            foreach (var assignment in assignments)
            {
                var devCfg = CityDevConfig.GetConfig(assignment.devId);
                var heroData = GameManager.Instance.GetHero(assignment.heroId);
                
                CalculateForceAttrAddonsForAssignment(devCfg, heroData, city, attrAddons);
            }
        }
        
        return attrAddons;
    }

    private void CalculateForceAttrAddonsForAssignment(CityDevConfig devCfg, SaveHeroData heroData, SaveCityData cityData, Dictionary<string, float> attrAddons)
    {
        float avgWeightedValue = SysFormula.City.GetHeroWeightedAttrValue(heroData, devCfg.Attrs);
        int tier = SysFormula.City.GetHeroTier(avgWeightedValue);
        float multiplier = cityData.GetProductionMultiplier();

        if (!string.IsNullOrEmpty(devCfg.DevAttr1))
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr1.ToLower());
            if (attrConfig.IsForceAttr)
            {
                float addon = CalculateForceDevAddonByTier(devCfg.DevAttr1, devCfg.DevAttr1Value[tier]);
                if (addon > 0)
                {
                    if (SaveCityData.CityHasResAddon(cityData.cityId, devCfg.DevAttr1))
                        addon += SystemConst.City.RES_ADDON_BONUS;
                    addon = ApplyProductionMultiplierToAddon(devCfg.DevAttr1.ToLower(), addon, multiplier);
                    string attrName = devCfg.DevAttr1.ToLower();
                    if (!attrAddons.ContainsKey(attrName))
                        attrAddons[attrName] = 0;
                    attrAddons[attrName] += addon;
                }
            }
        }
        
        if (!string.IsNullOrEmpty(devCfg.DevAttr2) && devCfg.DevAttr2Value != null && devCfg.DevAttr2Value.Length >= 4)
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr2.ToLower());
            if (attrConfig.IsForceAttr)
            {
                float addon = CalculateForceDevAddonByTier(devCfg.DevAttr2, devCfg.DevAttr2Value[tier]);
                if (addon > 0)
                {
                    if (SaveCityData.CityHasResAddon(cityData.cityId, devCfg.DevAttr2))
                        addon += SystemConst.City.RES_ADDON_BONUS;
                    addon = ApplyProductionMultiplierToAddon(devCfg.DevAttr2.ToLower(), addon, multiplier);
                    string attrName = devCfg.DevAttr2.ToLower();
                    if (!attrAddons.ContainsKey(attrName))
                        attrAddons[attrName] = 0;
                    attrAddons[attrName] += addon;
                }
            }
        }

        if (devCfg.GoldCost > 0)
        {
            string goldAttrName = "gold";
            if (!attrAddons.ContainsKey(goldAttrName))
                attrAddons[goldAttrName] = 0;
            attrAddons[goldAttrName] -= devCfg.GoldCost;
        }
    }

    private static float ApplyProductionMultiplierToAddon(string attrName, float addon, float multiplier)
    {
        if (multiplier >= 0.999f && multiplier <= 1.001f)
            return addon;
        if (attrName == "gold" || attrName == "food" || attrName == "soldier")
            return addon * multiplier;
        return addon;
    }

    private float CalculateForceDevAddonByTier(string attrName, float tierValue)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(attrName.ToLower());
        
        float currentVal = GetAttr(attrName);
        int valMax = attrConfig.ValMaxForce;
        
        float addon = tierValue;
        float remaining = valMax - currentVal;
        
        if (addon > remaining)
            addon = Math.Max(0, remaining);
        
        return addon;
    }

    public float GetPredictedGoldBalance()
    {
        var addons = CalculateForceAttrAddons();
        addons.TryGetValue("gold", out float goldAddon);
        return gold + goldAddon;
    }

    /// <summary>
    /// 判断是否已解锁指定科技
    /// </summary>
    public bool HasTech(int techId)
    {
        return unlockedTechIds != null && unlockedTechIds.Contains(techId);
    }

    /// <summary>
    /// 解锁科技
    /// </summary>
    public void UnlockTech(int techId)
    {
        if (unlockedTechIds == null)
            unlockedTechIds = new List<int>();
        if (!unlockedTechIds.Contains(techId))
        {
            unlockedTechIds.Add(techId);
            GameLog.Info($"Force {forceId} unlocked tech {techId}");
        }
    }
}
