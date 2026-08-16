[System.Serializable]
public class MoveAction : ChessAction
{
    public int TargetId;
    public int TargetCellId;

    public MoveAction(int sourceId, float time, int targetId, int targetCellId)
        : base(sourceId, time)
    {
        TargetId = targetId;
        TargetCellId = targetCellId;
    }

    public override void Doing()
    {
        var chess = BattleManager.Instance.GetChess(SourceId);
        if(chess == null)
        {
            GameLog.Error($"MoveAction[{ActionId}] SourceId not found {SourceId}");
            return;
        }
        var cell = BattleManager.Instance.GetMapCellById(TargetCellId);
        if (cell == null)
        {
            GameLog.Error($"MoveAction[{ActionId}] TargetCellId not found {TargetCellId}");
            return;
        }
        GameLog.Info($"MoveAction[{ActionId}] src={SourceId} cell={TargetCellId} pos={cell.worldPos}");
        chess?.viewObj?.FaceTo(cell.worldPos);
        BattleManager.Instance.MoveToCell(chess, cell.id, true);
        chess?.viewObj?.PlaySodAnim("sodmove");
    }
}
