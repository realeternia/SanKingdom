using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitAttr : BattleSkill
{
    public SkillHitAttr(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            var roll = Random.Range(0, 3);
            var attr = roll == 0 ? "inte" : (roll == 1 ? "str" : "leadShip");
            owner.AddAttr(attr, skillCfg.StrengthInt);
            this.OnPlaySkill(defender, 0);

        }
    }

    public override void OnPlaySkill(Chess targetUnit, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);  
        EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);        
    }

}
