using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitFood : BattleSkill
{
    public SkillHitFood(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
        }
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        BattleManager.Instance.AddBattleText("粮-" + parm1.ToString(), target.position, new UnityEngine.Vector2(0, -30), SysColor.Battle.FoodLossColor, 3);
        BattleManager.Instance.AddBattleText("粮+" + parm1.ToString(), owner.position, new UnityEngine.Vector2(0, 60), SysColor.Battle.FoodGainColor, 3);        
    }
}
