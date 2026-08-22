using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 击退：攻击时几率将目标沿攻击方向击退 skillCfg.Strength 格。
/// 目标格不可通行时位移失败（仅结算伤害），BuffId>0 时附加控制效果。
/// </summary>
public class SkillHitKnockBack : BattleSkill
{
    public SkillHitKnockBack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 城门/城墙/箭塔（UnitType 非 0）为静态建筑，不可被位移，仅结算伤害/Buff
        if (defender.isGate || defender.isWall || defender.isTower)
            return;

        if (!CheckBurst(defender))
            return;

        this.OnPlaySkill(defender, 0);

        {
            var bm = BattleManager.Instance;
            var defCell = bm.GetMapCellById(defender.cellId);
            var ownCell = bm.GetMapCellById(owner.cellId);
            if (defCell != null && ownCell != null)
            {
                int knockDistance = Mathf.Max(1, (int)skillCfg.Strength);
                // 逐格推进：路径中任何一格不可通行（含墙/塔/城门/占格）则位移失败，防瞬移穿墙
                int targetCellId = 0;
                for (int i = 1; i <= knockDistance; i++)
                {
                    var step = HexUtil.GetCellTowards(ownCell.gridX, ownCell.gridZ, defCell.gridX, defCell.gridZ, i);
                    if (step == null)
                    {
                        targetCellId = 0;
                        break;
                    }
                    var cell = bm.GetMapCell(step.Value.gx, step.Value.gz);
                    if (cell == null || !bm.CanEnterCell(cell.gridX, cell.gridZ, defender))
                    {
                        targetCellId = 0;
                        break;
                    }
                    targetCellId = cell.id;
                }
                if (targetCellId > 0)
                    bm.AddChessAction(new DisplaceAction(owner.id, bm.battleTime, defender.id, targetCellId));
                else
                    GameLog.Info($"SkillHitKnockBack 路径存在不可通行格，位移失败 skill={skillId} tgt={defender.id}");
            }
        }

        // 额外伤害（位移成败均结算）
        if (skillCfg.SkillDamageRate > 0)
            defender.DoSkillDamage(owner, skillId, (int)(damage * skillCfg.SkillDamageRate), false, 0);

        // 附加控制（如铁骑冲锋晕眩）
        if (skillCfg.BuffId > 0)
            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        if (target != null && !string.IsNullOrEmpty(skillCfg.EffectHit))
            EffectManager.PlaySkillEffect(target, skillCfg.EffectHit);
    }
}
