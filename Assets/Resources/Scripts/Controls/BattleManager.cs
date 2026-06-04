using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;
public enum BattleResult
{
    Win,
    Lose,
    Draw
}

[Serializable]
public class BattleManager : MonoBehaviour
{
    [Serializable]
    public class BattlePlayerINfo
    {
        public int forceId;

        public int soldierNumInit;
    }
    [NonSerialized]
    public static BattleManager Instance;
    [NonSerialized]
    public BattleUIManager battleUIManager;
    [NonSerialized]
    private GameObject mapObj;

    public int battleId;
    [NonSerialized]
    public int cityId;


    public List<BattlePlayerINfo> playerInfoList = new List<BattlePlayerINfo>();

    public List<Chess> chessList = new List<Chess>(); // 所有棋子
    public List<Missile> missileList = new List<Missile>(); // 所有导弹

    [NonSerialized]
    public List<(int gridX, int gridZ, int chessId)> gridOccupancy = new List<(int, int, int)>();

    [NonSerialized]
    private NLCoroutineManager coroutineManager = new NLCoroutineManager();

    [SerializeReference]
    public List<ChessAction> actions = new List<ChessAction>();    

    [NonSerialized]
    private bool gameFinish = false;
    [NonSerialized]
    private BattleResult battleResult;
    public int idCounter = 100;
    public int tickIndex = 1;
    public int lastFoodDeductionTick = 0;    
    public int round = 0;
    public const int MaxRound = SystemConst.Battle.MAX_ROUND;

    [NonSerialized]
    public bool quickMode = true;
    [NonSerialized]
    public bool showUI = true;

    [NonSerialized]
    private Action<BattleResult, Dictionary<int, int>, Dictionary<int, int>> battleEndCallback;



    [NonSerialized]
    private List<SaveTroopsData> attackTroops;
    [NonSerialized]
    private List<SaveTroopsData> defenderTroops;

    [NonSerialized]
    private bool isDoingAction = false;

    [NonSerialized]
    public bool isDeployPhase = false;
    [NonSerialized]
    private Chess draggingChess = null;
    [NonSerialized]
    private Vector3 dragStartPos;
    [NonSerialized]
    private Dictionary<int, Vector3> playerDeployPositions = new Dictionary<int, Vector3>();
    [NonSerialized]
    private Dictionary<int, int> deployAttackSoldierMap;
    [NonSerialized]
    private Dictionary<int, int> deployDefenderSoldierMap;
    [NonSerialized]
    private int playerSideIndex = -1;


    private void Awake()
    {
        Instance = this;
    }

    public void SetMode(bool quickMode, bool showUI)
    {
        this.quickMode = quickMode;
        this.showUI = showUI;
    }

    [NonSerialized]
    public bool IsBattleRunning = false;
    [NonSerialized]
    private Coroutine currentBattleCoroutine = null;
    
    public void BattleBegin(SaveForceData force1, SaveForceData force2, List<SaveTroopsData> troops1, List<SaveTroopsData> troops2, Dictionary<int, int> soldierMap1, Dictionary<int, int> soldierMap2, int cityId, Action<BattleResult, Dictionary<int, int>, Dictionary<int, int>> callback = null)
    {
        if (IsBattleRunning)
        {
            GameLog.Warn($"BattleBegin called while battle is running, skipping. cityId={cityId}");
            return;
        }
        GameLog.Info($"BattleBegin cityId={cityId}");
        IsBattleRunning = true;
        
        battleEndCallback = callback;
        this.cityId = cityId;
        playerInfoList.Clear();
        playerInfoList.Add(new BattlePlayerINfo() { forceId = force1.forceId, soldierNumInit = GetTotalSoldierCount(soldierMap1) });
        playerInfoList.Add(new BattlePlayerINfo() { forceId = force2.forceId, soldierNumInit = GetTotalSoldierCount(soldierMap2) });

        chessList.Clear();
        missileList.Clear();
        actions.Clear();
        gridOccupancy.Clear();
        idCounter = 100;
        lastFoodDeductionTick = 0;        
        battleId = GameManager.Instance.SaveData.battleStatManager.OnNewBattle();
        SkillManager.isReplay = false;

        gameFinish = false;
        round = 0;
        playerDeployPositions.Clear();

        InitUI(force1, force2);

        attackTroops = troops1;
        defenderTroops = troops2;
        attackTroops.Sort((a, b) => ArmsConfig.GetConfig(a.armsId).Range.CompareTo(ArmsConfig.GetConfig(b.armsId).Range));
        defenderTroops.Sort((a, b) => ArmsConfig.GetConfig(a.armsId).Range.CompareTo(ArmsConfig.GetConfig(b.armsId).Range));

        deployAttackSoldierMap = soldierMap1;
        deployDefenderSoldierMap = soldierMap2;

        if (!quickMode && showUI)
        {
            StartDeployPhase();
        }
        else
        {
            currentBattleCoroutine = StartCoroutine(GameUpdate(soldierMap1, soldierMap2));
        }
    }

    private int GetTotalSoldierCount(Dictionary<int, int> soldierMap)
    {
        int total = 0;
        foreach (var kvp in soldierMap)
        {
            total += kvp.Value;
        }
        return total;
    }

    private void InitUI(SaveForceData force1, SaveForceData force2)
    {
        if (showUI)
        {
            var startTime = Time.realtimeSinceStartup;
            var newMapId = 1;
            var mapNode = ResourceCache.LoadPrefabBattle(ResPath.Prefab.BattleMap(newMapId));
            if (mapObj != null)
                UnityEngine.Object.Destroy(mapObj);

            mapObj = UnityEngine.Object.Instantiate(mapNode, battleUIManager.NodeUnits.transform.parent);
            var endTime = Time.realtimeSinceStartup;
            GameLog.Info("加载地图耗时：" + (endTime - startTime) + "秒");

            battleUIManager.ShowBattleBegin(force1, force2, MaxRound, playerInfoList[0].soldierNumInit, playerInfoList[1].soldierNumInit);

        }
    }

    public void ReplayBattle(int replayBattleId)
    {
        if (IsBattleRunning)
        {
            GameLog.Warn($"ReplayBattle called while battle is running, skipping. replayBattleId={replayBattleId}");
            return;
        }
        IsBattleRunning = true;
        
        LoadFromFile("battlereplayer" + replayBattleId + ".json");
        SkillManager.isReplay = true;

        chessList.Clear();
        missileList.Clear();
        gridOccupancy.Clear();
        GameManager.Instance.SaveData.battleStatManager.LoadBattleForReplay(battleId);

        gameFinish = false;
        var player1 = GameManager.Instance.GetForce(playerInfoList[0].forceId);
        var player2 = GameManager.Instance.GetForce(playerInfoList[1].forceId);
        
        InitUI(player1, player2);

        currentBattleCoroutine = StartCoroutine(GameUpdate(null, null, true));
    }

    private void StartDeployPhase()
    {
        isDeployPhase = true;

        // 判断玩家是哪一方
        var force0 = GameManager.Instance.GetForce(playerInfoList[0].forceId);
        var force1 = GameManager.Instance.GetForce(playerInfoList[1].forceId);
        playerSideIndex = force0.isPlayer ? 0 : 1;

        var playerForce = playerSideIndex == 0 ? force0 : force1;
        var playerTroops = playerSideIndex == 0 ? attackTroops : defenderTroops;

        int count = Math.Min(playerTroops.Count, SystemConst.Battle.MAX_BATTLE_HEROES_PER_SIDE);
        for (int i = 0; i < count; i++)
        {
            var troop = playerTroops[i];
            var spawnPos = GetSpawnPosition(playerSideIndex + 1, i);
            CreateDeployChess(playerForce, troop, spawnPos);
        }

        FillEmptyGridsWithSodNull();

        foreach (var chess in chessList)
        {
            if (chess.viewObj != null)
            {
                var collider = chess.viewObj.gameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3(12, 10, 12);
            }
        }

        battleUIManager.ShowDeployConfirmButton();
        GameLog.Info($"布阵阶段开始 玩家方=side{playerSideIndex + 1}");
    }

    private void CreateDeployChess(SaveForceData force, SaveTroopsData troop, Vector3 spawnPos)
    {
        if (troop.heroId1 <= 0)
            return;

        var heroData1 = GameManager.Instance.GetHero(troop.heroId1);
        var heroData2 = troop.heroId2 > 0 ? GameManager.Instance.GetHero(troop.heroId2) : null;
        var heroData3 = troop.heroId3 > 0 ? GameManager.Instance.GetHero(troop.heroId3) : null;

        int inte = Math.Max(Math.Max(heroData1.inte, heroData2?.inte ?? 0), heroData3?.inte ?? 0);
        var (atk, def) = SysFormula.Battle.CalculateCombatAttrForTroop(troop);

        var id = idCounter++;
        var chessObj = new Chess(id);
        chessObj.forceId = force.forceId;
        chessObj.position = spawnPos;
        chessObj.isHero = true;
        chessObj.heroId = troop.heroId1;
        chessObj.heroId2 = troop.heroId2;
        chessObj.heroId3 = troop.heroId3;
        chessObj.level = heroData1.GetLevel();
        chessObj.armsId = troop.armsId;
        chessObj.atk = atk;
        chessObj.def = def;
        chessObj.inte = inte;
        chessObj.maxHp = SystemConst.Hero.MAX_SOLDIER_PER_HERO;
        chessObj.hp = chessObj.maxHp;

        chessList.Add(chessObj);
        chessObj.Init(force.forceId);
    }

    private void FillEmptyGridsWithSodNull()
    {
        int baseGx = playerSideIndex == 0
            ? SystemConst.Battle.DEPLOY_SIDE1_BASE_GX
            : SystemConst.Battle.DEPLOY_SIDE2_BASE_GX;
        int baseGz = playerSideIndex == 0
            ? SystemConst.Battle.DEPLOY_SIDE1_BASE_GZ
            : SystemConst.Battle.DEPLOY_SIDE2_BASE_GZ;

        for (int row = 0; row < SystemConst.Battle.DEPLOY_GRID_ROWS; row++)
        {
            for (int col = 0; col < SystemConst.Battle.DEPLOY_GRID_COLS; col++)
            {
                int gx = baseGx + row;
                int gz = baseGz + col;
                bool occupied = chessList.Any(c =>
                {
                    var (cx, cz) = WorldToGridCoord(c.position);
                    return cx == gx && cz == gz;
                });
                if (!occupied)
                    CreateSodNullChess(playerInfoList[playerSideIndex].forceId, GridCoordToWorld(gx, gz));
            }
        }
    }

    private void CreateSodNullChess(int forceId, Vector3 position)
    {
        var id = idCounter++;
        var chessObj = new Chess(id);
        chessObj.forceId = forceId;
        chessObj.position = position;
        chessObj.isSodNull = true;
        chessObj.isHero = false;
        chessObj.armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
        chessObj.maxHp = 1;
        chessObj.hp = 1;
        chessObj.atk = 0;
        chessObj.def = 0;
        chessObj.inte = 0;

        chessList.Add(chessObj);
        chessObj.Init(forceId);
    }

    public void OnDeployConfirm()
    {
        // 记录玩家布阵位置（按 heroId1 映射）
        playerDeployPositions.Clear();
        foreach (var chess in chessList)
        {
            if (!chess.isSodNull && chess.isHero)
                playerDeployPositions[chess.heroId] = chess.position;
        }

        // 清除所有布阵阶段棋子
        foreach (var chess in chessList.ToList())
        {
            if (chess.viewObj != null)
            {
                chess.viewObj.DestroyHUD();
                Destroy(chess.viewObj.gameObject);
            }
        }
        chessList.Clear();

        isDeployPhase = false;
        draggingChess = null;
        battleUIManager.HideDeployConfirmButton();
        GameLog.Info("布阵阶段结束，开始战斗");

        currentBattleCoroutine = StartCoroutine(GameUpdate(deployAttackSoldierMap, deployDefenderSoldierMap));
    }

    public void SwapChessPosition(Chess chess1, Chess chess2)
    {
        var pos1 = chess1.position;
        var pos2 = chess2.position;

        chess1.SetPosition(pos2);
        chess2.SetPosition(pos1);
    }

    private bool IsInPlayerDeployArea(int gx, int gz)
    {
        int baseGx = playerSideIndex == 0
            ? SystemConst.Battle.DEPLOY_SIDE1_BASE_GX
            : SystemConst.Battle.DEPLOY_SIDE2_BASE_GX;
        int baseGz = playerSideIndex == 0
            ? SystemConst.Battle.DEPLOY_SIDE1_BASE_GZ
            : SystemConst.Battle.DEPLOY_SIDE2_BASE_GZ;

        return gx >= baseGx &&
               gx < baseGx + SystemConst.Battle.DEPLOY_GRID_ROWS &&
               gz >= baseGz &&
               gz < baseGz + SystemConst.Battle.DEPLOY_GRID_COLS;
    }

    private void Update()
    {
        if (!isDeployPhase) return;

        if (Input.GetMouseButtonDown(0) && draggingChess == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var chessViewObj = hit.collider.GetComponentInParent<ChessViewObj>();
                if (chessViewObj != null && !chessViewObj.chessUnit.isSodNull)
                {
                    draggingChess = chessViewObj.chessUnit;
                    dragStartPos = draggingChess.position;
                }
            }
        }

        if (draggingChess != null)
        {
            Plane groundPlane = new Plane(Vector3.up, new Vector3(0, 7, 0));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 worldPos = ray.GetPoint(distance);
                if (draggingChess.viewObj != null)
                    draggingChess.viewObj.transform.position = new Vector3(worldPos.x, 12, worldPos.z);
            }

            if (Input.GetMouseButtonUp(0))
            {
                Plane gp = new Plane(Vector3.up, new Vector3(0, 7, 0));
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool swapped = false;
                if (gp.Raycast(r, out float d))
                {
                    Vector3 worldPos = r.GetPoint(d);
                    var (gx, gz) = WorldToGridCoord(worldPos);

                    if (IsInPlayerDeployArea(gx, gz))
                    {
                        // 在 chessList 中查找目标位置的棋子
                        Chess targetChess = null;
                        foreach (var c in chessList)
                        {
                            if (c == draggingChess) continue;
                            var (cx, cz) = WorldToGridCoord(c.position);
                            if (cx == gx && cz == gz)
                            {
                                targetChess = c;
                                break;
                            }
                        }

                        if (targetChess != null)
                        {
                            SwapChessPosition(draggingChess, targetChess);
                            swapped = true;
                        }
                    }
                }

                if (!swapped)
                    draggingChess.SetPosition(dragStartPos);

                draggingChess = null;
            }
        }
    }

    private Vector3 GetSpawnPosition(int side, int indx)
    {
        if(side == 1)
            return new Vector3(330 - (indx / 4) * 15, 7, 240 - (indx % 4) * 15);
        else
            return new Vector3(435 + (indx / 4) * 15, 7, 240 - (indx % 4) * 15);
    }

    public int SpawnUnitsForRegion(SaveForceData force, int battleUnitId, UnityEngine.Vector3 spawnPos, float summonTime, Action<int> cb = null)
    {
        var id = idCounter++;

        var battleUnitConfig = BattleUnitConfig.GetConfig(battleUnitId);
        var armsId = battleUnitConfig.ArmsId;
        var atk = battleUnitConfig.Atk;
        var def = battleUnitConfig.Def;
        var soldierNum = battleUnitConfig.Hp;
        
        var action = new CreateChessAction(0, tickIndex, id, force.forceId, battleUnitId, soldierNum, armsId, atk, def, spawnPos, summonTime, cb);
        AddChessAction(action);

        return id;
    }

    private void SpawnTroopForRegion(SaveForceData force, int tickAdd, UnityEngine.Vector3 spawnPoint, SaveTroopsData troop, int soldierCount)
    {
        if (troop.heroId1 <= 0)
            return;

        var heroData1 = GameManager.Instance.GetHero(troop.heroId1);
        var heroData2 = troop.heroId2 > 0 ? GameManager.Instance.GetHero(troop.heroId2) : null;
        var heroData3 = troop.heroId3 > 0 ? GameManager.Instance.GetHero(troop.heroId3) : null;

        int inte = Math.Max(Math.Max(heroData1.inte, heroData2?.inte ?? 0), heroData3?.inte ?? 0);

        var (atk, def) = SysFormula.Battle.CalculateCombatAttrForTroop(troop);
        
        var id = idCounter++;
        var action = new CreateChessAction(0, tickAdd, id, force.forceId, 
            troop.heroId1, troop.heroId2, troop.heroId3, 
            heroData1.GetLevel(), 
            soldierCount, troop.armsId, atk, def, inte, spawnPoint);
        AddChessAction(action);
    }

    public static float tickTimeReal = 0.1f; //加速功能
    
    private IEnumerator GameUpdate(Dictionary<int, int> attackSoldierMap, Dictionary<int, int> defenderSoldierMap, bool replay = false)
    {
        yield return new WaitForSeconds(0.5f);

        GameLog.Debug($"GameUpdatett start battleId={battleId} realTime={Time.time} cityId={cityId}");
        var speed = 1;
        if (quickMode && showUI)
            speed = 10;
        else if(quickMode)
            speed = 400;
        tickIndex = 1;

        var waitTick = GetTickFromTime(SystemConst.Battle.WAIT_TIME);
        var battleBeginTick = GetTickFromTime(SystemConst.Battle.BATTLE_BEGIN_TIME);
        var foodDeductionTick = GetTickFromTime(SystemConst.Battle.FOOD_DEDUCTION_INTERVAL);

        var player1 = GameManager.Instance.GetForce(playerInfoList[0].forceId);
        var magicHelperUnitId = SpawnUnitsForRegion(player1, SystemConst.Battle.MAGIC_HELPER_UNIT_ID, new Vector3(1, 7, 1), 10);

        while (!gameFinish)
        {
            for (int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(tickTimeReal / 4); //高频帧，给missile这种表现用

                if(showUI)
                {
                    for (int j = 0; j < missileList.Count; j++)                                                             
                    {
                        var missile = missileList[j];
                        if (missile != null)
                            missile.RenderUpdate(tickIndex, (float)i / 4, 1f/40);
                    }
                    foreach (var chess in chessList.ToArray())
                    {
                        if (chess != null)
                            chess.RenderUpdate(tickIndex, (float)i / 4, 1f/40);
                    }
                }
            }

            //  var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < speed; i++)
            {

                if(!replay)
                {
                    if(waitTick > 0 && tickIndex >= waitTick)
                    {
                        InitSummon(magicHelperUnitId, attackSoldierMap, defenderSoldierMap);
                        waitTick = 0;
                    }

                    if(battleBeginTick > 0)
                    {
                        if(tickIndex >= battleBeginTick)
                        {
                            foreach (var chess in chessList.ToArray()) //防止召唤
                                SkillManager.CheckAddSkill(chess);  
                            foreach (var chess in chessList.ToArray()) //防止召唤
                                SkillManager.BattleBegin(chess);
                            lastFoodDeductionTick = tickIndex;                                
                            battleBeginTick = 0;
                        }
                    }

                    if(battleBeginTick == 0)
                    {
                        foreach (var chess in chessList.ToArray())
                        {
                            if (chess != null)
                                chess.LogicUpdate(tickIndex);
                        }
                        foreach (var missile in missileList.ToArray())
                        {
                            if (missile != null)
                                missile.LogicUpdate(tickIndex);
                        }
                        // 每个回合结束，玩家消耗食物
                        if (tickIndex - lastFoodDeductionTick >= foodDeductionTick)
                        {
                            lastFoodDeductionTick = tickIndex;                            
                            var roundAction = new RoundUpdateAction(0, tickIndex, round + 1);
                            AddChessAction(roundAction);
                            if (round >= MaxRound)
                            {
                                gameFinish = true;
                                battleResult = BattleResult.Draw;
                                GameLog.Info($"战斗达到{MaxRound}回合，强制结束，平局");
                            }
                        }
                    }

                }
                isDoingAction = true;
                actions.FindAll(x => x.Tick == tickIndex).ForEach(x => x.Doing());
                isDoingAction = false;
                coroutineManager.Update(tickTimeReal);
                tickIndex++;                
            }

            if(showUI)
            {
                var leftSoldierTotal = chessList.Sum(x => x.forceId == playerInfoList[0].forceId && x.isHero ? Math.Max(0, x.hp) : 0);
                var rightSoldierTotal = chessList.Sum(x => x.forceId == playerInfoList[1].forceId && x.isHero ? Math.Max(0, x.hp) : 0);
                BattleInfoTop.Instance.UpdateSoldierCount(leftSoldierTotal, rightSoldierTotal);
            }
            //    sw.Stop();
            //    UnityEngine.Debug.Log($"GameUpdate 循环耗时: {sw.ElapsedMilliseconds} ms");
        }
        GameLog.Info($"GameUpdatett end battleId={battleId} realTime={Time.time} cityId={cityId}");


        if(showUI)
            battleUIManager.OnBattleEnd(battleResult, replay);

        // 调用战斗结束回调
        if (battleEndCallback != null)
        {
            var attackerResult = new Dictionary<int, int>();
            var defenderResult = new Dictionary<int, int>();
            int attackerForceId = playerInfoList[0].forceId;
            int defenderForceId = playerInfoList[1].forceId;
            for (int i = 0; i < chessList.Count; i++)
            {
                var chess = chessList[i];
                if (chess.isHero)
                {
                    if (chess.forceId == attackerForceId)
                        attackerResult.Add(chess.heroId, chess.hp);
                    else if (chess.forceId == defenderForceId)
                        defenderResult.Add(chess.heroId, chess.hp);
                }
            }

            battleEndCallback(battleResult, attackerResult, defenderResult);
        }

        if(!replay)
        {
            var soldierLoss1 = playerInfoList[0].soldierNumInit - chessList.Where(x => x.forceId == playerInfoList[0].forceId && x.isHero).Sum(x => Math.Max(0, x.hp));
            var soldierLoss2 = playerInfoList[1].soldierNumInit - chessList.Where(x => x.forceId == playerInfoList[1].forceId && x.isHero).Sum(x => Math.Max(0, x.hp));
            
            GameManager.Instance.SaveData.battleStatManager.SaveCurrentBattle(
                cityId,
                playerInfoList[0].forceId, playerInfoList[1].forceId,
                battleResult,
                round,
                soldierLoss1, soldierLoss2);
            
            LogBattleResult();
            SaveToFile("battlereplayer" + battleId + ".json");
        }

        ResourceCache.ClearBattleCache();
        IsBattleRunning = false;
        currentBattleCoroutine = null;
    }

    private void InitSummon(int magicHelperUnitId, Dictionary<int, int> attackSoldierMap, Dictionary<int, int> defenderSoldierMap)
    {
        var player1 = GameManager.Instance.GetForce(playerInfoList[0].forceId);
        var player2 = GameManager.Instance.GetForce(playerInfoList[1].forceId);

        int count = Math.Min(defenderTroops.Count, SystemConst.Battle.MAX_BATTLE_HEROES_PER_SIDE);
        for (int i = 0; i < count; i++)
        {
            // 防守方是玩家时优先使用布阵位置
            var spawnPos = playerSideIndex == 1 && playerDeployPositions.ContainsKey(defenderTroops[i].heroId1)
                ? playerDeployPositions[defenderTroops[i].heroId1]
                : GetSpawnPosition(2, i);
            var tick = tickIndex + (count > SystemConst.Battle.SUMMON_BATCH_THRESHOLD ? (i/2) : i);
            var eff = new CreateEffectAction(magicHelperUnitId, tick, spawnPos, "SoftFireBigRed", 0.7f);
            AddChessAction(eff);
            SpawnTroopForRegion(player2, tick + SystemConst.Battle.SUMMON_HERO_DELAY_TICKS, spawnPos, defenderTroops[i], defenderSoldierMap.ContainsKey(defenderTroops[i].heroId1) ? defenderSoldierMap[defenderTroops[i].heroId1] : 0);
        }

        count = Math.Min(attackTroops.Count, SystemConst.Battle.MAX_BATTLE_HEROES_PER_SIDE);
        for (int i = 0; i < count; i++)
        {
            // 攻击方是玩家时优先使用布阵位置
            var spawnPos = playerSideIndex == 0 && playerDeployPositions.ContainsKey(attackTroops[i].heroId1)
                ? playerDeployPositions[attackTroops[i].heroId1]
                : GetSpawnPosition(1, i);
            var tick = tickIndex + SystemConst.Battle.ATTACKER_SPAWN_DELAY_TICKS + (count > SystemConst.Battle.SUMMON_BATCH_THRESHOLD ? (i/2) : i);
            var eff = new CreateEffectAction(magicHelperUnitId, tick, spawnPos, "LightningExplosionBlue", 0.7f);
            AddChessAction(eff);
            SpawnTroopForRegion(player1, tick + SystemConst.Battle.SUMMON_HERO_DELAY_TICKS, spawnPos, attackTroops[i], attackSoldierMap.ContainsKey(attackTroops[i].heroId1) ? attackSoldierMap[attackTroops[i].heroId1] : 0);
        }

        GameLog.Info($"InitSummon {player1.Name} {attackTroops.Count} {player2.Name} {defenderTroops.Count}");
    }

    public int GetTickFromTime(float time)
    {
        return (int)(time / tickTimeReal);
    } 

    public void CreateAttackMissile(Chess sourceChess, Chess targetChess, int attackDamage = 0, bool attackIsCrit = false, bool attackIsDodge = false, string hitEffect = "", string attackDamType = "str")
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetChess.id, sourceChess.position, 0, 0, attackDamage, attackIsCrit, attackIsDodge, attackDamType);
        AddChessAction(action);
    }

    public void CreateSpellMissile(Chess sourceChess, Chess targetChess, Vector3 startPos, int skillId, int damage)
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetChess.id, startPos, skillId, damage);
        AddChessAction(action);
    }    

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, int skillId, int damage)
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetPos, sourceChess.position, skillId, damage, time);
        AddChessAction(action);
    }
    
    public void RemoveMissile(Missile missile)
    {
        var action = new RemoveMissileAction(missile.ownerId, tickIndex, missile.id);
        AddChessAction(action);
    }

    public Chess GetChess(int id)
    {
        return chessList.Find(x => x.id == id);
    }

    public Missile GetMissile(int id)
    {
        return missileList.Find(x => x.id == id);
    }

    public void OccupyGrid(int chessId, Vector3 worldPos)
    {
        var (gx, gz) = WorldToGridCoord(worldPos);
        gridOccupancy.Add((gx, gz, chessId));
    }

    public void ReleaseGrid(int chessId)
    {
        gridOccupancy.RemoveAll(g => g.chessId == chessId);
    }

    public void UpdateGrid(int chessId, Vector3 newWorldPos)
    {
        ReleaseGrid(chessId);
        OccupyGrid(chessId, newWorldPos);
    }

    public (int gx, int gz) WorldToGridCoord(Vector3 worldPos)
    {
        int gx = Mathf.RoundToInt(worldPos.x / SystemConst.Battle.GRID_CELL_SIZE);
        int gz = Mathf.RoundToInt(worldPos.z / SystemConst.Battle.GRID_CELL_SIZE);
        return (gx, gz);
    }

    public Vector3 GridCoordToWorld(int gx, int gz, float y = 7f)
    {
        return new Vector3(gx * SystemConst.Battle.GRID_CELL_SIZE, y, gz * SystemConst.Battle.GRID_CELL_SIZE);
    }

    public bool IsGridOccupied(int gx, int gz)
    {
        return gridOccupancy.Exists(g => g.gridX == gx && g.gridZ == gz);
    }

    public bool IsGridOccupiedByOther(int gx, int gz, int excludeChessId)
    {
        return gridOccupancy.Exists(g => g.gridX == gx && g.gridZ == gz && g.chessId != excludeChessId);
    }

    // 世界坐标转格子坐标
    public static Vector2Int WorldToGridPosition(Vector3 worldPosition, bool FloorToInt)
    {
        int x = 0;
        int z = 0;
        if (FloorToInt)
        {
            x = Mathf.FloorToInt(worldPosition.x / SystemConst.Battle.GRID_CELL_SIZE) * SystemConst.Battle.GRID_CELL_SIZE;
                z = Mathf.FloorToInt(worldPosition.z / SystemConst.Battle.GRID_CELL_SIZE) * SystemConst.Battle.GRID_CELL_SIZE;
        }
        else
        {
            x = Mathf.CeilToInt(worldPosition.x / SystemConst.Battle.GRID_CELL_SIZE) * SystemConst.Battle.GRID_CELL_SIZE;
            z = Mathf.CeilToInt(worldPosition.z / SystemConst.Battle.GRID_CELL_SIZE) * SystemConst.Battle.GRID_CELL_SIZE;
        }
        return new Vector2Int(x, z);
    }

    public bool IsPositionFree(Chess unit, Vector3 targetPosition)
    {
        var (gx, gz) = WorldToGridCoord(targetPosition);
        return !IsGridOccupiedByOther(gx, gz, unit.id);
    }

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        if (isForce)
        {
            UpdateGrid(unit.id, targetPosition);
            unit.SetPosition(targetPosition);
            return true;
        }
        else
        { 
            if(!IsPositionFree(unit, targetPosition))
                return false;
            
            UpdateGrid(unit.id, targetPosition);
            unit.SetPosition(targetPosition);
            return true;
        }
    }

    public void OnUnitDying(Chess dieUnit)
    {
        if (dieUnit.isHero)
        {
            BattleStatManager.SetHeroDead(dieUnit.forceId, dieUnit.heroId);
        }

        chessList.Remove(dieUnit);

        gameFinish = false;
        battleResult = BattleResult.Lose;
        bool[] sideHasUnits = new bool[playerInfoList.Count];
        int aliveSideCount = 0;

        var unit = GameManager.Instance.GetHero(dieUnit.heroId);
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                int sideIndex = -1;
                for (int i = 0; i < playerInfoList.Count; i++)
                {
                    if (playerInfoList[i].forceId == chessComponent.forceId)
                    {
                        sideIndex = i;
                        break;
                    }
                }
                if (sideIndex >= 0 && sideIndex < sideHasUnits.Length)
                {
                    if (!sideHasUnits[sideIndex])
                    {
                        sideHasUnits[sideIndex] = true;
                        aliveSideCount++;
                    }
                }
            }
        }

        GameLog.Info($"id:{dieUnit.id} dieUnit.forceId:{dieUnit.forceId} 存活阵营数:{aliveSideCount}");
        if (aliveSideCount <= 1)
        {
            gameFinish = true;
            battleResult = sideHasUnits[0] ? BattleResult.Win : BattleResult.Lose;
        }
    }

    public static bool CheckInRange(Vector3 pos1, Vector3 pos2, float range)
    {
        Vector2Int pos1a = WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldToGridPosition(pos2, true);

        return Vector2Int.Distance(pos1a, pos2a) <= range;
    }

    public static float GetRange(Vector3 pos1, Vector3 pos2)
    {
        Vector2Int pos1a = WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldToGridPosition(pos2, true);

        return Vector2Int.Distance(pos1a, pos2a);
    }


    public List<Chess> GetUnitsInRange(Vector3 wPos, float range, int myForceId, bool findEnemy)
    {
        Vector2Int center = WorldToGridPosition(wPos, true);
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                Vector2Int chessPos = WorldToGridPosition(chessComponent.position, true);
                if (Vector2Int.Distance(center, chessPos) <= range || range == 0)
                {
                    if(findEnemy)
                    {
                        if(chessComponent.forceId != myForceId)
                            unitsInRange.Add(chessComponent);
                    }
                    else
                    {
                        if(chessComponent.forceId == myForceId) 
                            unitsInRange.Add(chessComponent);
                    }
                }
            }
        }

        return unitsInRange;
    }

    public static void RandomSelect(List<Chess> unitsInRange, int limit)
    {
        if (unitsInRange.Count <= limit)
            return;

        GameLog.Info($"RandomSelect limit:{limit} unitsInRange.Count:{unitsInRange.Count}");
        
        while (unitsInRange.Count > limit)
        {
            int indexToRemove = BattleRandom.Range(0, unitsInRange.Count);
            unitsInRange.RemoveAt(indexToRemove);
        }
    }

    public List<Chess> GetUnitsMyForce(Vector3 wPos, float range, int myForceId)
    {
        Vector2Int center = WorldToGridPosition(wPos, true);
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                Vector2Int chessPos = WorldToGridPosition(chessComponent.position, true);
                if (range == 0 || Vector2Int.Distance(center, chessPos) <= range)
                {
                    if(chessComponent.forceId == myForceId)
                        unitsInRange.Add(chessComponent);
                }
            }
        }
        return unitsInRange;
    } 

    public List<Chess> GetUnitsByForceId(int forceId)
    {
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if (chessComponent.forceId == forceId)
                    unitsInRange.Add(chessComponent);
            }
        }
        return unitsInRange;
    }

    public void AddBattleText(string text, UnityEngine.Vector3 worldPos, UnityEngine.Vector2 speed, Color color, int duration)
    {
        if(showUI&&!quickMode)
            battleUIManager.AddBattleText(text, worldPos, speed, color, duration);
    }

    public Vector2 TransformWorldToScreen(Vector3 worldPosition, RectTransform canvas)
    {
        return battleUIManager.TransformWorldToScreen(worldPosition, canvas);
    }

    public void AddChessAction(ChessAction action)
    {
        if(isDoingAction && action.Tick == tickIndex)
            action.Tick ++; //顺延到下一帧
        
        actions.Add(action);
    }

    // 序列化到文件
    public void SaveToFile(string filePath)
    {
        filePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, filePath + ".json");
        string json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(filePath, json);
    }

    // 从文件反序列化
    public void LoadFromFile(string filePath)
    {
        filePath = System.IO.Path.Combine(UnityEngine.Application.persistentDataPath, filePath + ".json");  
        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);

            foreach (var chessComponent in chessList)
            {
                chessComponent.OnRecover();
                OccupyGrid(chessComponent.id, chessComponent.position);
            }
            foreach (var missileComponent in missileList)
            {
                missileComponent.OnRecover();
            }
        }
    }

    private void LogBattleResult()
    {
        var attackerForceId = playerInfoList[0].forceId;
        var defenderForceId = playerInfoList[1].forceId;
        
        var attackerHeroNames = string.Join(",", attackTroops.Select(c => ConfigNameHelper.GetHeroName(c.heroId1)));
        var defenderHeroNames = string.Join(",", defenderTroops.Select(c => ConfigNameHelper.GetHeroName(c.heroId1)));
        
        string resultStr = battleResult switch
        {
            BattleResult.Win => "攻击方胜",
            BattleResult.Lose => "防守方胜",
            BattleResult.Draw => "平局",
            _ => "未知"
        };
        
        var attackerRemaining = chessList.Where(x => x.forceId == attackerForceId && x.isHero).Sum(x => Math.Max(0, x.hp));
        var defenderRemaining = chessList.Where(x => x.forceId == defenderForceId && x.isHero).Sum(x => Math.Max(0, x.hp));
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(attackerForceId)} vs {ConfigNameHelper.GetForceName(defenderForceId)} [{ConfigNameHelper.GetCityName(cityId)}] " +
            $"攻击方:[{attackerHeroNames}] 防守方:[{defenderHeroNames}] " +
            $"结果:{resultStr} 回合:{round} " +
            $"剩余兵力 攻:{attackerRemaining} 守:{defenderRemaining}");
    }

}
