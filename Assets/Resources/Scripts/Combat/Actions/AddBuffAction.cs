[System.Serializable]
public class AddBuffAction : ChessAction
{
    public int CasterId;
    public int SkillId;
    public int BuffId;
    public int LastRounds;

    public AddBuffAction(int sourceId, float time, int casterId, int skillId, int buffId, int lastRounds)
        : base(sourceId, time)
    {
        CasterId = casterId;
        SkillId = skillId;
        BuffId = buffId;
        LastRounds = lastRounds;
    }

    public override void Doing()
    {
        var sourceChess = BattleManager.Instance.GetChess(SourceId);
        var caster = BattleManager.Instance.GetChess(CasterId);
        if (sourceChess != null && caster != null)
        {
            GameLog.Info($"AddBuffAction[{ActionId}] tgt={SourceId} caster={CasterId} skill={SkillId} buff={BuffId}");
            BuffManager.DoAddBuff(sourceChess, caster, SkillId, BuffId, LastRounds, ActionId);
        }
    }
}
