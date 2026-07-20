- [x] `TechConfig` 类位于 `CommonConfig` 命名空间，文件路径 `Assets/Resources/Scripts/Configs/TechConfig_s.cs`
- [x] `TechConfig` 包含 11 个公共字段：Id/Cname/Des/Category/Level/PreTechIds/ResearchValue/SkillIds/Icon/IsSpecial/AiWeight
- [x] `TechConfig` 遵循 `CityDevConfig` 模板：FieldMetaInfo/CellMeta/fieldMeta/cellMeta/Load/GetConfig/HasConfig/Refresh/Add/Remove/Assign/RebuildIndex/ConfigList
- [x] `TechConfig.GetConfig` 找不到时抛 `NullReferenceException`，消息含 "配置表TechConfig不存在id=xxx"
- [x] `TechConfig` 的 `PreTechIds`/`SkillIds` 无元素时为 `new int[0]`，不为 null
- [x] `TechConfig` 样例数据覆盖全部 4 种 Category：Battle/Development/Institution/Engineering
- [x] `TechConfig` 样例数据 Battle/Development/Institution 每类 6 个科技，Engineering 8 个科技，共 26 个
- [x] `TechConfig` 样例数据覆盖 Level 1-5
- [x] `TechConfig` 样例中 Level=1 的科技 `PreTechIds` 为空数组
- [x] `TechConfig` 样例中 L3a 与 L3b 均只依赖 L2，互不依赖（可并行研究）
- [x] `TechConfig` 样例中 L4 的 `PreTechIds` 同时含 L3a 和 L3b
- [x] `TechConfig` 样例中 L5 的 `PreTechIds` 含 L4
- [x] `TechConfig` 样例中至少 1 处跨类前置依赖（30203 胡服骑射 PreTechIds 含 30002 Battle L2）
- [x] `TechConfig` 样例中每个科技 `SkillIds` 数量 ∈ [1, 3]
- [x] `TechConfig` ID 分配遵循规则：Battle 30001-30010、Development 30101-30110、Institution 30201-30210、Engineering 30400-30410
- [x] `TechSkillConfig` 类位于 `CommonConfig` 命名空间，文件路径 `Assets/Resources/Scripts/Configs/TechSkillConfig_s.cs`
- [x] `TechSkillConfig` 包含 13 个公共字段：Id/Cname/Des/TechId/Category/EffectType/EffectTarget/EffectAttr/EffectValue/EffectOp/EffectId/ResearchValue/Icon
- [x] `TechSkillConfig` 遵循 `CityDevConfig` 模板：FieldMetaInfo/CellMeta/Load/GetConfig/HasConfig/Refresh/Add/Remove/Assign/RebuildIndex/ConfigList
- [x] `TechSkillConfig.GetConfig` 找不到时抛 `NullReferenceException`
- [x] `TechSkillConfig` 样例数据覆盖全部 5 种技术 Category：Arms/Yield/Defense/Policy/Unlock
- [x] `TechSkillConfig` 样例数据覆盖全部 6 种 EffectType：ArmsAttr/ArmsSkillEnhance/CityAttr/ForceBuff/UnlockArms/UnlockBuilding
- [x] `TechSkillConfig` 样例中不存在 HeroAttr / BattleSkill / ForceAttr 效果（已清理）
- [x] `TechSkillConfig` 样例的 `TechId` 与 `TechConfig` 的 `SkillIds` 双向对应（父子关系一致）
- [x] `TechSkillConfig` 样例中至少 1 处跨类联动（31204 骑射改革 TechId=30203 Institution，Category=Arms，EffectType=ArmsSkillEnhance）
- [x] `TechSkillConfig` 中 ArmsAttr 效果的 EffectTarget 使用 ArmsType 名（SodWalk/SodHorse/SodBow 等）
- [x] `TechSkillConfig` ID 按父科技 ID 段对齐（Battle 系 31001-、Development 系 31101-、Institution 系 31201-、Engineering 系 31400-）
- [x] `ConfigManager.Init()` 在 `FairConfig.Load()` 之后调用 `TechConfig.Load()` 和 `TechSkillConfig.Load()`
- [x] `Assembly-CSharp.csproj` 中添加 `TechConfig_s.cs` 的 `<Compile Include>`
- [x] `Assembly-CSharp.csproj` 中添加 `TechSkillConfig_s.cs` 的 `<Compile Include>`

## v2.1 检查点：三11设计融入（框架不变）

- [x] `TechConfig` 字段数仍为 11（未新增「相关能力」字段）
- [x] `TechSkillConfig` 字段数仍为 13（未新增「战术规则」字段）
- [x] 科技分类仍为 4 种（Battle/Development/Institution/Engineering），未改为三11的 9 种
- [x] 科技等级仍为 5 级（L1/L2/L3a/L3b/L4/L5），未改为三11的 4 级
- [x] 科技总数仍为 26 个（Battle/Development/Institution 各 6 + Engineering 8）
- [x] 技术总数仍为 34 个
- [x] 战术规则融入：31006 百炼刀描述含「奇袭战法（森林地形免反击）」
- [x] 战术规则融入：31007 明光铠描述含「矢盾战法（30% 弓矢免伤）」
- [x] 战术规则融入：31405 床弩描述含「应射战法（攻击免反击）」
- [x] 精锐兵种 Lv5 模式：31008 铁骑描述含「精锐骑兵模式」，效果为固定值+10 且百分比+10%
- [x] 精锐兵种 Lv5 模式：31009 诸葛连弩描述含「精锐弩兵模式」
- [x] 跨类联动：31204 骑射改革 TechId=30203（Institution）、Category="Arms"、描述含「胡服骑射，跨类联动」
- [x] 能力关联设计参考：Battle→统率/Development→政治/Institution→政治/Engineering→智力（不在配置表落地）
- [x] spec.md 已新增「三11设计融入点（v2.1）」与「框架稳定性」章节
- [x] tasks.md 已新增 Task 8/9/10（v2.1 融入任务）

## v2.2 检查点：删除 Economy 系

- [x] `TechConfig` 中 Economy 系（30301-30306）6 个科技已删除
- [x] `TechSkillConfig` 中 Economy 系（31301-31307）7 个技术已删除
- [x] 科技分类由 5 种减为 4 种（Battle/Development/Institution/Engineering）
- [x] 科技总数由 30 减为 24（4 大类 × 6 科技）
- [x] 技术总数由 38 减为 31
- [x] Economy 删除后 5 种技术 Category 仍覆盖（Yield 由 Development/Institution 提供）
- [x] Economy 删除后 6 种 EffectType 仍覆盖（UnlockBuilding 由 Development/Engineering 提供）
- [x] Economy 删除后跨类联动（31204 胡服骑射）仍存在
- [x] Economy 删除后无悬空引用（无其他系依赖 Economy 科技）
- [x] spec.md 已同步更新（4 大类、26 科技、34 技术）
- [x] tasks.md 已新增 Task 11（删除 Economy 系）

## v2.3 检查点：Engineering 新增修路+地动仪

- [x] 新增 30400 修路（L1，无前置，SkillIds=[31400,31408]）
- [x] 30401 筑城由 L1 调整为 L2（PreTechIds 由 []→[30400]）
- [x] 新增 30407 地动仪（L2，PreTechIds=[30400]，与筑城并行，防灾）
- [x] 30402 营造由 L2 调整为 L3a（PreTechIds 仍为 [30401]）
- [x] 30404 弩机由 L3b 调整为 L3b（PreTechIds 由 [30402]→[30401]，依赖筑城）
- [x] 30403 砖石由 L3a 调整为 L4（PreTechIds 仍为 [30402]）
- [x] 30405 烽火由 L4 调整为 L5（PreTechIds 仍为 [30403,30404]）
- [x] 30406 运河由 L5 调整为 L6（PreTechIds 仍为 [30405]，ResearchValue 1000→1200）
- [x] 新增 31400 驿道（TechId=30400，英雄移动效率+15%，heroMove）
- [x] 新增 31408 官道（TechId=30400，部队移动效率+15%，armyMove）
- [x] 新增 31409 地动仪（TechId=30407，灾害损失-30%，disaster）
- [x] Engineering 科技树结构：L1 修路 → L2a 筑城/L2b 地动仪 → L3a 营造/L3b 弩机 → L4 砖石 → L5 烽火 → L6 运河
- [x] 科技总数由 24 增为 26，技术总数由 31 增为 34
