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
        if(!BattleManager.CheckInRange(owner.position, attacker.position, skillCfg.Range) && CheckBurst(attacker))
        {
            this.OnPlaySkill(attacker, 0);

            Vector3 direction = (attacker.position - owner.position).normalized;
            Vector3 randomPosition = attacker.position - direction * SystemConst.Battle.TELEPORT_DISTANCE * SystemConst.Battle.GRID_CELL_SIZE;

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
