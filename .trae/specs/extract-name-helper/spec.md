# 提取名称工具类 Spec

## Why
项目中存在多处重复的名称获取代码（获取势力名、英雄名、城市名），分布在 `AI.cs`、`BattleManager.cs`、`StrategicDecider.cs` 等文件中。这些重复代码违反了 DRY 原则，增加了维护成本，且容易导致不一致。

## What Changes
- 新建 `ConfigNameHelper.cs` 工具类，统一管理配置表名称获取逻辑
- 包含方法：`GetForceName(int forceId)`、`GetHeroName(int heroId)`、`GetCityName(int cityId)`、`GetHeroNames(int[] heroIds)`
- 重构 `AI.cs`、`BattleManager.cs`、`StrategicDecider.cs`，移除重复的私有方法，改用工具类

## Impact
- Affected specs: 无
- Affected code: 
  - `AI.cs` - 移除 `GetForceName`、`GetHeroName`、`GetCityName`、`GetHeroNames` 方法
  - `BattleManager.cs` - 移除 `GetForceName`、`GetHeroName`、`GetCityName` 方法
  - `StrategicDecider.cs` - 移除 `GetForceName`、`GetCityName` 方法

## ADDED Requirements

### Requirement: 名称获取工具类
系统应提供统一的配置表名称获取工具类 `ConfigNameHelper`，用于获取势力、英雄、城市等实体的显示名称。

#### Scenario: 获取势力名称
- **WHEN** 调用 `ConfigNameHelper.GetForceName(forceId)` 时
- **THEN** 返回对应势力的中文名称，若配置不存在则返回 ID 的字符串形式

#### Scenario: 获取英雄名称
- **WHEN** 调用 `ConfigNameHelper.GetHeroName(heroId)` 时
- **THEN** 返回对应英雄的中文名称，若配置不存在则返回 ID 的字符串形式

#### Scenario: 获取城市名称
- **WHEN** 调用 `ConfigNameHelper.GetCityName(cityId)` 时
- **THEN** 返回对应城市的中文名称，若配置不存在则返回 ID 的字符串形式

#### Scenario: 批量获取英雄名称
- **WHEN** 调用 `ConfigNameHelper.GetHeroNames(heroIds)` 时
- **THEN** 返回英雄名称的逗号分隔字符串

## MODIFIED Requirements
无

## REMOVED Requirements
无
