using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillInitAddRege : BattleSkill
{
    public SkillInitAddRege(int id, Chess unit) : base(id, unit)
    {
    }

    public override void BattleBegin()
    {
        owner.regeHp += skillCfg.StrengthInt;
        this.OnPlaySkill(null, 0);
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }


}
