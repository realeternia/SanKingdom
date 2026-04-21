# Tasks

- [x] Task 1: 创建 ConfigNameHelper 工具类
  - [x] SubTask 1.1: 在 `SystemTool/` 目录下创建 `ConfigNameHelper.cs` 文件
  - [x] SubTask 1.2: 实现 `GetForceName(int forceId)` 静态方法
  - [x] SubTask 1.3: 实现 `GetHeroName(int heroId)` 静态方法
  - [x] SubTask 1.4: 实现 `GetCityName(int cityId)` 静态方法
  - [x] SubTask 1.5: 实现 `GetHeroNames(int[] heroIds)` 静态方法

- [x] Task 2: 重构 AI.cs 使用工具类
  - [x] SubTask 2.1: 移除 `AI.cs` 中的 `GetForceName` 私有方法
  - [x] SubTask 2.2: 移除 `AI.cs` 中的 `GetHeroName` 私有方法
  - [x] SubTask 2.3: 移除 `AI.cs` 中的 `GetCityName` 私有方法
  - [x] SubTask 2.4: 移除 `AI.cs` 中的 `GetHeroNames` 私有方法
  - [x] SubTask 2.5: 更新调用处使用 `ConfigNameHelper` 的方法

- [x] Task 3: 重构 BattleManager.cs 使用工具类
  - [x] SubTask 3.1: 移除 `BattleManager.cs` 中的 `GetForceName` 私有方法
  - [x] SubTask 3.2: 移除 `BattleManager.cs` 中的 `GetHeroName` 私有方法
  - [x] SubTask 3.3: 移除 `BattleManager.cs` 中的 `GetCityName` 私有方法
  - [x] SubTask 3.4: 更新调用处使用 `ConfigNameHelper` 的方法

- [x] Task 4: 重构 StrategicDecider.cs 使用工具类
  - [x] SubTask 4.1: 移除 `StrategicDecider.cs` 中的 `GetForceName` 私有方法
  - [x] SubTask 4.2: 移除 `StrategicDecider.cs` 中的 `GetCityName` 私有方法
  - [x] SubTask 4.3: 更新调用处使用 `ConfigNameHelper` 的方法

# Task Dependencies
- Task 2, Task 3, Task 4 都依赖于 Task 1 完成
- Task 2, Task 3, Task 4 可以并行执行
