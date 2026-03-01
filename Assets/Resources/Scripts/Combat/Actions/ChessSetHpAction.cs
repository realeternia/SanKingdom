using System;
using UnityEngine;

[System.Serializable]
public class ChessSetHpAction : ChessAction
{
    public int Value;

    public ChessSetHpAction(int sourceId, int tick, int value)
        : base(sourceId, tick)
    {
        Value = value;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        if (sourceChess != null)
        {
            sourceChess.hp = Value;
            sourceChess.OnHpChanged();
        }
    }
}
