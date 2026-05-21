# Tasks

- [x] Task 1: 新增 SystemConst.Diplomacy 常量类和 SysFormula.Diplomacy 公式类
  - [x] SubTask 1.1: 在 SystemConst.cs 中新增 Diplomacy 嵌套静态类，包含 RELATION_MIN/MAX/DEFAULT、PEACE_DECAY_MIN/MAX、PEACE_DECAY_ADJACENT_MIN/MAX、BATTLE_RISE_MIN/MAX 常量
  - [x] SubTask 1.2: 在 SysFormula.cs 中新增 Diplomacy 嵌套静态类，包含 CalculatePeaceDecay(bool isAdjacent) 和 CalculateBattleRise() 方法，使用 SysRandom

- [x] Task 2: 在 MapTool 中新增 AreForcesAdjacent(forceId1, forceId2) 方法
  - [x] SubTask 2.1: 实现 AreForcesAdjacent：遍历 forceId1 的所有城市，检查是否有城市邻接 forceId2 的城市

- [x] Task 3: 新增 SaveDatas/ForceRelation.cs（ForceRelationEntry + ForceRelation 类）
  - [x] SubTask 3.1: 创建 ForceRelationEntry 序列化数据类（forceId1, forceId2, score）
  - [x] SubTask 3.2: 创建 ForceRelation 类：relations 列表、[NonSerialized] foughtPairs HashSet、初始关系二维数组
  - [x] SubTask 3.3: 实现 InitForNewGame()：根据初始二维数组填充 relations
  - [x] SubTask 3.4: 实现 GetRelation(forceId1, forceId2)、SetRelation(forceId1, forceId2, score)、AddRelation(forceId1, forceId2, delta)
  - [x] SubTask 3.5: 实现 RecordBattle(forceId1, forceId2)：记录本回合交战对
  - [x] SubTask 3.6: 实现 OnRound()：遍历势力对，根据和平/交战状态演变关系分数，跳过已消灭势力，结束后清空交战记录

- [x] Task 4: 修改 SaveData.cs 集成外交关系
  - [x] SubTask 4.1: 新增 public ForceRelation forceRelation = new ForceRelation() 字段
  - [x] SubTask 4.2: 在 OnRound() 中调用 forceRelation.OnRound()

- [x] Task 5: 修改 GameManager.cs 在 NewGame 中初始化外交关系
  - [x] SubTask 5.1: 在 NewGame() 势力初始化后调用 SaveData.forceRelation.InitForNewGame()

- [x] Task 6: 修改 SaveForceData.cs 在战斗时记录交战
  - [x] SubTask 6.1: 在 ExecuteBattle() 中调用 GameManager.Instance.SaveData.forceRelation.RecordBattle(srcForceId, destForceId)

- [x] Task 7: 在 Assembly-CSharp.csproj 中添加 ForceRelation.cs 的 Compile Include

# Task Dependencies
- [Task 3] depends on [Task 1] (ForceRelation.OnRound 使用 SysFormula.Diplomacy 和 SystemConst.Diplomacy)
- [Task 3] depends on [Task 2] (ForceRelation.OnRound 使用 MapTool.AreForcesAdjacent)
- [Task 4] depends on [Task 3] (SaveData 需要 ForceRelation 类)
- [Task 5] depends on [Task 3] (NewGame 需要 ForceRelation.InitForNewGame)
- [Task 6] depends on [Task 3] (RecordBattle 需要 ForceRelation 类)
- [Task 1] 和 [Task 2] 可并行执行
