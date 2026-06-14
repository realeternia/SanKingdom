using UnityEngine;

[System.Serializable]
public class CreateEffectAction : ChessAction
{
    public Vector3 TargetPos;
    public string EffectName;
    public float Time;

    public CreateEffectAction(int sourceId, int tick, Vector3 targetPos, string effectName, float time)
        : base(sourceId, tick)
    {
        TargetPos = targetPos;
        EffectName = effectName;
        Time = time;
    }

    public override void Doing()
    {
        var battleManager = BattleManager.Instance;
        var casterChess = battleManager.GetChess(SourceId);

        GameLog.Info($"CreateEffectAction[{ActionId}] src={SourceId} effect={EffectName}");
        EffectManager.PlayPosSkillEffect(casterChess, TargetPos, 1, EffectName, Time);
    }
}