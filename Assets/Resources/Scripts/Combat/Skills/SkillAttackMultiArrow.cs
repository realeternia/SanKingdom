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
        var unitsInRange = BattleManager.Instance.GetUnitsInRange(defender.position, skillCfg.Range, owner.forceId, true);
        unitsInRange.Remove(defender);

        if (unitsInRange.Count > 0 && CheckBurst(defender))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);
            BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach (var unit in unitsInRange)
                BattleManager.Instance.CreateAttackMissile(owner, unit);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
