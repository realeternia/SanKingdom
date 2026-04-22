# Tasks

- [x] Task 1: 修改 SaveForceData — 新增 gold 和 food 字段
  - [x] SubTask 1.1: 在 SaveForceData 中添加 `public float gold;` 和 `public float food;` 字段

- [x] Task 2: 修改 SaveCityData — 移除 gold/food/power，新增 happy，修改方法
  - [x] SubTask 2.1: 移除 `gold`、`food`、`power` 字段声明
  - [x] SubTask 2.2: 新增 `public float happy;` 字段声明
  - [x] SubTask 2.3: 修改 `OnRound` 方法，gold/food 产出改为写入势力数据
  - [x] SubTask 2.4: 修改 `AddAttr` 方法，移除 gold/food/power 分支，新增 happy 分支
  - [x] SubTask 2.5: 修改 `GetAttr` 方法，移除 gold/food/power 分支，新增 happy 分支

- [x] Task 3: 修改 CityAttrConfig_s.cs — 配置项调整（已完成）

- [x] Task 4: 修改 CityNeedType 和 CityEvaluator — 移除 PowerLow，新增 HappyLow
  - [x] SubTask 4.1: CityNeedType 枚举中移除 PowerLow，新增 HappyLow
  - [x] SubTask 4.2: CityEvaluator.EvaluateCity 中移除 power 评估，新增 happy 评估
  - [x] SubTask 4.3: CityEvaluator 中将 POWER_ALERT 改为 HAPPY_ALERT

- [x] Task 5: 修改 GameManager.cs — 初始化逻辑
  - [x] SubTask 5.1: NewGame 中移除 `city.gold`、`city.food`、`city.power` 赋值
  - [x] SubTask 5.2: NewGame 中新增 `city.happy` 赋值
  - [x] SubTask 5.3: NewGame 中为 SaveForceData 初始化 gold 和 food（从 ForceConfig 获取）

- [x] Task 6: 修改 Player.cs — 所有 gold/food 操作改为势力级
  - [x] SubTask 6.1: ExecuteCityDev 中 gold 检查和扣除改为通过 forceData
  - [x] SubTask 6.2: ExecuteCityBattleDev 中 food 扣除改为通过 forceData
  - [x] SubTask 6.3: OnBattleEnd 中移除 food 归还逻辑
  - [x] SubTask 6.4: ExecuteCityChange 中 gold/food 交易改为通过 forceData
  - [x] SubTask 6.5: ExecuteCityPraiseHero 中 gold 扣除改为通过 forceData
  - [x] SubTask 6.6: ExecuteCityMoveDev 中 food 移动逻辑改为势力级

- [x] Task 7: 修改 AI 相关代码 — 使用势力级 gold/food
  - [x] SubTask 7.1: AI.cs TryCreateWarPlan 中 food 检查改为 forceData.food
  - [x] SubTask 7.2: TaskPriorityCalculator.IsTaskAvailable 中 gold 检查改为 forceData.gold
  - [x] SubTask 7.3: TaskPriorityCalculator.AdjustPriorityByNeeds 中 food 检查改为 forceData.food
  - [x] SubTask 7.4: TaskPriorityCalculator.TaskMatchesNeed 中 PowerLow 改为 HappyLow
  - [x] SubTask 7.5: StrategicDecider.SelectAttackTargetsByOwnCity 中 food 检查改为 forceData.food
  - [x] SubTask 7.6: StrategicDecider.CanExpand 中 gold/food 改为 forceData 级别

- [x] Task 8: 修改 UI 代码 — 显示势力级 gold/food，新增 happy
  - [x] SubTask 8.1: CityPanelManager.UpdateCityAttrText 中 gold/food 改为势力级
  - [x] SubTask 8.2: CityDetail.SetCityDetail 中 gold/food 改为势力级，power 改为 happy
  - [x] SubTask 8.3: CityDevNodeBattle 中 food 检查改为 forceData.food
  - [x] SubTask 8.4: CityDevNodeMove 中 food 检查改为 forceData.food

- [x] Task 9: 修改 SystemConst — 常量调整
  - [x] SubTask 9.1: 移除 SystemConst.City.INITIAL_CITY_POWER
  - [x] SubTask 9.2: 新增 SystemConst.City.INITIAL_CITY_HAPPY 常量
  - [x] SubTask 9.3: SystemConst.AICity 中 POWER_ALERT 改为 HAPPY_ALERT
