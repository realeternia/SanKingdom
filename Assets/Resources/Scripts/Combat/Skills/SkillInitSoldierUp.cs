using System;
using CommonConfig;
using UnityEngine;

public class SkillInitSoldierUp : Skill
{
    public SkillInitSoldierUp(int id, Chess chess) : base(id, chess)
    {
    }

    public override void BattleBegin()
    {
        UnityEngine.Debug.Log("SkillInitSoldierUp BattleBegin");

        var unitsInRange = BattleManager.Instance.GetUnitsMyForce(owner.position, skillCfg.Range, owner.forceId);
        var atkAdd = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillDamageAttrRate);
        var hpAdd = (int)(owner.GetAttr(skillCfg.Attr) * skillCfg.SkillAttrRate);
        owner.PlayerAnim(skillCfg.Action);

        foreach(var unit in unitsInRange)
        {
            if(unit.isHero)
                continue;

            unit.AddSoldierLevel(1, atkAdd, hpAdd);
            EffectManager.PlaySkillEffect(unit, skillCfg.HitEffect);
        }
        var castleHUD = owner.GetPlayerInfo().castleHUD;
        if(castleHUD != null)
            castleHUD.AddSoldierLevel(1, atkAdd, hpAdd);
    }
}