using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitArea : Skill
{
    public SkillHitArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            var targetPos = defender.position;
            //创建一个hitEffect
            SkillManager.AddSkillAction(owner, defender, id, 0);

            var unitsInRange = BattleManager.Instance.GetUnitsInRange(targetPos, skillCfg.Range, owner.forceId, true);
            unitsInRange.Remove(defender);
            if (unitsInRange.Count > 0)
            {
                BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage2 = (int)(damage * skillCfg.SkillDamageRate);
                if(skillCfg.SkillDamageRate > 0 && damage2 <= 0)
                    return;
                foreach(var unit in unitsInRange)
                    unit.DoSkillDamage(owner, skillId, damage2, false, 0);
            }
        }
    }

    public override void OnPlaySkill(Chess targetUnit, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(targetUnit, skillCfg.EffectHit);
    }    

}
