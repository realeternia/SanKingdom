# MapCell 类 Spec

## Why
战斗层的格子占用关系（gridOccupancy）目前以扁平的 `List<(int gridX, int gridZ, int chessId)>` 形式散落在 BattleManager 中，缺乏对"一个格子"概念的封装。引入 MapCell 类可将"格子"作为一等公民，明确"一格最多一棋子"的约束，并在地图初始化时确定战场区域，提升可读性与可维护性。

## What Changes
- 新增 `MapCell` 纯数据类（无 view，不继承 MonoBehaviour），封装格子坐标与占用棋子 ID
- 在 `SystemConst.Battle` 新增地图边界常量（GRID_MIN/MAX_GX/GZ），明确战场矩形区域
- `BattleManager` 用 `MapCell[,]` 二维数组替代 `gridOccupancy` 列表，地图初始化时按边界常量创建全部 MapCell
- 迁移 OccupyGrid/ReleaseGrid/UpdateGrid/IsGridOccupied/IsGridOccupiedByOther 为基于 MapCell 的实现
- `IsGridBlockedByObstacle` 保持不变（仍查 chessList，因墙/城门/箭塔的友敌方判定需访问 Chess 属性）
- MapCell 集合标记 `[NonSerialized]`，与原 gridOccupancy 一致；BattleBegin 与 ReplayBattle 中初始化，回放反序列化后由 InitLoadedData 重建

## Impact
- Affected specs: 无
- Affected code:
  - `SystemTool/SystemConst.cs`（新增地图边界常量）
  - `Combat/MapCell.cs`（新增类）
  - `Controls/BattleManager.cs`（替换 gridOccupancy，迁移方法）
  - `Combat/ChessAI.cs`（调用方不变，仍通过 BattleManager 公开方法访问）
  - `Assembly-CSharp.csproj`（新增 Compile Include）

## ADDED Requirements

### Requirement: MapCell 格子封装
系统 SHALL 提供 `MapCell` 类，封装战斗地图中一个格子的坐标与占用关系。
- 字段：`gridX`、`gridZ`（格子坐标）、`chessId`（占用该格的棋子 ID，0 表示空）
- 方法：`IsOccupied()`、`Occupy(int chessId)`、`Release()`
- 一个 MapCell 同一时刻最多容纳一个 chess（Occupy 时若已占用，覆盖旧值并记录 Warn 日志）
- 无 view，纯数据类，不继承 MonoBehaviour

#### Scenario: 棋子占用空格子
- WHEN 调用 `cell.Occupy(chessId)` 且 cell 当前为空
- THEN `cell.chessId` 设为 chessId，`cell.IsOccupied()` 返回 true

#### Scenario: 棋子离开格子
- WHEN 调用 `cell.Release()`
- THEN `cell.chessId` 重置为 0，`cell.IsOccupied()` 返回 false

### Requirement: 地图边界常量
系统 SHALL 在 `SystemConst.Battle` 中定义战场矩形区域边界常量：
- `GRID_MIN_GX = 14`、`GRID_MAX_GX = 36`
- `GRID_MIN_GZ = 8`、`GRID_MAX_GZ = 22`
- 覆盖现有布阵区域（攻方 X=20-22、防守方 X=29-31、城墙 X=26、GZ 13-17）并留余量

### Requirement: 地图初始化创建全部 MapCell
系统 SHALL 在 BattleBegin 与 ReplayBattle 时，按地图边界常量创建全部 MapCell（二维数组），此后地图区域固定不变。

#### Scenario: 战斗开始初始化地图
- WHEN BattleBegin 执行
- THEN 创建 `(GRID_MAX_GX - GRID_MIN_GX + 1) × (GRID_MAX_GZ - GRID_MIN_GZ + 1)` 个 MapCell，所有 cell 初始为空

#### Scenario: 越界坐标查询
- WHEN 以超出地图边界的 (gx, gz) 查询 MapCell
- THEN 返回 null 并记录 Warn 日志

## MODIFIED Requirements

### Requirement: 格子占用查询
原 `gridOccupancy` 列表的占用查询逻辑 SHALL 改为通过 MapCell 实现：
- `OccupyGrid(chessId, worldPos)` → 转换坐标后调用对应 MapCell.Occupy
- `ReleaseGrid(chessId)` → 遍历查找 chessId 所在 cell 并 Release（或记录 cell 引用加速）
- `UpdateGrid(chessId, newWorldPos)` → Release 旧格子 + Occupy 新格子
- `IsGridOccupied(gx, gz)` → MapCell != null && MapCell.IsOccupied()
- `IsGridOccupiedByOther(gx, gz, excludeChessId)` → MapCell.IsOccupied() && chessId != excludeChessId

## REMOVED Requirements

### Requirement: gridOccupancy 列表
**Reason**: 占用关系迁移至 MapCell 二维数组
**Migration**: 删除 `gridOccupancy` 字段及所有直接引用，由 MapCell 集合替代
