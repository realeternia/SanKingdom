# Checklist: 删除 SaveHeroData.soldier 属性，改为战斗现场调配

## 数据层变更

- [ ] SaveHeroData.soldier 字段已删除
- [ ] SaveHeroData.GetAttr("soldier") 分支已删除
- [ ] SaveHeroData.CreateWildHero 不再设置 soldier
- [ ] SaveCityData.GetAttr("soldier") 简化为直接返回 city.soldier（不再累加英雄兵力）
- [ ] SaveCityData.AutoSetSoldierOnInit 已删除
- [ ] SaveCityData.DistributeSoldierDefault 新方法已实现

## 接口层变更

- [ ] SaveCityData.GetBattleHeroList 新增 heroSoldierDict/heroArmsDict 参数
- [ ] GetBattleHeroList 中 SoldierNum 来源改为 heroSoldierDict 或默认分配
- [ ] GetBattleHeroList 中 ArmsId 来源改为 heroArmsDict 或 hero.armsId
- [ ] Player.ExecuteCityBattleDev 新增 heroSoldierDict/heroArmsDict 参数
- [ ] Player.OnBattleEnd 兵力回写目标改为城市兵池

## 战斗层变更

- [ ] BattleManager.OnUnitDying 不再设置 hero.soldier = 0

## AI 层变更

- [ ] AI.DistributeSoldierToHeroes 返回 Dictionary<int,int> 而非修改 hero.soldier
- [ ] AI.TryExecuteAttack 使用调配字典传递兵力
- [ ] AI.HandleFoodPurchase 使用 city.GetAttr("soldier")
- [ ] AI.ExecuteHeroMove 移除 hero.soldier 引用
- [ ] TaskPriorityCalculator.HasSoldier 改用 city.GetAttr("soldier") > 0
- [ ] TaskPriorityCalculator.AdjustPriorityByNeeds 使用 city.GetAttr("soldier")

## UI 层变更（接口预留）

- [ ] SelectHeroArmyControl 不再引用 heroData.soldier
- [ ] PopArmySetManager 所有 hero.soldier 引用已替换为城市兵池操作

## 验证

- [ ] 全局搜索 "hero.soldier" 无残留引用
- [ ] 全局搜索 "heroData.soldier" 无残留引用
- [ ] 全局搜索 "h.soldier" 无残留引用
- [ ] 编译无错误
- [ ] 战斗流程完整：调配兵力 → 进入战斗 → 战斗结束 → 兵力回写城市
- [ ] AI 攻击流程完整：选择英雄 → 分配兵力 → 发起攻击 → 兵力回写
