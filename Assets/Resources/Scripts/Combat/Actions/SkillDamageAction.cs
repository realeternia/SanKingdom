using CommonConfig;
using UnityEngine;

[System.Serializable]
public class SkillDamageAction : ChessAction
{
    public int TargetChessId;
    public int SkillId;
    public int Damage;

    public SkillDamageAction(int sourceId, float time, int targetChessId, int skillId, int damage)
        : base(sourceId, time)
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
            GameLog.Info($"SkillDamageAction[{ActionId}] caster={SourceId} tgt={TargetChessId} skill={SkillId} dmg={Damage}");

            var actualDamage = Mathf.Min(Damage, targetChess.hp);
            targetChess.hp -= Damage;
            if(casterChess != targetChess)
                targetChess.lastDamagedPlayerId = casterChess.forceId;

            // 城门血量同步：一扇门受伤，其余城门同损（回放时同步确定性由同一条 Action 保证）
            if (targetChess.isGate)
                battleManager.SyncGateDamage(targetChess, Damage);

            var skillCfg = BattleSkillConfig.GetConfig(SkillId);
            if(!string.IsNullOrEmpty(skillCfg.EffectHit))
                EffectManager.PlaySkillEffect(targetChess, skillCfg.EffectHit);

            if(casterChess.isHero && actualDamage > 0)
                BattleStatManager.AddDamage(casterChess.forceId, casterChess.heroId, actualDamage);
            
            if(targetChess.isHero && actualDamage > 0)
                BattleStatManager.AddBeDamaged(targetChess.forceId, targetChess.heroId, actualDamage);

            BattleManager.Instance.AddBattleText("-" + (Damage).ToString(), targetChess.position, new UnityEngine.Vector2(0, 60), SysColor.Battle.DamageColor, 7);

            targetChess.OnHpChanged();

            // 技能伤害结算后结束施法者的待定回合
            casterChess.FinishPendingAction();
        }
    }
}