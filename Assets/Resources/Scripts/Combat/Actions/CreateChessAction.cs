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
    public int SummonRound;

    public int Inte;
    public int NoActionCount;

    /// <summary>
    /// 出生特效名，非空时先播放特效，延迟片刻后再真正创建棋子
    /// </summary>
    public string SpawnEffect;

    [NonSerialized]
    public Action<int> CallBack;


    public CreateChessAction(int sourceId, float time, int id, int forceId, int heroId, int heroId2, int heroId3, int level, int soldierNum, int armsId, int atk, int def, int inte, UnityEngine.Vector3 spawnPos)
        : base(sourceId, time)
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

    public CreateChessAction(int sourceId, float time, int id, int forceId, int battleUnitId, int soldierNum, int armsId, int atk, int def, UnityEngine.Vector3 spawnPos, int summonRound, Action<int> cb, int noActionCount = 0)
        : base(sourceId, time)
    {
        Id = id;
        ForceId = forceId;
        BattleUnitId = battleUnitId;
        SoldierNum = soldierNum;
        ArmsId = armsId;
        Atk = atk;
        Def = def;
        SpawnPos = spawnPos;
        SummonRound = summonRound;
        CallBack = cb;
        Inte = 50;
        NoActionCount = noActionCount;
    }

    // 出生特效延迟创建副本：时间顺延delaySeconds，且不再重复播放特效
    private CreateChessAction(CreateChessAction src, float delaySeconds)
        : base(src.SourceId, src.Time + delaySeconds)
    {
        Id = src.Id;
        ForceId = src.ForceId;
        SpawnPos = src.SpawnPos;
        IsHero = src.IsHero;
        HeroId = src.HeroId;
        HeroId2 = src.HeroId2;
        HeroId3 = src.HeroId3;
        Level = src.Level;
        SoldierNum = src.SoldierNum;
        ArmsId = src.ArmsId;
        Atk = src.Atk;
        Def = src.Def;
        IsFakeHero = src.IsFakeHero;
        SummonRound = src.SummonRound;
        Inte = src.Inte;
        NoActionCount = src.NoActionCount;
        CallBack = src.CallBack;
    }

    public override void Doing()
    {
        var battleManager = BattleManager.Instance;

        // 出生特效：先播放特效，延迟片刻后再真正创建棋子
        if (!string.IsNullOrEmpty(SpawnEffect))
        {
            EffectManager.PlayPosSkillEffect(null, SpawnPos, 1f, SpawnEffect, SystemConst.Battle.SUMMON_EFFECT_DURATION);
            battleManager.AddChessAction(new CreateChessAction(this, SystemConst.Battle.SUMMON_HERO_DELAY_SECONDS));
            return;
        }

        GameLog.Info($"CreateChessAction[{ActionId}] {Id} {ForceId} {SpawnPos} {IsHero} {HeroId} {Level} {SoldierNum}");

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
            else if (battleUnitCfg.UnitType == 3)
            {
                chessObj.isTower = true;
            }
            else
            {
                chessObj.isFakeHero = IsFakeHero || battleUnitCfg.Model == "UnitHero";
                chessObj.isShadow = battleUnitCfg.IsShadow;
            }
        }

        chessObj.hp = chessObj.maxHp;
        chessObj.noActionCount = NoActionCount;
        battleManager.chessList.Add(chessObj);
        // 所有棋子（含城门/城墙/箭塔）占格，一格最多一棋
        battleManager.OccupyGrid(chessObj.id, chessObj.position);
        chessObj.Init(ForceId);

        if (SummonRound > 0)
            chessObj.SetLifeRound(SummonRound);

        if (CallBack != null)
            CallBack(Id);
        
        
    }
}