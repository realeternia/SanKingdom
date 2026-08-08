using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillDefHpLow : BattleSkill
{
    public SkillDefHpLow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if (owner.HpRate < skillCfg.ConditionParm && CheckBurst(attacker))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);
            damageMulti -= skillCfg.Strength;
        }
    }

    public override void OnBeDoSkillDamage(Chess caster, BattleSkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        if(isFeedback)
            return;
        if (owner.HpRate < skillCfg.ConditionParm && CheckBurst(caster))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);
            damage = (int)(damage * (1 - skillCfg.Strength));
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        BattleManager.Instance.AddBattleText("抵抗", owner.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }



}
