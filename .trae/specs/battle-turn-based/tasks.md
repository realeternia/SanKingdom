# Tasks

- [x] Task 1: 定义回合制核心数据结构和枚举
  - [x] 在 BattleManager 中新增回合调度状态枚举 `BattleTurnPhase`（RoundStart, TurnStart, TurnAction, TurnEnd, NextTurn, RoundEnd）
  - [x] 在 Chess 中新增回合制字段：`isTurnFinished`
  - [x] 在 Buff 中将 `endTime`(Tick) 改为 `endRound`(Round)
  - [x] 在 Skill 中将 `lastUpdateTick` 改为 `lastUseRound`，CD 从 Tick 改为 Round

- [x] Task 2: 实现回合排序与调度状态机
  - [x] 实现 `SortTurnOrder()`：按 moveSpeed 降序排列所有存活棋子，相同则按 forceId、id 排序
  - [x] 在 BattleManager 中新增 `currentTurnIndex`、`turnOrder` 列表
  - [x] 实现状态机：RoundStart(排序) → TurnStart(重置棋子) → TurnAction(执行行动) → TurnEnd(标记完成+等待) → NextTurn(下一个棋子) → RoundEnd

- [x] Task 3: 重构 BattleManager 主循环
  - [x] 修改 `GameUpdate` 协程：移除 Tick 驱动的棋子自主 LogicUpdate 调用
  - [x] 保留 RenderUpdate 用于 Missile 和动画
  - [x] 在 GameUpdate 中驱动回合状态机推进
  - [x] 实现 TurnEnd 后的 0.5s 等待计时（NextTurn 阶段累加 deltaTime）
  - [x] quickMode 下跳过等待时间

- [x] Task 4: 重构 Chess 回合行动逻辑
  - [x] 新增 `OnTurnStart()`：重置行动状态（isTurnFinished=false, attackPoint 重置等）
  - [x] 新增 `OnTurnAction()`：从 MoveAndFight 迁移逻辑，执行寻敌→移动→攻击
  - [x] 新增 `OnTurnEnd()`：设置 isTurnFinished=true
  - [x] 处理 noActionCount > 0 的棋子直接跳过
  - [x] 保留 ChessAction 创建机制不变

- [x] Task 5: Buff 系统改为回合制
  - [x] Buff 基类：`endTime` → `endRound`，构造时计算 `endRound = currentRound + durationRounds`
  - [x] Buff 过期检查从 Chess.LogicUpdate 移到 ProcessTurnState RoundStart 阶段
  - [x] Buff 刷新逻辑：`endRound = Max(existingEndRound, currentRound + durationRounds)`
  - [x] 所有 Buff 子类适配新字段
  - [x] BuffManager.DoAddBuff 适配回合制参数

- [x] Task 6: 技能系统改为回合制
  - [x] Skill 基类：`lastUpdateTick` → `lastUseRound`，`IsInCD()` 改为比较 Round
  - [x] Skill CD 从 Tick 数改为 Round 数
  - [x] SkillManager 中的技能触发检查适配回合制
  - [x] 延迟效果 `RegisterDelayEffect` 改为基于 Round 的延迟

- [x] Task 7: AddBuffAction / RemoveBuffAction 适配
  - [x] AddBuffAction 中 `LastTime` 改为 `LastRounds`
  - [x] AddBuffAction.Doing() 中计算 `endRound` 而非 `endTime`
  - [x] RemoveBuffAction 无需改动

- [x] Task 8: RoundUpdateAction 扩展
  - [x] RoundUpdateAction 保留兼容（不再由 GameUpdate 调用，回合逻辑在 ProcessTurnState 中）
  - [x] Round 开始时触发行动排序（在 ProcessTurnState.RoundStart 中）

- [x] Task 9: Missile 系统保持不变
  - [x] 确认 Missile 的 LogicUpdate/RenderUpdate 不受回合调度影响
  - [x] 确认 Missile 命中判定仍走 Update

- [x] Task 10: 序列化与回放适配
  - [x] BattleManager 序列化新增字段（turnOrder, currentTurnIndex, turnPhase 等）— 均为可序列化类型
  - [x] Chess 序列化新增字段（isTurnFinished）— public bool 可序列化
  - [x] Buff 序列化字段变更（endTime → endRound）— public int 可序列化
  - [x] Skill 序列化字段变更（lastUpdateTick → lastUseRound）— public int 可序列化
  - [x] IRecoverable.OnRecover() 无需额外适配（新字段无需运行时引用重建）

# Task Dependencies
- Task 1 是所有后续 Task 的基础
- Task 2 依赖 Task 1
- Task 3 依赖 Task 2
- Task 4 依赖 Task 2
- Task 5 依赖 Task 1
- Task 6 依赖 Task 1
- Task 7 依赖 Task 5
- Task 8 依赖 Task 2, Task 5
- Task 9 无依赖，可并行
- Task 10 依赖 Task 1-8 全部完成
