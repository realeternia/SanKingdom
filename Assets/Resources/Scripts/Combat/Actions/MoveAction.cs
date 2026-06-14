[System.Serializable]
public class MoveAction : ChessAction
{
    public int TargetId;
    public UnityEngine.Vector3 TargetPosition;

    public MoveAction(int sourceId, int tick, int targetId, UnityEngine.Vector3 targetPosition)
        : base(sourceId, tick)
    {
        TargetId = targetId;
        TargetPosition = targetPosition;
    }

    public override void Doing()
    {
        var chess = BattleManager.Instance.GetChess(SourceId);
        if(chess == null)
        {
            GameLog.Error($"MoveAction[{ActionId}] SourceId not found {SourceId}");
            return;
        }
        GameLog.Info($"MoveAction[{ActionId}] src={SourceId} pos={TargetPosition}");
        chess?.viewObj?.FaceTo(TargetPosition);
        BattleManager.Instance.MoveTo(chess, TargetPosition, true);
        chess?.viewObj?.PlaySodAnim("sodmove");
    }
}
