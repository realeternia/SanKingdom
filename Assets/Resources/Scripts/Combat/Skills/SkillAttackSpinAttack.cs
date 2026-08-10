using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackSpinAttack : BattleSkill
{
    public SkillAttackSpinAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            var unitsInRange = BattleManager.Instance.GetUnitsInRange(owner.position, skillCfg.Range, owner.forceId, true);
            unitsInRange.Remove(defender);
            BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach(var unit in unitsInRange)
            {
                unit.DoSkillDamage(owner, skillId, (int)(damage * skillCfg.SkillDamageRate), false, 0);
            }

            this.OnPlaySkill(null, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);
    }
}
