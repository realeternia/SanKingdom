using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;
using Controls.Utils;

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
    public class FoodInfo
    {
        public int forceId;
        public int food;
        public int maxFood;

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


    public List<FoodInfo> playerInfoList = new List<FoodInfo>();

    public List<Chess> chessList = new List<Chess>(); // 所有棋子
    public List<Missile> missileList = new List<Missile>(); // 所有导弹

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
    private List<BattleCardData> cards1;
    [NonSerialized]
    private List<BattleCardData> cards2;

    [NonSerialized]
    private bool isDoingAction = false;


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
    
    public void BattleBegin(SaveForceData force1, SaveForceData force2, List<BattleCardData> cards1, List<BattleCardData> cards2, int food1, int food2, int cityId, Action<BattleResult, Dictionary<int, int>, Dictionary<int, int>> callback = null)
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
        playerInfoList.Add(new FoodInfo() { forceId = force1.forceId, food = food1, maxFood = food1, soldierNumInit = cards1.Sum(x => x.SoldierNum) });
        playerInfoList.Add(new FoodInfo() { forceId = force2.forceId, food = food2, maxFood = food2, soldierNumInit = cards2.Sum(x => x.SoldierNum) });

        chessList.Clear();
        missileList.Clear();
        actions.Clear();
        idCounter = 100;
        lastFoodDeductionTick = 0;
        battleId = GameManager.Instance.SaveData.battleStatManager.OnNewBattle();
        SkillManager.isReplay = false;

        gameFinish = false;
        round = 0;

        InitUI(force1, force2);
        //对cards1和card2都按HeroConfig的Range排序，确保远程在后面
        cards1.Sort((a, b) => ArmsConfig.GetConfig(a.ArmsId).Range.CompareTo(ArmsConfig.GetConfig(b.ArmsId).Range));
        cards2.Sort((a, b) => ArmsConfig.GetConfig(a.ArmsId).Range.CompareTo(ArmsConfig.GetConfig(b.ArmsId).Range));

        this.cards1 = cards1;
        this.cards2 = cards2;    

        currentBattleCoroutine = StartCoroutine(GameUpdate());
    }

    private void InitUI(SaveForceData force1, SaveForceData force2)
    {
        if (showUI)
        {
            var startTime = Time.realtimeSinceStartup;
            var newMapId = 1;
            var mapNode = Resources.Load<GameObject>("Prefabs/BattleMaps/Map" + newMapId);
            if (mapObj != null)
                UnityEngine.Object.Destroy(mapObj);

            mapObj = UnityEngine.Object.Instantiate(mapNode, battleUIManager.NodeUnits.transform.parent);
            var endTime = Time.realtimeSinceStartup;
            GameLog.Info("加载地图耗时：" + (endTime - startTime) + "秒");

            battleUIManager.ShowBattleBegin(force1, force2, MaxRound, playerInfoList[0].soldierNumInit, playerInfoList[1].soldierNumInit);
            battleUIManager.CreateCastleHUD(force1, GetSpawnPosition(1, 5));
            battleUIManager.CreateCastleHUD(force2, GetSpawnPosition(2, 5));
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

        foreach (var playerInfo in playerInfoList)
        {
            playerInfo.food = playerInfo.maxFood;
        }

        chessList.Clear();
        missileList.Clear();
        GameManager.Instance.SaveData.battleStatManager.LoadBattleForReplay(battleId);

        gameFinish = false;
        var player1 = GameManager.Instance.GetForce(playerInfoList[0].forceId);
        var player2 = GameManager.Instance.GetForce(playerInfoList[1].forceId);
        
        InitUI(player1, player2);

        currentBattleCoroutine = StartCoroutine(GameUpdate(true));
    }

    private Vector3 GetSpawnPosition(int side, int indx)
    {
        if(side == 1)
            return new Vector3(330 - (indx / 4) * 15, 7, 245 - (indx % 4) * 20);
        else
            return new Vector3(430 + (indx / 4) * 15, 7, 245 - (indx % 4) * 20);
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

    private void SpawnHerosForRegion(SaveForceData force, int tickAdd, UnityEngine.Vector3 spawnPoint, BattleCardData heroCardData)
    {
        var heroData = GameManager.Instance.GetHero(heroCardData.CardId);
        
        var id = idCounter++;
        var (atk, def) = SysFormula.Battle.CalculateCombatAttr(heroData, heroData.armsId);
        var action = new CreateChessAction(0, tickAdd, id, force.forceId, heroCardData.CardId, 1, heroCardData.SoldierNum, heroData.armsId, atk, def, heroData.str, heroData.leadShip, heroData.inte, spawnPoint);
        AddChessAction(action);
    }

    public static float tickTimeReal = 0.1f; //加速功能
    
    private IEnumerator GameUpdate(bool replay = false)
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
                        InitSummon(magicHelperUnitId);
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
                            foreach (var foodInfo in playerInfoList)
                            {
                                var soldierNum = GetUnitsByForceId(foodInfo.forceId).Where(x => x.isHero).Sum(x => x.hp);
                                var costAmount = SysFormula.Battle.CalculateFoodCost(soldierNum);

                                var action = new FoodCostAction(0, tickIndex, foodInfo.forceId, costAmount);
                                AddChessAction(action);

                                if(foodInfo.food < costAmount)
                                {
                                    var units = GetUnitsByForceId(foodInfo.forceId);
                                    foreach (var unit in units)
                                        unit.LackFood((float)(costAmount - foodInfo.food) / costAmount);
                                }

                            }
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
            var result = new Dictionary<int, int>();
            for (int i = 0; i < chessList.Count; i++)
            {
                var chess = chessList[i];
                if (chess.isHero)
                    result.Add(chess.heroId, chess.hp);
            }
            var result2 = new Dictionary<int, int>();
            result2[playerInfoList[0].forceId] = playerInfoList[0].food;
            result2[playerInfoList[1].forceId] = playerInfoList[1].food;
            battleEndCallback(battleResult, result, result2);
        }

        if(!replay)
        {
            var soldierLoss1 = playerInfoList[0].soldierNumInit - chessList.Where(x => x.forceId == playerInfoList[0].forceId && x.isHero).Sum(x => Math.Max(0, x.hp));
            var soldierLoss2 = playerInfoList[1].soldierNumInit - chessList.Where(x => x.forceId == playerInfoList[1].forceId && x.isHero).Sum(x => Math.Max(0, x.hp));
            var foodCost1 = playerInfoList[0].maxFood - playerInfoList[0].food;
            var foodCost2 = playerInfoList[1].maxFood - playerInfoList[1].food;
            
            GameManager.Instance.SaveData.battleStatManager.SaveCurrentBattle(
                cityId,
                playerInfoList[0].forceId, playerInfoList[1].forceId,
                battleResult,
                round,
                soldierLoss1, soldierLoss2,
                foodCost1, foodCost2);
            
            LogBattleResult();
            SaveToFile("battlereplayer" + battleId + ".json");
        }
        
        IsBattleRunning = false;
        currentBattleCoroutine = null;
    }

    private void InitSummon(int magicHelperUnitId)
    {
        var player1 = GameManager.Instance.GetForce(playerInfoList[0].forceId);
        var player2 = GameManager.Instance.GetForce(playerInfoList[1].forceId);

        int count = Math.Min(cards2.Count, SystemConst.Battle.MAX_BATTLE_HEROES_PER_SIDE);
        for (int i = 0; i < count; i++)
        {
            var tick = tickIndex + (count > SystemConst.Battle.SUMMON_BATCH_THRESHOLD ? (i/2) : i);
            var eff = new CreateEffectAction(magicHelperUnitId, tick, GetSpawnPosition(2, i), "SoftFireBigRed", 0.7f);
            AddChessAction(eff);
            SpawnHerosForRegion(player2, tick + SystemConst.Battle.SUMMON_HERO_DELAY_TICKS, GetSpawnPosition(2, i), cards2[i]);
        }

        count = Math.Min(cards1.Count, SystemConst.Battle.MAX_BATTLE_HEROES_PER_SIDE);
        for (int i = 0; i < count; i++) 
        {
            var tick = tickIndex + SystemConst.Battle.ATTACKER_SPAWN_DELAY_TICKS + (count > SystemConst.Battle.SUMMON_BATCH_THRESHOLD ? (i/2) : i);
            var eff = new CreateEffectAction(magicHelperUnitId, tick, GetSpawnPosition(1, i), "LightningExplosionBlue", 0.7f);
            AddChessAction(eff);
            SpawnHerosForRegion(player1, tick + SystemConst.Battle.SUMMON_HERO_DELAY_TICKS, GetSpawnPosition(1, i), cards1[i]);
        }

        GameLog.Info($"InitSummon {player1.Name} {cards1.Count} {player2.Name} {cards2.Count}");
    }

    public int GetTickFromTime(float time)
    {
        return (int)(time / tickTimeReal);
    } 

    public void CreateAttackMissile(Chess sourceChess, Chess targetChess)
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetChess.id, sourceChess.position, 0, 0);
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
        var ckSize = SystemConst.Battle.CHESS_COLLISION_SIZE;
        var findInRange = false;
        foreach(var ckUnit in chessList)
        {
            if(ckUnit == unit)
                continue;
            
            if(Math.Abs(ckUnit.position.x - targetPosition.x) + Math.Abs(ckUnit.position.z - targetPosition.z) < ckSize)
            {
                findInRange = true;
                break;
            }
        }

        if(findInRange)
            return false;

        return true;
    }

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        if (isForce)
        {
            unit.SetPosition(targetPosition);

            return true;
        }
        else
        { 
            if(!IsPositionFree(unit, targetPosition))
                return false;
            
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

    public FoodInfo GetFoodInfo(int forceId)
    {
        return playerInfoList.Find(foodInfo => foodInfo.forceId == forceId);
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
            }
            foreach (var missileComponent in missileList)
            {
                missileComponent.OnRecover();
            }
        }
    }

    private void LogBattleResult()
    {
        if (cards1 == null || cards2 == null) return;
        
        var attackerForceId = playerInfoList[0].forceId;
        var defenderForceId = playerInfoList[1].forceId;
        
        var attackerHeroNames = string.Join(",", cards1.Select(c => ConfigNameHelper.GetHeroName(c.CardId)));
        var defenderHeroNames = string.Join(",", cards2.Select(c => ConfigNameHelper.GetHeroName(c.CardId)));
        
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
