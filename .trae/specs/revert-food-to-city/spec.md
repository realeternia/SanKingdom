# 粮食回归城市级 & 势力新增马木铁属性 Spec

## Why
粮食（food）应作为城市级属性管理，每个城市独立拥有粮仓。而金钱（gold）仍保持势力级共享。同时，战斗中消耗的粮草为一次性消耗，战后不再返还。此外，势力需要新增马（horse）、木（wood）、铁（steel）三种战略资源，从 ForceConfig 初始化。

## What Changes
- **BREAKING**: 从 `SaveForceData` 移除 `food` 字段（保留 `gold`）
- **BREAKING**: 在 `SaveCityData` 新增 `food` 字段
- 在 `SaveForceData` 新增 `wood`、`horse`、`steel` 字段
- `SaveCityData.AddAttr` / `GetAttr` 方法新增 food 分支
- `SaveCityData.OnRound` 中 food 产出写回城市数据
- `SaveForceData` 中所有 `this.food` 引用改为通过 `cityData.food` 访问
- `GameManager.NewGame` 中城市 food 从 `WorldConfig.Food` 初始化，势力不再初始化 food
- `GameManager.NewGame` 中势力新增 `wood`、`horse`、`steel` 从 `ForceConfig.InitWood`、`InitHorse`、`InitSteel` 初始化
- AI 代码中 food 检查改为 `city.food`
- UI 代码中 food 显示改为城市级
- 战斗粮草仍为一次性消耗，OnBattleEnd 不返还

## Impact
- Affected specs: 存档系统、战斗系统、AI策略系统、UI显示
- Affected code:
  - `SaveCityData.cs` — 新增 food 字段、AddAttr/GetAttr 新增 food 分支、OnRound food 产出写回城市
  - `SaveForceData.cs` — 移除 food 字段、新增 wood/horse/steel 字段、所有 food 操作改为 cityData
  - `GameManager.cs` — 城市初始化 food 从 WorldConfig.Food；势力初始化 wood/horse/steel 从 ForceConfig
  - `AI.cs` — food 检查改为 city.food
  - `TaskPriorityCalculator.cs` — food 检查改为 city.food
  - `StrategicDecider.cs` — food 检查改为 city.food
  - `CityPanelManager.cs` — food 显示改为城市级
  - `CityDetail.cs` — food 显示改为城市级
  - `CityDevNodeBattle.cs` — food 检查改为 city.food
  - `CityDevNodeMove.cs` — food 检查改为 city.food

## ADDED Requirements

### Requirement: 城市级粮食属性
系统 SHALL 将 food 作为城市级属性管理，每个城市独立拥有粮仓。

#### Scenario: 城市独立粮食
- **WHEN** 城市产出 food
- **THEN** 产出累加到该城市自身的 food 中

#### Scenario: 扣除城市粮食
- **WHEN** 玩家从某城市出征或移动英雄
- **THEN** 从该城市的 food 中扣除粮草

#### Scenario: 检查城市粮食
- **WHEN** 系统检查 food 是否足够
- **THEN** 应检查城市级别的 food

#### Scenario: 初始化城市粮食
- **WHEN** 新游戏开始
- **THEN** 城市的 food 从 WorldConfig.Food 初始化

### Requirement: 战斗粮草一次性消耗
系统 SHALL 在战斗中将粮草视为一次性消耗，战斗结束后不再返还。

#### Scenario: 出征消耗粮草
- **WHEN** 玩家从某城市出征
- **THEN** 粮草从该城市的 food 中一次性扣除

#### Scenario: 战斗结束不返还粮草
- **WHEN** 战斗结束
- **THEN** 剩余粮草不再归还，粮草已作为出征成本消耗

### Requirement: 势力级马木铁资源
系统 SHALL 在 `SaveForceData` 中提供 `wood`（木）、`horse`（马）、`steel`（铁）三种势力级战略资源。

#### Scenario: 初始化势力马木铁
- **WHEN** 新游戏开始
- **THEN** 势力的 wood/horse/steel 从 ForceConfig.InitWood / InitHorse / InitSteel 初始化

#### Scenario: 马木铁作为势力共享资源
- **WHEN** 任一城市的操作产出或消耗马木铁
- **THEN** 应作用于该城市所属势力的 wood/horse/steel

## MODIFIED Requirements

### Requirement: SaveCityData 属性访问
`SaveCityData.AddAttr` 和 `GetAttr` 方法 SHALL：
- 新增对 "food" 的处理分支
- 保留对 "level"、"exp"、"soldier"、"happy"、"wall" 的处理

### Requirement: SaveForceData 数据结构
`SaveForceData` SHALL：
- 移除 `food` 字段，所有 food 操作通过 cityData 访问
- 新增 `wood`、`horse`、`steel` 字段

### Requirement: AI 策略使用城市级粮食
AI 策略系统 SHALL 使用城市级别的 food 进行决策。

#### Scenario: AI 检查粮食
- **WHEN** AI 评估是否可以执行某操作（出征、移动等）
- **THEN** 应检查城市级别的 food

### Requirement: UI 显示城市级粮食
城市面板和详情面板 SHALL 显示城市级别的 food。

## REMOVED Requirements

### Requirement: 势力级粮食资源
**Reason**: food 改回城市级属性，每个城市独立管理粮仓
**Migration**: 所有 forceData.food 访问改回通过 cityData.food 获取
