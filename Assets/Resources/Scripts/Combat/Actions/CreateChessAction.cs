using System;
using CommonConfig;

[System.Serializable]
public class CreateChessAction : ChessAction
{
    public int Id;
    public int ForceId;
    public int SoldierId;
    public UnityEngine.Vector3 SpawnPos;
    public bool IsHero;
    public int HeroId;
    public int Level;
    public int SoldierNum;
    public bool IsFakeHero;
    public float SummonTime;

    [NonSerialized]
    public Action<int> CallBack;


    public CreateChessAction(int sourceId, int tick, int id, int forceId, UnityEngine.Vector3 spawnPos, int heroId, int level, int soldierNum)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        SpawnPos = spawnPos;
        IsHero = true;
        HeroId = heroId;
        Level = level;
        SoldierNum = soldierNum;
    }

    public CreateChessAction(int sourceId, int tick, int id, int forceId, int soldierId, UnityEngine.Vector3 spawnPos, float summonTime, Action<int> cb)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        SoldierId = soldierId;
        SpawnPos = spawnPos;
        SummonTime = summonTime;
        CallBack = cb;
    }

    public override void Doing()
    {
        UnityEngine.Debug.Log($"CreateChessAction {Id} {ForceId} {SpawnPos} {IsHero} {HeroId} {Level} {SoldierNum}");

        var battleManager = BattleManager.Instance;
        var chessObj = new Chess(Id);
        chessObj.forceId = ForceId;
        chessObj.position = SpawnPos;

        if (IsHero)
        {
            chessObj.isHero = true;
            chessObj.heroId = HeroId;
            chessObj.level = Level;

            var attr = HeroSelectionTool.GetCardAttr(HeroId, Level);

            chessObj.maxHp = SoldierNum;
            chessObj.inte = attr.Inte;
            chessObj.str = attr.Str;
            chessObj.leadShip = attr.Lead;
        }
        else
        {
            chessObj.isHero = false;
            var soldierConfig = SoldierConfig.GetConfig(SoldierId);
            chessObj.maxHp = soldierConfig.Hp;
            chessObj.isFakeHero = IsFakeHero || soldierConfig.Model == "UnitHero";
            chessObj.soldierId = SoldierId;
        }

        chessObj.hp = chessObj.maxHp;
        battleManager.chessList.Add(chessObj);
        chessObj.Init(ForceId);

        if (SummonTime > 0)
            chessObj.SetLifeTime(SummonTime);

        if (CallBack != null)
            CallBack(Id);
        
        
    }
}