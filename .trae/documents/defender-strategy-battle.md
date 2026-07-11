# 战斗模块防御方策略修改

## Context

当前战斗系统中，防御方在有城门时被完全冻结（`noActionCount=99999`），城门被破后才解冻迎敌。这种"纯龟缩"策略不够智能：
- 防御方明明兵力占优也只能被动挨打，无法主动出击扩大优势；
- 弓箭兵贴城墙时本可隔墙射击外部敌人，却被一并冻结无法输出；
- 箭塔预定格可能被防御方部署单位占用，导致箭塔无法生成。

本次修改引入双方战力估算，让防御方在战力优势足够时主动出击；战力不足时改为"弓箭兵贴墙射击、近战冻结"的智能龟缩；并禁止防御方在箭塔格部署单位，保障箭塔正常生成。

## 用户确认的关键决策

1. **战力公式**：`Σ soldierCount * (atk + def) / 2`，atk/def 来自 `SysFormula.Battle.CalculateCombatAttrForTroop`
2. **出击阈值**：防御方战力 > 攻击方战力 × 1.5 时主动出击
3. **适用范围**：所有防御方（AI 与玩家防御方都按战力自动决策）
4. **出击布阵**：AI 防御方出击时按标准部署站位（近战 row=0/row=2，远程 row=1，复用 GetAISpawnPosition 语义）
5. **龟缩布阵**：AI 防御方龟缩时弓箭兵 row=0 贴墙、近战 row=1/row=2

## 实现步骤

### 1. SystemConst.cs（`public static class Battle` 内，L278 起）

新增常量：
```csharp
public const float DEFENDER_SALLY_POWER_RATIO = 1.5f;   // 防御方主动出击的战力倍率阈值
public const int INFINITE_COUNT = 99999;                // 无限行动/移动计数（替代魔数）
```

### 2. SysFormula.cs（`public static class Battle` 内，L14 起）

新增 `CalculateForcePower`：
- 遍历 troops（截断到 `MAX_BATTLE_HEROES_PER_SIDE`，与 InitSummon 一致）
- 累加 `soldierMap[heroId1] * (atk + def) / 2`，返回 `long`
- soldierMap 为空或英雄无效时跳过

### 3. BattleManager.cs（核心修改）

**新增字段**（L107 附近）：
```csharp
[NonSerialized] private bool defenderSally = false;
```
在 BattleBegin（L128）中重置 `defenderSally = false`。

**新增 `DecideDefenderStrategy(attackSoldierMap, defenderSoldierMap)`**：
- 调用 `SysFormula.Battle.CalculateForcePower` 计算双方战力
- `defenderSally = defPower > (long)(atkPower * DEFENDER_SALLY_POWER_RATIO)`
- 在 InitSummon 开头调用

**新增 `DefenderTowerExists()`**：`cityData.GetAttr("wall") >= SystemConst.City.TOWER_MIN_WALL`

**新增 `IsTowerSpawnGrid(gx, gz)`**：`playerSideIndex == 1 && DefenderTowerExists() && gx == DEPLOY_SIDE2_BASE_GX && gz == DEPLOY_SIDE2_BASE_GZ + 2`

**新增 `GetDefenderSpawnPosition(troops, index, sally, ref int[] rowCounts)`**：
- sally 模式行优先级：远程 {1,2,0}、近战 {0,2,1}（复用 GetAISpawnPosition 语义）
- hold 模式行优先级：远程 {0,1,2}（贴墙）、近战 {1,2,0}
- row=0 且有箭塔时跳过 col=1（可用列序列 {0,2,3,4}），用 `GetDefenderCol` 映射
- 溢出（15 英雄 + 箭塔 = 14 格 < 15）回退到箭塔格并 `GameLog.Warn`

**修改 `InitSummon`（L850-900）**：
- 开头调用 `DecideDefenderStrategy`
- 防御方 AI 布阵：`GetSpawnPosition(2, 2, defCol%cols)` → `GetDefenderSpawnPosition(defenderTroops, i, defenderSally, ref defRowCounts)`
- `noActionCount: 99999` → `noActionCount: SystemConst.Battle.INFINITE_COUNT`
- 玩家防御方仍用 playerDeployPositions（不变）

**修改 `FreezeDefenders`（L1148-1162）**：
- `defenderSally || !hasGate`：`noActionCount=0, noMoveCount=0`（出击/无城门自由行动）
- 龟缩（!sally && hasGate）：
  - 远程（`ArmsConfig.Range >= RANGE_ATTACK_THRESHOLD`）：`noActionCount=0, noMoveCount=INFINITE_COUNT`（可攻击不动）
  - 近战：`noActionCount=INFINITE_COUNT`（冻结）
- 末尾加 `GameLog.Info` 记录策略

**修改 `UnfreezeDefendersIfNoGates`（L1164-1173）**：新增 `chess.noMoveCount = 0`（龟缩弓箭兵城门被破后需解除移动限制，否则仍不能动）。保留既有"任一城门死亡即解冻全部"行为。

**修改 `StartDeployPhase`（L258-265）**：玩家防御方初始放置跳过箭塔格（row=0,col=1），改为双重循环按格放置。

**修改 `FillEmptyGridsWithSodNull`（L311-335）**：内层循环增加 `if (IsTowerSpawnGrid(gx, gz)) continue;`

**修改 `Update` 拖拽（L448）**：`if (IsInPlayerDeployArea(gx, gz) && !IsTowerSpawnGrid(gx, gz))`

### 4. CreateChessAction.cs（L99，可选）

`chessObj.noMoveCount = 99999` → `SystemConst.Battle.INFINITE_COUNT`（箭塔，保持一致性）

## 关键时序与风险

- **时序安全**：InitSummon 在 tick=10 创建棋子（防御方最晚 tick≈20），FreezeDefenders 在 tick=30 配置冻结，所有棋子已就位。
- **弓箭兵龟缩射击可行**：MoveAndFight 先检查 noActionCount=0（通过），射程内直接攻击 return（L573），不走到移动检查（L592）。GetUnitsInRange 不过滤墙的视线，可隔墙攻击。
- **城门通行已支持**：IsGridBlockedByObstacle 中友方城门放行，出击防御方可从城门格子穿过。
- **溢出兜底**：15 英雄 + 箭塔时回退到箭塔格（友方不互挡，仅视觉重叠），记录 Warn。
- **回放不受影响**：回放跳过 InitSummon/FreezeDefenders/ProcessTurnState，noActionCount/noMoveCount 由序列化数据恢复但不实际生效。`defenderSally` 标 `[NonSerialized]`。
- **数据一致性**：DefenderTowerExists 读 `cityData.wall`，战斗中不变，与 InitWallsAndGates 判断一致。

## 涉及文件

- [SystemConst.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SystemConst.cs)（`Battle` 嵌套类新增 2 常量）
- [SysFormula.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs)（`Battle` 嵌套类新增 `CalculateForcePower`）
- [BattleManager.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Controls/BattleManager.cs)（核心：字段、DecideDefenderStrategy、GetDefenderSpawnPosition、FreezeDefenders、UnfreezeDefendersIfNoGates、InitSummon、StartDeployPhase、FillEmptyGridsWithSodNull、Update 拖拽）
- [CreateChessAction.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Combat/Actions/CreateChessAction.cs)（L99 魔数替换，可选）

## 验证方式

1. **编译**：Unity 编译无错误（注意 BattleManager 新增方法签名）。
2. **AI 防御方出击场景**：构造防御方兵力远超攻击方（如防御 1500 兵 vs 攻击 500 兵），城防≥100 有城门。战斗开始后观察：
   - GameLog 出现 `防御方策略决策 ... sally=True`
   - 防御方单位从城门主动冲出迎敌（noActionCount=0）
3. **AI 防御方龟缩场景**：构造防御方兵力略低于或等于攻击方，城防≥100。观察：
   - `sally=False`
   - 弓箭兵在 row=0 贴墙、原地射击外部敌人不移动
   - 近战冻结不动
   - 攻击方破城门后，防御方全员（含弓箭兵）开始移动迎敌
4. **箭塔格验证**：城防≥300 时，防御方布阵阶段（玩家防御方）无法将单位放到 row=0 col=1 格；AI 防御方布阵避开该格；箭塔正常生成。
5. **回归**：无城防（wall<100）时无墙/门/塔，防御方自由行动（sally 判断仍执行但 hasGate=false 走自由分支）。
