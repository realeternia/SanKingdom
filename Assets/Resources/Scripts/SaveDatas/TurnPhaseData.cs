using System.Collections.Generic;

public enum TurnPhase
{
    None,
    Planning,
    Execution,
    Battle,
}

public class WarPlanData
{
    public int forceId;
    public int sourceCityId;
    public int targetCityId;
    public int[] heroIds;
    public int foodCost;
    public Dictionary<int, int> heroSoldierDict;
    public Dictionary<int, int> heroArmsDict;

    public WarPlanData()
    {
        heroIds = new int[0];
        heroSoldierDict = new Dictionary<int, int>();
        heroArmsDict = new Dictionary<int, int>();
    }
}
