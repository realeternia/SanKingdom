using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public class Player
{
    public string pname;
    public int forceId;  //配置表idP

    public int mark;

    public Color lineColor;

    public CastleHUD castleHUD;
    public string imgPath;

    public bool IsPlayer{ get { return GameManager.Instance.GetForce(forceId).isPlayer; } }

    public Player(int id)
    {
        forceId = id;

        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);

        lineColor = ColorUtility.TryParseHtmlString(forceCfg.Color, out lineColor) ? lineColor : Color.white;
        pname = heroCfg.Name;
        imgPath = "Skins/" + heroCfg.Icon;
    }

    // 检查英雄是否在当前年份已经执行过动作
    public bool CheckHeroRound(int heroId)
    {
        var hero = GameManager.Instance.GetHero(heroId);
        var currentRound = GameManager.Instance.SaveData.round;
        return hero.round != currentRound;
    }

    public void UpdateHeroesRound(int[] heroIds)
    {
        var currentRound = GameManager.Instance.SaveData.round;
        foreach (var heroId in heroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            hero.round = currentRound;
        }
    }

    // 获取当前年份可用的英雄列表
    public List<int> GetAvailableHeroesThisYear(int[] heroList)
    {
        var validHeroList = new List<int>();
        foreach (var heroId in heroList)
        {
            if (CheckHeroRound(heroId))
            {
                validHeroList.Add(heroId);
            }
        }
        return validHeroList;
    }

    // 执行城市发展
    public bool ExecuteCityDev(int cityId, int devId, int[] heroList, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        heroList = GetAvailableHeroesThisYear(heroList).ToArray();

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
                Debug.LogError($"玩家 {pname} 城市 {cityId} 发展任务 {devId} 失败，{mainAttr} 已达最大值");
                return false;
            }
        }
        
        // 检查黄金是否足够
        if (cityData.gold < devConfig.GoldCost * heroList.Length)
        {
            Debug.LogError($"玩家 {pname} 城市 {cityId} 发展任务 {devId} 失败，黄金不足");
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
                    attrVal += (attrVal2 - attrVal) / 3;
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

            if (devConfig.DevAttr3Value != null && devConfig.DevAttr3Value[1] != 0)
            {
                resultTmp.Add(0);
                if (devConfig.DevAttr3Value[1] > 0)
                {
                    resultTmp[2] += GetVal(devConfig.DevAttr3, devConfig.DevAttr3Value[0], devConfig.DevAttr3Value[1], cityData.GetAttr(devConfig.DevAttr3), attrVal);
                }
                else
                {
                    resultTmp[2] += GetVal(devConfig.DevAttr3, devConfig.DevAttr3Value[0], devConfig.DevAttr3Value[1], cityData.GetAttr(devConfig.DevAttr3), attrVal);
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
            valStr = null
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
                valStr = null
            });
        }
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr3))
        {
            int attr3Old = cityData.GetAttr(devConfig.DevAttr3);
            cityData.AddAttr(devConfig.DevAttr3, results[2]);
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = devConfig.DevAttr3,
                valOld = attr3Old,
                valAddon = results[2],
                valStr = null
            });
        }

        // 记录发展动作
        cityData.AddAction(devId, heroList.Length);

        // 处理搜索动作，发现在野英雄
        if (devConfig.ActionName == "find")
        {
            CheckFindAction(cityId, cityData, attrDatas);
        }

        UpdateHeroesRound(heroList);

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
            if (currentYear - heroConfig.BornYear < 16)
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
                    attr = "发现",
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
    public void ExecuteCityBattleDev(int cityId, int devId, int[] heroList, int foodUse, int targetCityId)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var cityDest = GameManager.Instance.GetCity(targetCityId);
        var devConfig = CityDevConfig.GetConfig(devId);

        // 过滤掉当前年份已经执行过动作的英雄
        var validHeroList = GetAvailableHeroesThisYear(heroList).ToArray();
        
        if (devConfig.FindEnemy)
        {
            BattleManager.Instance.SetMode(false, true);

            citySrc.food -= foodUse;
            var defenceFood = cityDest.food;
            cityDest.food = 0;
            // 开始战斗
            BattleManager.Instance.BattleBegin(citySrc.GetPlayer(), cityDest.GetPlayer(), citySrc.GetBattleHeroList(validHeroList), cityDest.GetBattleHeroList(), foodUse, defenceFood, (hasWin, soldierCount, foodCount) => {
                foreach (var item in soldierCount)
                    GameManager.Instance.GetHero(item.Key).soldier = item.Value;

                var destCity2 = GameManager.Instance.GetCity(targetCityId);
                var srcCity2 = GameManager.Instance.GetCity(cityId);
                if (hasWin)
                {
                    destCity2.food += foodCount[cityDest.forceId] + foodCount[citySrc.forceId];

                    destCity2.Occupy(citySrc.GetPlayer().forceId, citySrc.GetBattleHeroList(validHeroList).Select(x => x.CardId).ToList(),
                        cityDest.GetPlayer().forceId, cityDest.GetBattleHeroList().Select(x => x.CardId).ToList());
                    srcCity2.RecalculateHeros(); //因为有一帮人出去了
                }
                else
                {
                    srcCity2.food += foodCount[citySrc.forceId];
                    destCity2.food += foodCount[cityDest.forceId];
                }
            });
        }
        else
        {
            // 移动英雄到目标城市
            MoveHeroToCity(cityId, targetCityId, validHeroList);
        }
               
        // 更新英雄的年份
        UpdateHeroesRound(validHeroList);
    }

    // 移动英雄到目标城市
    public void MoveHeroToCity(int srcCityId, int destCityId, int[] heroIds)
    {
        var citySrc = GameManager.Instance.GetCity(srcCityId);
        var cityDest = GameManager.Instance.GetCity(destCityId);
        
        citySrc.MoveHeroTo(heroIds, destCityId);
        citySrc.RecalculateHeros();
        cityDest.RecalculateHeros();
        
        // 发送城市属性变化信号
        PanelManager.Instance.SendSignal("CityAttrChange", "", 0);
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
        heroList = GetAvailableHeroesThisYear(heroList).ToArray();

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
                valStr = null
            });

            int foodOld = cityData.GetAttr("Food");
            cityData.AddAttr("Food", (int)(rate * amount));
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Food",
                valOld = foodOld,
                valAddon = (int)(rate * amount),
                valStr = null
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
                valStr = null
            });

            int goldOld = cityData.GetAttr("Gold");
            cityData.AddAttr("Gold", (int)(rate * amount));
            attrDatas.Add(new PopResultPanelManager.AttrData()
            {
                attr = "Gold",
                valOld = goldOld,
                valAddon = (int)(rate * amount),
                valStr = null
            });
        }

        // 记录发展动作
        cityData.AddAction(devId, heroList.Length);

        UpdateHeroesRound(heroList);

        return true;
    }

    // 执行城市使用在野英雄
    public bool ExecuteCityUseHero(int cityId, int devId, int[] heroList, int targetHeroId, out List<PopResultPanelManager.AttrData> attrDatas)
    {
        attrDatas = new List<PopResultPanelManager.AttrData>();
        
        var cityData = GameManager.Instance.GetCity(cityId);

        var hero = GameManager.Instance.GetHero(targetHeroId);
        if(hero.state == HeroState.Normal || hero.cityId != cityId)
        {
            SystemTip.Instance.ShowTip("只有在野英雄才能使用");
            return false;
        }
        hero.state = HeroState.Normal;
        hero.round = int.MaxValue; // 重置回合，使英雄可以执行任务

        attrDatas.Add(new PopResultPanelManager.AttrData()
        {
            attr = "登用" + HeroConfig.GetConfig(targetHeroId).Name,
            valStr = "<color=green>成功</color>",
        });

        // 记录发展动作
        cityData.AddAction(devId, heroList.Length);

        UpdateHeroesRound(heroList);

        return true;
    }
}

