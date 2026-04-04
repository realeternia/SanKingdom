# Tasks

- [x] Task 1: 创建AI策略基础架构
  - [x] SubTask 1.1: 创建IAIStrategy策略接口，定义Execute方法签名
  - [x] SubTask 1.2: 创建AIStrategyContext上下文类，封装决策所需数据
  - [x] SubTask 1.3: 创建AIStrategyManager管理器类，提供策略实例获取

- [x] Task 2: 实现城市状态评估器
  - [x] SubTask 2.1: 创建CityEvaluator类，实现城市状态评估
  - [x] SubTask 2.2: 实现资源短缺检测方法（金钱、粮食）
  - [x] SubTask 2.3: 实现安全状态检测方法（治安、城墙）
  - [x] SubTask 2.4: 实现军事状态检测方法（士兵、士气）
  - [x] SubTask 2.5: 实现城市需求优先级列表生成

- [x] Task 3: 实现任务优先级计算器
  - [x] SubTask 3.1: 创建TaskPriorityCalculator类
  - [x] SubTask 3.2: 实现基础优先级读取（从CityDevConfig）
  - [x] SubTask 3.3: 实现状态加权调整逻辑
  - [x] SubTask 3.4: 实现最终优先级排序方法

- [x] Task 4: 实现英雄任务匹配器
  - [x] SubTask 4.1: 创建HeroTaskMatcher类
  - [x] SubTask 4.2: 实现英雄属性与任务需求匹配度计算
  - [x] SubTask 4.3: 实现单英雄最优任务选择
  - [x] SubTask 4.4: 实现多英雄全局优化分配算法

- [x] Task 5: 实现战略决策系统
  - [x] SubTask 5.1: 创建StrategicDecider类
  - [x] SubTask 5.2: 实现势力整体状态评估
  - [x] SubTask 5.3: 实现发展战略决策逻辑
  - [x] SubTask 5.4: 实现扩张战略决策逻辑（攻击目标选择）
  - [x] SubTask 5.5: 实现防御战略决策逻辑
  - [x] SubTask 5.6: 实现势力城市状态平衡策略（最多2城处于Def/Atk）

- [x] Task 6: 实现英雄调度系统
  - [x] SubTask 6.1: 创建HeroDispatcher类
  - [x] SubTask 6.2: 实现前线/后方城市识别（是否临近敌方）
  - [x] SubTask 6.3: 实现英雄类型分类（战斗型/内政型）
  - [x] SubTask 6.4: 实现前线英雄需求计算
  - [x] SubTask 6.5: 实现英雄调度执行（后方->前线移动）

- [x] Task 7: 实现具体策略类
  - [x] SubTask 7.1: 创建DevelopmentStrategy发展策略类
  - [x] SubTask 7.2: 创建ExpansionStrategy扩张策略类
  - [x] SubTask 7.3: 创建DefenseStrategy防御策略类

- [x] Task 8: 重构AI.cs主类
  - [x] SubTask 8.1: 移除旧的静态方法实现
  - [x] SubTask 8.2: 集成AIStrategyManager
  - [x] SubTask 8.3: 保持ExecuteAiActions方法签名兼容
  - [x] SubTask 8.4: 实现策略选择逻辑
  - [x] SubTask 8.5: 集成英雄调度系统

- [x] Task 9: 实现特殊任务处理器
  - [x] SubTask 9.1: 创建SpecialTaskHandler类
  - [x] SubTask 9.2: 实现登用英雄逻辑
  - [x] SubTask 9.3: 实现褒奖英雄逻辑
  - [x] SubTask 9.4: 实现搜索英雄逻辑

# Task Dependencies
- [Task 2] depends on [Task 1]
- [Task 3] depends on [Task 2]
- [Task 4] depends on [Task 2]
- [Task 5] depends on [Task 2]
- [Task 6] depends on [Task 2]
- [Task 7] depends on [Task 3, Task 4, Task 5]
- [Task 8] depends on [Task 6, Task 7]
- [Task 9] depends on [Task 8]
