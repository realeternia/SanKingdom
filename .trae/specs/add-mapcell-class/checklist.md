# Checklist

- [x] SystemConst.Battle 中新增 GRID_MIN_GX/MAX_GX/MIN_GZ/MAX_GZ 四个边界常量，数值正确
- [x] MapCell.cs 已创建于 Combat 目录，为纯数据类（不继承 MonoBehaviour）
- [x] MapCell 字段 gridX/gridZ/chessId 完整，chessId=0 表示空格
- [x] MapCell.Occupy 在已占用时覆盖并记录 Warn 日志
- [x] BattleManager 中 gridOccupancy 字段已删除
- [x] BattleManager 新增 mapCells 二维数组，标记 [NonSerialized]
- [x] InitMapCells() 按边界常量创建全部 MapCell
- [x] GetMapCell(gx, gz) 越界返回 null 并记 Warn
- [x] BattleBegin 与 ReplayBattle 中调用 InitMapCells() 替换原 Clear
- [x] OccupyGrid/ReleaseGrid/UpdateGrid/IsGridOccupied/IsGridOccupiedByOther 改为基于 MapCell 实现
- [x] IsGridBlockedByObstacle 保持不变（仍查 chessList）
- [x] Assembly-CSharp.csproj 已新增 MapCell.cs 的 Compile Include
- [x] 全局 GetDiagnostics 无编译错误
- [x] ChessAI.cs 等调用方接口未变，无需修改
