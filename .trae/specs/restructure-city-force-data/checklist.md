# Checklist

## 数据结构变更
- [x] SaveForceData 包含 `gold` 和 `food` 字段
- [x] SaveCityData 不包含 `gold`、`food`、`power` 字段
- [x] SaveCityData 包含 `happy` 字段
- [x] SaveCityData.AddAttr 移除 gold/food/power 分支，新增 happy 分支
- [x] SaveCityData.GetAttr 移除 gold/food/power 分支，新增 happy 分支
- [x] SaveCityData.OnRound 中 gold/food 产出写入势力数据

## 配置系统
- [x] CityAttrConfig 移除 power 配置项
- [x] CityAttrConfig 新增 happy 配置项
- [x] CityAttrConfig 的 idxname/idxCname 映射已更新

## 初始化逻辑
- [x] GameManager.NewGame 中城市不再初始化 gold/food/power
- [x] GameManager.NewGame 中城市初始化 happy（使用 SystemConst.City.INITIAL_CITY_HAPPY）
- [x] GameManager.NewGame 中势力初始化 gold 和 food（从 ForceConfig.InitGold / ForceConfig.InitFood 获取）

## 业务逻辑
- [x] Player.ExecuteCityDev 中 gold 检查和扣除使用 forceData
- [x] Player.ExecuteCityBattleDev 中 food 扣除使用 forceData
- [x] Player.OnBattleEnd 中移除 food 归还逻辑（粮草一次性消耗）
- [x] Player.ExecuteCityChange 中 gold/food 交易使用 forceData
- [x] Player.ExecuteCityPraiseHero 中 gold 扣除使用 forceData
- [x] Player.ExecuteCityMoveDev 中 food 逻辑使用 forceData（同势力内移动无需扣粮）

## AI 策略
- [x] AI.TryCreateWarPlan 中 food 检查使用 forceData
- [x] TaskPriorityCalculator 中 gold/food 检查使用 forceData
- [x] TaskPriorityCalculator.TaskMatchesNeed 中 PowerLow 替换为 HappyLow
- [x] StrategicDecider 中 food 检查使用 forceData
- [x] StrategicDecider.CanExpand 中 gold/food 使用 forceData
- [x] CityNeedType 枚举移除 PowerLow，新增 HappyLow
- [x] CityEvaluator 移除 power 评估，新增 happy 评估

## UI 显示
- [x] CityPanelManager 显示势力级 gold/food，新增 happy
- [x] CityDetail 显示势力级 gold/food，power 替换为 happy
- [x] CityDevNodeBattle 中 food 检查使用 forceData
- [x] CityDevNodeMove 中 food 检查使用 forceData

## 常量系统
- [x] SystemConst.City 中移除 INITIAL_CITY_POWER，新增 INITIAL_CITY_HAPPY
- [x] SystemConst.AICity 中 POWER_ALERT 替换为 HAPPY_ALERT

## 存档兼容性
- [x] 旧存档加载时，SaveForceData 的 gold/food 为零值（JsonUtility 默认行为），不崩溃
- [x] 旧存档加载时，SaveCityData 的 happy 为零值，不崩溃
