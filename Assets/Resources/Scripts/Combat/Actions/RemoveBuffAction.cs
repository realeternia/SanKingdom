[System.Serializable]
public class RemoveBuffAction : ChessAction
{
    public int BuffId;

    public RemoveBuffAction(int sourceId, float time, int buffId)
        : base(sourceId, time)
    {
        BuffId = buffId;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        if (sourceChess != null)
        {
            GameLog.Info($"RemoveBuffAction[{ActionId}] tgt={SourceId} buff={BuffId}");
            BuffManager.DoRemoveBuff(sourceChess, BuffId, ActionId);
        }
    }
}
