using CommonConfig;
using UnityEngine;

public class SkillHitRegion : BattleSkill
{
    public SkillHitRegion(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            var roundCount = GetSummonRoundCount();
            var currentRound = BattleManager.Instance.round;

            var bm = BattleManager.Instance;
            var cellId = defender.cellId;
            if (cellId <= 0)
            {
                GameLog.Warn($"SkillHitRegion 目标 {defender.id} cellId={cellId} 无效，跳过落雷");
                return;
            }
            // 落雷暂无专用派生类，走通用基类（只伤敌方）
            var effect = CellEffect.Create("", skillCfg, owner, currentRound + roundCount);
            bm.AddCellEffect(cellId, effect);

            this.OnPlaySkill(null, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
