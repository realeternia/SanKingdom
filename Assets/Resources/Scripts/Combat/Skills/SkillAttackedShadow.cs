using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAttackedShadow : Skill
{
    private int count;
    public SkillAttackedShadow(int id, Chess unit) : base(id, unit)
    {
        count = skillCfg.DoCount;
    }

    public override void OnAttacked(Chess attacker, string damType, int damage)
    {
        if (count > 0 && CheckBurst(attacker))
        {
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 randomPosition = owner.position + new Vector3(randomDir.x, 0, randomDir.y) * skillCfg.Range;
            BattleManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501002, randomPosition, 0, (shadowUnitId) =>
            {
                var shadowUnit = BattleManager.Instance.GetChess(shadowUnitId);
                shadowUnit.atk = (int)(owner.atk * skillCfg.SkillDamageRate);
                shadowUnit.maxHp = (int)(owner.maxHp * skillCfg.SkillAttrRate);

                //todo 这里需要放到action里
                shadowUnit.atk = (int)(owner.atk * skillCfg.SkillDamageRate);
                shadowUnit.maxHp = (int)(owner.maxHp * skillCfg.SkillAttrRate);
                shadowUnit.hp = (int)(shadowUnit.maxHp * owner.HpRate);

                SkillManager.AddSkillAction(owner, shadowUnit, id, 0);
            });

            count--;
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);
        EffectManager.PlaySkillEffect(target, skillCfg.EffectHit);

        owner.PlayerAnim(skillCfg.Action);
    }

}
