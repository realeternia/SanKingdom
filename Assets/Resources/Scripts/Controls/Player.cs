using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class Player
{
    public string pname;
    public int forceId;  //配置表idP

    public int gold;

    public int mark;

    public int food;
    public int maxFood;
    private float lastFoodDeductionTime = 0f;

    public Color lineColor;

    public CastleHUD castleHUD;
    public string imgPath;

    public bool IsPlayer{ get { return GameManager.Instance.GetForce(forceId).isPlayer; } }

    public Player(int id)
    {
        forceId = id;

        gold = 0;
        maxFood = 100;
        food = maxFood;

        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);

        lineColor = ColorUtility.TryParseHtmlString(forceCfg.Color, out lineColor) ? lineColor : Color.white;
        pname = heroCfg.Name;
        imgPath = "Skins/" + heroCfg.Icon;
    }

    public void AddGold(int g)
    {
        if(g <= 0)
            throw new ArgumentException("Gold must be greater than 0");

        gold += g;
    }

    public void AddFood(int f)
    {
        if(f <= 0)
            throw new ArgumentException("Food must be greater than 0");

        food += f;
    }

    public void SubGold(int g, bool isHero)
    {
        gold -= g;
    }
    
    public int SubFood(int f)
    {
        if(food <= 0)
            return 0;
        var sub = Mathf.Min(f, food);
        food -= sub;
        return sub;
    }

    public void OnBattleBegin()
    {
        food = maxFood;
        // 重置上次扣除粮食的时间为当前时间
        lastFoodDeductionTime = BattleManager.Instance.time;
    }

    public void RoundFoodCost()
    {
        // 粮食扣除逻辑
        if (BattleManager.Instance.time - lastFoodDeductionTime >= 5f) // 每5秒扣除一次粮食
        {
            // 计算时间差，每5s，扣10点粮食
            if(food < 10)
            {
                var units = BattleManager.Instance.GetUnitsMySide(1); //todo
                foreach(var unit in units)
                    unit.LackFood((float)(10 - food) / 10);
            }
            food -= 10;
            if (food < 0) food = 0;

            // 更新上次扣除粮食的时间
            lastFoodDeductionTime = BattleManager.Instance.time;
        }
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
    public bool ExecuteCityDev(int cityId, int devId, int[] heroList, out List<string> attrs, out List<int> attrOlds, out List<int> results)
    {
        heroList = GetAvailableHeroesThisYear(heroList).ToArray();

        attrs = new List<string>();
        attrOlds = new List<int>();
        results = new List<int>();
        var resultTmp = new List<float>();
        
        var devConfig = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        
        // 检查黄金是否足够
        if (cityData.gold < devConfig.GoldCost * heroList.Length)
        {
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
            
            // 计算各属性的发展结果
            if (resultTmp.Count == 0)
            {
                resultTmp.Add(0);
            }
            resultTmp[0] += Math.Max(devConfig.DevAttr1Value[0], (float)attrVal / 100 * devConfig.DevAttr1Value[1]);
            
            if (devConfig.DevAttr2Value != null && devConfig.DevAttr2Value[1] != 0)
            {
                if (resultTmp.Count < 2)
                {
                    resultTmp.Add(0);
                }
                if (devConfig.DevAttr2Value[1] > 0)
                {
                    resultTmp[1] += Math.Max(devConfig.DevAttr2Value[0], (float)attrVal / 100 * devConfig.DevAttr2Value[1]);
                }
                else
                {
                    resultTmp[1] += Math.Min(devConfig.DevAttr2Value[0], (float)attrVal / 100 * devConfig.DevAttr2Value[1]);
                }
            }
            
            if (devConfig.DevAttr3Value != null && devConfig.DevAttr3Value[1] != 0)
            {
                if (resultTmp.Count < 3)
                {
                    resultTmp.Add(0);
                }
                if (devConfig.DevAttr3Value[1] > 0)
                {
                    resultTmp[2] += Math.Max(devConfig.DevAttr3Value[0], (float)attrVal / 100 * devConfig.DevAttr3Value[1]);
                }
                else
                {
                    resultTmp[2] += Math.Min(devConfig.DevAttr3Value[0], (float)attrVal / 100 * devConfig.DevAttr3Value[1]);
                }
            }
        }
        
        // 转换结果为整数
        for (int i = 0; i < resultTmp.Count; i++)
        {
            results.Add((int)resultTmp[i]);
        }
        
        // 更新城市属性
        cityData.AddAttr(devConfig.DevAttr1, results[0]);
        attrs.Add(devConfig.DevAttr1);
        attrOlds.Add(cityData.GetAttr(devConfig.DevAttr1));
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            cityData.AddAttr(devConfig.DevAttr2, results[1]);
            attrs.Add(devConfig.DevAttr2);
            attrOlds.Add(cityData.GetAttr(devConfig.DevAttr2));
        }
        
        if (!string.IsNullOrEmpty(devConfig.DevAttr3))
        {
            cityData.AddAttr(devConfig.DevAttr3, results[2]);
            attrs.Add(devConfig.DevAttr3);
            attrOlds.Add(cityData.GetAttr(devConfig.DevAttr3));
        }

        UpdateHeroesRound(heroList);

        return true;
    }

    // 执行城市战斗发展
    public void ExecuteCityBattleDev(int cityId, int devId, int[] heroList, int targetCityId)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var cityDest = GameManager.Instance.GetCity(targetCityId);
        var devConfig = CityDevConfig.GetConfig(devId);

        // 过滤掉当前年份已经执行过动作的英雄
        var validHeroList = GetAvailableHeroesThisYear(heroList).ToArray();
        
        if (devConfig.FindEnemy)
        {
            // 开始战斗
            BattleManager.Instance.BattleBegin(citySrc.GetPlayer(), cityDest.GetPlayer(), cityId, targetCityId, citySrc.GetBattleHeroList(validHeroList), cityDest.GetBattleHeroList());
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

    // 获取城市可用的英雄列表（用于发展）
    public List<int> GetCityHeroListForDev(int cityId)
    {
        var cityData = GameManager.Instance.GetCity(cityId);
        return cityData.GetHeroList();
    }

}
