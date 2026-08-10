[System.Serializable]
public class AddCellEffectAction : ChessAction
{
    public int CellId;
    public CellEffect effect;

    public AddCellEffectAction(int sourceId, float time, int cellId, CellEffect effect)
        : base(sourceId, time)
    {
        CellId = cellId;
        this.effect = effect;
    }

    public override void Doing()
    {
        GameLog.Info($"AddCellEffectAction[{ActionId}] cellId={CellId} skillId={effect.skillId}");
        BattleManager.Instance.DoAddCellEffect(CellId, effect);
    }
}
