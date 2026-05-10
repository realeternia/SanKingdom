using System.Collections.Generic;
using CommonConfig;
using Controls.Utils;

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
    public HeroState state;
    public int loyalty;
    public int forceId;

    public int str;
    public int inte;
    public int fair;
    public int charm;
    public int leadShip;

    public void InitAttrsFromConfig()
    {
        var heroConfig = HeroConfig.GetConfig(heroId);
        if (heroConfig == null) 
            return;
        if (str == 0) str = heroConfig.Str;
        if (inte == 0) inte = heroConfig.Inte;
        if (fair == 0) fair = heroConfig.Fair;
        if (charm == 0) charm = heroConfig.Charm;
        if (leadShip == 0) leadShip = heroConfig.LeadShip;
    }

    public int GetAttr(string attr)
    {
        InitAttrsFromConfig();
        switch (attr.ToLower())
        {
            case "str":
                return str;
            case "inte":
                return inte;
            case "fair":
                return fair;
            case "leadship":
                return leadShip;
            case "charm":
                return charm;
            default:
                return 0;
        }
    }

    public int GetLevel()
    {
        return HeroSelectionTool.GetCardLevel(exp, true);
    }

    public int GetArmsId()
    {
        var troop = GetTroop();
        return troop != null ? troop.armsId : 0;
    }

    public SaveTroopsData GetTroop()
    {
        return SaveTroopsData.FindByHeroId(heroId);
    }

    public static SaveHeroData CreateWildHero(int heroId, int cityId)
    {
        SaveHeroData newHero = new SaveHeroData();
        newHero.heroId = heroId;
        newHero.exp = 0;
        newHero.cityId = cityId;
        newHero.state = HeroState.Wild;
        newHero.loyalty = SystemConst.Hero.WILD_HERO_DEFAULT_LOYALTY;
        newHero.forceId = SystemConst.Hero.WILD_FORCE_ID;
        newHero.InitAttrsFromConfig();
        return newHero;
    }

}