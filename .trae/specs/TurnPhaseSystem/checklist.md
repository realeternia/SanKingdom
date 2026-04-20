# 回合阶段管理重构 - 验收检查清单

## 1. Player 类验收

### 1.1 Phase 属性
- [ ] `Phase` 属性存在且为 `public TurnPhase Phase { get; private set; }`
- [ ] 默认值为 `TurnPhase.None`
- [ ] 外部只能读取，不能直接设置

### 1.2 SetPhase 方法
- [ ] `SetPhase(TurnPhase phase)` 方法存在且为 public
- [ ] 调用后 `Phase` 值正确更新

### 1.3 StartPlanningPhase 方法
- [ ] 方法存在且为 public
- [ ] 调用后 `Phase` 正确设置为 `TurnPhase.Planning`
- [ ] 人类玩家：`forbidPlayerAct` 设为 false
- [ ] 人类玩家：发送 "PhaseChange" "Planning" 信号
- [ ] 人类玩家：发送 "AICheck" "" 0 信号（清除 AI 信息）
- [ ] AI 玩家：`forbidPlayerAct` 设为 true
- [ ] AI 玩家：发送 "AICheck" pname forceId 信号
- [ ] AI 玩家：不调用 AI.ExecutePlanningPhase（idle 模式）
- [ ] AI 玩家：启动 AI idle 协程

---

## 2. GameManager 验收

### 2.1 currentPhase 移除
- [ ] `private TurnPhase currentPhase` 字段已删除
- [ ] 所有 `currentPhase =` 赋值已替换

### 2.2 计算属性
- [ ] `CurrentPhase` 返回 `currentPlayer?.Phase ?? TurnPhase.None`
- [ ] `IsPlanningPhase` 返回 `CurrentPhase == TurnPhase.Planning`
- [ ] 无 currentPlayer 时 `CurrentPhase` 返回 `TurnPhase.None`

### 2.3 阶段切换
- [ ] `PlayerTurnCoroutine` 中使用 `player.SetPhase(TurnPhase.Execution)`
- [ ] `PlayerTurnCoroutine` 中使用 `player.SetPhase(TurnPhase.Battle)`
- [ ] `EndRound` 中使用 `currentPlayer.SetPhase(TurnPhase.None)`

### 2.4 StartPlayerPlanningPhase 重构
- [ ] 方法简化为调用 `player.StartPlanningPhase()`
- [ ] AI 协程启动逻辑正确

### 2.5 玩家排序
- [ ] `GetSortedPlayers()` 方法存在
- [ ] 人类玩家排在最前
- [ ] AI 玩家按 forceId 排序
- [ ] `NextRound()` 使用排序后的玩家列表
- [ ] `NewGame()` 使用排序后的玩家列表

---

## 3. AICheck 信号验收

### 3.1 信号发送
- [ ] AI 玩家回合开始时发送 "AICheck" 信号（含玩家名和 forceId）
- [ ] 人类玩家回合开始时发送 "AICheck" "" 0（清除 AI 信息）
- [ ] EndRound 时发送 "AICheck" "" 0（清除 AI 信息）

### 3.2 UI 显示
- [ ] AI 玩家回合时 textAiInfo 显示 "[玩家名] 进行中"
- [ ] AI 玩家回合时 textAiInfo 使用势力颜色
- [ ] 人类玩家回合时 textAiInfo 隐藏
- [ ] 回合结束时 textAiInfo 隐藏

---

## 4. 游戏流程验收

### 4.1 新游戏
- [ ] NewGame 后人类玩家先行动
- [ ] 人类玩家可立即派遣英雄（无"不是玩家回合"提示）
- [ ] 人类玩家可立即操作（无"当前阶段无法派遣英雄"提示）

### 4.2 回合流转
- [ ] 人类玩家确认计划后进入执行阶段
- [ ] 执行阶段后 AI 玩家依次行动
- [ ] AI 行动时 textAiInfo 正确显示
- [ ] AI 回合仅做短暂等待（~0.3秒）后切换下一个玩家
- [ ] AI 回合不执行开发、战斗等逻辑
- [ ] 所有玩家行动后回合结束
- [ ] 下一回合人类玩家仍先行动

### 4.3 加载存档
- [ ] 加载存档后游戏状态正确
- [ ] 加载存档后点击"下一回合"可正常进入新回合

---

## 5. 编译验收

- [ ] 无编译错误
- [ ] 无编译警告
- [ ] 所有 currentPhase 引用已清理
