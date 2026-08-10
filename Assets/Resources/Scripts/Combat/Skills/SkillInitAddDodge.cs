using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddDodge : BattleSkill
{
    public SkillInitAddDodge(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.dodgeRate += skillCfg.Strength;
        this.OnPlaySkill(null, 0);
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }


}
