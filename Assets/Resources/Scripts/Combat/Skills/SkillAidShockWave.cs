using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using Controls.Utils;

public class SkillAidShockWave : Skill
{
    public SkillAidShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill(int tickIndex)
    {
        var targetChess = BattleManager.Instance.GetChess(owner.targetChessId);
        if (targetChess == null)
            return false;

        if (!BattleManager.CheckInRange(owner.position, targetChess.position, skillCfg.Range))
            return false;

        if (!CheckBurst(null))
            return false;

        var targetPos = targetChess.position; // 使用目标位置而不是自身位置

        var damage = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
        SkillManager.AddSkillAction(owner, targetChess, id, damage);
        BattleManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonTime(), skillCfg.Id, damage);
        GameLog.Debug("SkillAidShockWave id=" + id.ToString() + " damage=" + damage.ToString());

        return true;
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
