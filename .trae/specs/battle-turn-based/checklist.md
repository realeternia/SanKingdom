# 战斗回合制改造检查清单

## 核心架构
- [x] BattleManager 新增回合调度状态枚举 BattleTurnPhase（RoundStart, TurnStart, TurnAction, TurnEnd, NextTurn, RoundEnd）
- [x] BattleManager 新增 currentTurnIndex、turnOrder 列表字段
- [x] GameUpdate 协程改为 Round 驱动，移除 Tick 驱动的棋子自主 LogicUpdate
- [x] 保留 RenderUpdate 用于 Missile 和动画插值
- [x] quickMode 下跳过 0.5s 等待时间

## 回合排序
- [x] SortTurnOrder() 按 moveSpeed 降序排列存活棋子
- [x] 相同 moveSpeed 时按 forceId、id 排序保证确定性
- [x] 每 Round 开始时重新排序

## 棋子回合行动
- [x] Chess 新增 isTurnFinished 字段
- [x] OnTurnStart() 重置行动状态
- [x] OnTurnAction() 执行寻敌→移动→攻击
- [x] OnTurnEnd() 设置 isTurnFinished=true
- [x] noActionCount > 0 的棋子直接跳过行动
- [x] 行动结束后等待 0.5s 再调度下一个棋子

## Buff 回合制
- [x] Buff.endTime(Tick) 改为 endRound(Round)
- [x] Buff 过期检查移到 RoundStart 阶段（ProcessTurnState.RoundStart 中检查 round >= endRound）
- [x] Buff 刷新逻辑：endRound = Max(existingEndRound, currentRound + durationRounds)
- [x] 所有 Buff 子类适配 endRound 字段

## 技能回合制
- [x] Skill.lastUpdateTick 改为 lastUseRound
- [x] IsInCD() 改为比较 Round
- [x] 技能 CD 从 Tick 数改为 Round 数
- [x] 延迟效果改为基于 Round

## Action 适配
- [x] AddBuffAction.LastTime 改为 LastRounds
- [x] AddBuffAction.Doing() 计算 endRound 而非 endTime
- [x] RoundUpdateAction 保留兼容（回合逻辑在 ProcessTurnState 中）

## Missile 不变
- [x] Missile 的 LogicUpdate/RenderUpdate 不受回合调度影响
- [x] Missile 命中判定仍走 Update

## 序列化与回放
- [x] BattleManager 新增字段可序列化
- [x] Chess 新增字段可序列化
- [x] Buff 字段变更序列化适配
- [x] IRecoverable.OnRecover() 无需额外适配

## 战斗确定性
- [x] 回合排序结果确定性（相同属性时排序稳定）
- [x] ChessAction 队列机制保留
- [x] 回放功能正常
