[System.Serializable]
public class AddBuffAction : ChessAction
{
    public int CasterId;
    public int SkillId;
    public int BuffId;
    public float LastTime;

    public AddBuffAction(int sourceId, int tick, int casterId, int skillId, int buffId, float lastTime)
        : base(sourceId, tick)
    {
        CasterId = casterId;
        SkillId = skillId;
        BuffId = buffId;
        LastTime = lastTime;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        var caster = BattleManager.Instance.GetChess(CasterId);
        if (sourceChess != null && caster != null)
        {
            BuffManager.DoAddBuff(sourceChess, caster, SkillId, BuffId, LastTime);
        }
    }
}
