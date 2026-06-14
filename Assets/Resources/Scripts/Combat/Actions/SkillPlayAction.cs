[System.Serializable]
public class SkillPlayAction : ChessAction
{
    public int TargetChessId;
    public int SkillId;

    public int Parm1;

    public SkillPlayAction(int sourceId, int tick, int targetChessId, int skillId, int parm1)
        : base(sourceId, tick)
    {
        TargetChessId = targetChessId;
        SkillId = skillId;
        Parm1 = parm1;
    }

    public override void Doing()
    {
        var battleManager = BattleManager.Instance;
        var targetChess = battleManager.GetChess(TargetChessId);
        var casterChess = battleManager.GetChess(SourceId);

        if (targetChess != null && casterChess != null)
        {
            var skill = casterChess.skills.Find(x => x.skillId == SkillId);
            if (skill != null)
            {
                GameLog.Info($"SkillPlayAction[{ActionId}] caster={SourceId} tgt={TargetChessId} skill={SkillId}");
                skill.OnPlaySkill(targetChess, Parm1);
            }
            else
            {
                GameLog.Error($"SkillPlayAction[{ActionId}]: Hero{casterChess.heroId} SkillId {SkillId} not found");
            }
        }
    }
}