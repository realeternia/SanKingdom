using System;
using CommonConfig;
using Controls.Utils;

[System.Serializable]
public class CreateChessAction : ChessAction
{
    public int Id;
    public int ForceId;
    public int BattleUnitId;
    public UnityEngine.Vector3 SpawnPos;
    public bool IsHero;
    public int HeroId;
    public int Level;
    public int SoldierNum;
    public int ArmsId;
    public int Atk;
    public int Def;
    public bool IsFakeHero;
    public float SummonTime;

    public int Str;
    public int LeadShip;
    public int Inte;

    [NonSerialized]
    public Action<int> CallBack;


    public CreateChessAction(int sourceId, int tick, int id, int forceId, int heroId, int level, int soldierNum, int armsId, int atk, int def, int str, int leadShip, int inte, UnityEngine.Vector3 spawnPos)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        SpawnPos = spawnPos;
        IsHero = true;
        HeroId = heroId;
        Level = level;
        SoldierNum = soldierNum;
        ArmsId = armsId;
        Atk = atk;
        Def = def;
        Str = str;
        LeadShip = leadShip;
        Inte = inte;
    }
    public CreateChessAction(int sourceId, int tick, int id, int forceId, int battleUnitId, int soldierNum, int armsId, int atk, int def, UnityEngine.Vector3 spawnPos, float summonTime, Action<int> cb)
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
        Str = 50;
        LeadShip = 50;
        Inte = 50;
    }

    public override void Doing()
    {
        GameLog.Info($"CreateChessAction {Id} {ForceId} {SpawnPos} {IsHero} {HeroId} {Level} {SoldierNum}");

        var battleManager = BattleManager.Instance;
        var chessObj = new Chess(Id);
        chessObj.forceId = ForceId;
        chessObj.position = SpawnPos;
        chessObj.atk = Atk;
        chessObj.def = Def;     
        chessObj.armsId = ArmsId;           
        chessObj.maxHp = SoldierNum;        
        chessObj.str = Str;
        chessObj.leadShip = LeadShip;
        chessObj.inte = Inte;

        if (IsHero)
        {
            chessObj.isHero = true;
            chessObj.heroId = HeroId;
            chessObj.level = Level;
        }
        else
        {
            var battleUnitCfg = BattleUnitConfig.GetConfig(BattleUnitId);
            chessObj.battleUnitId = BattleUnitId;
            chessObj.isHero = false;
            chessObj.isFakeHero = IsFakeHero || battleUnitCfg.Model == "UnitHero";
            chessObj.isShadow = battleUnitCfg.IsShadow;
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