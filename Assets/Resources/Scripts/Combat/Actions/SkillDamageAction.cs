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
            targetChess.OnSkillDamaged(casterChess, SkillId, Damage);
        }
    }
}