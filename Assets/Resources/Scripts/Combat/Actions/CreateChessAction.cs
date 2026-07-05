using System;
using CommonConfig;
[System.Serializable]
public class CreateChessAction : ChessAction
{
    public int Id;
    public int ForceId;
    public int BattleUnitId;
    public UnityEngine.Vector3 SpawnPos;
    public bool IsHero;
    public int HeroId;
    public int HeroId2;
    public int HeroId3;
    public int Level;
    public int SoldierNum;
    public int ArmsId;
    public int Atk;
    public int Def;
    public bool IsFakeHero;
    public float SummonTime;

    public int Inte;
    public int NoActionCount;

    [NonSerialized]
    public Action<int> CallBack;


    public CreateChessAction(int sourceId, int tick, int id, int forceId, int heroId, int heroId2, int heroId3, int level, int soldierNum, int armsId, int atk, int def, int inte, UnityEngine.Vector3 spawnPos)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        SpawnPos = spawnPos;
        IsHero = true;
        HeroId = heroId;
        HeroId2 = heroId2;
        HeroId3 = heroId3;
        Level = level;
        SoldierNum = soldierNum;
        ArmsId = armsId;
        Atk = atk;
        Def = def;
        Inte = inte;
    }

    public CreateChessAction(int sourceId, int tick, int id, int forceId, int battleUnitId, int soldierNum, int armsId, int atk, int def, UnityEngine.Vector3 spawnPos, float summonTime, Action<int> cb, int noActionCount = 0)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        BattleUnitId = battleUnitId;
        SoldierNum = soldierNum;
        ArmsId = armsId;
        Atk = atk;
        Def = def;
        SpawnPos = spawnPos;
        SummonTime = summonTime;
        CallBack = cb;
        Inte = 50;
        NoActionCount = noActionCount;
    }

    public override void Doing()
    {
        GameLog.Info($"CreateChessAction[{ActionId}] {Id} {ForceId} {SpawnPos} {IsHero} {HeroId} {Level} {SoldierNum}");

        var battleManager = BattleManager.Instance;
        var chessObj = new Chess(Id);
        chessObj.forceId = ForceId;
        chessObj.position = SpawnPos;
        chessObj.atk = Atk;
        chessObj.def = Def;     
        chessObj.armsId = ArmsId;           
        chessObj.maxHp = SoldierNum;        
        chessObj.inte = Inte;

        if (IsHero)
        {
            chessObj.isHero = true;
            chessObj.heroId = HeroId;
            chessObj.heroId2 = HeroId2;
            chessObj.heroId3 = HeroId3;
            chessObj.level = Level;
        }
        else
        {
            var battleUnitCfg = BattleUnitConfig.GetConfig(BattleUnitId);
            chessObj.battleUnitId = BattleUnitId;
            chessObj.isHero = false;

            if (battleUnitCfg.UnitType == 1)
                chessObj.isGate = true;
            else if (battleUnitCfg.UnitType == 2)
                chessObj.isWall = true;
            else
            {
                chessObj.isFakeHero = IsFakeHero || battleUnitCfg.Model == "UnitHero";
                chessObj.isShadow = battleUnitCfg.IsShadow;
            }
        }

        chessObj.hp = chessObj.maxHp;
        chessObj.noActionCount = NoActionCount;
        battleManager.chessList.Add(chessObj);
        if (!chessObj.isGate)
            battleManager.OccupyGrid(chessObj.id, chessObj.position);
        chessObj.Init(ForceId);

        if (SummonTime > 0)
            chessObj.SetLifeTime(SummonTime);

        if (CallBack != null)
            CallBack(Id);
        
        
    }
}