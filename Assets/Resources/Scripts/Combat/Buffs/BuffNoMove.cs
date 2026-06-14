public class BuffNoMove : Buff
{
    public BuffNoMove(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.noMoveCount++;
    }

    public override void OnRemove(Chess chess)
    {
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.noMoveCount--;
        base.OnRemove(chess);
    }

}