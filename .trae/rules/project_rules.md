# SanKingdom 项目规则

## 项目概述

SanKingdom 是一款基于 Unity 引擎的三国题材策略游戏，采用 C# 开发。游戏包含回合制战略层（城市管理、英雄调度、势力对抗）和实时战斗层（棋盘式自动战斗）两大核心系统。

## 技术栈

- **引擎**: Unity (C#)
- **运行时**: .NET Framework / Mono (Unity 内置)
- **序列化**: Unity JsonUtility
- **UI 框架**: Unity UGUI
- **文本渲染**: TextMeshPro (SDF Font)

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

### 单例模式

- Manager 类使用 Unity MonoBehaviour 单例模式：在 `Awake()` 中赋值 `Instance = this`
- 静态工具类使用 `static class`（如 `SysFormula`, `SystemConst`, `ConfigManager`, `AI`）
- 常见单例：`GameManager.Instance`, `BattleManager.Instance`, `PanelManager.Instance`

### 日志规范

- 使用 `GameLog` 而非 `UnityEngine.Debug.Log`
- 日志级别：`GameLog.Debug()`, `GameLog.Info()`, `GameLog.Warn()`, `GameLog.Error()`
- 日志消息使用中文描述，关键参数用 `$"..."` 内插

### 随机数规范

项目提供两个随机数工具类，封装 `System.Random` 以保证帧同步一致性：

- **`BattleRandom`** — 战斗层专用（详见 battle skill）
- **`SysRandom`** — 战略层专用，用于非战斗的游戏逻辑（登用判定、俘虏判定、英雄移动、城市选择等）
  - 方法：`Range(int min, int max)`、`Value`（0-1浮点）、`Next(int max)`

使用规则：
- 战斗代码（`Combat/` 目录下）必须使用 `BattleRandom`
- 战略层代码使用 `SysRandom`
- 禁止在游戏逻辑中使用 `UnityEngine.Random`

## 核心架构模式

### 配置系统 - 静态加载

- 所有配置类在 `CommonConfig` 命名空间下
- 配置类使用 `XxxConfig_s.cs` 命名
- 通过 `XxxConfig.GetConfig(id)` 静态方法获取配置
- 通过 `XxxConfig.ConfigList` 获取全部配置列表
- `ConfigManager.Init()` 在游戏启动时统一加载所有配置
- GetConfig或者GetConfigByname之后，不需要再检查是否为null
- 几种值的类型配置，需要考虑是否用枚举表示，枚举类型放到PO目录下
- **配置文件是模板化的，不要在配置类中添加定制方法**。定制逻辑应放到 `SystemTool/` 目录下的工具类中

### 公式与常量分离

- **所有游戏数值常量** 必须定义在 `SystemConst` 的嵌套静态类中
- **所有计算公式** 必须定义在 `SysFormula` 的嵌套静态类中
- `SystemConst` 和 `SysFormula` 使用相同的嵌套类分类（如 `Battle`, `Hero`, `City`, `Economy`, `AIStrategy` 等）
- 禁止在业务代码中出现魔法数字，必须提取到 `SystemConst`
- 禁止在业务代码中内联计算公式，必须提取到 `SysFormula`

### 资源路径管理 - ResPath

所有 `Resources.Load` 的路径字符串必须通过 `ResPath` 静态工具类获取，禁止硬编码路径。

- **工具类位置**: `SystemTool/ResPath.cs`，`static class`
- **嵌套分类**: `ResPath.Texture`（纹理）、`ResPath.Prefab`（预制体）、`ResPath.Material`（材质）、`ResPath.Font`（字体）
- **使用方式**: `Resources.Load<Sprite>(ResPath.Texture.HeroIcon(heroCfg.Icon))`

主要方法：
- `ResPath.Texture.HeroIcon(icon)` — 英雄头像 `"Textures/Skins/"`
- `ResPath.Texture.HeroDefaultIcon()` — 默认头像 `"Textures/Skins/moren"`
- `ResPath.Texture.HeroBigIcon(icon)` — 大头像 `"Textures/SkinsBig/"`
- `ResPath.Texture.AttrIcon(icon)` — 属性图标 `"Textures/Icons/"`

新增路径时：
1. 在 `ResPath` 对应嵌套类中添加静态方法
2. 方法应接受可变部分作为参数，固定前缀封装在方法内
3. 仅当路径完全固定（无参数）时才使用无参方法

### 回合系统

- 回合阶段：`TurnPhase.None` → `Planning` → `Execution` → `Battle` → 回合结束
- 玩家确认计划后进入执行阶段：`GameManager.ConfirmPlan()`
- 面板信号系统：`PanelManager.Instance.SendSignal(SignalData data)`

### 面板信号系统 - SignalData 继承体系

信号采用基类 + 派生类的方式传递数据，基类 `SignalData` 持有 `Name` 字段标识信号类型，派生类持有各自专属字段：

```
SignalData (基类, Name 字段)
├── CityResChangeSignal      — CityId, ResType, Value
├── CityForceChangeSignal    — CityId
├── ForceResChangeSignal     — ResType, Value
├── PhaseChangeSignal        — PhaseName, ForceId
├── AICheckSignal            — AIName, ForceId
├── RoundChangeSignal        — Round
└── CityAttrChangeSignal     — CityId
```

- 接口：`IPanelEvent.SendSignal(SignalData data)`
- 信号分发中心：`PanelManager.SendSignal(SignalData data)` 转发给 `MainPanelManager` 和 `openPanelList` 中实现了 `IPanelEvent` 的面板
- 接收方通过 `data.Name` 判断是否处理，需要字段时用 `as` 转换为具体派生类
- 发送方直接 `new` 具体派生类并设置字段

新增信号时：
1. 在 `PO/SignalData.cs` 中新增派生类，继承 `SignalData`，在无参构造函数中设置 `Name`
2. 在发送方构造新派生类实例
3. 在接收方通过 `data.Name` 判断并 `as` 转换后使用

## 修改代码时的注意事项

### 新增文件

- 新增 `.cs` 文件后，必须在 `Assembly-CSharp.csproj` 中添加对应的 `<Compile Include="..." />` 条目
- csproj 路径：项目根目录下 `Assembly-CSharp.csproj`
- 条目格式：`<Compile Include="Assets\Resources\Scripts\PO\SignalData.cs" />`（使用相对路径，反斜杠分隔）

## 全局标识符

- **forceId**: 势力 ID，标识归属阵营（跨所有系统）
- **heroId**: 英雄配置 ID（全局唯一，跨所有系统）
- **cityId**: 城市配置 ID（跨所有系统）
- **armsId**: 兵种配置 ID（跨战斗+存档+配置）

领域专属概念见各 skill：战斗层概念（isHero, isShadow, HpRate, BattleResult）见 battle skill；战略层概念（ownerHeroId, loyalty, HeroState）见 save skill；AI 概念（CityStrategyState）见 ai skill

## 禁止事项

- 不要在战斗逻辑中使用 `UnityEngine.Random`，必须使用 `BattleRandom`（战斗层）或 `SysRandom`（战略层）
- 不要在配置类中使用属性（Property），使用公共字段（Field）
- 不要在业务代码中硬编码数值，必须提取到 `SystemConst`
- 不要在业务代码中内联计算逻辑，必须提取到 `SysFormula`
- 不要使用 `UnityEngine.Debug.Log`，统一使用 `GameLog`
- 不要在 `Resources.Load` 中硬编码路径字符串，必须使用 `ResPath` 工具类
- 不要新增 `.cs` 文件后忘记在 `Assembly-CSharp.csproj` 中添加 `<Compile Include>` 条目
