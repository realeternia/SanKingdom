using System.Collections.Generic;
using CommonConfig;

public enum HeroState
{
    Normal, // 正常
    Wild    // 在野
}

[System.Serializable]
public class SaveHeroData
{
    public int heroId;
    public int soldier;
    public int exp;

    public int cityId;
    public bool cityOwner; //太守
    public int round; // 当前年份，用于标记英雄是否已执行动作
    public HeroState state; // 状态

    
    public int GetAttr(string attr)
    {
        var heroConfig = HeroConfig.GetConfig(heroId);
        switch (attr.ToLower())
        {
            case "str":
                return heroConfig.Str;
            case "inte":
                return heroConfig.Inte;
            case "fair":
                return heroConfig.Fair;
            case "leadship":
                return heroConfig.LeadShip;
            case "charm":
                return heroConfig.Charm;
            case "soldier":
                return soldier;
            default:
                return 0;
        }
    }

    public int GetLevel()
    {
        return HeroSelectionTool.GetCardLevel(exp, true);
    }

    // 创建在野英雄的静态方法
    public static SaveHeroData CreateWildHero(int heroId, int cityId)
    {
        SaveHeroData newHero = new SaveHeroData();
        newHero.heroId = heroId;
        newHero.soldier = 100; // 初始士兵数
        newHero.exp = 0;
        newHero.cityId = cityId;
        newHero.cityOwner = false;
        newHero.round = int.MaxValue; // 设置为很大的值，使英雄无法执行任务
        newHero.state = HeroState.Wild;
        return newHero;
    }

}