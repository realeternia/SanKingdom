# 战斗 Tick 清理（秒制逻辑时钟）Spec

## Why

战斗已从实时制改为回合制，tick（0.1s 逻辑步计数器）概念失去存在意义。当前 tick 仅剩 4 类用途，全部可以"秒"为单位表达（动画表现单位本就是 s）：

1. 动作队列定序（`ChessAction.Tick`）
2. 导弹飞行时长（`startTick/tickTotal`）
3. 等待/延迟（回合切换等待、开场等待已是秒；召唤错帧、命中延迟 `HitDelay` 配置本身就是秒，tick 换算纯属多余）
4. 废弃参数与死代码（`LogicUpdate(int tickIndex)` 参数大多无人使用、`coroutineManager` 从未注册协程、`lastTargetUpdateTick` 死字段）

清理后消除 tick 术语、删除多余换算与死代码。

## What Changes

- **BREAKING**: `ChessAction.Tick`(int) → `ChessAction.Time`(float 秒)。旧回放文件字段名/类型变化，**旧回放不可加载**（战斗回放为临时记录，可接受）
- **BREAKING**: 回血改为每回合（`OnTurnStart`）触发一次，去掉 `REGE_INTERVAL_TICKS` 计数器，回血频率变为原 10 倍（用户已确认）
- BattleManager：`tickIndex`(int) → `battleTime`(float 秒)，每逻辑步固定推进 `SystemConst.Battle.LOGIC_STEP = 0.1f`（保留确定性逻辑时钟，回放与现在完全一致）
- 动作执行从 `Tick == tickIndex` 精确匹配改为 `Time <= battleTime` 快照扫描执行（替代 `isDoingAction` 顺延逻辑）
- Missile：`startTick/tickTotal` → `startTime/travelSeconds`（float 秒）
- 常量：`SUMMON_HERO_DELAY_TICKS(3)` → `SUMMON_HERO_DELAY_SECONDS(0.3)`、`ATTACKER_SPAWN_DELAY_TICKS(10)` → `ATTACKER_SPAWN_DELAY_SECONDS(1.0)`、`IN_FIGHT_TICK_THRESHOLD(3)` → `IN_FIGHT_TIME_THRESHOLD(0.3)`、`REGE_INTERVAL_TICKS` 删除
- 废弃参数清理：`SceneObj.LogicUpdate/RenderUpdate`、`ChessAI.ProcessTurn`、`SkillManager.LogicUpdate/CheckAidSkill`、技能子类 `LogicUpdate/CheckAidSkill` 移除 `tickIndex` 参数（内部改读 `BattleManager.Instance.battleTime`）
- 死代码删除：`coroutineManager` 及其 `Update` 调用（从未注册协程）、`Chess.lastTargetUpdateTick`
- `MoveToDirection` 速度语义保留：秒制下按 `missileSpeed * (elapsedSeconds / LOGIC_STEP)` 折算，保持原"单位/逻辑步"语义不变
- `SkillAidShockWave` 传 `GetRoundCount()`（int 回合数）作为飞行秒数：秒制下该参数本意就是"表现时长"，保持现状不改

## Impact

- Affected specs: [battle-turn-based](..//battle-turn-based/spec.md)（在其基础上继续清理）
- Affected code:
  - `Assets/Resources/Scripts/Controls/BattleManager.cs`
  - `Assets/Resources/Scripts/Combat/Actions/*`（ChessAction + 15 个子类）
  - `Assets/Resources/Scripts/Combat/Missile.cs` / `SceneObj.cs` / `Chess.cs` / `ChessAI.cs`
  - `Assets/Resources/Scripts/Combat/Skills/*`（SkillManager、BattleSkill、技能子类）
  - `Assets/Resources/Scripts/Combat/Buffs/BuffManager.cs`
  - `Assets/Resources/Scripts/SystemTool/SystemConst.cs`

## ADDED Requirements

### Requirement: 秒制逻辑时钟

系统 SHALL 用 `battleTime`（float 秒）替代 `tickIndex`，每逻辑步固定推进 `SystemConst.Battle.LOGIC_STEP`。

#### Scenario: 正常模式节流

- **WHEN** 战斗循环每执行一步逻辑且 `!quickMode`
- **THEN** `battleTime += LOGIC_STEP`，由帧累计器按真实时间节流（每 0.1s 真实时间推进一步）

#### Scenario: quickMode 批量推进

- **WHEN** `quickMode`
- **THEN** 每帧按 10 步（有 UI）或 100 步（无 UI）批量推进 `battleTime`

### Requirement: 动作队列按秒定序

系统 SHALL 用 `ChessAction.Time`（float 秒）替代 `Tick`，在 `battleTime >= Time` 时执行并移除该动作。

#### Scenario: 顺延语义

- **WHEN** 某步执行动作期间新创建了 `Time <= battleTime` 的动作
- **THEN** 该动作在下一步执行（快照扫描执行，防止同一步重复触发，等价于原 `isDoingAction` 顺延）

## MODIFIED Requirements

### Requirement: 导弹飞行

Missile 用 `startTime/travelSeconds`（float 秒）计算位置插值与崩溃判定，`MoveToDirection` 保持原速度语义。

### Requirement: 回血

`CheckHpReg` 每回合（`OnTurnStart`）触发一次，移除 `regeTickCount` 计数器与 `REGE_INTERVAL_TICKS` 常量。

### Requirement: 交战判定

`Chess.IsInFight` 参数由 `int nowTick` 改为 `float nowTime`，与 `battleTime` 比较，阈值改用 `IN_FIGHT_TIME_THRESHOLD`。

## REMOVED Requirements

### Requirement: coroutineManager 每步更新

**Reason**: 从未有任何代码注册协程，`Update` 调用是死代码。
**Migration**: 直接删除字段与 `coroutineManager.Update(...)` 调用；如未来需要确定性协程可重新引入。

### Requirement: tick 常量与参数

**Reason**: tick 概念移除后，`SUMMON_HERO_DELAY_TICKS`、`ATTACKER_SPAWN_DELAY_TICKS`、`IN_FIGHT_TICK_THRESHOLD`、`REGE_INTERVAL_TICKS` 及各处 `int tickIndex` 参数不再有意义。
**Migration**: 替换为秒制常量；参数删除后内部直接读取 `BattleManager.Instance.battleTime`。
