# 非战斗类数值计算整理

> 整理 SanKingdom 项目中不在 `CityDevKingActionConfig` 配置表内、非战斗层、非 UI 布局的数值计算过程。
> 配置表 `CityDevKingActionConfig` 仅提供参数（`BaseRate / AttrHighBound / BonusPerPoint / KingBonus / EffectMin/Max / Effect2Min/Max`），实际公式逻辑集中在 `SysFormula` + `SaveForceData` 中。
> 科技影响列标注该计算是否受科技加成（ForceTech）影响及加成类型。

---

## 一、忠心度相关计算

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 俘虏忠心每回合自然降低 | `SysFormula.Hero.GetSysConfigModifyResult("CapturedLoyaltyDecay", forceId)`：BaseVal + Random(RandomMin, RandomMax+1) | AmountMul+AmountAdd | [SysFormula.cs:359-375](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L359-L375) |
| 褒奖/奖赏忠心提升量 | `SysRandom.Range(kingCfg.EffectMin, kingCfg.EffectMax+1)`，封顶 100 | AmountMul | [SaveForceData.cs:1344-1349](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L1344-L1349) |
| 扰乱导致忠心降低 | `SysRandom.Range(Effect2Min, Effect2Max+1)`，主公免疫 | AmountMul | [SaveForceData.cs:1693](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L1693) |

---

## 二、概率类计算（被抓 / 逃脱 / 移动）

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 被抓概率 | `CalculateCaptureChance(str, forceId)`：CaptureBaseChance - max(0, str-70)/6 | AmountMul+AmountAdd（CaptureBaseChance） | [SysFormula.cs:351-357](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L351-L357) |
| 俘虏逃脱概率 | `SysFormula.Hero.GetSysConfigModifyResult("EscapeChance", forceId)`：BaseVal=20，RandomMin=RandomMax=0 | AmountMul+AmountAdd | [SysFormula.cs:359-375](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L359-L375) |
| 在野武将随机迁移概率 | `SysFormula.Hero.GetSysConfigModifyResult("WildHeroMoveChance", forceId)`：BaseVal=20，RandomMin=RandomMax=0 | AmountMul+AmountAdd | [SysFormula.cs:359-375](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L359-L375) |
| 登庸成功率（在野） | 基础 30%，非己方城市 ×0.5（state 判断在 SysFormula 外部） | SuccessMul | [SysFormula.cs:211-224](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L211-L224) |
| 登庸成功率（俘虏/敌方） | `diff * 3/4 - 5`（diff = 100 - 忠诚），最低 0（state 判断在 SysFormula 外部） | SuccessMul | [SysFormula.cs:231-241](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L231-L241) |

---

## 三、KingAction 成功率与加成（基于 CityDevKingActionConfig 参数计算）

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| KingAction 基础成功率 | `BaseRate×100 + 派系相同(+5) + 每个相同爱好(+1)（NeedAdditiveBonus 时）+ (attr-AttrHighBound)×BonusPerPoint×100 + 君主KingBonus×100`，封顶 100 | SuccessMul | [SysFormula.cs:263-335](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L263-L335) |

---

## 四、日程距离与跨国判定

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 武将移动日程 | `SysFormula.City.CalculateMoveDayDistance`：`ceil(曼哈顿距离 / MoveBaseDist)`，敌方城市距离×1.5 且至少 2 天 | AmountMul+AmountAdd（MoveBaseDist） | [SysFormula.cs:478-488](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L478-L488) |
| 登庸日程 | `SysFormula.City.CalculateRecruitDayDistance`：`ceil(曼哈顿距离 / RecruitBaseDist)`，敌方城市距离×1.5 且至少 2 天 | AmountMul+AmountAdd（RecruitBaseDist） | [SysFormula.cs:493-503](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L493-L503) |

---

## 五、城市发展产出

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 城市产出倍率 | `CalculateProductionMultiplier(happy, isInWar, defenceDevDiscount)`：民心分级（≥95=1.2, ≥60=1.0, ≥30=0.8, 否则0.6）→ 战争叠加 WAR_PRODUCTION_MULTIPLIER-1 → × 防御打折 | — | [SysFormula.cs:525-542](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L525-L542) |
| 战斗导致 dev 收入打折 | 10 回合=1.0，30 回合=0，中间线性插值 | — | [SysFormula.cs:560-570](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L560-L570) |
| 战斗过多民心衰减 | `(round - 10) × 1f`（round > 10 时生效） | — | [SaveForceData.cs:704-708](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L704-L708) |
| Dev 产出基础值 | `max(min, addon/100 × max)`，封顶 `valMax - currentVal` | — | [SysFormula.cs:488-492](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L488-L492) |
---

## 六、经济 / 交易

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 交易基础量 | `goldCost × 2` | — | [SysFormula.cs:575-578](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L575-L578) |
| 武将交易量 | 基础量 × (1 + max(0, inte-70) × 0.02) | AmountMul | [SysFormula.cs:583-589](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L583-L589) |

---

## 七、外交 / 友好度

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 战斗导致关系下降 | `GetSysConfigModifyResult("BattleRelationRise", forceId)`：BaseVal=0，RandomMin=3，RandomMax=7 | AmountMul+AmountAdd | [SysFormula.cs:359-375](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L359-L375) |
| 亲善关系提升 | `SysRandom.Range(EffectMin, EffectMax+1)`（EffectMin/Max=10），需要 KingAction 成功率判定 | SuccessMul（成功率部分） | [SaveForceData.cs:1819](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L1819) |
| 挑拨关系下降 | 同上公式，但是 -totalRelationChange | SuccessMul（成功率部分） | [SaveForceData.cs:1921](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L1921) |

---

## 八、走访搜索

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 走访资源量 | `SysRandom.Range(AttrValMin, AttrValMax+1)` | — | [SaveForceData.cs:1060](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L1060) |
| 走访结果选择 | 加权随机（按 `Weight` 累计分布抽取） | — | [SaveForceData.cs:1018-1033](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SaveDatas/SaveForceData.cs#L1018-L1033) |

---

## 九、其他通用计算

| 计算项 | 公式 / 逻辑 | 科技影响 | 代码位置 |
|---|---|---|---|
| 武将属性成长 | `Max(8×(level-1), baseAttr×(level-1)/10)`，level≤1 返回 0 | — | [SysFormula.cs:393-397](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L393-L397) |
| 次要属性贡献值 | 次属性 > 主属性时 = `(次-主)/3` | — | [SysFormula.cs:484-489](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L484-L489) |
| 武将条件判定 | 解析 `"inte>=90"` 等字符串，支持 `>=/<=/==/!=/>/<` | — | [SysFormula.cs:163-204](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/SystemTool/SysFormula.cs#L163-L204) |

