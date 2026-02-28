using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddCrit : Skill
{
    public SkillInitAddCrit(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.critRate += skillCfg.Strength;
        SkillManager.AddSkillAction(owner, null, id, 0);
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }


}
