using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;    


public class SkillAttackRunCross : Skill
{
    public SkillAttackRunCross(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 计算镜像位置
        Vector3 ownerPos = owner.position;
        Vector3 defenderPos = defender.position;

        // 计算镜像位置（以defender为中心）
        float mirrorX = 2 * defenderPos.x - ownerPos.x;
        float mirrorZ = 2 * defenderPos.z - ownerPos.z;
        Vector3 mirrorPos = new Vector3(mirrorX, ownerPos.y, mirrorZ);

        // 检查是否可以移动到镜像位置
        if (CheckBurst(defender))
        {
            // 启动协程移动
            owner.noMoveCount++;
            EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);

            owner.JumpToPosition(mirrorPos, 10f, 0.5f);

            defender.DoSkillDamage(owner, skillId, (int)(damage * skillCfg.SkillDamageRate));

            BuffManager.AddBuff(defender, owner, id, skillCfg.BuffId, skillCfg.BuffTime); //加负面buff                    
        }
    }
 
}
