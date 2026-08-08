using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitAround : BattleSkill
{
    public SkillHitAround(int id, Chess unit) : base(id, unit)
    {
    }

    public override void DuringAttack(Chess defender, string damType, ref int damageBase, ref float damageMulti,ref int damageReal,  ref string effect)
    {
        if (CheckBurst(defender))
            effect = "";
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (isBurst)
        {
            var startPos = owner.position;
            var targetPos = defender.position;

            SkillManager.AddSkillAction(owner, defender, id, 0);

            var unitsInRange = BattleManager.Instance.GetUnitsInRange(startPos, skillCfg.Range, owner.forceId, true);
            unitsInRange.Remove(defender);
            
            // 筛选startPos到targetPos方向，左右各60°开角内的单位
            if (unitsInRange.Count > 0)
            {
                Vector3 direction = (targetPos - startPos).normalized;
                List<Chess> filteredUnits = new List<Chess>();
                
                foreach (var unit in unitsInRange)
                {
                    Vector3 unitDirection = (unit.position - startPos).normalized;
                    float angle = Vector3.Angle(direction, unitDirection);
                    
                    // 检查是否在左右各60°开角内（总共120°扇形）
                    if (angle <= SystemConst.Battle.AROUND_ATTACK_ANGLE_THRESHOLD)
                        filteredUnits.Add(unit);
                }
                
                if (filteredUnits.Count > 0)
                {
                    BattleManager.RandomSelect(filteredUnits, skillCfg.TargetCount);
                    var damage2 = (int)(damage * skillCfg.SkillDamageRate);
                    foreach(var unit in filteredUnits)
                        unit.DoSkillDamage(owner, skillId, damage2, false, 0);
                }
            }
        }
    }

    public override void OnPlaySkill(Chess targetUnit, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);        
        var startPos = owner.position;
        var targetPos = targetUnit.position;        
        //创建一个hitEffect
        var hitEffect = EffectManager.PlaySkillEffect(targetUnit, skillCfg.EffectHit);
        if(hitEffect != null)
            hitEffect.transform.forward = (targetPos - startPos).normalized;
    }    

}
