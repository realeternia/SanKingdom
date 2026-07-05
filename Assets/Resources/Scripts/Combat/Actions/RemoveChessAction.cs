using UnityEngine;
[System.Serializable]
public class RemoveChessAction : ChessAction
{
    public RemoveChessAction(int sourceId, int tick)
        : base(sourceId, tick)
    {
    }

    public override void Doing()
    {
        var ownerChess = BattleManager.Instance.GetChess(SourceId);
        if (ownerChess != null)
        {
            ownerChess.buffs.Clear();
            
            if (ownerChess.viewObj != null)
            {
                ownerChess.viewObj.DestroyHUD();
            }
            GameLog.Info($"RemoveChessAction[{ActionId}] {ownerChess.id}");
            if (ownerChess.viewObj != null)
            {
                UnityEngine.Object.Destroy(ownerChess.viewObj.gameObject);
                ownerChess.viewObj = null;
            }

            if ((ownerChess.forceId == 1 || ownerChess.forceId == 2 || ownerChess.isGate) && !ownerChess.isShadow && BattleManager.Instance.showUI)
                BGMPlayer.Instance.PlaySound("Sounds/tnt", 7);

            BattleManager.Instance.ReleaseGrid(ownerChess.id);
            BattleManager.Instance.OnUnitDying(ownerChess);
        }
    }
}
