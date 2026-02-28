using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitRepeat : Skill
{
    public int defenderId;
    public int damage;

    public SkillHitRepeat(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            this.defenderId = defender.id;
            this.damage = damage;
            RegisterDelayEffect(BattleManager.Instance.tickIndex, skillCfg.TimeDelay * skillCfg.DoCount, skillCfg.DoCount);
        }
    }

    public override void OnDelayEffectHit()
    {
        var defender = BattleManager.Instance.GetChess(defenderId);
        if (defender != null && defender.hp > 0)
        {
            var d = (int)(damage * skillCfg.SkillDamageRate);
            defender.DoSkillDamage(owner, skillId, d);
            SkillManager.AddSkillAction(defender, defender, id, d);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
        BattleManager.Instance.AddBattleText(parm1.ToString() + "!", target.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
    }
 }
