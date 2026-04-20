# 回合阶段管理重构规格说明

## 1. 概述

将回合阶段（TurnPhase）管理从 GameManager 迁移到 Player，明确职责分离：Player 负责自身阶段切换，GameManager 负责回合（Round）流转。同时修复 AI 信息显示和玩家回合判定问题。

## 2. 现有问题

### 2.1 阶段管理职责不清
- `currentPhase` 存放在 GameManager 上，是全局状态
- 阶段本质上是每个 Player 的状态，应由 Player 自行管理
- GameManager 既管 Round 又管 Phase，职责混乱

### 2.2 AI 信息显示失效
- `MainPanelManager.SendSignal` 中有 "AICheck" 信号处理逻辑（L398-413）
- 但整个代码库中**没有任何地方发送 "AICheck" 信号**
- 导致切换 AI 玩家时 `textAiInfo` 不会更新

### 2.3 游戏开始时玩家无法操作
- 游戏开始后调用 `StartNextPlayerTurn()`，按 `ForceConfig.ConfigList` 顺序遍历玩家
- 如果 AI 玩家排在人类玩家前面，人类玩家需等待所有 AI 完成后才能操作
- 用户期望：游戏开始和每个新赛季开始时，应该是人类玩家的回合

### 2.4 AI 回合流程卡顿
- 当前 AI 回合会执行完整的计划、开发、战斗流程
- 用户期望：AI 暂时 idle，跳过所有阶段，仅做短暂等待（0.x 秒）后切换到下一个玩家
- 目的：保持流程通畅，同时让玩家能看到 AI 信息提示

## 3. 架构设计

### 3.1 职责划分

| 职责 | 管理者 | 说明 |
|------|--------|------|
| Round 流转 | GameManager | NextRound / EndRound / round++ |
| 玩家轮次顺序 | GameManager | currentPlayerIndex / currentPlayer |
| 阶段状态 | Player | 每个 Player 持有自己的 Phase |
| 阶段切换 | Player | StartPlanningPhase / SetPhase |
| 战争计划收集 | GameManager | warPlans / confirmedForces |

### 3.2 Player 类新增

```csharp
public class Player
{
    public TurnPhase Phase { get; private set; } = TurnPhase.None;

    public void StartPlanningPhase() { ... }
    public void SetPhase(TurnPhase phase) { ... }
}
```

### 3.3 GameManager 类变更

**移除：**
- `private TurnPhase currentPhase` 字段

**改为计算属性：**
- `public TurnPhase CurrentPhase => currentPlayer?.Phase ?? TurnPhase.None;`
- `public bool IsPlanningPhase => CurrentPhase == TurnPhase.Planning;`

**保留不变：**
- `currentPlayerIndex`, `currentPlayer`, `warPlans`, `confirmedForces`
- `forbidPlayerAct`（仍由 GameManager 控制，因为 UI 交互限制是全局的）
- `NextRound()`, `EndRound()` 核心流程

### 3.4 玩家轮次顺序调整

在 `NewGame()` 和 `NextRound()` 中，将人类玩家排在轮次首位，确保人类玩家总是先行动。

排序规则：
1. 人类玩家（isPlayer == true）排在最前
2. 其余 AI 玩家按 forceId 排序

### 3.5 AICheck 信号发送

在 `StartPlayerPlanningPhase()` 中：
- AI 玩家回合开始时：`PanelManager.Instance.SendSignal("AICheck", player.pname, player.forceId);`
- 人类玩家回合开始时：`PanelManager.Instance.SendSignal("AICheck", "", 0);`（清除 AI 信息）

### 3.6 AI Idle 模式

AI 玩家回合暂时采用 idle 模式：
- 跳过所有阶段（Planning / Execution / Battle）
- 仅做短暂等待（`WaitForSeconds(0.3f)`），让玩家看到 AI 信息提示
- 等待结束后直接切换到下一个玩家
- 不调用 `AI.ExecutePlanningPhase()`、`ExecutePlayerDevActions()`、战争执行等逻辑

## 4. 详细设计

### 4.1 Player.StartPlanningPhase()

```csharp
public void StartPlanningPhase()
{
    Phase = TurnPhase.Planning;

    if (IsPlayer)
    {
        GameManager.Instance.forbidPlayerAct = false;
        PanelManager.Instance.SendSignal("PhaseChange", "Planning", forceId);
        PanelManager.Instance.SendSignal("AICheck", "", 0);
    }
    else
    {
        GameManager.Instance.forbidPlayerAct = true;
        PanelManager.Instance.SendSignal("AICheck", pname, forceId);
        GameManager.Instance.StartCoroutine(GameManager.Instance.AIPlayerTurnCoroutine(this));
    }
}
```

注意：AI 分支不再调用 `AI.ExecutePlanningPhase(this)`，AI idle 模式下跳过所有逻辑。

### 4.2 GameManager 中的阶段切换改为 player.SetPhase()

**PlayerTurnCoroutine：**
```csharp
private IEnumerator PlayerTurnCoroutine(Player player)
{
    player.SetPhase(TurnPhase.Execution);
    forbidPlayerAct = true;
    PanelManager.Instance.SendSignal("PhaseChange", "Execution", player.forceId);
    // ... 执行逻辑不变 ...
    
    if (playerWarPlans.Count > 0)
    {
        player.SetPhase(TurnPhase.Battle);
        PanelManager.Instance.SendSignal("PhaseChange", "Battle", player.forceId);
        // ... 战斗逻辑不变 ...
    }
    
    StartNextPlayerTurn();
}
```

**AIPlayerTurnCoroutine（Idle 模式）：**
```csharp
private IEnumerator AIPlayerTurnCoroutine(Player player)
{
    yield return new WaitForSeconds(0.3f);
    GameLog.Info($"AI {player.pname} idle 回合完成");
    StartNextPlayerTurn();
}
```

AI 暂时跳过所有阶段逻辑，仅做 0.3 秒短暂等待后切换到下一个玩家，保持流程通畅。

### 4.3 EndRound 处理

```csharp
public void EndRound()
{
    if (currentPlayer != null)
        currentPlayer.SetPhase(TurnPhase.None);
    forbidPlayerAct = false;
    PanelManager.Instance.SendSignal("PhaseChange", "None", 0);
    PanelManager.Instance.SendSignal("AICheck", "", 0);
    PanelManager.Instance.SendSignal("RoundChange", "", SaveData.round);
    SaveToFile();
    PanelManager.Instance.SwitchBGM();
}
```

### 4.4 玩家排序

```csharp
private List<Player> GetSortedPlayers()
{
    var sorted = new List<Player>(players);
    sorted.Sort((a, b) =>
    {
        bool aIsPlayer = a.IsPlayer;
        bool bIsPlayer = b.IsPlayer;
        if (aIsPlayer != bIsPlayer)
            return aIsPlayer ? -1 : 1;
        return a.forceId - b.forceId;
    });
    return sorted;
}
```

在 `NextRound()` 和 `NewGame()` 中使用排序后的玩家列表。

### 4.5 CityPanelManager 检查逻辑

现有检查逻辑无需修改：
```csharp
var currentPlayer = GameManager.Instance.CurrentPlayer;
if (currentPlayer == null || !currentPlayer.IsPlayer)
{
    SystemTip.Instance.ShowTip("当前不是你的回合");
    return;
}

if (!GameManager.Instance.IsPlanningPhase)
{
    SystemTip.Instance.ShowTip("当前阶段无法派遣英雄");
    return;
}
```

因为 `IsPlanningPhase` 现在基于 `currentPlayer.Phase`，而人类玩家总是先行动，所以在人类玩家的回合中 `IsPlanningPhase` 为 true。

## 5. 影响范围

| 文件 | 修改类型 | 说明 |
|------|----------|------|
| Player.cs | 新增字段和方法 | 添加 Phase 属性、StartPlanningPhase()、SetPhase() |
| GameManager.cs | 重构 | 移除 currentPhase，改用计算属性；调整玩家排序；发送 AICheck 信号 |
| MainPanelManager.cs | 无修改 | AICheck 信号处理已存在，无需修改 |
| CityPanelManager.cs | 无修改 | 检查逻辑兼容新架构 |
| TurnPhaseData.cs | 无修改 | 枚举定义不变 |

## 6. 不涉及的内容

- 不修改 TurnPhase 枚举定义
- 不修改 WarPlanData 结构
- 不修改存档格式（Phase 是运行时状态，不需要持久化）
- 不修改 AI 决策逻辑
- 不修改战斗系统
