using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillAidShockWave : Skill
{
    public SkillAidShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill(int tickIndex)
    {
        if (owner.targetChess == null)
            return false;

        if (!BattleManager.Instance.CheckInRange(owner.position, owner.targetChess.position, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        var targetPos = owner.targetChess.position; // 使用目标位置而不是自身位置

        owner.PlayerAnim(skillCfg.Action);
        var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
        BattleManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.Id, damage);

        Debug.Log("SkillAidShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

}
