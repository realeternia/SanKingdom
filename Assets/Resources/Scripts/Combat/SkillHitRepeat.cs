using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitRepeat : Skill
{
    public SkillHitRepeat(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if(CheckBurst(defender))
        {
            BattleManager.Instance.AddBattleText(damage.ToString() + "!", defender.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            owner.PlayerAnim(skillCfg.Action);
            BattleManager.Instance.StartNLCoroutine(DelayAttack(defender, damage));
        }
    }

    IEnumerator DelayAttack(Chess defender, int damage)
    {
        for (int i = 0; i < skillCfg.DoCount; i++)
        {
            yield return new NLWaitForSeconds(skillCfg.TimeDelay);
            if (defender != null && defender.hp > 0)
            {
                var d = (int)(damage * skillCfg.SkillDamageRate);
                defender.OnSkillDamaged(owner, skillId, d);
                EffectManager.PlaySkillEffect(defender, skillCfg.HitEffect);
                BattleManager.Instance.AddBattleText(d.ToString() + "!", defender.position, new UnityEngine.Vector2(0, 60), Color.red, 3);
            }
        }
    }
}
