using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonConfig;

/// <summary>
/// AI交易工具类
/// 城市士兵或粮草＜阈值时，派遣可用武将去交易补足（每人一次交易）
/// 每个城市独立判断，士兵/粮草分别处理
/// </summary>
public static class AIToolTrade
{
    /// <summary>
    /// 交易入口
    /// </summary>
    public static void Process(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        int devId = CityDevConfig.GetConfigByName("Trade").Id;
        var devCfg = CityDevConfig.GetConfig(devId);
        int goldCost = devCfg.GoldCost;
        int tradeAmount = goldCost * SystemConst.Economy.TRADE_BASE_MULTIPLIER;
        int threshold = AIConst.AIKingAction.TRADE_RESOURCE_THRESHOLD;
        int currentRound = GameManager.Instance.SaveData.round;

        var cities = force.GetCityList();
        foreach (var city in cities)
        {
            TradeResourceUntilThreshold(force, city, devId, true, goldCost, tradeAmount, threshold, excludedHeroIds, currentRound);
            TradeResourceUntilThreshold(force, city, devId, false, goldCost, tradeAmount, threshold, excludedHeroIds, currentRound);
        }
    }

    /// <summary>
    /// 交易指定资源直到达到阈值、可用武将耗尽或金钱不足
    /// </summary>
    private static void TradeResourceUntilThreshold(SaveForceData force, SaveCityData city, int devId,
        bool buySoldier, int goldCost, int tradeAmount, int threshold,
        HashSet<int> excludedHeroIds, int currentRound)
    {
        string resType = buySoldier ? "soldier" : "food";
        float currentVal = city.GetAttr(resType);
        if (currentVal >= threshold) return;

        int neededTrades = Mathf.CeilToInt((threshold - currentVal) / tradeAmount);
        var availableHeroes = GetAvailableHeroes(force, excludedHeroIds, currentRound);
        int tradeCount = Mathf.Min(neededTrades, availableHeroes.Count);

        for (int i = 0; i < tradeCount; i++)
        {
            if (force.gold < goldCost) return;
            if (city.GetAttr(resType) >= threshold) return;

            int[] heroIds = new int[] { availableHeroes[i] };
            force.ExecuteCityTrade(city.cityId, devId, heroIds, buySoldier, out var attrDatas);

            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 在 {ConfigNameHelper.GetCityName(city.cityId)} 派遣武将{availableHeroes[i]}交易 {resType}+{tradeAmount}");
        }
    }

    /// <summary>
    /// 获取本势力可用武将（未行动、非 WarPlan、Normal 状态），按统帅升序优先用低统帅
    /// </summary>
    private static List<int> GetAvailableHeroes(SaveForceData force, HashSet<int> excludedHeroIds, int currentRound)
    {
        return GameManager.Instance.GetHerosByForce(force.forceId)
            .Where(h => !excludedHeroIds.Contains(h.heroId))
            .Where(h => h.state == HeroState.Normal)
            .Where(h => h.round < currentRound)
            .OrderBy(h => h.GetAttr("leadship"))
            .Select(h => h.heroId)
            .ToList();
    }
}
