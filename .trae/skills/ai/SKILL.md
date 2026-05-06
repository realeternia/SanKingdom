---
name: "ai"
description: "AI策略系统规则，包含分层决策架构、HeroDispatcher、StrategicDecider等组件规范。Invoke when working on AI logic, hero dispatching, strategic decisions, or AI-related code."
---

# AI 策略系统规则

## 分层决策架构

AI 系统采用分层流水线架构，入口为 `AI.ExecutePlanningPhase()`：

```
AI.ExecutePlanningPhase(force)
  │
  ├── 1. StrategicDecider.ClearRoundData()          // 清除回合数据
  ├── 2. new AIStrategyContext(force)                // 构建上下文
  ├── 3. HeroDispatcher.DispatchHeroes(force)        // 英雄前后线调度
  ├── 4. StrategicDecider.DetermineCityStrategies()   // 决定各城市战略状态
  ├── 5. AssignHeroesToDev(force, context)            // 分配英雄到内政任务
  ├── 6. GenerateWarPlans(force, context, ...)        // 生成战争计划
  └── 7. GameManager.Instance.ConfirmPlan(forceId)    // 确认计划
```

## 组件清单

| 文件 | 类名 | 职责 |
|------|------|------|
| AI.cs | `AI` (static) | 入口编排、内政分配、战争计划生成 |
| AIStrategyContext.cs | `AIStrategyContext` | AI 上下文数据封装 |
| HeroDispatcher.cs | `HeroDispatcher` | 英雄调度（前后线分配） |
| StrategicDecider.cs | `StrategicDecider` | 战略决策（攻击/防御/发展） |
| CityEvaluator.cs | `CityEvaluator` | 城市需求评估 |
| HeroTaskMatcher.cs | `HeroTaskMatcher` | 英雄任务匹配 |
| TaskPriorityCalculator.cs | `TaskPriorityCalculator` | 任务优先级计算 |
| CityStrategyState.cs | `CityStrategyState` (enum) | 城市战略状态枚举 |

## 各组件详细规范

### AI — 入口静态类

| 方法 | 签名 | 说明 |
|------|------|------|
| `ExecutePlanningPhase` | `static void ExecutePlanningPhase(SaveForceData force)` | AI 计划阶段入口 |
| `AssignHeroesToDev` | `static void AssignHeroesToDev(SaveForceData force)` | 为势力所有城市分配内政英雄 |

内部方法：`FindBestHeroForDev`、`CalculateHeroDevScore`、`GetWeightedAttrValue`、`GenerateWarPlans`、`TryCreateWarPlan`

### AIStrategyContext — 上下文数据

```csharp
public class AIStrategyContext
{
    public SaveForceData force;
    public List<SaveCityData> cities;
    public Dictionary<int, List<SaveHeroData>> cityHeroes;  // cityId → 英雄列表

    public AIStrategyContext(SaveForceData force)
    public List<SaveHeroData> GetAvailableHeroes(int cityId)
}
```

### HeroDispatcher — 英雄调度器

```csharp
public enum HeroType { Combat, Domestic, Balanced }

public class HeroDispatcher
{
    public static HeroType ClassifyHero(SaveHeroData hero)   // 委托 SysFormula.Hero.ClassifyHero()
    public static void DispatchHeroes(SaveForceData force)    // 将后方战斗英雄调往前线
}
```

分类逻辑：
- 战斗分 = str + leadShip + inte，内政分 = inte + fair + charm
- 战斗分 ≥ 150 且 > 内政分 × 优势比 → Combat
- 内政分 ≥ 150 且 > 战斗分 × 优势比 → Domestic
- 否则 → Balanced

调度逻辑：使用 `MapTool.GetFrontlineCityIds()` / `GetRearCityIds()` 区分前后线，前线目标战斗英雄数 = `FRONTLINE_COMBAT_HEROES_TARGET`，后方保留最少 = `MIN_REAR_HEROES`

### StrategicDecider — 战略决策器

```csharp
public class AttackCandidate
{
    public int sourceCityId, targetCityId, mySoldier, targetSoldier;
    public float advantage;    // = mySoldier / max(1, targetSoldier)
    public string sourceType;  // "目标优先" 或 "己方城市优先"
}

public class StrategicDecider
{
    public static void ClearRoundData()
    public static int? GetAttackTarget(int sourceCityId)
    public static void MarkTargetAttacked(int forceId, int targetCityId)
    public static bool HasAttackedTarget(int forceId, int targetCityId)
    public static Dictionary<int, CityStrategyState> DetermineCityStrategies(SaveForceData force)
}
```

决策逻辑：
- 默认所有城市为 Dev
- 若 `CanExpand()` 为 true，从双路候选中选择攻击目标（最多 `MAX_ATK_CITIES` 个）
- 若无攻击计划，前线城市检查威胁，有威胁则设为 Def

**双路攻击候选选择**：
1. `SelectAttackTargetsByEnemy`：以敌方城市为目标，找相邻己方城市中兵力最多的作为攻击源（"目标优先"）
2. `SelectAttackTargetsByOwnCity`：以己方城市为源，找相邻敌方城市中兵力最少的作为目标（"己方城市优先"）

候选按 `advantage` 降序排列，去重选择。维护 `attackedTargetsThisRound` 和 `attackTargets` 静态字典，每回合通过 `ClearRoundData()` 清除。

### CityEvaluator — 城市评估器

```csharp
public enum CityNeedType { None, GoldShortage, FoodShortage, WallLow, SoldierShortage, HappyLow }

public class CityNeed
{
    public CityNeedType needType;
    public int priority;
    public string attrName;
    public int currentValue, alertValue;
}

public class CityEvaluator
{
    public static List<CityNeed> EvaluateCity(SaveCityData city)
    public static bool IsFrontlineCity(SaveCityData city)  // 委托 MapTool
}
```

评估 5 种需求缺口（金币/粮草/城墙/士兵/民心），优先级公式：`(alert - current) * 100 / alert`

### HeroTaskMatcher — 英雄任务匹配器

```csharp
public class HeroTaskMatch
{
    public SaveHeroData hero;
    public int devId;
    public float matchScore;
}

public class HeroTaskMatcher
{
    public static float CalculateMatchScore(SaveHeroData hero, CityDevConfig config)
    public static HeroTaskMatch FindBestTask(SaveHeroData hero, List<TaskPriorityInfo> availableTasks)
    public static List<HeroTaskMatch> AssignTasksToHeroes(List<SaveHeroData> heroes, List<TaskPriorityInfo> availableTasks)
    public static Dictionary<int, List<int>> AssignHeroesToTasks(List<SaveHeroData> availableHeroes, List<TaskPriorityInfo> availableTasks)
}
```

综合分 = `matchScore + adjustedPriority * TASK_PRIORITY_WEIGHT`

### TaskPriorityCalculator — 任务优先级计算器

```csharp
public class TaskPriorityInfo
{
    public int devId, basePriority, adjustedPriority;
    public CityDevConfig config;
}

public class TaskPriorityCalculator
{
    public static TaskPriorityInfo GetBattleTask(SaveCityData city)
    public static List<TaskPriorityInfo> GetAvailableTasks(SaveCityData city, CityStrategyState state, List<CityNeed> cityNeeds)
}
```

基础优先级根据城市战略状态选择配置字段：Dev→`AiPriotyDev`，Atk→`AiPriotyAtk`，Def→`AiPriotyDef`。调整优先级 = 基础优先级 + `NEED_WEIGHT * needPriority / 100`

### CityStrategyState — 城市战略状态枚举

```csharp
public enum CityStrategyState { Dev, Def, Atk }
```

## 常量依赖（SystemConst）

| 嵌套类 | 关键常量 | 用途 |
|--------|----------|------|
| **AIStrategy** | `MAX_ATK_CITIES=2` | 最大攻击城市数 |
| | `MIN_RESOURCE_FOR_ATTACK=1500` | 攻击所需最小资源 |
| | `MIN_SOLDIER_FOR_ATTACK=3000` | 攻击所需最小总兵力 |
| | `AI_MIN_ATTACK_SOLDIER=500` | 战争计划最低兵力 |
| | `FRONTLINE_COMBAT_HEROES_TARGET=3` | 前线战斗英雄目标数 |
| | `MAX_HEROES_PER_TASK=3` | 每任务最大英雄数 |
| | `TASK_PRIORITY_WEIGHT=0.5` | 任务优先级权重 |
| **AIHero** | `COMBAT_THRESHOLD=150` | 战斗英雄阈值 |
| | `MIN_REAR_HEROES=1` | 后方最少英雄数 |
| **AICity** | `GOLD_ALERT/FOOD_ALERT=500` | 金/粮警戒值 |
| | `WALL_ALERT=150` | 城墙警戒值 |
| | `SOLDIER_ALERT=500` | 士兵警戒值 |
| | `HAPPY_ALERT=50` | 民心警戒值 |
| | `NEED_WEIGHT=30` | 需求权重 |

## 公式依赖（SysFormula.AIStrategy）

| 方法 | 逻辑 |
|------|------|
| `CalculatePriority(current, alert)` | `(alert - current) * 100 / alert` |
| `AdjustPriorityByNeeds(basePriority, needPriority)` | `basePriority + NEED_WEIGHT * needPriority / 100` |
| `CalculateAdvantageRatio(my, target)` | `my / max(1, target)` |
| `CalculateEffectiveSoldier(citySoldier, heroCount)` | `min(citySoldier, (heroCount-1) * 1000)` |
| `CheckAttackSourceAdvantage(my, target)` | `my >= target * 0.7` |
| `CheckOwnCityAttackAdvantage(my, target)` | `my >= target * 0.8` |
| `CheckAttackFoodSufficient(soldier, food)` | `food >= soldier / 2` |
| `HasThreat(enemySoldier)` | `enemySoldier >= 500` |
| `CanExpand(gold, food, soldier)` | 三项均 ≥ 对应阈值 |
| `CalculateFoodNeeded(totalSoldier)` | `totalSoldier / 2` |

## AI 日志规范

- AI 相关日志使用标签：`GameLog.SetTag("AI").Info(...)`
- 日志消息使用中文描述，关键参数用 `$"..."` 内插

## 设计模式

- **静态类无状态设计**：除 `StrategicDecider` 维护回合级静态状态外，所有 AI 组件均为静态类/静态方法
- **常量/公式外提**：严格遵循项目规范，所有数值在 `SystemConst`，所有公式在 `SysFormula`
- **需求驱动优先级调整**：`CityEvaluator` 识别需求 → `TaskPriorityCalculator` 调整优先级 → `HeroTaskMatcher` 匹配英雄
- **前后线分离调度**：`HeroDispatcher` 将后方战斗英雄调往前线

## AI 与回合系统

- AI 自动执行计划阶段：`AI.ExecutePlanningPhase()`
- 回合阶段：`TurnPhase.None` → `Planning` → `Execution` → `Battle` → 回合结束
