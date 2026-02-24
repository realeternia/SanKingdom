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

    public Chess CreatedChess { get; private set; }

    public CreateChessAction(int sourceId, int tick, int id, int forceId, int soldierId, UnityEngine.Vector3 spawnPos, bool isHero = false, int heroId = 0, int level = 0, int soldierNum = 0, bool isFakeHero = false)
        : base(sourceId, tick)
    {
        Id = id;
        ForceId = forceId;
        SoldierId = soldierId;
        SpawnPos = spawnPos;
        IsHero = isHero;
        HeroId = heroId;
        Level = level;
        SoldierNum = soldierNum;
        IsFakeHero = isFakeHero;
    }

    public override void Doing()
    {
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

            if (chessObj.heroInfo != null)
                chessObj.heroInfo.SetAttr(chessObj.inte, chessObj.str, chessObj.leadShip);
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
        
        CreatedChess = chessObj;
    }
}