# Tasks: 删除 SaveHeroData.round 字段及相关逻辑

## Task 1: 修改 SaveHeroData.cs

- [ ] 删除 `public int round;` 字段声明（L20）
- [ ] 删除 `SetRoundForRecruit()` 方法（L53-56）
- [ ] 删除 `CreateWildHero()` 中的 `newHero.round = int.MaxValue;`（L66）

**文件**: `Assets/Resources/Scripts/SaveDatas/SaveHeroData.cs`

---

## Task 2: 修改 Player.cs — 删除 round 相关方法

- [ ] 删除 `CheckHeroRound(int heroId)` 方法（L36-41）
- [ ] 删除 `UpdateHeroesRound(int[] heroIds)` 方法（L43-51）
- [ ] 删除 `GetAvailableHeroesThisYear(int[] heroList)` 方法（L54-65）

**文件**: `Assets/Resources/Scripts/Controls/Player.cs`

---

## Task 3: 修改 Player.cs — 清理方法调用

- [ ] `ExecuteCityDev`: 移除 `GetAvailableHeroesThisYear` 过滤（L70），删除 `UpdateHeroesRound` 调用（L208）
- [ ] `ExecuteCityBattleDev`: 移除 `GetAvailableHeroesThisYear` 过滤（L290），删除 `UpdateHeroesRound` 调用（L306）
- [ ] `ExecuteCityMoveDev`: 移除 `GetAvailableHeroesThisYear` 过滤（L364），删除 `UpdateHeroesRound` 调用（L371）
- [ ] `ExecuteCityChange`: 移除 `GetAvailableHeroesThisYear` 过滤（L395），删除 `UpdateHeroesRound` 调用（L457）
- [ ] `ExecuteCityUseHero`: 删除 `hero.SetRoundForRecruit()` 调用（L514），删除 `UpdateHeroesRound` 调用（L538）
- [ ] `ExecuteCityPraiseHero`: 移除 `GetAvailableHeroesThisYear` 过滤（L550），删除 `UpdateHeroesRound` 调用（L602），移除 methodId==1 时"所选英雄本回合已行动"的检查逻辑（L548-556）

**文件**: `Assets/Resources/Scripts/Controls/Player.cs`

---

## Task 4: 修改 CityDetail.cs — 移除已行动遮罩

- [ ] 删除 `var currentRound = ...` 变量（L95）
- [ ] 删除 `bool hasActed = hero.round >= currentRound;` 及对应的 if 块（L114-118）

**文件**: `Assets/Resources/Scripts/CityDetail.cs`

---

## Task 5: 修改 PopHeroSelectPanelCell.cs

- [ ] 删除 `public int heroYear;` 字段（L13）
- [ ] 删除 `heroYear = heroData.round;` 赋值（L53）
- [ ] 将 `isAvailable = ignoreActionCheck || heroData.round != currentYear;` 改为 `isAvailable = true;`（L91）
- [ ] 清理不再需要的 `var currentYear = ...` 变量（L55）

**文件**: `Assets/Resources/Scripts/PopHeroSelectPanelCell.cs`

---

## Task 6: 修改 PopHeroBattleSelectPanelCell.cs

- [ ] 删除 `public int heroYear;` 字段（L15）
- [ ] 删除 `heroYear = heroData.round;` 赋值（L55）
- [ ] 将 `isAvailable = heroData.round != currentYear;` 改为 `isAvailable = true;`（L75）
- [ ] 清理不再需要的 `var currentYear = ...` 变量（L57）

**文件**: `Assets/Resources/Scripts/PopHeroBattleSelectPanelCell.cs`

---

## Task 7: 修改 PopHeroSelectPanelManager.cs — 调整排序

- [ ] 移除排序中的 `heroYear` 比较逻辑（L101-105），仅保留按 `attr1Val` 降序排列

**文件**: `Assets/Resources/Scripts/PopHeroSelectPanelManager.cs`

---

## Task 8: 修改 PopHeroBattleSelectPanelManager.cs — 调整排序

- [ ] 移除排序中的 `heroYear` 比较逻辑（L96-100），仅保留按 `attr1Val` 降序排列

**文件**: `Assets/Resources/Scripts/PopHeroBattleSelectPanelManager.cs`

---

## Task 9: 修改 HeroDispatcher.cs（AI）

- [ ] 移除 `CheckHeroRound` 检查（L120），将 if 块内容直接执行（L121-134）

**文件**: `Assets/Resources/Scripts/Controls/AI/HeroDispatcher.cs`

---

## Task 10: 修改 AIStrategyContext.cs（AI）

- [ ] 移除 `CheckHeroRound` 条件检查（L35），英雄始终加入可用列表

**文件**: `Assets/Resources/Scripts/Controls/AI/AIStrategyContext.cs`

---

## Task 11: 修改 SaveCityData.cs

- [ ] 删除 `UpdateHeroesRound` 调用（L280）

**文件**: `Assets/Resources/Scripts/SaveDatas/SaveCityData.cs`
