using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitTeleport : BattleSkill
{
    public SkillHitTeleport(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        var bm = BattleManager.Instance;
        if (!bm.CheckInCellRange(owner.cellId, attacker.cellId, skillCfg.Range) && CheckBurst(attacker))
        {
            this.OnPlaySkill(attacker, 0);

            // 沿 owner→attacker 反方向传送固定距离：方向为连续量，保留世界坐标计算
            Vector3 direction = (attacker.position - owner.position).normalized;
            Vector3 randomPosition = attacker.position - direction * SystemConst.Battle.TELEPORT_DISTANCE * SystemConst.Battle.GRID_CELL_SIZE;

            // 一格一棋：目标格被城墙/城门/箭塔或他人占据时放弃传送
            var (tgx, tgz) = bm.WorldToGridCoord(randomPosition);
            if (!bm.CanEnterCell(tgx, tgz, owner))
            {
                GameLog.Warn($"SkillHitTeleport 目标格不可进入({tgx},{tgz})，跳过传送");
                return;
            }

            owner.MoveTo(randomPosition, true);
            owner.LockTarget(attacker);

            BuffManager.AddBuff(attacker, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);        
    }

}
