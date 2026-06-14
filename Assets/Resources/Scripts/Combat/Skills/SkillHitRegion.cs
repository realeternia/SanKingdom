using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitRegion : Skill
{
    private Vector3 targetPos;
    public SkillHitRegion(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            targetPos = defender.position;

            BattleManager.Instance.SpawnUnitsForRegion(owner.GetForceInfo(), 501001, targetPos, GetSummonTime(), (id) =>
            {
                var magicStubUnit = BattleManager.Instance.GetChess(id);
                SkillManager.AddSkillAction(owner, magicStubUnit, id, 0);
            });

            var summonTime = GetSummonTime();
            var term = (int) System.Math.Floor(summonTime / skillCfg.SummonHitInterval);
            RegisterDelayEffect(BattleManager.Instance.round, summonTime, term);
        }
    }

    public override void OnDelayEffectHit()
    {
        if(owner == null || owner.hp <= 0)
            return;

        var unitsInRange = BattleManager.Instance.GetUnitsInRange(targetPos, skillCfg.SummonArea, owner.forceId, true);
        if (unitsInRange.Count > 0)
        {
            BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);
            var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
            foreach(var unit in unitsInRange)
                unit.DoSkillDamage(owner, skillId, damage, false, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        var summonTime = GetSummonTime();
        //创建一个hitEffect
        EffectManager.PlayPosSkillEffect(target, targetPos, skillCfg.EffectSize, skillCfg.EffectArea, summonTime);        
    }

}
