# A* 寻路模块 Spec

## Why

当前 `ChessAI.GetMoveDest` 采用贪心策略：从六方向邻格中挑一个「最接近目标」的格子走一步。单位会被墙/其他单位卡住时原地徘徊，且无法提前感知整条路径是否可行。需要一个 A* 寻路模块：在移动索敌时按最短可行路径前进，所有存活 chess（单位/城门/城墙/箭塔）均视为障碍。

## What Changes

- 新增静态类 `AStarPathfinding`（`Assets/Resources/Scripts/Combat/AStarPathfinding.cs`）：通用 A* 搜索，返回完整路径。
- `ChessAI.GetMoveDest` 接入 A*：以「目标棋子射程内的可通行格」为终点集合，取路径下一步作为本回合移动格。
- 障碍模型：所有存活 chess 不可通行（沿用 `BattleManager.CanEnterCell` 语义）；**保留友方城门跳边**（从门内相邻格直达门外相邻格，不落门格，走 `HexUtil.GetCellBeyond`）。
- `Assembly-CSharp.csproj` 注册新增 `.cs` 文件。
- 移动执行链路不变：仍由 `MoveAction`（force 移动）落到 A* 校验过的格子上。

## Impact

- Affected specs: 无（新建模块，不修改既有 spec 的接口）
- Affected code:
  - 新增：`Assets/Resources/Scripts/Combat/AStarPathfinding.cs`
  - 修改：`Assets/Resources/Scripts/Combat/ChessAI.cs`（`GetMoveDest` 重写）
  - 修改：`Assembly-CSharp.csproj`（新增 Compile Include）
  - 复用（不改动）：`BattleManager.CanEnterCell / IsFriendlyGateCell`、`HexUtil.GetCellBeyond / HexDistance / GetNeighbors`、`MoveAction`
- 约束：战斗层禁用 `UnityEngine.Random`（回放确定性），A* 必须纯确定性；日志用 `GameLog`。

## ADDED Requirements

### Requirement: A* 寻路模块

系统 SHALL 提供静态类 `AStarPathfinding`，包含通用 A* 搜索，返回从起点到目标集合的完整路径（`List<(int gx, int gz)>`，含起点与终点）。

#### Scenario: 基础寻路
- **GIVEN** 起点格子、目标判定函数 `isGoal`、邻格扩展函数 `expand`（返回 `(cell, cost)`）、启发函数 `heuristic`、扩展上限 `maxExpand`
- **WHEN** 调用 `FindPath(start, isGoal, expand, heuristic, maxExpand)`
- **THEN** 返回起点到某一目标格的路径列表；路径中相邻格之间均为 `expand` 生成的边；无可行路径返回 `null`

#### Scenario: 确定性
- **WHEN** 同一输入重复调用
- **THEN** 结果完全一致（不使用 `UnityEngine.Random`/`BattleRandom`；f 值相同时按「g 更小 → gx 更小 → gz 更小」稳定平局，保证回放一致性）

#### Scenario: 扩展上限保护
- **WHEN** 扩展节点数超过 `maxExpand`（默认 512）
- **THEN** 终止搜索并返回 `null`，用 `GameLog.Debug` 记录（战斗常态拥堵不视为错误）

#### Scenario: 成本模型
- **WHEN** `expand` 提供普通步 cost=1、跳边 cost=2
- **THEN** 启发函数保证可采纳（heuristic ≤ 任意路径真实成本），A* 返回最优路径

### Requirement: 寻路接入移动决策

`ChessAI.GetMoveDest` SHALL 使用 `AStarPathfinding` 计算下一步，替代原贪心邻格逻辑。

#### Scenario: 常规移动
- **GIVEN** 单位 `self` 与目标 `targetChess` 距离超出攻击射程
- **WHEN** 调用 `GetMoveDest(self)`
- **THEN** 目标集合 = 距目标 `RangeToCells(self.attackRange)` 内且 `CanEnterCell` 可通行的格；返回 A* 路径第 1 步（`path[1]`）的世界坐标；`path.Count < 2` 或 `null` 时返回 `Vector3.zero`（本回合不移动）

#### Scenario: 邻居扩展
- **WHEN** 对当前格 `c` 枚举六方向邻格 `n`
- **THEN** `CanEnterCell(n, self)` 为真 → 生成 cost=1 的普通边；否则若 `IsFriendlyGateCell(n, self)` 为真且 `GetCellBeyond(c, n)` 的目标格可通行 → 生成 cost=2 的跳边；其余邻格不生成边

### Requirement: 障碍模型

所有存活 chess（单位/城门/城墙/箭塔）SHALL 作为移动障碍，任何单位不可停留其上。

#### Scenario: 全阻挡
- **WHEN** 邻格被存活 chess 占据（含己方/敌方城门、城墙、箭塔）
- **THEN** 该格不可作为普通落点，A* 不会将其作为路径途经格

#### Scenario: 友方跳门保留
- **WHEN** 邻格为己方城门格（`IsFriendlyGateCell` 为真）且门后格可通行
- **THEN** 生成跳边，单位可从门内/外侧相邻格直达门后相邻格（不落门格），sally 出击可穿门而出
- **AND** 敌方不可跳门：敌方视角城门格仅为障碍

#### Scenario: 濒死棋子
- **WHEN** 格子上 chess 已死亡（`hp <= 0`）但尚未从 `chessList` 移除
- **THEN** 该格视为可通行（沿用 `CanEnterCell` 语义）

### Requirement: 编译注册

新增 `.cs` 文件 SHALL 在 `Assembly-CSharp.csproj` 的 `<Compile Include>` 中注册（按字母序插入）。

#### Scenario: 构建
- **WHEN** 执行 `dotnet build Assembly-CSharp.csproj`
- **THEN** 0 错误，寻路模块与 ChessAI 改动编译通过

## MODIFIED Requirements

### Requirement: 移动落点选择（原贪心逻辑）

原 `GetMoveDest` 行为：枚举六方向邻格，过滤占格/阻挡后，按「到目标六边形距离」排序，优先选能缩短距离的格，否则回退最近可行格（绕行）。

修改为：以目标射程内可通行格为终点集合执行 A*，取路径第一步作为落点；无可行路径则本回合不移动（不再原地绕行）。

## REMOVED Requirements

### Requirement: 贪心邻格候选逻辑

**Reason**: 被 A* 寻路取代。贪心策略只看一步，无法判断整条路径可行性，卡住时会在邻近格绕行或原地徘徊。

**Migration**: 移除 `GetMoveDest` 中基于 `HexUtil.GetNeighbors` 的候选筛选与距离排序代码，替换为 `AStarPathfinding.FindPath` 调用。
