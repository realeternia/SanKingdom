using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitTeleport : Skill
{
    public SkillHitTeleport(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if(!BattleManager.Instance.CheckInRange(owner.position, attacker.position, skillCfg.Range) && CheckBurst(attacker))
        {
            owner.PlayerAnim(skillCfg.Action);

            Vector3 direction = (attacker.position - owner.position).normalized;
            Vector3 randomPosition = attacker.position - direction * 12;

            owner.MoveTo(randomPosition, true);
            owner.LockTarget(attacker);
            EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);

            BuffManager.AddBuff(attacker, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

}
