public class BuffNoAction : Buff
{
    public BuffNoAction(int id, int skillId, Chess caster, Chess target, int endRound)
     : base(id, skillId, caster, target, endRound)
    {
    }

    public override void OnAdd(Chess chess, Chess caster)
    {
        base.OnAdd(chess, caster);
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.noActionCount++;
    }

    public override void OnRemove(Chess chess)
    {
        var owner = BattleManager.Instance.GetChess(ownerId);
        owner.noActionCount--;
        base.OnRemove(chess);
    }

}