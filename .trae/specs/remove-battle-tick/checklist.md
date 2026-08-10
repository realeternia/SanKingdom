# 战斗 Tick 清理检查清单

## 核心时钟
- [x] BattleManager 用 `battleTime`(float 秒) 替代 `tickIndex`，无残留 tick 术语
- [x] `tickTimeReal` 移除，改用 `SystemConst.Battle.LOGIC_STEP`
- [x] 正常模式按真实时间节流、quickMode 每帧批量推进 `battleTime`
- [x] 回放结束判定改为 `battleTime > replayMaxTime`

## 动作队列
- [x] `ChessAction.Time`(float) 替代 `Tick`(int)
- [x] 全部动作子类与调用点适配 float 秒调度
- [x] `Time <= battleTime` 快照扫描执行，替代 `isDoingAction` 顺延
- [x] AttackAction 命中延迟直接用 `HitDelay`(秒)，无 tick 换算

## 导弹
- [x] `startTime/travelSeconds` 替代 `startTick/tickTotal`
- [x] 崩溃判定、位置插值全部按秒计算
- [x] `MoveToDirection` 保持原速度语义

## 常量与行为
- [x] tick 常量（SUMMON_HERO_DELAY_TICKS/ATTACKER_SPAWN_DELAY_TICKS/IN_FIGHT_TICK_THRESHOLD/REGE_INTERVAL_TICKS）全部移除或替换为秒制
- [x] 回血每回合一次（无计数器）
- [x] `IsInFight` 按 `battleTime` 秒制判定

## 参数与死代码
- [x] `LogicUpdate`/`RenderUpdate`/`ProcessTurn`/`CheckAidSkill` 无 tickIndex 参数
- [x] `coroutineManager` 死代码删除
- [x] `lastTargetUpdateTick` 死字段删除

## 验证
- [x] 全量编译无报错
- [x] 全工程 grep 无残留 `tickIndex`/`tickTimeReal`/`_TICKS` 引用
- [x] 回合调度与回放流程走查符合预期
