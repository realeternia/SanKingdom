using System.Collections.Generic;
using CommonConfig;

public enum CityNeedType
{
    None,
    GoldShortage,
    FoodShortage,
    WallLow,
    SoldierShortage,
    PowerLow
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
    private const int GOLD_ALERT = 500;
    private const int FOOD_ALERT = 500;
    private const int WALL_ALERT = 150;
    private const int SOLDIER_ALERT = 500;
    private const int POWER_ALERT = 50;
    
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
        
        if (city.GetAttr("power") < POWER_ALERT)
        {
            int prio = CalculatePriority(city.GetAttr("power"), POWER_ALERT);
            needs.Add(new CityNeed(CityNeedType.PowerLow, prio, "power", city.GetAttr("power"), POWER_ALERT));
        }
        
        needs.Sort((a, b) => b.priority.CompareTo(a.priority));
        
        return needs;
    }
    
    private static int CalculatePriority(int current, int alert)
    {
        if (current <= 0) return 100;
        int deficit = alert - current;
        return (deficit * 100) / alert;
    }
    
    public static bool IsFrontlineCity(SaveCityData city)
    {
        var nearCityIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
        if (nearCityIds == null) return false;
        
        foreach (var nearId in nearCityIds)
        {
            var nearCity = GameManager.Instance.GetCity(nearId);
            if (nearCity != null && nearCity.forceId != city.forceId)
            {
                return true;
            }
        }
        return false;
    }
}
