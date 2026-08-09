using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 技能类，处理技能相关逻辑
/// </summary>
[Serializable]
public class BattleSkill : IRecoverable
{
    public int id;
    public int ownerId;
    [NonSerialized]
    public Chess owner;
    public bool isGivenSkill; //别人给的技能
    [NonSerialized]
    public BattleSkillConfig skillCfg;
    public int lastUseRound; // 上次更新CD的回合
    [NonSerialized]
    public bool isBurst;
    public int skillId{ get{ return id; } }

    public List<int> delayedFrames = new List<int>();

    public BattleSkill(int id, Chess unit)
    {
        this.id = id;
        this.owner = unit;
        ownerId = unit.id;

        skillCfg = BattleSkillConfig.GetConfig(id);
    }

    public void OnRecover()
    {
        owner = BattleManager.Instance.GetChess(ownerId);
        skillCfg = BattleSkillConfig.GetConfig(id);
    }

    /// <summary>
    /// 更新技能CD时间
    /// </summary>
    public void UpdateCD()
    {
        if (skillCfg.RoundCd > 0)
        {
            if (IsInCD())
            {
                return;
            }

            var cdTime = skillCfg.RoundCd;
            SkillManager.OnCheckCD(owner, skillCfg, ref cdTime);

            lastUseRound = BattleManager.Instance.round + cdTime;
        }
    }

    /// <summary>
    /// 检查技能是否在CD中
    /// </summary>
    /// <returns>如果在CD中返回true，否则返回false</returns>
    public bool IsInCD()
    {
        if(skillCfg.RoundCd <= 0)
            return false;

        return BattleManager.Instance.round < lastUseRound;
    }

    public bool CheckBurst(Chess target)
    {
        var rate = skillCfg.Rate;
        if (rate > 0 && rate < 1 && target != null && target != owner)
        {
            if (owner.forceId != target.forceId)
            {
                var myAttr = owner.GetAttr(skillCfg.Attr);
                var defAttr = target.GetAttr(skillCfg.Attr);
                rate = SysFormula.Battle.AdjustBurstRateByAttr(rate, myAttr, defAttr, true);
            }

            SkillManager.OnCheckBurst(owner, skillCfg, ref rate);
        }

        isBurst = !IsInCD() && (skillCfg.Rate <= 0 || BattleRandom.Value < rate);
      //  UnityEngine.Debug.Log("CheckBurst isBurst=" + isBurst.ToString() + " skillId=" + id.ToString());
        if(isBurst)
            UpdateCD();
        return isBurst;
    }

    protected void RegisterDelayEffect(int currentRound, float roundsDelay, int count)
    {
        delayedFrames.Clear();
        for (int i = 0; i < count; i++)
        {
            var roundDelay = (int)(roundsDelay * (i + 1) / count);
            delayedFrames.Add(currentRound + roundDelay);
        }
    }

    public virtual void BattleBegin()
    {

    }

    public virtual void LogicUpdate(int tickIndex)
    {
        if(delayedFrames.Count > 0 && delayedFrames[0] <= BattleManager.Instance.round)
        {
            delayedFrames.RemoveAt(0);
            OnDelayEffectHit();
        }
    }

    public virtual void OnDelayEffectHit()
    {
    }

    public virtual void AimTarget(Chess target)
    {

    }

    public virtual void OnAttack(Chess defender, string damType, int damage)
    {
    }

    public virtual void OnAttacked(Chess attacker, string damType, int damage)
    {
    }

    public virtual void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref int damageReal, ref string effect)
    {
    }

    public virtual void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual bool CheckAidSkill(int tickIndex)
    {
        return false;
    }

    public virtual void OnCheckBurst(BattleSkillConfig checkSkillCfg, ref float rate)
    {
        
    }

    public virtual void OnAddBuff(Chess target, ref int buffId, int skillId, ref int lastTime)
    {

    }

    public virtual void OnCheckCD(BattleSkillConfig checkSkillCfg, ref int cdTime)
    {

    }

    public virtual void OnBeAddBuff(Chess caster, ref int buffId, int checkSkillId, ref int lastTime)
    {

    }

    public virtual void OnDoSkillDamage(Chess target, BattleSkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        
    }

    public virtual void OnBeDoSkillDamage(Chess caster, BattleSkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        
    }

    public virtual void OnHealTarget(Chess target, int checkSkillId, ref int addon)
    {
        
    }

    public virtual void OnCheckRoundCount(BattleSkillConfig checkSkillCfg, ref int roundCount)
    {

    }

    public virtual void OnPlaySkill(Chess target, int parm1)
    {

    }

    public int GetRoundCount()
    {
        var roundCount = skillCfg.RoundCount;
        SkillManager.OnCheckRoundCount(owner, skillCfg, ref roundCount);
        return roundCount;
    }

}
