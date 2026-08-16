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

    /// <summary>
    /// 判断是否为攻击方作战单位（非防御方、非城墙/城门/箭塔）
    /// </summary>
    private static bool IsAttacker(Chess self)
    {
        return self.forceId != defenderForceId && !self.isGate && !self.isWall && !self.isTower;
    }

    /// <summary>
    /// 统计存活的城门数（含双方）
    /// </summary>
    private static int CountAliveGates()
    {
        int count = 0;
        foreach (var chess in BattleManager.Instance.chessList)
        {
            if (chess.isGate && chess.hp > 0)
                count++;
        }
        return count;
    }

    /// <summary>
    /// 查找距指定位置最近的存活城门
    /// </summary>
    private static Chess FindNearestGate(Vector3 pos)
    {
        Chess nearest = null;
        int nearestDist = int.MaxValue;
        foreach (var chess in BattleManager.Instance.chessList)
        {
            if (!chess.isGate || chess.hp <= 0) continue;
            int dist = HexUtil.WorldDistance(pos, chess.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = chess;
            }
        }
        return nearest;
    }

    // ===== 索敌 =====

    public static void FindTarget(Chess self)
    {
        if (self.attackRange == 0)
            return;

        int rangeCells = BattleManager.RangeToCells(self.attackRange);
        int aliveGates = CountAliveGates();

        // 攻击方优先破门：防御方龟缩且仍有城门存活（3 门共享血量）时，全体锁定最近的城门（集中火力）
        if (IsAttacker(self) && !defenderSally && aliveGates >= 2)
        {
            Chess nearestGate = FindNearestGate(self.position);
            if (nearestGate != null)
            {
                self.targetChessId = nearestGate.id;
                if (self.viewObj != null)
                    self.viewObj.lockTargetId = self.targetChessId;
                return;
            }
        }

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
        if (nearestDistance <= rangeCells)
            filteredTargets = validTargets.Where(t => t.distance <= rangeCells).ToList();
        else
            filteredTargets = validTargets.Where(t => t.distance <= nearestDistance + BattleManager.RangeToCells(SystemConst.Battle.TARGET_SEARCH_EXTRA_RANGE)).ToList();

        int takeCount = Mathf.Min(SystemConst.Battle.TARGET_SCORE_SELECT_COUNT, filteredTargets.Count);
        List<(Chess chess, float distance)> topTargets = filteredTargets.Take(takeCount).ToList();

        List<(Chess chess, float score)> scoredTargets = new List<(Chess, float)>();
        foreach (var (chess, distance) in topTargets)
        {
            float score = CalculateTargetScore(self, chess, distance, rangeCells, aliveGates);
            scoredTargets.Add((chess, score));
        }

        scoredTargets.Sort((a, b) => b.score.CompareTo(a.score));
        self.targetChessId = scoredTargets[0].chess.id;
        if (self.viewObj != null)
            self.viewObj.lockTargetId = self.targetChessId;
    }

    private static float CalculateTargetScore(Chess self, Chess target, float distance, int rangeCells, int aliveGates)
    {
        // 攻击方索敌规则：
        // - 城墙永远不是目标（破口通行，无需拆墙）
        // - 城门存活时门必须为最高分（sally 出击时除外，直接与出击敌军交战）
        // - 城门全破后（aliveGates<=1）剩余城门不再是目标，转打城内箭塔/敌人
        if (IsAttacker(self))
        {
            if (target.isWall)
                return float.MinValue;
            if (target.isGate)
                return (aliveGates <= 1 || defenderSally) ? float.MinValue : float.MaxValue;
            if (target.isTower && aliveGates >= 2 && !defenderSally)
                return float.MinValue;
        }

        // 城门/箭塔加分（破门后箭塔成为优先目标）
        if (target.isGate || target.isTower)
        {
            float score = SystemConst.Battle.TARGET_SCORE_GATE;
            if (distance < rangeCells * 2)
                score += 100f / (distance + 1f);
            return score;
        }
        return SysFormula.Battle.CalculateTargetScore(
            target.isHero, distance, rangeCells,
            SysFormula.Battle.CalculateDamage(self.atk, self.hp, target.def),
            self.level, target.level, (float)target.hp / target.maxHp);
    }

    // ===== 回合制入口 =====

    /// <summary>
    /// 回合制：棋子行动决策。由 Chess.OnTurnAction 调用。
    /// 攻击时设置 self.hasPendingAction=true，其余情况由调用方结束回合。
    /// </summary>
    public static void ProcessTurn(Chess self)
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

        if (SkillManager.CheckAidSkill(self))
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
            self.Attack(targetChess, self.hitEffect);
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
            var bm = BattleManager.Instance;
            targetChess = bm.GetChess(self.targetChessId);
            var (gx, gz) = bm.WorldToGridCoord(moveDest);
            var cellId = bm.GetCellId(gx, gz);
            var moveAction = new MoveAction(self.id, bm.battleTime, targetChess != null ? targetChess.id : -1, cellId);
            bm.AddChessAction(moveAction);
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
        var bm = BattleManager.Instance;
        var targetChess = bm.GetChess(self.targetChessId);
        if (targetChess == null)
            return Vector3.zero;

        var (curGx, curGz) = bm.WorldToGridCoord(self.position);
        var (tarGx, tarGz) = bm.WorldToGridCoord(targetChess.position);
        if (curGx == tarGx && curGz == tarGz)
            return Vector3.zero;

        int rangeCells = BattleManager.RangeToCells(self.attackRange);

        // 目标集合：目标射程内且可通行的格
        bool IsGoal((int gx, int gz) c)
        {
            return bm.CanEnterCell(c.gx, c.gz, self)
                && HexUtil.HexDistance(c.gx, c.gz, tarGx, tarGz) <= rangeCells;
        }

        // 可采纳启发：到目标的最短格距减去射程
        int Heuristic((int gx, int gz) c)
        {
            return Math.Max(0, HexUtil.HexDistance(c.gx, c.gz, tarGx, tarGz) - rangeCells);
        }

        // 邻格扩展：可通行格 cost=1；友方城门生成 cost=2 的跳边（门后格）
        IEnumerable<((int gx, int gz) cell, int cost)> Expand((int gx, int gz) c)
        {
            foreach (var (ngx, ngz) in HexUtil.GetNeighbors(c.gx, c.gz))
            {
                if (bm.CanEnterCell(ngx, ngz, self))
                {
                    yield return ((ngx, ngz), 1);
                    continue;
                }
                if (!bm.IsFriendlyGateCell(ngx, ngz, self))
                    continue;
                var beyond = HexUtil.GetCellBeyond(c.gx, c.gz, ngx, ngz);
                if (beyond != null && bm.CanEnterCell(beyond.Value.gx, beyond.Value.gz, self))
                    yield return ((beyond.Value.gx, beyond.Value.gz), 2);
            }
        }

        var path = AStarPathfinding.FindPath((curGx, curGz), IsGoal, Expand, Heuristic);
        if (path == null || path.Count < 2)
            return Vector3.zero;

        var next = path[1];
        return bm.GridCoordToWorld(next.gx, next.gz, self.position.y);
    }
}
