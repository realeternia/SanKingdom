# 游戏事件系统实现计划

## Context

新增一个游戏事件记录系统，与 `SaveData` 平级存在于 `GameManager` 上，拥有独立的 JSON 存储/加载机制（`game_events.json`），生命周期与 SaveData 同步。所有 hero 相关事件（战斗、KingAction、dev 委派、被抓/下野/逃脱/招募等状态变化）通过统一接口记录，每条事件含年份、forceId、cityName、heroId 列表等上下文。每回合末按队列头检查并移除超过游戏时间 1 年（36 回合）的事件。

Dev 事件特殊处理：玩家/AI 在一回合内可能反复设置/取消委派，为避免中间过程产生噪音，dev 不在 SetDevAssignment/RemoveDevAssignment 时立即记录，而是在回合末通过快照 diff 仅记录净变化（新增/取消/变更）。

## 设计要点

### 文件结构

**新增 2 个文件**（目录 `Assets/Resources/Scripts/EventSystem/`）：

1. **`GameEventData.cs`** — 事件数据类 + 枚举
2. **`GameEventLog.cs`** — 事件日志主类（生命周期 + 记录接口 + dev 快照 diff + 过期清理）

### GameEventData 字段（全 int，无 string 字段）

```
int eventId              // 自增唯一 ID
GameEventType eventType  // 事件类型枚举
int year                 // 游戏年份（BASE_YEAR + round/SEASONS_PER_YEAR），用于展示
int round                // 原始回合数，用于过期判断
int forceId              // 主要势力
int relatedForceId       // 关联势力（战斗对手）
int cityId               // 主要城市 ID
List<int> heroIds        // 主要英雄列表
List<int> relatedHeroIds // 关联英雄列表（防御方、招募目标等）
int devId                // 委派/行为 ID（dev/kingaction 用）
int intParam             // 通用整型参数，按 eventType 区分含义（见下表）
```

**intParam 语义表**（按 eventType）：

| eventType | intParam 含义 |
|---|---|
| BattleAttack | 未用 (0) |
| BattleDefend | 未用 (0) |
| BattleResult | 1=攻方胜, 0=攻方败 |
| Dev | 0=assign, 1=cancel, 2=change |
| KingActionMove | 目标城市 ID（relatedCity） |
| KingActionTrade | 1=买兵, 0=卖粮 |
| KingActionSearch | 未用 (0) |
| KingActionRecruit | 1=成功, 0=失败 |
| KingActionPraise | methodId（1=praise, 2=reward） |
| Capture | 未用 (0) |
| Wild | 未用 (0) |
| Escape | 目标城市 ID（relatedCity） |
| RecruitSuccess | 未用 (0) |

### GameEventType 枚举

```
BattleAttack      // 战斗-进攻方（开始时记录 herolist）
BattleDefend      // 战斗-防御方（开始时记录 herolist）
BattleResult      // 战斗结果（结束时补记胜负）
Dev               // 委派（回合末 diff 记录；intParam: 0=assign 1=cancel 2=change）
KingActionMove    // 移动英雄
KingActionTrade   // 交易
KingActionSearch  // 搜索
KingActionRecruit // 登庸
KingActionPraise  // 赏赐
Capture           // 被俘虏
Wild              // 下野（新发现英雄初始状态）
Escape            // 逃脱（俘虏 → normal）
RecruitSuccess    // 招募成功（wild/catched → normal，force 变更）
```

### GameEventLog 类

```
public class GameEventLog {
    public List<GameEventData> events = new List<GameEventData>();
    public int nextEventId = 1;
    // dev 快照：heroId → (devId, cityId)，用于回合末 diff
    [NonSerialized] private Dictionary<int, DevSnapshotEntry> lastDevSnapshot = new ...;

    // 生命周期（由 GameManager 同步调用）
    public void OnNewGame()                       // 清空 events，构建初始 dev 快照
    public void InitLoadedData()                  // 加载后重建 dev 快照（基于当前 SaveData 状态）
    public void BeforeSave()                      // 保存前清理（暂无操作，预留）
    public void OnRoundEnd(int finishedRound)     // 回合末：dev diff 记录 + 过期清理

    // 低层记录接口
    public void RecordEvent(GameEventData data)   // 统一入口，自动填 eventId

    // 高层便捷工厂（在 GameEventData 上以 static CreateXxx 形式实现）
    // 调用方: GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateXxx(...))
}
```

**Dev 快照 diff 流程**（在 `OnRoundEnd(finishedRound)` 中）：
1. 遍历 `SaveData.cities` 的所有 `devAssignments`，构建 `currentSnapshot: heroId → (devId, cityId)`
2. 与 `lastDevSnapshot` 比较：
   - current 有、last 无 → Dev 事件（intParam=0 assign，devId=new，cityId=current.cityId）
   - current 无、last 有 → Dev 事件（intParam=1 cancel，devId=0，cityId=last.cityId）
   - 都有但 devId 不同 → Dev 事件（intParam=2 change，devId=new，cityId=current.cityId）
3. 用 finishedRound 作为事件 round（这是刚结束的那一回合）
4. `lastDevSnapshot = currentSnapshot`
5. 过期清理：从 events 列表头（最旧）开始，移除 `event.round < finishedRound - SEASONS_PER_YEAR` 的项，遇到第一个未过期项停止（队列头检查）

### GameManager 集成

在 [GameManager.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Controls/GameManager.cs) 修改：

1. **新增字段**（与 `SaveData` 平级，line 12 旁）：
   ```
   public GameEventLog GameEventLog;
   ```

2. **NewGame** (line 137)：SaveData 创建后追加
   ```
   GameEventLog = new GameEventLog();
   GameEventLog.OnNewGame();
   ```

3. **LoadFromSave** (line 378)：在 SaveData 加载后，独立加载事件文件
   ```
   // 读 game_events.json → JsonUtility.FromJson<GameEventLog>
   // 失败则 new GameEventLog()（向后兼容旧存档）
   GameEventLog.InitLoadedData();
   ```

4. **SaveToFile** (line 403)：在 SaveData 保存后，独立保存事件文件
   ```
   GameEventLog.BeforeSave();
   // JsonUtility.ToJson(GameEventLog) → 写 game_events.json
   ```

5. **NextRound** (line 146)：在 `SaveData.OnRound()` 之前调用
   ```
   GameEventLog.OnRoundEnd(SaveData.round);  // 用当前 round（刚结束的回合）
   SaveData.OnRound();
   ...
   ```

### 事件触发点（Hook 点）

#### 战斗（[SaveForceData.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs)）

**`ExecuteBattle` (line 493)**：在 `defenceTroops` 最终确定后（约 line 580 之后、`BattleBegin` 调用前），记两条事件：
- `BattleAttack`：forceId=srcForceId, relatedForceId=destForceId, cityId=targetCityId, heroIds=validAttackTroops 的 heroId1 列表
- `BattleDefend`：forceId=destForceId, relatedForceId=srcForceId, cityId=targetCityId, heroIds=defenceTroops 的 heroId1 列表

**`OnBattleEnd` (line 592)**：在回调开头记一条：
- `BattleResult`：forceId=srcForceId, relatedForceId=destForceId, cityId=targetCityId, heroIds=攻方 heroId, relatedHeroIds=守方 heroId, intParam=result==Win?1:0

#### KingAction（[SaveForceData.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs)）

立即记录（执行方法体内、return 成功路径上）：
- **`MoveHeroToCity` (line 661)**：`KingActionMove`，heroIds=传入的 heroIds，cityId=srcCityId，intParam=destCityId，forceId=this.forceId
- **`ExecuteCityTrade` (line 752)**：`KingActionTrade`，heroIds=heroIds，cityId=cityId，devId=devId，intParam=buySoldier?1:0，forceId=this.forceId
- **`ExecuteCitySearch` (line 827)**：`KingActionSearch`，heroIds=heroIds，cityId=cityId，devId=devId，forceId=this.forceId
- **`ExecuteCityUseHero` (line 1036)**：`KingActionRecruit`，heroIds=myHeroIds，relatedHeroIds=targetHeroIds，cityId=cityId，forceId=this.forceId，intParam=success?1:0
- **`ExecuteCityPraiseHero` (line 1120)**：`KingActionPraise`，heroIds=heroList，cityId=cityId，devId=devId，intParam=methodId，forceId=this.forceId

#### 状态变化

- **Capture**（[SaveCityData.Occupy](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveCityData.cs) line 460/481/491）：每次 `hero.state = HeroState.Catched` 后记 `Capture`，heroIds={heroId}，forceId=forceWin（俘虏方），relatedForceId=forceLose，cityId=该城市 ID
- **Wild**（[SaveForceData.FindUndiscoveredHero](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs) line 1011 附近，新增 hero 到 heros 列表后）：`Wild`，heroIds={新 heroId}，forceId=发现方 forceId，cityId=城市 ID
- **Escape**（[SaveData.ProcessHeros](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveData.cs) line 137，hero.state 设为 Normal 后）：`Escape`，heroIds={heroId}，forceId=hero.forceId，cityId=原城市 ID，intParam=目标城市 ID
- **RecruitSuccess**（[SaveForceData.ExecuteCityUseHero](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs) line 1086 招募成功分支）：`RecruitSuccess`，heroIds={被招募 heroId}，forceId=this.forceId（新势力），relatedForceId=原 forceId，cityId=城市 ID

#### Dev 委派

不在 `SetDevAssignment/RemoveDevAssignment` 立即记录，由 `GameEventLog.OnRoundEnd` 的快照 diff 统一处理。

### 复用已有工具

- **年份计算**：复用 `SysFormula.Game.CalculateCurrentYear(round)`，取整作为 `year` 字段
- **常量**：`SystemConst.Game.SEASONS_PER_YEAR` (36)、`SystemConst.Game.BASE_YEAR` (194)
- **日志**：使用 `GameLog`（不使用 UnityEngine.Debug.Log）
- **序列化**：`JsonUtility`（与 SaveData 一致）

### 工程规范遵守

- 新文件用 PascalCase 类名、camelCase 字段
- 新增 .cs 文件在 `Assembly-CSharp.csproj` 注册 `<Compile Include>`
- 不使用属性，使用公共字段
- 日志使用 `GameLog`，消息用中文
- 不硬编码路径/颜色/数值（年份常量复用 SystemConst）

## 实现步骤

1. 创建 `Assets/Resources/Scripts/EventSystem/` 目录
2. 写 `GameEventData.cs`（枚举 + 数据类 + static 工厂方法 `CreateBattleAttack/CreateBattleDefend/CreateBattleResult/CreateDev/CreateKingActionMove/...`）
3. 写 `GameEventLog.cs`（生命周期 + RecordEvent + OnRoundEnd 的 dev diff + 过期清理 + 序列化字段）
4. 在 `Assembly-CSharp.csproj` 注册两个新文件
5. 修改 `GameManager.cs`：新增字段 + 4 个生命周期 hook
6. 修改 `SaveForceData.cs`：battle 2 个 hook + OnBattleEnd 1 个 + 5 个 KingAction hook + Wild 1 个 + RecruitSuccess 1 个
7. 修改 `SaveCityData.cs`：Occupy 中 3 处 Capture hook
8. 修改 `SaveData.cs`：ProcessHeros 中 Escape 1 处 hook
9. 编译验证 + 简单日志检查

## 验证

1. **编译**：确保 Unity 编译通过无报错
2. **新游戏**：开始新游戏 → 检查 `Application.persistentDataPath/game_events.json` 生成，初始 events 为空，dev 快照已建立
3. **战斗事件**：玩家发动一次攻城战 → 检查 events 中出现 BattleAttack + BattleDefend + BattleResult 三条
4. **Dev diff**：玩家在 Planning 阶段对一个英雄反复设置/取消 dev，ConfirmPlan 后过一回合 → 检查 events 中只出现 1 条 Dev 净变化事件（或 0 条若最终状态与上回合相同）
5. **过期清理**：连续推进 36+ 回合 → 检查旧事件被从队列头移除
6. **加载存档**：保存后重新加载 → 检查 events 列表完整恢复，dev 快照重建
7. **KingAction 各类**：执行移动/交易/搜索/登庸/赏赐 → 检查对应事件被记录
8. **状态变化**：触发俘虏/逃脱/招募成功 → 检查对应事件被记录
