using UnityEngine;
using CommonConfig;

[System.Serializable]
public class AttackAction : ChessAction
{
    public int TargetId;
    public string HitEffect;
    public string DamType;
    public bool IsRanged;

    public AttackAction(int sourceId, int tick, int targetId, string hitEffect, string damType, bool isRanged)
        : base(sourceId, tick)
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

        sourceChess?.viewObj?.FaceTo(targetChess.position);
        sourceChess?.viewObj?.PlaySodAnim("sodattack");

        // 伤害计算（从Chess.Attack移入）
        var (damage, isCrit, isDodge, effect) = Chess.CalculateAttackDamage(sourceChess, targetChess, DamType, HitEffect);

        if (damage <= 0 && !isCrit && !isDodge)
            return;

        // 根据ArmsConfig.HitDelay延迟伤害结算
        var armsConfig = ArmsConfig.GetConfig(sourceChess.armsId);
        var hitDelayTicks = BattleManager.Instance.GetTickFromTime(armsConfig.HitDelay);

        if (hitDelayTicks <= 0)
        {
            if (IsRanged)
                BattleManager.Instance.CreateAttackMissile(sourceChess, targetChess, damage, isCrit, isDodge, effect, DamType);
            else
                sourceChess.OnAttackDamage(targetChess, damage, isCrit, isDodge, effect, DamType);
        }
        else
        {
            var hitAction = new AttackHitAction(SourceId, Tick + hitDelayTicks, TargetId, damage, isCrit, isDodge, effect, DamType, IsRanged);
            BattleManager.Instance.AddChessAction(hitAction);
        }
    }
}
