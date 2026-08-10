# Tasks

- [x] Task 1: 秒制逻辑时钟与主循环改造（BattleManager + SystemConst）
  - [x] SystemConst.Battle 新增 `LOGIC_STEP = 0.1f`；新增 `SUMMON_HERO_DELAY_SECONDS(0.3)`、`ATTACKER_SPAWN_DELAY_SECONDS(1.0)`、`IN_FIGHT_TIME_THRESHOLD(0.3)`；删除 `SUMMON_HERO_DELAY_TICKS`、`ATTACKER_SPAWN_DELAY_TICKS`、`IN_FIGHT_TICK_THRESHOLD`、`REGE_INTERVAL_TICKS`
  - [x] BattleManager：`tickIndex`(int) → `battleTime`(float)，初始化/重置为 0；删除 `tickTimeReal`，改用 `LOGIC_STEP`
  - [x] GameUpdate：`stepAccumulator` 与 `LOGIC_STEP` 节流、quickMode 批量推进 `battleTime += LOGIC_STEP`；`replayMaxTick` → `replayMaxTime`（`actions.Max(x => x.Time)`），回放结束判定 `battleTime > replayMaxTime`
  - [x] 动作执行改快照扫描：`due = actions.Where(a => a.Time <= battleTime)` 后移除再执行；删除 `isDoingAction` 顺延逻辑（`AddChessAction` 不再 +1）
  - [x] 初始化等待（waitSeconds/battleBeginSeconds）、召唤错帧（`staggerIndex * LOGIC_STEP`）、`AddCellEffectAction` 调度全部改秒
  - [x] 删除 `coroutineManager` 字段及 `coroutineManager.Update(...)` 调用

- [x] Task 2: 动作队列秒制化（ChessAction + 全部子类 + 调度点）
  - [x] ChessAction：`Tick`(int) → `Time`(float)；ctor `(int sourceId, int tick)` → `(int sourceId, float time)`
  - [x] 15 个动作子类 ctor 参数 `int tick` → `float time` 适配（AttackAction、AttackHitAction、SkillDamageAction、ChessChangeHpAction、CreateChessAction×2、RemoveChessAction、MoveAction、CreateMissileAction、RemoveMissileAction、AddBuffAction、RemoveBuffAction、CreateEffectAction、FoodCostAction、RoundUpdateAction、SkillPlayAction、AddCellEffectAction）
  - [x] AttackAction：`hitDelayTicks = (int)(HitDelay / tickTimeReal)` → 直接用 `armsConfig.HitDelay`（秒），`AttackHitAction(..., Time + hitDelaySeconds, ...)`
  - [x] 全部 `new XxxAction(..., tickIndex, ...)` 调用点改 `BattleManager.Instance.battleTime`（含 BuffManager、SkillManager、Chess、ChessAI、BattleManager 内部）

- [x] Task 3: Missile 秒制化
  - [x] `startTick/tickTotal` → `startTime/travelSeconds`（float）；ctor 中 `startTime = battleTime`
  - [x] `MoveToTarget`：`travelSeconds = distance / speed`（去掉 `/ tickTimeReal`）；`MoveToDirection`：`travelSeconds = time`
  - [x] `LogicUpdate`：`battleTime - startTime >= travelSeconds` 判定崩溃；`RenderUpdate` 用 `battleTime` 计算 `fractionOfJourney`
  - [x] `UpdateMoveToDirection`：`moveDistance = missileSpeed * (elapsedSeconds / LOGIC_STEP)`（保持原"单位/逻辑步"语义）
  - [x] `OnCrash`/`RenderUpdate` 签名清理

- [x] Task 4: 战斗实体/技能/AI 参数清理
  - [x] SceneObj：`LogicUpdate(int tickIndex)` → `LogicUpdate()`；`RenderUpdate(int tickIndex, float indexMini, float timeElapsed)` → `RenderUpdate()`
  - [x] Chess：`Attack` 去 tickIndex 参数；`IsInFight(int)` → `IsInFight(float nowTime)` 用 `battleTime` 与 `IN_FIGHT_TIME_THRESHOLD`；`CheckHpReg` 每回合回血（删 `regeTickCount` 计数）；`RenderUpdate` 签名；删除死字段 `lastTargetUpdateTick`
  - [x] ChessAI：`ProcessTurn(Chess self, int tickIndex)` → `ProcessTurn(Chess self)`；内部 `MoveAction`/`Attack` 调度用 `battleTime`
  - [x] SkillManager：`LogicUpdate(Chess, int)` / `CheckAidSkill(int, int)` 去参；BattleSkill 及各技能子类 `LogicUpdate(int)` / `CheckAidSkill(int)` 去参（`SkillHelpAidBuff.IsInFight` 改 `battleTime`）

- [x] Task 5: 编译与行为验证
  - [x] 全量编译无报错
  - [x] 全工程 grep 确认无残留 `tickIndex`/`tickTimeReal`/`\.Tick`（动作类字段）/`_TICKS` 引用
  - [x] 走查动作执行顺序、回放流程与回合调度逻辑符合预期

# Task Dependencies
- Task 1 是后续所有任务的基础（定义 battleTime/LOGIC_STEP API）
- Task 2 依赖 Task 1（调度点使用 battleTime）
- Task 3 依赖 Task 1
- Task 4 依赖 Task 1
- Task 5 依赖 Task 2/3/4 全部完成
