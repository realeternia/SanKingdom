# Checklist: 删除 SaveHeroData.round 字段及相关逻辑

## 编译检查

- [ ] 项目编译通过，无编译错误
- [ ] 无残留的 `round` 字段引用（除 `SaveData.round` 全局回合数外）
- [ ] 无残留的 `heroYear` 字段引用
- [ ] 无残留的 `SetRoundForRecruit` 方法引用
- [ ] 无残留的 `CheckHeroRound` 方法引用
- [ ] 无残留的 `UpdateHeroesRound` 方法引用
- [ ] 无残留的 `GetAvailableHeroesThisYear` 方法引用

## 功能验证

- [ ] SaveHeroData 类中不再包含 round 字段和 SetRoundForRecruit 方法
- [ ] Player 类中不再包含 CheckHeroRound、UpdateHeroesRound、GetAvailableHeroesThisYear 方法
- [ ] Player 类中所有 Execute* 方法不再调用已删除的方法
- [ ] CityDetail 中英雄头像不再显示"已行动"遮罩
- [ ] PopHeroSelectPanelCell 中英雄始终可选（isAvailable = true）
- [ ] PopHeroBattleSelectPanelCell 中英雄始终可选（isAvailable = true）
- [ ] 英雄选择面板排序仅按属性值排列，不再按 heroYear 排序
- [ ] AI 英雄调度不再检查行动回合
- [ ] AI 策略上下文中英雄始终可用
- [ ] SaveCityData.Occupy 中不再调用 UpdateHeroesRound

## 回归风险

- [ ] 确认 `SaveData.round`（全局回合数）未被误删或误改
- [ ] 确认 `ExecuteCityPraiseHero` 中 methodId==1 的逻辑已正确简化（移除行动检查但保留褒奖逻辑）
- [ ] 确认 `ExecuteCityBattleDev` 中 validHeroList 替换为 heroList 后逻辑正确
- [ ] 确认 `ExecuteCityUseHero` 中删除 SetRoundForRecruit 后登用逻辑仍完整
