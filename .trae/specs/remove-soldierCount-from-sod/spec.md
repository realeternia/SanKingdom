# SaveTroopsData 移除 soldierCount Spec

## Why
`SaveTroopsData.soldierCount` 是一个临时值，不应作为持久化数据存储在存档类中。soldierCount 仅在战斗组建和战斗执行时需要，属于运行时计算/缓存数据，应从序列化数据类中移除，改为在需要时通过映射缓存或计算给出。

## What Changes
- **BREAKING**: 从 `SaveTroopsData` 移除 `soldierCount` 字段
- `BattleBegin` 方法签名增加 `Dictionary<int, int> soldierMap1, Dictionary<int, int> soldierMap2` 参数
- `SpawnTroopForRegion` 方法签名增加 `int soldierCount` 参数
- `TroopsBuilder` 返回 `Dictionary<int, int>` 映射（heroId → soldierCount），不再写入 `SaveTroopsData.soldierCount`
- `CityBattlePanelManager` 使用已有的 `heroSoldierAllocations` 静态字典作为缓存
- `CityBattleItem` 通过 `CityBattlePanelManager.GetAllocatedSoldier()` 获取兵力，不再读取 `warTeamData.soldierCount`
- `SaveForceData.ExecuteBattle()` 构建并传递 soldierMap
- `SaveForceData.OnBattleEnd()` 移除 `troop.soldierCount = 0` 赋值

## Impact
- Affected specs: 无
- Affected code:
  - `SaveDatas/SaveTroopsData.cs` — 移除字段和构造函数中的初始化
  - `Controls/BattleManager.cs` — BattleBegin、SpawnTroopForRegion、GetTotalSoldierCount 签名变更
  - `Controls/AI/TroopsBuilder.cs` — 返回值变更，不再设置 soldierCount
  - `Panels/CityBattlePanelManager.cs` — 使用 heroSoldierAllocations 替代 troop.soldierCount
  - `Panels/ListItem/CityBattleItem.cs` — 使用 GetAllocatedSoldier 替代 warTeamData.soldierCount
  - `SaveDatas/SaveForceData.cs` — ExecuteBattle 传递 soldierMap，OnBattleEnd 移除 soldierCount=0

## ADDED Requirements

### Requirement: soldierCount 从 SaveTroopsData 中移除
系统 SHALL 从 `SaveTroopsData` 中移除 `soldierCount` 字段，因为它不属于持久化数据。

#### Scenario: 存档序列化兼容
- **WHEN** 加载旧存档（包含 soldierCount 字段）
- **THEN** JsonUtility 自动忽略多余字段，不影响反序列化

### Requirement: BattleBegin 接受兵力映射参数
系统 SHALL 在 `BattleBegin` 方法中接受 `Dictionary<int, int> soldierMap1, Dictionary<int, int> soldierMap2` 参数，用于传递每支队伍的兵力数据。

#### Scenario: 战斗开始时传入兵力
- **WHEN** 调用 `BattleBegin` 发起战斗
- **THEN** 通过 soldierMap 参数获取每个 heroId 对应的 soldierCount，用于初始化 BattlePlayerINfo 和 SpawnTroopForRegion

### Requirement: SpawnTroopForRegion 接受 soldierCount 参数
系统 SHALL 在 `SpawnTroopForRegion` 方法中接受 `int soldierCount` 参数，替代从 `SaveTroopsData.soldierCount` 读取。

#### Scenario: 生成部队棋子
- **WHEN** 调用 `SpawnTroopForRegion` 生成部队
- **THEN** 使用传入的 soldierCount 参数创建 CreateChessAction

### Requirement: TroopsBuilder 返回兵力映射
系统 SHALL 让 `TroopsBuilder.BuildAttackTroopsFromHeroList` 和 `BuildDefenceTroops` 返回兵力映射 `Dictionary<int, int>`（heroId → soldierCount），不再将 soldierCount 写入 SaveTroopsData。

#### Scenario: AI 构建攻击部队
- **WHEN** AI 调用 BuildAttackTroopsFromHeroList
- **THEN** 返回 (List<SaveTroopsData>, Dictionary<int, int>) 元组，soldierCount 通过映射获取

#### Scenario: 构建防守部队
- **WHEN** 调用 BuildDefenceTroops
- **THEN** 返回 (List<SaveTroopsData>, Dictionary<int, int>) 元组，soldierCount 通过映射获取

### Requirement: CityBattlePanelManager 使用 heroSoldierAllocations 缓存
系统 SHALL 在 `CityBattlePanelManager` 中使用已有的 `heroSoldierAllocations` 静态字典作为兵力缓存，不再依赖 `SaveTroopsData.soldierCount`。

#### Scenario: 初始化部队列表
- **WHEN** 创建 CityBattleItem 列表
- **THEN** 从城市士兵池分配默认兵力到 heroSoldierAllocations

#### Scenario: 出战校验
- **WHEN** 点击出战按钮
- **THEN** 从 heroSoldierAllocations 获取兵力，构建 soldierMap 传递给战斗

### Requirement: CityBattleItem 通过缓存获取兵力
系统 SHALL 让 `CityBattleItem` 通过 `CityBattlePanelManager.GetAllocatedSoldier()` 获取和设置兵力，不再直接读写 `warTeamData.soldierCount`。

#### Scenario: 编辑兵力
- **WHEN** 用户编辑部队兵力
- **THEN** 通过 SetAllocatedSoldier 更新缓存，RefreshUI 从缓存读取显示

### Requirement: SaveForceData 传递 soldierMap
系统 SHALL 在 `SaveForceData.ExecuteBattle()` 中构建 soldierMap 并传递给 `BattleBegin`，在 `OnBattleEnd()` 中移除 `troop.soldierCount = 0` 赋值。

#### Scenario: 执行战斗
- **WHEN** 调用 ExecuteBattle
- **THEN** 从 attackTroops 对应的 heroSoldierAllocations 或 TroopsBuilder 返回值构建 soldierMap，传入 BattleBegin

#### Scenario: 战斗结束
- **WHEN** 战斗结束退回士兵
- **THEN** 不再设置 troop.soldierCount = 0，因为 soldierCount 已不存在

## MODIFIED Requirements

### Requirement: GetTotalSoldierCount 改为从 soldierMap 计算
`BattleManager.GetTotalSoldierCount` 改为接受 `Dictionary<int, int> soldierMap` 参数，从映射中汇总兵力而非遍历 SaveTroopsData.soldierCount。

## REMOVED Requirements

### Requirement: SaveTroopsData.soldierCount 字段
**Reason**: soldierCount 是临时运行时数据，不应持久化在存档类中
**Migration**: 旧存档中的 soldierCount 字段会被 JsonUtility 自动忽略，无需迁移
