# Checklist

## 数据结构变更
- [x] SaveCityData 包含 `food` 字段
- [x] SaveForceData 不包含 `food` 字段（保留 `gold`）
- [x] SaveForceData 包含 `wood`、`horse`、`steel` 字段
- [x] SaveCityData.AddAttr 新增 food 分支
- [x] SaveCityData.GetAttr 新增 food 分支
- [x] SaveCityData.OnRound 中 food 产出写回城市数据

## 初始化逻辑
- [x] GameManager.NewGame 中城市初始化 food（从 WorldConfig.Food）
- [x] GameManager.NewGame 中势力初始化 wood/horse/steel（从 ForceConfig.InitWood/InitHorse/InitSteel）
- [x] GameManager.NewGame 中势力不再初始化 food

## 业务逻辑
- [x] SaveForceData.ExecuteCityBattleDev 中 food 扣除使用 citySrc.food
- [x] SaveForceData.ExecuteCityBattleDev 中 defenceFood 从 cityDest.food 获取
- [x] SaveForceData.ExecuteCityMoveDev 中 food 移动使用 citySrc.food / cityDest.food
- [x] SaveForceData.ExecuteCityChange 中 food 交易使用 cityData.food
- [x] OnBattleEnd 中不返还 food（一次性消耗）

## AI 策略
- [x] AI.TryCreateWarPlan 中 food 检查使用 city.food
- [x] TaskPriorityCalculator.AdjustPriorityByNeeds 中 food 检查使用 city.food
- [x] StrategicDecider 中 food 检查使用 city.food
- [x] StrategicDecider.CanExpand 中 totalFood 遍历城市汇总

## UI 显示
- [x] CityPanelManager 显示城市级 food
- [x] CityDetail 显示城市级 food
- [x] CityDevNodeBattle 中 food 检查使用 city.food
- [x] CityDevNodeMove 中 food 检查使用 city.food

## 存档兼容性
- [x] 旧存档加载时，SaveCityData 的 food 为零值（JsonUtility 默认行为），不崩溃
- [x] 旧存档加载时，SaveForceData 的 wood/horse/steel 为零值，不崩溃
