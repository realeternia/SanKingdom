using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class Player
{
    public string pname;
    public int pid;  //自增id
    public int forceId;  //配置表id

    public int gold;

    public int mark;

    public bool isAI = false;
    public int food;
    public int maxFood;
    private float lastFoodDeductionTime = 0f;

    public Color lineColor;

    public int sodatk = 0; //士兵atk强化
    public int sodhp = 0; //士兵def强化

    public int lastFightMark;

    public CastleHUD castleHUD;
    public string imgPath;

    public void Init(int id, int forceId, string imagePath)
    {
        pid = id;
        this.forceId = forceId;
        isAI = id > 0;

        gold = 0;
        maxFood = 100;
        food = maxFood;

        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);

        lineColor = ColorUtility.TryParseHtmlString(forceCfg.Color, out lineColor) ? lineColor : Color.white;
        pname = heroCfg.Name;
        this.imgPath = imagePath;
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
        lastFoodDeductionTime = Time.time;
    }

    public void RoundFoodCost()
    {
        // 粮食扣除逻辑
        if (Time.time - lastFoodDeductionTime >= 5f) // 每5秒扣除一次粮食
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
            lastFoodDeductionTime = Time.time;
        }
    }

    public void onBattleResult(bool isWin, int add)
    {
        mark += add;
    }
}
