using System.Collections.Generic;

public class WarPlanData
{
    public List<WarTroopsData> teams;
    public int targetCityId;
    public int sourceCityId;
    public int[] heroIds;
    public Dictionary<int, int> heroSoldierDict;
    public Dictionary<int, int> heroArmsDict;
    public int foodCost;

    public WarPlanData()
    {
        teams = new List<WarTroopsData>();
        targetCityId = 0;
        sourceCityId = 0;
        heroIds = new int[0];
        heroSoldierDict = new Dictionary<int, int>();
        heroArmsDict = new Dictionary<int, int>();
        foodCost = 0;
    }
}
