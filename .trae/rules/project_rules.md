# SanKingdom 项目规则

## 项目概述

Unity C# 三国策略游戏，包含回合制战略层和实时战斗层两大系统。

## 技术栈

Unity (C#) | JsonUtility | UGUI | TextMeshPro

## 编码规范

### 命名约定

- 类名/方法名/属性/枚举：PascalCase
- 公共/私有字段：camelCase（不使用下划线前缀）
- 常量：UPPER_SNAKE_CASE
- 命名空间：配置类用 `CommonConfig`，工具类用 `Controls.Utils`

### 单例模式

- Manager 类：MonoBehaviour 单例，`Awake()` 中赋值 `Instance = this`
- 静态工具类：`static class`（如 `SysFormula`, `SystemConst`, `ConfigManager`, `AI`）

### 日志规范

使用 `GameLog`（Debug/Info/Warn/Error），禁止 `UnityEngine.Debug.Log`，消息用中文。

### 随机数规范

- 战斗层：`BattleRandom`（Combat/ 目录）
- 战略层：`SysRandom`
- 禁止使用 `UnityEngine.Random`

## 核心架构

### 配置系统

- 命名空间：`CommonConfig`，命名：`XxxConfig_s.cs`
- 获取配置：`XxxConfig.GetConfig(id)` / `XxxConfig.ConfigList`
- GetConfig/GetConfigByname 后无需 null 检查
- 配置类只存数据，定制逻辑放 `SystemTool/` 目录
- 禁止在配置类（`XxxConfig_s.cs`）中添加业务方法，配置类仅保留数据定义、Load、GetConfig、HasConfig、Refresh、Add、Remove、Assign 等基础方法

### 公式与常量

- 数值常量：`SystemConst` 嵌套静态类（`SystemTool/SystemConst.cs`）
- AI相关常量：`AIConst` 嵌套静态类（`Controls/AI/AIConst.cs`），禁止将AI常量放在 `SystemConst` 中
- 计算公式：`SysFormula` 嵌套静态类
- 禁止魔法数字和内联计算公式
- `SysFormula` 仅收录含实质计算逻辑（含参数运算、随机数、阈值判断等）的公式，禁止封装「直接返回常量」的方法：调用方直接使用 `SystemConst` 中的常量即可
- `SysFormula` 仅收录游戏数值公式，禁止放入 UI 布局/尺寸计算等无跨模块复用性的方法：UI 相关计算应放在对应 UI 组件（如 `ResItem`）的 static 方法中
- `SysFormula` 内部禁止引用 `SystemConst`：公式自身的数值边界（如随机区间上下限、阈值）直接内联字面量，`SystemConst` 仅服务于外部调用方/跨模块共享常量

### 资源路径 - ResPath

禁止硬编码路径，使用 `ResPath` 静态类（`SystemTool/ResPath.cs`）：
- `ResPath.Texture.HeroIcon(icon)` / `HeroDefaultIcon()` / `HeroBigIcon(icon)` / `AttrIcon(icon)`

### 资源缓存 - ResourceCache

禁止直接使用 `Resources.Load`，使用 `ResourceCache`（`SystemTool/ResourceCache.cs`）：
- 战略层：`LoadUI<T>` / `LoadPrefabUI` / `LoadSpriteUI`
- 战斗层：`LoadBattle<T>` / `LoadPrefabBattle` / `LoadSpriteBattle`

### 颜色系统 - SysColor

禁止硬编码颜色，使用 `SysColor` 静态类（`SystemTool/SysColor.cs`）：
- 兵种等级颜色：`SysColor.GetArmsLevelColor(level)` 返回 `Color`，配合 `ColorUtility.ToHtmlStringRGB` 生成富文本 hex
- 势力颜色：`SysColor.GetForceColor(forceId)`
- 属性值颜色：`SysColor.GetColorByValue(attrName, value)` / `GetColoredText` / `GetColoredTextWithRule`

### 回合系统

`TurnPhase.None` → `Planning` → `Execution` → `Battle` → 回合结束

### 事件系统 - GameEventLog

与 `SaveData` 平级挂载于 `GameManager`，独立 `game_events.json` 存取，生命周期同步 SaveData。

- **数据类**：`GameEventData`（`EventSystem/GameEventData.cs`），全 int 字段，无 string。`effectValue`/`effectValue2` 按 eventType 区分语义
- **日志类**：`GameEventLog`（`EventSystem/GameEventLog.cs`），提供 `RecordEvent` 入口和生命周期方法
- **工厂方法**：`GameEventData.CreateXxx(...)`，禁止直接 new GameEventData 手动赋值
- **记录时机**：
  - 战斗：ExecuteBattle 记 BattleAttack+BattleDefend，OnBattleEnd 记 BattleResult
  - KingAction：各 ExecuteCityXxx 方法内立即记录
  - 状态变化：Catched/Wild/Escape/RecruitSuccess 在状态变更点立即记录
  - Dev 委派：不在 SetDevAssignment/RemoveDevAssignment 记录，回合末 OnRoundEnd 通过快照 diff 仅记录净变化（assign/cancel/change）
- **过期清理**：OnRoundEnd 从队列头移除 `round < currentRound - SEASONS_PER_YEAR` 的事件
- **Dev 快照**：`lastDevSnapshot` 标记 `[NonSerialized]`，加载后由 `InitLoadedData` 从当前 SaveData 重建

### 面板信号系统

基类 `SignalData`（Name 字段）+ 派生类（专属字段），通过 `PanelManager.SendSignal` 分发。

新增信号：在 `PO/SignalData.cs` 新增派生类，构造函数设置 `Name`。

## 全局标识符

- forceId：势力 ID
- heroId：英雄配置 ID
- cityId：城市配置 ID
- armsId：兵种配置 ID

领域专属概念见各 skill：battle skill（战斗层）、save skill（战略层）、ai skill（AI）

## 文档目录 - Doc

项目文档统一存放于 `Doc/` 目录（项目根目录下）：
- 系统设计、数值公式整理、架构说明等长篇文档均放此目录
- 命名规范：小写连字符（kebab-case），如 `non-combat-calculations.md`
- 文档内引用代码位置时使用 `file:///` 绝对路径 + 行号锚点，便于 IDE 跳转

## 禁止事项

- 禁止 `UnityEngine.Random`，用 `BattleRandom` 或 `SysRandom`
- 禁止配置类使用属性，用公共字段
- 禁止在配置类（`XxxConfig_s.cs`）中添加业务方法，仅保留数据定义、Load、GetConfig、HasConfig、Refresh、Add、Remove、Assign 等基础方法
- 禁止硬编码数值，提取到 `SystemConst`
- 禁止内联计算公式，提取到 `SysFormula`
- 禁止在 `SysFormula` 中封装「直接返回常量」的方法，调用方直接使用 `SystemConst` 常量
- 禁止在 `SysFormula` 中放入 UI 布局/尺寸计算方法，应放在对应 UI 组件的 static 方法中
- 禁止 `SysFormula` 内部引用 `SystemConst`，公式数值边界直接内联字面量
- 禁止 `UnityEngine.Debug.Log`，用 `GameLog`
- 禁止硬编码资源路径，用 `ResPath`
- 禁止直接 `Resources.Load`，用 `ResourceCache`
- 禁止硬编码颜色，用 `SysColor`
- 新增 `.cs` 文件必须在 `Assembly-CSharp.csproj` 添加 `<Compile Include>`
- 禁止静默 null check return，必须记录日志

## 错误处理

从 ID 获取数据失败、配置加载失败、关键业务逻辑前置条件不满足、网络/存储操作失败时必须记录日志。可静默返回的情况：遍历跳过无效项、可选参数为空、UI 数据未就绪。
