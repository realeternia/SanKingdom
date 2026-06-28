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

        // 决定各城市战略状态（攻击/防御/发展）并执行
        var cityStrategies = StrategicDecider.DetermineCityStrategies(force, context);
        
        foreach (var strategy in cityStrategies.Values)
        {
            strategy.Execute();
        }
        
        // 金钱为负则逐步移除收益最低的发展委派
        AIToolHeroDev.AdjustForGoldBalance(force);

        // 君主行动判断（褒奖/移动/登庸，除战斗外的判断均在此进行）
        AIToolKingAction.CheckKingAction(force);

        // KingAction 调动的英雄导致城市dev位空缺，重新分配填充
        AIToolHeroDev.AssignHeroesToDev(force);

        // 确认计划，进入执行阶段
        GameManager.Instance.ConfirmPlan(force.forceId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 计划阶段完成");
    }
}
