using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackAddDamage : BattleSkill
{
    public SkillAttackAddDamage(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {
        if(skillCfg.BuffId > 0 && !defender.HasBuff(skillCfg.BuffId))
            return;

        if(CheckBurst(defender))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);

            damageBase += skillCfg.StrengthInt;
            if(skillCfg.SkillDamageRate > 0)
                damageMulti += skillCfg.SkillDamageRate;
            effect = skillCfg.EffectHit;
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }

}
