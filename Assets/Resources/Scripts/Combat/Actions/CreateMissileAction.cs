[System.Serializable]
public class CreateMissileAction : ChessAction
{
    public int Id;
    public int TargetChessId;
    public UnityEngine.Vector3 TargetPos;
    public UnityEngine.Vector3 StartPos;
    public int SkillId;
    public int Damage;
    public float Time;
    public bool IsDirectional;

    public CreateMissileAction(int sourceId, int tick, int id, int targetChessId, UnityEngine.Vector3 startPos, int skillId, int damage)
        : base(sourceId, tick)
    {
        Id = id;
        TargetChessId = targetChessId;
        StartPos = startPos;
        SkillId = skillId;
        Damage = damage;
        IsDirectional = false;
    }

    public CreateMissileAction(int sourceId, int tick, int id, UnityEngine.Vector3 targetPos, UnityEngine.Vector3 startPos, int skillId, int damage, float time)
        : base(sourceId, tick)
    {
        Id = id;
        TargetPos = targetPos;
        StartPos = startPos;
        SkillId = skillId;
        Damage = damage;
        Time = time;
        IsDirectional = true;
    }

    public override void Doing()
    {
        var battleManager = BattleManager.Instance;
        var sourceChess = battleManager.GetChess(SourceId);
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