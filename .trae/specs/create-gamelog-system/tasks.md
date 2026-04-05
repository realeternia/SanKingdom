# Tasks

- [x] Task 1: 创建 GameLog 核心类
  - [x] SubTask 1.1: 创建 `Controls/Utils/GameLog.cs` 文件
  - [x] SubTask 1.2: 实现日志级别枚举 (Debug, Info, Warn, Error)
  - [x] SubTask 1.3: 实现基本日志方法 (Debug, Info, Warn, Error)
  - [x] SubTask 1.4: 封装 UnityEngine.Debug 接口调用

- [x] Task 2: 实现日志文件写入功能
  - [x] SubTask 2.1: 实现日志文件初始化，创建日志目录
  - [x] SubTask 2.2: 实现按日期自动轮转功能
  - [x] SubTask 2.3: 实现日志格式化输出 `[时间戳][级别][标签] 消息`
  - [x] SubTask 2.4: 实现文件写入缓冲机制

- [x] Task 3: 实现标签分类输出功能
  - [x] SubTask 3.1: 实现 SetTag 方法返回带标签的日志实例
  - [x] SubTask 3.2: 实现标签日志同时写入主日志文件和标签文件
  - [x] SubTask 3.3: 标签文件命名为 `log.{tag}`

- [x] Task 4: 迁移 Controls/AI 目录下的日志调用 (11处)
  - [x] SubTask 4.1: 迁移 AI.cs 中的 9 处日志调用
  - [x] SubTask 4.2: 迁移 StrategicDecider.cs 中的 1 处日志调用
  - [x] SubTask 4.3: 迁移 HeroDispatcher.cs 中的 1 处日志调用

- [x] Task 5: 迁移 Controls 目录下的日志调用 (9处)
  - [x] SubTask 5.1: 迁移 BattleManager.cs 中的 6 处日志调用
  - [x] SubTask 5.2: 迁移 GameManager.cs 中的 1 处日志调用
  - [x] SubTask 5.3: 迁移 BattleUIManager.cs 中的 1 处日志调用
  - [x] SubTask 5.4: 迁移 SelectHeroControl.cs 中的 1 处日志调用

- [x] Task 6: 迁移 Combat 目录下的日志调用 (16处)
  - [x] SubTask 6.1: 迁移 BuffShield.cs 中的 3 处日志调用
  - [x] SubTask 6.2: 迁移 EffectManager.cs 中的 2 处日志调用
  - [x] SubTask 6.3: 迁移 ChessViewObj.cs 中的 2 处日志调用
  - [x] SubTask 6.4: 迁移 BuffManager.cs 中的 2 处日志调用
  - [x] SubTask 6.5: 迁移 Buff.cs 中的 2 处日志调用
  - [x] SubTask 6.6: 迁移其他 Combat 文件中的 5 处日志调用

- [x] Task 7: 迁移其他目录下的日志调用 (14处)
  - [x] SubTask 7.1: 迁移 SaveCityData.cs 中的 3 处日志调用
  - [x] SubTask 7.2: 迁移其他文件中的 11 处日志调用

# Task Dependencies
- Task 2 依赖 Task 1
- Task 3 依赖 Task 2
- Task 4-7 依赖 Task 3
- Task 4, 5, 6, 7 可并行执行
