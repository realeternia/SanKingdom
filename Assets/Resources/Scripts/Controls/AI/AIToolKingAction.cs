using System.Collections.Generic;

/// <summary>
/// AI君主行动判断入口
/// 与 AIToolHeroDev（内政派遣）平行，负责除战斗外的君主行动判断
/// 在 ExecutePlanningPhase 的 AdjustForGoldBalance 之后执行
/// 流程：排除WarPlan英雄 → 移动 → 褒奖 → 登庸 → 交易
/// </summary>
public static class AIToolKingAction
{
    /// <summary>
    /// 君主行动判断入口（除战斗外均在此判断）
    /// </summary>
    public static void CheckKingAction(SaveForceData force)
    {
        // 1. 排除 WarPlan 中的英雄
        var excludedHeroIds = CollectWarPlanHeroIds(force);

        // 3. 移动：前后线战斗英雄调度 → 名将保障 → 名将均衡 → 全英雄均衡
        AIToolMove.Process(force, excludedHeroIds);

        // 2. 褒奖：忠心 ≤ 阈值的武将先褒奖
        AIToolPraise.Process(force, excludedHeroIds);

        // 4. 登庸：空闲英雄魅力/智力 > 阈值去登庸，最多 MAX_RECRUIT_COUNT 人次
        AIToolRecruit.Process(force, excludedHeroIds);

        // 5. 交易：城市士兵/粮草＜阈值且金钱充足时交易补足
        AIToolTrade.Process(force, excludedHeroIds);
    }

    /// <summary>
    /// 收集所有 WarPlan 中涉及的英雄 ID
    /// </summary>
    private static HashSet<int> CollectWarPlanHeroIds(SaveForceData force)
    {
        var heroIds = new HashSet<int>();
        foreach (var plan in force.warPlans)
        {
            if (plan.heroIds == null) continue;
            foreach (var heroId in plan.heroIds)
            {
                heroIds.Add(heroId);
            }
        }
        return heroIds;
    }
}
