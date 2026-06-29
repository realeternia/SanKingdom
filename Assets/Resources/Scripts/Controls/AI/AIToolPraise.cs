using System.Collections.Generic;
using System.Linq;
using CommonConfig;

/// <summary>
/// AI褒奖工具类
/// 忠心 ≤ 阈值的武将进行褒奖（methodId=1 褒奖，不花黄金）
/// 仅处理免费褒奖（21205），不管付费奖赏（21206）
/// </summary>
public static class AIToolPraise
{
    /// <summary>
    /// 褒奖：忠心 ≤ 阈值的武将进行褒奖
    /// </summary>
    public static void Process(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        int devId = SystemConst.CityDev.PRAISE_DEV_ID;
        var devCfg = CityDevConfig.GetConfig(devId);
        int currentRound = GameManager.Instance.SaveData.round;

        // 计算本回合剩余可参与人数（HeroCount=0 表示不限）
        int usedCount = force.GetKingActionCount(devId);
        int remaining = devCfg.HeroCount > 0
            ? System.Math.Max(0, devCfg.HeroCount - usedCount)
            : int.MaxValue;
        if (remaining == 0) return;

        // 忠心升序，优先褒奖忠心最低的武将（忠心≥阈值的不褒奖）
        var praiseableHeroes = GameManager.Instance.GetHerosByForce(force.forceId)
            .Where(h => !excludedHeroIds.Contains(h.heroId))
            .Where(h => h.state == HeroState.Normal)
            .Where(h => h.round < currentRound)
            .Where(h => h.loyalty < AIConst.AIKingAction.PRAISE_LOYALTY_THRESHOLD)
            .OrderBy(h => h.loyalty)
            .Take(remaining)
            .Select(h => h.heroId)
            .ToList();

        if (praiseableHeroes.Count == 0) return;

        // 褒奖为势力级动作，取首个城市作为执行城市
        var firstCity = force.GetCityList().FirstOrDefault();
        if (firstCity == null) return;

        force.ExecuteCityPraiseHero(firstCity.cityId, devId,
            praiseableHeroes.ToArray(), out var attrDatas);

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 褒奖{praiseableHeroes.Count}名忠心<{AIConst.AIKingAction.PRAISE_LOYALTY_THRESHOLD}的武将");
    }
}
