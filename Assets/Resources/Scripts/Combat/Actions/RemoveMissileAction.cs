[System.Serializable]
public class RemoveMissileAction : ChessAction
{
    public int MissileId;

    public RemoveMissileAction(int sourceId, int tick, int missileId)
        : base(sourceId, tick)
    {
        MissileId = missileId;
    }

    public override void Doing()
    {
        var missile = BattleManager.Instance.GetMissile(MissileId);
        if (missile != null)
        {
            if (missile.viewObj != null)
            {
                UnityEngine.Object.Destroy(missile.viewObj.gameObject);
            }
        }
        BattleManager.Instance.missileList.RemoveAll(m => m.id == MissileId);
    }
}
