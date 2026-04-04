using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public class ExpansionStrategy : IAIStrategy
{
    public string GetStrategyName()
    {
        return "Expansion";
    }
    
    public void Execute(AIStrategyContext context)
    {
        var attackTarget = SelectBestAttackTarget(context);
        if (!attackTarget.HasValue)
        {
            ExecuteFallbackDevelopment(context);
            return;
        }
        
        var attackSource = SelectBestAttackSource(context, attackTarget.Value);
        if (!attackSource.HasValue)
        {
            ExecuteFallbackDevelopment(context);
            return;
        }
        
        ExecuteAttack(context.player, attackSource.Value, attackTarget.Value, context);
    }
    
    private int? SelectBestAttackTarget(AIStrategyContext context)
    {
        var myCityIds = new HashSet<int>(context.cities.Select(c => c.cityId));
        var potentialTargets = new List<int>();
        
        foreach (var city in context.cities)
        {
            var nearCityIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
            if (nearCityIds == null) continue;
            
            foreach (var nearId in nearCityIds)
            {
                if (myCityIds.Contains(nearId))
                    continue;
                
                var nearCity = GameManager.Instance.GetCity(nearId);
                if (nearCity != null && nearCity.forceId != context.player.forceId)
                {
                    potentialTargets.Add(nearId);
                }
            }
        }
        
        if (potentialTargets.Count == 0)
            return null;
        
        potentialTargets.Sort((a, b) => 
        {
            var cityA = GameManager.Instance.GetCity(a);
            var cityB = GameManager.Instance.GetCity(b);
            return cityA.GetAttr("soldier").CompareTo(cityB.GetAttr("soldier"));
        });
        
        return potentialTargets[0];
    }
    
    private int? SelectBestAttackSource(AIStrategyContext context, int targetCityId)
    {
        var nearCityIds = WorldConfig.GetConfig(targetCityId)?.WorldNearIds;
        if (nearCityIds == null) return null;
        
        var candidateCities = context.cities
            .Where(c => System.Array.Exists(nearCityIds, id => id == c.cityId))
            .ToList();
        
        if (candidateCities.Count == 0)
            return null;
        
        candidateCities.Sort((a, b) => 
            b.GetAttr("soldier").CompareTo(a.GetAttr("soldier")));
        
        return candidateCities[0].cityId;
    }
    
    private void ExecuteAttack(Player player, int sourceCityId, int targetCityId, AIStrategyContext context)
    {
        var sourceCity = GameManager.Instance.GetCity(sourceCityId);
        var availableHeroes = context.GetAvailableHeroes(sourceCityId);
        
        var combatHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();
        
        if (combatHeroes.Count == 0)
            combatHeroes = availableHeroes.Take(3).ToList();
        
        if (combatHeroes.Count == 0)
            return;
        
        int totalSoldier = combatHeroes.Sum(h => h.soldier);
        int foodNeeded = totalSoldier / 2;
        
        if (sourceCity.food < foodNeeded)
            foodNeeded = sourceCity.food;
        
        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        
        var battleConfig = CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Prefab == "CityDevBattle" && c.FindEnemy);
        
        if (battleConfig != null)
        {
            StrategicDecider.MarkTargetAttacked(player.forceId, targetCityId);
            player.ExecuteCityBattleDev(sourceCityId, battleConfig.Id, heroIds, foodNeeded, targetCityId, true);
            UnityEngine.Debug.Log($"AI攻击: 城市{sourceCityId} 攻击{targetCityId} 英雄{string.Join(",", heroIds)}");
        }
    }
    
    private void ExecuteFallbackDevelopment(AIStrategyContext context)
    {
        var devStrategy = new DevelopmentStrategy();
        devStrategy.Execute(context);
    }
}
