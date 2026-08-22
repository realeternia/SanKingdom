[System.Serializable]
public class DisplaceAction : ChessAction
{
    public int TargetId;
    public int TargetCellId;

    public DisplaceAction(int sourceId, float time, int targetId, int targetCellId)
        : base(sourceId, time)
    {
        TargetId = targetId;
        TargetCellId = targetCellId;
    }

    public override void Doing()
    {
        var chess = BattleManager.Instance.GetChess(TargetId);
        if (chess == null)
        {
            GameLog.Error($"DisplaceAction[{ActionId}] TargetId not found {TargetId}");
            return;
        }
        var cell = BattleManager.Instance.GetMapCellById(TargetCellId);
        if (cell == null)
        {
            GameLog.Error($"DisplaceAction[{ActionId}] TargetCellId not found {TargetCellId}");
            return;
        }
        GameLog.Info($"DisplaceAction[{ActionId}] src={TargetId} cell={TargetCellId} pos={cell.worldPos}");
        chess?.viewObj?.FaceTo(cell.worldPos);
        // 执行时再校验一次可通行，目标格已被占用则放弃位移
        if (BattleManager.Instance.MoveToCell(chess, cell.id, false))
            chess?.viewObj?.PlaySodAnim("sodmove");
    }
}
