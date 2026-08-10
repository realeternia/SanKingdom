using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackMultiArrow : BattleSkill
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
            this.OnPlaySkill(null, 0);
            BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);
            foreach (var unit in unitsInRange)
            {
                var (damage, isCrit, isDodge, effect) = Chess.CalculateAttackDamage(owner, unit, "str", owner.hitEffect);
                BattleManager.Instance.CreateAttackMissile(owner, unit, damage, isCrit, isDodge, effect, "str");
            }
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
