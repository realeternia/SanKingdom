using System.Collections.Generic;
using CommonConfig;

public class TaskPriorityInfo
{
    public int devId;
    public int basePriority;
    public int adjustedPriority;
    public CityDevConfig config;
    
    public TaskPriorityInfo(int id, int basePrio, CityDevConfig cfg)
    {
        devId = id;
        basePriority = basePrio;
        adjustedPriority = basePrio;
        config = cfg;
    }
}

public class TaskPriorityCalculator
{
    private const int NEED_WEIGHT = 30;
    
    public static List<TaskPriorityInfo> CalculatePriorities(
        SaveCityData city, 
        CityStrategyState strategyState,
        List<CityNeed> cityNeeds)
    {
        var result = new List<TaskPriorityInfo>();
        
        foreach (var devConfig in CityDevConfig.ConfigList)
        {
            if (devConfig.Prefab == "CityDevBattle" || 
                devConfig.Prefab == "CityDevUseHero" || 
                devConfig.Prefab == "CityDevChange")
            {
                continue;
            }
            
            string mainAttr = devConfig.DevAttr1?.ToLower() ?? "";
            if (string.IsNullOrEmpty(mainAttr))
                continue;
            
            var attrConfig = CityAttrConfig.GetConfigByname(mainAttr);
            if (attrConfig == null)
                continue;
                
            int currentVal = city.GetAttr(mainAttr);
            if (currentVal >= attrConfig.ValMax)
                continue;
            
            if (city.gold < devConfig.GoldCost)
                continue;
            
            int basePriority = GetBasePriority(devConfig, strategyState);
            
            var taskInfo = new TaskPriorityInfo(devConfig.Id, basePriority, devConfig);
            
            taskInfo.adjustedPriority = AdjustPriorityByNeeds(taskInfo, cityNeeds);
            
            result.Add(taskInfo);
        }
        
        result.Sort((a, b) => b.adjustedPriority.CompareTo(a.adjustedPriority));
        
        return result;
    }
    
    private static int GetBasePriority(CityDevConfig config, CityStrategyState state)
    {
        int priority = 0;
        
        switch (state)
        {
            case CityStrategyState.Dev:
                priority = config.AiPriotyDev;
                break;
            case CityStrategyState.Atk:
                priority = config.AiPriotyAtk;
                break;
            case CityStrategyState.Def:
                priority = config.AiPriotyDef;
                break;
        }
        
        return priority;
    }
    
    private static int AdjustPriorityByNeeds(TaskPriorityInfo taskInfo, List<CityNeed> needs)
    {
        int adjusted = taskInfo.basePriority;
        
        foreach (var need in needs)
        {
            if (TaskMatchesNeed(taskInfo.config, need))
            {
                adjusted += NEED_WEIGHT * need.priority / 100;
            }
        }
        
        return adjusted;
    }
    
    private static bool TaskMatchesNeed(CityDevConfig config, CityNeed need)
    {
        string attr1 = config.DevAttr1.ToLower();
        string attr2 = config.DevAttr2?.ToLower() ?? "";
        
        switch (need.needType)
        {
            case CityNeedType.GoldShortage:
                return attr1 == "archgold" || attr2 == "archgold";
            case CityNeedType.FoodShortage:
                return attr1 == "archfood" || attr2 == "archfood";
            case CityNeedType.SecureLow:
                return attr1 == "secure" || attr2 == "secure";
            case CityNeedType.WallLow:
                return attr1 == "wall" || attr2 == "wall";
            case CityNeedType.SoldierShortage:
                return attr1 == "soldier" || attr2 == "soldier";
            case CityNeedType.PowerLow:
                return attr1 == "power" || attr2 == "power";
            default:
                return false;
        }
    }
    
    public static List<TaskPriorityInfo> GetAvailableTasks(SaveCityData city, CityStrategyState state, List<CityNeed> needs)
    {
        return CalculatePriorities(city, state, needs);
    }
}
