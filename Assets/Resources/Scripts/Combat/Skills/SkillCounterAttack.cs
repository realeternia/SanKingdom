using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 反戈：受击时几率对攻击者造成一次反击伤害（SkillDamageRate 比例，isFeedback 防反馈链）。
/// </summary>
public class SkillCounterAttack : BattleSkill
{
    public SkillCounterAttack(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if (!CheckBurst(attacker))
            return;

        var damageBack = (int)(damage * skillCfg.SkillDamageRate);
        if (damageBack <= 0)
            return;
        attacker.DoSkillDamage(owner, skillId, damageBack, true, 0);
        this.OnPlaySkill(attacker, damageBack);
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        if (target != null && !string.IsNullOrEmpty(skillCfg.EffectHit))
            EffectManager.PlaySkillEffect(target, skillCfg.EffectHit);
    }
}
