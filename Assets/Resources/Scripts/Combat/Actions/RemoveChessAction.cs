using UnityEngine;

[System.Serializable]
public class RemoveChessAction : ChessAction
{
    public int TargetId;

    public RemoveChessAction(int sourceId, int tick, int targetId)
        : base(sourceId, tick)
    {
        TargetId = targetId;
    }

    public override void Doing()
    {
        var targetChess = BattleManager.Instance.GetChess(TargetId);
        if (targetChess != null)
        {
            targetChess.buffs.Clear();
            BattleManager.Instance.OnUnitDying(targetChess);

            if (targetChess.viewObj != null)
            {
                targetChess.viewObj.DestroyHUD();
            }
            Debug.Log("OnDying " + targetChess.id);
            if (targetChess.viewObj != null)
            {
                UnityEngine.Object.Destroy(targetChess.viewObj.gameObject);
                targetChess.viewObj = null;
            }

            if ((targetChess.forceId == 1 || targetChess.forceId == 2) && !targetChess.isShadow)
                BGMPlayer.Instance.PlaySound("Sounds/tnt", 7);
        }
    }
}
