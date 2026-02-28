using System;
using UnityEngine;

[System.Serializable]
public class AddHpAction : ChessAction
{
    public int TargetId;
    public int Addon;

    public AddHpAction(int sourceId, int tick, int targetId, int addon)
        : base(sourceId, tick)
    {
        TargetId = targetId;
        Addon = addon;
    }

    public override void Doing()
    {
        var targetChess = BattleManager.Instance.GetChess(TargetId);
        if (targetChess != null)
        {
            targetChess.hp = Mathf.Clamp(targetChess.hp + Addon, 0, targetChess.maxHp);
            targetChess.OnHpChanged();
        }
    }
}
