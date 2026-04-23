using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;

public static class AI
{
    public static void ExecutePlanningPhase(SaveForceData force)
    {
        StrategicDecider.ClearRoundData();
        
        var context = new AIStrategyContext(force);
        
        HeroDispatcher.DispatchHeroes(force);
        
        var cityStrategies = StrategicDecider.DetermineCityStrategies(force);
        
        GenerateWarPlans(force, context, cityStrategies);
        
        GameManager.Instance.ConfirmPlan(force.forceId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 计划阶段完成");
    }
    
    private static void GenerateWarPlans(SaveForceData force, AIStrategyContext context, Dictionary<int, CityStrategyState> cityStrategies)
    {
        foreach (var city in context.cities)
        {
            var state = cityStrategies.ContainsKey(city.cityId) ? 
                cityStrategies[city.cityId] : CityStrategyState.Dev;
            
            if (state == CityStrategyState.Atk)
            {
                var attackTarget = StrategicDecider.GetAttackTarget(city.cityId);
                if (attackTarget.HasValue)
                {
                    TryCreateWarPlan(force, city, attackTarget.Value);
                }
            }
        }
    }
    
    private static void TryCreateWarPlan(SaveForceData force, SaveCityData city, int targetCityId)
    {
        var normalHeroes = city.GetNormalHeroList();
        if (normalHeroes.Count == 0)
            return;
        
        var combatHeroes = normalHeroes
            .Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null && HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();
        
        if (combatHeroes.Count == 0)
        {
            combatHeroes = normalHeroes
                .Select(id => GameManager.Instance.GetHero(id))
                .Where(h => h != null)
                .Take(3)
                .ToList();
        }
        
        if (combatHeroes.Count == 0)
            return;
        
        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        var heroSoldierDict = city.DistributeSoldierDefault(heroIds);
        
        int totalSoldier = heroSoldierDict.Values.Sum();
        
        if (totalSoldier < SystemConst.AIStrategy.AI_MIN_ATTACK_SOLDIER)
            return;
        
        int foodNeeded = SysFormula.AIStrategy.CalculateFoodNeeded(totalSoldier);
        
        if (city.food < foodNeeded)
            return;
        
        var warPlan = new WarPlanData
        {
            forceId = force.forceId,
            sourceCityId = city.cityId,
            targetCityId = targetCityId,
            heroIds = heroIds,
            foodCost = foodNeeded,
            heroSoldierDict = heroSoldierDict,
            heroArmsDict = new Dictionary<int, int>()
        };
        
        force.AddWarPlan(warPlan);
        
        StrategicDecider.MarkTargetAttacked(force.forceId, targetCityId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 计划攻击[{ConfigNameHelper.GetCityName(targetCityId)}] 英雄:[{ConfigNameHelper.GetHeroNames(heroIds)}] 兵力:{totalSoldier}");
    }
}
