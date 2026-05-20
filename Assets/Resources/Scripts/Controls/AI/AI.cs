using System.Collections.Generic;
using CommonConfig;

public static class AI
{
    public static void ExecutePlanningPhase(SaveForceData force)
    {
        StrategicDecider.ClearRoundData();
        
        var context = new AIStrategyContext(force);
        
        HeroDispatcher.DispatchHeroes(force);
        
        var cityStrategies = StrategicDecider.DetermineCityStrategies(force, context);
        
        foreach (var strategy in cityStrategies.Values)
        {
            strategy.Execute();
        }
        
        GameManager.Instance.ConfirmPlan(force.forceId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 计划阶段完成");
    }
}
