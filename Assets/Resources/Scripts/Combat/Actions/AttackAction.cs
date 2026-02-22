public class AttackAction : ChessAction
{
    public int TargetId;
    public int Damage;
    public bool IsCrit;
    public bool IsDodge;
    public string HitEffect;

    public override void Doing(Chess chess)
    {
        var targetChess = BattleManager.Instance.GetChess(TargetId);
        targetChess.OnAttackDamaged(Damage, IsCrit, IsDodge, chess.id);
    }
}
