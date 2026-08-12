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

            var targetPos = defender.position;
            var roundCount = GetSummonRoundCount();
            var currentRound = BattleManager.Instance.round;

            Vector3 direction = (defender.position - owner.position).normalized;
            Vector3 rightDirection = Quaternion.Euler(0, 90, 0) * direction;
            Vector3 leftDirection = Quaternion.Euler(0, -90, 0) * direction;

            var posList = new List<Vector3>();
            posList.Add(targetPos);
            if (skillCfg.SummonCount > 1)
            {
                posList.Add(targetPos + leftDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE);
                posList.Add(targetPos + rightDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE);
            }
            if (skillCfg.SummonCount > 3)
            {
                posList.Add(targetPos + rightDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE_FAR);
                posList.Add(targetPos + leftDirection * SystemConst.Battle.WALL_OFFSET_DISTANCE_FAR);
            }

            var endRound = currentRound + roundCount;

            foreach (var pos in posList)
            {
                var bm = BattleManager.Instance;
                var (gx, gz) = bm.WorldToGridCoord(pos);
                var cellId = bm.GetCellId(gx, gz);
                if (cellId <= 0)
                {
                    GameLog.Warn($"SkillHitWall 格子越界 gx={gx} gz={gz}，跳过生成火墙");
                    continue;
                }
                var effect = new CellEffect
                {
                    skillId = id,
                    casterId = owner.id,
                    forceId = owner.forceId,
                    attr = skillCfg.Attr,
                    damageRate = skillCfg.SkillDamageAttrRate,
                    endRound = endRound
                };
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
