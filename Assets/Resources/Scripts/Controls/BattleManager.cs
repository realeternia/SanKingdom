using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public Camera uiCamera;
    public bool isDebug = true; //自动判定的，不要改
    public GameObject Units;
    public int gridCellSize = 3; // 每个格子的实际大小(米)

    private List<int> playerList = new List<int>();
    private Dictionary<int, List<Vector2Int>> occupiedGrids = new Dictionary<int, List<Vector2Int>>(); // 所有被占据的格子，键为chess.id

    public bool showDebugCube = false;
    private Dictionary<Vector2Int, GameObject> debugGridCubes = new Dictionary<Vector2Int, GameObject>(); // 格子与调试cube的映射

    private List<Chess> chessList = new List<Chess>(); // 所有棋子

    private NLCoroutineManager coroutineManager = new NLCoroutineManager();

    private bool gameFinish = false;
    private bool hasWin;
    private MapConfig mapConfig;
   
    public HeroInfoGroup heroInfoGroup;
    public Button buttonRestart;
    public TMP_Text textRestart;
    public Button buttonInfo;
    public GameObject BattleResultPanel;
    public GameObject BattleResultCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    public GameObject BattleResultHeroCellPrefab; // 用于显示玩家战斗结果的单元格预制体
    private List<GameObject> battleResultCells = new List<GameObject>(); // 维护创建的结果单元格列表

    public GameObject HudNode;
    public GameObject BattleTextNode;
    private int idCounter = 100;
    public float time;
    public bool quickMode = true;

    void Start()
    {
        Instance = this;
        time = 10000;

        buttonRestart.onClick.AddListener(BattleEnd);
        buttonInfo.onClick.AddListener(ShowBattleResult);

        StartCoroutine(DebugBattleBeginCheck());
    }

    IEnumerator DebugBattleBeginCheck()
    {      
        // 延迟2秒
        yield return new WaitForSeconds(2f);
        ConfigManager.Init();
        if(isDebug)
        {
           // BattleBegin();
        }
    }

    public void BattleBegin(Player player1, Player player2, List<BattleCardData> cards1, List<BattleCardData> cards2)
    {
        playerList.Clear();
        playerList.Add(player1.forceId);
        playerList.Add(player2.forceId);

        var newMapId = 1;
        gameFinish = false;
        if (mapConfig == null || newMapId != mapConfig.Mapid)
        {
            // 打印加载耗时
            var startTime = Time.realtimeSinceStartup;
            var mapNode = Resources.Load<GameObject>("Prefabs/BattleMaps/Map" + newMapId);
            if (mapConfig != null)
                Destroy(mapConfig.gameObject);

            GameObject cell = Instantiate(mapNode, gameObject.transform.parent);
            mapConfig = cell.GetComponent<MapConfig>();
            var endTime = Time.realtimeSinceStartup;
            Debug.Log("加载地图耗时：" + (endTime - startTime) + "秒");
        }

        BattleStatManager.Clear();

        // 通知所有玩家开始战斗
        player1.OnBattleBegin();
        player2.OnBattleBegin();

        BattleResultPanel.gameObject.SetActive(false);
        SpawnUnitsInRegions(player1, cards1, player2, cards2);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.CheckAddSkill(chess);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.BattleBegin(chess);
        StartCoroutine(GameUpdate());
    }

    public void BattleEnd()
    {
        // 销毁所有结果单元格
        foreach (GameObject cell in battleResultCells)
        {
            if (cell != null)
                Destroy(cell);
        }
        battleResultCells.Clear();
        
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        chessList.Clear();

        foreach (Transform cell in HudNode.transform)
        {
            Destroy(cell.gameObject);
        }

        PanelManager.Instance.ShowWorld();
    }

    public void ShowBattleResult()
    {
        var top10 = BattleStatManager.GetTop10();
        buttonInfo.gameObject.SetActive(false);
        // 获取RectTransform组件并设置宽度
        RectTransform battleResultRect = BattleResultPanel.GetComponent<RectTransform>();
        battleResultRect.sizeDelta = new Vector2(battleResultRect.sizeDelta.x + 800, battleResultRect.sizeDelta.y);
        for (int i = 0; i < top10.Count; i++)
        {
            var battleStat = top10[i];
            var cell = Instantiate(BattleResultHeroCellPrefab, BattleResultPanel.transform);
            cell.GetComponent<BattleResultHeroCellControl>().SetData(battleStat, i + 1);

            // 设置位置，每个单元格垂直偏移50
            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(302 + 700, -120 - i * 50); // 起始位置向下100，每个单元格间距50

            battleResultCells.Add(cell);
        }
    }

    private void SpawnUnitsInRegions(Player player1, List<BattleCardData> cards1, Player player2, List<BattleCardData> cards2)
    {
        // 清空之前的单位
        foreach (Transform child in Units.transform)
        {
            Destroy(child.gameObject);
        }
        occupiedGrids.Clear();
        heroInfoGroup.Reset();

        List<Vector2Int> unitGrids = new List<Vector2Int>();
        // 生成墙
        for (int i = 0; i < mapConfig.WallNode.transform.childCount; i++)
        {
            var wallNodeCell = mapConfig.WallNode.transform.GetChild(i);
            // 使用GetOccupiedGrids方法获取需要锁定的格子列表
            List<Vector2Int> requiredGrids = GetOccupiedGrids(wallNodeCell.transform.position);
            // 锁定新格子

            foreach (var gridPos in requiredGrids)
            {
                unitGrids.Add(gridPos);
                CreateDebugCube(300001, gridPos);
                //  UnityEngine.Debug.Log("Lock " + gridPos + " for wall");
            }
        }
        occupiedGrids[300001] = unitGrids;

        if (!quickMode)
        {
            CreateCastleHUD(player1, mapConfig.RegionHeroSide1[4]);
            CreateCastleHUD(player2, mapConfig.RegionHeroSide2[4]);
        }

        for (int i = 0; i < Math.Min(cards1.Count, mapConfig.RegionHeroSide1.Length); i++)
            SpawnHerosForRegion(player1, i, mapConfig.RegionHeroSide1[i], cards1[i], 1);

        for (int i = 0; i < Math.Min(cards2.Count, mapConfig.RegionHeroSide2.Length); i++)
            SpawnHerosForRegion(player2, i, mapConfig.RegionHeroSide2[i], cards2[i], 2);

           //   SpawnHerosForRegion(player1, 0, mapConfig.RegionHeroSide1[0], cards1[0], 1);
           //    SpawnHerosForRegion(player2, 0, mapConfig.RegionHeroSide2[0], cards2[0], 2);


    }

    public Chess SpawnUnitsForRegion(Player p, int soldierId, int posId, UnityEngine.Vector3 spawnPos, int side, string imgPath)
    {
        var soldierConfig = SoldierConfig.GetConfig(soldierId);
        ChessViewObj viewObj = null;
        if (!quickMode)
        {
            GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/" + soldierConfig.Model);

            // 实例化单位
            GameObject unitModel = Instantiate(unitPrefab, spawnPos, Quaternion.identity, Units.transform);
            unitModel.name = $"UnitBing_{side}_{idCounter}";
            unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            // 获取并初始化Chess组件
            viewObj = unitModel.GetComponent<ChessViewObj>();
        }
        Chess chess = new Chess();
        if (viewObj != null)
            chess.viewObj = viewObj;

        chess.id = idCounter;
        chess.isHero = false;
        chess.side = side;
        chess.chessName = imgPath;
        chess.maxHp = soldierConfig.Hp;
        chess.moveSpeed = soldierConfig.MoveSpeed;
        chess.attackRange = soldierConfig.Range;
        chess.attackDamage = soldierConfig.Atk;
        chess.isFakeHero = soldierConfig.Model == "UnitHero";

        chess.hitEffect = soldierConfig.HitEffect;
        chess.soldierId = soldierId;
        chess.playerId = p.forceId;
        chess.Init(p.forceId, posId, p.lineColor);

        chessList.Add(chess);
        chess.SetPosition(spawnPos);
        idCounter++;

        return chess;
    }

    private Chess SpawnHerosForRegion(Player p, int posId, GameObject spawnPoint, BattleCardData heroData, int side)
    {
        var heroConfig = HeroConfig.GetConfig(heroData.CardId);
        ChessViewObj viewObj = null;
        if (!quickMode)
        {
            Debug.Log($"SpawnHerosForRegion Hero_{side}_{idCounter}");
            GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/UnitHero");

            // 实例化单位
            GameObject unitModel = Instantiate(heroPrefab, spawnPoint.transform.position, Quaternion.identity, Units.transform);
            unitModel.name = $"Hero_{side}_{idCounter}";
            unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            viewObj = unitModel.GetComponent<ChessViewObj>();
        }

        Chess chess = new Chess();
        if (viewObj != null)
            chess.viewObj = viewObj;
        chess.id = idCounter;
        chess.isHero = true;
        chess.heroId = (int)heroConfig.Id;
        chess.side = side;
        chess.chessName = heroConfig.Icon;
        chess.hitEffect = heroConfig.HitEffect;
        chess.missileSpeed = heroConfig.MissileSpeed;
        chess.missileHight = heroConfig.MissileHight;

        if (side <= 2)
        {
            var heroInfo = heroInfoGroup.AddHero(side, heroConfig.Id, heroData.Level);
            chess.heroInfo = heroInfo;
        }
        chess.CheckInitAttr(heroData.Level, heroData.SoliderNum);
        chess.Init(p.forceId, posId, p.lineColor);

        chessList.Add(chess);
        chess.SetPosition(spawnPoint.transform.position);
        idCounter++;

        return chess;
    }


    // 创建血条HUD
    private void CreateCastleHUD(Player p, GameObject castleSpawn)
    {
        // 加载Hud预制体
        GameObject hudPrefab = Resources.Load<GameObject>("Prefabs/HudCastle");

        // 实例化HUD对象
        GameObject hudObj = Instantiate(hudPrefab, HudNode.transform);
        hudObj.name = "CastleHUD";

        // 获取ChessHUD组件
        var hud = hudObj.GetComponent<CastleHUD>();

        // 初始化血条显示
        hud.Init(p, castleSpawn);
        p.castleHUD = hud;
    }

    public static float tickTime = 0.025f;
    public static float tickTimeReal = 0.025f; //加速功能
    private IEnumerator GameUpdate()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log($"GameUpdatett start logicTime={time} realTime={Time.time}");
        var speed = 1;
        if(quickMode)
            speed = 200;
        while (!gameFinish)
        {
            yield return new NLWaitForPreciseTime(tickTimeReal);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < speed; i++)
            {
                time += tickTime;
                coroutineManager.Update(tickTime);

                foreach (var chess in chessList.ToArray())
                {
                    if (chess != null && chess.hp > 0)
                        chess.LogicUpdate(tickTime);
                }
                
                // 每个回合结束，玩家消耗食物
                foreach (var player in GameManager.Instance.players)
                {
                    player.RoundFoodCost();
                }
            }
            sw.Stop();
            UnityEngine.Debug.Log($"GameUpdate 循环耗时: {sw.ElapsedMilliseconds} ms");
        }
        Debug.Log($"GameUpdatett end logicTime={time} realTime={Time.time}");

        {
            if (hasWin)
                textRestart.text = "你获胜了!!!";
            else
                textRestart.text = "你输了!!!";

            // 销毁之前的结果单元格
            foreach (GameObject cell in battleResultCells)
            {
                if (cell != null)
                {
                    Destroy(cell);
                }
            }
            battleResultCells.Clear();

            // 为每个玩家创建结果单元格
            if (BattleResultCellPrefab != null)
            {
                // 根据玩家的 mark 进行排序
                var sortedPlayers = playerList
                    .Select(id => new { Id = id, Mark = GameManager.Instance.GetPlayer(id)?.mark ?? 0 })
                    .OrderByDescending(p => p.Mark)
                    .Select(p => p.Id)
                    .ToArray();
                for (int i = 0; i < sortedPlayers.Length; i++)
                {
                    int playerId = sortedPlayers[i];
                    // 创建结果单元格
                    GameObject cell = Instantiate(BattleResultCellPrefab, BattleResultPanel.transform);

                    // 设置位置，每个单元格垂直偏移50
                    RectTransform rectTransform = cell.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = new Vector2(302, -120 - i * 50); // 起始位置向下100，每个单元格间距50
                    }

                    // 获取并设置单元格数据
                    BattleResultCellControl cellControl = cell.GetComponent<BattleResultCellControl>();
                    if (cellControl != null)
                    {
                        var player = GameManager.Instance.GetPlayer(playerId);
                        if (player != null)
                        {
                            cellControl.SetData(player, i + 1, 1);
                        }
                    }

                    // 添加到维护列表
                    battleResultCells.Add(cell);
                }
            }
            buttonInfo.gameObject.SetActive(true);
            // 获取RectTransform组件并设置宽度
            RectTransform battleResultRect = BattleResultPanel.GetComponent<RectTransform>();
            battleResultRect.sizeDelta = new Vector2(650, battleResultRect.sizeDelta.y);
            BattleResultPanel.gameObject.SetActive(true);
        }
    }


    public void CreateAttackMissile(Chess sourceChess, Chess targetChess, string effectName)
    {
        MissileViewObj viewObj = null;
        if(!quickMode)
        {
            var missilePrefab = Resources.Load<MissileViewObj>("Prefabs/MissileCom");
            viewObj = Instantiate(missilePrefab, sourceChess.position, Quaternion.identity, Units.transform);
        }
        var missile = new Missile();
        missile.viewObj = viewObj;
        missile.Init(sourceChess, 1, effectName);
        missile.SetPosition(sourceChess.position);
        missile.MoveToTarget(targetChess, sourceChess.missileSpeed, sourceChess.missileHight);
    }

    public void CreateSpellMissile(Chess sourceChess, Chess targetChess, Vector3 startPos, int skillId, int damage, string effectName)
    {
        MissileViewObj viewObj = null;
        if(!quickMode)
        {
            var missilePrefab = Resources.Load<MissileViewObj>("Prefabs/MissileCom");
            viewObj = Instantiate(missilePrefab, startPos, Quaternion.identity, Units.transform);
        }
        var missile = new Missile();
        missile.viewObj = viewObj;
        missile.Init(sourceChess, 1, effectName);
        missile.SetPosition(startPos);
        missile.SetSkillInfo(skillId, damage);
        missile.MoveToTarget(targetChess, Mathf.Max(sourceChess.missileSpeed, 14), sourceChess.missileHight);
    }    

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, float speed, float size, int skillId, int damage, string effectName)
    {
        MissileViewObj viewObj = null;
        if(!quickMode)
        {
            var missilePrefab = Resources.Load<MissileViewObj>("Prefabs/MissileCom");
            viewObj = Instantiate(missilePrefab, sourceChess.position, Quaternion.identity, Units.transform);
        }
        var missile = new Missile();
        missile.viewObj = viewObj;
        missile.Init(sourceChess, size, effectName);
        missile.SetPosition(sourceChess.position);
        missile.SetSkillInfo(skillId, damage);
        missile.MoveToDirection(targetPos, time, speed);
    }


    // 世界坐标转格子坐标
    public Vector2Int WorldToGridPosition(Vector3 worldPosition, bool FloorToInt)
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

    // 尝试锁定目标位置的格子
    public bool TryLockGridPositions(Chess unit, Vector3 targetPosition, out List<Vector2Int> requiredGrids)
    {
        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
        // requiredGrids = GetOccupiedGrids(targetPosition);
        // // UnityEngine.Debug.Log($"id:{unit.id} requiredGrids: Target Position = {targetPosition}, Collider Size = {collider.bounds.size}");
        // // string gridPositions = string.Join(", ", requiredGrids);
        // // UnityEngine.Debug.Log($"Grids: {gridPositions}");

        // // 检查所有格子是否可用
        // foreach (var gridPos in requiredGrids)
        // {
        //     foreach (var entry in occupiedGrids)
        //     {
        //         if (entry.Key != unit.id)
        //         {
        //             foreach (var occupiedGrid in entry.Value)
        //             {
        //                 if (occupiedGrid.x == gridPos.x && occupiedGrid.y == gridPos.y)
        //                 {
        //                  //   UnityEngine.Debug.Log("Grid " + gridPos + " is already occupied by unit: " + entry.Key);
        //                     return false; // 格子不可用
        //                 }
        //             }
        //         }
        //     }
        // }
        requiredGrids = null;
        return true;
    }

    public void DoLockGridPositions(Chess unit, List<Vector2Int> requiredGrids)
    {
        // ReleaseGridPositions(unit.id);
        // // 锁定新格子
        // List<Vector2Int> unitGrids = new List<Vector2Int>();
        // foreach (var gridPos in requiredGrids)
        // {
        //     unitGrids.Add(gridPos);
        //     CreateDebugCube(unit.id, gridPos);
        //  //   UnityEngine.Debug.Log("Lock " + gridPos + " for unit: " + unit.id);
        // }

        // // 存储单位占据的格子
        // occupiedGrids[unit.id] = unitGrids;
    }

    public void ForceLockGridPositions(Chess unit, Vector3 targetPosition)
    {
        // 使用GetOccupiedGrids方法获取需要锁定的格子列表
        List<Vector2Int> requiredGrids = GetOccupiedGrids(targetPosition);
        List<Vector2Int> toRemoves = new List<Vector2Int>();

        // 检查所有格子是否可用
        foreach (var gridPos in requiredGrids)
        {
            foreach (var entry in occupiedGrids)
            {
                if (entry.Key != unit.id)
                {
                    foreach (var occupiedGrid in entry.Value)
                    {
                        if (occupiedGrid.x == gridPos.x && occupiedGrid.y == gridPos.y)
                            toRemoves.Add(occupiedGrid);
                    }
                }
            }
        }

        ReleaseGridPositions(unit.id);
        requiredGrids.RemoveAll(x => toRemoves.Contains(x));
        // 锁定新格子
        List<Vector2Int> unitGrids = new List<Vector2Int>();
        foreach (var gridPos in requiredGrids)
        {
            unitGrids.Add(gridPos);
            CreateDebugCube(unit.id, gridPos);
         //   UnityEngine.Debug.Log("Lock " + gridPos + " for unit: " + unit.id);
        }

        // 存储单位占据的格子
        occupiedGrids[unit.id] = unitGrids;
    }    

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        if (isForce)
        {
            ForceLockGridPositions(unit, targetPosition);
            unit.SetPosition(targetPosition);

            return true;
        }
        else
        { 
            if(TryLockGridPositions(unit, targetPosition, out List<Vector2Int> requiredGrids))
            {
                DoLockGridPositions(unit, requiredGrids);
                unit.SetPosition(targetPosition);
                return true;
            }
            return false;
        }

    }

    // 获取指定位置和碰撞体占据的所有格子
    public List<Vector2Int> GetOccupiedGrids(Vector3 position)
    {
        List<Vector2Int> occupiedGrids = new List<Vector2Int>();

        // // 获取碰撞体边界
        // Vector3 boundsSize = new Vector3(10, 0.5f, 10);
        // Vector3 halfBounds = boundsSize / 3f;

        // // 计算边界的最小和最大世界坐标
        // Vector3 minWorldPos = position - halfBounds;
        // Vector3 maxWorldPos = position + halfBounds;

        // // 将世界坐标转换为格子坐标
        // Vector2Int minGridPos = WorldToGridPosition(minWorldPos, true);
        // Vector2Int maxGridPos = WorldToGridPosition(maxWorldPos, false);

        // // 遍历从最小到最大格子坐标的所有格子
        // for (int x = minGridPos.x; x <= maxGridPos.x; x+= gridCellSize)
        // {
        //     for (int z = minGridPos.y; z <= maxGridPos.y; z+= gridCellSize)
        //     {
        //         Vector2Int currentGrid = new Vector2Int(x, z);
        //         occupiedGrids.Add(currentGrid);
        //     }
        // }
        
        return occupiedGrids;
    }

    // 释放指定位置的格子
    // 释放指定单位占据的格子
    public void ReleaseGridPositions(int id)
    {
        // 检查单位是否有占据的格子
        if (occupiedGrids.ContainsKey(id))
        {
            // 删除该单位占据的所有格子的调试cube
            foreach (var gridPos in occupiedGrids[id])
            {
                DestroyDebugCube(gridPos);
            }
            occupiedGrids[id].Clear();
          //  UnityEngine.Debug.Log("Released all grids for unit: " + unit.id);
        }
    }

    // 创建调试用的cube
    private void CreateDebugCube(int oid, Vector2Int gridPos)
    {
        if(!showDebugCube)
            return;

        if (debugGridCubes.ContainsKey(gridPos))
            return; // 已存在则不再创建

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(gridPos.x, 0.5f, gridPos.y);
        cube.transform.localScale = new Vector3(gridCellSize * 0.9f, 1f, gridCellSize * 0.9f);
        // 将oid散列到RGB值中
        int hash = oid * oid * 31 + oid * 3779; // 对哈希值进行位运算打散，避免值为1时不被打散的问题
        float r = Mathf.Abs((float)(hash & 0xFF) / 255f);
        float g = Mathf.Abs((float)((hash >> 8) & 0xFF) / 255f);
        float b = Mathf.Abs((float)((hash >> 16) & 0xFF) / 255f);
        cube.GetComponent<Renderer>().material.color = new Color(r, g, b);
        cube.name = "GridCube_" + hash;
        cube.transform.parent = Units.transform;
        cube.transform.localPosition += new Vector3(0, 10f, 0);

        debugGridCubes[gridPos] = cube;
    }

    // 销毁调试用的cube
    private void DestroyDebugCube(Vector2Int gridPos)
    {
        if(!showDebugCube)
            return;

        if (debugGridCubes.TryGetValue(gridPos, out GameObject cube))
        {
            Destroy(cube);
            debugGridCubes.Remove(gridPos);
        }
    }

    public bool IsEnemy(int a, int b)
    {
        if (mapConfig.TeamMode == 1)
        {
            // 阵营1、3、4为一个阵营，阵营2、5、6为另一个阵营
            bool isTeam1 = a == 1 || a == 3 || a == 5 || a == 7;
            bool isTeam2 = a == 2 || a == 4 || a == 6 || a == 8;
            bool targetIsTeam1 = b == 1 || b == 3 || b == 5 || b == 7;
            bool targetIsTeam2 = b == 2 || b == 4 || b == 6 || b == 8;

            // 不同阵营之间是敌人
            return (isTeam1 && targetIsTeam2) || (isTeam2 && targetIsTeam1);
        }
        else if (mapConfig.TeamMode == 0)
        {
            return a != b && (a + 1) / 2 == (b + 1) / 2;
        }
        else
        {
            return a != b;
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
        bool[] sideHasUnits = new bool[2];
        int aliveSideCount = 0;

        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                int sideIndex = chessComponent.side - 1;
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

        UnityEngine.Debug.Log($"id:{dieUnit.id} dieUnit.side:{dieUnit.side} 存活阵营数:{aliveSideCount}");
        // 如果只剩一个阵营有存活单位，显示重启按钮
        if (aliveSideCount <= 1)
        {
            gameFinish = true;
            hasWin = sideHasUnits[0];
        }
    }

    public bool CheckInRange(Vector3 pos1, Vector3 pos2, float range)
    {
        Vector2Int pos1a = WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldToGridPosition(pos2, true);

        return Vector2Int.Distance(pos1a, pos2a) <= range;
    }

    public float GetRange(Vector3 pos1, Vector3 pos2)
    {
        Vector2Int pos1a = WorldToGridPosition(pos1, true);
        Vector2Int pos2a = WorldToGridPosition(pos2, true);

        return Vector2Int.Distance(pos1a, pos2a);
    }


    public List<Chess> GetUnitsInRange(Vector3 wPos, float range, int mySide, bool findEnemy)
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
                        if(IsEnemy(chessComponent.side, mySide))
                            unitsInRange.Add(chessComponent);
                    }
                    else
                    {
                        if(!IsEnemy(chessComponent.side, mySide)) 
                            unitsInRange.Add(chessComponent);
                    }
                }
            }
        }

        return unitsInRange;
    }

    public void RandomSelect(List<Chess> unitsInRange, int limit)
    {
        if(unitsInRange.Count > limit)
        {
            System.Random random = new System.Random();
            while (unitsInRange.Count > limit)
            {
                int indexToRemove = random.Next(0, unitsInRange.Count);
                unitsInRange.RemoveAt(indexToRemove);
            }
        }
    }

    public List<Chess> GetUnitsMySide(Vector3 wPos, float range, int mySide)
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
                    if(chessComponent.side == mySide)
                        unitsInRange.Add(chessComponent);
                }
            }
        }
        return unitsInRange;
    }

    public List<Chess> GetUnitsMySide(int mySide)
    {
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if (chessComponent.side == mySide)
                    unitsInRange.Add(chessComponent);
            }
        }
        return unitsInRange;
    }

    public Chess FindByHeroIdAndSide(int heroId, int side)
    {
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if(chessComponent.isHero && chessComponent.heroId == heroId && chessComponent.side == side)
                    return chessComponent;
            }
        }   
        return null;
    }

    public List<Chess> GetUnitsMySidePosType(int mySide, int pos, bool isHero, int selectType)
    {
        List<Chess> unitsInRange = new List<Chess>();
        foreach (var chessComponent in chessList)
        {
            if (chessComponent != null && chessComponent.hp > 0 && !chessComponent.isShadow)
            {
                if (chessComponent.side == mySide && chessComponent.isHero == isHero)
                {
                    if(selectType == 1 && pos / 3 == chessComponent.pos / 3)
                        unitsInRange.Add(chessComponent);
                    else if(selectType == 2 && ((pos % 3) == (chessComponent.pos % 3)))
                        unitsInRange.Add(chessComponent);
                    else if(selectType == 3)
                        unitsInRange.Add(chessComponent);
                }
            }
        }
        return unitsInRange;
    }    

    public void AddBattleText(string text, UnityEngine.Vector3 worldPos, UnityEngine.Vector2 speed, Color color, int duration)
    {
        if(quickMode)
            return;

        var prefab = Resources.Load<GameObject>("Prefabs/BattleTxt");
        var battleText = Instantiate(prefab, BattleTextNode.transform);

        // 将世界坐标转换为屏幕坐标
        RectTransform rectTransform = battleText.GetComponent<RectTransform>();
        RectTransform parentCanvas = rectTransform.parent as RectTransform;
        var screenPos = TransformWorldToScreen(worldPos + new UnityEngine.Vector3(5, 0, 5), parentCanvas);

        rectTransform.anchoredPosition = screenPos;

        var textCtr = battleText.transform.GetChild(0).GetComponent<TMP_Text>();
        textCtr.color = color;
        textCtr.text = text;
        Destroy(battleText, duration);

        //如果speed不为0，开一个协程移动文本
        if(speed != UnityEngine.Vector2.zero)
        {
            StartCoroutine(MoveText(battleText, speed, duration));
        }
    }

    // 战斗文本移动协程
    private IEnumerator MoveText(GameObject battleText, UnityEngine.Vector2 speed, int duration)
    {
        //获得屏幕分辨率
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        UnityEngine.Debug.Log($"screenWidth:{screenWidth} screenHeight:{screenHeight}");

        // 假设设计分辨率为 1920x1080，可根据实际项目修改
        const float designWidth = 2048f;
        const float designHeight = 1536f;
        // 根据当前屏幕分辨率计算缩放比例
        float scaleX = (float)screenWidth / designWidth;
        float scaleY = (float)screenHeight / designHeight;

        var nowTime = Time.time;
        float startTime = nowTime;
        float endTime = startTime + duration;
        RectTransform rectTransform = battleText.GetComponent<RectTransform>();
        var lastTime = nowTime;

        while (lastTime < endTime)
        {
            // 考虑分辨率和缩放因素计算移动距离
            var timeDiff = Time.time - lastTime;
            lastTime = Time.time;

            float moveX = speed.x * timeDiff * scaleX;
            float moveY = speed.y * timeDiff * scaleY / 80;

            if (rectTransform == null)
            {
                Destroy(battleText);
                yield break;
            }

            // 更新位置
            rectTransform.Translate(new Vector3(moveX, moveY, 0));

            yield return new WaitForSeconds(0.05f); // 使用 yield return null 在下一帧继续执行，保证流畅移动

        }
    }

    public Vector2 TransformWorldToScreen(Vector3 worldPosition, RectTransform canvas)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas,
            screenPosition,
            uiCamera,
            out localPosition
        );

        return localPosition;
    }

    public IEnumerator StartNLCoroutine(IEnumerator routine)
    {
        coroutineManager.StartCoroutine(routine);
        return routine;
    }
    public void StopNLCoroutine(IEnumerator routine)
    {
        coroutineManager.StopCoroutine(routine);
    }


    // 管理器销毁时释放所有格子
    private void OnDestroy()
    {
        occupiedGrids.Clear();

        // 销毁所有调试cube
        foreach (var cube in debugGridCubes.Values)
        {
            Destroy(cube);
        }
        debugGridCubes.Clear();
    }
}