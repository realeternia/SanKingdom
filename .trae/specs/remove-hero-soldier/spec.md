# Spec: 删除 SaveHeroData.soldier 属性，改为战斗现场调配

## 1. 背景与目标

当前设计中，`SaveHeroData.soldier` 表示英雄携带的兵力，兵力持久绑定在英雄身上。现需改为**每次进攻或防守时，兵力从城市预备兵池中现场调配**，英雄不再持久持有兵力。

同时，现场调配时需要选择兵种（ArmsConfig 的 id），而非使用英雄固定的 `armsId`。

**核心原则**：
- 兵力归属于城市（`SaveCityData.soldier`），英雄不再持有兵力
- 战斗时从城市兵池中按需调配给参战英雄
- 兵种（ArmsId）在调配时选择，不再硬绑定在英雄上
- Prefab/UI 层暂不改动，底层接口预留好即可

## 2. 当前数据流

```
SaveHeroData.soldier (持久)
    ↓
SaveCityData.GetBattleHeroList() → BattleCardData.SoldierNum = hero.soldier
    ↓
BattleManager.BattleBegin() → Chess.maxHp = SoldierNum
    ↓
战斗结束 → hero.soldier = chess.hp (回写英雄)
```

城市总兵力 = `city.soldier`（预备兵）+ Σ`hero.soldier`（英雄带兵）

## 3. 目标数据流

```
SaveCityData.soldier (城市兵池，唯一兵力来源)
    ↓
SaveCityData.GetBattleHeroList(heroSoldierDict, heroArmsDict) → BattleCardData
    ↓
BattleManager.BattleBegin() → Chess.maxHp = SoldierNum
    ↓
战斗结束 → city.soldier += 剩余兵力 (回写城市兵池)
```

城市总兵力 = `city.soldier`（唯一）

## 4. 变更范围

### 4.1 SaveHeroData.cs — 删除 soldier 属性

| 变更项 | 说明 |
|--------|------|
| 删除 `public int soldier;` | 英雄不再持久持有兵力 |
| 删除 `GetAttr("soldier")` 分支 | 英雄不再有 soldier 属性 |
| `CreateWildHero` 删除 `newHero.soldier = 100` | 在野英雄不再初始化兵力 |

### 4.2 SaveCityData.cs — 兵力调配核心逻辑

| 变更项 | 说明 |
|--------|------|
| `GetBattleHeroList` 签名变更 | 新增 `Dictionary<int, int> heroSoldierDict` 和 `Dictionary<int, int> heroArmsDict` 参数 |
| `GetBattleHeroList` 兵力来源变更 | `SoldierNum` 从 `heroSoldierDict` 获取，若无则使用默认分配逻辑 |
| `GetBattleHeroList` 兵种来源变更 | `ArmsId` 从 `heroArmsDict` 获取，若无则使用英雄默认 `armsId` |
| `GetAttr("soldier")` 简化 | 不再累加英雄兵力，直接返回 `city.soldier` |
| `AutoSetSoldierOnInit` 删除 | 不再需要给英雄分配兵力，全部保留在城市兵池 |
| 新增 `DistributeSoldierDefault` 方法 | 默认兵力分配逻辑：按英雄统率等属性从城市兵池分配 |

### 4.3 Player.cs — 战斗结束回写

| 变更项 | 说明 |
|--------|------|
| `OnBattleEnd` 兵力回写目标变更 | 剩余兵力回写到城市兵池而非英雄 |
| `ExecuteCityBattleDev` 传递调配参数 | 需要接收并传递 heroSoldierDict/heroArmsDict |

### 4.4 BattleManager.cs — 战斗单位死亡

| 变更项 | 说明 |
|--------|------|
| `OnUnitDying` 删除 `unit.soldier = 0` | 英雄不再有 soldier 属性 |

### 4.5 AI.cs — AI 兵力调配

| 变更项 | 说明 |
|--------|------|
| `DistributeSoldierToHeroes` 重构 | 改为生成 `heroSoldierDict` 而非直接修改 hero.soldier |
| `TryExecuteAttack` 使用调配字典 | 通过 heroSoldierDict 传递兵力而非修改英雄属性 |
| `HandleFoodPurchase` 兵力计算 | 使用 `city.GetAttr("soldier")` 替代 `Sum(h => h.soldier)` |
| `ExecuteHeroMove` 移除兵力引用 | 移动英雄不再涉及兵力 |

### 4.6 TaskPriorityCalculator.cs — 任务可用性判断

| 变更项 | 说明 |
|--------|------|
| `HasSoldier` 改用城市兵力 | 检查 `city.GetAttr("soldier") > 0` 而非 `hero.soldier > 0` |
| `AdjustPriorityByNeeds` 兵力计算 | 使用 `city.GetAttr("soldier")` 替代 `Sum(h => h.soldier)` |

### 4.7 SelectHeroArmyControl.cs — 英雄选择控件

| 变更项 | 说明 |
|--------|------|
| 显示兵力来源变更 | 不再显示 `heroData.soldier`，预留接口供后续 UI 改造 |

### 4.8 PopArmySetManager.cs — 配兵面板

| 变更项 | 说明 |
|--------|------|
| 预留接口 | Prefab 暂不改，底层接口留好。当前面板逻辑暂时保留但标记为待重构 |

## 5. 新增接口设计

### 5.1 GetBattleHeroList 新签名

```csharp
public List<BattleCardData> GetBattleHeroList(
    int[] filterHeroList = null,
    Dictionary<int, int> heroSoldierDict = null,
    Dictionary<int, int> heroArmsDict = null)
```

- `heroSoldierDict`: heroId → soldierCount，指定每个英雄的调配兵力
- `heroArmsDict`: heroId → armsId，指定每个英雄的兵种
- 若 `heroSoldierDict` 为 null，调用 `DistributeSoldierDefault` 自动分配
- 若 `heroArmsDict` 为 null，使用英雄默认 `armsId`

### 5.2 DistributeSoldierDefault 默认分配

```csharp
public Dictionary<int, int> DistributeSoldierDefault(int[] heroIds, int maxPerHero = 1000)
```

- 从城市兵池中按统率优先级分配兵力给指定英雄
- 返回 heroId → soldierCount 的字典
- 分配后从 `city.soldier` 中扣除

### 5.3 ExecuteCityBattleDev 新签名

```csharp
public void ExecuteCityBattleDev(
    int cityId, int devId, int[] heroList, int foodUse,
    int targetCityId, bool isAI,
    Dictionary<int, int> heroSoldierDict = null,
    Dictionary<int, int> heroArmsDict = null)
```

### 5.4 战斗结束回写逻辑

```csharp
// OnBattleEnd 中：
// 剩余兵力回写到源城市兵池
srcCity.soldier += soldierCount.Values.Sum();
```

## 6. 不变更项

| 项目 | 说明 |
|------|------|
| `SaveHeroData.armsId` | 保留，作为默认兵种，调配时可覆盖 |
| `BattleCardData` 结构 | 保持不变，SoldierNum/ArmsId 仍由调配逻辑填充 |
| `Chess` 及战斗逻辑 | 不变，Chess.hp 仍等于 SoldierNum |
| `ArmsConfig` | 不变 |
| Prefab/UI 面板 | 暂不改动，底层接口预留 |
