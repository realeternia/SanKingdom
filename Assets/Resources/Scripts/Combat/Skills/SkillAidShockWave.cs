using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
public class SkillAidShockWave : BattleSkill
{
    public SkillAidShockWave(int id, Chess unit) : base(id, unit)
    {
    }

    public override bool CheckAidSkill()
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
        this.OnPlaySkill(targetChess, damage);
        BattleManager.Instance.CreateSpellMissile(owner, targetPos, GetSummonRoundCount(), skillCfg.Id, damage);
        GameLog.Debug($"SkillAidShockWave[aid=0] id={id} damage={damage}");

        return true;
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
