using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddDodge : Skill
{
    public SkillInitAddDodge(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.dodgeRate += skillCfg.Strength;
        SkillManager.AddSkillAction(owner, null, id, 0);
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }


}
