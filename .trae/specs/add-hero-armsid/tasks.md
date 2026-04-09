# Tasks

- [x] Task 1: 在 SaveHeroData 中添加 armsId 字段
  - [x] SubTask 1.1: 在 SaveHeroData.cs 中添加 public int armsId 字段
  - [x] SubTask 1.2: 在 CreateWildHero 方法中初始化 armsId = 601
  - [x] SubTask 1.3: 在 GameManager 初始化 SaveHeroData 时设置 armsId = 601

- [x] Task 2: 在 Chess 类中添加 armsId 字段
  - [x] SubTask 2.1: 在 Chess.cs 中添加 public int armsId 字段

- [x] Task 3: 修改 Chess.CreateChessView 方法
  - [x] SubTask 3.1: Hero 分支从 ArmsConfig 获取 hitEffect、missileSpeed、missileHeight、moveSpeed、attackRange
  - [x] SubTask 3.2: 移除从 HeroConfig 获取这些属性的代码

- [x] Task 4: 在 BattleCardData 中添加 armsId 字段
  - [x] SubTask 4.1: 添加 public int ArmsId 字段
  - [x] SubTask 4.2: 在 SaveCityData.GetBattleHeroList() 中从 SaveHeroData 获取 armsId（含默认值处理）

- [x] Task 5: 在 CreateChessAction 中添加 armsId 支持
  - [x] SubTask 5.1: 添加 public int ArmsId 字段
  - [x] SubTask 5.2: Hero 构造函数添加 armsId 参数
  - [x] SubTask 5.3: Doing() 方法中赋值 chessObj.armsId
  - [x] SubTask 5.4: BattleUnit 分支从 BattleUnitConfig.ArmsId 获取

- [x] Task 6: 在 BattleManager 中传递 armsId
  - [x] SubTask 6.1: SpawnHerosForRegion() 方法传递 heroData.ArmsId

- [x] Task 7: 删除 JobConfig 相关代码
  - [x] SubTask 7.1: 删除 JobConfig_s.cs 文件
  - [x] SubTask 7.2: 移除 ConfigManager 中 JobConfig 相关代码
  - [x] SubTask 7.3: 移除 Chess.Attack 中兵种克制逻辑
  - [x] SubTask 7.4: 移除 Tooltip 中 Job 相关代码
  - [x] SubTask 7.5: 移除 SkillManager 中 Job 相关代码

- [x] Task 8: 修复编译错误
  - [x] SubTask 8.1: 移除 BattleManager 中按 Range 排序的逻辑（后改为按 ArmsId 排序）
  - [x] SubTask 8.2: HeroSelectionTool.GetPrice() 使用 ArmsConfig.GetConfig(601).Range

- [x] Task 9: 验证 ArmsConfig 配置存在
  - [x] SubTask 9.1: 确认 ArmsConfig 中存在 id=601 的配置（刀兵）

# Task Dependencies
- Task 9 可与其他任务并行执行
- Task 3 依赖 Task 2
- Task 5 依赖 Task 4
- Task 6 依赖 Task 5
