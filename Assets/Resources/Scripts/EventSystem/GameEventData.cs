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
    Capture,           // 被俘虏
    Wild,              // 下野（新发现英雄初始状态）
    Escape,            // 逃脱（intParam=目标城市 ID）
    RecruitSuccess     // 招募成功（wild/catched → normal，force 变更）
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
    public List<int> heroIds = new List<int>();
    public List<int> relatedHeroIds = new List<int>();
    public int devId;
    public int intParam;

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

    public static GameEventData CreateKingActionTrade(int round, int forceId, int cityId, int devId, int[] heroIds, bool buySoldier)
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
            intParam = buySoldier ? 1 : 0
        };
    }

    public static GameEventData CreateKingActionSearch(int round, int forceId, int cityId, int devId, int[] heroIds)
    {
        return new GameEventData
        {
            eventType = GameEventType.KingActionSearch,
            year = CalcYear(round),
            round = round,
            forceId = forceId,
            cityId = cityId,
            heroIds = heroIds != null ? new List<int>(heroIds) : new List<int>(),
            devId = devId
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

    public static GameEventData CreateKingActionPraise(int round, int forceId, int cityId, int devId, int[] heroIds, int methodId)
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
            intParam = methodId
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
}
