using System.Collections.Generic;
using System.Linq;
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
    
    public static TaskPriorityInfo GetBattleTask(SaveCityData city)
    {
        foreach (var devConfig in CityDevConfig.ConfigList)
        {
            if (devConfig.Prefab == "CityDevBattle" && IsTaskAvailable(city, devConfig))
            {
                return new TaskPriorityInfo(devConfig.Id, devConfig.AiPriotyAtk, devConfig);
            }
        }
        return null;
    }
    
    public static List<TaskPriorityInfo> GetAvailableTasks(SaveCityData city, CityStrategyState state, List<CityNeed> cityNeeds)
    {
        var result = new List<TaskPriorityInfo>();
        
        foreach (var devConfig in CityDevConfig.ConfigList)
        {
            if (!IsTaskAvailable(city, devConfig))
                continue;
            
            int basePriority = GetBasePriority(devConfig, state);
            if (basePriority <= 0)
                continue;
            
            var taskInfo = new TaskPriorityInfo(devConfig.Id, basePriority, devConfig);
            taskInfo.adjustedPriority = AdjustPriorityByNeeds(taskInfo, cityNeeds, city);
            result.Add(taskInfo);
        }
        
        result.Sort((a, b) => b.adjustedPriority.CompareTo(a.adjustedPriority));
        
        return result;
    }
    
    private static bool IsTaskAvailable(SaveCityData city, CityDevConfig config)
    {
        if (city.gold < config.GoldCost)
            return false;
        
        switch (config.Prefab)
        {
            case "CityDevNormal":
                return IsNormalTaskAvailable(city, config);
            case "CityDevBattle":
                return HasSoldier(city);
            case "CityDevMove":
                return HasSoldier(city);
            case "CityDevUseHero":
                return city.GetRecruitableHeroList().Count > 0;
            case "CityDevChange":
                return city.gold >= 300;
            case "CityDevPraiseHero":
                return HasLowLoyaltyHero(city);
            default:
                return false;
        }
    }
    
    private static bool IsNormalTaskAvailable(SaveCityData city, CityDevConfig config)
    {
        string mainAttr = config.DevAttr1?.ToLower() ?? "";
        if (string.IsNullOrEmpty(mainAttr))
            return false;
        
        var attrConfig = CityAttrConfig.GetConfigByname(mainAttr);
        if (attrConfig == null)
            return false;
        
        int currentVal = city.GetAttr(mainAttr);
        return currentVal < attrConfig.ValMax;
    }
    
    private static bool HasSoldier(SaveCityData city)
    {
        return city.GetAttr("soldier") > 0;
    }
    
    private static bool HasLowLoyaltyHero(SaveCityData city)
    {
        return city.GetNormalHeroList()
            .Select(h => GameManager.Instance.GetHero(h))
            .Any(h => h.loyalty < 80);
    }
    
    private static int GetBasePriority(CityDevConfig config, CityStrategyState state)
    {
        switch (state)
        {
            case CityStrategyState.Dev:
                return config.AiPriotyDev;
            case CityStrategyState.Atk:
                return config.AiPriotyAtk;
            case CityStrategyState.Def:
                return config.AiPriotyDef;
            default:
                return 0;
        }
    }
    
    private static int AdjustPriorityByNeeds(TaskPriorityInfo taskInfo, List<CityNeed> needs, SaveCityData city)
    {
        int adjusted = taskInfo.basePriority;
        
        if (taskInfo.config.Prefab == "CityDevChange")
        {
            int totalSoldier = city.GetAttr("soldier");
            int foodThreshold = totalSoldier / 2;
            if (totalSoldier > 0 && city.food < foodThreshold)
            {
                adjusted += NEED_WEIGHT;
            }
        }
        
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
        string attr1 = config.DevAttr1?.ToLower() ?? "";
        string attr2 = config.DevAttr2?.ToLower() ?? "";
        
        switch (need.needType)
        {
            case CityNeedType.GoldShortage:
                return attr1 == "gold" || attr2 == "gold";
            case CityNeedType.FoodShortage:
                return attr1 == "food" || attr2 == "food" || config.Prefab == "CityDevChange";
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
}
