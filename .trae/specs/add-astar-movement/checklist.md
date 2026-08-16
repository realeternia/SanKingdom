# Checklist

- [x] 新增 `AStarPathfinding.cs`，`FindPath` 签名与 spec 一致（start/isGoal/expand/heuristic/maxExpand，返回完整路径，无路径返回 null）
- [x] 二叉堆 open set + gScore 字典实现正确；路径回溯反转正确
- [x] 寻路确定性：无 `UnityEngine.Random`/`BattleRandom`，平局按「g → gx → gz」
- [x] `maxExpand`（默认 512）超限终止、`GameLog.Debug` 记录、返回 null
- [x] `Assembly-CSharp.csproj` 已注册 `AStarPathfinding.cs`
- [x] `ChessAI.GetMoveDest` 使用 A*：目标集合=射程内可通行格；取 `path[1]`；`null`/`Count<2` 返回 `Vector3.zero`
- [x] 普通边 cost=1、友方城门跳边 cost=2；heuristic = `max(0, dist - range)` 可采纳
- [x] 障碍模型：所有存活 chess（单位/城门/城墙/箭塔）不可通行；友方跳门保留、敌方不可跳门
- [x] 原贪心邻格候选+距离排序逻辑已删除
- [x] `dotnet build Assembly-CSharp.csproj` 0 错误
