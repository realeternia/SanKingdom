using System.Collections.Generic;
using System.Linq;
using CommonConfig;

/// <summary>
/// AI登庸工具类
/// 空闲英雄魅力/智力 > 阈值去登庸，最多 MAX_RECRUIT_COUNT 人次
/// 目标排序：1日名将 > 2日名将 > 1日普通 > 2日普通，忠心越低优先级越高
/// </summary>
public static class AIToolRecruit
{
    /// <summary>
    /// 登庸入口
    /// </summary>
    public static void Process(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        // 1. 收集所有可登庸目标（在野/俘虏，且在己方城市内）
        var recruitTargets = CollectRecruitTargets(force);
        if (recruitTargets.Count == 0) return;

        // 2. 按优先级排序目标（降序）
        recruitTargets.Sort((a, b) =>
            SysFormula.AIStrategy.CalculateRecruitPriority(b.dayDistance, b.isStarHero, b.loyalty)
            .CompareTo(SysFormula.AIStrategy.CalculateRecruitPriority(a.dayDistance, a.isStarHero, a.loyalty)));

        // 3. 收集空闲英雄（无委派、未被占用、魅力或智力 > 阈值）
        var executors = CollectRecruitExecutors(force, excludedHeroIds);
        if (executors.Count == 0) return;

        // 4. 分配登庸任务（最多 MAX_RECRUIT_COUNT 人次）
        int maxCount = AIConst.AIKingAction.MAX_RECRUIT_COUNT;
        int assignedCount = 0;
        var usedExecutorIds = new HashSet<int>();

        foreach (var target in recruitTargets)
        {
            if (assignedCount >= maxCount) break;
            if (usedExecutorIds.Count >= executors.Count) break;

            var executor = executors.FirstOrDefault(e => !usedExecutorIds.Contains(e.heroId));
            if (executor == null) break;

            usedExecutorIds.Add(executor.heroId);

            force.ExecuteCityUseHero(target.cityId, SystemConst.CityDev.USE_HERO_DEV_ID,
                new int[] { executor.heroId }, new int[] { target.heroId }, out var attrDatas);

            assignedCount++;

            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 登庸 {ConfigNameHelper.GetHeroName(target.heroId)}（{target.dayDistance}日 名将:{target.isStarHero} 忠心:{target.loyalty}）执行者:{ConfigNameHelper.GetHeroName(executor.heroId)}");
        }

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 登庸完成，共{assignedCount}人次");
    }

    private class RecruitTarget
    {
        public int heroId;
        public int cityId;
        public int dayDistance;
        public bool isStarHero;
        public int loyalty;
    }

    private class RecruitExecutor
    {
        public int heroId;
        public int cityId;
    }

    /// <summary>
    /// 收集所有可登庸目标（在野/俘虏，位于己方城市）
    /// </summary>
    private static List<RecruitTarget> CollectRecruitTargets(SaveForceData force)
    {
        var targets = new List<RecruitTarget>();
        var myCityIds = new HashSet<int>(force.GetCityList().Select(c => c.cityId));
        var kingCity = force.GetKingCity();
        int kingCityId = kingCity != null ? kingCity.cityId : 0;

        foreach (var hero in GameManager.Instance.SaveData.heros)
        {
            if (hero.state != HeroState.Wild && hero.state != HeroState.Catched) continue;
            if (!myCityIds.Contains(hero.cityId)) continue;

            var heroCfg = HeroConfig.GetConfig(hero.heroId);
            int dayDistance = kingCityId > 0
                ? SysFormula.City.CalculateHeroDayDistance(kingCityId, hero.cityId, false)
                : SystemConst.CityDev.CITY_DAY_MAX;

            targets.Add(new RecruitTarget
            {
                heroId = hero.heroId,
                cityId = hero.cityId,
                dayDistance = dayDistance,
                isStarHero = heroCfg.StarHero,
                loyalty = hero.loyalty
            });
        }

        return targets;
    }

    /// <summary>
    /// 收集空闲英雄（无委派、未被占用、魅力或智力 > 阈值）
    /// </summary>
    private static List<RecruitExecutor> CollectRecruitExecutors(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        var executors = new List<RecruitExecutor>();
        int currentRound = GameManager.Instance.SaveData.round;

        foreach (var city in force.GetCityList())
        {
            var normalHeroes = city.GetNormalHeroList();
            var assignedHeroIds = new HashSet<int>(city.GetDevAssignments().Select(a => a.heroId));

            foreach (var heroId in normalHeroes)
            {
                if (excludedHeroIds.Contains(heroId)) continue;
                if (assignedHeroIds.Contains(heroId)) continue;

                var hero = GameManager.Instance.GetHero(heroId);
                if (hero == null) continue;
                // 已被占用（如上一轮登庸尚未返回）
                if (hero.round >= currentRound) continue;

                if (hero.charm > AIConst.AIKingAction.RECRUIT_ATTR_THRESHOLD
                    || hero.inte > AIConst.AIKingAction.RECRUIT_ATTR_THRESHOLD)
                {
                    executors.Add(new RecruitExecutor { heroId = heroId, cityId = city.cityId });
                }
            }
        }

        return executors;
    }
}
