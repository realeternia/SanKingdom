# 检查清单

## 文件创建
- [ ] SystemConst.cs 已创建到正确路径
- [ ] 包含所有7个功能分区的嵌套静态类
- [ ] 常量命名统一为 UPPER_SNAKE_CASE
- [ ] 常量值与原定义完全一致

## 常量移除与引用替换
- [ ] GameManager.cs: BASE_YEAR, BORN_AGE, SEASONS_PER_YEAR 已移除并替换引用
- [ ] BattleManager.cs: gridCellSize, MaxRound, WaitTime, BattleBeginTime 已移除并替换引用
- [ ] BattleStatManager.cs: MaxBattleCount 已移除并替换引用
- [ ] StrategicDecider.cs: 6个AI策略常量已移除并替换引用
- [ ] HeroDispatcher.cs: 3个AI英雄常量已移除并替换引用
- [ ] CityEvaluator.cs: 5个AI城市告警常量已移除并替换引用
- [ ] TaskPriorityCalculator.cs: NEED_WEIGHT 已移除并替换引用
- [ ] AI.cs: EXCHANGE_RATE 局部常量已移除并替换引用
- [ ] CityDevNodeChange.cs: EXCHANGE_RATE 已移除并替换引用
- [ ] WorldPieceControl.cs: MAP_SCALE_FACTOR 已移除并替换引用

## 重复常量合并
- [ ] EXCHANGE_RATE (AI.cs + CityDevNodeChange.cs) 合并为 SystemConst.Economy.EXCHANGE_RATE

## UI常量排除
- [ ] BattleUIManager.cs 的 designWidth/designHeight 未被移动
- [ ] MainPanelManager.cs 的 MAP_SCALE_FACTOR 未被移动
- [ ] GameLog.cs 的锁对象未被移动

## 编译验证
- [ ] 工程编译无错误
