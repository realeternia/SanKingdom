using System.Collections.Generic;
using CommonConfig;

[System.Serializable]
public class SaveHeroData
{
    public int heroId;
    public int soldier;
    public int exp;

    public int cityId;
    public bool cityOwner; //太守
    public int currentYear; // 当前年份，用于标记英雄是否已执行动作

    
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

}