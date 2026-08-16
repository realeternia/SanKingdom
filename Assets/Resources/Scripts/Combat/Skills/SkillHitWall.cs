using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public class SkillHitWall : BattleSkill
{
    public SkillHitWall(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            this.OnPlaySkill(null, 0);

            var bm = BattleManager.Instance;
            var currentRound = bm.round;
            var roundCount = GetSummonRoundCount();
            var endRound = currentRound + roundCount;

            // 火墙沿六边形纵向(同列 gz±1)展开，呈一条连通直线
            var (tgx, tgz) = bm.WorldToGridCoord(defender.position);
            var offsets = new List<int> { 0 };
            if (skillCfg.SummonCount > 1)
            {
                offsets.Add(-1);
                offsets.Add(1);
            }
            if (skillCfg.SummonCount > 3)
            {
                offsets.Add(-2);
                offsets.Add(2);
            }

            foreach (var offset in offsets)
            {
                var (gx, gz) = (tgx, tgz + offset);
                var cellId = bm.GetCellId(gx, gz);
                if (cellId <= 0)
                {
                    GameLog.Warn($"SkillHitWall 格子越界 gx={gx} gz={gz}，跳过生成火墙");
                    continue;
                }
                var effect = CellEffect.Create("Fire", skillCfg, owner, endRound);
                bm.AddCellEffect(cellId, effect);
            }
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        if (parm1 == 0)
            owner.PlayerAnim(skillCfg.Action);
    }
}
