---
name: "battle"
description: "战斗系统规则，包含Tick驱动架构、ChessAction队列、IRecoverable接口、BattleRandom规范。Invoke when working on combat system, ChessAction, Buff, Skill, or battle-related code."
---

# 战斗系统规则

## Tick 驱动 + Action 队列架构

战斗采用 Tick 驱动的帧同步架构：
- `tickTimeReal = 0.1f` 为基础 Tick 间隔（1秒 = 10 Tick）
- 所有战斗动作封装为 `ChessAction` 子类，放入 `actions` 队列
- `ChessAction` 包含 `SourceId` 和 `Tick`，在对应 Tick 执行 `Doing()`
- 逻辑更新（`LogicUpdate`）和渲染更新（`RenderUpdate`）分离
- 战斗回放通过序列化/反序列化 `BattleManager` 实现

### Tick 执行流程

每个 Tick 的执行顺序：
1. **逻辑更新**（非回放时）：`chess.LogicUpdate()` → `missile.LogicUpdate()` → 粮食消耗 → 回合更新
2. **Action 执行**：`actions.FindAll(x => x.Tick == tickIndex).ForEach(x => x.Doing())`
3. **协程管理器更新**
4. `tickIndex++`

### 速度控制

| 模式 | 条件 | speed |
|------|------|-------|
| 正常 | `!quickMode` | 1 |
| 快速有UI | `quickMode && showUI` | 10 |
| 快速无UI | `quickMode && !showUI` | 400（纯计算） |

### Action 队列机制

- 通过 `BattleManager.AddChessAction()` 添加到 `actions` 列表
- **Tick 顺延规则**：如果在 `isDoingAction` 期间添加的新 Action 的 Tick 等于当前 `tickIndex`，则自动顺延到下一帧（`action.Tick++`）
- 这保证了 Action 的执行顺序性和确定性

### 逻辑/渲染分离

- `LogicUpdate(int tickIndex)`：纯逻辑计算，每 Tick 调用一次
- `RenderUpdate(int tickIndex, float indexMini, float timeElapsed)`：渲染插值，每帧调用 4 次高频帧（`tickTimeReal / 4` 间隔），用于 Missile 飞行和 Chess 跳跃的平滑动画

### quickMode 模式影响

| 行为 | quickMode=true | quickMode=false |
|------|---------------|-----------------|
| Tick 执行速度 | 10x 或 400x | 1x |
| 特效播放 | 跳过 | 正常播放 |
| 跳跃动画 | 跳过 | 正常播放 |
| 动画播放 | 跳过 | 正常播放 |
| 战斗文字 | 跳过 | 正常显示 |

## ChessAction 子类清单

基类：`ChessAction`（`SourceId`, `Tick`, `Doing()`）

| 子类 | 关键字段 | 说明 |
|------|----------|------|
| `AttackAction` | `TargetId, Damage, IsCrit, IsDodge, HitEffect, DamType` | 普通攻击结算 |
| `SkillDamageAction` | `TargetChessId, SkillId, Damage` | 技能伤害结算 |
| `ChessChangeHpAction` | `Value`（正数加血，负数减血，clamp 到 1~maxHp） | 血量变更 |
| `CreateChessAction` | `Id, ForceId, IsHero, HeroId, Level, SoldierNum, ArmsId, Atk, Def, Str, LeadShip, Inte, SpawnPos, SummonTime` | 创建棋子 |
| `RemoveChessAction` | 无额外字段，用 `SourceId` 标识死者 | 移除棋子 |
| `MoveAction` | `TargetId, TargetPosition` | 棋子移动 |
| `CreateMissileAction` | `Id, TargetChessId, TargetPos, StartPos, SkillId, Damage, Time, IsDirectional` | 创建投射物 |
| `RemoveMissileAction` | `MissileId` | 移除投射物 |
| `AddBuffAction` | `CasterId, SkillId, BuffId, LastTime` | 添加 Buff |
| `RemoveBuffAction` | `BuffId` | 移除 Buff |
| `CreateEffectAction` | `TargetPos, EffectName, Time` | 播放位置特效 |
| `FoodCostAction` | `ForceId, CostAmount` | 粮食消耗 |
| `RoundUpdateAction` | `Round` | 回合数更新 |
| `SkillPlayAction` | `TargetChessId, SkillId, Parm1` | 触发技能播放 |

### 新增战斗动作

1. 继承 `ChessAction`，实现 `Doing()` 方法
2. 通过 `BattleManager.AddChessAction()` 添加到队列
3. 注意 `isDoingAction` 时的 Tick 顺延逻辑

## Command 模式（间接操作）

所有对战斗状态的修改都不直接执行，而是通过创建对应的 `ChessAction` 加入队列，在对应 Tick 由 `Doing()` 统一执行：
- `Chess.Attack()` → 创建 `AttackAction`
- `Chess.AddHp()` → 创建 `ChessChangeHpAction`
- `Chess.Ondying()` → 创建 `RemoveChessAction`
- `BuffManager.AddBuff()` → 创建 `AddBuffAction`
- `BuffManager.RemoveBuff()` → 创建 `RemoveBuffAction`

这种模式确保了战斗的确定性和可回放性。

## IRecoverable 接口

```csharp
public interface IRecoverable { void OnRecover(); }
```

- 实现者：`SceneObj` → `Chess`、`Missile`；`Buff`；`Skill`
- `OnRecover()` 在反序列化后调用，用于重建 `[NonSerialized]` 运行时引用（如 `viewObj`、`buffCfg`、`skillCfg`、`owner`）
- `Chess.OnRecover()` 内部递归调用 `buffs[i].OnRecover()` 和 `skills[i].OnRecover()`
- 调用时机：`BattleManager.LoadFromFile()` 中反序列化后遍历 `chessList` 和 `missileList`

## BattleManager 关键结构

### 核心字段

| 字段 | 类型 | 说明 |
|------|------|------|
| `chessList` | `List<Chess>` | 所有棋子 |
| `missileList` | `List<Missile>` | 所有投射物 |
| `actions` | `List<ChessAction>` | Action 队列（`[SerializeReference]` 多态序列化） |
| `playerInfoList` | `List<FoodInfo>` | 双方粮食信息 |
| `tickIndex` | `int` | 当前 Tick（从 1 开始） |
| `idCounter` | `int` | ID 计数器（初始 100） |
| `round` | `int` | 当前回合 |
| `IsBattleRunning` | `bool` | 防止战斗重入 |

### 关键方法

| 方法 | 说明 |
|------|------|
| `BattleBegin(force1, force2, cards1, cards2, food1, food2, cityId, callback)` | 发起战斗 |
| `ReplayBattle(int replayBattleId)` | 战斗回放 |
| `AddChessAction(ChessAction)` | 添加 Action（isDoingAction 时同 Tick 顺延） |
| `GetChess(int id)` | 按 ID 查找棋子 |
| `GetUnitsInRange(wPos, range, myForceId, findEnemy)` | 范围内查找单位 |
| `GetUnitsByForceId(int forceId)` | 按势力查找存活单位 |
| `OnUnitDying(Chess)` | 棋子死亡处理，判断胜负 |
| `MoveTo(Chess, targetPos, isForce)` | 棋子移动（含碰撞检测） |
| `CheckInRange(pos1, pos2, range)` | 格子坐标距离判断 |
| `SaveToFile(filePath)` | 序列化战斗状态到 JSON |
| `LoadFromFile(filePath)` | 反序列化 + OnRecover |

### FoodInfo 内部类

```csharp
public class FoodInfo { public int forceId, food, maxFood, soldierNumInit; }
```

## Chess 关键结构

继承：`IRecoverable` ← `SceneObj` ← `Chess`

### 核心字段

| 分类 | 字段 | 说明 |
|------|------|------|
| 身份 | `forceId, isHero, isFakeHero, isShadow, heroId, battleUnitId, armsId` | 棋子标识 |
| 三维 | `str, leadShip, inte` | 武力/统率/智力 |
| 战斗 | `maxHp, hp, HpRate, dodgeRate, critRate, critDamageMulti` | 血量和战斗属性 |
| 攻击 | `atk, def, moveSpeed, attackRange` | 攻防和移动（`[NonSerialized]`） |
| 攻击系统 | `attackPoint, attackRate, lastAttackTime, targetChessId, isInAttackRange` | 攻击蓄力 |
| 控制 | `noMoveCount, noActionCount` | 禁移/禁行动计数 |
| 技能/Buff | `skills: List<Skill>, buffs: List<Buff>` | 技能和 Buff 列表 |
| 生命周期 | `dieAfterLifeTime, lifeTickCount, regeTickCount, regeHp` | 限时存活/回血 |

### 关键方法

| 方法 | 说明 |
|------|------|
| `Init(int forceId)` | 初始化棋子，设置 HP、攻击点数、创建技能和 ViewObj |
| `LogicUpdate(int tickIndex)` | 逻辑更新：死亡判定 → Buff 过期 → 技能更新 → HP 回复 → DOT → 跳跃/移动战斗 |
| `FindTarget()` | 寻找目标：距离排序 → 射程内优先 → 打分选择 |
| `MoveAndFight(int tickIndex)` | 移动与战斗主逻辑 |
| `Attack(victim, hitEffectName, tickIndex)` | 普通攻击：伤害计算 → 暴击/闪避 → 创建 AttackAction |
| `AddHp(int addon)` | 加血：创建 ChessChangeHpAction |
| `Ondying()` | 死亡：创建 RemoveChessAction |
| `AddBuff(Buff, Chess caster, int endTick)` | 添加 Buff（同 ID 刷新） |
| `JumpToPosition(targetPos, jumpHeight, moveDuration)` | 跳跃移动（quickMode 下跳过） |
| `GetAttr(string attr)` / `AddAttr(string, int)` | 获取/修改属性值 |
| `AddSkill(int skillId, int parentSkillId)` | 动态添加技能 |

## SkillManager 静态分发器

- **技能工厂**：`CreateSkill(int skillId, Chess owner)` 根据 `ScriptName` 创建对应子类
- **事件分发**：将攻击/受击/加 Buff 等事件分发给所有 Skill 和 Buff 的钩子方法
- **回放保护**：`isReplay` 标志控制所有事件分发是否执行（回放时跳过）

## BuffManager 静态管理器

- **Buff 工厂**：`DoAddBuff()` 根据 `ScriptName` 创建对应 Buff 子类
- **双层操作**：`AddBuff()` 先通过 `SkillManager.OnAddBuff()` 让技能修改 Buff 参数，再创建 `AddBuffAction` 入队
- **移除同理**：`RemoveBuff()` 创建 `RemoveBuffAction`，`DoRemoveBuff()` 执行实际移除

## 战斗结束判定

在 `OnUnitDying()` 中：
- 遍历所有存活且非 shadow 的棋子，统计存活阵营数
- 存活阵营数 ≤ 1 时战斗结束
- 只有 `playerInfoList[0]` 阵营存活 → `BattleResult.Win`
- 否则 → `BattleResult.Lose`
- 达到 `MaxRound` → `BattleResult.Draw`

## BattleStatManager 战斗统计

- 记录每场战斗的详细数据（英雄伤害/受伤/死亡/被俘）
- 战斗记录包含 battleId、cityId、双方 forceId、结果、回合数、兵力损失、粮食消耗
- 最多保留 `MaxBattleCount` 条记录
- 回放模式下不记录统计

## BattleRandom 规范

**`BattleRandom`** — 战斗层专用随机数工具：
- 用于战斗逻辑（暴击、闪避、技能触发、寻路等）
- 支持 `Seed(int)` 设置种子，用于战斗回放
- 支持 `Reset()` 重置为随机种子
- 方法：`Range(int min, int max)`、`Value`（0-1浮点）、`InsideUnitCircle`（单位圆内随机点）

使用规则：
- 战斗代码（`Combat/` 目录下）必须使用 `BattleRandom`
- 禁止在战斗逻辑中使用 `UnityEngine.Random`

## 继承关系总览

```
IRecoverable
├── SceneObj (id, position, LogicUpdate, RenderUpdate, SetPosition)
│   ├── Chess (棋子)
│   └── Missile (投射物: 追踪型/方向型)
├── Buff (OnAdd, OnRemove, DuringAttack 等钩子)
│   ├── BuffShield / BuffShieldValue
│   ├── BuffCoolDown / BuffNoAction / BuffNoMove / BuffLock
│   ├── BuffSuck / BuffDamageAddRate / BuffDamagedAddRate
│   └── BuffSpeedDown / BuffTimeDamage
└── Skill (CheckBurst, BattleBegin, LogicUpdate, 各种钩子)
    ├── SkillAttack* (攻击型)
    ├── SkillDef* (防御型)
    ├── SkillHit* (命中触发型)
    ├── SkillAid* (辅助型)
    ├── SkillHelp* (帮助型)
    ├── SkillInit* (初始化型)
    ├── SkillModify* (修改型)
    ├── SkillAttacked* (被击触发)
    └── SkillDumb (空技能/默认)

ChessAction (SourceId, Tick, Doing())
├── AttackAction / SkillDamageAction / ChessChangeHpAction
├── CreateChessAction / RemoveChessAction
├── MoveAction
├── CreateMissileAction / RemoveMissileAction
├── AddBuffAction / RemoveBuffAction
├── CreateEffectAction
├── FoodCostAction / RoundUpdateAction
└── SkillPlayAction
```

## 战斗系统修改注意事项

- 新增棋子行为时，确保同时处理逻辑层和表现层
- 修改 Tick 相关逻辑时，注意 `quickMode` 下的加速倍率
- `chessList.ToArray()` 用于遍历中可能新增元素的场景
- 战斗中新增字段如需序列化，注意 `[NonSerialized]` 和 `[SerializeReference]` 的使用
- `BattleManager.IsBattleRunning` 用于防止战斗重入
- 新增 Skill 子类需在 `SkillManager.CreateSkill()` 中注册创建逻辑
- 新增 Buff 子类需在 `BuffManager.DoAddBuff()` 中注册创建逻辑

## 禁止事项

- 不要在战斗逻辑中使用 `UnityEngine.Random`，必须使用 `BattleRandom`
- 不要直接修改战斗状态，必须通过 ChessAction 队列间接操作
