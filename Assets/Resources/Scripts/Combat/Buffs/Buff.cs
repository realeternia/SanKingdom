using System;
using CommonConfig;
using UnityEngine;


[Serializable]
public class Buff : IRecoverable
{
    public int id;

    public int casterId;
    
    public int ownerId;
    
    public int skillId;

    [NonSerialized]
    public BuffConfig buffCfg;
    [NonSerialized]
    public SkillConfig skillCfg;

    public int endTime;
    [NonSerialized]
    public GameObject effect;


    public Buff(int id, int skillId, Chess caster, Chess unit, int endTick)
    {
        this.id = id;
        casterId = caster.id;
        ownerId = unit.id;
        this.skillId = skillId;
        endTime = endTick;
        buffCfg = BuffConfig.GetConfig(id);
        skillCfg = SkillConfig.GetConfig(skillId);
    }

    public void OnRecover()
    {
        buffCfg = BuffConfig.GetConfig(id);
        skillCfg = SkillConfig.GetConfig(skillId);
    }

    public virtual void OnAdd(Chess chess, Chess caster)
    {
       // UnityEngine.Debug.Log("Buff OnAdd " + id + " " + skillCfg.Id + " " + caster + " " + chess);
        ownerId = chess.id;

        if (!string.IsNullOrEmpty(buffCfg.BuffEffect))
        {
            effect = EffectManager.PlayBuffEffect(chess, buffCfg.BuffEffect);
        }

        if(!string.IsNullOrEmpty(buffCfg.ColorStart))
        {
            Color start = ColorUtility.TryParseHtmlString(buffCfg.ColorStart, out start) ? start : Color.white;
            Color end = ColorUtility.TryParseHtmlString(buffCfg.ColorEnd, out end) ? end : Color.white;
            chess.AddColorEffect(start, end);
        }

    }

    public virtual void OnRemove(Chess chess)
    {
       // UnityEngine.Debug.Log("Buff OnRemove " + id + " " + skillCfg.Id + " " + caster + " " + chess);
        if (effect != null)
        {
            GameObject.Destroy(effect);
            effect = null;
        }
        if (!string.IsNullOrEmpty(buffCfg.ColorStart))
        { 
            chess.RemoveColorEffect();
        }

        ownerId = 0;
    }

    //刷新
    public virtual void Refresh(Chess caster, int endTick)
    {
        endTime = Math.Max(endTime, endTick);
    }

    public void WaitForRemove()
    {
        endTime = 0;
    }

    public virtual void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual void DuringAttacked(Chess attacker, string damType, ref int damageBase, ref float damageMulti, ref string effect)
    {
    }

    public virtual void BeforeAttacked(Chess defender, ref int damage)
    {
    }


    public virtual void OnAttack(Chess defender, int damage)
    {
    }

    public virtual void OnAttacked(Chess attacker, int damage)
    {
    }

}