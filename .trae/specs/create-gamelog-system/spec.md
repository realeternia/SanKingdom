# GameLog 日志系统 Spec

## Why
项目中有 50 处 `UnityEngine.Debug` 日志调用分散在 28 个文件中，缺乏统一的日志管理。需要创建一个统一的日志系统，支持日志级别、按日期轮转、标签分类输出等功能。

## What Changes
- 在 `Controls/Utils` 目录下创建 `GameLog.cs` 日志类
- 支持 Debug、Info、Warn、Error 四种日志级别
- 封装 `UnityEngine.Debug` 接口调用
- 实现日志文件写入，按日期自动轮转
- 支持 `SetTag()` 方法实现标签分类输出
- 迁移所有现有的 `UnityEngine.Debug` 调用到新接口

## Impact
- Affected specs: 日志系统
- Affected code: 28 个文件中的 50 处日志调用
  - Controls/AI/AI.cs (9处)
  - Controls/BattleManager.cs (6处)
  - Controls/AI/StrategicDecider.cs (1处)
  - Controls/GameManager.cs (1处)
  - Controls/AI/HeroDispatcher.cs (1处)
  - Controls/BattleUIManager.cs (1处)
  - 其他 22 个文件

## ADDED Requirements

### Requirement: GameLog 核心功能
系统应提供统一的日志接口，支持多种日志级别和文件输出。

#### Scenario: 基本日志输出
- **WHEN** 调用 `GameLog.Debug(message)` 或 `GameLog.Info(message)` 或 `GameLog.Warn(message)` 或 `GameLog.Error(message)`
- **THEN** 日志同时输出到 Unity Console 和日志文件

#### Scenario: 日志文件轮转
- **WHEN** 日志文件日期变更（跨天）
- **THEN** 自动创建新的日志文件，文件名包含日期

### Requirement: 标签分类输出
系统应支持通过标签将日志输出到特定文件。

#### Scenario: 标签日志输出
- **WHEN** 调用 `GameLog.SetTag("AI").Info(message)`
- **THEN** 日志输出到主日志文件和 `log.ai` 文件

#### Scenario: 多标签支持
- **WHEN** 使用不同标签调用日志
- **THEN** 每个标签的日志输出到对应的标签文件

### Requirement: 日志格式
系统应提供统一的日志格式。

#### Scenario: 日志格式化
- **WHEN** 输出日志
- **THEN** 格式为 `[时间戳][级别][标签] 消息内容`

### Requirement: 日志级别控制
系统应支持通过日志级别控制输出。

#### Scenario: 级别过滤
- **WHEN** 设置最低日志级别
- **THEN** 低于该级别的日志不被输出

## MODIFIED Requirements

### Requirement: 现有日志迁移
所有现有的 `UnityEngine.Debug` 调用应迁移到新的 GameLog 接口。

#### Scenario: Log 迁移
- **WHEN** 原代码使用 `UnityEngine.Debug.Log(message)`
- **THEN** 迁移为 `GameLog.Info(message)`

#### Scenario: LogWarning 迁移
- **WHEN** 原代码使用 `UnityEngine.Debug.LogWarning(message)`
- **THEN** 迁移为 `GameLog.Warn(message)`

#### Scenario: LogError 迁移
- **WHEN** 原代码使用 `UnityEngine.Debug.LogError(message)`
- **THEN** 迁移为 `GameLog.Error(message)`

## REMOVED Requirements
无
