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
            this.OnPlaySkill(defender, 0);

            var bm = BattleManager.Instance;
            var defenderCell = bm.GetMapCellById(defender.cellId);
            if (defenderCell == null)
            {
                GameLog.Warn($"SkillHitAround 目标 {defender.id} cellId={defender.cellId} 无效，跳过");
                return;
            }

            // 候选目标 = 当前单位射程内 且 位于目标周围1格
            var unitsInRange = bm.GetUnitsInCellRange(owner.cellId, skillCfg.Range, owner.forceId, true);
            List<Chess> aroundTargets = new List<Chess>();
            foreach (var unit in unitsInRange)
            {
                if (unit == defender) continue;
                var unitCell = bm.GetMapCellById(unit.cellId);
                if (unitCell == null) continue;
                if (HexUtil.HexDistance(unitCell.gridX, unitCell.gridZ, defenderCell.gridX, defenderCell.gridZ) <= 1)
                    aroundTargets.Add(unit);
            }

            if (aroundTargets.Count == 0)
                return;

            // TargetCount 非0且目标数超过时随机选取
            if (skillCfg.TargetCount > 0 && aroundTargets.Count > skillCfg.TargetCount)
                BattleManager.RandomSelect(aroundTargets, skillCfg.TargetCount);

            var damage2 = (int)(damage * skillCfg.SkillDamageRate);
            foreach (var unit in aroundTargets)
                unit.DoSkillDamage(owner, skillId, damage2, false, 0);
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
