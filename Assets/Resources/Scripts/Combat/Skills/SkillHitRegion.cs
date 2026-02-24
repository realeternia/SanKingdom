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
            owner.PlayerAnim(skillCfg.Action);

            targetPos = defender.position;

            var magicStub = BattleManager.Instance.SpawnUnitsForRegion(owner.GetPlayerInfo(), 501001, targetPos);
            var summonTime = GetSummonTime();
            magicStub.SetLifeTime(summonTime);

            //创建一个hitEffect
            EffectManager.PlayPosSkillEffect(magicStub, targetPos, skillCfg.EffectSize, skillCfg.EffectArea, summonTime);

            BattleManager.Instance.StartNLCoroutine(DelayDamage(summonTime));
        }
    }

    IEnumerator DelayDamage(float summonTime)
    {
        var term = (int) System.Math.Floor(summonTime / skillCfg.SummonHitInterval);
        for (int i = 0; i < term; i++)
        {
            if(owner == null || owner.hp <= 0)
                yield break;

            var unitsInRange = BattleManager.Instance.GetUnitsInRange(targetPos, skillCfg.SummonArea, owner.forceId, true);
            if (unitsInRange.Count > 0)
            {
                BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);
                var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
                foreach(var unit in unitsInRange)
                    unit.DoSkillDamage(owner, skillId, damage);
            }
            yield return new NLWaitForSeconds(skillCfg.SummonHitInterval);
        }
    }

}
