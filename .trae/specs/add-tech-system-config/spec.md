# 科技系统配置表 Spec

## Why
当前游戏缺少势力层面的长线研究成长系统，城市发展（CityDev）只能短期迭代且受英雄配置限制。需要引入科技树作为长线成长线：玩家通过派遣英雄投入研究值推进科技，研究完成后解锁旗下技术（带来实际效果）。科技有等级、分类、前置依赖，形成有深度的科技树。科技本身不提供效果，所有实际加成由旗下技术承担。

本次 Spec **仅设计配置表框架**（TechConfig + TechSkillConfig），存档数据、研究执行逻辑、UI 面板后续单独 Spec。

## What Changes
- 新增 `Configs/TechConfig_s.cs`：科技配置表，定义科技树节点（等级、分类、前置依赖、研究值、子技术列表）
- 新增 `Configs/TechSkillConfig_s.cs`：技术配置表，定义科技旗下具体技术及其效果参数
- 修改 `Configs/ConfigManager.cs`：在 `Init()` 加载流程末尾追加 `TechConfig.Load()` 与 `TechSkillConfig.Load()`
- 在 `Assembly-CSharp.csproj` 中添加两个新文件的 `<Compile Include>`

## Impact
- Affected specs: 无已有 spec 受影响
- Affected code: `Configs/ConfigManager.cs`、`Assembly-CSharp.csproj`

## ADDED Requirements

### Requirement: 科技分类
系统 SHALL 在 TechConfig 中以字符串字段 `Category` 标识科技大类，共 4 种取值：
- `"Battle"`：战斗类，影响兵种属性、战斗伤害、英雄战力
- `"Development"`：发展类，影响资源产量、城市建设、人口
- `"Institution"`：制度类，影响忠心、登用、外交、治安
- `"Engineering"`：工程类，影响城防、建筑耐久、特殊建筑

#### Scenario: 获取科技分类
- **WHEN** 调用 `TechConfig.GetConfig(id).Category`
- **THEN** 返回该科技的分类字符串（上述 4 种之一）

### Requirement: TechConfig 科技配置表
系统 SHALL 提供 `TechConfig` 配置类（位于 `CommonConfig` 命名空间，文件 `Configs/TechConfig_s.cs`），遵循 `CityDevConfig` 模板（含 `FieldMetaInfo`/`CellMeta` 元数据、`Load`/`GetConfig`/`HasConfig`/`Refresh`/`Add`/`Remove`/`Assign`/`RebuildIndex`/`ConfigList`），包含以下公共字段：

| 字段 | 类型 | 说明 |
|------|------|------|
| `Id` | int | 科技 ID（30001+） |
| `Cname` | string | 科技中文名 |
| `Des` | string | 科技描述 |
| `Category` | string | 科技分类（4 种之一） |
| `Level` | int | 科技等级（1 起步，表示在科技树中的层级） |
| `PreTechIds` | int[] | 前置科技 ID 列表（0-2 个；等级 1 时为空数组） |
| `ResearchValue` | int | 研究完成所需总研究值 |
| `SkillIds` | int[] | 旗下技术 ID 列表（1-3 个） |
| `Icon` | string | 科技图标资源名 |
| `IsSpecial` | bool | 是否为势力特有科技 |
| `AiWeight` | float | AI 研究优先级权重 |

`GetConfig` 找不到时抛 `NullReferenceException`，不返回 null。`PreTechIds`/`SkillIds` 无元素时为空数组 `new int[0]`，不为 null。

#### Scenario: 获取科技配置
- **WHEN** 调用 `TechConfig.GetConfig(30001)`
- **THEN** 返回对应 TechConfig 实例，含上述全部字段

#### Scenario: 配置不存在时抛异常
- **WHEN** 调用 `TechConfig.GetConfig(99999)`（不存在）
- **THEN** 抛出 `NullReferenceException`，消息含 "配置表TechConfig不存在id=99999"

### Requirement: TechSkillConfig 技术配置表
系统 SHALL 提供 `TechSkillConfig` 配置类（位于 `CommonConfig` 命名空间，文件 `Configs/TechSkillConfig_s.cs`），遵循 `CityDevConfig` 模板，包含以下公共字段：

| 字段 | 类型 | 说明 |
|------|------|------|
| `Id` | int | 技术 ID（31001+） |
| `Cname` | string | 技术中文名 |
| `Des` | string | 技术描述 |
| `TechId` | int | 所属科技 ID（外键指向 `TechConfig.Id`） |
| `Category` | string | 技术分类（5 种之一，见下） |
| `EffectType` | string | 效果类型（6 种之一，见下） |
| `EffectTarget` | string | 效果目标范围 |
| `EffectAttr` | string | 受影响属性名（Attr 类效果使用） |
| `EffectValue` | float[] | 效果数值数组（支持多维：[固定值, 百分比]） |
| `EffectOp` | string | 运算符：`"Add"` 或 `"Mul"` |
| `EffectId` | int | 关联实体 ID（Unlock/ArmsSkillEnhance/ForceBuff 类使用） |
| `ResearchValue` | int | 技术研究所需研究值 |
| `Icon` | string | 技术图标资源名 |

**技术分类 `Category`（5 种）**：
- `"Arms"`：兵种类 — 兵种属性加成 / 兵种技能提升 / 兵种解锁
- `"Yield"`：产出类 — 城市/资源产出加成
- `"Defense"`：防御类 — 城防/守备相关
- `"Policy"`：政策类 — 势力级 buff / 制度效果
- `"Unlock"`：解锁类 — 解锁特殊建筑

**效果类型 `EffectType`（6 种）**：
- `"ArmsAttr"`：兵种属性加成（`EffectTarget`=ArmsType 名如 `SodHorse` 或具体 armsId；`EffectAttr`=Atk/Def/MoveSpeed/Range/MissileSpeed）
- `"ArmsSkillEnhance"`：兵种技能提升（`EffectTarget`=ArmsType 名；`EffectId`=skillId，将技能赋予该兵种）
- `"CityAttr"`：城市属性加成（`EffectAttr`=food/gold/soldier/wall/happy/exp/steel/wood/stone/horse/elephant/fish/salt）
- `"ForceBuff"`：势力级 buff（`EffectId`=buffId，全势力部队获得该 buff）
- `"UnlockArms"`：解锁兵种（`EffectId`=armsId）
- `"UnlockBuilding"`：解锁建筑（`EffectId`=CityDevConfig.Id）

**禁止事项**：禁止使用 `HeroAttr`（不挂钩武将属性）和 `BattleSkill`（不解锁兵种战法，仅通过 `ArmsSkillEnhance` 提升兵种技能）。

`GetConfig` 找不到时抛 `NullReferenceException`，消息含 "配置表TechSkillConfig不存在id=xxx"。

#### Scenario: 获取技术配置
- **WHEN** 调用 `TechSkillConfig.GetConfig(31001)`
- **THEN** 返回对应 TechSkillConfig 实例

#### Scenario: 查询科技旗下所有技术
- **WHEN** 遍历 `TechSkillConfig.ConfigList` 过滤 `TechId == techId`
- **THEN** 返回该科技下所有技术配置（1-3 个）

#### Scenario: 跨类联动（胡服骑射）
- **WHEN** 查询 Institution 类科技「胡服骑射」（30203）旗下技术
- **THEN** 含 `Category="Arms"` 的技术（如「骑射改革」，EffectType=ArmsSkillEnhance），体现制度改革推动军事效果

### Requirement: ID 分配规则
系统 SHALL 按以下规则分配科技与技术 ID：

TechConfig ID（30000+），按 Category 分段，每类预留 10 槽位：
- Battle：30001-30010
- Development：30101-30110
- Institution：30201-30210
- Engineering：30400-30410（L1 从 30400 起，L2a/L2b 含 30407）

TechSkillConfig ID（31000+），按父科技 ID 段对齐：
- Battle 系父科技（30001-30006）旗下技术：31001-31010
- Development 系父科技（30101-30106）旗下技术：31101-31110
- Institution 系父科技（30201-30206）旗下技术：31201-31210
- Engineering 系父科技（30400-30407）旗下技术：31400-31410

#### Scenario: 科技 ID 与分类对应
- **WHEN** 查询 ID=30203 的科技
- **THEN** 该科技属于 Institution 分类

### Requirement: 科技树 5 级 + 2 路分支结构
系统 SHALL 在 TechConfig 数据中为每个 Category 构建如下科技树结构（每类 6 个科技）：

```
L1 ── L2 ──┬── L3a ──┐
           └── L3b ──┴── L4 ── L5
```

- **L1**：无前置依赖（`PreTechIds` 为空数组）
- **L2**：前置依赖 1 个 L1 科技
- **L3a / L3b**：均前置依赖同一个 L2 科技；L3a 与 L3b **可并行研究**（互不依赖）
- **L4**：前置依赖 L3a 和 L3b（汇合点，需两路都完成）
- **L5**：前置依赖 L4

跨类联动：L3a/L3b/L4/L5 中可包含跨类前置依赖（如 Institution 类的 L3a 可前置依赖 Battle 类的 L2），体现"不同系科技可解锁不同系技术"。

#### Scenario: L1 无前置
- **WHEN** 查询任意 `Level=1` 的科技
- **THEN** `PreTechIds` 为空数组

#### Scenario: L3a 与 L3b 可并行
- **WHEN** 查询某类 L3a 和 L3b 科技
- **THEN** 两者的 `PreTechIds` 均只含同一个 L2 科技 ID，且互不包含对方 ID

#### Scenario: L4 汇合两路
- **WHEN** 查询某类 L4 科技
- **THEN** `PreTechIds` 同时包含该类的 L3a 和 L3b 两个 ID

#### Scenario: 每类 6 个科技
- **WHEN** 遍历 `TechConfig.ConfigList` 按 Category 分组
- **THEN** 每个 Category 恰好含 6 个科技（L1/L2/L3a/L3b/L4/L5 各 1 个）

### Requirement: 前置依赖规则
系统 SHALL 在 TechConfig 数据中遵循以下前置依赖约束：
- 等级 1 的科技无前置依赖（`PreTechIds` 为空数组）
- 等级 N（N≥2）的科技前置依赖 1-2 个等级为 `N-1` 或 `N-2` 的科技
- 前置依赖可在同分类或跨分类
- 数据中不允许出现循环依赖
- 每个科技的 `SkillIds` 数量 ∈ [1, 3]

#### Scenario: 等级 N 科技依赖等级 N-1/N-2
- **WHEN** 查询 `Level=4` 的科技
- **THEN** `PreTechIds` 中所有 ID 对应的科技 `Level ∈ {2, 3}`，且数量 ≤ 2

### Requirement: ConfigManager 集成
系统 SHALL 在 `ConfigManager.Init()` 中追加 TechConfig 与 TechSkillConfig 的加载：
- 在 `FairConfig.Load()` 之后调用 `TechConfig.Load()` 和 `TechSkillConfig.Load()`
- 加载顺序在所有依赖配置（ArmsConfig、HeroConfig、CityDevConfig 等）之后，便于后续样例数据引用已有实体 ID

#### Scenario: 配置管理器初始化
- **WHEN** 调用 `ConfigManager.Init()`
- **THEN** `TechConfig.ConfigList` 与 `TechSkillConfig.ConfigList` 均非空，且包含样例数据

### Requirement: 样例数据覆盖度
系统 SHALL 在 `Load()` 中填充足够样例数据以验证设计：
- TechConfig 样例覆盖全部 4 种 `Category`，Battle/Development/Institution 每类 6 个科技（L1/L2/L3a/L3b/L4/L5），Engineering 8 个科技（L1/L2a/L2b/L3a/L3b/L4/L5/L6），共 26 个科技
- TechConfig 样例覆盖含/无前置依赖、1-3 个 SkillIds 各种组合
- TechConfig 样例中至少有 1 处跨类前置依赖（如 Institution 类 L3a 前置依赖 Battle 类 L2）
- TechSkillConfig 样例覆盖全部 5 种技术 `Category`（Arms/Yield/Defense/Policy/Unlock）
- TechSkillConfig 样例覆盖全部 6 种 `EffectType`（ArmsAttr/ArmsSkillEnhance/CityAttr/ForceBuff/UnlockArms/UnlockBuilding）
- TechSkillConfig 样例的 `TechId` 必须与 TechConfig 样例的 `SkillIds` 双向对应（父子关系一致）
- TechSkillConfig 样例中至少有 1 处跨类联动（如 Institution 类科技旗下含 `Category="Arms"` 的技术，模拟「胡服骑射」场景）

#### Scenario: 样例覆盖所有科技分类
- **WHEN** 遍历 `TechConfig.ConfigList` 按 Category 去重
- **THEN** 集合包含 Battle/Development/Institution/Engineering 全部 4 种

#### Scenario: 样例覆盖所有技术分类
- **WHEN** 遍历 `TechSkillConfig.ConfigList` 按 Category 去重
- **THEN** 集合包含 Arms/Yield/Defense/Policy/Unlock 全部 5 种

#### Scenario: 样例覆盖所有效果类型
- **WHEN** 遍历 `TechSkillConfig.ConfigList` 按 EffectType 去重
- **THEN** 集合包含 ArmsAttr/ArmsSkillEnhance/CityAttr/ForceBuff/UnlockArms/UnlockBuilding 全部 6 种

#### Scenario: 样例覆盖 5 级科技
- **WHEN** 遍历 `TechConfig.ConfigList` 按 Level 去重
- **THEN** 集合包含 1/2/3/4/5 全部 5 个等级

### Requirement: 三11设计融入点（v2.1）
系统 SHALL 在 v2 框架（4 大类 + 2 路分支 + 26 科技 + 34 技术）基础上，融入以下三11（三国志11）设计点，**不改变 v2 框架结构**，仅在样例数据描述与效果参数中体现：

#### 融入点 1：战术规则 ArmsSkillEnhance
在 TechSkillConfig 样例中，`EffectType="ArmsSkillEnhance"` 类技术的 `Des` 字段 SHALL 体现三11的具象战术规则命名，使玩家直观理解战术效果：

| 技术 ID | 名称 | 描述 | 战术规则来源 |
|---------|------|------|------------|
| 31006 | 百炼刀 | 步兵获得奇袭战法（森林地形免反击） | 三11枪兵系 Lv3 奇袭 |
| 31007 | 明光铠 | 步兵获得矢盾战法（30% 弓矢免伤） | 三11戟兵系 Lv3 矢盾 |
| 31405 | 床弩 | 弓兵获得应射战法（攻击免反击） | 三11弩兵系 Lv3 应射 |

#### 融入点 2：精锐兵种 Lv5 模式
Lv5 旗下技术 SHALL 采用三11「精锐X」命名与全面提升效果模式：
- 31008 铁骑（Lv5 重骑兵旗下）：骑兵攻击+10 且 +10%（精锐骑兵模式，固定值+百分比双重加成）
- 31009 诸葛连弩（Lv5 诸葛连弩旗下）：弓兵获得连击战法（精锐弩兵模式）

#### 融入点 3：跨类联动（胡服骑射）
Institution 类 L3a「胡服骑射」（30203）旗下技术 31204「骑射改革」SHALL 体现跨类联动：
- TechId=30203（Institution 类），但 Category="Arms"、EffectType="ArmsSkillEnhance"
- 描述含「胡服骑射，跨类联动」，模拟三11改革制度推动军事效果

#### 融入点 4：能力关联（设计参考，不在配置表中落地）
设计上参考三11「每大类关联一个英雄能力」的思路，但**不在 TechConfig 中新增字段**（避免与 v2 的 11 字段框架冲突）。能力关联在后续研究执行逻辑 Spec 中通过 `SysFormula` 实现：
- Battle → 统率
- Development → 政治
- Institution → 政治
- Engineering → 智力

#### Scenario: 三11战术规则融入
- **WHEN** 查询 `TechSkillConfig.GetConfig(31006).Des`
- **THEN** 描述含「奇袭战法」与「森林地形免反击」字样，体现三11战术规则

#### Scenario: 精锐兵种 Lv5 模式
- **WHEN** 查询 Lv5 科技（30005 重骑兵、30006 诸葛连弩）旗下技术
- **THEN** 技术描述含「精锐X模式」字样，且效果数值采用固定值+百分比双重加成

#### Scenario: 胡服骑射跨类联动
- **WHEN** 查询 `TechSkillConfig.GetConfig(31204)`
- **THEN** TechId=30203（Institution 类），但 Category="Arms"、EffectType="ArmsSkillEnhance"，描述含「跨类联动」

### Requirement: 框架稳定性
系统 SHALL 保持 v2 框架结构不变，**禁止**因三11融入而修改以下内容：
- TechConfig 字段数仍为 11 个（不新增「相关能力」字段）
- TechSkillConfig 字段数仍为 13 个（不新增「战术规则」字段）
- 科技分类仍为 4 种（Battle/Development/Institution/Engineering），不改为三11的 9 种
- 科技等级仍为 5 级（L1/L2/L3a/L3b/L4/L5），不改为三11的 4 级
- 科技总数仍为 26 个（Battle/Development/Institution 各 6 + Engineering 8）
- 技术总数仍为 34 个

#### Scenario: 字段数不变
- **WHEN** 检查 `TechConfig.FieldMeta` 与 `TechSkillConfig.FieldMeta` 的 Count
- **THEN** 分别为 11 和 13，与 v2 一致
