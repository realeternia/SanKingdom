using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddRege : Skill
{
    public SkillInitAddRege(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.regeHp += skillCfg.StrengthInt;
        SkillManager.AddSkillAction(owner, null, id, 0);
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }


}
