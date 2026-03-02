using System;
using UnityEngine;

[System.Serializable]
public class ChessChangeHpAction : ChessAction
{
    public int Value;

    public ChessChangeHpAction(int sourceId, int tick, int changeVal)
        : base(sourceId, tick)
    {
        Value = changeVal;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        if (sourceChess != null)
        {
            if(Value == 0)
                return;
            
            var hpval = Math.Clamp(sourceChess.hp + Value, 1, sourceChess.maxHp);
            sourceChess.hp = hpval;
            sourceChess.OnHpChanged();
        }
    }
}
