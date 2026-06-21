using System.Collections.Generic;
using CommonConfig;

public static class AI
{
    /// <summary>
    /// AI计划阶段总入口
    /// </summary>
    public static void ExecutePlanningPhase(SaveForceData force)
    {
        // 清除本回合攻击记录
        StrategicDecider.ClearRoundData();
        
        var context = new AIStrategyContext(force);
        
        // 英雄调度：前后线战斗英雄 → 名将保障 → 名将均衡 → 全英雄均衡
        HeroDispatcher.DispatchHeroes(force);
        
        // 决定各城市战略状态（攻击/防御/发展）并执行
        var cityStrategies = StrategicDecider.DetermineCityStrategies(force, context);
        
        foreach (var strategy in cityStrategies.Values)
        {
            strategy.Execute();
        }
        
        // 金钱为负则逐步移除收益最低的发展委派
        AIToolHeroDev.AdjustForGoldBalance(force);
        
        // 确认计划，进入执行阶段
        GameManager.Instance.ConfirmPlan(force.forceId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 计划阶段完成");
    }
}
