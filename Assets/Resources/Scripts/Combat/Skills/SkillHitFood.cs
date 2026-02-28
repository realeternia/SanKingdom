using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitFood : Skill
{
    public SkillHitFood(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            var foodInfo = BattleManager.Instance.GetFoodInfo(defender.forceId);
            var sub = foodInfo.food -= skillCfg.StrengthInt;
            if (sub > 0)
            {
                SkillManager.AddSkillAction(defender, null, id, sub);
                owner.PlayerAnim(skillCfg.Action);
                foodInfo.food += sub;

            }
        }
    }
    public override void OnPlaySkill(Chess target, int parm1)
    {
        target.PlayerAnim(skillCfg.Action);
        BattleManager.Instance.AddBattleText("粮-" + parm1.ToString(), target.position, new UnityEngine.Vector2(0, -30), Color.red, 3);
        BattleManager.Instance.AddBattleText("粮+" + parm1.ToString(), owner.position, new UnityEngine.Vector2(0, 60), Color.green, 3);        
    }
}
