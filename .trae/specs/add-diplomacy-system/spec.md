# 外交系统 Spec

## Why
当前势力之间没有关系数值系统，无法表达势力间的外交亲疏，也无法基于关系分数驱动开战/和平等战略决策。需要增加外交关系管理器，在每回合自动演变关系分数，为后续外交决策提供数据基础。

## What Changes
- 新增 `SaveDatas/ForceRelation.cs`：外交关系管理器，包含 force×force 二维关系表、初始配置、存档数据、回合演变逻辑
- 新增 `ForceRelationEntry` 序列化数据类：存储单对势力关系
- 修改 `SaveDatas/SaveData.cs`：新增 `ForceRelation` 字段，在 `OnRound()` 中调用关系演变
- 修改 `Controls/GameManager.cs`：在 `NewGame()` 中初始化外交关系
- 修改 `SaveDatas/SaveForceData.cs`：在 `ExecuteBattle()` 中记录交战势力对
- 新增 `SystemConst.Diplomacy` 常量类：外交相关数值常量
- 新增 `SysFormula.Diplomacy` 公式类：外交关系计算公式
- 新增 `MapTool.AreForcesAdjacent()` 方法：判断两势力是否城市相邻
- 在 `Assembly-CSharp.csproj` 中添加新文件引用

## Impact
- Affected specs: 无已有 spec 受影响
- Affected code: SaveData.cs, GameManager.cs, SaveForceData.cs, SystemConst.cs, SysFormula.cs, MapTool.cs, Assembly-CSharp.csproj

## ADDED Requirements

### Requirement: 外交关系数据模型
系统 SHALL 提供 `ForceRelation` 类（位于 `SaveDatas/ForceRelation.cs`），包含：
- `List<ForceRelationEntry> relations`：可序列化的关系列表，存储所有势力对的关系分数
- `[NonSerialized]` 运行时数据：记录本回合交战的势力对
- 初始关系二维数组：NewGame 时使用的默认关系配置
- 关系分数范围 1-100，1 表示最恶劣（容易开战），100 表示关系最好

#### Scenario: 获取势力关系分数
- **WHEN** 调用 `GetRelation(forceId1, forceId2)`
- **THEN** 返回两势力间的关系分数（1-100），如果不存在则返回默认值

#### Scenario: 设置势力关系分数
- **WHEN** 调用 `SetRelation(forceId1, forceId2, score)`
- **THEN** 设置两势力间的关系分数，分数被 Clamp 到 1-100 范围

### Requirement: ForceRelationEntry 序列化数据类
系统 SHALL 提供 `ForceRelationEntry` 类（位于 `SaveDatas/ForceRelation.cs`），包含：
- `int forceId1`：势力1 ID
- `int forceId2`：势力2 ID
- `int score`：关系分数（1-100）

该类必须可被 JsonUtility 序列化。

### Requirement: 初始关系配置
系统 SHALL 在 `ForceRelation` 中提供初始关系二维数组，用于 NewGame 初始化：
- 基于三国历史背景设置各势力间的初始关系分数
- 同势力对自身不存储关系（forceId1 < forceId2 的组合唯一）
- 默认初始关系分数为 50（中性关系），特定势力对有差异化初始值

#### Scenario: 新游戏初始化外交关系
- **WHEN** 调用 `InitForNewGame()`
- **THEN** 根据初始关系二维数组填充 `relations` 列表，所有非自身势力对都有关系分数

### Requirement: 回合关系演变
系统 SHALL 在每回合（OnRound）自动演变势力间关系分数：
- 遍历所有势力对（两两判断）
- 如果两势力本回合处于和平状态（未交战）：关系分数下降 1-4 随机值；如果两势力城市相邻，则下降 1-2 随机值
- 如果两势力本回合交战过：关系分数上升 3-8 随机值
- 关系分数 Clamp 到 1-100 范围
- 回合结束后清空交战记录

#### Scenario: 和平状态且不相邻的势力对
- **WHEN** 两势力本回合未交战且城市不相邻
- **THEN** 关系分数下降 `SysRandom.Range(1, 5)`（即 1-4）

#### Scenario: 和平状态且相邻的势力对
- **WHEN** 两势力本回合未交战但城市相邻
- **THEN** 关系分数下降 `SysRandom.Range(1, 3)`（即 1-2）

#### Scenario: 交战过的势力对
- **WHEN** 两势力本回合交战过
- **THEN** 关系分数上升 `SysRandom.Range(3, 9)`（即 3-8）

### Requirement: 交战记录
系统 SHALL 在战斗发生时记录交战的势力对：
- 在 `SaveForceData.ExecuteBattle()` 中，战斗开始时调用 `ForceRelation.RecordBattle(srcForceId, destForceId)`
- 交战记录为 `[NonSerialized]` 运行时数据，不参与存档序列化
- 使用 HashSet 存储交战对，key 为 `min(forceId1, forceId2) * 100 + max(forceId1, forceId2)`

#### Scenario: 记录交战
- **WHEN** 势力 A 对势力 B 的城市发起战斗
- **THEN** 记录 (A, B) 为本回合交战对

### Requirement: 势力相邻判断
系统 SHALL 在 `MapTool` 中提供 `AreForcesAdjacent(forceId1, forceId2)` 方法：
- 遍历 forceId1 的所有城市，检查是否有城市与 forceId2 的城市相邻
- 使用已有的 `WorldConfig.WorldNearIds` 进行邻接判断

#### Scenario: 判断两势力是否相邻
- **WHEN** 调用 `AreForcesAdjacent(1, 2)`
- **THEN** 如果势力1有任何城市与势力2的城市相邻，返回 true；否则返回 false

### Requirement: 外交常量
系统 SHALL 在 `SystemConst` 中新增 `Diplomacy` 嵌套静态类，包含：
- `RELATION_MIN = 1`：最低关系分数
- `RELATION_MAX = 100`：最高关系分数
- `RELATION_DEFAULT = 50`：默认初始关系分数
- `PEACE_DECAY_MIN = 1`：和平关系下降最小值
- `PEACE_DECAY_MAX = 4`：和平关系下降最大值（不相邻）
- `PEACE_DECAY_ADJACENT_MIN = 1`：和平关系下降最小值（相邻）
- `PEACE_DECAY_ADJACENT_MAX = 2`：和平关系下降最大值（相邻）
- `BATTLE_RISE_MIN = 3`：交战关系上升最小值
- `BATTLE_RISE_MAX = 8`：交战关系上升最大值

### Requirement: 外交公式
系统 SHALL 在 `SysFormula` 中新增 `Diplomacy` 嵌套静态类，包含：
- `CalculatePeaceDecay(bool isAdjacent)`：计算和平关系下降值
- `CalculateBattleRise()`：计算交战关系上升值

### Requirement: SaveData 集成
系统 SHALL 修改 `SaveData` 类：
- 新增 `public ForceRelation forceRelation = new ForceRelation()` 字段
- 在 `OnRound()` 中调用 `forceRelation.OnRound()`

### Requirement: NewGame 集成
系统 SHALL 修改 `GameManager.NewGame()`：
- 在势力初始化后调用 `SaveData.forceRelation.InitForNewGame()`

### Requirement: 已消灭势力处理
系统 SHALL 在关系演变时跳过已消灭的势力：
- 在 `ForceRelation.OnRound()` 中，如果任一势力 `isEliminated` 为 true，则跳过该势力对的关系演变
