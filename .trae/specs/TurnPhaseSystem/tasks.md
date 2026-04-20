# 回合阶段管理重构 - 任务列表

## 任务概览

| ID | 任务名称 | 优先级 | 依赖 |
|----|----------|--------|------|
| T1 | Player 类添加 Phase 属性和方法 | P0 | 无 |
| T2 | GameManager 移除 currentPhase，改用计算属性 | P0 | T1 |
| T3 | GameManager 中阶段切换改为 player.SetPhase() | P0 | T1, T2 |
| T4 | Player.StartPlanningPhase() 实现（含 AI idle） | P0 | T1 |
| T5 | GameManager.StartPlayerPlanningPhase() 重构 | P0 | T4 |
| T6 | AIPlayerTurnCoroutine 改为 idle 模式 | P0 | T5 |
| T7 | 玩家排序：人类玩家优先 | P0 | T5 |
| T8 | AICheck 信号发送 | P0 | T5 |
| T9 | EndRound 适配 | P0 | T2 |
| T10 | LoadFromSave 适配 | P1 | T2 |
| T11 | 编译验证 | P0 | T1-T10 |

---

## T1: Player 类添加 Phase 属性和方法

### 描述
在 Player 类中添加阶段状态管理。

### 详细任务
- [ ] T1.1 添加 `public TurnPhase Phase { get; private set; } = TurnPhase.None;` 属性
- [ ] T1.2 添加 `public void SetPhase(TurnPhase phase)` 方法
- [ ] T1.3 在 Player.cs 顶部确保 using 引用正确

### 文件
- `Assets/Resources/Scripts/Controls/Player.cs`

---

## T2: GameManager 移除 currentPhase，改用计算属性

### 描述
移除 GameManager 中的 currentPhase 私有字段，改为基于 currentPlayer.Phase 的计算属性。

### 详细任务
- [ ] T2.1 删除 `private TurnPhase currentPhase = TurnPhase.None;`
- [ ] T2.2 修改 `CurrentPhase` 为 `public TurnPhase CurrentPhase => currentPlayer?.Phase ?? TurnPhase.None;`
- [ ] T2.3 修改 `IsPlanningPhase` 为 `public bool IsPlanningPhase => CurrentPhase == TurnPhase.Planning;`

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T3: GameManager 中阶段切换改为 player.SetPhase()

### 描述
将所有 `currentPhase = TurnPhase.xxx` 替换为 `player.SetPhase(TurnPhase.xxx)`。

### 详细任务
- [ ] T3.1 `PlayerTurnCoroutine` 中 `currentPhase = TurnPhase.Execution` → `player.SetPhase(TurnPhase.Execution)`
- [ ] T3.2 `PlayerTurnCoroutine` 中 `currentPhase = TurnPhase.Battle` → `player.SetPhase(TurnPhase.Battle)`

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T4: Player.StartPlanningPhase() 实现（含 AI idle）

### 描述
在 Player 类中实现 StartPlanningPhase() 方法。AI 玩家采用 idle 模式，不执行实际逻辑。

### 详细任务
- [ ] T4.1 实现 StartPlanningPhase() 方法
  - 设置 Phase = TurnPhase.Planning
  - 人类玩家：forbidPlayerAct = false，发送 PhaseChange 和 AICheck（清除）信号
  - AI 玩家：forbidPlayerAct = true，发送 AICheck 信号，启动 idle 协程（不调用 AI.ExecutePlanningPhase）

### 文件
- `Assets/Resources/Scripts/Controls/Player.cs`

---

## T5: GameManager.StartPlayerPlanningPhase() 重构

### 描述
简化 GameManager.StartPlayerPlanningPhase()，改为调用 player.StartPlanningPhase()。

### 详细任务
- [ ] T5.1 将 StartPlayerPlanningPhase 改为调用 player.StartPlanningPhase()
- [ ] T5.2 AI 协程启动逻辑移到 Player.StartPlanningPhase() 中

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T6: AIPlayerTurnCoroutine 改为 idle 模式

### 描述
将 AI 协程从执行完整逻辑改为 idle 模式：仅做短暂等待后切换下一个玩家。

### 详细任务
- [ ] T6.1 移除 `AIPlayerTurnCoroutine` 中的 `ExecutePlayerDevActions` 调用
- [ ] T6.2 移除 `AIPlayerTurnCoroutine` 中的战争计划执行逻辑
- [ ] T6.3 改为 `yield return new WaitForSeconds(0.3f);` + `StartNextPlayerTurn();`
- [ ] T6.4 保留日志输出

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T7: 玩家排序：人类玩家优先

### 描述
确保每个 Round 开始时，人类玩家总是第一个行动。

### 详细任务
- [ ] T7.1 在 GameManager 中添加 GetSortedPlayers() 方法
- [ ] T7.2 NextRound() 中使用排序后的玩家列表
- [ ] T7.3 NewGame() 中使用排序后的玩家列表

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T8: AICheck 信号发送

### 描述
在切换 AI 玩家时发送 AICheck 信号，切换人类玩家时清除 AI 信息。

### 详细任务
- [ ] T8.1 AI 玩家回合开始时发送 `PanelManager.Instance.SendSignal("AICheck", player.pname, player.forceId)`
- [ ] T8.2 人类玩家回合开始时发送 `PanelManager.Instance.SendSignal("AICheck", "", 0)`
- [ ] T8.3 EndRound 时发送 `PanelManager.Instance.SendSignal("AICheck", "", 0)`

### 文件
- `Assets/Resources/Scripts/Controls/Player.cs`（在 StartPlanningPhase 中）
- `Assets/Resources/Scripts/Controls/GameManager.cs`（在 EndRound 中）

---

## T9: EndRound 适配

### 描述
修改 EndRound() 以适配新的 Phase 架构。

### 详细任务
- [ ] T9.1 移除 `currentPhase = TurnPhase.None`，改为 `currentPlayer.SetPhase(TurnPhase.None)`
- [ ] T9.2 添加 AICheck 清除信号

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T10: LoadFromSave 适配

### 描述
确保加载存档后游戏状态正确。

### 详细任务
- [ ] T10.1 LoadFromSave 后 currentPhase 不再需要恢复（已移除）
- [ ] T10.2 加载存档后玩家处于 None 阶段，需点击"下一回合"进入新回合

### 文件
- `Assets/Resources/Scripts/Controls/GameManager.cs`

---

## T11: 编译验证

### 描述
确保所有修改后代码无编译错误。

### 详细任务
- [ ] T11.1 检查所有 currentPhase 引用已移除
- [ ] T11.2 检查所有 Phase 相关逻辑正确
- [ ] T11.3 无编译错误

---

## 实现顺序

```
T1 (Player Phase) → T2 (GameManager 计算属性) → T3 (SetPhase 替换)
                                                   ↓
T4 (Player.StartPlanningPhase + AI idle) → T5 (GM 重构) → T6 (AI idle 协程)
                                                            ↓
T7 (排序) → T8 (AICheck) → T9 (EndRound) → T10 (LoadFromSave) → T11 (编译验证)
```
