using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public class DefenseStrategy : IAIStrategy
{
    public string GetStrategyName()
    {
        return "Defense";
    }
    
    public void Execute(AIStrategyContext context)
    {
        foreach (var city in context.cities)
        {
            ExecuteCityDefense(context.player, city, context);
        }
    }
    
    private void ExecuteCityDefense(Player player, SaveCityData city, AIStrategyContext context)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return;
        
        var cityNeeds = CityEvaluator.EvaluateCity(city);
        var availableTasks = TaskPriorityCalculator.GetAvailableTasks(
            city, CityStrategyState.Def, cityNeeds);
        
        EnsureDefenseTasks(ref availableTasks);
        
        if (availableTasks.Count == 0)
            return;
        
        var assignments = HeroTaskMatcher.AssignHeroesToTasks(availableHeroes, availableTasks);
        
        foreach (var kvp in assignments)
        {
            int devId = kvp.Key;
            var heroIds = kvp.Value.ToArray();
            
            if (heroIds.Length > 0)
            {
                player.ExecuteCityDev(city.cityId, devId, heroIds, out _);
                UnityEngine.Debug.Log($"AI防御: 城市{city.cityId} 执行任务{devId} 英雄{string.Join(",", heroIds)}");
            }
        }
    }
    
    private void EnsureDefenseTasks(ref List<TaskPriorityInfo> tasks)
    {
        bool hasWallTask = tasks.Any(t => t.config.DevAttr1.ToLower() == "wall");
        bool hasSoldierTask = tasks.Any(t => t.config.DevAttr1.ToLower() == "soldier");
        
        foreach (var task in tasks)
        {
            if (task.config.DevAttr1.ToLower() == "wall" || 
                task.config.DevAttr1.ToLower() == "soldier" ||
                task.config.DevAttr1.ToLower() == "power")
            {
                task.adjustedPriority += 20;
            }
        }
    }
}
