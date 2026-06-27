using System.Collections.Generic;
using System.Linq;

/// <summary>
/// AI褒奖工具类
/// 忠心 ≤ 阈值的武将进行褒奖（methodId=1 褒奖，不花黄金）
/// </summary>
public static class AIToolPraise
{
    /// <summary>
    /// 褒奖：忠心 ≤ 阈值的武将进行褒奖
    /// </summary>
    public static void Process(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        var praiseableHeroes = new List<int>();
        foreach (var hero in GameManager.Instance.GetHerosByForce(force.forceId))
        {
            if (excludedHeroIds.Contains(hero.heroId)) continue;
            if (hero.state != HeroState.Normal) continue;
            if (hero.loyalty <= AIConst.AIKingAction.PRAISE_LOYALTY_THRESHOLD)
                praiseableHeroes.Add(hero.heroId);
        }

        if (praiseableHeroes.Count == 0) return;

        // 褒奖为势力级动作，取首个城市作为执行城市
        var firstCity = force.GetCityList().FirstOrDefault();
        if (firstCity == null) return;

        force.ExecuteCityPraiseHero(firstCity.cityId, SystemConst.CityDev.PRAISE_DEV_ID,
            praiseableHeroes.ToArray(), 1, out var attrDatas);

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 褒奖{praiseableHeroes.Count}名忠心≤{AIConst.AIKingAction.PRAISE_LOYALTY_THRESHOLD}的武将");
    }
}
