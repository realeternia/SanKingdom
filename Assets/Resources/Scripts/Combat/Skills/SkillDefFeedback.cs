using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public class SkillDefFeedback : BattleSkill
{
    public SkillDefFeedback(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        DoFeedback(attacker, damType, damage);
    }

    public override void OnBeDoSkillDamage(Chess caster, BattleSkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        if(isFeedback)
            return;
        DoFeedback(caster, checkSkillCfg.Attr, damage);
    }    

    private void DoFeedback(Chess attacker, string damType, int damage)
    {
        if (skillCfg.CheckAttrs != null && !skillCfg.CheckAttrs.Contains(damType))
            return;

        if (skillCfg.Range > 0)
        {
            var isInRange = BattleManager.CheckInRange(owner.position, attacker.position, skillCfg.Range);
            if (!isInRange)
                return;
        }

        if (CheckBurst(attacker))
        {
            var damageBack = (int)(damage * skillCfg.Strength);
            attacker.DoSkillDamage(owner, skillId, damageBack, true, 0);

            this.OnPlaySkill(null, damageBack);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        BattleManager.Instance.AddBattleText("反!", owner.position, new UnityEngine.Vector2(0, 150), new UnityEngine.Color(0.65f, 0.31f, 0), 3);
    }

}
