# Tasks

- [x] Task 1: 移除玩家侧主将与委派互斥逻辑
  - [x] SubTask 1.1: 移除 CityTroopsItem.OnHeroDropped 中两处 `RemoveHeroFromDev` 调用（创建模式 slotIndex==0 和编辑模式 slotIndex==0）
  - [x] SubTask 1.2: 移除 CityPanelManager.AssignHeroToDevNode 中的 `IsCommander` 校验（第502-506行）
- [x] Task 2: 移除AI侧组建军团时对已委派英雄的排除逻辑
  - [x] SubTask 2.1: 修改 AI.FormCityTroops 中 `assignedHeroIds` 的构建逻辑，不再将 devAssignments 中的英雄加入排除集合
- [x] Task 3: 修改英雄工作状态图标显示逻辑
  - [x] SubTask 3.1: 修改 CityCellHero.UpdateWorkState，支持主将和委派图标同时显示（job1显示主将图标，job2显示委派图标）

# Task Dependencies
- Task 1 和 Task 2 相互独立，可并行执行
- Task 3 依赖 Task 1 和 Task 2（需要先解除互斥限制，图标双显才有意义）
