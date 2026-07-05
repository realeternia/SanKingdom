# 战斗障碍物系统实现计划

## Context

战斗系统目前没有障碍物/地形阻挡机制。棋子移动仅检查 `gridOccupancy`（其他棋子占用），无静态障碍概念。用户需要在战斗中支持创建障碍对象（wall/gate），障碍位于两个相邻格子之间并阻挡移动穿过。初始化时需在防御方第一排前方自动创建一排 wall。

## 实现步骤

### 1. 新建 `Combat/BattleObstacle.cs` — 障碍物数据类

```csharp
public enum ObstacleType { Wall, Gate }

[System.Serializable]
public class BattleObstacle
{
    public ObstacleType type;
    public int gx1, gz1; // 相邻格子1的网格坐标
    public int gx2, gz2; // 相邻格子2的网格坐标
}
```

### 2. 修改 `BattleManager.cs`

- 新增字段：
  - `public List<BattleObstacle> obstacles = new List<BattleObstacle>();` — 可序列化，用于回放
  - `[NonSerialized] private List<GameObject> obstacleViews = new List<GameObject>();` — 视觉对象

- 新增方法：
  - `CreateObstacle(ObstacleType type, int gx1, int gz1, int gx2, int gz2)` — 核心接口
    - 校验两格子相邻（|dx|+|dz|==1）
    - 添加到 `obstacles` 列表
    - 若 `showUI`，加载对应 prefab 实例化到场景，定位在两格子中点，挂载到 `mapObj` 下
    - prefab 路径：Wall → `ResPath.Prefab.BattleItem("Wall_B_wall")`，Gate → `ResPath.Prefab.BattleItem("Wall_B_gate")`
  - `IsEdgeBlocked(int gx1, int gz1, int gx2, int gz2)` — 检查两格子之间是否有障碍
    - 遍历 obstacles，匹配 (gx1,gz1)→(gx2,gz2) 或 (gx2,gz2)→(gx1,gz1)
  - `InitDefenderWalls()` — 在防御方第一排前方创建一排 wall
    - 防御方 side=2，第一排 row=0，grid gx=29
    - 前一排 gx=28
    - gz 范围 13~17（对应 col 0~4）
    - 对每个 gz 调用 `CreateObstacle(ObstacleType.Wall, 28, gz, 29, gz)`

- 修改 `InitSummon()`：在防御方棋子生成之后、攻击方棋子生成之前，调用 `InitDefenderWalls()`

- 修改 `BattleBegin()`：在 `chessList.Clear()` 附近添加 `obstacles.Clear()`

- 修改 `LoadFromFile()`：反序列化后遍历 `obstacles` 重建视觉对象

- 修改战斗结束清理：销毁 `obstacleViews` 中的 GameObject

### 3. 修改 `Chess.cs` — `GetMoveDest()`

在每个候选移动位置检查中，增加边阻挡判断：

```csharp
// 原来只检查：
if (!bm.IsGridOccupiedByOther(gx, gz, id))
// 改为：
if (!bm.IsGridOccupiedByOther(gx, gz, id) && !bm.IsEdgeBlocked(curGx, curGz, gx, gz))
```

同样修改侧向偏移的候选位置检查。

### 4. 修改 `ResPath.cs`

在 `Prefab` 类中新增：

```csharp
public static string BattleItem(string itemName)
{
    return "Prefabs/BattleItems/" + itemName;
}
```

### 5. 修改 `SystemConst.cs`

在 `Battle` 嵌套类中新增常量（如需要）：

```csharp
public const int DEFENDER_WALL_GX_FRONT = 28; // 防御方城墙前方格子X
public const int DEFENDER_WALL_GX_BACK = 29;  // 防御方城墙后方格子X（第一排）
```

### 6. 修改 `Assembly-CSharp.csproj`

添加 `<Compile Include="Assets\Resources\Scripts\Combat\BattleObstacle.cs" />`

## 关键文件

| 文件 | 操作 |
|------|------|
| `Assets/Resources/Scripts/Combat/BattleObstacle.cs` | 新建 |
| `Assets/Resources/Scripts/Controls/BattleManager.cs` | 修改 |
| `Assets/Resources/Scripts/Combat/Chess.cs` | 修改 |
| `Assets/Resources/Scripts/SystemTool/ResPath.cs` | 修改 |
| `Assets/Resources/Scripts/SystemTool/SystemConst.cs` | 修改 |
| `Assembly-CSharp.csproj` | 修改 |

## 验证方式

1. 编译通过，无报错
2. 战斗初始化后，在防御方第一排（gx=29）前方（gx=28）之间生成 5 个 wall 障碍
3. 攻击方棋子无法穿过 wall 边界移动到防御方第一排
4. 防御方棋子也无法穿过 wall 向前移动（双向阻挡）
5. 回放正常，障碍物数据正确序列化/反序列化
