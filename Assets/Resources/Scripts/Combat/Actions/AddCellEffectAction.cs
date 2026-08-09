[System.Serializable]
public class AddCellEffectAction : ChessAction
{
    public int gridX;
    public int gridZ;
    public CellEffect effect;

    public AddCellEffectAction(int sourceId, int tick, int gridX, int gridZ, CellEffect effect)
        : base(sourceId, tick)
    {
        this.gridX = gridX;
        this.gridZ = gridZ;
        this.effect = effect;
    }

    public override void Doing()
    {
        GameLog.Info($"AddCellEffectAction[{ActionId}] gridX={gridX} gridZ={gridZ} skillId={effect.skillId}");
        BattleManager.Instance.DoAddCellEffect(gridX, gridZ, effect);
    }
}
