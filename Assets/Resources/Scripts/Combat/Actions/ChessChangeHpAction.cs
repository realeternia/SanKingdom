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

            GameLog.Info($"ChessChangeHpAction[{ActionId}] chess={SourceId} val={Value}");

            var hpval = Math.Clamp(sourceChess.hp + Value, 1, sourceChess.maxHp);
            sourceChess.hp = hpval;

            if(Value > 0)
                BattleManager.Instance.AddBattleText("+" + Value.ToString(), sourceChess.position, new UnityEngine.Vector2(0, 60), SysColor.Battle.HealColor, 7);

            sourceChess.OnHpChanged();
        }
    }
}
