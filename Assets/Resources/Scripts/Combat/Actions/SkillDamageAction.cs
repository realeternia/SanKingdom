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
            var actualDamage = Mathf.Min(Damage, targetChess.hp);
            targetChess.hp -= Damage;
            if(casterChess != targetChess)
                targetChess.lastDamagedPlayerId = casterChess.forceId;

            var skillCfg = SkillConfig.GetConfig(SkillId);
            if(!string.IsNullOrEmpty(skillCfg.EffectHit))
                EffectManager.PlaySkillEffect(targetChess, skillCfg.EffectHit);

            if(casterChess.isHero && actualDamage > 0)
                BattleStatManager.AddDamage(casterChess.forceId, casterChess.heroId, actualDamage);
            
            if(targetChess.isHero && actualDamage > 0)
                BattleStatManager.AddBeDamaged(targetChess.forceId, targetChess.heroId, actualDamage);

            BattleManager.Instance.AddBattleText("-" + (Damage).ToString(), targetChess.position, new UnityEngine.Vector2(0, 60), SysColor.Battle.DamageColor, 7);

            targetChess.OnHpChanged();
        }
    }
}