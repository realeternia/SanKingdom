using UnityEngine;
using CommonConfig;

[System.Serializable]
public class AttackAction : ChessAction
{
    public int TargetId;
    public string HitEffect;
    public string DamType;
    public bool IsRanged;

    public AttackAction(int sourceId, float time, int targetId, string hitEffect, string damType, bool isRanged)
        : base(sourceId, time)
    {
        TargetId = targetId;
        HitEffect = hitEffect;
        DamType = damType;
        IsRanged = isRanged;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        var targetChess = BattleManager.Instance.GetChess(TargetId);

        if (sourceChess == null || targetChess == null)
            return;

        GameLog.Info($"AttackAction[{ActionId}] src={SourceId} tgt={TargetId}");

        sourceChess?.viewObj?.FaceTo(targetChess.position);

        var armsConfig = ArmsConfig.GetConfig(sourceChess.armsId);
        var animName = "sodattack";
        if (armsConfig.AttackAnimCount > 1)
        {
            var randIdx = BattleRandom.Range(0, armsConfig.AttackAnimCount);
            animName = randIdx == 0 ? "sodattack" : $"sodattack{randIdx}";
        }
        sourceChess?.viewObj?.PlaySodAnim(animName);

        // 伤害计算（从Chess.Attack移入）
        var (damage, isCrit, isDodge, effect) = Chess.CalculateAttackDamage(sourceChess, targetChess, DamType, HitEffect);

        if (damage <= 0 && !isCrit && !isDodge)
            return;

        var battleManager = BattleManager.Instance;

        // 命中延迟：先播攻击动画，延迟HitDelay秒后结算伤害
        if (armsConfig.HitDelay > 0)
        {
            battleManager.DelayedCall(armsConfig.HitDelay, () =>
            {
                var src = battleManager.GetChess(SourceId);
                var tgt = battleManager.GetChess(TargetId);
                if (src == null || tgt == null)
                    return;
                if (IsRanged)
                    battleManager.CreateAttackMissile(src, tgt, damage, isCrit, isDodge, effect, DamType, ActionId);
                else
                    src.OnAttackDamage(tgt, damage, isCrit, isDodge, effect, DamType, ActionId);
            });
        }
        else if (IsRanged)
        {
            battleManager.CreateAttackMissile(sourceChess, targetChess, damage, isCrit, isDodge, effect, DamType, ActionId);
        }
        else
        {
            sourceChess.OnAttackDamage(targetChess, damage, isCrit, isDodge, effect, DamType, ActionId);
        }
    }
}
