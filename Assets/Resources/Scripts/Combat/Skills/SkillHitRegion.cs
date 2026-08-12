using CommonConfig;
using UnityEngine;

public class SkillHitRegion : BattleSkill
{
    private Vector3 targetPos;

    public SkillHitRegion(int id, Chess unit) : base(id, unit)
    {
    }

    public override void OnAttack(Chess defender, string damType, int damage)
    {
        if (CheckBurst(defender))
        {
            targetPos = defender.position;
            var roundCount = GetSummonRoundCount();
            var currentRound = BattleManager.Instance.round;

            var bm = BattleManager.Instance;
            var (gx, gz) = bm.WorldToGridCoord(targetPos);
            var cellId = bm.GetCellId(gx, gz);
            if (cellId <= 0)
                return;
            var effect = new CellEffect
            {
                skillId = id,
                casterId = owner.id,
                forceId = owner.forceId,
                attr = skillCfg.Attr,
                damageRate = skillCfg.SkillDamageAttrRate,
                endRound = currentRound + roundCount
            };
            bm.AddCellEffect(cellId, effect);

            this.OnPlaySkill(null, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
