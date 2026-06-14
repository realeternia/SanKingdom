# 战斗回合制改造 Spec

## Why
当前战斗系统是 Tick 驱动的实时制，所有棋子同时自主行动，缺乏策略深度。改为回合制后，每个 Round 按行动顺序轮流执行棋子的移动和攻击，让战斗更有策略性和可读性。

## What Changes
- **核心架构**：从 Tick 驱动实时制改为 Round 回合制，每个 Round 内棋子按排序轮流行动
- **行动顺序**：每 Round 开始时对所有存活棋子排序（按速度/统帅等属性），决定行动顺序
- **棋子回合**：轮到棋子时执行移动→攻击→结束，行动完成后设结束标记，等待 0.5s 后进入下一个棋子
- **Missile**：仍走 Update 判定，不受回合制影响
- **Buff 改为回合制**：Buff 持续时间从 Tick 数改为回合数，每 Round 结束时减少剩余回合
- **移除 Tick 驱动的自主行动**：Chess.LogicUpdate 不再自主寻敌/移动/攻击，改为回合调度触发
- **保留 ChessAction 队列**：保持 Command 模式的确定性执行和回放能力
- **保留 quickMode 加速**：回合制下 quickMode 跳过等待时间和动画

## Impact
- Affected specs: BattleDamageCalculation（伤害公式不变，但攻击触发方式改变）
- Affected code:
  - `BattleManager.cs`：主循环从 Tick 驱动改为 Round 驱动
  - `Chess.cs`：LogicUpdate 重构，新增回合行动方法
  - `Buff.cs`：过期机制从 Tick 改为 Round
  - `RoundUpdateAction.cs`：扩展回合逻辑
  - 所有 Buff 子类：endTime 改为 endRound
  - 所有 Skill 子类：CD 从 Tick 改为 Round

## ADDED Requirements

### Requirement: 回合制战斗调度
系统 SHALL 提供 Round 回合制调度，每个 Round 内按行动顺序轮流执行棋子行动。

#### Scenario: Round 开始
- **WHEN** 新 Round 开始
- **THEN** 系统对所有存活棋子按行动力排序，生成行动队列 `turnOrder`

#### Scenario: 棋子轮次执行
- **WHEN** 轮到某棋子行动
- **THEN** 该棋子执行移动→攻击→结束标记，完成后等待 0.5s，再进入下一个棋子

#### Scenario: 所有棋子行动完毕
- **WHEN** 当前 Round 行动队列中所有棋子都已完成行动
- **THEN** 触发 Round 结束处理（Buff 回合递减等），然后进入下一个 Round

### Requirement: 行动排序
系统 SHALL 在每 Round 开始时对所有存活棋子排序，决定行动顺序。

#### Scenario: 排序规则
- **WHEN** Round 开始排序
- **THEN** 按行动力（moveSpeed 为主属性，相同则按 forceId、id 排序）降序排列

### Requirement: 棋子回合行动
系统 SHALL 为每个棋子提供回合内行动流程：移动→攻击→结束。

#### Scenario: 移动阶段
- **WHEN** 棋子开始行动且目标不在攻击范围内
- **THEN** 棋子向目标移动一格

#### Scenario: 攻击阶段
- **WHEN** 棋子目标在攻击范围内且攻击点数足够
- **THEN** 棋子执行攻击，创建 AttackAction

#### Scenario: 行动结束标记
- **WHEN** 棋子完成移动和攻击
- **THEN** 设置行动结束标记 `isTurnFinished = true`，等待 0.5s 后调度下一个棋子

#### Scenario: 被控制棋子
- **WHEN** 棋子有 noActionCount > 0
- **THEN** 跳过该棋子行动，直接标记结束

### Requirement: 回合间等待
系统 SHALL 在棋子行动结束后等待指定时间再调度下一个棋子。

#### Scenario: 正常模式等待
- **WHEN** 棋子行动结束且非 quickMode
- **THEN** 在 Update 中计时 0.5s 后调度下一个棋子

#### Scenario: 快速模式跳过等待
- **WHEN** 棋子行动结束且 quickMode
- **THEN** 立即调度下一个棋子，不等待

### Requirement: Missile 独立更新
系统 SHALL 保持 Missile 的 Update 驱动判定不变。

#### Scenario: Missile 飞行与命中
- **WHEN** Missile 存在
- **THEN** 仍通过 Update/RenderUpdate 进行飞行插值和命中判定，不受回合调度影响

### Requirement: Buff 回合制
系统 SHALL 将 Buff 过期机制从 Tick 计数改为 Round 计数。

#### Scenario: Buff 添加
- **WHEN** 添加 Buff 时指定持续回合数
- **THEN** 记录 `endRound = currentRound + durationRounds`

#### Scenario: Buff 过期检查
- **WHEN** Round 结束时
- **THEN** 检查所有 Buff，`currentRound >= endRound` 的 Buff 被移除

#### Scenario: Buff 刷新
- **WHEN** 对已存在的同 ID Buff 刷新
- **THEN** `endRound = Max(existingEndRound, currentRound + durationRounds)`

### Requirement: 技能 CD 回合制
系统 SHALL 将技能 CD 从 Tick 计数改为 Round 计数。

#### Scenario: 技能 CD 判定
- **WHEN** 检查技能是否在 CD 中
- **THEN** 比较 `currentRound >= lastUseRound + cdRounds`

### Requirement: 回合制下 ChessAction 保留
系统 SHALL 保留 ChessAction 队列机制，确保战斗确定性和回放能力。

#### Scenario: Action 创建与执行
- **WHEN** 棋子执行攻击/移动等操作
- **THEN** 仍通过创建对应 ChessAction 入队，在回合调度中执行 Doing()

## MODIFIED Requirements

### Requirement: BattleManager 主循环
原 Tick 驱动主循环改为 Round 回合制主循环：
- 移除 Tick 驱动的 `GameUpdate` 协程中的棋子自主 LogicUpdate 调用
- 新增 Round 调度状态机：`RoundStart` → `TurnStart` → `TurnAction` → `TurnEnd` → `NextTurn` → `RoundEnd`
- 保留 RenderUpdate 用于 Missile 和动画插值
- 保留 quickMode 加速机制

### Requirement: Chess.LogicUpdate
原 Chess.LogicUpdate 中的自主寻敌/移动/攻击逻辑移除，改为：
- `OnTurnStart()`：回合开始，重置行动状态
- `OnTurnAction()`：执行移动和攻击
- `OnTurnEnd()`：回合结束，标记完成
- 保留 Buff/技能的回合制更新检查

### Requirement: RoundUpdateAction
扩展 RoundUpdateAction：
- 增加 Round 结束处理逻辑（Buff 回合递减、技能 CD 递减等）
- Round 开始时生成行动排序

## REMOVED Requirements

### Requirement: Tick 驱动棋子自主行动
**Reason**: 改为回合制后，棋子不再自主行动，由回合调度触发
**Migration**: Chess.LogicUpdate 中的 MoveAndFight 逻辑迁移到 OnTurnAction

### Requirement: Tick 计数 Buff 过期
**Reason**: Buff 改为回合数过期
**Migration**: Buff.endTime(Tick) → Buff.endRound(Round)

### Requirement: Tick 计数技能 CD
**Reason**: 技能 CD 改为回合数
**Migration**: Skill.lastUpdateTick → Skill.lastUseRound
