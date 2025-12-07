using System.Collections;
using System.Collections.Generic;

// 用于Unity JsonUtility序列化的辅助类
[System.Serializable]
public class SaveData
{
    public List<string> players = new List<string>();
    public List<SaveCityData> cities = new List<SaveCityData>();
    public int year;
}

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
    public int leader;
    public List<int> members;

    public List<int> GetHeroList()
    {
        List<int> heroList = new List<int>();
        foreach (var member in members)
        {
            heroList.Add(member);
        }
        if (leader != 0)
            heroList.Add(leader);
        return heroList;    
    }
}