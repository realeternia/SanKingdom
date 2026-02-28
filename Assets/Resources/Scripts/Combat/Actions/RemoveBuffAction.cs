[System.Serializable]
public class RemoveBuffAction : ChessAction
{
    public int BuffId;

    public RemoveBuffAction(int sourceId, int tick, int buffId)
        : base(sourceId, tick)
    {
        BuffId = buffId;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        if (sourceChess != null)
        {
            BuffManager.DoRemoveBuff(sourceChess, BuffId);
        }
    }
}
