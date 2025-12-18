using System.Collections.Generic;

[System.Serializable]
public class SaveCityData
{
    public int cityId;
    public int forceId;
    public int archGold; //商业
    public int archFood; //农业
    public int archPeople; //人口
    public int gold; //现有黄金
    public int food; //现有粮食
    public int soldier; //士兵
    public int secure; //安全系数
    public int wall; //城防
    public List<SaveHeroData> heros;

    public List<int> GetHeroList()
    {
        List<int> heroList = new List<int>();
        foreach (var member in heros)
        {
            heroList.Add(member.heroId);
        }
        return heroList;    
    }

    public int GetOwner()
    {
        foreach (var member in heros)
        {
            if (member.cityOwner)
                return member.heroId;
        }
        return 0;
    }

    public int GetAttr(string type)
    {
        switch (type.ToLower())
        {
            case "archgold":
                return archGold;
            case "archfood":
                return archFood;
            case "archpeople":
                return archPeople;
            case "gold":
                return gold;
            case "food":
                return food;
            case "soldier":
                return soldier;
            case "secure":
                return secure;
            case "wall":
                return wall;
            default:
                return 0;
        }
    }
}