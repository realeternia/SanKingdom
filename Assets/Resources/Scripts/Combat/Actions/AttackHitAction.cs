[System.Serializable]
public class AttackHitAction : ChessAction
{
    public int TargetId;
    public int Damage;
    public bool IsCrit;
    public bool IsDodge;
    public string HitEffect;
    public string DamType;
    public bool IsRanged;

    public AttackHitAction(int sourceId, int tick, int targetId, int damage, bool isCrit, bool isDodge, string hitEffect, string damType, bool isRanged)
        : base(sourceId, tick)
    {
        TargetId = targetId;
        Damage = damage;
        IsCrit = isCrit;
        IsDodge = isDodge;
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

        if (IsRanged)
        {
            BattleManager.Instance.CreateAttackMissile(sourceChess, targetChess, Damage, IsCrit, IsDodge, HitEffect, DamType);
        }
        else
        {
            sourceChess.OnAttackDamage(targetChess, Damage, IsCrit, IsDodge, HitEffect, DamType);
        }
    }
}
