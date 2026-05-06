using System.Collections.Generic;
using CommonConfig;

public enum CityNeedType
{
    None,
    GoldShortage,
    FoodShortage,
    WallLow,
    SoldierShortage,
    HappyLow
}

public class CityNeed
{
    public CityNeedType needType;
    public int priority;
    public string attrName;
    public int currentValue;
    public int alertValue;
    
    public CityNeed(CityNeedType type, int prio, string attr, int current, int alert)
    {
        needType = type;
        priority = prio;
        attrName = attr;
        currentValue = current;
        alertValue = alert;
    }
}

public class CityEvaluator
{
    private const int GOLD_ALERT = SystemConst.AICity.GOLD_ALERT;
    private const int FOOD_ALERT = SystemConst.AICity.FOOD_ALERT;
    private const int WALL_ALERT = SystemConst.AICity.WALL_ALERT;
    private const int SOLDIER_ALERT = SystemConst.AICity.SOLDIER_ALERT;
    private const int HAPPY_ALERT = SystemConst.AICity.HAPPY_ALERT;
    
    public static List<CityNeed> EvaluateCity(SaveCityData city)
    {
        var needs = new List<CityNeed>();
        
        if (city.GetAttr("gold") < GOLD_ALERT)
        {
            int prio = CalculatePriority(city.GetAttr("gold"), GOLD_ALERT);
            needs.Add(new CityNeed(CityNeedType.GoldShortage, prio, "gold", city.GetAttr("gold"), GOLD_ALERT));
        }
        
        if (city.GetAttr("food") < FOOD_ALERT)
        {
            int prio = CalculatePriority(city.GetAttr("food"), FOOD_ALERT);
            needs.Add(new CityNeed(CityNeedType.FoodShortage, prio, "food", city.GetAttr("food"), FOOD_ALERT));
        }
        
        if (city.GetAttr("wall") < WALL_ALERT)
        {
            int prio = CalculatePriority(city.GetAttr("wall"), WALL_ALERT);
            needs.Add(new CityNeed(CityNeedType.WallLow, prio, "wall", city.GetAttr("wall"), WALL_ALERT));
        }
        
        int totalSoldier = city.GetAttr("soldier");
        if (totalSoldier < SOLDIER_ALERT)
        {
            int prio = CalculatePriority(totalSoldier, SOLDIER_ALERT);
            needs.Add(new CityNeed(CityNeedType.SoldierShortage, prio, "soldier", totalSoldier, SOLDIER_ALERT));
        }
        
        if (city.GetAttr("happy") < HAPPY_ALERT)
        {
            int prio = CalculatePriority(city.GetAttr("happy"), HAPPY_ALERT);
            needs.Add(new CityNeed(CityNeedType.HappyLow, prio, "happy", city.GetAttr("happy"), HAPPY_ALERT));
        }
        
        needs.Sort((a, b) => b.priority.CompareTo(a.priority));
        
        return needs;
    }
    
    private static int CalculatePriority(int current, int alert)
    {
        return SysFormula.AIStrategy.CalculatePriority(current, alert);
    }
    
    public static bool IsFrontlineCity(SaveCityData city)
    {
        return MapTool.IsFrontlineCity(city.cityId);
    }
}
