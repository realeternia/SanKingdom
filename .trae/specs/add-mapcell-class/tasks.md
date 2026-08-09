# Tasks

- [x] Task 1: 在 SystemConst.Battle 新增地图边界常量
  - [x] 新增 `GRID_MIN_GX = 14`、`GRID_MAX_GX = 36`、`GRID_MIN_GZ = 8`、`GRID_MAX_GZ = 22`
  - [x] 确认覆盖现有 DEPLOY_SIDE 区域与城墙位置

- [x] Task 2: 创建 MapCell 类
  - [x] 新建 `Assets/Resources/Scripts/Combat/MapCell.cs`
  - [x] 字段：`gridX`、`gridZ`、`chessId`（0 表示空）
  - [x] 方法：`IsOccupied()`、`Occupy(int chessId)`、`Release()`
  - [x] Occupy 时若已占用，覆盖旧值并记录 Warn 日志
  - [x] 纯数据类，不继承 MonoBehaviour

- [x] Task 3: BattleManager 集成 MapCell 网格
  - [x] 删除 `gridOccupancy` 字段
  - [x] 新增 `[NonSerialized] MapCell[,] mapCells` 二维数组
  - [x] 新增 `InitMapCells()` 方法：按边界常量创建全部 MapCell
  - [x] 新增 `GetMapCell(int gx, int gz)` 方法：返回 cell 或 null（越界记 Warn）
  - [x] BattleBegin 中 `gridOccupancy.Clear()` 替换为 `InitMapCells()`
  - [x] ReplayBattle 中 `gridOccupancy.Clear()` 替换为 `InitMapCells()`

- [x] Task 4: 迁移占用方法到 MapCell 实现
  - [x] `OccupyGrid(chessId, worldPos)` → GetMapCell + Occupy
  - [x] `ReleaseGrid(chessId)` → 遍历 mapCells 查找 chessId 并 Release
  - [x] `UpdateGrid(chessId, newWorldPos)` → ReleaseGrid + OccupyGrid
  - [x] `IsGridOccupied(gx, gz)` → GetMapCell + IsOccupied
  - [x] `IsGridOccupiedByOther(gx, gz, excludeChessId)` → GetMapCell + chessId 比对
  - [x] `IsGridBlockedByObstacle` 保持不变（仍查 chessList）

- [x] Task 5: 更新 Assembly-CSharp.csproj
  - [x] 新增 `<Compile Include="Assets\Resources\Scripts\Combat\MapCell.cs" />`

- [x] Task 6: 验证编译与调用方
  - [x] 全局 GetDiagnostics 无错误
  - [x] 确认 ChessAI.cs 等调用方接口未变（仍通过 BattleManager 公开方法访问）

# Task Dependencies
- Task 3 依赖 Task 1（边界常量）和 Task 2（MapCell 类）
- Task 4 依赖 Task 3（mapCells 字段）
- Task 5 可与 Task 2 并行
- Task 6 依赖 Task 4 完成
