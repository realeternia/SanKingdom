using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 突击·跃过：攻击时几率跃过目标落位至其身后一格。
/// 目标不相邻或落点格被占用/不可通行时位移失败，不落位至其他任意格。
/// </summary>
public class SkillJumpOver : BattleSkill
{
    public SkillJumpOver(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 城门/城墙/箭塔（UnitType 非 0）为静态建筑，不可跃过（防穿门/穿墙）
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

        // 落位到目标身后一格（沿施法者→目标方向越过目标，需相邻）
        var targetPos = HexUtil.GetCellBeyond(ownCell.gridX, ownCell.gridZ, defCell.gridX, defCell.gridZ);
        var cell = targetPos != null ? bm.GetMapCell(targetPos.Value.gx, targetPos.Value.gz) : null;
        if (cell != null && bm.CanEnterCell(cell.gridX, cell.gridZ, owner))
            bm.AddChessAction(new DisplaceAction(owner.id, bm.battleTime, owner.id, cell.id));
        else
            GameLog.Info($"SkillJumpOver 目标不相邻或落点不可通行，位移失败 skill={skillId} tgt={defender.id}");
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
