using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

/// <summary>
/// 乱射：攻击时几率对以目标为中心、skillCfg.Range 为半径的范围内随机 skillCfg.TargetCount 个目标射击（不分敌我，不含施法者）。
/// </summary>
public class SkillRandomArea : BattleSkill
{
    public SkillRandomArea(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (!CheckBurst(defender))
            return;

        this.OnPlaySkill(defender, 0);

        var bm = BattleManager.Instance;
        var defCell = bm.GetMapCellById(defender.cellId);
        if (defCell == null)
            return;

        // 以目标为中心 Range 半径，收集范围内所有存活单位（不分敌我，排除施法者/影子/城墙）
        List<Chess> candidates = new List<Chess>();
        foreach (var chessComponent in bm.chessList)
        {
            if (chessComponent == null || chessComponent == owner || chessComponent.hp <= 0 || chessComponent.isShadow || chessComponent.isWall)
                continue;
            var unitCell = bm.GetMapCellById(bm.GetChessCellId(chessComponent));
            if (unitCell == null)
                continue;
            if (HexUtil.HexDistance(defCell.gridX, defCell.gridZ, unitCell.gridX, unitCell.gridZ) <= skillCfg.Range)
                candidates.Add(chessComponent);
        }

        if (candidates.Count == 0)
            return;

        BattleManager.RandomSelect(candidates, skillCfg.TargetCount);
        int areaDamage = (int)(damage * skillCfg.SkillDamageRate);
        foreach (var unit in candidates)
            unit.DoSkillDamage(owner, skillId, areaDamage, false, 0);
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
