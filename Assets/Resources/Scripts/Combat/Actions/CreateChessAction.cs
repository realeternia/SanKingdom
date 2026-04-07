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

    public CreateChessAction(int sourceId, int tick, int id, int forceId, int battleUnitId, UnityEngine.Vector3 spawnPos, float summonTime, Action<int> cb)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        BattleUnitId = battleUnitId;
        SpawnPos = spawnPos;
        SummonTime = summonTime;
        CallBack = cb;
    }

    public override void Doing()
    {
        GameLog.Info($"CreateChessAction {Id} {ForceId} {SpawnPos} {IsHero} {HeroId} {Level} {SoldierNum}");

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
            var battleUnitCfg = BattleUnitConfig.GetConfig(BattleUnitId);
            chessObj.maxHp = battleUnitCfg.Hp;
            chessObj.isFakeHero = IsFakeHero || battleUnitCfg.Model == "UnitHero";
            chessObj.isShadow = battleUnitCfg.IsShadow;
            chessObj.battleUnitId = BattleUnitId;
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