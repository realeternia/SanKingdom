# 城市与势力数据重构 Spec

## Why
当前 gold（金钱）和 food（粮食）作为城市级资源管理，但设计上它们应是势力级共享资源（一个势力的所有城市共享金库和粮仓）。同时 power（士气）属性不再需要，而需要新增 happy（民心）属性来衡量城市治理状况。

## What Changes
- **BREAKING**: 从 `SaveCityData` 移除 `gold`、`food`、`power` 字段
- **BREAKING**: 在 `SaveForceData` 新增 `gold`、`food` 字段
- 在 `SaveCityData` 新增 `happy`（民心）字段
- 所有引用 `cityData.gold` 的代码改为通过 `forceData.gold` 访问
- 所有引用 `cityData.food` 的代码改为通过 `forceData.food` 访问
- 所有引用 `cityData.power` 的代码移除或替换为 `happy`
- `SaveCityData.AddAttr` / `GetAttr` 方法移除 gold/food/power 分支，新增 happy 分支
- `SaveCityData.OnRound` 中 gold/food 产出改为写入势力数据
- `CityAttrConfig_s.cs` 已完成：新增 happy 配置项（id=7），已删除 power 配置项
- `CityNeedType` 枚举移除 `PowerLow`，新增 `HappyLow`
- 战斗系统（BattleBegin）的 food 参数改为从势力数据获取
- UI 层显示 gold/food 改为势力级别
- `ForceConfig` 已有 `InitGold` 和 `InitFood` 字段，`WorldConfig` 已无 `Gold`/`Food` 字段
- `OnBattleEnd` 中 food 不再返还，改为一次性消耗

## Impact
- Affected specs: 存档系统、战斗系统、AI策略系统、UI显示
- Affected code:
  - `SaveCityData.cs` — 移除字段、新增字段、修改方法
  - `SaveForceData.cs` — 新增字段
  - `GameManager.cs` — 初始化逻辑（gold/food 从 ForceConfig 获取）
  - `Player.cs` — 所有 gold/food 操作
  - `BattleManager.cs` — 战斗初始化 food 来源
  - `AI.cs` — AI 战争计划 food 检查
  - `TaskPriorityCalculator.cs` — 任务可用性检查
  - `StrategicDecider.cs` — 战略决策 food 检查
  - `CityEvaluator.cs` — 城市评估
  - `CityPanelManager.cs` — UI 显示
  - `CityDetail.cs` — UI 显示
  - `CityDevNodeBattle.cs` — 出征 food 检查
  - `CityDevNodeMove.cs` — 移动 food 检查
  - `SystemConst.cs` — 常量调整
  - `CityAttrConfig_s.cs` — ✅ 已完成（happy 已添加，power 已删除）

## ADDED Requirements

### Requirement: 势力级金粮资源
系统 SHALL 将 gold 和 food 作为势力级资源管理，而非城市级资源。

#### Scenario: 势力共享金粮
- **WHEN** 任一城市产出 gold 或 food
- **THEN** 产出应累加到该城市所属势力的 gold/food 中

#### Scenario: 扣除金粮
- **WHEN** 玩家执行消耗 gold 或 food 的操作（发展、出征、褒奖、交易等）
- **THEN** 从该城市所属势力的 gold/food 中扣除

#### Scenario: 检查金粮是否充足
- **WHEN** 系统检查 gold/food 是否足够
- **THEN** 应检查势力级别的 gold/food，而非城市级别

#### Scenario: 初始化势力金粮
- **WHEN** 新游戏开始
- **THEN** 势力的 gold/food 从 ForceConfig.InitGold / ForceConfig.InitFood 初始化

### Requirement: 城市民心属性
系统 SHALL 在 `SaveCityData` 中提供 `happy`（民心）属性。

#### Scenario: 民心初始化
- **WHEN** 新游戏开始
- **THEN** 每个城市的 `happy` 应初始化为 SystemConst.City.INITIAL_CITY_HAPPY

#### Scenario: 民心通过发展任务增减
- **WHEN** 玩家执行影响民心的城市发展任务
- **THEN** `happy` 属性应相应增减

#### Scenario: 民心通过 GetAttr/AddAttr 访问
- **WHEN** 代码通过 `GetAttr("happy")` 或 `AddAttr("happy", value)` 访问
- **THEN** 应正确返回或修改 happy 值

### Requirement: 战斗粮草一次性消耗
系统 SHALL 在战斗中将粮草视为一次性消耗，战斗结束后不再返还。

#### Scenario: 出征消耗粮草
- **WHEN** 玩家从某城市出征
- **THEN** 粮草从该城市所属势力的 food 中一次性扣除

#### Scenario: 战斗结束不返还粮草
- **WHEN** 战斗结束
- **THEN** 剩余粮草不再归还到势力的 food 中，粮草已作为出征成本消耗

## MODIFIED Requirements

### Requirement: SaveCityData 属性访问
`SaveCityData.AddAttr` 和 `GetAttr` 方法 SHALL：
- 移除对 "gold"、"food"、"power" 的处理分支
- 新增对 "happy" 的处理分支
- 保留对 "level"、"exp"、"soldier"、"wall" 的处理

### Requirement: 战斗系统 food 来源
`BattleManager.BattleBegin` 的 food 参数 SHALL 从势力数据获取而非城市数据。

#### Scenario: 出征时携带粮草
- **WHEN** 玩家从某城市出征
- **THEN** 粮草从该城市所属势力的 food 中扣除

#### Scenario: 战斗结束后不返还粮草
- **WHEN** 战斗结束
- **THEN** 粮草已作为一次性消耗，不再归还

### Requirement: AI 策略使用势力级金粮
AI 策略系统 SHALL 使用势力级别的 gold/food 进行决策。

#### Scenario: AI 检查金粮
- **WHEN** AI 评估是否可以执行某操作（发展、出征、交易等）
- **THEN** 应检查势力级别的 gold/food

### Requirement: 城市评估移除 power 新增 happy
`CityEvaluator` SHALL 移除 `PowerLow` 评估，新增 `HappyLow` 评估。

### Requirement: UI 显示势力级金粮
城市面板和详情面板 SHALL 显示势力级别的 gold/food 而非城市级别。

## REMOVED Requirements

### Requirement: 城市级金粮资源
**Reason**: gold 和 food 升级为势力级共享资源
**Migration**: 所有 cityData.gold/food 访问改为通过 forceData 获取

### Requirement: 城市士气（power）属性
**Reason**: power 属性不再需要，由 happy（民心）替代
**Migration**: 移除所有 power 相关代码和配置，新增 happy 相关代码和配置

### Requirement: 战斗结束返还粮草
**Reason**: 粮草改为一次性消耗，不再返还
**Migration**: OnBattleEnd 中移除 food 归还逻辑
