using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 战斗 AI：统一负责棋子的索敌、移动、攻击决策，以及防御方策略。
/// 防御方策略：
///   - 主动出击(sally)：单位往外移动攻击，通过城门出击
///   - 龟缩防守(!sally)：不越过城墙；远程兵种贴墙索敌并隔墙攻击，近战原地待命
/// 城门被破后龟缩自动解除（IsDefenderHolding 实时检查存活城门）。
/// noMoveCount/noActionCount 字段保留，仅供 Buff 系统（BuffNoMove/BuffNoAction）使用。
/// </summary>
public static class ChessAI
{
    private static int defenderForceId = -1;
    private static bool defenderSally = false;
    private static bool battleStarted = false;

    public static void Init(int defenderForceId)
    {
        ChessAI.defenderForceId = defenderForceId;
        ChessAI.defenderSally = false;
        ChessAI.battleStarted = false;
    }

    public static void Reset()
    {
        defenderForceId = -1;
        defenderSally = false;
        battleStarted = false;
    }

    public static void SetBattleStarted()
    {
        battleStarted = true;
    }

    /// <summary>
    /// 根据双方战力决定防御方策略：防御方战力 > 攻击方战力 * DEFENDER_SALLY_POWER_RATIO 时主动出击
    /// </summary>
    public static void DecideDefenderStrategy(List<SaveTroopsData> attackTroops, List<SaveTroopsData> defenderTroops,
        Dictionary<int, int> attackSoldierMap, Dictionary<int, int> defenderSoldierMap)
    {
        long atkPower = SysFormula.Battle.CalculateForcePower(attackTroops, attackSoldierMap);
        long defPower = SysFormula.Battle.CalculateForcePower(defenderTroops, defenderSoldierMap);
        defenderSally = defPower > (long)(atkPower * SystemConst.Battle.DEFENDER_SALLY_POWER_RATIO);
        GameLog.Info($"ChessAI.DecideDefenderStrategy atkPower={atkPower} defPower={defPower} sally={defenderSally}");
    }

    /// <summary>
    /// 判断棋子是否为防御方单位（不含城门/墙/箭塔）
    /// </summary>
    private static bool IsDefender(Chess chess)
    {
        return chess.forceId == defenderForceId && !chess.isGate && !chess.isWall && !chess.isTower;
    }

    /// <summary>
    /// 判断防御方是否处于龟缩状态：非 sally 模式 且 仍有存活的己方城门。
    /// 城门被破后自动返回 false，无需外部解冻调用。
    /// </summary>
    private static bool IsDefenderHolding()
    {
        if (defenderSally) return false;
        foreach (var chess in BattleManager.Instance.chessList)
        {
            if (chess.isGate && chess.hp > 0 && chess.forceId == defenderForceId)
                return true;
        }
        return false;
    }

    // ===== 索敌 =====

    public static void FindTarget(Chess self)
    {
        if (self.attackRange == 0)
            return;

        var allChess = BattleManager.Instance.GetUnitsInRange(self.position, 0, self.forceId, true);
        List<(Chess chess, float distance)> validTargets = new List<(Chess, float)>();

        foreach (Chess chess in allChess)
        {
            if (chess != self)
            {
                float distance = BattleManager.GetRange(self.position, chess.position);
                validTargets.Add((chess, distance));
            }
        }

        if (validTargets.Count == 0)
        {
            self.targetChessId = 0;
            return;
        }

        validTargets.Sort((a, b) => a.distance.CompareTo(b.distance));

        float nearestDistance = validTargets[0].distance;
        List<(Chess chess, float distance)> filteredTargets;
        if (nearestDistance <= self.attackRange)
            filteredTargets = validTargets.Where(t => t.distance <= self.attackRange).ToList();
        else
            filteredTargets = validTargets.Where(t => t.distance <= nearestDistance + SystemConst.Battle.TARGET_SEARCH_EXTRA_RANGE).ToList();

        int takeCount = Mathf.Min(SystemConst.Battle.TARGET_SCORE_SELECT_COUNT, filteredTargets.Count);
        List<(Chess chess, float distance)> topTargets = filteredTargets.Take(takeCount).ToList();

        List<(Chess chess, float score)> scoredTargets = new List<(Chess, float)>();
        foreach (var (chess, distance) in topTargets)
        {
            float score = CalculateTargetScore(self, chess, distance);
            scoredTargets.Add((chess, score));
        }

        scoredTargets.Sort((a, b) => b.score.CompareTo(a.score));
        self.targetChessId = scoredTargets[0].chess.id;
        if (self.viewObj != null)
            self.viewObj.lockTargetId = self.targetChessId;
    }

    private static float CalculateTargetScore(Chess self, Chess target, float distance)
    {
        if (target.isGate || target.isTower)
        {
            float score = SystemConst.Battle.TARGET_SCORE_GATE;
            if (distance < self.attackRange * 2)
                score += 100f / (distance + 1f);
            return score;
        }
        return SysFormula.Battle.CalculateTargetScore(
            target.isHero, distance, self.attackRange,
            SysFormula.Battle.CalculateDamage(self.atk, self.hp, target.def),
            self.level, target.level, (float)target.hp / target.maxHp);
    }

    // ===== 回合制入口 =====

    /// <summary>
    /// 回合制：棋子行动决策。由 Chess.OnTurnAction 调用。
    /// 攻击时设置 self.hasPendingAction=true，其余情况由调用方结束回合。
    /// </summary>
    public static void ProcessTurn(Chess self, int tickIndex)
    {
        // Buff 系统的硬限制（BuffNoAction）
        if (self.noActionCount > 0)
            return;

        // 城门/墙是静态建筑，不参与回合行动；箭塔可射击，走后续流程
        if (self.isGate || self.isWall)
            return;

        // 战斗尚未开始时，防御方棋子不行动（原 InitSummon 临时冻结逻辑已由 ChessAI 动态判断替代）
        if (!battleStarted && IsDefender(self))
            return;

        FindTarget(self);

        var targetChess = BattleManager.Instance.GetChess(self.targetChessId);
        if (targetChess == null || targetChess.hp <= 0)
            return;

        if (SkillManager.CheckAidSkill(self, tickIndex))
            return;

        // 射程内攻击
        if (BattleManager.CheckInRange(self.position, targetChess.position, self.attackRange))
        {
            if (!self.isInAttackRange)
            {
                self.isInAttackRange = true;
                self.viewObj?.PlaySodAnim("idle");
            }
            SkillManager.AimTarget(self, targetChess);
            self.Attack(targetChess, self.hitEffect, tickIndex);
            self.hasPendingAction = true;
            return;
        }

        // 移动决策
        if (!CanMove(self))
            return;

        if (self.isInAttackRange)
        {
            self.isInAttackRange = false;
            self.viewObj?.PlaySodAnim("sodmove");
        }

        var moveDest = GetMoveDest(self);
        if (moveDest != Vector3.zero)
        {
            targetChess = BattleManager.Instance.GetChess(self.targetChessId);
            var moveAction = new MoveAction(self.id, tickIndex, targetChess != null ? targetChess.id : -1, moveDest);
            BattleManager.Instance.AddChessAction(moveAction);
        }
    }

    /// <summary>
    /// 判断棋子是否可以移动：
    /// - Buff 禁移（noMoveCount > 0，来自 BuffNoMove）→ 不能移动
    /// - moveSpeed == 0 → 不能移动
    /// - 箭塔 → 不能移动
    /// - 防御方龟缩（!sally 且仍有存活城门）→ 不能移动（不越过城墙，远程兵种贴墙索敌攻击）
    /// </summary>
    private static bool CanMove(Chess self)
    {
        if (self.noMoveCount > 0 || self.moveSpeed == 0)
            return false;
        if (self.isTower)
            return false;
        if (IsDefender(self) && IsDefenderHolding())
            return false;
        return true;
    }

    private static Vector3 GetMoveDest(Chess self)
    {
        var targetChess = BattleManager.Instance.GetChess(self.targetChessId);
        if (targetChess == null)
            return Vector3.zero;

        var bm = BattleManager.Instance;
        var (curGx, curGz) = bm.WorldToGridCoord(self.position);
        var (tarGx, tarGz) = bm.WorldToGridCoord(targetChess.position);

        int dx = tarGx - curGx;
        int dz = tarGz - curGz;

        if (dx == 0 && dz == 0)
            return Vector3.zero;

        List<(int gx, int gz, int priority)> candidates = new List<(int, int, int)>();

        if (Math.Abs(dx) >= Math.Abs(dz))
        {
            int stepX = dx > 0 ? 1 : -1;
            candidates.Add((curGx + stepX, curGz, 0));
            if (dz != 0)
            {
                int stepZ = dz > 0 ? 1 : -1;
                candidates.Add((curGx, curGz + stepZ, 1));
            }
        }
        else
        {
            int stepZ = dz > 0 ? 1 : -1;
            candidates.Add((curGx, curGz + stepZ, 0));
            if (dx != 0)
            {
                int stepX = dx > 0 ? 1 : -1;
                candidates.Add((curGx + stepX, curGz, 1));
            }
        }

        foreach (var (gx, gz, _) in candidates)
        {
            if (!bm.IsGridOccupiedByOther(gx, gz, self.id) && !bm.IsGridBlockedByObstacle(gx, gz, self.forceId))
                return bm.GridCoordToWorld(gx, gz, self.position.y);
        }

        int[] sideOffsets = { 1, -1 };
        if (Math.Abs(dx) >= Math.Abs(dz))
        {
            foreach (int offset in sideOffsets)
            {
                int newGz = curGz + offset;
                if (!bm.IsGridOccupiedByOther(curGx, newGz, self.id) && !bm.IsGridBlockedByObstacle(curGx, newGz, self.forceId))
                    return bm.GridCoordToWorld(curGx, newGz, self.position.y);
            }
        }
        else
        {
            foreach (int offset in sideOffsets)
            {
                int newGx = curGx + offset;
                if (!bm.IsGridOccupiedByOther(newGx, curGz, self.id) && !bm.IsGridBlockedByObstacle(newGx, curGz, self.forceId))
                    return bm.GridCoordToWorld(newGx, curGz, self.position.y);
            }
        }

        return Vector3.zero;
    }
}
