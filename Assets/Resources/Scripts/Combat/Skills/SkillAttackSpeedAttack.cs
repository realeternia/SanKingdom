using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
public class SkillAttackSpeedAttack : BattleSkill
{
    public SkillAttackSpeedAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            this.OnPlaySkill(null, 0);
            GameLog.Debug("SkillSpeedAttack");

            owner.Cooldown((int)(2 * skillCfg.Strength));
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
