using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

public class SkillDefPlantSkin : Skill
{
    public SkillDefPlantSkin(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
        if (!skillCfg.CheckAttrs.Contains(damType))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);
            damageMulti += skillCfg.Strength;
        }
        else if (CheckBurst(attacker))
        {
            SkillManager.AddSkillAction(owner, null, id, 1);
            damageMulti -= skillCfg.Strength;
        }
    }

    public override void OnBeDoSkillDamage(Chess caster, SkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        if(isFeedback)
            return;

        if (!skillCfg.CheckAttrs.Contains(checkSkillCfg.Attr))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);
            damage = (int)(damage * (1 + skillCfg.Strength));
        }
        else if (CheckBurst(caster))
        {
            SkillManager.AddSkillAction(owner, null, id, 1);
            damage = (int)(damage * (1 - skillCfg.Strength));
        }
    }   

     public override void OnPlaySkill(Chess target, int parm1)
    {
        if(parm1 == 0)
        BattleManager.Instance.AddBattleText("弱点", owner.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
        else if(parm1 == 1)
        BattleManager.Instance.AddBattleText("抵抗", owner.position, new UnityEngine.Vector2(0, 60), Color.green, 3);
    }

}
