using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 技能类，处理技能相关逻辑
/// </summary>
[Serializable]
public class Skill : IRecoverable
{
    public int id;
    public int ownerId;
    [NonSerialized]
    public Chess owner;
    public bool isGivenSkill; //别人给的技能
    [NonSerialized]
    public SkillConfig skillCfg;
    public int lastUpdateTick; // 上次更新CD的时间 (tick index)
    [NonSerialized]
    public bool isBurst;
    public int skillId{ get{ return id; } }

    public List<int> delayedFrames = new List<int>();

    public Skill(int id, Chess unit)
    {
        this.id = id;
        this.owner = unit;
        ownerId = unit.id;

        skillCfg = SkillConfig.GetConfig(id);
    }

    public void OnRecover()
    {
        owner = BattleManager.Instance.GetChess(ownerId);
        skillCfg = SkillConfig.GetConfig(id);
    }

    /// <summary>
    /// 更新技能CD时间
    /// </summary>
    public void UpdateCD()
    {
        if (skillCfg.CD > 0)
        {
            if (IsInCD())
            {
                return;
            }

            var cdTime = skillCfg.CD;
            SkillManager.OnCheckCD(owner, skillCfg, ref cdTime);

            lastUpdateTick = BattleManager.Instance.tickIndex + BattleManager.Instance.GetTickFromTime(cdTime);
        }
    }

    /// <summary>
    /// 检查技能是否在CD中
    /// </summary>
    /// <returns>如果在CD中返回true，否则返回false</returns>
    public bool IsInCD()
    {
        if(skillCfg.CD <= 0)
            return false;

        return BattleManager.Instance.tickIndex < lastUpdateTick;
    }

    public bool CheckBurst(Chess target)
    {
        var rate = skillCfg.Rate;
        if (rate > 0 && rate < 1 && target != null && target != owner)
        {
            var myAttr = owner.GetAttr(skillCfg.Attr);
            var defAttr = target.GetAttr(skillCfg.Attr);
            if (owner.forceId != target.forceId)
            {
                if (myAttr > defAttr)
                    rate *= Math.Min(2, 1 + (myAttr - defAttr) * .02f);
                else if (myAttr < defAttr)
                    rate /= Math.Min(2, 1 + (defAttr - myAttr) * .02f);
            }

            SkillManager.OnCheckBurst(owner, skillCfg, ref rate);
        }

        isBurst = !IsInCD() && (skillCfg.Rate <= 0 || UnityEngine.Random.value < rate);
      //  UnityEngine.Debug.Log("CheckBurst isBurst=" + isBurst.ToString() + " skillId=" + id.ToString());
        if(isBurst)
            UpdateCD();
        return isBurst;
    }

    protected void RegisterDelayEffect(int tickIndex, float time, int count)
    {
        delayedFrames.Clear();
        for (int i = 0; i < count; i++)
        {
            var tickDelay = BattleManager.Instance.GetTickFromTime(time * (i + 1) / count);
            delayedFrames.Add(tickIndex + tickDelay);
        }
    }

    public virtual void BattleBegin()
    {

    }

    public virtual void LogicUpdate(int tickIndex)
    {
        if(delayedFrames.Count > 0 && delayedFrames[0] == tickIndex)
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

    public virtual void OnCheckBurst(SkillConfig checkSkillCfg, ref float rate)
    {
        
    }

    public virtual void OnAddBuff(Chess target, ref int buffId, int skillId, ref float lastTime)
    {
        
    }

    public virtual void OnCheckCD(SkillConfig checkSkillCfg, ref float cdTime)
    {

    }

    public virtual void OnBeAddBuff(Chess caster, ref int buffId, int checkSkillId, ref float lastTime)
    {
        
    }

    public virtual void OnDoSkillDamage(Chess target, SkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        
    }

    public virtual void OnBeDoSkillDamage(Chess caster, SkillConfig checkSkillCfg, ref int damage, bool isFeedback)
    {
        
    }

    public virtual void OnHealTarget(Chess target, int checkSkillId, ref int addon)
    {
        
    }

    public virtual void OnCheckSummonTime(SkillConfig checkSkillCfg, ref float summonTime)
    {

    }

    public virtual void OnPlaySkill(Chess target, int parm1)
    {
        
    }

    public float GetSummonTime()
    {
        var summonTime = skillCfg.SummonTime;
        SkillManager.OnCheckSummonTime(owner, skillCfg, ref summonTime);
        return summonTime;
    }

}
