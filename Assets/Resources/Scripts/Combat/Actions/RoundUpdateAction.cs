[System.Serializable]
public class RoundUpdateAction : ChessAction
{
    public int Round;

    public RoundUpdateAction(int sourceId, int tick, int round)
        : base(sourceId, tick)
    {
        Round = round;
    }

    public override void Doing()
    {
        BattleManager.Instance.round = Round;
        if (BattleManager.Instance.showUI)
            BattleInfoTop.Instance.UpdateRound(Round, BattleManager.MaxRound);
    }
}
