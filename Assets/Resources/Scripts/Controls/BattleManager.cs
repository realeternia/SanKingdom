using System;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public class FoodInfo
    {
        public int food;
        public int maxFood;
    }
    public static BattleManager Instance;

    private GameObject mapObj;

    public int gridCellSize = 3; // 每个格子的实际大小(米)

    private List<int> playerForceIdList = new List<int>();
    private Dictionary<int, FoodInfo> forceId2FoodDict = new Dictionary<int, FoodInfo>();
    private int cityAtkId;

    private int cityDefId;

    private List<Chess> chessList = new List<Chess>(); // 所有棋子
    private List<int> attackHeroIds = new List<int>();
    private List<int> defHeroIds = new List<int>();

    private NLCoroutineManager coroutineManager = new NLCoroutineManager();

    private bool gameFinish = false;
    private bool hasWin;    
    private int idCounter = 100;
    public float time;
    private float lastFoodDeductionTime = 0;

    public bool quickMode = true;
    public bool showUI = true;

    public BattleUIManager battleUIManager;

    private void Awake()
    {
        Instance = this;
        time = 10000;
    }

    public void BattleBegin(Player player1, Player player2, int cityAtkId, int cityDefId, List<BattleCardData> cards1, List<BattleCardData> cards2)
    {
        playerForceIdList.Clear();
        playerForceIdList.Add(player1.forceId);
        playerForceIdList.Add(player2.forceId);
        this.cityAtkId = cityAtkId;
        this.cityDefId = cityDefId;

        forceId2FoodDict.Clear(); //todo 这里需要传进来
        forceId2FoodDict.Add(player1.forceId, new FoodInfo() { food = 100, maxFood = 100 });
        forceId2FoodDict.Add(player2.forceId, new FoodInfo() { food = 100, maxFood = 100 });

        attackHeroIds.Clear();
        defHeroIds.Clear();
        attackHeroIds.AddRange(cards1.Select(x => x.CardId));
        defHeroIds.AddRange(cards2.Select(x => x.CardId));

        chessList.Clear();

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
        }
        SpawnUnitsInRegions(player1, cards1, player2, cards2);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.CheckAddSkill(chess);

        foreach (var chess in chessList.ToArray()) //防止召唤
            SkillManager.BattleBegin(chess);
            
        StartCoroutine(GameUpdate());
    }

    private void SpawnUnitsInRegions(Player player1, List<BattleCardData> cards1, Player player2, List<BattleCardData> cards2)
    {
        if (showUI)
        {
            // 清空之前的单位
            foreach (Transform child in battleUIManager.NodeUnits.transform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        if (showUI)
        {
            battleUIManager.heroInfoGroup.Reset();
            battleUIManager.CreateCastleHUD(player1, GetSpawnPosition(1, 5));
            battleUIManager.CreateCastleHUD(player2, GetSpawnPosition(2, 5));
        }

        //对cards1和card2都按HeroConfig的Range排序，确保远程在后面
        cards1.Sort((a, b) => HeroConfig.GetConfig(a.CardId).Range.CompareTo(HeroConfig.GetConfig(b.CardId).Range));
        cards2.Sort((a, b) => HeroConfig.GetConfig(a.CardId).Range.CompareTo(HeroConfig.GetConfig(b.CardId).Range));

        for (int i = 0; i < Math.Min(cards1.Count, 12); i++)
            SpawnHerosForRegion(player1, i, GetSpawnPosition(1, i), cards1[i], 1);

        for (int i = 0; i < Math.Min(cards2.Count, 12); i++)
            SpawnHerosForRegion(player2, i, GetSpawnPosition(2, i), cards2[i], 2);

           //   SpawnHerosForRegion(player1, 0, mapConfig.RegionHeroSide1[0], cards1[0], 1);
           //    SpawnHerosForRegion(player2, 0, mapConfig.RegionHeroSide2[0], cards2[0], 2);
    }

    private Vector3 GetSpawnPosition(int side, int indx)
    {
        if(side == 1)
            return new Vector3(300 - (indx / 4) * 15, 7, 245 - (indx % 4) * 20);
        else
            return new Vector3(455 + (indx / 4) * 15, 7, 245 - (indx % 4) * 20);
    }

    public Chess SpawnUnitsForRegion(Player p, int soldierId, int posId, UnityEngine.Vector3 spawnPos, int side, string imgPath)
    {
        var soldierConfig = SoldierConfig.GetConfig(soldierId);
        ChessViewObj viewObj = null;
        if (showUI)
        {
            GameObject unitPrefab = Resources.Load<GameObject>("Prefabs/" + soldierConfig.Model);
            GameObject unitModel = UnityEngine.Object.Instantiate(unitPrefab, spawnPos, Quaternion.identity, battleUIManager.NodeUnits.transform);
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
        chess.forceId = p.forceId;
        chess.Init(p.forceId, posId, p.lineColor);

        chessList.Add(chess);
        chess.SetPosition(spawnPos);
        idCounter++;

        return chess;
    }

    private Chess SpawnHerosForRegion(Player p, int posId, UnityEngine.Vector3 spawnPoint, BattleCardData heroData, int side)
    {
        var heroConfig = HeroConfig.GetConfig(heroData.CardId);
        ChessViewObj viewObj = null;
        if (showUI)
        {
            Debug.Log($"SpawnHerosForRegion Hero_{side}_{idCounter}");
            GameObject heroPrefab = Resources.Load<GameObject>("Prefabs/UnitHero");

            // 实例化单位
            GameObject unitModel = UnityEngine.Object.Instantiate(heroPrefab, spawnPoint, Quaternion.identity, battleUIManager.NodeUnits.transform);
            unitModel.name = $"Hero_{side}_{idCounter}";
            unitModel.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            viewObj = unitModel.GetComponent<ChessViewObj>();
        }

        Chess chess = new Chess();
        if (viewObj != null)
            chess.viewObj = viewObj;
        chess.id = idCounter;
        chess.isHero = true;
        chess.heroId = heroConfig.Id;
        chess.side = side;
        chess.chessName = heroConfig.Icon;
        chess.hitEffect = heroConfig.HitEffect;
        chess.missileSpeed = heroConfig.MissileSpeed;
        chess.missileHight = heroConfig.MissileHight;

        if (showUI)
        {
            var heroInfo = battleUIManager.heroInfoGroup.AddHero(side, heroConfig.Id, heroData.Level);
            chess.heroInfo = heroInfo;
        }
        chess.CheckInitAttr(heroData.Level, heroData.SoldierNum);
        chess.Init(p.forceId, posId, p.lineColor);

        chessList.Add(chess);
        chess.SetPosition(spawnPoint);
        idCounter++;

        return chess;
    }

    public static float tickTime = 0.025f;
    public static float tickTimeReal = 0.025f; //加速功能
    private IEnumerator GameUpdate()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log($"GameUpdatett start logicTime={time} realTime={Time.time}");
        var speed = 1;
        if (quickMode && showUI)
            speed = 10;
        else if(quickMode)
            speed = 400;
        while (!gameFinish)
        {
            yield return new WaitForSeconds(tickTimeReal);
            //  var sw = System.Diagnostics.Stopwatch.StartNew();
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
                foreach (var forceId in playerForceIdList)
                {
                    RoundFoodCost(forceId);
                }
            }
            var leftSoldierTotal = chessList.Sum(x => x.forceId == playerForceIdList[0] && x.isHero ? Math.Max(0, x.hp) : 0);
            var rightSoldierTotal = chessList.Sum(x => x.forceId == playerForceIdList[1] && x.isHero ? Math.Max(0, x.hp) : 0);
            BattleInfoTop.Instance.UpdateSoldierCount(leftSoldierTotal, rightSoldierTotal);
            //    sw.Stop();
            //    UnityEngine.Debug.Log($"GameUpdate 循环耗时: {sw.ElapsedMilliseconds} ms");
        }
        Debug.Log($"GameUpdatett end logicTime={time} realTime={Time.time}");

        for (int i = 0; i < chessList.Count; i++)
        {
            var chess = chessList[i];
            if (chess.isHero)
                GameManager.Instance.GetHero(chess.heroId).soldier = chess.hp; //设置士兵数目
        }

        if(showUI)
            battleUIManager.OnBattleEnd(playerForceIdList, hasWin);
    }

    private void RoundFoodCost(int forceId)
    {
        // 粮食扣除逻辑
        if (time - lastFoodDeductionTime >= 5f) // 每5秒扣除一次粮食
        {
            var food = forceId2FoodDict[forceId];
            // 计算时间差，每5s，扣10点粮食
            if(food.food < 10)
            {
                var units = GetUnitsByForceId(forceId); //todo
                foreach(var unit in units)
                    unit.LackFood((float)(10 - food.food) / 10);
            }
            forceId2FoodDict[forceId].food -= 10;
            if (forceId2FoodDict[forceId].food < 0)
                forceId2FoodDict[forceId].food = 0;

            // 更新上次扣除粮食的时间
            lastFoodDeductionTime = time;
        }
    }    

    public void CreateAttackMissile(Chess sourceChess, Chess targetChess, string effectName)
    {
        var missile = new Missile();
        missile.Init(sourceChess, sourceChess.position, 1, effectName);
        missile.MoveToTarget(targetChess, sourceChess.missileSpeed, sourceChess.missileHight);
    }

    public void CreateSpellMissile(Chess sourceChess, Chess targetChess, Vector3 startPos, int skillId, int damage, string effectName)
    {
        var missile = new Missile();
        missile.Init(sourceChess, startPos, 1, effectName);
        missile.SetSkillInfo(skillId, damage);
        missile.MoveToTarget(targetChess, Mathf.Max(sourceChess.missileSpeed, 14), sourceChess.missileHight);
    }    

    public void CreateSpellMissile(Chess sourceChess, Vector3 targetPos, float time, float speed, float size, int skillId, int damage, string effectName)
    {
        var missile = new Missile();
        missile.Init(sourceChess, sourceChess.position, size, effectName);
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

    public bool MoveTo(Chess unit, Vector3 targetPosition, bool isForce = false)
    {
        if (isForce)
        {
            unit.SetPosition(targetPosition);

            return true;
        }
        else
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

            unit.SetPosition(targetPosition);
            return true;
        }

    }

    public bool IsEnemy(int a, int b)
    {
        return a != b;
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

        var unit = GameManager.Instance.GetHero(dieUnit.heroId);
        if(unit != null)
            unit.soldier = 0; //设置士兵数目
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

            if (hasWin)
            {
                GameManager.Instance.GetCity(cityDefId).Occupy(playerForceIdList[0], attackHeroIds, playerForceIdList[1], defHeroIds);
                GameManager.Instance.GetCity(cityAtkId).RecalculateHeros(); //因为有一帮人出去了
            }
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

    public FoodInfo GetFoodInfo(int forceId)
    {
        return forceId2FoodDict[forceId];
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

    public IEnumerator StartNLCoroutine(IEnumerator routine)
    {
        coroutineManager.StartCoroutine(routine);
        return routine;
    }
    public void StopNLCoroutine(IEnumerator routine)
    {
        coroutineManager.StopCoroutine(routine);
    }


}