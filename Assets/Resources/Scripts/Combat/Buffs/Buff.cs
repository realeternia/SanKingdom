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
    public BattleSkillConfig skillCfg;

    public int endRound;
    [NonSerialized]
    public GameObject effect;


    public Buff(int id, int skillId, Chess caster, Chess unit, int endRound)
    {
        this.id = id;
        casterId = caster.id;
        ownerId = unit.id;
        this.skillId = skillId;
        this.endRound = endRound;
        buffCfg = BuffConfig.GetConfig(id);
        skillCfg = BattleSkillConfig.GetConfig(skillId);
    }

    public void OnRecover()
    {
        buffCfg = BuffConfig.GetConfig(id);
        skillCfg = BattleSkillConfig.GetConfig(skillId);
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
    public virtual void Refresh(Chess caster, int endRound)
    {
        this.endRound = Math.Max(this.endRound, endRound);
    }

    public void WaitForRemove()
    {
        endRound = 0;
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