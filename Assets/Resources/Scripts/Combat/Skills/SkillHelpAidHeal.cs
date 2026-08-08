using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;


public class SkillHelpAidHeal : BattleSkill
{
    public SkillHelpAidHeal(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill(int tickIndex)
    {
        var unitsInRange = BattleManager.Instance.GetUnitsInRange(owner.position, skillCfg.Range, owner.forceId, false);
        unitsInRange = unitsInRange.FindAll(x => x.hp < x.maxHp * SystemConst.Battle.HEAL_TARGET_HP_RATE && x != owner);

        if (unitsInRange.Count == 0)
            return false;

        if (!CheckBurst(null))
            return false;

        //排序，优先给hero，然后优先给生命值低的
        unitsInRange.Sort((a, b) =>
        {
            if (a.isHero && !b.isHero)
                return -1;
            if (b.isHero && !a.isHero)
                return 1;
            return a.hp.CompareTo(b.hp);
        });

        var targetUnit = unitsInRange[0];
        owner.HealTarget(targetUnit, skillId, (int)(owner.inte * skillCfg.SkillAttrRate));

        SkillManager.AddSkillAction(owner, targetUnit, id, 0);

        return true;
    }

    public override void OnPlaySkill(Chess targetUnit, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        EffectManager.PlaySkillEffect(targetUnit, skillCfg.EffectHit);
    }    
}
