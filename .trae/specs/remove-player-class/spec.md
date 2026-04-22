# 移除 Player 类重构 Spec

## Why
Player 类作为运行时对象与 SaveForceData 存在职责重叠，增加了代码复杂度。将 Player 类的逻辑合并到 SaveForceData 可以简化架构，减少数据冗余，使势力数据管理更加统一。

## What Changes
- **删除** `Player.cs` 文件
- **扩展** `SaveForceData` 类，添加运行时状态字段和方法
- **迁移** Player 类的所有方法到 SaveForceData
- **更新** 所有引用 Player 类的代码，改为使用 SaveForceData

## Impact
- Affected specs: 势力管理、回合系统、AI系统
- Affected code: 
  - `SaveForceData.cs` - 主要修改
  - `GameManager.cs` - 移除 Player 相关逻辑
  - `AI.cs` - 参数类型变更
  - `AIStrategyContext.cs` - 参数类型变更
  - `StrategicDecider.cs` - 参数类型变更
  - `HeroDispatcher.cs` - 参数类型变更
  - `SaveCityData.cs` - GetPlayer 方法返回类型变更
  - 其他 26 个引用 Player 的文件

## ADDED Requirements

### Requirement: SaveForceData 扩展字段
SaveForceData 类 SHALL 包含以下新增字段以支持运行时状态：
- `phase` (TurnPhase) - 当前回合阶段，使用 `[NonSerialized]` 标记
- `warPlans` (List<WarPlanData>) - 战争计划列表，使用 `[NonSerialized]` 标记
- `planConfirmed` (bool) - 计划是否确认，使用 `[NonSerialized]` 标记

### Requirement: SaveForceData 计算属性
SaveForceData 类 SHALL 提供以下计算属性，从配置动态获取：
- `Name` - 势力名称（从 HeroConfig 获取君主名称）
- `LineColor` - 势力颜色（从 ForceConfig 获取）
- `IconPath` - 势力图标路径（从 HeroConfig 获取君主图标）

### Requirement: SaveForceData 方法迁移
SaveForceData 类 SHALL 包含以下从 Player 类迁移的方法：
- `SetPhase(TurnPhase)` - 设置回合阶段
- `AddWarPlan(WarPlanData)` - 添加战争计划
- `ResetRoundState()` - 重置回合状态
- `StartPlanningPhase()` - 开始计划阶段
- `ExecuteCityDev(...)` - 执行城市发展
- `ExecuteCityBattleDev(...)` - 执行城市战斗发展
- `MoveHeroToCity(...)` - 移动英雄到城市
- `ExecuteCityMoveDev(...)` - 执行城市移动发展
- `GetCityList()` - 获取城市列表
- `GetKingCity()` - 获取王城
- `ExecuteCityChange(...)` - 执行城市变更
- `ExecuteCityUseHero(...)` - 执行登用英雄
- `ExecuteCityPraiseHero(...)` - 执行褒奖英雄

### Requirement: GameManager 简化
GameManager 类 SHALL 进行以下修改：
- 移除 `players` 列表字段
- 移除 `currentPlayer` 字段
- 移除 `GetPlayer(int forceId)` 方法
- 使用 `GetForce(int forceId)` 替代所有 `GetPlayer` 调用
- 修改回合管理逻辑，直接操作 SaveForceData

## MODIFIED Requirements

### Requirement: AI 系统接口变更
AI 系统的所有方法 SHALL 接收 `SaveForceData` 参数而非 `Player` 参数：
- `AI.ExecutePlanningPhase(SaveForceData)`
- `AIStrategyContext` 构造函数接收 `SaveForceData`
- `HeroDispatcher.DispatchHeroes(SaveForceData)`
- `StrategicDecider.DetermineCityStrategies(SaveForceData)`

### Requirement: SaveCityData 接口变更
`SaveCityData.GetPlayer()` 方法 SHALL 返回 `SaveForceData` 类型，并重命名为 `GetForce()`。

## REMOVED Requirements

### Requirement: Player 类
**Reason**: Player 类的功能已完全迁移到 SaveForceData，不再需要独立的 Player 类。
**Migration**: 所有 Player 实例替换为 SaveForceData 实例，所有 Player 方法调用替换为 SaveForceData 方法调用。
