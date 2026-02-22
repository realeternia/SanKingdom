public class MoveAction : ChessAction
{
    public int TargetId;
    public UnityEngine.Vector3 TargetPosition;

    public override void Doing(Chess chess)
    {
        BattleManager.Instance.MoveTo(chess, TargetPosition, true);
    }
}
