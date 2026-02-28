using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

[Serializable]
public class BattleManager : MonoBehaviour
{
    [Serializable]
    public class FoodInfo
    {
        public int forceId;
        public int food;
        public int maxFood;
    }
    [NonSerialized]
    public static BattleManager Instance;
    [NonSerialized]
    public BattleUIManager battleUIManager;
    [NonSerialized]
    private GameObject mapObj;

    public const int gridCellSize = 3; // 每个格子的实际大小(米)

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
    private bool hasWin;
    public int idCounter = 100;
    public int tickIndex = 1;
    public int lastFoodDeductionTick = 0;

    public bool quickMode = true;
    public bool showUI = true;

    [NonSerialized]
    private Action<bool> battleEndCallback;

    private void Awake()
    {
        Instance = this;
    }

    public void BattleBegin(Player player1, Player player2, List<BattleCardData> cards1, List<BattleCardData> cards2, Action<bool> callback = null)
    {
        battleEndCallback = callback;
        playerInfoList.Clear();
        playerInfoList.Add(new FoodInfo() { forceId = player1.forceId, food = 100, maxFood = 100 });
        playerInfoList.Add(new FoodInfo() { forceId = player2.forceId, food = 100, maxFood = 100 });

        chessList.Clear();
        missileList.Clear();

        var newMapId = 1;
        gameFinish = false;
        if (showUI)
        {
            // 打印加载耗时
            var startTime = Time.realtimeSinceStartup;
            var mapNode = Resources.Load<GameObject>("Prefabs/BattleMaps/Map" + newMapId);
            if (mapObj != null)
                UnityEngine.Object.Destroy(mapObj);

            mapObj = UnityEngine.Object.Instantiate(mapNode, battleUIManager.NodeUnits.transform.parent);
            var endTime = Time.realtimeSinceStartup;
            Debug.Log("加载地图耗时：" + (endTime - startTime) + "秒");
        }

        BattleStatManager.Clear();
        // 计算双方总兵力
        int leftSoldierTotal = cards1.Sum(x => x.SoldierNum);
        int rightSoldierTotal = cards2.Sum(x => x.SoldierNum);
        BattleInfoTop.Instance.Init(player1.forceId, player2.forceId, leftSoldierTotal, rightSoldierTotal);

        if (showUI)
        {
            battleUIManager.BattleResultPanel.gameObject.SetActive(false);
            // 清空之前的单位
            foreach (Transform child in battleUIManager.NodeUnits.transform)
                UnityEngine.Object.Destroy(child.gameObject);
            battleUIManager.heroInfoGroup.Reset();
            battleUIManager.CreateCastleHUD(player1, GetSpawnPosition(1, 5));
            battleUIManager.CreateCastleHUD(player2, GetSpawnPosition(2, 5));            
        }
        //对cards1和card2都按HeroConfig的Range排序，确保远程在后面
        cards1.Sort((a, b) => HeroConfig.GetConfig(a.CardId).Range.CompareTo(HeroConfig.GetConfig(b.CardId).Range));
        cards2.Sort((a, b) => HeroConfig.GetConfig(a.CardId).Range.CompareTo(HeroConfig.GetConfig(b.CardId).Range));

        for (int i = 0; i < Math.Min(cards1.Count, 12); i++)
            SpawnHerosForRegion(player1, GetSpawnPosition(1, i), cards1[i]);

        for (int i = 0; i < Math.Min(cards2.Count, 12); i++)
            SpawnHerosForRegion(player2, GetSpawnPosition(2, i), cards2[i]);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.CheckAddSkill(chess);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.BattleBegin(chess);
        
        SaveToFile("battle.json");

        StartCoroutine(GameUpdate());
    }

    private Vector3 GetSpawnPosition(int side, int indx)
    {
        if(side == 1)
            return new Vector3(300 - (indx / 4) * 15, 7, 245 - (indx % 4) * 20);
        else
            return new Vector3(455 + (indx / 4) * 15, 7, 245 - (indx % 4) * 20);
    }

    public Chess SpawnUnitsForRegion(Player p, int soldierId, UnityEngine.Vector3 spawnPos)
    {
        var id = idCounter++;
        var action = new CreateChessAction(0, tickIndex, id, p.forceId, soldierId, spawnPos);
        AddChessAction(action);
        
        action.Doing();

        return action.CreatedChess;
    }

    private Chess SpawnHerosForRegion(Player p, UnityEngine.Vector3 spawnPoint, BattleCardData heroData)
    {
        var heroConfig = HeroConfig.GetConfig(heroData.CardId);

        var id = idCounter++;
        var action = new CreateChessAction(0, tickIndex, id, p.forceId, 0, spawnPoint, true, heroConfig.Id, heroData.Level, heroData.SoldierNum);
        AddChessAction(action);

        action.Doing();
        
        return action.CreatedChess;
    }

    public static float tickTimeReal = 0.1f; //加速功能
    
    private IEnumerator GameUpdate()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log($"GameUpdatett start realTime={Time.time}");
        var speed = 1;
        if (quickMode && showUI)
            speed = 10;
        else if(quickMode)
            speed = 400;
        while (!gameFinish)
        {
            for (int i = 0; i < 4; i++)
            {
                yield return new WaitForSeconds(tickTimeReal / 4); //高频帧，给missile这种表现用
                for (int j = 0; j < missileList.Count; j++)
                {
                    var missile = missileList[j];
                    if (missile != null)
                        missile.FixUpdate(tickIndex, (float)j / 4, 1f/40);
                }   
            }

            //  var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < speed; i++)
            {
                foreach (var chess in chessList.ToArray())
                {
                    if (chess != null && chess.hp > 0)
                        chess.LogicUpdate(tickIndex);
                }
                foreach (var missile in missileList.ToArray())
                {
                    if (missile != null)
                        missile.LogicUpdate(tickIndex);
                }
                foreach (var chess in chessList.ToArray())
                {
                    if (chess != null && chess.hp > 0)
                        chess.RenderUpdate();
                }
                coroutineManager.Update(tickTimeReal);
                tickIndex++;

                // 每个回合结束，玩家消耗食物
                if (tickIndex - lastFoodDeductionTick >= 200) // 每5秒扣除一次粮食 (5s / 0.025s = 200 ticks)
                {
                    foreach (var foodInfo in playerInfoList)
                    {
                        RoundFoodCost(foodInfo);
                    }
                    lastFoodDeductionTick = tickIndex;
                }
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
        Debug.Log($"GameUpdatett end realTime={Time.time}");

        for (int i = 0; i < chessList.Count; i++)
        {
            var chess = chessList[i];
            if (chess.isHero)
                GameManager.Instance.GetHero(chess.heroId).soldier = chess.hp; //设置士兵数目
        }

        if(showUI)
            battleUIManager.OnBattleEnd(playerInfoList.Select(foodInfo => foodInfo.forceId).ToList(), hasWin);

        // 调用战斗结束回调
        if (battleEndCallback != null)
        {
            battleEndCallback(hasWin);
        }
        SaveToFile("battle1.json");
    }

    public int GetTickFromTime(float time)
    {
        return (int)(time / tickTimeReal);
    }

    private void RoundFoodCost(FoodInfo foodInfo)
    {
        // 粮食扣除逻辑
        // 计算时间差，每5s，扣10点粮食
        if(foodInfo.food < 10)
        {
            var units = GetUnitsByForceId(foodInfo.forceId); //todo
            foreach(var unit in units)
                unit.LackFood((float)(10 - foodInfo.food) / 10);
        }
        foodInfo.food -= 10;
        if (foodInfo.food < 0)
            foodInfo.food = 0;
    }    

    public void CreateAttackMissile(Chess sourceChess, Chess targetChess)
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetChess.id, sourceChess.position, 0, 0);
        AddChessAction(action);

        action.Doing();
    }

    public void CreateSpellMissile(Chess sourceChess, Chess targetChess, Vector3 startPos, int skillId, int damage)
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetChess.id, startPos, skillId, damage);
        AddChessAction(action);
        
        action.Doing();
    }    

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, int skillId, int damage)
    {
        var id = idCounter++;
        var action = new CreateMissileAction(sourceChess.id, tickIndex, id, targetPos, sourceChess.position, skillId, damage, time);
        AddChessAction(action);
        
        action.Doing();
    }
    
    public void RemoveMissile(Missile missile)
    {
        var action = new RemoveMissileAction(missile.ownerId, tickIndex, missile.id);
        AddChessAction(action);
        action.Doing();
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
            x = Mathf.FloorToInt(worldPosition.x / gridCellSize) * gridCellSize;
            z = Mathf.FloorToInt(worldPosition.z / gridCellSize) * gridCellSize;
        }
        else
        {
            x = Mathf.CeilToInt(worldPosition.x / gridCellSize) * gridCellSize;
            z = Mathf.CeilToInt(worldPosition.z / gridCellSize) * gridCellSize;
        }
        return new Vector2Int(x, z);
    }

    public bool IsPositionFree(Chess unit, Vector3 targetPosition)
    {
        var ckSize = 7.5f;
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
        // 从chessList中移除死亡单位
        chessList.Remove(dieUnit);

        gameFinish = false;
        hasWin = false;
        // 检查所有阵营是否还有存活单位
        // 创建一个数组来统计每个阵营是否有存活单位，数组索引对应阵营编号减1
        bool[] sideHasUnits = new bool[playerInfoList.Count];
        int aliveSideCount = 0;

        var unit = GameManager.Instance.GetHero(dieUnit.heroId);
        if(unit != null)
            unit.soldier = 0; //设置士兵数目
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

        UnityEngine.Debug.Log($"id:{dieUnit.id} dieUnit.forceId:{dieUnit.forceId} 存活阵营数:{aliveSideCount}");
        // 如果只剩一个阵营有存活单位，显示重启按钮
        if (aliveSideCount <= 1)
        {
            gameFinish = true;
            hasWin = sideHasUnits[0];
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

        UnityEngine.Debug.Log($"RandomSelect limit:{limit} unitsInRange.Count:{unitsInRange.Count}");
        
        System.Random random = new System.Random();
        while (unitsInRange.Count > limit)
        {
            int indexToRemove = random.Next(0, unitsInRange.Count);
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
        actions.Add(action);
    }
    // 序列化到文件
    public void SaveToFile(string filePath)
    {
        string json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(filePath, json);
    }

    // 从文件反序列化
    public void LoadFromFile(string filePath)
    {
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

}
