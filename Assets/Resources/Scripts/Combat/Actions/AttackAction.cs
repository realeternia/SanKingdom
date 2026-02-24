[System.Serializable]
public class AttackAction : ChessAction
{
    public int TargetId;
    public int Damage;
    public bool IsCrit;
    public bool IsDodge;
    public string HitEffect;
    public string DamType;

    public AttackAction(int sourceId, int tick, int targetId, int damage, bool isCrit, bool isDodge, string hitEffect, string damType)
        : base(sourceId, tick)
    {
        TargetId = targetId;
        Damage = damage;
        IsCrit = isCrit;
        IsDodge = isDodge;
        HitEffect = hitEffect;
        DamType = damType;
    }

    public override void Doing()
    {
        var targetChess = BattleManager.Instance.GetChess(TargetId);
        targetChess.OnAttackDamaged(Damage, DamType, HitEffect, IsCrit, IsDodge, SourceId);
    }
}
