using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;    

public class SkillAttackRunCrossPlus : Skill
{
    public SkillAttackRunCrossPlus(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        // 计算镜像位置
        Vector3 ownerPos = owner.position;
        Vector3 defenderPos = defender.position;

        // 计算镜像位置（以defender为中心）
        float mirrorX = 3 * defenderPos.x - 2 * ownerPos.x;
        float mirrorZ = 3 * defenderPos.z - 2 * ownerPos.z;
        Vector3 mirrorPos = new Vector3(mirrorX, ownerPos.y, mirrorZ);

        // 检查是否可以移动到镜像位置
        if (CheckBurst(defender))
        {
            // 启动协程移动
            owner.noMoveCount++;
            EffectManager.PlaySkillEffect(owner, skillCfg.EffectSelf);

            owner.JumpToPosition(mirrorPos, 10f, 0.5f);
        }
    }
}
