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
            var roundCount = GetRoundCount();
            var currentRound = BattleManager.Instance.round;

            var (gx, gz) = BattleManager.Instance.WorldToGridCoord(targetPos);
            var effect = new CellEffect
            {
                skillId = id,
                casterId = owner.id,
                forceId = owner.forceId,
                attr = skillCfg.Attr,
                damageRate = skillCfg.SkillDamageAttrRate,
                endRound = currentRound + roundCount
            };
            BattleManager.Instance.AddCellEffect(gx, gz, effect);

            SkillManager.AddSkillAction(owner, null, id, 0);
        }
    }

    public override void OnPlaySkill(Chess target, int parm1)
    {
        owner.PlayerAnim(skillCfg.Action);
    }
}
