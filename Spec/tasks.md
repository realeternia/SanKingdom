# Tasks: 删除 SaveHeroData.soldier 属性，改为战斗现场调配

## Task 1: 修改 SaveHeroData.cs — 删除 soldier 属性

**文件**: `Assets/Resources/Scripts/SaveDatas/SaveHeroData.cs`

**操作**:
1. 删除 `public int soldier;` 字段（第15行）
2. 删除 `GetAttr` 方法中的 `case "soldier": return soldier;` 分支（第40-41行）
3. `CreateWildHero` 方法中删除 `newHero.soldier = 100;`（第56行）

**依赖**: 无

---

## Task 2: 修改 SaveCityData.cs — 兵力调配核心逻辑

**文件**: `Assets/Resources/Scripts/SaveDatas/SaveCityData.cs`

**操作**:
1. `GetBattleHeroList` 方法签名变更，新增 `heroSoldierDict` 和 `heroArmsDict` 参数
2. `GetBattleHeroList` 内部逻辑：SoldierNum 从 heroSoldierDict 获取，若无则调用默认分配；ArmsId 从 heroArmsDict 获取，若无则用 hero.armsId
3. `GetAttr("soldier")` 简化：直接返回 `(int)Math.Floor(soldier)`，不再累加英雄兵力
4. 删除 `AutoSetSoldierOnInit` 方法整体（第409-486行）
5. 新增 `DistributeSoldierDefault` 方法：按统率优先级从城市兵池分配兵力，返回 `Dictionary<int, int>`

**依赖**: Task 1

---

## Task 3: 修改 Player.cs — 战斗结束回写与调配参数传递

**文件**: `Assets/Resources/Scripts/Controls/Player.cs`

**操作**:
1. `ExecuteCityBattleDev` 方法新增 `heroSoldierDict` 和 `heroArmsDict` 参数，传递给 `GetBattleHeroList`
2. `OnBattleEnd` 方法：剩余兵力回写到源城市兵池（`srcCity.soldier += ...`），而非写回英雄
3. 删除 `OnBattleEnd` 中的 `GameManager.Instance.GetHero(item.Key).soldier = item.Value;`

**依赖**: Task 2

---

## Task 4: 修改 BattleManager.cs — 删除英雄 soldier 引用

**文件**: `Assets/Resources/Scripts/Controls/BattleManager.cs`

**操作**:
1. `OnUnitDying` 方法中删除 `unit.soldier = 0;`（第527-528行）

**依赖**: Task 1

---

## Task 5: 修改 AI.cs — AI 兵力调配重构

**文件**: `Assets/Resources/Scripts/Controls/AI/AI.cs`

**操作**:
1. `DistributeSoldierToHeroes` 重构：返回 `Dictionary<int, int>` 而非直接修改 hero.soldier，从城市兵池分配
2. `TryExecuteAttack` 方法：
   - 使用 `DistributeSoldierToHeroes` 返回的字典
   - `totalSoldier` 计算改用字典值求和
   - 调用 `ExecuteCityBattleDev` 时传入 heroSoldierDict
3. `HandleFoodPurchase` 方法：`totalSoldier` 改用 `city.GetAttr("soldier")`
4. `ExecuteHeroMove` 方法：移除 `hero.soldier` 引用，粮草消耗改用城市兵力计算

**依赖**: Task 2, Task 3

---

## Task 6: 修改 TaskPriorityCalculator.cs — 任务可用性判断

**文件**: `Assets/Resources/Scripts/Controls/AI/TaskPriorityCalculator.cs`

**操作**:
1. `HasSoldier` 方法：改用 `city.GetAttr("soldier") > 0` 替代遍历英雄
2. `AdjustPriorityByNeeds` 方法：`totalSoldier` 改用 `city.GetAttr("soldier")`

**依赖**: Task 1

---

## Task 7: 修改 SelectHeroArmyControl.cs — 英雄选择控件

**文件**: `Assets/Resources/Scripts/SelectHeroArmyControl.cs`

**操作**:
1. 第40行 `heroData.soldier` 改为显示城市兵力或预留占位（Prefab 暂不改，底层接口留好）

**依赖**: Task 1

---

## Task 8: 修改 PopArmySetManager.cs — 配兵面板接口预留

**文件**: `Assets/Resources/Scripts/PopArmySetManager.cs`

**操作**:
1. 所有 `hero.soldier` 引用改为使用城市兵池数据
2. 确认/滑块逻辑改为操作城市兵池而非英雄兵力
3. Prefab 暂不改，底层接口留好

**依赖**: Task 1, Task 2

---

## Task 9: 全局搜索验证 — 确保无遗漏引用

**操作**:
1. 全局搜索 `hero.soldier`、`heroData.soldier`、`h.soldier`、`.soldier` 等模式
2. 确保所有对 SaveHeroData.soldier 的引用已清除或替换
3. 确保编译无错误

**依赖**: Task 1-8
