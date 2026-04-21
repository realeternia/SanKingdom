# SanKingdom 项目规则

## 项目概述

SanKingdom 是一款基于 Unity 引擎的三国题材策略游戏，采用 C# 开发。游戏包含回合制战略层（城市管理、英雄调度、势力对抗）和实时战斗层（棋盘式自动战斗）两大核心系统。

## 技术栈

- **引擎**: Unity (C#)
- **运行时**: .NET Framework / Mono (Unity 内置)
- **序列化**: Unity JsonUtility
- **UI 框架**: Unity UGUI
- **文本渲染**: TextMeshPro (SDF Font)

## 项目结构

```
Assets/Resources/Scripts/
├── Combat/                    # 战斗系统
│   ├── Actions/               # 战斗动作（Command模式）
│   ├── Buffs/                 # Buff系统
│   ├── Skills/                # 技能系统
│   ├── Effects/               # 视觉特效
│   ├── OOs/                   # 战斗接口，面向对象的设计
│   ├── Chess.cs               # 棋子（战斗单位）
│   ├── Missile.cs             # 投射物
│   ├── SceneObj.cs            # 场景对象基类
│   ├── SkillManager.cs        # 技能管理器
│   ├── BuffManager.cs         # Buff管理器
│   ├── EffectManager.cs       # 特效管理器
│   └── BattleStatManager.cs   # 战斗统计，支持战斗回放
├── Controls/                  # 控制层
│   ├── AI/                    # AI策略系统
│   │   ├── AI.cs              # AI入口
│   │   ├── AIStrategyContext.cs  # AI上下文
│   ├── Utils/                 # 工具类
│   │   └── GameLog.cs         # 日志系统
│   ├── BattleManager.cs       # 战斗管理器
│   ├── BattleUIManager.cs     # 战斗UI管理
│   ├── GameManager.cs         # 游戏主管理器
│   ├── PanelManager.cs        # 面板管理器
│   └── Player.cs              # 玩家逻辑
├── Configs/                   # 配置表系统
│   ├── ConfigManager.cs       # 配置管理器
├── PO/                        # 数据对象，建议可以把简单的数据结构放到这里
│   ├── AttrInfo.cs            # 属性信息
│   ├── BattleCardData.cs      # 战斗卡牌数据
├── SaveDatas/                 # 存档数据，持久化的数据
│   ├── SaveData.cs            # 总存档
│   ├── SaveCityData.cs        # 城市存档
│   ├── SaveHeroData.cs        # 英雄存档
├── SystemTool/                # 工具类方法
└── UIScripts/                 # UI脚本
```

## 编码规范

### 命名约定

- **类名**: PascalCase（如 `BattleManager`, `SaveCityData`）
- **公共字段**: camelCase（如 `forceId`, `heroId`, `cityId`）
- **私有字段**: camelCase，不使用下划线前缀（如 `gameFinish`, `battleResult`）
- **常量**: UPPER_SNAKE_CASE（如 `MAX_ROUND`, `BASE_YEAR`）
- **方法名**: PascalCase（如 `BattleBegin`, `GetUnitsInRange`）
- **枚举**: PascalCase 枚举名和成员（如 `BattleResult.Win`, `HeroState.Normal`）
- **属性**: PascalCase（如 `IsPlayer`, `HpRate`, `Phase`）
- **命名空间**: 配置类使用 `CommonConfig`，工具类使用 `Controls.Utils`

### 序列化规则

- 使用 `[Serializable]` 标记需要序列化的类
- 使用 `[NonSerialized]` 标记运行时引用（如 `viewObj`, `owner` 等Unity对象引用和回调）
- 使用 `[SerializeReference]` 标记多态序列化字段（如 `List<ChessAction> actions`）
- 使用 Unity `JsonUtility` 进行序列化/反序列化
- 所有存档数据类（`SaveData`, `SaveCityData`, `SaveHeroData` 等）必须可被 JsonUtility 序列化

### 单例模式

- Manager 类使用 Unity MonoBehaviour 单例模式：在 `Awake()` 中赋值 `Instance = this`
- 静态工具类使用 `static class`（如 `SysFormula`, `SystemConst`, `ConfigManager`, `AI`）
- 常见单例：`GameManager.Instance`, `BattleManager.Instance`, `PanelManager.Instance`

### 日志规范

- 使用 `GameLog` 而非 `UnityEngine.Debug.Log`
- 日志级别：`GameLog.Debug()`, `GameLog.Info()`, `GameLog.Warn()`, `GameLog.Error()`
- AI 相关日志使用标签：`GameLog.SetTag("AI").Info(...)`
- 日志消息使用中文描述，关键参数用 `$"..."` 内插

## 核心架构模式

### 1. 战斗系统 - Tick 驱动 + Action 队列

战斗采用 Tick 驱动的帧同步架构：
- `tickTimeReal = 0.1f` 为基础 Tick 间隔
- 所有战斗动作封装为 `ChessAction` 子类，放入 `actions` 队列
- `ChessAction` 包含 `SourceId` 和 `Tick`，在对应 Tick 执行 `Doing()`
- 逻辑更新（`LogicUpdate`）和渲染更新（`RenderUpdate`）分离
- 战斗回放通过序列化/反序列化 `BattleManager` 实现

新增战斗动作时：
1. 继承 `ChessAction`，实现 `Doing()` 方法
2. 通过 `BattleManager.AddChessAction()` 添加到队列
3. 注意 `isDoingAction` 时的 Tick 顺延逻辑

### 2. 配置系统 - 静态加载

- 所有配置类在 `CommonConfig` 命名空间下
- 配置类使用 `XxxConfig_s.cs` 命名
- 通过 `XxxConfig.GetConfig(id)` 静态方法获取配置
- 通过 `XxxConfig.ConfigList` 获取全部配置列表
- `ConfigManager.Init()` 在游戏启动时统一加载所有配置

### 3. 公式与常量分离

- **所有游戏数值常量** 必须定义在 `SystemConst` 的嵌套静态类中
- **所有计算公式** 必须定义在 `SysFormula` 的嵌套静态类中
- `SystemConst` 和 `SysFormula` 使用相同的嵌套类分类（如 `Battle`, `Hero`, `City`, `Economy`, `AIStrategy` 等）
- 禁止在业务代码中出现魔法数字，必须提取到 `SystemConst`
- 禁止在业务代码中内联计算公式，必须提取到 `SysFormula`

### 4. AI 策略系统 - 分层决策

AI 系统采用分层架构：
- `AI.ExecutePlanningPhase()` - 入口
- `HeroDispatcher` - 英雄调度（前后线分配）
- `StrategicDecider` - 战略决策（攻击/防御/发展）
- `CityEvaluator` - 城市评估
- `HeroTaskMatcher` - 英雄任务匹配
- `TaskPriorityCalculator` - 任务优先级计算
- `AIStrategyContext` - AI 上下文数据

### 5. 存档系统

- `SaveData` 为顶层存档对象，包含 `forces`, `cities`, `heros` 列表
- 存档通过 `JsonUtility.ToJson()` / `JsonUtility.FromJson()` 序列化
- 存储路径：`Application.persistentDataPath`
- 战斗回放文件：`battlereplayer{battleId}.json.json`
- 游戏存档文件：`game_save.json`

### 6. 回合系统

- 回合阶段：`TurnPhase.None` → `Planning` → `Execution` → `Battle` → 回合结束
- 玩家确认计划后进入执行阶段：`GameManager.ConfirmPlan()`
- AI 自动执行计划阶段：`AI.ExecutePlanningPhase()`
- 面板信号系统：`PanelManager.Instance.SendSignal(signalName, data, id)`

### 7. IRecoverable 接口

- 战斗对象（`Chess`, `Missile`, `Skill`, `Buff`）实现 `IRecoverable` 接口
- `OnRecover()` 在反序列化后调用，用于重建运行时引用
- 反序列化后必须遍历所有对象调用 `OnRecover()`

## 修改代码时的注意事项

### 战斗系统修改

- 新增棋子行为时，确保同时处理逻辑层和表现层
- 修改 Tick 相关逻辑时，注意 `quickMode` 下的加速倍率
- `chessList.ToArray()` 用于遍历中可能新增元素的场景
- 战斗中新增字段如需序列化，注意 `[NonSerialized]` 和 `[SerializeReference]` 的使用
- `BattleManager.IsBattleRunning` 用于防止战斗重入

### 存档数据修改

- 新增 `SaveData` 字段时，需考虑旧存档兼容（JsonUtility 反序列化时缺失字段为零值）
- `SaveCityData.actions` 使用 `[NonSerialized]`，不会持久化
- `DevAssignmentData` 使用列表存储，通过 `SetDevAssignment` / `RemoveDevAssignment` 管理

## 关键游戏概念

- **forceId**: 势力 ID，标识归属阵营
- **heroId**: 英雄配置 ID（全局唯一）
- **cityId**: 城市配置 ID
- **armsId**: 兵种配置 ID
- **isHero**: 棋子是否为英雄单位（区分英雄和召唤物）
- **isShadow**: 棋子是否为影子/幻象（不计入存活判定）
- **HpRate**: 当前血量比率（hp / maxHp）
- **ownerHeroId**: 城市太守的英雄 ID
- **loyalty**: 英雄忠诚度（0-100）
- **HeroState**: Normal / Wild / Catched
- **CityStrategyState**: Dev / Atk / Def
- **BattleResult**: Win / Lose / Draw

## 禁止事项

- 不要在战斗逻辑中使用 `UnityEngine.Random`，应使用 `System.Random` 以保证帧同步一致性（当前部分代码已使用 `UnityEngine.Random`，新增代码应尽量使用 `System.Random`）
- 不要在 `SaveData` 及其子类中存储 Unity 对象引用
- 不要在配置类中使用属性（Property），使用公共字段（Field）
- 不要在业务代码中硬编码数值，必须提取到 `SystemConst`
- 不要在业务代码中内联计算逻辑，必须提取到 `SysFormula`
- 不要使用 `UnityEngine.Debug.Log`，统一使用 `GameLog`
