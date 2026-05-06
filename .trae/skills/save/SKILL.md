---
name: "save"
description: "存档序列化规则，包含JsonUtility序列化规范、数据类目录规范、存档系统架构。Invoke when working on save data, serialization, SaveData classes, or data persistence."
---

# 存档序列化规则

## 序列化规则

- 使用 `[Serializable]` 标记需要序列化的类
- 使用 `[NonSerialized]` 标记运行时引用（如 `viewObj`, `owner` 等Unity对象引用和回调）
- 使用 `[SerializeReference]` 标记多态序列化字段（如 `List<ChessAction> actions`）
- 使用 Unity `JsonUtility` 进行序列化/反序列化
- 所有存档数据类（`SaveData`, `SaveCityData`, `SaveHeroData` 等）必须可被 JsonUtility 序列化

## [NonSerialized] 使用模式

1. **Dictionary 类型**：JsonUtility 不支持 Dictionary，必须标记 `[NonSerialized]`（如 `SaveCityData.actions`、`SaveForceData.posResCache`）
2. **运行时状态**：回合阶段、计划确认等临时状态（如 `SaveForceData.phase`、`SaveForceData.planConfirmed`）
3. **运行时引用**：Unity 对象引用、回调、静态实例等（如 `BattleStatManager.currentInstance`、`Chess.viewObj`）
4. **战斗回放数据**：`SaveForceData.warPlans` 标记为 `[NonSerialized]`，因为战斗计划是每回合临时生成的

## [SerializeReference] 使用模式

项目中仅有一处使用 `[SerializeReference]`：

```csharp
// BattleManager.cs
[SerializeReference]
List<ChessAction> actions  // 多态序列化 ChessAction 子类
```

这是为了支持 `ChessAction` 的多态序列化（不同子类需保持类型信息）。

## Dictionary 替代方案

由于 JsonUtility 不支持 `Dictionary<K,V>` 序列化，项目采用两种替代方案：
- **List 替代**：`SaveCityData.devAssignments` 用 `List<DevAssignmentData>` 存储映射，通过 `GetDevIdByHeroId()` 查询
- **[NonSerialized] Dictionary**：`SaveCityData.actions` 等不需要持久化的映射直接用 Dictionary 并标记不序列化

## 数据类目录规范

- **持久化数据结构**（需要序列化保存的数据类）必须定义在 `SaveDatas/` 目录下
  - 例如：`SaveData`, `SaveForceData`, `SaveCityData`, `SaveHeroData`, `WarTeamData`, `WarPlanData`, `DevAssignmentData`
- **运行时数据结构**（不需要持久化的数据类）定义在 `PO/` 目录下
  - 例如：`SignalData`, `AttrInfo`, `BattleCardData`, `ArmsType`, `TurnPhase` 等

## 存档数据类结构

### SaveData — 顶层存档

```csharp
[Serializable]
public class SaveData
{
    public List<SaveForceData> forces;
    public List<SaveCityData> cities;
    public List<SaveHeroData> heros;
    public BattleStatManager battleStatManager;
    public int round;
    public int currentForceIndex;
}
```

### SaveForceData — 势力数据

```csharp
[Serializable]
public class SaveForceData
{
    // 持久化字段
    public int forceId;
    public bool isPlayer;
    public bool isEliminated;
    public float gold;

    // 非序列化字段（运行时状态）
    [NonSerialized] public TurnPhase phase = TurnPhase.None;
    [NonSerialized] public List<WarPlanData> warPlans;
    [NonSerialized] public bool planConfirmed = false;
    [NonSerialized] private Dictionary<string, float> posResCache;
    [NonSerialized] private Dictionary<string, int> resUsedCache;
}
```

加载后通过 `InitRuntimeState()` 重新初始化 `[NonSerialized]` 字段。

### SaveCityData — 城市数据

```csharp
[Serializable]
public class SaveCityData
{
    // 持久化字段
    public int cityId, forceId, level, exp, ownerHeroId;
    public float soldier, happy, food, wall;
    public List<DevAssignmentData> devAssignments;

    // 非序列化字段
    [NonSerialized] public Dictionary<int, int> actions;
}
```

`devAssignments` 通过 `SetDevAssignment()` / `RemoveDevAssignment()` / `ClearDevAssignments()` 管理，修改后触发 `RecalculatePosRes()`。

### SaveHeroData — 英雄数据

```csharp
public enum HeroState { Normal, Wild, Catched }

[Serializable]
public class SaveHeroData
{
    public int heroId, exp, cityId, forceId, armsId;
    public HeroState state;
    public int loyalty;
    public int str, inte, fair, charm, leadShip;  // 五维属性
}
```

`InitAttrsFromConfig()` 在属性为零值时从配置表回填，实现"存档优先、配置兜底"的兼容策略。

### WarTeamData — 战斗队伍

```csharp
[Serializable]
public class WarTeamData
{
    public int heroId1, heroId2, heroId3;  // 固定3个英雄槽位
    public int armsId;
    public int targetCityId;
}
```

### WarPlanData — 战斗计划

```csharp
public class WarPlanData
{
    public List<WarTeamData> teams;
}
```

注意：`WarPlanData` 本身**没有** `[Serializable]` 标记，被 `SaveForceData.warPlans`（`[NonSerialized]`）引用，是纯运行时数据。

### DevAssignmentData — 发展指派

```csharp
[Serializable]
public class DevAssignmentData
{
    public int heroId;
    public int devId;
}
```

### BattleStatManager — 战斗统计

虽然定义在 `Combat/` 目录下，但作为 `SaveData` 的字段被一起序列化。包含 `BattleRecord` 和 `BattleStat` 内部类。

## 双套序列化体系

### 1. 战略层存档（GameManager 管理）

| 操作 | 方法 |
|------|------|
| 保存 | `GameManager.SaveToFile()` |
| 加载 | `GameManager.LoadFromSave()` |
| 检测 | `GameManager.IsGameSaveExist()` |

```csharp
// 保存
string json = JsonUtility.ToJson(SaveData);
File.WriteAllText(savePath, json);

// 加载
SaveData saveData = JsonUtility.FromJson<SaveData>(json);
SaveData = saveData;
foreach (var forceData in SaveData.forces)
    forceData.InitRuntimeState();  // 重新初始化运行时状态
```

- 存储路径：`Application.persistentDataPath + "/game_save.json"`
- 使用 `JsonUtility.ToJson()` / `JsonUtility.FromJson<T>()` 全量序列化/反序列化
- 加载后必须调用 `InitRuntimeState()` 恢复 `[NonSerialized]` 字段

### 2. 战斗回放存档（BattleManager 管理）

| 操作 | 方法 |
|------|------|
| 保存 | `BattleManager.SaveToFile(filePath)` |
| 加载 | `BattleManager.LoadFromFile(filePath)` |

```csharp
// 保存
string json = JsonUtility.ToJson(this);
File.WriteAllText(filePath, json);

// 加载（覆盖式）
JsonUtility.FromJsonOverwrite(json, this);
foreach (var chess in chessList) chess.OnRecover();
foreach (var missile in missileList) missile.OnRecover();
```

- 文件名格式：`battlereplayer{battleId}.json.json`
- 使用 `JsonUtility.FromJsonOverwrite()` 覆盖式反序列化（不创建新实例，直接覆盖现有对象字段）
- 加载后通过 `OnRecover()` 接口恢复运行时引用

## 存档兼容性

- JsonUtility 反序列化时，JSON 中缺失的字段会自动填充零值（int=0, bool=false, null 引用等）
- `SaveHeroData.InitAttrsFromConfig()` 利用此特性：属性为零值时从配置表回填，保证旧存档兼容
- 新增 `SaveData` 字段时需考虑旧存档的零值兼容问题

## 存档触发时机

- `GameManager.NewGame()` 结束时保存
- `GameManager.ConfirmPlan()` 确认计划后保存
- `SaveCityData.Occupy()` 城市被占领后保存

## 存档数据修改注意事项

- 新增 `SaveData` 字段时，需考虑旧存档兼容（JsonUtility 反序列化时缺失字段为零值）
- `SaveCityData.actions` 使用 `[NonSerialized]`，不会持久化
- `DevAssignmentData` 使用列表存储，通过 `SetDevAssignment` / `RemoveDevAssignment` 管理
- 新增需要持久化的映射关系时，使用 `List<数据类>` 而非 Dictionary

## 禁止事项

- 不要在 `SaveData` 及其子类中存储 Unity 对象引用
- 不要将持久化数据类定义在 `PO/` 目录下，必须放在 `SaveDatas/` 目录下
- 不要在存档数据类中使用 Dictionary（JsonUtility 不支持），改用 List 或标记 `[NonSerialized]`
