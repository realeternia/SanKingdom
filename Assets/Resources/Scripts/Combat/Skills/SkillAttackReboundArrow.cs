using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackReboundArrow : Skill
{
    public SkillAttackReboundArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        var unitsInRange = BattleManager.Instance.GetUnitsInRange(defender.position, skillCfg.Range, owner.forceId, true);
        unitsInRange.Remove(defender);

        if (unitsInRange.Count > 0 && CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);

            var reboundDamage = (int)(damage * skillCfg.SkillDamageRate);
            foreach (var unit in unitsInRange)
                BattleManager.Instance.CreateSpellMissile(owner, unit, defender.position, id, reboundDamage);
        }
    }
}
