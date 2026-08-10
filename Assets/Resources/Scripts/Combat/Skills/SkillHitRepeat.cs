using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitRepeat : BattleSkill
{
    public int defenderId;
    public int damage;
    public int remainingCount; // 剩余连击次数

    public SkillHitRepeat(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            this.defenderId = defender.id;
            this.damage = damage;
            remainingCount = skillCfg.DoCount;
        }
    }

    public override void LogicUpdate()
    {
        if (remainingCount <= 0)
            return;

        var defender = BattleManager.Instance.GetChess(defenderId);
        if (defender == null || defender.hp <= 0)
        {
            remainingCount = 0;
            return;
        }

        // 回合制下：下一次行动回合结算全部连击，直至次数用尽
        while (remainingCount > 0)
        {
            remainingCount--;
            var d = (int)(damage * skillCfg.SkillDamageRate);
            defender.DoSkillDamage(owner, skillId, d, false, 0);
            this.OnPlaySkill(defender, d);
            if (defender.hp <= 0)
                break;
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
 }
