using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitWall : Skill
{
    public List<Vector3> targetPosList;
    public SkillHitWall(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            SkillManager.AddSkillAction(owner, null, id, 0);
            // 在目标位置，以及owner和defender方向90度两侧，各创建一个effect
            var targetPos = defender.position;

            BattleManager.Instance.SpawnUnitsForRegion(owner.GetForceInfo(), SystemConst.Battle.MAGIC_HELPER_UNIT_ID, targetPos, GetSummonTime(), (stubId) =>
            {
                var magicStub = BattleManager.Instance.GetChess(stubId);
                SkillManager.AddSkillAction(owner, magicStub, id, 0);

                // 计算owner到defender的方向
                Vector3 direction = (defender.position - owner.position).normalized;
                
                // 计算90度和-90度旋转的方向
                Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * direction;
                Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * direction;

                targetPosList = new List<Vector3>();
                
                targetPosList.Add(targetPos);
                if (skillCfg.SummonCount > 1)
                {
                    targetPosList.Add(targetPos + leftDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE);
                    targetPosList.Add(targetPos + rightDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE);
                }
                if (skillCfg.SummonCount > 3)
                {
                    targetPosList.Add(targetPos + rightDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE_FAR);
                    targetPosList.Add(targetPos + leftDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE_FAR);
                }

                for(int i = 0; i < targetPosList.Count; i++)
                {
                    SkillManager.AddSkillAction(owner, magicStub, id, i + 1);
                }
            });
            
            var summonTime = GetSummonTime();
            var term = (int)Math.Floor(summonTime / skillCfg.SummonHitInterval);
            RegisterDelayEffect(BattleManager.Instance.round, summonTime, term);
        }
    }

    public override void OnDelayEffectHit()
    {
        if (owner == null || owner.hp <= 0)
            return;

        var unitList = new List<Chess>();
        foreach (var pos in targetPosList)
        {
            var unitsInRange = BattleManager.Instance.GetUnitsInRange(pos, skillCfg.SummonArea * SystemConst.Battle.WALL_DAMAGE_AREA_EXPAND, owner.forceId, true);
            BattleManager.RandomSelect(unitsInRange, skillCfg.TargetCount);

            foreach (var unit in unitsInRange)
            {
                if (unitList.Contains(unit))
                    continue;
                unitList.Add(unit);
            }
        }
        var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
        foreach (var unit in unitList)
        {
            unit.DoSkillDamage(owner, skillId, damage, false, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        if(parm1 == 0)
            owner.PlayerAnim(skillCfg.Action);
        else
        {
            var summonTime = GetSummonTime();
            // EffectManager.PlayPosSkillEffect(target, targetPosList[parm1 - 1], skillCfg.EffectSize, skillCfg.EffectArea, summonTime);
        }

    }

}
