# AI策略体系重构 Spec

## Why
当前AI系统过于简单，仅随机分配发展任务，缺乏智能决策能力。需要实现一套完整的AI策略体系，使AI能够根据游戏状态做出合理决策，提升游戏挑战性和可玩性。

## What Changes
- 重构AI.cs为面向对象的策略系统架构
- 实现城市状态评估机制
- 实现基于优先级的任务决策系统
- 实现英雄-任务最优匹配算法
- 实现战略决策系统（发展/攻击/防御）
- **BREAKING**: AI类从静态类改为实例化策略模式

## Impact
- Affected specs: AI决策系统、游戏难度平衡
- Affected code: 
  - [AI.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Controls/AI.cs)
  - [Player.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Controls/Player.cs) (调用方)
  - [GameManager.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Controls/GameManager.cs) (调用方)

## ADDED Requirements

### Requirement: AI策略架构
系统应提供模块化的AI策略架构，支持不同类型的AI行为。

#### Scenario: 策略接口定义
- **WHEN** 系统初始化AI模块
- **THEN** 应提供统一的策略接口，包含执行决策的方法签名

#### Scenario: 策略上下文
- **WHEN** AI执行决策
- **THEN** 应能访问当前玩家、城市、英雄等上下文信息

### Requirement: 城市状态评估
系统应能评估城市当前状态并识别需求优先级。

#### Scenario: 资源短缺评估
- **WHEN** 城市金钱低于告警值(500)
- **THEN** 应提高商业发展优先级
- **WHEN** 城市粮食低于告警值(500)
- **THEN** 应提高农业发展优先级

#### Scenario: 安全状态评估
- **WHEN** 城市治安低于告警值(60)
- **THEN** 应提高巡逻任务优先级
- **WHEN** 城市城墙低于告警值(150)
- **THEN** 应提高加固城墙优先级

#### Scenario: 军事状态评估
- **WHEN** 城市士兵数量低于告警值(500)
- **THEN** 应提高征兵优先级
- **WHEN** 城市士气低于告警值(50)
- **THEN** 应提高训练优先级

### Requirement: 任务优先级决策
系统应根据城市状态和配置计算任务优先级。

#### Scenario: 发展期优先级
- **WHEN** 城市处于和平状态（无临近敌军）
- **THEN** 使用CityDevConfig.AiPriotyDev作为基础优先级
- **AND** 根据城市状态调整优先级权重

#### Scenario: 战争期优先级
- **WHEN** 城市面临攻击威胁
- **THEN** 使用CityDevConfig.AiPriotyDef作为基础优先级
- **AND** 提高防御相关任务优先级

#### Scenario: 攻击期优先级
- **WHEN** AI决定发起攻击
- **THEN** 使用CityDevConfig.AiPriotyAtk作为基础优先级
- **AND** 提高军事相关任务优先级

### Requirement: 英雄任务匹配
系统应根据英雄属性匹配最优任务。

#### Scenario: 属性匹配计算
- **WHEN** 为英雄分配任务
- **THEN** 应计算英雄属性与任务需求属性的匹配度
- **AND** 优先分配匹配度高的任务

#### Scenario: 多英雄分配优化
- **WHEN** 城市有多个可用英雄
- **THEN** 应优化整体分配方案，使总效益最大化

### Requirement: 势力城市状态平衡
系统应平衡势力内各城市的状态分配，确保大部分城市处于发展状态。

#### Scenario: 城市状态分配比例
- **WHEN** 势力有N个城市
- **THEN** 应保证至少(N-2)个城市处于发展(Dev)状态
- **AND** 最多2个城市处于防御(Def)或攻击(Atk)状态

#### Scenario: 攻击限制策略
- **WHEN** 势力已有2个城市处于Def或Atk状态
- **THEN** 不再发起新的攻击行动
- **AND** 全力进行防守和发展

### Requirement: 英雄调度系统
系统应根据城市战略位置和英雄类型进行英雄调度。

#### Scenario: 前线城市英雄配置
- **WHEN** 城市被识别为前线（临近敌方城市）
- **THEN** 应优先配置高战斗属性（武力、统帅）的英雄
- **AND** 保持较多英雄数量以应对战斗

#### Scenario: 后方城市英雄配置
- **WHEN** 城市被识别为后方（无临近敌方城市）
- **THEN** 应配置内政型（智力、政治、魅力）英雄
- **AND** 保持较少英雄数量，将多余战斗英雄调往前线

#### Scenario: 英雄调度执行
- **WHEN** 系统检测到前线城市战斗英雄不足
- **THEN** 应从后方城市调度战斗英雄前往前线
- **WHEN** 系统检测到后方城市战斗英雄过多
- **THEN** 应将战斗英雄调度至前线城市

### Requirement: 战略决策系统
系统应根据势力整体状态制定战略方向。

#### Scenario: 发展战略
- **WHEN** 势力城市数量较少（<=2）且无紧迫威胁
- **THEN** 采取发展战略，优先发展经济和军事

#### Scenario: 扩张战略
- **WHEN** 势力资源充足且军力优势明显
- **AND** 当前处于Def/Atk状态的城市少于2个
- **THEN** 采取扩张战略，寻找攻击机会

#### Scenario: 防御战略
- **WHEN** 势力面临多方威胁
- **OR** 已有2个城市处于Def/Atk状态
- **THEN** 采取防御战略，加强城防和兵力集中

### Requirement: 特殊任务处理
系统应正确处理特殊类型的任务。

#### Scenario: 登用英雄
- **WHEN** 城市存在可登用的在野/俘虏英雄
- **THEN** 应评估登用价值并尝试登用

#### Scenario: 褒奖英雄
- **WHEN** 存在忠诚度低于80的英雄
- **THEN** 应安排褒奖任务提升忠诚度

#### Scenario: 搜索英雄
- **WHEN** 城市英雄数量不足
- **THEN** 应安排搜索任务发现新英雄

## MODIFIED Requirements

### Requirement: AI执行入口
原静态方法ExecuteAiActions改为策略模式调用。

#### Scenario: 兼容性调用
- **WHEN** GameManager调用AI执行
- **THEN** 应通过AIStrategyManager获取策略实例并执行

## REMOVED Requirements

### Requirement: 随机任务分配
**Reason**: 随机分配不符合智能AI的设计目标
**Migration**: 使用基于优先级的决策系统替代随机选择
