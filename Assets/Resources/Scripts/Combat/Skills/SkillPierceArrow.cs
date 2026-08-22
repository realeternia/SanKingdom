using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 贯矢：攻击时几率射出穿透箭，沿攻击方向直线穿透最多 skillCfg.Range 格。
/// 每个命中目标造成递减伤害（首段 SkillDamageRate，每格递减 SkillDamageAttrRate 系数，均走配置）。
/// 城墙/箭塔/城门阻挡穿透。
/// </summary>
public class SkillPierceArrow : BattleSkill
{
    public SkillPierceArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (!CheckBurst(defender))
            return;

        this.OnPlaySkill(defender, 0);

        var bm = BattleManager.Instance;
        var defCell = bm.GetMapCellById(defender.cellId);
        var ownCell = bm.GetMapCellById(owner.cellId);
        if (defCell == null || ownCell == null)
            return;

        var dir = HexUtil.GetUnitDirection(ownCell.gridX, ownCell.gridZ, defCell.gridX, defCell.gridZ);
        if (dir == null)
            return;

        int udq = dir.Value.dq;
        int udr = dir.Value.dr;
        int tq = defCell.gridX;
        int tr = defCell.gridZ - (defCell.gridX - (defCell.gridX & 1)) / 2;

        // 从目标格起沿攻击方向依次检查最多 Range 格
        for (int i = 0; i < skillCfg.Range; i++)
        {
            int bq = tq + udq * i;
            int br = tr + udr * i;
            int bgx = bq;
            int bgz = br + (bq - (bq & 1)) / 2;
            var cell = bm.GetMapCell(bgx, bgz);
            if (cell == null)
                break;

            int blockerId = cell.chessId;
            if (blockerId <= 0)
                continue;

            var blocker = bm.GetChess(blockerId);
            if (blocker == null || blocker.hp <= 0)
                continue;

            // 城墙/箭塔/城门阻挡穿透
            if (blocker.isWall || blocker.isTower || blocker.isGate)
                break;
            // 友方单位阻挡穿透（只对直线敌人造成伤害）
            if (blocker.forceId == owner.forceId)
                break;

            // 命中单位：伤害逐格递减
            float decay = Mathf.Pow(skillCfg.SkillDamageAttrRate, i);
            int pierceDamage = (int)(damage * skillCfg.SkillDamageRate * decay);
            if (pierceDamage > 0)
                blocker.DoSkillDamage(owner, skillId, pierceDamage, false, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
