# 兵种技能设计

## 1. 设计概述

### 1.1 核心规则

每个兵种拥有 **3 个技能**，分别对应兵种适应等级的 **B / A / S**：

| 技能槽 | 兵种适应要求 | 说明 |
|--------|-------------|------|
| 技能 1 | B（驾驭值 7-8） | 该兵种的基础战法 |
| 技能 2 | A（驾驭值 9） | 进阶战法 |
| 技能 3 | S（驾驭值 10） | 顶级战法 |

> 兵种适应等级映射（[HeroAttrConfig_s.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Configs/HeroAttrConfig_s.cs)）：10:S、9:A、7-8:B、5-6:C、3-4:D、2:E、0-1:F。
> 技能掌握为**累加制**（已确认）：B 级掌握技能 1，A 级掌握技能 1+2，S 级掌握技能 1+2+3；C 级及以下不掌握兵种技能。
> 技能掌握判定取部队三将（heroId1/heroId2/heroId3）对该兵种的最大驾驭值（同 `SysFormula.Battle.GetTroopMaxSod` 逻辑），阈值常量见 `SystemConst.Battle.SOD_SKILL_LEVEL_S/A/B`。

### 1.2 设计原则

- **攻击触发型**：技能跟随普通攻击/技能伤害的命中而判定，符合现有 `OnAttack` 事件分发架构（[SkillManager.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Combat/Skills/SkillManager.cs)）。
- **参考三国志 11 战法**：突刺（击退）、螺旋突刺（晕眩）、火矢（火箭）、突击（越过目标）等经典战法。
- **每兵种一主控场一输出**：3 个技能在伤害、位移、控制三个维度上递进，S 级技能包含 B/A 级的效果强化。

### 1.3 现有系统映射

技能通过 [BattleSkillConfig_s.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Configs/BattleSkillConfig_s.cs) 配置，字段含义：

| 字段 | 含义 |
|------|------|
| `Rate` | 发动概率 |
| `RoundCd` | 发动 CD（回合数） |
| `BuffId` | 附加 Buff（晕眩=301001 混乱、减速=301004、溃败=301006 等） |
| `BuffTime` | Buff 持续回合数 |
| `SkillDamageRate` | 技能伤害比例 |
| `SkillDamageAttrRate` | 技能伤害属性比例（用于 DOT/区域） |
| `ScriptName` | 技能脚本（需在 [SkillManager.CreateSkill](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Combat/Skills/SkillManager.cs#L10-L85) 注册） |
| `TargetCount` / `Range` | 目标数 / 范围 |

## 2. 技能总览

| 兵种 | 技能 1（B） | 技能 2（A） | 技能 3（S） |
|------|------------|------------|------------|
| 枪兵 | 突刺 · 击退 | 螺旋突刺 · 晕眩 | 二段突刺 · 强力击退 |
| 戟兵 | 钩镰 · 拉拽 | 戟阵 · 周边范围 | 旋风斩 · 周身范围 |
| 骑兵 | 突击 · 跃过 | 突破 · 推挤 | 铁骑冲锋 · 击退+晕眩 |
| 弓兵 | 火箭 · 灼烧 | 贯矢 · 直线穿透 | 乱射 · 范围随机（不分敌我） |
| 刀兵 | 重斩 · 增伤 | 反戈 · 受击反击 | 铁壁 · 受击护盾 |
| 象兵 | 践踏 · 范围 | 冲撞 · 击退 | 象鸣 · 震慑混乱 |

## 3. 各兵种技能详解

### 3.1 枪兵（ArmsId 602，SodSpear）

| | 技能 1：突刺 | 技能 2：螺旋突刺 | 技能 3：二段突刺 |
|---|---|---|---|
| 适应要求 | B | A | S |
| 参考 311 | 突刺 | 螺旋突刺 | 二段突刺 |
| 效果 | 攻击时几率将目标**击退 1 格**（位移至施法者背向的下一格） | 攻击时几率使目标**晕眩 1 回合**（附加 Buff 301001 混乱，无法行动） | 攻击时几率将目标**击退 2 格**，并造成额外伤害 |
| 发动概率 | 30% | 20% | 25% |
| CD | 3 回合 | 4 回合 | 5 回合 |
| 伤害 | 0% | 0% | 150%（SkillDamageRate） |
| 位移 | 击退 1 格 | 无 | 击退 2 格 |
| 新增脚本 | `HitKnockBack` | `HitBuff`（复用） | `HitKnockBack`（Strength 控制格数） |

### 3.2 戟兵（ArmsId 603，SodHalberd）

| | 技能 1：钩镰 | 技能 2：戟阵 | 技能 3：旋风斩 |
|---|---|---|---|
| 适应要求 | B | A | S |
| 参考 311 | 戟 · 钩 | 横斩 | 旋风 |
| 效果 | 攻击时几率将目标**拉拽 1 格**靠近自己（与击退反向位移） | 攻击目标时对**周边敌人**造成伤害（Range 2，最多 2 个） | 攻击时几率对**附近敌人**造成伤害（Range 1，最多 5 个） |
| 发动概率 | 35% | 35%（Rate=0.35） | 40%（Rate=0.4） |
| CD | 3 回合 | 4 回合 | 5 回合 |
| 伤害 | 0% | 60%（SkillDamageRate=0.6） | 80%（SkillDamageRate=0.8） |
| 相关属性 | — | leadShip | str |
| 脚本 | `HitPull`（新增） | `HitAround`（复用 [200015 戟阵](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Configs/BattleSkillConfig_s.cs)） | `AttackSpinAttack`（复用 [201010 旋风斩](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Configs/BattleSkillConfig_s.cs)） |

### 3.3 骑兵（ArmsId 101，SodHorse）

| | 技能 1：突击 | 技能 2：突破 | 技能 3：铁骑冲锋 |
|---|---|---|---|
| 适应要求 | B | A | S |
| 参考 311 | 突击 | 突破 | 突进 |
| 效果 | 攻击时几率**跃过目标**，落位至目标身后一格（越过后进入攻击射程内） | 攻击时几率将目标**推挤 1 格**后退并造成额外伤害 | 攻击时几率将目标**击退 1 格并晕眩**（Buff 301001），造成高额伤害 |
| 发动概率 | 35% | 30% | 20% |
| CD | 3 回合 | 4 回合 | 6 回合 |
| 伤害 | 0% | 130% | 180% |
| Buff | 无 | 无 | 混乱（301001，BuffTime 1） |
| 位移 | 施法者位移 | 目标位移（推挤） | 目标位移（击退） |
| 新增脚本 | `SkillJumpOver` | `HitPush` | `HitKnockBack` + `HitBuff` 组合 |

> 突击（跃过）参考战斗系统已有 `Chess.JumpToPosition` / `MoveAction`，需新增“跃过目标落位身后格”的位移 Action。落点格被占用/不可通行时**位移失败**，不落位至其他任意格。

### 3.4 弓兵（ArmsId 201，SodBow）

| | 技能 1：火箭 | 技能 2：贯矢 | 技能 3：乱射 |
|---|---|---|---|
| 适应要求 | B | A | S |
| 参考 311 | 火矢 | 贯矢 | 乱射 |
| 效果 | 攻击时几率射出**火箭**，附加灼烧（持续伤害，参考火计 SummonTag 火 / 溃败 301006） | 攻击时几率射出**穿透箭**，对目标后方直线（Range 3）多个敌人造成伤害，**伤害逐目标递减（递减系数走配置）** | 攻击时几率对**以目标为中心 + Range 为半径范围**内随机数个目标各射一箭（**不分敌我**） |
| 发动概率 | 35% | 30% | 30% |
| CD | 3 回合 | 4 回合 | 5 回合 |
| 伤害 | 100% + DOT | 140% × 递减系数（配置） | 100% × 每个被选中的目标 |
| DOT | 溃败（301006）或火区域，BuffTime 3 | 无 | 无 |
| 新增脚本 | `HitWall`（复用火）+ 灼烧 | `SkillPierceArrow`（穿透，递减系数读配置） | `SkillRandomArea`（新增，目标为中心 + Range 范围随机选，不分敌我） |

### 3.5 刀兵（ArmsId 601，SodSword）

| | 技能 1：重斩 | 技能 2：反戈 | 技能 3：铁壁 |
|---|---|---|---|
| 适应要求 | B | A | S |
| 参考 311 | 突击式重击 | 反击 | 铁壁防御 |
| 效果 | 攻击时几率造成额外伤害 | 受击时几率对攻击者造成一次反击伤害 | 受击时几率获得**护盾**（Buff 300001，吸收值 = 攻击力 × 系数，系数为技能表固定配置） |
| 发动概率 | 40% | 25% | 25% |
| CD | 2 回合 | 3 回合 | 5 回合 |
| 伤害 | 150%（SkillDamageRate） | 80%（反击） | 0% |
| Buff | 无 | 无 | 护盾（300001，BuffTime 2） |
| 新增脚本 | `AttackAddDamage`（复用） | `SkillCounterAttack` | `AttackedBuff`（复用，Buff 300001） |

### 3.6 象兵（ArmsId 109，SodElephant）

| | 技能 1：践踏 | 技能 2：冲撞 | 技能 3：象鸣 |
|---|---|---|---|
| 适应要求 | B | A | S |
| 参考 311 | 范围践踏 | 冲撞 | 震慑 |
| 效果 | 攻击时几率对周身范围（Range 1）敌人造成范围伤害 | 攻击时几率将目标**击退 1 格**并造成额外伤害 | 攻击时几率使目标**晕眩 1 回合**，并对其周围敌人造成范围伤害 |
| 发动概率 | 30% | 30% | 20% |
| CD | 3 回合 | 4 回合 | 6 回合 |
| 伤害 | 120% | 150% | 160% + 范围 |
| Buff | 无 | 无 | 混乱（301001，BuffTime 1） |
| 新增脚本 | `HitAround`（复用） | `HitKnockBack` | `HitBuffArea`（复用，Buff 301001） |

## 4. 位移机制说明

本设计引入 4 种目标/自身位移，均为现有系统未覆盖的新机制，需新增 `ChessAction` 子类（遵循 [battle skill](file:///d:/U3dPrj/SanKingdom/.trae/skills/battle) 的 Command 模式，入队而非直接改状态）：

| 位移 | 方向 | 对象 | 实现要点 |
|------|------|------|----------|
| 击退（KnockBack） | 施法者 → 目标 连线反向 1~2 格 | 目标 | 新 `DisplaceAction`，目标格需可通行 |
| 拉拽（Pull） | 目标 → 施法者 方向 1 格 | 目标 | 与击退同机制，方向取反 |
| 推挤（Push） | 与击退一致 | 目标 | 同击退 |
| 跃过（JumpOver） | 施法者越过目标至身后格 | 施法者 | 复用 `Chess.JumpToPosition`，落点需可通行 |

> 位移格判定建议走 `MapCell.Occupy` / 通行判定，若目标格被占用则位移失败（技能仍结算伤害，不结算位移）。

## 5. 配置与实现清单

### 5.1 新增技能脚本（需注册到 SkillManager.CreateSkill）

| ScriptName | 说明 |
|------------|------|
| `HitKnockBack` | 击退 1~2 格（Strength/StrengthInt 控制格数） |
| `HitPull` | 拉拽 1 格靠近 |
| `HitPush` | 推挤 1 格 |
| `SkillJumpOver` | 跃过目标落位身后 |
| `SkillPierceArrow` | 直线穿透多目标 |
| `SkillCounterAttack` | 受击反击 |
| `SkillRandomArea` | 范围内随机选目标（不分敌我），弓兵乱射 |

### 5.2 复用脚本

| ScriptName | 复用场景 |
|------------|----------|
| `HitBuff` | 枪兵螺旋突刺、骑兵铁骑冲锋（晕眩） |
| `HitAround` | 戟兵戟阵（200015）、象兵践踏 |
| `AttackSpinAttack` | 戟兵旋风斩（201010） |
| `HitWall` | 弓兵火箭（火区域） |
| `AttackAddDamage` | 刀兵重斩 |
| `AttackedBuff` | 刀兵铁壁（护盾） |

### 5.3 Buff 复用

| BuffId | 用途 |
|--------|------|
| 301001 混乱（BuffNoAction） | 晕眩控制 |
| 301004 减速（BuffSpeedDown） | 减速 |
| 301006 溃败（BuffTimeDamage） | 灼烧 DOT |
| 300001 护盾（BuffShield） | 刀兵铁壁 |

## 6. 平衡性设计原则

- **技能强度随适应等级递增**：S 级技能在伤害或控制上显著强于 B 级，鼓励武将培养高适应兵种。
- **位移与控制的 CD 高于纯伤害**：击退/晕眩类技能 CD 至少 4 回合，避免无限控。
- **概率 + CD 双门槛**：`Rate` 概率触发，`RoundCd` 限制频率，与现有 `CheckBurst` 逻辑一致。
- **范围技能伤害比例 < 单体**：范围技能 `SkillDamageRate` 不超过 120%~140%，单体高伤技能可达 150%~180%。
- **兵种克制不做硬编码**：技能效果以位移/控制/范围为核心差异，克制关系交由数值（攻防、射程、移速）体现。

## 7. 已确认设计决策

| # | 问题 | 决策 |
|---|------|------|
| 1 | 位移至不可通行格（城门/城墙/箭塔） | 位移**失败**，仅结算伤害，不结算位移 |
| 2 | 骑兵「突击·跃过」落点被占用 | **不行**，不落位至其他任意格，位移失败仅结算伤害 |
| 3 | 弓兵「贯矢」穿透伤害 | **逐目标递减**，递减系数做成配置（BattleSkillConfig） |
| 4 | 弓兵「乱射」选目标范围 | **以目标为中心 + Range 为半径**的范围，范围内随机取 TargetCount 个，不分敌我 |
| 5 | 刀兵「铁壁」护盾值公式 | **不进 SysFormula**，固定乘法系数直接配置在技能表 |
