using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public class SkillDefSkillDamageReduce : BattleSkill
{
    public SkillDefSkillDamageReduce(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnBeDoSkillDamage(Chess caster, BattleSkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(checkSkillCfg.Attr))
            return;

        if (CheckBurst(caster))
        {
            damage = (int)(damage * skillCfg.Strength);
        }
    }    

}
