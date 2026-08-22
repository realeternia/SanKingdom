using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 拉拽：攻击时几率将目标拉向自己。
/// 目标不在相邻格时向施法者靠近1格；已在相邻格时越过施法者拉到身后1格，保证位移始终有效。
/// </summary>
public class SkillHitPull : BattleSkill
{
    public SkillHitPull(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 城门/城墙/箭塔（UnitType 非 0）为静态建筑，不可被位移
        if (defender.isGate || defender.isWall || defender.isTower)
            return;

        if (!CheckBurst(defender))
            return;

        this.OnPlaySkill(defender, 0);

        var bm = BattleManager.Instance;
        var defCell = bm.GetMapCellById(defender.cellId);
        var ownCell = bm.GetMapCellById(owner.cellId);
        if (defCell == null || ownCell == null)
            return;

        int dist = HexUtil.HexDistance(defCell.gridX, defCell.gridZ, ownCell.gridX, ownCell.gridZ);
        // 相邻时越过施法者拉到身后1格，否则向施法者靠近1格
        int pullDistance = dist <= 1 ? 2 : 1;
        var targetPos = HexUtil.GetCellTowards(defCell.gridX, defCell.gridZ, ownCell.gridX, ownCell.gridZ, pullDistance);
        var cell = targetPos != null ? bm.GetMapCell(targetPos.Value.gx, targetPos.Value.gz) : null;
        if (cell != null && bm.CanEnterCell(cell.gridX, cell.gridZ, defender))
            bm.AddChessAction(new DisplaceAction(owner.id, bm.battleTime, defender.id, cell.id));
        else
            GameLog.Info($"SkillHitPull 目标格不可通行，位移失败 skill={skillId} tgt={defender.id}");
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        if (target != null && !string.IsNullOrEmpty(skillCfg.EffectHit))
            EffectManager.PlaySkillEffect(target, skillCfg.EffectHit);
    }
}
