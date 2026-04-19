# SystemConst.cs 常量集中管理规范

## 目标
将散落在工程各处的非UI常量集中到 `SystemConst.cs` 文件中统一管理，按功能区域划分。

## 常量来源与分类

### 1. 游戏核心 (Game)
来源: `GameManager.cs`
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| BASE_YEAR | int | 194 | 游戏起始年份 |
| BORN_AGE | int | 16 | 武将出场年龄 |
| SEASONS_PER_YEAR | int | 36 | 一年的季节数 |

### 2. 战斗 (Battle)
来源: `BattleManager.cs`, `BattleStatManager.cs`
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| GRID_CELL_SIZE | int | 3 | 战斗地图每格大小(米) |
| MAX_ROUND | int | 30 | 战斗最大回合数 |
| WAIT_TIME | float | 1f | 战斗动作间等待时间(秒) |
| BATTLE_BEGIN_TIME | float | 3f | 战斗开始前准备时间(秒) |
| MAX_BATTLE_COUNT | int | 20 | 最大保存战斗记录数 |

### 3. 大地图 (WorldMap)
来源: `WorldPieceControl.cs`
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| MAP_SCALE_FACTOR | float | 1.25f | 地图缩放因子 |

### 4. AI策略 (AIStrategy)
来源: `StrategicDecider.cs`
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| MAX_ATK_CITIES | int | 2 | 每轮最多同时攻击城市数 |
| MIN_RESOURCE_FOR_ATTACK | int | 1500 | 发起攻击最低资源 |
| MIN_SOLDIER_FOR_ATTACK | int | 3000 | 发起攻击最低总兵力 |
| MIN_CITY_SOLDIER_FOR_ATTACK | int | 5000 | 攻击发起城市最低驻军 |
| MIN_CITY_HEROES_FOR_ATTACK | int | 3 | 攻击发起城市最低武将数 |
| MAX_SOLDIER_PER_HERO | int | 1000 | 每个武将最多带兵数 |

### 5. AI英雄 (AIHero)
来源: `HeroDispatcher.cs`
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| COMBAT_THRESHOLD | int | 150 | 战斗型武将属性阈值 |
| DOMESTIC_THRESHOLD | int | 150 | 内政型武将属性阈值 |
| MIN_REAR_HEROES | int | 1 | 后方城市最少保留武将数 |

### 6. AI城市 (AICity)
来源: `CityEvaluator.cs`, `TaskPriorityCalculator.cs`
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| GOLD_ALERT | int | 500 | 黄金告警阈值 |
| FOOD_ALERT | int | 500 | 粮食告警阈值 |
| WALL_ALERT | int | 150 | 城墙告警阈值 |
| SOLDIER_ALERT | int | 500 | 士兵告警阈值 |
| POWER_ALERT | int | 50 | 势力告警阈值 |
| NEED_WEIGHT | int | 30 | 城市需求权重 |

### 7. 经济 (Economy)
来源: `AI.cs`, `CityDevNodeChange.cs` (重复定义，合并为一处)
| 常量名 | 类型 | 值 | 说明 |
|--------|------|-----|------|
| EXCHANGE_RATE | float | 0.9f | 黄金-粮食兑换比率 |

## 排除的常量（UI相关，不移动）
- `BattleUIManager.cs`: `designWidth = 2048f`, `designHeight = 1536f` (UI设计分辨率)
- `MainPanelManager.cs`: `MAP_SCALE_FACTOR = 1.25f` (UI层地图缩放)
- `GameLog.cs`: `_lock`, `_fileLock` (线程锁对象，非业务常量)

## 文件结构设计

```csharp
public static class SystemConst
{
    public static class Game { ... }
    public static class Battle { ... }
    public static class WorldMap { ... }
    public static class AIStrategy { ... }
    public static class AIHero { ... }
    public static class AICity { ... }
    public static class Economy { ... }
}
```

使用嵌套静态类按功能分区，引用方式如 `SystemConst.Game.BASE_YEAR`。

## 引用替换策略
- 将各文件中的常量声明删除
- 替换所有引用为 `SystemConst.XXX.YYY` 形式
- 对于原 `public const` 在外部有引用的情况，需同步更新所有引用处

## 命名规范
- 统一使用大写蛇形命名 (UPPER_SNAKE_CASE)
- 原来使用 PascalCase 的常量（如 `gridCellSize` → `GRID_CELL_SIZE`，`MaxRound` → `MAX_ROUND`）统一转换
