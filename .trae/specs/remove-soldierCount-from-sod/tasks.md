# Tasks

- [x] Task 1: 从 SaveTroopsData 移除 soldierCount 字段
  - [x] SubTask 1.1: 移除 `public int soldierCount` 字段
  - [x] SubTask 1.2: 移除无参构造函数中 `soldierCount = 0`
  - [x] SubTask 1.3: 移除有参构造函数中 `soldierCount = 0`

- [x] Task 2: 修改 TroopsBuilder 返回兵力映射
  - [x] SubTask 2.1: `BuildAttackTroopsFromHeroList` 返回 `(List<SaveTroopsData>, Dictionary<int, int>)` 元组，不再设置 troop.soldierCount
  - [x] SubTask 2.2: `BuildDefenceTroops` 返回 `(List<SaveTroopsData>, Dictionary<int, int>)` 元组，不再设置 troop.soldierCount

- [x] Task 3: 修改 CityBattlePanelManager 使用 heroSoldierAllocations 缓存
  - [x] SubTask 3.1: `CreateCityBattleItems` 中初始化 heroSoldierAllocations 时，从城市士兵池分配默认兵力（而非读取 troop.soldierCount）
  - [x] SubTask 3.2: `OnBattle` 中构建 attackTroops 时，从 heroSoldierAllocations 构建 soldierMap，不再检查 troop.soldierCount > 0
  - [x] SubTask 3.3: `OnRun` 中将 soldierMap 传递给 ExecuteBattle

- [x] Task 4: 修改 CityBattleItem 使用缓存获取兵力
  - [x] SubTask 4.1: `OnEdit` 中使用 `CityBattlePanelManager.GetAllocatedSoldier()` 替代 `warTeamData.soldierCount`
  - [x] SubTask 4.2: `RefreshUI` 中使用 `CityBattlePanelManager.GetAllocatedSoldier(warTeamData.heroId1)` 替代 `warTeamData.soldierCount`

- [x] Task 5: 修改 BattleManager 接受 soldierMap 参数
  - [x] SubTask 5.1: `BattleBegin` 签名增加 `Dictionary<int, int> soldierMap1, Dictionary<int, int> soldierMap2`，存储为字段
  - [x] SubTask 5.2: `GetTotalSoldierCount` 改为接受 `Dictionary<int, int> soldierMap` 参数
  - [x] SubTask 5.3: `SpawnTroopForRegion` 签名增加 `int soldierCount` 参数，替代从 troop.soldierCount 读取
  - [x] SubTask 5.4: `InitSummon` 调用 `SpawnTroopForRegion` 时传入从 soldierMap 获取的 soldierCount

- [x] Task 6: 修改 SaveForceData 传递 soldierMap
  - [x] SubTask 6.1: `ExecuteBattle` 签名增加 `Dictionary<int, int> attackSoldierMap` 参数
  - [x] SubTask 6.2: `ExecuteBattle` 中从 attackSoldierMap 计算总兵力扣除，传递给 BattleBegin
  - [x] SubTask 6.3: `OnBattleEnd` 移除 `troop.soldierCount = 0` 赋值

- [x] Task 7: 修改 GameManager 和 AI 调用链适配
  - [x] SubTask 7.1: `GameManager` 中调用 `TroopsBuilder.BuildAttackTroopsFromHeroList` 适配新返回值
  - [x] SubTask 7.2: `GameManager` 中调用 `force.ExecuteBattle` 时传递 soldierMap
  - [x] SubTask 7.3: `CityBattlePanelManager.OnRun` 调用 `force.ExecuteBattle` 时传递 soldierMap

# Task Dependencies
- Task 1 独立，可先执行
- Task 2 依赖 Task 1（移除字段后才能修改 TroopsBuilder）
- Task 5 依赖 Task 1（移除字段后才能修改 BattleManager）
- Task 6 依赖 Task 5（需要 BattleBegin 新签名）
- Task 7 依赖 Task 2 和 Task 6（需要 TroopsBuilder 和 ExecuteBattle 新签名）
- Task 3 和 Task 4 可并行，依赖 Task 1
