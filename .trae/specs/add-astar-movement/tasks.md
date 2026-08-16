# Tasks

- [x] Task 1: 新建 AStarPathfinding 模块（`Assets/Resources/Scripts/Combat/AStarPathfinding.cs`）
  - [x] 静态类 `AStarPathfinding`，公开 `FindPath(start, isGoal, expand, heuristic, maxExpand)`，返回 `List<(int gx, int gz)>`（含起点与终点），无路径返回 `null`
  - [x] open set 用二叉堆（.NET Standard 2.1 无 PriorityQueue），closed/gScore 用 `Dictionary<(int,int), int>`
  - [x] 确定性：f 相同时按「g 更小 → gx 更小 → gz 更小」稳定平局；不使用 `UnityEngine.Random`/`BattleRandom`
  - [x] `maxExpand` 默认 512，超限终止并 `GameLog.Debug` 记录，返回 `null`
  - [x] 路径重建：从终点沿 parent 回溯到起点，反转
- [x] Task 2: 注册 csproj 编译项
  - [x] `Assembly-CSharp.csproj` 的 `<Compile Include>` 按字母序插入 `Assets\Resources\Scripts\Combat\AStarPathfinding.cs`
- [x] Task 3: ChessAI.GetMoveDest 接入 A*
  - [x] 目标集合：`RangeToCells(self.attackRange)` 内且 `bm.CanEnterCell(gx, gz, self)` 为真的格
  - [x] heuristic：`max(0, HexUtil.HexDistance(c, target) - rangeCells)`（可采纳）
  - [x] expand：六方向邻格 `n`——`CanEnterCell(n)` → cost=1 普通边；否则 `IsFriendlyGateCell(n)` 且 `GetCellBeyond(c, n)` 门后格可通行 → cost=2 跳边
  - [x] 取 `path[1]` 为落点（`GridCoordToWorld`），`null` 或 `Count < 2` 返回 `Vector3.zero`
  - [x] 删除原「邻格候选筛选 + 距离排序 + 跳门候选」贪心逻辑
- [x] Task 4: 验证（编译 + 静态检查 + 代码走查）
  - [x] 编译：`dotnet build Assembly-CSharp.csproj` 0 错误
  - [x] 静态检查：AStarPathfinding.cs 无 `UnityEngine.Random`/`BattleRandom`
  - [x] 代码走查：`GetMoveDest` 已无贪心候选逻辑；跳边仅友方城门生成；目标集合含射程内可通行格

# Task Dependencies

- [Task 2] depends on [Task 1]（需先有文件）
- [Task 3] depends on [Task 1]（需先有模块 API）
- [Task 2]、[Task 3] 相互独立，可并行
- [Task 4（验证）] depends on [Task 1][Task 2][Task 3]

# 验证（Task 4）

- [ ] 编译：`dotnet build Assembly-CSharp.csproj` 0 错误
- [ ] 静态检查：AStarPathfinding.cs 无 `UnityEngine.Random`/`BattleRandom`
- [ ] 代码走查：`GetMoveDest` 已无贪心候选逻辑；跳边仅友方城门生成；目标集合含射程内可通行格
