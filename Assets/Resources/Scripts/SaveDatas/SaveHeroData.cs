using System.Collections.Generic;
using CommonConfig;

public enum HeroState
{
    Normal, // 正常
    Wild,   // 在野
    Catched // 被俘虏
}

[System.Serializable]
public class SaveHeroData
{
    public int heroId;
    public int exp;

    public int cityId;
    public bool cityOwner;
    public HeroState state;
    public int loyalty;
    public int forceId;
    public int armsId;

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
            default:
                return 0;
        }
    }

    public int GetLevel()
    {
        return HeroSelectionTool.GetCardLevel(exp, true);
    }

    public static SaveHeroData CreateWildHero(int heroId, int cityId)
    {
        SaveHeroData newHero = new SaveHeroData();
        newHero.heroId = heroId;
        newHero.exp = 0;
        newHero.cityId = cityId;
        newHero.cityOwner = false;
        newHero.state = HeroState.Wild;
        newHero.loyalty = SystemConst.Hero.WILD_HERO_DEFAULT_LOYALTY;
        newHero.forceId = SystemConst.Hero.WILD_FORCE_ID;
        newHero.armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
        return newHero;
    }

}