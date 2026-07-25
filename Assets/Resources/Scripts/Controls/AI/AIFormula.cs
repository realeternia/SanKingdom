using System;

/// <summary>
/// AI策略公式
/// </summary>
public static class AIFormula
{
    public static float CalculateAdvantageRatio(int mySoldier, int targetSoldier)
    {
        return mySoldier > 0 ? (float)mySoldier / Math.Max(1, targetSoldier) : 0;
    }

    public static int CalculateEffectiveSoldier(int citySoldier, int heroCount)
    {
        int maxSoldierByHeroes = (heroCount - 1) * AIConst.AIStrategy.MAX_SOLDIER_PER_HERO;
        return Math.Min(citySoldier, maxSoldierByHeroes);
    }

    public static bool CheckOwnCityAttackAdvantage(int mySoldier, int targetSoldier)
    {
        return mySoldier >= targetSoldier * AIConst.AIStrategy.AI_OWN_CITY_ATTACK_ADVANTAGE_RATIO;
    }

    public static bool CheckAttackFoodSufficient(int soldier, int food)
    {
        return food >= soldier / AIConst.AIStrategy.AI_ATTACK_FOOD_DIVISOR;
    }

    public static bool HasThreat(int enemySoldier)
    {
        return enemySoldier >= AIConst.AIStrategy.AI_THREAT_ENEMY_SOLDIER_THRESHOLD;
    }

    public static int CalculateFoodNeeded(int totalSoldier)
    {
        return totalSoldier / AIConst.AIStrategy.AI_FOOD_NEED_DIVISOR;
    }

    public static int CalculateTroopLimit(int commanderCount, int heroCount, int citySoldier)
    {
        int limitByCommander = commanderCount;
        int soldierPerCorps = CalculateSoldierPerCorps(heroCount);
        int limitBySoldier = citySoldier / soldierPerCorps;
        int hardLimit = AIConst.AIStrategy.TROOP_CITY_HARD_LIMIT;

        return Math.Max(0, Math.Min(hardLimit, Math.Min(limitByCommander, limitBySoldier)));
    }

    /// <summary>
    /// 梯度计算每个军团所需士兵数：武将越多，每个军团所需士兵越少
    /// ≤6武将=50，7~11武将线性递减至30，11+武将保持30
    /// </summary>
    public static int CalculateSoldierPerCorps(int heroCount)
    {
        int baseValue = AIConst.AIStrategy.TROOP_SOLDIER_PER_CORPS;
        int minValue = AIConst.AIStrategy.TROOP_SOLDIER_PER_CORPS_RELAXED;
        int startThreshold = AIConst.AIStrategy.TROOP_HERO_RICH_THRESHOLD;
        int endThreshold = AIConst.AIStrategy.TROOP_HERO_FULL_RICH_THRESHOLD;

        if (heroCount <= startThreshold) return baseValue;
        if (heroCount >= endThreshold) return minValue;

        int range = baseValue - minValue;
        int steps = endThreshold - startThreshold;
        int progress = heroCount - startThreshold;

        return baseValue - progress * range / steps;
    }

    /// <summary>
    /// 计算登庸目标优先级分数
    /// 优先级：1日名将 > 2日名将 > 1日普通 > 2日普通
    /// 忠诚越低优先级系数越高
    /// </summary>
    public static int CalculateRecruitPriority(int dayDistance, bool isStarHero, int loyalty)
    {
        // 组别基础分：1日名将 > 2日名将 > 1日普通 > 2日普通（间距10000确保组别优先于忠诚差异）
        int groupBase;
        if (dayDistance <= 1 && isStarHero)
            groupBase = 40000;
        else if (dayDistance <= 2 && isStarHero)
            groupBase = 30000;
        else if (dayDistance <= 1)
            groupBase = 20000;
        else
            groupBase = 10000;

        // 忠诚越低系数越高（0~100）
        int loyaltyBonus = (100 - loyalty) * 10;

        return groupBase + loyaltyBonus;
    }
}
