# Tasks

- [x] Task 1: 在SaveCityData中添加委派数据存储字段
  - [x] SubTask 1.1: 添加 `List<DevAssignmentData> devAssignments` 字段存储heroId到devId的映射（使用List而非Dictionary以确保可序列化）
  - [x] SubTask 1.2: 创建可序列化的 `DevAssignmentData` 类，包含heroId和devId
  - [x] SubTask 1.3: 添加 `SetDevAssignment(int heroId, int devId)` 方法
  - [x] SubTask 1.4: 添加 `RemoveDevAssignment(int heroId)` 方法
  - [x] SubTask 1.5: 添加 `ClearDevAssignments()` 方法
  - [x] SubTask 1.6: 添加 `GetDevAssignments()` 方法获取所有委派映射

- [x] Task 2: 修改CityPanelManager实现委派数据读取和保存
  - [x] SubTask 2.1: 在 `CreateDevItems()` 中读取SaveCityData的委派数据初始化面板
  - [x] SubTask 2.2: 在 `AssignHeroToDevNode()` 中调用SaveCityData保存委派数据并触发存档
  - [x] SubTask 2.3: 在 `RemoveHeroFromDevNode()` 中调用SaveCityData移除委派数据
  - [x] SubTask 2.4: 在 `OnSelectCity()` 切换城市时重新加载委派数据

- [x] Task 3: 实现hero移动时清空委派记录
  - [x] SubTask 3.1: 在 `SaveCityData.MoveHeroTo()` 中移除移动hero的委派记录

- [x] Task 4: 实现hero被俘虏时清空委派记录
  - [x] SubTask 4.1: 在 `SaveCityData.Occupy()` 中，当hero.state设为Catched时，移除该hero的委派记录（通过ClearDevAssignments()在Occupy开始时清空所有委派）

- [x] Task 5: 实现俘虏逃跑时清空委派记录
  - [x] SubTask 5.1: 在 `GameManager.ProcessHeros()` 中，当俘虏成功逃跑时，从原城市移除该hero的委派记录

- [x] Task 6: 实现城市被攻占时清空委派队列
  - [x] SubTask 6.1: 在 `SaveCityData.Occupy()` 开始时调用 `ClearDevAssignments()`

# Task Dependencies
- Task 2 依赖 Task 1（需要SaveCityData中的方法）
- Task 3, 4, 5, 6 依赖 Task 1（需要SaveCityData中的方法）
- Task 3, 4, 5, 6 可以并行执行
