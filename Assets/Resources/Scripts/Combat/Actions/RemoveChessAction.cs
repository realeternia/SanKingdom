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
            Debug.Log("RemoveChessAction " + ownerChess.id);
            if (ownerChess.viewObj != null)
            {
                UnityEngine.Object.Destroy(ownerChess.viewObj.gameObject);
                ownerChess.viewObj = null;
            }

            if ((ownerChess.forceId == 1 || ownerChess.forceId == 2) && !ownerChess.isShadow)
                BGMPlayer.Instance.PlaySound("Sounds/tnt", 7);

            BattleManager.Instance.OnUnitDying(ownerChess);
        }
    }
}
