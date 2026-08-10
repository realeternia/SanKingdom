using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitBuff : BattleSkill
{
    public SkillHitBuff(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            this.OnPlaySkill(defender, 0);
            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime);
        }
    }

    public override void OnPlaySkill(Chess targetUnit, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }

}
