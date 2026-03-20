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
    public int soldier;
    public int exp;

    public int cityId;
    public bool cityOwner;
    public int round;
    public HeroState state;
    public int loyalty;
    public int forceId;

    
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

    public void SetRoundForRecruit()
    {
        round = GameManager.Instance.SaveData.round + 1;
    }

    public static SaveHeroData CreateWildHero(int heroId, int cityId)
    {
        SaveHeroData newHero = new SaveHeroData();
        newHero.heroId = heroId;
        newHero.soldier = 100;
        newHero.exp = 0;
        newHero.cityId = cityId;
        newHero.cityOwner = false;
        newHero.round = int.MaxValue;
        newHero.state = HeroState.Wild;
        newHero.loyalty = 90;
        newHero.forceId = 0;
        return newHero;
    }

}