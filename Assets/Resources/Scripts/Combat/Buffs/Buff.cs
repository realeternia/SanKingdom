using System;
using CommonConfig;
using UnityEngine;


public class Buff
{
    public int id;

    public int casterId;
    public Chess caster{get{return BattleManager.Instance.GetChess(casterId);}}
    
    public int ownerId;
    public Chess owner{get{return BattleManager.Instance.GetChess(ownerId);}}
    
    public BuffConfig buffCfg;
    public SkillConfig skillCfg;

    public int endTime;
    public GameObject effect;


    public Buff(int id, int skillId, Chess caster, Chess unit, float lastTime)
    {
        this.id = id;
        casterId = caster.id;
        ownerId = unit.id;
        buffCfg = BuffConfig.GetConfig(id);
        skillCfg = SkillConfig.GetConfig(skillId);
        endTime = BattleManager.Instance.tickIndex + (int)(lastTime / BattleManager.tickTimeReal);

    }

    public void SetTime(float time)
    {
        endTime = BattleManager.Instance.tickIndex + (int)(time / BattleManager.tickTimeReal);
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
    public virtual void Refresh(Chess caster, float lastTime)
    {
        endTime = Math.Max(endTime, BattleManager.Instance.tickIndex + (int)(lastTime / BattleManager.tickTimeReal));
    }

    public void WaitForRemove()
    {
        endTime = BattleManager.Instance.tickIndex - 1;

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