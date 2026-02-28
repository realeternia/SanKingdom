using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackedBuff : Skill
{
    public SkillAttackedBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if(damage > 10 && CheckBurst(attacker))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);

            BuffManager.AddBuff(owner, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }

}
