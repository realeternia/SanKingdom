using System.Collections.Generic;
using CommonConfig;

public class DevelopmentStrategy : IAIStrategy
{
    public string GetStrategyName()
    {
        return "Development";
    }
    
    public void Execute(AIStrategyContext context)
    {
        foreach (var city in context.cities)
        {
            ExecuteCityDevelopment(context.player, city, context);
        }
    }
    
    private void ExecuteCityDevelopment(Player player, SaveCityData city, AIStrategyContext context)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return;
        
        var cityNeeds = CityEvaluator.EvaluateCity(city);
        var availableTasks = TaskPriorityCalculator.GetAvailableTasks(
            city, CityStrategyState.Dev, cityNeeds);
        
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
                UnityEngine.Debug.Log($"AI发展: 城市{city.cityId} 执行任务{devId} 英雄{string.Join(",", heroIds)}");
            }
        }
    }
}
