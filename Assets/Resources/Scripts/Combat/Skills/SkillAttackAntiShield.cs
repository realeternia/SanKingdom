using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackAntiShield : BattleSkill
{
    public SkillAttackAntiShield(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {
        var buff = defender.GetBuff(skillCfg.BuffId);
        if (buff != null)
        {
            var shield = buff as BuffShield;
            if (shield != null)
            {
                this.OnPlaySkill(null, 0);
                
                shield.SubHp((int)(damageBase * skillCfg.Strength));
            }
        }
    }
    
    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
