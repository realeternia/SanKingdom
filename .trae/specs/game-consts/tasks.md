# 任务列表

## 任务1: 创建 SystemConst.cs 文件
- 在 `D:\U3dPrj\SanKingdom\Assets\Resources\Scripts\SystemTool\` 下创建 `SystemConst.cs`
- 按功能分区定义所有常量（Game, Battle, WorldMap, AIStrategy, AIHero, AICity, Economy）
- 使用嵌套静态类结构

## 任务2: 替换 GameManager.cs 中的常量
- 删除 `BASE_YEAR`, `BORN_AGE`, `SEASONS_PER_YEAR` 声明
- 替换所有引用为 `SystemConst.Game.BASE_YEAR` 等

## 任务3: 替换 BattleManager.cs 中的常量
- 删除 `gridCellSize`, `MaxRound`, `WaitTime`, `BattleBeginTime` 声明
- 替换所有引用为 `SystemConst.Battle.GRID_CELL_SIZE` 等

## 任务4: 替换 BattleStatManager.cs 中的常量
- 删除 `MaxBattleCount` 声明
- 替换引用为 `SystemConst.Battle.MAX_BATTLE_COUNT`

## 任务5: 替换 StrategicDecider.cs 中的常量
- 删除 `MAX_ATK_CITIES`, `MIN_RESOURCE_FOR_ATTACK`, `MIN_SOLDIER_FOR_ATTACK`, `MIN_CITY_SOLDIER_FOR_ATTACK`, `MIN_CITY_HEROES_FOR_ATTACK`, `MAX_SOLDIER_PER_HERO` 声明
- 替换所有引用为 `SystemConst.AIStrategy.XXX`

## 任务6: 替换 HeroDispatcher.cs 中的常量
- 删除 `COMBAT_THRESHOLD`, `DOMESTIC_THRESHOLD`, `MIN_REAR_HEROES` 声明
- 替换所有引用为 `SystemConst.AIHero.XXX`

## 任务7: 替换 CityEvaluator.cs 中的常量
- 删除 `GOLD_ALERT`, `FOOD_ALERT`, `WALL_ALERT`, `SOLDIER_ALERT`, `POWER_ALERT` 声明
- 替换所有引用为 `SystemConst.AICity.XXX`

## 任务8: 替换 TaskPriorityCalculator.cs 中的常量
- 删除 `NEED_WEIGHT` 声明
- 替换引用为 `SystemConst.AICity.NEED_WEIGHT`

## 任务9: 替换 AI.cs 中的常量
- 删除方法体内的 `EXCHANGE_RATE` 局部常量
- 替换引用为 `SystemConst.Economy.EXCHANGE_RATE`

## 任务10: 替换 CityDevNodeChange.cs 中的常量
- 删除 `EXCHANGE_RATE` 声明
- 替换引用为 `SystemConst.Economy.EXCHANGE_RATE`

## 任务11: 替换 WorldPieceControl.cs 中的常量
- 删除 `MAP_SCALE_FACTOR` 声明
- 替换引用为 `SystemConst.WorldMap.MAP_SCALE_FACTOR`

## 任务12: 验证编译
- 确保所有修改后工程无编译错误
