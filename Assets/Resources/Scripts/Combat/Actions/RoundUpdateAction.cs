[System.Serializable]
public class RoundUpdateAction : ChessAction
{
    public int Round;

    public RoundUpdateAction(int sourceId, float time, int round)
        : base(sourceId, time)
    {
        Round = round;
    }

    public override void Doing()
    {
        GameLog.Info($"RoundUpdateAction[{ActionId}] round={Round}");
        BattleManager.Instance.round = Round;
        if (BattleManager.Instance.showUI)
            BattleInfoTop.Instance.UpdateRound(Round, BattleManager.MaxRound);
    }
}
