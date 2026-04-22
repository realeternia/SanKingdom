# Tasks

- [x] Task 1: 扩展 SaveForceData 类
  - [x] SubTask 1.1: 添加运行时状态字段（phase, warPlans, planConfirmed），使用 [NonSerialized] 标记
  - [x] SubTask 1.2: 添加计算属性（Name, LineColor, IconPath）
  - [x] SubTask 1.3: 添加初始化方法 `InitRuntimeState()` 用于初始化运行时数据
  - [x] SubTask 1.4: 迁移 Player 类的所有方法到 SaveForceData

- [x] Task 2: 更新 GameManager 类
  - [x] SubTask 2.1: 移除 `players` 列表和 `currentPlayer` 字段
  - [x] SubTask 2.2: 移除 `GetPlayer(int forceId)` 方法
  - [x] SubTask 2.3: 修改 `NewGame()` 方法，初始化 SaveForceData 运行时状态
  - [x] SubTask 2.4: 修改 `LoadFromSave()` 方法，初始化 SaveForceData 运行时状态
  - [x] SubTask 2.5: 修改回合管理逻辑，使用 `currentForceId` 替代 `currentPlayer`
  - [x] SubTask 2.6: 修改 `GetRandomForceCityId()` 方法，使用 SaveForceData

- [x] Task 3: 更新 SaveCityData 类
  - [x] SubTask 3.1: 修改 `GetPlayer()` 方法为 `GetForce()`，返回 SaveForceData
  - [x] SubTask 3.2: 更新 `Occupy()` 方法中的 Player 引用

- [x] Task 4: 更新 AI 系统文件
  - [x] SubTask 4.1: 更新 `AI.cs`，将 Player 参数改为 SaveForceData
  - [x] SubTask 4.2: 更新 `AIStrategyContext.cs`，将 Player 参数改为 SaveForceData
  - [x] SubTask 4.3: 更新 `StrategicDecider.cs`，将 Player 参数改为 SaveForceData
  - [x] SubTask 4.4: 更新 `HeroDispatcher.cs`，将 Player 参数改为 SaveForceData

- [x] Task 5: 更新其他引用 Player 的文件
  - [x] SubTask 5.1: 更新 `BattleManager.cs`
  - [x] SubTask 5.2: 更新 `BattleUIManager.cs`
  - [x] SubTask 5.3: 更新 `PanelManager.cs`
  - [x] SubTask 5.4: 更新 UI 相关文件（CityPanelManager, MainPanelManager 等）

- [x] Task 6: 删除 Player.cs 文件

- [x] Task 7: 验证编译和功能
  - [x] SubTask 7.1: 检查编译错误
  - [x] SubTask 7.2: 验证游戏启动和基本功能

# Task Dependencies
- [Task 2] depends on [Task 1]
- [Task 3] depends on [Task 1]
- [Task 4] depends on [Task 1]
- [Task 5] depends on [Task 1, Task 2, Task 3, Task 4]
- [Task 6] depends on [Task 5]
- [Task 7] depends on [Task 6]
