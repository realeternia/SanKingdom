using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackMultiArrow : Skill
{
    public SkillAttackMultiArrow(int id, Chess unit) : base(id, unit)
    {
    }

    public override void AimTarget(Chess defender)
    {
        var unitsInRange = BattleManager.Instance.GetUnitsInRange(defender.transform.position, skillCfg.Range, owner.side, true);
        unitsInRange.Remove(defender);

        if (unitsInRange.Count > 0 && CheckBurst(defender))
        {
            owner.PlayerAnim(skillCfg.Action);
            BattleManager.Instance.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach (var unit in unitsInRange)
                BattleManager.Instance.CreateAttackMissile(owner, unit, owner.hitEffect);
        }
    }
}
