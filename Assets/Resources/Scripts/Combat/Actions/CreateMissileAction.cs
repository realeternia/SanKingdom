[System.Serializable]
public class CreateMissileAction : ChessAction
{
    public int Id;
    public int SourceChessId;
    public int TargetChessId;
    public UnityEngine.Vector3 TargetPos;
    public UnityEngine.Vector3 StartPos;
    public int SkillId;
    public int Damage;
    public float Time;
    public bool IsDirectional;

    public CreateMissileAction(int sourceId, int tick, int id, int sourceChessId, int targetChessId, UnityEngine.Vector3 startPos, int skillId, int damage)
    {
        SourceId = sourceId;
        Tick = tick;
        Id = id;
        SourceChessId = sourceChessId;
        TargetChessId = targetChessId;
        StartPos = startPos;
        SkillId = skillId;
        Damage = damage;
        IsDirectional = false;
    }

    public CreateMissileAction(int sourceId, int tick, int id, int sourceChessId, UnityEngine.Vector3 targetPos, UnityEngine.Vector3 startPos, int skillId, int damage, float time)
    {
        SourceId = sourceId;
        Tick = tick;
        Id = id;
        SourceChessId = sourceChessId;
        TargetPos = targetPos;
        StartPos = startPos;
        SkillId = skillId;
        Damage = damage;
        Time = time;
        IsDirectional = true;
    }

    public override void Doing(Chess chess)
    {
        var battleManager = BattleManager.Instance;
        var sourceChess = battleManager.GetChess(SourceChessId);
        var missile = new Missile(Id, sourceChess, StartPos, SkillId, Damage);
        missile.Init();
        battleManager.missileList.Add(missile);

        if (IsDirectional)
        {
            missile.MoveToDirection(TargetPos, Time);
        }
        else
        {
            var targetChess = battleManager.GetChess(TargetChessId);
            if (targetChess != null)
            {
                missile.MoveToTarget(targetChess);
            }
        }
    }
}