using CommonConfig;
using UnityEngine;

[System.Serializable]
public class SkillDamageAction : ChessAction
{
    public int TargetChessId;
    public int SkillId;
    public int Damage;

    public SkillDamageAction(int sourceId, int tick, int targetChessId, int skillId, int damage)
        : base(sourceId, tick)
    {
        TargetChessId = targetChessId;
        SkillId = skillId;
        Damage = damage;
    }

    public override void Doing()
    {
        var battleManager = BattleManager.Instance;
        var targetChess = battleManager.GetChess(TargetChessId);
        var casterChess = battleManager.GetChess(SourceId);

        if (targetChess != null && casterChess != null)
        {
            // 直接执行伤害回调
            targetChess.hp -= Damage;
            if(casterChess != targetChess)
                targetChess.lastDamagedPlayerId = casterChess.forceId;

            var skillCfg = SkillConfig.GetConfig(SkillId);
            if(!string.IsNullOrEmpty(skillCfg.EffectHit))
                EffectManager.PlaySkillEffect(targetChess, skillCfg.EffectHit);

            if(casterChess.isHero && Damage > 0)
                BattleStatManager.AddBattleStat(casterChess.forceId, casterChess.heroId, Damage);

            BattleManager.Instance.AddBattleText("-" + (Damage).ToString(), targetChess.position, new UnityEngine.Vector2(0, 60), new Color(1, 0, 0), 2);

            targetChess.OnHpChanged();
        }
    }
}