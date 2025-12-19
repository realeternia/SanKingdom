using System.Collections.Generic;
using CommonConfig;

[System.Serializable]
public class SaveHeroData
{
    public int heroId;
    public int exp;

    public int cityId;
    public bool cityOwner; //太守

    
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

}