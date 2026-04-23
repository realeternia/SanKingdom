# Tasks

- [x] Task 1: 修改 SaveCityData — 新增 food 字段，修改方法
  - [x] SubTask 1.1: 新增 `public float food;` 字段声明
  - [x] SubTask 1.2: 修改 `OnRound` 方法，food 产出写回城市数据（不再写入 forceData）
  - [x] SubTask 1.3: 修改 `AddAttr` 方法，新增 food 分支
  - [x] SubTask 1.4: 修改 `GetAttr` 方法，新增 food 分支

- [x] Task 2: 修改 SaveForceData — 移除 food，新增 wood/horse/steel，food 操作改为 cityData
  - [x] SubTask 2.1: 移除 `public float food;` 字段声明
  - [x] SubTask 2.2: 新增 `public float wood;`、`public float horse;`、`public float steel;` 字段声明
  - [x] SubTask 2.3: ExecuteCityBattleDev 中 food 操作改为 citySrc.food / cityDest.food
  - [x] SubTask 2.4: ExecuteCityMoveDev 中 food 移动使用 citySrc.food / cityDest.food
  - [x] SubTask 2.5: ExecuteCityChange 中所有 `food` 引用改为 `cityData.food`

- [x] Task 3: 修改 GameManager.cs — 初始化逻辑
  - [x] SubTask 3.1: NewGame 中城市新增 `city.food = cityCfg.Food;` 初始化
  - [x] SubTask 3.2: NewGame 中 SaveForceData 初始化移除 food，新增 wood/horse/steel

- [x] Task 4: 修改 AI 相关代码 — food 检查改为 city.food
  - [x] SubTask 4.1: AI.cs TryCreateWarPlan 中 food 检查改为 city.food
  - [x] SubTask 4.2: TaskPriorityCalculator.AdjustPriorityByNeeds 中 food 检查改为 city.food
  - [x] SubTask 4.3: StrategicDecider.SelectAttackTargetsByOwnCity 中 food 检查改为 city.food
  - [x] SubTask 4.4: StrategicDecider.CanExpand 中 totalFood 改为遍历城市汇总

- [x] Task 5: 修改 UI 代码 — food 显示改为城市级
  - [x] SubTask 5.1: CityPanelManager.UpdateCityAttrText 中 food 改为城市级
  - [x] SubTask 5.2: CityDetail.SetCityDetail 中 food 改为城市级
  - [x] SubTask 5.3: CityDevNodeBattle 中 food 检查改为 city.food
  - [x] SubTask 5.4: CityDevNodeMove 中 food 检查改为 city.food
