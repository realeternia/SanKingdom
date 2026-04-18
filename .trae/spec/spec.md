# Spec: 删除 SaveHeroData.round 字段及相关逻辑

## 概述

从 `SaveHeroData` 类中删除 `public int round` 字段，并清理所有依赖该字段的判断逻辑、赋值操作和 UI 展示。

## 背景

`SaveHeroData.round` 字段用于记录英雄最后一次执行行动的回合数，通过与 `SaveData.round`（当前游戏回合）比较来判断英雄在本回合是否已经行动过。该机制涉及：

- **行动限制**：每回合每个英雄只能执行一次任务（发展、战斗、移动、褒奖、登用等）
- **UI 展示**：已行动英雄显示灰色遮罩/灰色文字，不可被选中
- **AI 决策**：AI 在调度和策略中检查英雄是否可行动
- **排序**：英雄选择面板按 `heroYear`（即 `round`）排序

## 影响范围

### 1. SaveHeroData.cs（字段定义 + 内部方法）

| 位置 | 代码 | 操作 |
|------|------|------|
| L20 | `public int round;` | 删除字段 |
| L53-56 | `SetRoundForRecruit()` 方法 | 删除整个方法 |
| L66 | `newHero.round = int.MaxValue;` | 删除该行赋值 |

### 2. Player.cs（核心判断逻辑）

| 位置 | 代码 | 操作 |
|------|------|------|
| L36-41 | `CheckHeroRound(int heroId)` | 删除方法 |
| L43-51 | `UpdateHeroesRound(int[] heroIds)` | 删除方法 |
| L54-65 | `GetAvailableHeroesThisYear(int[] heroList)` | 删除方法 |
| L70 | `heroList = GetAvailableHeroesThisYear(heroList).ToArray();` | 移除过滤，直接使用原始 heroList |
| L208 | `UpdateHeroesRound(heroList);` | 删除调用 |
| L290 | `var validHeroList = GetAvailableHeroesThisYear(heroList).ToArray();` | 移除过滤，直接使用原始 heroList |
| L306 | `UpdateHeroesRound(validHeroList);` | 删除调用 |
| L364 | `var validHeroList = GetAvailableHeroesThisYear(heroList).ToArray();` | 移除过滤，直接使用原始 heroList |
| L371 | `UpdateHeroesRound(validHeroList);` | 删除调用 |
| L395 | `heroList = GetAvailableHeroesThisYear(heroList).ToArray();` | 移除过滤，直接使用原始 heroList |
| L457 | `UpdateHeroesRound(heroList);` | 删除调用 |
| L514 | `hero.SetRoundForRecruit();` | 删除调用 |
| L538 | `UpdateHeroesRound(heroList);` | 删除调用 |
| L550 | `heroList = GetAvailableHeroesThisYear(heroList).ToArray();` | 移除过滤，直接使用原始 heroList |
| L602 | `UpdateHeroesRound(heroList);` | 删除调用 |

### 3. CityDetail.cs（UI 遮罩展示）

| 位置 | 代码 | 操作 |
|------|------|------|
| L95 | `var currentRound = GameManager.Instance.SaveData.round;` | 删除变量 |
| L114 | `bool hasActed = hero.round >= currentRound;` | 删除变量及判断 |
| L115-118 | `if (hasActed) { AddOverlay(...) }` | 删除整个 if 块（不再显示已行动遮罩） |

### 4. PopHeroSelectPanelCell.cs（英雄选择面板单元格）

| 位置 | 代码 | 操作 |
|------|------|------|
| L13 | `public int heroYear;` | 删除字段 |
| L53 | `heroYear = heroData.round;` | 删除赋值 |
| L91 | `isAvailable = ignoreActionCheck \|\| heroData.round != currentYear;` | 改为 `isAvailable = true;`（或直接移除 isAvailable 机制） |

### 5. PopHeroBattleSelectPanelCell.cs（战斗英雄选择面板单元格）

| 位置 | 代码 | 操作 |
|------|------|------|
| L15 | `public int heroYear;` | 删除字段 |
| L55 | `heroYear = heroData.round;` | 删除赋值 |
| L75 | `isAvailable = heroData.round != currentYear;` | 改为 `isAvailable = true;`（或直接移除 isAvailable 机制） |

### 6. PopHeroSelectPanelManager.cs（英雄选择面板排序）

| 位置 | 代码 | 操作 |
|------|------|------|
| L101-105 | 排序逻辑中 `a.heroYear.CompareTo(b.heroYear)` | 移除 heroYear 排序，仅按 attr1Val 排序 |

### 7. PopHeroBattleSelectPanelManager.cs（战斗英雄选择面板排序）

| 位置 | 代码 | 操作 |
|------|------|------|
| L96-100 | 排序逻辑中 `a.heroYear.CompareTo(b.heroYear)` | 移除 heroYear 排序，仅按 attr1Val 排序 |

### 8. HeroDispatcher.cs（AI 英雄调度）

| 位置 | 代码 | 操作 |
|------|------|------|
| L120 | `bool canMove = player.CheckHeroRound(heroToMove.heroId);` | 删除检查，英雄始终可调度 |
| L121-134 | `if (canMove) { ... }` | 移除 if 条件，内容直接执行 |

### 9. AIStrategyContext.cs（AI 策略上下文）

| 位置 | 代码 | 操作 |
|------|------|------|
| L35 | `if (player.CheckHeroRound(hero.heroId))` | 移除条件检查，英雄始终可用 |

### 10. SaveCityData.cs（城市占领逻辑）

| 位置 | 代码 | 操作 |
|------|------|------|
| L280 | `GameManager.Instance.GetPlayer(forceWin).UpdateHeroesRound(catchedHeroList.ToArray());` | 删除调用 |

## 设计决策

1. **移除行动限制机制**：删除 `round` 后，英雄不再受每回合一次行动的限制，所有英雄在任何时候都可被选中执行任务
2. **isAvailable 简化**：UI 中的 `isAvailable` 标志直接设为 `true`，保留该字段以维持 UI 交互逻辑的完整性（点击判断、颜色状态等），但英雄始终可用
3. **排序调整**：移除 `heroYear` 排序维度，英雄列表仅按主属性值降序排列
4. **AI 简化**：AI 调度和策略中不再检查行动回合，所有英雄始终可参与行动

## 不在范围内

- 不修改 `SaveData.round`（游戏全局回合数），该字段仍用于其他游戏逻辑
- 不修改存档序列化/反序列化逻辑（旧存档中包含 round 字段的数据在加载时会被忽略）
