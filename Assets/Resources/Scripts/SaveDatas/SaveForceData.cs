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
    private Dictionary<string, int> posResCache = new Dictionary<string, int>();

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
            return GetPosResFromCache(type.ToLower());
        }
        
        switch (type.ToLower())
        {
            case "gold":
                return (int)Math.Floor(gold);
            default:
                return 0;
        }
    }

    private int GetPosResFromCache(string type)
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
            posResCache[attr.name] = 0;
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
                
                string resType = devConfig.DevAttr1.ToLower();
                posResCache[resType] += devConfig.DevAttr1Value[0];
            }
        }        
        
        if (isPlayer)
        {
            foreach (var kvp in posResCache)
            {
                PanelManager.Instance.SendSignal(new ForceResChangeSignal { ResType = kvp.Key, Value = kvp.Value });
            }
        }
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
        posResCache = new Dictionary<string, int>();
        RecalculatePosRes();
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
            var checkAttr = devConfig.Attrs[0];
            var attrVal = heroData.GetAttr(checkAttr);

            if (devConfig.Attrs.Length > 1)
            {
                var attrVal2 = heroData.GetAttr(devConfig.Attrs[1]);
                if (attrVal2 > attrVal)
            {
                attrVal += SysFormula.City.CalculateSecondaryAttrContribution(attrVal, attrVal2);
            }
            }

            resultTmp.Add(0);
            var val = GetVal(devConfig.DevAttr1, devConfig.DevAttr1Value[0], devConfig.DevAttr1Value[1], cityData.GetAttr(devConfig.DevAttr1), attrVal);
            resultTmp[0] += val;

            if (devConfig.DevAttr2Value != null && devConfig.DevAttr2Value[1] != 0)
            {
                resultTmp.Add(0);
                if (devConfig.DevAttr2Value[1] > 0)
                {
                    resultTmp[1] += GetVal(devConfig.DevAttr2, devConfig.DevAttr2Value[0], devConfig.DevAttr2Value[1], cityData.GetAttr(devConfig.DevAttr2), attrVal);
                }
                else
                {
                    resultTmp[1] += GetVal(devConfig.DevAttr2, devConfig.DevAttr2Value[0], devConfig.DevAttr2Value[1], cityData.GetAttr(devConfig.DevAttr2), attrVal);
                }
            }

        }

        List<int> results = new List<int>();
        for (int i = 0; i < resultTmp.Count; i++)
        {
            results.Add((int)resultTmp[i]);
        }
        
        var attr1Config = CityAttrConfig.GetConfigByname(devConfig.DevAttr1.ToLower());
        int attr1Old = attr1Config.IsForceAttr ? GetAttr(devConfig.DevAttr1) : cityData.GetAttr(devConfig.DevAttr1);
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
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            var attr2Config = CityAttrConfig.GetConfigByname(devConfig.DevAttr2.ToLower());
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

    private static float GetVal(string resName, int min, int max, int nowVal, int addon)
    {
        var cityAttrConfig = CityAttrConfig.GetConfigByname(resName.ToLower());
        int valMax = cityAttrConfig.IsForceAttr ? cityAttrConfig.ValMaxForce : cityAttrConfig.ValMaxCity;
        var val = SysFormula.City.CalculateDevValue(min, max, addon, nowVal, valMax);
        return val;
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
}
