using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using Controls.Utils;

public class Player
{
    public string pname;
    public int forceId;  //配置表idP

    public int mark;

    public Color lineColor;

    public CastleHUD castleHUD;
    public string imgPath;

    public bool IsPlayer{ get { return GameManager.Instance.GetForce(forceId).isPlayer; } }

    public TurnPhase Phase { get; private set; } = TurnPhase.None;

    public List<WarPlanData> warPlans = new List<WarPlanData>();
    public bool planConfirmed = false;

    public void SetPhase(TurnPhase phase)
    {
        Phase = phase;
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
        Phase = TurnPhase.Planning;

        if (IsPlayer)
        {
            PanelManager.Instance.SendSignal("PhaseChange", "Planning", forceId);
            PanelManager.Instance.SendSignal("AICheck", "", 0);
        }
        else
        {
            PanelManager.Instance.SendSignal("AICheck", pname, forceId);
            GameManager.Instance.StartCoroutine(GameManager.Instance.AIPlayerTurnCoroutine(this));
        }
    }

    public Player(int id)
    {
        forceId = id;

        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);

        lineColor = ColorUtility.TryParseHtmlString(forceCfg.Color, out lineColor) ? lineColor : Color.white;
        pname = heroCfg.Name;
        imgPath = "Textures/Skins/" + heroCfg.Icon;
    }

    public bool ExecuteCityDev(int cityId, int devId, int[] heroList, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        if(heroList.Length == 0)
        {
            GameLog.Warn($"玩家 {pname} 城市 {cityId} 发展任务 {devId} 失败，没有可用英雄");
            attrDatas = null;
            return false;
        }

        attrDatas = new List<PopResultPanelManager.AttrData>();
        var resultTmp = new List<float>();
        
        var devConfig = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        
        // 检查发展任务的主要属性是否已达到最大值
        if (devConfig.Attrs.Length > 0)
        {
            string mainAttr = devConfig.DevAttr1.ToLower();
            var attrConfig = CityAttrConfig.GetConfigByname(mainAttr);
            int currentVal = cityData.GetAttr(mainAttr);
            if (currentVal >= attrConfig.ValMax)
            {
                GameLog.Warn($"玩家 {pname} 城市 {cityId} 发展任务 {devId} 失败，{mainAttr} 已达最大值");
                return false;
            }
        }
        
        // 检查黄金是否足够
        if (cityData.gold < devConfig.GoldCost * heroList.Length)
        {
            GameLog.Warn($"玩家 {pname} 城市 {cityId} 发展任务 {devId} 失败，黄金不足");
            return false;
        }
        
        // 扣除发展成本
        if (devConfig.GoldCost > 0)
        {
            cityData.gold -= devConfig.GoldCost * heroList.Length;
        }
        
        // 计算发展结果
        for (int i = 0; i < heroList.Length; i++)
        {
            var heroData = GameManager.Instance.GetHero(heroList[i]);
            var checkAttr = devConfig.Attrs[0];
            var attrVal = heroData.GetAttr(checkAttr);

            // 计算综合属性值
            if (devConfig.Attrs.Length > 1)
            {
                var attrVal2 = heroData.GetAttr(devConfig.Attrs[1]);
                if (attrVal2 > attrVal)
                {
                    attrVal += (attrVal2 - attrVal) / SystemConst.Hero.SECONDARY_ATTR_CONTRIBUTION_DIVISOR;
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

        // 转换结果为整数并创建 AttrData
        List<int> results = new List<int>();
        for (int i = 0; i < resultTmp.Count; i++)
        {
            results.Add((int)resultTmp[i]);
        }
        
        // 更新城市属性
        int attr1Old = cityData.GetAttr(devConfig.DevAttr1);
        cityData.AddAttr(devConfig.DevAttr1, results[0]);
        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = devConfig.DevAttr1,
            valOld = attr1Old,
            valAddon = results[0],
        });
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            int attr2Old = cityData.GetAttr(devConfig.DevAttr2);
            cityData.AddAttr(devConfig.DevAttr2, results[1]);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = devConfig.DevAttr2,
                valOld = attr2Old,
                valAddon = results[1],
            });
        }


        // 记录发展动作
        cityData.AddAction(devId, heroList.Length);

        // 处理搜索动作，发现在野英雄
        if (devConfig.ActionName == "find")
        {
            CheckFindAction(cityId, cityData, attrDatas);
        }

        return true;
    }

    private static void CheckFindAction(int cityId, SaveCityData cityData, List<PopResultPanelManager.AttrData> attrDatas)
    {
        // 创建城市名称到城市ID的映射字典（提高效率）
        Dictionary<string, int> cityNameToIdMap = new Dictionary<string, int>();
        foreach (var cityConfig in WorldConfig.ConfigList)
        {
            cityNameToIdMap[cityConfig.Cname] = cityConfig.Id;
        }

        // 遍历所有英雄配置
        foreach (var heroConfig in HeroConfig.ConfigList)
        {
            // 先检查出生地是否匹配当前城市（提高效率）
            // 查找heroConfig.BornCity对应的城市ID
            int bornCityId;
            if (!cityNameToIdMap.TryGetValue(heroConfig.BornCity, out bornCityId))
                continue;

            // 用城市ID比较（更高效）
            if (bornCityId != cityId)
                continue;
            
            // 检查英雄年龄是否达到16岁
            float currentYear = GameManager.Instance.GetCurrentYear();
            if (currentYear - heroConfig.BornYear < SystemConst.Game.BORN_AGE)
                continue;

            // 检查英雄是否已经在游戏中
            bool isHeroInGame = false;
            foreach (var existingHero in GameManager.Instance.SaveData.heros)
            {
                if (existingHero.heroId == heroConfig.Id)
                {
                    isHeroInGame = true;
                    break;
                }
            }

            // 如果英雄不在游戏中
            if (!isHeroInGame)
            {
                // 创建新的在野英雄
                SaveHeroData newHero = SaveHeroData.CreateWildHero(heroConfig.Id, cityId);

                // 添加到游戏中
                GameManager.Instance.SaveData.heros.Add(newHero);

                attrDatas.Add(new PopResultPanelManager.AttrData()
                {
                    attrStr = "发现",
                    valStr = string.Format("<color=green>{0}</color>", heroConfig.Name),
                });

                // 重新计算城市英雄
                cityData.RecalculateHeros();

                break; // 每次搜索只发现一个英雄
            }
        }
    }


    private static float GetVal(string resName, int min, int max, int nowVal, int addon)
    {
        var cityAttrConfig = CityAttrConfig.GetConfigByname(resName.ToLower());
        var val = Math.Max(min, (float)addon / 100 * max);
        val = Math.Min(val, cityAttrConfig.ValMax - nowVal);
        return val;
    }


    // 执行城市战斗发展
    public void ExecuteCityBattleDev(int cityId, int devId, int[] heroList, int foodUse, int targetCityId, bool isAI, Dictionary<int, int> heroSoldierDict = null, Dictionary<int, int> heroArmsDict = null)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var cityDest = GameManager.Instance.GetCity(targetCityId);

        if (isAI)
            BattleManager.Instance.SetMode(true, false);
        else
            BattleManager.Instance.SetMode(false, true);

        citySrc.food -= foodUse;
        var defenceFood = cityDest.GetAttr("food");
        cityDest.food = 0;
        int srcForceId = citySrc.forceId;
        int destForceId = cityDest.forceId;
        var battleHeroListSrc = citySrc.GetBattleHeroList(heroList, heroSoldierDict, heroArmsDict);
        BattleManager.Instance.BattleBegin(citySrc.GetPlayer(), cityDest.GetPlayer(), battleHeroListSrc, cityDest.GetBattleHeroList(), foodUse, defenceFood, targetCityId,
            (result, soldierCount, foodCount) => OnBattleEnd(result, soldierCount, foodCount, cityId, targetCityId, battleHeroListSrc.Select(x => x.CardId).ToArray(), srcForceId, destForceId));
    }

    private void OnBattleEnd(BattleResult result, Dictionary<int, int> soldierCount, Dictionary<int, int> foodCount, int cityId, int targetCityId, int[] attackHeroList, int srcForceId, int destForceId)
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
        srcCity.soldier += srcRemaining;
        destCity.soldier += destRemaining;

        if (result == BattleResult.Win)
        {
            destCity.food += foodCount[destForceId] + foodCount[srcForceId];

            destCity.Occupy(forceId, attackHeroList.ToList(), destForceId, destCity.GetBattleHeroList().Select(x => x.CardId).ToList());
            srcCity.RecalculateHeros();
        }
        else
        {
            srcCity.food += foodCount[srcForceId];
            destCity.food += foodCount[destForceId];
        }
    }

    // 移动英雄到目标城市
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
        
         PanelManager.Instance.SendSignal("CityAttrChange", "", destCityId);
    }

    // 执行城市移动发展（带粮草）
    public void ExecuteCityMoveDev(int cityId, int devId, int[] heroList, int foodUse, int targetCityId)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var cityDest = GameManager.Instance.GetCity(targetCityId);

        citySrc.food -= foodUse;
        cityDest.food += foodUse;

        MoveHeroToCity(cityId, targetCityId, heroList);
    }

    public List<SaveCityData> GetCityList()
    {
         return GameManager.Instance.GetCitiesByForce(forceId);
    }

    public SaveCityData GetKingCity()
    {
          var kingHeroId = ForceConfig.GetConfig(forceId).HeroId;
        // 找到 kingHeroId 对应的城市
        foreach (var city in GetCityList())
        {
            if (city.GetOwner() == kingHeroId)
                return city;
        }
        return null;
    }


    // 执行城市发展
    public bool ExecuteCityChange(int cityId, int devId, int[] heroList, bool isBuying, int amount, float rate, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();
        
        var cityData = GameManager.Instance.GetCity(cityId);

        if(isBuying)
        {
            if(cityData.gold < amount)
            {
                SystemTip.Instance.ShowTip("黄金不足");
                return false;
            }

            int goldOld = cityData.GetAttr("Gold");
            cityData.AddAttr("Gold", - amount);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = - amount,
            });

            int foodOld = cityData.GetAttr("Food");
            cityData.AddAttr("Food", (int)(rate * amount));
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Food",
                valOld = foodOld,
                valAddon = (int)(rate * amount),
            });
        }
        else
        {
            if(cityData.food < amount)
            {
                SystemTip.Instance.ShowTip("粮食不足");
                return false;
            }

            int foodOld = cityData.GetAttr("Food");
            cityData.AddAttr("Food", -amount);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Food",
                valOld = foodOld,
                valAddon = -amount,
            });

            int goldOld = cityData.GetAttr("Gold");
            cityData.AddAttr("Gold", (int)(rate * amount));
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = (int)(rate * amount),
            });
        }

        // 记录发展动作
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
            baseSuccessRate = SystemConst.Hero.RECRUIT_WILD_BASE_RATE;
        }
        else if(hero.state == HeroState.Catched || (hero.state == HeroState.Normal && hero.forceId != cityData.forceId))
        {
            int loyalty = hero.loyalty;
            int diff = 100 - loyalty;
            baseSuccessRate = diff * diff / SystemConst.Hero.RECRUIT_CAPTURED_FORMULA_A + diff / SystemConst.Hero.RECRUIT_CAPTURED_FORMULA_B;
        }

        if(myHeroId > 0)
        {
            var executorHero = GameManager.Instance.GetHero(myHeroId);
            if(executorHero != null)
            {
                int charm = executorHero.GetAttr("charm");
                if(charm >= SystemConst.Hero.CHARM_BONUS_TIER1)
                    baseSuccessRate = baseSuccessRate * SystemConst.Hero.RECRUIT_TIER1_MULTIPLIER / 100;
                else if(charm >= SystemConst.Hero.CHARM_BONUS_TIER2)
                    baseSuccessRate = baseSuccessRate * SystemConst.Hero.RECRUIT_TIER2_MULTIPLIER / 100;
                if(myHeroId == ForceConfig.GetConfig(executorHero.forceId).HeroId)
                    baseSuccessRate = baseSuccessRate * SystemConst.Hero.KING_RECRUIT_MULTIPLIER / 100;
            }
        }

        int randomVal = UnityEngine.Random.Range(0, 100);
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
            if(cityData.gold < totalCost)
            {
                SystemTip.Instance.ShowTip("黄金不足");
                return false;
            }
            cityData.gold -= totalCost;
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = cityData.GetAttr("gold") + totalCost,
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
                loyaltyAdd = UnityEngine.Random.Range(SystemConst.Hero.PRAISE_LOYALTY_ADD_MIN, SystemConst.Hero.PRAISE_LOYALTY_ADD_MAX);
            }
            else if(methodId == 2)
            {
                loyaltyAdd = UnityEngine.Random.Range(SystemConst.Hero.REWARD_LOYALTY_ADD_MIN, SystemConst.Hero.REWARD_LOYALTY_ADD_MAX);
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

