using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using Controls.Utils;

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
            PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = type.ToLower(), Value = GetAttr(type.ToLower()) });
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
        
        if (isPlayer)
        {
            foreach (var kvp in posResCache)
            {
                PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = kvp.Key, Value = (int)Math.Floor(kvp.Value) });
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
        
        var heroes = GameManager.Instance.GetHerosByForce(forceId);
        foreach (var hero in heroes)
        {
            if (hero.state != HeroState.Normal)
                continue;
            if (hero.armsId <= 0)
                continue;
            
            var armsConfig = ArmsConfig.GetConfig(hero.armsId);
            resUsedCache["horse"] += armsConfig.HorseCost;
            resUsedCache["steel"] += armsConfig.SteelCost;
            resUsedCache["wood"] += armsConfig.WoodCost;
            resUsedCache["stone"] += armsConfig.StoneCost;
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

    public bool CanAffordArms(int armsId, int excludeHeroId = 0)
    {
        var armsConfig = ArmsConfig.GetConfig(armsId);
        
        int horseAvailable = GetAttr("horse") - GetResUsed("horse");
        int steelAvailable = GetAttr("steel") - GetResUsed("steel");
        int woodAvailable = GetAttr("wood") - GetResUsed("wood");
        int stoneAvailable = GetAttr("stone") - GetResUsed("stone");
        
        if (excludeHeroId > 0)
        {
            var excludeHero = GameManager.Instance.GetHero(excludeHeroId);
            if (excludeHero != null && excludeHero.armsId > 0)
            {
                var excludeArmsConfig = ArmsConfig.GetConfig(excludeHero.armsId);
                horseAvailable += excludeArmsConfig.HorseCost;
                steelAvailable += excludeArmsConfig.SteelCost;
                woodAvailable += excludeArmsConfig.WoodCost;
                stoneAvailable += excludeArmsConfig.StoneCost;
            }
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
            var forceCfg = ForceConfig.GetConfig(forceId);
            Color color;
            return ColorUtility.TryParseHtmlString(forceCfg.Color, out color) ? color : Color.white;
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
        GameLog.Info($"AddWarPlan forceId={warPlan.forceId} source={warPlan.sourceCityId} target={warPlan.targetCityId}");
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
            AI.AssignHeroesToDev(this);
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
        
        if (devConfig.Attrs.Length > 0)
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
            var val = GetValByTier(devConfig.DevAttr1, devConfig.DevAttr1Value[tier], cityData.GetAttr(devConfig.DevAttr1));
            resultTmp[0] += val;

            if (devConfig.DevAttr2Value != null && devConfig.DevAttr2Value.Length > tier)
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

    public void ExecuteCityBattleDev(int cityId, int devId, int[] heroList, int foodUse, int targetCityId, bool isAI, Dictionary<int, int> heroSoldierDict = null, Dictionary<int, int> heroArmsDict = null)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var cityDest = GameManager.Instance.GetCity(targetCityId);

        if (isAI)
            BattleManager.Instance.SetMode(true, false);
        else
            BattleManager.Instance.SetMode(false, true);

        var destForceData = GameManager.Instance.GetForce(cityDest.forceId);
        citySrc.AddAttr("food", -foodUse);
        int defenceFood = (int)cityDest.food;
        cityDest.food = 0;
        int srcForceId = citySrc.forceId;
        int destForceId = cityDest.forceId;
        var battleHeroListSrc = citySrc.GetBattleHeroList(heroList, heroSoldierDict, heroArmsDict);
        BattleManager.Instance.BattleBegin(citySrc.GetForce(), cityDest.GetForce(), battleHeroListSrc, cityDest.GetBattleHeroList(), foodUse, defenceFood, targetCityId,
            (result, soldierCount, foodCount) => OnBattleEnd(result, soldierCount, cityId, targetCityId, battleHeroListSrc.Select(x => x.CardId).ToArray(), srcForceId, destForceId));
    }

    private void OnBattleEnd(BattleResult result, Dictionary<int, int> soldierCount, int cityId, int targetCityId, int[] attackHeroList, int srcForceId, int destForceId)
    {
        var destCity = GameManager.Instance.GetCity(targetCityId);
        var srcCity = GameManager.Instance.GetCity(cityId);

        int srcRemaining = 0;
        int destRemaining = 0;
        foreach (var item in soldierCount)
        {
            var hero = GameManager.Instance.GetHero(item.Key);
            if (hero != null)
            {
                if (hero.forceId == srcForceId)
                    srcRemaining += item.Value;
                else if (hero.forceId == destForceId)
                    destRemaining += item.Value;
            }
        }
        srcCity.AddAttr("soldier", srcRemaining);
        destCity.AddAttr("soldier", destRemaining);

        if (result == BattleResult.Win)
        {
            destCity.Occupy(forceId, attackHeroList.ToList(), destForceId, destCity.GetBattleHeroList().Select(x => x.CardId).ToList());
            srcCity.RecalculateHeros();
        }
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

    public bool ExecuteCityUseHero(int cityId, int devId, int myHeroId, int targetHeroId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();
        
        var cityData = GameManager.Instance.GetCity(cityId);

        var hero = GameManager.Instance.GetHero(targetHeroId);
        if(hero.state == HeroState.Normal && hero.forceId == cityData.forceId)
        {
            SystemTip.Instance.ShowTip("该英雄已经是己方英雄");
            return false;
        }
        bool success = false;
        string resultMsg = "";

        int baseSuccessRate = 0;
        
        if(hero.state == HeroState.Wild)
        {
            baseSuccessRate = SysFormula.Hero.CalculateRecruitWildRate();
        }
        else if(hero.state == HeroState.Catched || (hero.state == HeroState.Normal && hero.forceId != cityData.forceId))
        {
            baseSuccessRate = SysFormula.Hero.CalculateRecruitCapturedRate(hero.loyalty);
        }

        if(myHeroId > 0)
        {
            var executorHero = GameManager.Instance.GetHero(myHeroId);
            if(executorHero != null)
            {
                int charm = executorHero.GetAttr("charm");
                bool isKing = myHeroId == ForceConfig.GetConfig(executorHero.forceId).HeroId;
                baseSuccessRate = SysFormula.Hero.ApplyCharmBonus(baseSuccessRate, charm, isKing);
            }
        }

        int randomVal = SysRandom.Range(0, 100);
        success = randomVal < baseSuccessRate;

        
        if(success)
        {
            hero.state = HeroState.Normal;
            hero.forceId = cityData.forceId;
            hero.loyalty = SystemConst.Hero.RECRUIT_SUCCESS_LOYALTY;

            MoveHeroToCity(hero.cityId, cityId, new int[] { targetHeroId });

            resultMsg = string.Format("成功 ({0}%)", baseSuccessRate);

            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = "登用" + HeroConfig.GetConfig(targetHeroId).Name,
                valStr = string.Format("<color=green>{0}</color>", resultMsg),
            });
        }
        else
        {
            resultMsg = string.Format("失败 ({0}%)", baseSuccessRate);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attrStr = "登用" + HeroConfig.GetConfig(targetHeroId).Name,
                valStr = string.Format("<color=red>{0}</color>", resultMsg),
            });     
        }

        var heroList = new int[] { myHeroId };
        cityData.AddAction(devId, heroList.Length);
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
            GameLog.Debug($"CalculateForceAttrAddons cityId={city.cityId} assignments={assignments.Count}");
            
            foreach (var assignment in assignments)
            {
                var devCfg = CityDevConfig.GetConfig(assignment.devId);
                var heroData = GameManager.Instance.GetHero(assignment.heroId);
                
                GameLog.Debug($"CalculateForceAttrAddons devId={assignment.devId} DevAttr1={devCfg.DevAttr1} DevAttr2={devCfg.DevAttr2}");
                CalculateForceAttrAddonsForAssignment(devCfg, heroData, city, attrAddons);
            }
        }
        
        foreach (var kvp in attrAddons)
        {
            GameLog.Debug($"CalculateForceAttrAddons result: {kvp.Key}={kvp.Value}");
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
        
        GameLog.Debug($"CalculateForceDevAddonByTier attrName={attrName} tierValue={tierValue} currentVal={currentVal} valMax={valMax} addon={addon}");
        return addon;
    }
}
