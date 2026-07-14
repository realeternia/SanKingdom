using System.Collections.Generic;

public enum GameEventType
{
    BattleAttack,      // 战斗-进攻方（开始时记录 herolist）
    BattleDefend,      // 战斗-防御方（开始时记录 herolist）
    BattleResult,      // 战斗结果（结束时补记胜负）
    Dev,               // 委派（回合末 diff 记录；intParam: 0=assign 1=cancel 2=change）
    KingActionMove,    // 移动英雄（intParam=目标城市 ID）
    KingActionTrade,   // 交易（intParam: 1=买兵 0=卖粮）
    KingActionSearch,  // 搜索
    KingActionRecruit, // 登庸（intParam: 1=成功 0=失败）
    KingActionPraise,  // 赏赐（intParam=methodId）
    KingActionDestroy, // 破坏敌方城防（intParam=目标城市ID, effectValue=城防降低总量）
    KingActionDisturb, // 扰乱敌方民心/忠心（intParam=目标城市ID, effectValue=民心降低总量, effectValue2=忠心降低总量）
    Capture,           // 被俘虏
    Wild,              // 下野（新发现英雄初始状态）
    Escape,            // 逃脱（intParam=目标城市 ID）
    RecruitSuccess,    // 招募成功（wild/catched → normal，force 变更）
    Fair               // 天灾事件（effectValue=fairId, cityIds存储受影响城市）
}

[System.Serializable]
public class GameEventData
{
    public int eventId;
    public GameEventType eventType;
    public int year;
    public int round;
    public int forceId;
    public int relatedForceId;
    public int cityId;
    public List<int> cityIds = new List<int>();
    public List<int> heroIds = new List<int>();
    public List<int> relatedHeroIds = new List<int>();
    public int devId;
    public int intParam;
    public int effectValue;
    public int effectValue2;

    // effectValue / effectValue2 语义表（按 eventType）：
    // BattleAttack   = 0 / 0
    // BattleDefend   = 0 / 0
    // BattleResult   = 0 / 0
    // Dev            = 0 / 0
    // KingActionMove = 0 / 0
    // KingActionTrade= totalGain / 0
    // KingActionSearch= searchResultType(0=无 1=cityattr 2=forceattr 3=findhero 4=findherostar) / 资源总量
    // KingActionRecruit= 0 / 0
    // KingActionPraise= totalLoyaltyAdd / 0
    // KingActionDestroy= wallReduceTotal / 0（intParam=目标城市ID）
    // KingActionDisturb= happyReduceTotal / loyaltyReduceTotal（intParam=目标城市ID）
    // Capture        = 0 / 0
    // Wild           = 0 / 0
    // Escape         = 0 / 0
    // RecruitSuccess = 0 / 0
    // Fair           = fairId / 0

    private static int CalcYear(int round)
    {
        return (int)SysFormula.Game.CalculateCurrentYear(round);
    }

    public static GameEventData CreateBattleAttack(int round, int srcForceId, int destForceId, int targetCityId, List<int> attackerHeroIds)
    {
        return new GameEventData
        {
            eventType = GameEventType.BattleAttack,
            year = CalcYear(round),
            round = round,
            forceId = srcForceId,
            relatedForceId = destForceId,
            cityId = targetCityId,
            heroIds = attackerHeroIds != null ? new List<int>(attackerHeroIds) : new List<int>()
        };
    }

    public static GameEventData CreateBattleDefend(int round, int destForceId, int srcForceId, int targetCityId, List<int> defenderHeroIds)
    {
        return new GameEventData
        {
            eventType = GameEventType.BattleDefend,
            year = CalcYear(round),
            round = round,
            forceId = destForceId,
            relatedForceId = srcForceId,
            cityId = targetCityId,
            heroIds = defenderHeroIds != null ? new List<int>(defenderHeroIds) : new List<int>()
        };
    }

    public static GameEventData CreateBattleResult(int round, int srcForceId, int destForceId, int targetCityId, List<int> attackerHeroIds, List<int> defenderHeroIds, bool attackerWin)
    {
        return new GameEventData
        {
            eventType = GameEventType.BattleResult,
            year = CalcYear(round),
            round = round,
            forceId = srcForceId,
            relatedForceId = destForceId,
            cityId = targetCityId,
            heroIds = attackerHeroIds != null ? new List<int>(attackerHeroIds) : new List<int>(),
            relatedHeroIds = defenderHeroIds != null ? new List<int>(defenderHeroIds) : new List<int>(),
            intParam = attackerWin ? 1 : 0
        };
    }

    public static GameEventData CreateDev(int round, int forceId, int cityId, int heroId, int devId, int action)
    {
        return new GameEventData
        {
            eventType = GameEventType.Dev,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = new List<int> { heroId },
            devId = devId,
            intParam = action
        };
    }

    public static GameEventData CreateKingActionMove(int round, int forceId, int srcCityId, int destCityId, int[] heroIds)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionMove,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = srcCityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            intParam = destCityId
        };
    }

    public static GameEventData CreateKingActionTrade(int round, int forceId, int cityId, int devId, int[] heroIds, bool buySoldier, int totalGain)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionTrade,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            devId = devId,
            intParam = buySoldier ? 1 : 0,
            effectValue = totalGain
        };
    }

    public static GameEventData CreateKingActionSearch(int round, int forceId, int cityId, int devId, int[] heroIds, int searchResultType, int totalResourceAmount, List<int> discoveredHeroIds)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionSearch,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            relatedHeroIds = discoveredHeroIds != null ? new List<int>(discoveredHeroIds) : new List<int>(),
            devId = devId,
            effectValue = searchResultType,
            effectValue2 = totalResourceAmount
        };
    }

    public static GameEventData CreateKingActionRecruit(int round, int forceId, int cityId, int[] myHeroIds, int[] targetHeroIds, bool success)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionRecruit,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = myHeroIds != null ? new List<int>(myHeroIds) : new List<int>(),
            relatedHeroIds = targetHeroIds != null ? new List<int>(targetHeroIds) : new List<int>(),
            intParam = success ? 1 : 0
        };
    }

    public static GameEventData CreateKingActionPraise(int round, int forceId, int cityId, int devId, int[] heroIds, int methodId, int totalLoyaltyAdd)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionPraise,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            devId = devId,
            intParam = methodId,
            effectValue = totalLoyaltyAdd
        };
    }

    public static GameEventData CreateKingActionDestroy(int round, int forceId, int cityId, int targetCityId, int devId, int[] heroIds, int totalWallReduce)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionDestroy,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            devId = devId,
            intParam = targetCityId,
            effectValue = totalWallReduce
        };
    }

    public static GameEventData CreateKingActionDisturb(int round, int forceId, int cityId, int targetCityId, int devId, int[] heroIds, int totalHappyReduce, int totalLoyaltyReduce)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionDisturb,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            devId = devId,
            intParam = targetCityId,
            effectValue = totalHappyReduce,
            effectValue2 = totalLoyaltyReduce
        };
    }

    public static GameEventData CreateCapture(int round, int captorForceId, int loserForceId, int cityId, int heroId)
    {
        return new GameEventData
        {
            eventType = GameEventType.Capture,
            year = CalcYear(round),
            round = round,
            forceId = captorForceId,
            relatedForceId = loserForceId,
            cityId = cityId,
            heroIds = new List<int> { heroId }
        };
    }

    public static GameEventData CreateWild(int round, int discovererForceId, int cityId, int heroId)
    {
        return new GameEventData
        {
            eventType = GameEventType.Wild,
            year = CalcYear(round),
            round = round,
            forceId = discovererForceId,
            cityId = cityId,
            heroIds = new List<int> { heroId }
        };
    }

    public static GameEventData CreateEscape(int round, int heroForceId, int srcCityId, int destCityId, int heroId)
    {
        return new GameEventData
        {
            eventType = GameEventType.Escape,
            year = CalcYear(round),
            round = round,
            forceId = heroForceId,
            cityId = srcCityId,
            heroIds = new List<int> { heroId },
            intParam = destCityId
        };
    }

    public static GameEventData CreateRecruitSuccess(int round, int newForceId, int oldForceId, int cityId, int heroId)
    {
        return new GameEventData
        {
            eventType = GameEventType.RecruitSuccess,
            year = CalcYear(round),
            round = round,
            forceId = newForceId,
            relatedForceId = oldForceId,
            cityId = cityId,
            heroIds = new List<int> { heroId }
        };
    }

    public static GameEventData CreateFair(int round, int fairId, List<int> cityIds)
    {
        return new GameEventData
        {
            eventType = GameEventType.Fair,
            year = CalcYear(round),
            round = round,
            cityIds = cityIds != null ? new List<int>(cityIds) : new List<int>(),
            effectValue = fairId
        };
    }
}
