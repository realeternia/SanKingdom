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

    public void AddAttr(string type, int add)
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
        
        switch (type.ToLower())
        {
            case "gold":
                gold = Math.Min(gold + add, attrConfig.ValMaxForce);
                break;
            default:
                break;
        }
        if (isPlayer && PanelManager.Instance != null)
        {
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = type.ToLower(), Value = GetAttr(type.ToLower()), Used = GetResUsed(type.ToLower()) });
        }
    }

    public int GetAttr(string type)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(type.ToLower());
        if (!attrConfig.IsForceAttr)
            return 0;
        
        if (attrConfig.IsPosRes)
        {
            return (int)Math.Floor(GetPosResFromCache(type.ToLower()));
        }
        
        switch (type.ToLower())
        {
            case "gold":
                return (int)Math.Floor(gold);
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
                PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = kvp.Key, Value = (int)Math.Floor(kvp.Value), Used = GetResUsed(kvp.Key) });
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
        
        int horseAvailable = GetAttr("horse") - GetResUsed("horse");
        int steelAvailable = GetAttr("steel") - GetResUsed("steel");
        int woodAvailable = GetAttr("wood") - GetResUsed("wood");
        int stoneAvailable = GetAttr("stone") - GetResUsed("stone");
        
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
            int currentVal = attrConfig.IsForceAttr ? GetAttr(mainAttr) : cityData.GetAttr(mainAttr);
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
            AddAttr("gold", -devConfig.GoldCost * heroList.Length);
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
                resultTmp[0] += val;
            }

            if (!string.IsNullOrEmpty(devConfig.DevAttr2) && devConfig.DevAttr2Value != null && devConfig.DevAttr2Value.Length > tier)
            {
                resultTmp.Add(0);
                resultTmp[1] += GetValByTier(devConfig.DevAttr2, devConfig.DevAttr2Value[tier], cityData.GetAttr(devConfig.DevAttr2));
            }

        }

        List<int> results = new List<int>();
        for (int i = 0; i < resultTmp.Count; i++)
        {
            results.Add((int)Math.Floor(resultTmp[i]));
        }

        ApplyProductionMultiplier(cityData, devConfig, results);
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr1))
        {
            var attr1Config = CityAttrConfig.GetConfigByname(devConfig.DevAttr1.ToLower());
            int attr1Old = attr1Config.IsForceAttr ? GetAttr(devConfig.DevAttr1) : cityData.GetAttr(devConfig.DevAttr1);
            if (!attr1Config.IsPosRes)
            {
                if (attr1Config.IsForceAttr)
                    AddAttr(devConfig.DevAttr1, results[0]);
                else
                    cityData.AddAttr(devConfig.DevAttr1, results[0]);
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
                int attr2Old = attr2Config.IsForceAttr ? GetAttr(devConfig.DevAttr2) : cityData.GetAttr(devConfig.DevAttr2);
                if (attr2Config.IsForceAttr)
                    AddAttr(devConfig.DevAttr2, results[1]);
                else
                    cityData.AddAttr(devConfig.DevAttr2, results[1]);
                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attr = devConfig.DevAttr2,
                    valOld = attr2Old,
                    valAddon = results[1],
                });
            }
        }

        cityData.AddAction(devId, heroList.Length);

        if (devConfig.ActionName == "find")
        {
            CheckFindAction(cityId, cityData, attrDatas);
        }

        return true;
    }

    private static void CheckFindAction(int cityId, SaveCityData cityData, List<PopResultPanelManager.AttrData> attrDatas)
    {
        Dictionary<string, int> cityNameToIdMap = new Dictionary<string, int>();
        foreach (var cityConfig in WorldConfig.ConfigList)
        {
            cityNameToIdMap[cityConfig.Cname] = cityConfig.Id;
        }

        foreach (var heroConfig in HeroConfig.ConfigList)
        {
            int bornCityId;
            if (!cityNameToIdMap.TryGetValue(heroConfig.BornCity, out bornCityId))
                continue;

            if (bornCityId != cityId)
                continue;
            
            float currentYear = GameManager.Instance.GetCurrentYear();
            if (currentYear - heroConfig.BornYear < SystemConst.Game.BORN_AGE)
                continue;

            bool isHeroInGame = false;
            foreach (var existingHero in GameManager.Instance.SaveData.heros)
            {
                if (existingHero.heroId == heroConfig.Id)
                {
                    isHeroInGame = true;
                    break;
                }
            }

            if (!isHeroInGame)
            {
                SaveHeroData newHero = SaveHeroData.CreateWildHero(heroConfig.Id, cityId);

                GameManager.Instance.SaveData.heros.Add(newHero);

                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = "发现",
                    valStr = string.Format("<color=green>{0}</color>", heroConfig.Name),
                });

                cityData.RecalculateHeros();

                break;
            }
        }
    }

    private static void ApplyProductionMultiplier(SaveCityData cityData, CityDevConfig devConfig, List<int> results)
    {
        float multiplier = cityData.GetProductionMultiplier();
        if (multiplier >= 0.999f && multiplier <= 1.001f)
            return;

        int index = 0;
        if (!string.IsNullOrEmpty(devConfig.DevAttr1))
        {
            string attr1 = devConfig.DevAttr1.ToLower();
            if (attr1 == "food" || attr1 == "soldier" || attr1 == "gold")
                results[index] = (int)(results[index] * multiplier);
            index++;
        }
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            string attr2 = devConfig.DevAttr2.ToLower();
            if (attr2 == "food" || attr2 == "soldier" || attr2 == "gold")
                results[index] = (int)(results[index] * multiplier);
        }
    }

    private static float GetValByTier(string resName, float tierValue, int nowVal)
    {
        var cityAttrConfig = CityAttrConfig.GetConfigByname(resName.ToLower());
        int valMax = cityAttrConfig.IsForceAttr ? cityAttrConfig.ValMaxForce : cityAttrConfig.ValMaxCity;
        
        float addon = tierValue;
        int remaining = valMax - nowVal;
        
        if (addon > remaining)
            addon = Math.Max(0, remaining);
        
        return addon;
    }

    public void ExecuteBattle(List<int> srcCityIds, List<SaveTroopsData> attackTroops, Dictionary<int, int> attackSoldierMap, int targetCityId, bool isAI)
    {
        var cityDest = GameManager.Instance.GetCity(targetCityId);

        if (isAI)
            BattleManager.Instance.SetMode(true, false);
        else
            BattleManager.Instance.SetMode(false, true);

        foreach (var srcCityId in srcCityIds)
        {
            var citySrc = GameManager.Instance.GetCity(srcCityId);
            citySrc.OnBattle();
            int totalSoldiers = attackTroops
                .Where(t => t.heroId1 > 0 && GameManager.Instance.GetHero(t.heroId1).cityId == srcCityId)
                .Sum(t => attackSoldierMap.ContainsKey(t.heroId1) ? attackSoldierMap[t.heroId1] : 0);
            GameLog.Info($"ExecuteBattle 扣除士兵和粮食 cityId={srcCityId} totalSoldiers={totalSoldiers}");
            citySrc.AddAttr("soldier", -totalSoldiers);
            citySrc.AddAttr("food", -totalSoldiers);
        }
        cityDest.OnBattle();
        int srcForceId = forceId;
        int destForceId = cityDest.forceId;
        GameManager.Instance.SaveData.forceRelation.RecordBattle(srcForceId, destForceId);
        var (defenceTroops, defenceSoldierMap) = TroopsBuilder.BuildDefenceTroops(cityDest);
        
        BattleManager.Instance.BattleBegin(this, cityDest.GetForce(), attackTroops, defenceTroops, attackSoldierMap, defenceSoldierMap, targetCityId,
            (result, attackerSoldierCount, defenderSoldierCount) => OnBattleEnd(result, attackerSoldierCount, defenderSoldierCount, srcCityIds, targetCityId, srcForceId, destForceId));
    }

    private void OnBattleEnd(BattleResult result, Dictionary<int, int> attackerSoldierCount, Dictionary<int, int> defenderSoldierCount, List<int> srcCityIds, int targetCityId, int srcForceId, int destForceId)
    {
        var destCity = GameManager.Instance.GetCity(targetCityId);
        GameLog.Info($"OnBattleEnd result={result} attackerCount={attackerSoldierCount.Count} defenderCount={defenderSoldierCount.Count}");

        if (result == BattleResult.Win)
        {
            var attackHeroList = attackerSoldierCount.Keys.ToList();
            var defenceHeroList = defenderSoldierCount.Keys.ToList();
            
            destCity.Occupy(forceId, attackHeroList, destForceId, defenceHeroList);
        }

        destCity.AddAttr("wall", -10);
        GameLog.Info($"OnBattleEnd 防守城市城墙减少10 wall={destCity.GetAttr("wall")}");
        
        if (result == BattleResult.Win)
        {
            destCity.AddAttr("happy", -30);
            GameLog.Info($"OnBattleEnd 攻方胜利，城市民心减少30 happy={destCity.GetAttr("happy")}");
            
            destCity.MultiplyAttr("food", 0.5f);
            GameLog.Info($"OnBattleEnd 攻方胜利，城市粮食减少50% food={destCity.GetAttr("food")}");
        }

        foreach (var kvp in attackerSoldierCount)
        {
            if (kvp.Value > 0)
            {
                var troop = SaveTroopsData.FindByHeroId(kvp.Key);
                if (troop != null)
                {
                    var hero = GameManager.Instance.GetHero(kvp.Key);
                    if (hero != null && hero.state == HeroState.Normal)
                    {
                        var heroCity = GameManager.Instance.GetCity(hero.cityId);
                        if (heroCity != null)
                        {
                            heroCity.AddAttr("soldier", kvp.Value);
                            GameLog.Info($"OnBattleEnd 攻击方退回士兵 heroId={kvp.Key} soldier={kvp.Value} cityId={hero.cityId}");
                        }
                    }
                }
            }
        }

        foreach (var kvp in defenderSoldierCount)
        {
            if (kvp.Value > 0)
            {
                var troop = SaveTroopsData.FindByHeroId(kvp.Key);
                if (troop != null)
                {
                    var hero = GameManager.Instance.GetHero(kvp.Key);
                    if (hero != null && hero.state == HeroState.Normal)
                    {
                        var heroCity = GameManager.Instance.GetCity(hero.cityId);
                        if (heroCity != null)
                        {
                            heroCity.AddAttr("soldier", kvp.Value);
                            GameLog.Info($"OnBattleEnd 防守方退回士兵 heroId={kvp.Key} soldier={kvp.Value} cityId={hero.cityId}");
                        }
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

    public void MoveHeroToCity(int srcCityId, int destCityId, int[] heroIds)
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
         
         PanelManager.Instance.SendSignal(new CityAttrChangeSignal { CityId = destCityId });
    }

    public void ExecuteCityMoveDev(int cityId, int devId, int[] heroList, int foodUse, int targetCityId)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);

        citySrc.AddAttr("food", -foodUse);
        var cityDest = GameManager.Instance.GetCity(targetCityId);
        if (cityDest != null && cityDest.forceId == citySrc.forceId)
        {
            cityDest.AddAttr("food", foodUse);
        }

        MoveHeroToCity(cityId, targetCityId, heroList);
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

    public bool ExecuteCityChange(int cityId, int devId, int[] heroList, bool isBuying, int amount, float rate, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();
        
        var cityData = GameManager.Instance.GetCity(cityId);

        if(isBuying)
        {
            if(gold < amount)
            {
                SystemTip.Instance.ShowTip("黄金不足");
                return false;
            }

            int goldOld = (int)gold;
            AddAttr("gold", -amount);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = - amount,
            });

            int foodOld = (int)cityData.food;
            int foodAdd = SysFormula.Economy.CalculateExchangeResult(amount, true);
            cityData.AddAttr("food", foodAdd);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Food",
                valOld = foodOld,
                valAddon = foodAdd,
            });
        }
        else
        {
            if(cityData.food < amount)
            {
                SystemTip.Instance.ShowTip("粮食不足");
                return false;
            }

            int foodOld = (int)cityData.food;
            cityData.AddAttr("food", -amount);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Food",
                valOld = foodOld,
                valAddon = -amount,
            });

            int goldOld = (int)gold;
            int goldAdd = SysFormula.Economy.CalculateExchangeResult(amount, false);
            AddAttr("gold", goldAdd);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = goldAdd,
            });
        }

        cityData.AddAction(devId, heroList.Length);

        return true;
    }

    private int CalculateRecruitRate(int cityId, int myHeroId, int targetHeroId)
    {
        var cityData = GameManager.Instance.GetCity(cityId);
        var hero = GameManager.Instance.GetHero(targetHeroId);

        if (hero.state == HeroState.Normal && hero.forceId == cityData.forceId)
            return 0;

        int baseSuccessRate = 0;

        if (hero.state == HeroState.Wild)
        {
            baseSuccessRate = SystemConst.Hero.RECRUIT_WILD_BASE_RATE;
        }
        else if (hero.state == HeroState.Catched || (hero.state == HeroState.Normal && hero.forceId != cityData.forceId))
        {
            baseSuccessRate = SysFormula.Hero.CalculateRecruitCapturedRate(hero.loyalty);
        }

        if (myHeroId > 0)
        {
            var executorHero = GameManager.Instance.GetHero(myHeroId);
            if (executorHero != null)
            {
                int charm = executorHero.GetAttr("charm");
                bool isKing = myHeroId == ForceConfig.GetConfig(executorHero.forceId).HeroId;
                baseSuccessRate = SysFormula.Hero.ApplyCharmBonus(baseSuccessRate, charm, isKing);
            }
        }

        return baseSuccessRate;
    }

    public bool ExecuteCityUseHero(int cityId, int devId, int[] myHeroIds, int[] targetHeroIds, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();

        var cityData = GameManager.Instance.GetCity(cityId);
        List<int> remainingTargets = new List<int>(targetHeroIds);

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
                hero.state = HeroState.Normal;
                hero.forceId = cityData.forceId;
                hero.loyalty = SystemConst.Hero.RECRUIT_SUCCESS_LOYALTY;
                MoveHeroToCity(hero.cityId, cityId, new int[] { bestTargetId });
                remainingTargets.Remove(bestTargetId);

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
        return true;
    }

    public bool ExecuteCityPraiseHero(int cityId, int devId, int[] heroList, int methodId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();
        
        var cityData = GameManager.Instance.GetCity(cityId);
        
        if(methodId == 2)
        {
            int totalCost = heroList.Length * SystemConst.Hero.PRAISE_GOLD_COST_PER_HERO;
            if(gold < totalCost)
            {
                SystemTip.Instance.ShowTip("黄金不足");
                return false;
            }
            int goldOld = (int)gold;
            AddAttr("gold", -totalCost);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = -totalCost,
            });
        }

        foreach(var heroId in heroList)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            int loyaltyOld = hero.loyalty;
            int loyaltyAdd = 0;
            
            if(methodId == 1)
            {
                loyaltyAdd = SysFormula.Hero.CalculatePraiseLoyaltyAdd();
            }
            else if(methodId == 2)
            {
                loyaltyAdd = SysFormula.Hero.CalculateRewardLoyaltyAdd();
            }
            
            hero.loyalty = System.Math.Min(SystemConst.Hero.MAX_LOYALTY, hero.loyalty + loyaltyAdd);
            
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = HeroConfig.GetConfig(heroId).Name + "忠心",
                valOld = loyaltyOld,
                valAddon = hero.loyalty - loyaltyOld,
            });
        }

        cityData.AddAction(devId, heroList.Length);
        
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

        if (!string.IsNullOrEmpty(devCfg.DevAttr1))
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr1.ToLower());
            if (attrConfig.IsForceAttr)
            {
                float addon = CalculateForceDevAddonByTier(devCfg.DevAttr1, devCfg.DevAttr1Value[tier]);
                if (addon > 0)
                {
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

    private float CalculateForceDevAddonByTier(string attrName, float tierValue)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(attrName.ToLower());
        
        int currentVal = GetAttr(attrName);
        int valMax = attrConfig.ValMaxForce;
        
        float addon = tierValue;
        int remaining = valMax - currentVal;
        
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
}
