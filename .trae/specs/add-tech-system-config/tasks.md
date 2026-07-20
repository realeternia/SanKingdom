# Tasks

- [x] Task 1: 新增 `Configs/TechConfig_s.cs` 科技配置表（初版）
- [x] Task 2: 新增 `Configs/TechSkillConfig_s.cs` 技术配置表（初版）
- [x] Task 3: 修改 `Configs/ConfigManager.cs` 集成加载
- [x] Task 4: 在 `Assembly-CSharp.csproj` 中添加新文件引用

## 修订任务（v2：5 级 + 2 路分支 + 技术分类 + 兵种效果）

- [x] Task 5: 修订 `TechSkillConfig_s.cs` 字段定义
  - [x] SubTask 5.1: 在 12 字段基础上新增 `string Category`（技术分类），变为 13 字段
  - [x] SubTask 5.2: 调整构造函数、FieldMetaInfo 元数据增加 Category 项
  - [x] SubTask 5.3: 确认字段顺序：Id/Cname/Des/TechId/Category/EffectType/EffectTarget/EffectAttr/EffectValue/EffectOp/EffectId/ResearchValue/Icon

- [x] Task 6: 重写 `TechConfig_s.cs` 样例数据为 5 级 + 2 路分支
  - [x] SubTask 6.1: 每类 6 个科技（L1/L2/L3a/L3b/L4/L5），共 30 个科技
  - [x] SubTask 6.2: 严格遵循 ID 分配规则（Battle 30001-、Development 30101-、Institution 30201-、Economy 30301-、Engineering 30401-）
  - [x] SubTask 6.3: L1 无前置；L2 依赖 L1；L3a/L3b 均依赖 L2 且互不依赖；L4 依赖 L3a+L3b；L5 依赖 L4
  - [x] SubTask 6.4: 至少 1 处跨类前置依赖（如 Institution L3a 依赖 Battle L2，模拟胡服骑射需改革推动）
  - [x] SubTask 6.5: 每个科技 SkillIds 数量 ∈ [1, 3]，与 Task 7 的 TechId 双向对应

- [x] Task 7: 重写 `TechSkillConfig_s.cs` 样例数据
  - [x] SubTask 7.1: 约 38 条样例，TechId 与 Task 6 的 SkillIds 双向对应
  - [x] SubTask 7.2: 覆盖全部 5 种技术 Category（Arms/Yield/Defense/Policy/Unlock）
  - [x] SubTask 7.3: 覆盖全部 6 种 EffectType（ArmsAttr/ArmsSkillEnhance/CityAttr/ForceBuff/UnlockArms/UnlockBuilding）
  - [x] SubTask 7.4: 移除所有 HeroAttr / BattleSkill / ForceAttr 效果（旧版数据需清理）
  - [x] SubTask 7.5: 至少 1 处跨类联动（Institution 类科技旗下含 Category="Arms" 的技术，EffectType=ArmsSkillEnhance，模拟胡服骑射）
  - [x] SubTask 7.6: ArmsAttr 效果的 EffectTarget 使用 ArmsType 名（SodWalk/SodHorse/SodBow 等），EffectAttr 使用 Atk/Def/MoveSpeed/Range 等

## 修订任务（v2.1：三11设计融入，框架不变）

- [x] Task 8: 在 v2 框架基础上融入三11设计点（不改变字段数与结构）
  - [x] SubTask 8.1: 战术规则融入 — TechSkillConfig 中 ArmsSkillEnhance 类技术描述体现三11战术命名
    - 31006 百炼刀：描述含「奇袭战法（森林地形免反击）」
    - 31007 明光铠：描述含「矢盾战法（30% 弓矢免伤）」
    - 31405 床弩：描述含「应射战法（攻击免反击）」
  - [x] SubTask 8.2: 精锐兵种 Lv5 模式 — Lv5 旗下技术采用「精锐X」命名与全面提升效果
    - 31008 铁骑：骑兵攻击+10 且 +10%（精锐骑兵模式，固定值+百分比双重加成）
    - 31009 诸葛连弩：弓兵获得连击战法（精锐弩兵模式）
  - [x] SubTask 8.3: 跨类联动（胡服骑射）— 31204 骑射改革描述含「胡服骑射，跨类联动」
  - [x] SubTask 8.4: 能力关联设计参考（不在配置表落地，后续在 SysFormula 实现）

- [x] Task 9: 更新 spec.md 新增「三11设计融入点（v2.1）」与「框架稳定性」章节

- [x] Task 10: 同步 tasks.md 与 checklist.md 补充 v2.1 任务和检查点

## 修订任务（v2.2：删除 Economy 系）

- [x] Task 11: 删除 Economy 系（科技 30301-30306 + 技术 31301-31307）
  - [x] SubTask 11.1: 从 `TechConfig_s.cs` 删除 Economy 类 6 个科技（30301-30306）
  - [x] SubTask 11.2: 从 `TechSkillConfig_s.cs` 删除 Economy 系 7 个技术（31301-31307）
  - [x] SubTask 11.3: 确认无悬空引用（其他系不依赖 Economy 科技）
  - [x] SubTask 11.4: 确认 5 种技术 Category 仍覆盖（Yield 由 Development/Institution 提供）
  - [x] SubTask 11.5: 确认 6 种 EffectType 仍覆盖（UnlockBuilding 由 Development/Engineering 提供）
  - [x] SubTask 11.6: 同步 spec.md（4 大类、24 科技、31 技术）
  - [x] SubTask 11.7: 同步 checklist.md（新增 v2.2 检查点）

## 修订任务（v2.3：Engineering 新增修路+地动仪）

- [x] Task 12: Engineering 类新增修路和地动仪，调整现有科技等级
  - [x] SubTask 12.1: 新增 30400 修路（L1，无前置），旗下技术 31400 驿道+31408 官道
  - [x] SubTask 12.2: 30401 筑城由 L1→L2，PreTechIds 由 []→[30400]
  - [x] SubTask 12.3: 新增 30407 地动仪（L2，与筑城并行），旗下技术 31409 地动仪（防灾）
  - [x] SubTask 12.4: 30402 营造 L2→L3a，30404 弩机 L3b PreTechIds [30402]→[30401]
  - [x] SubTask 12.5: 30403 砖石 L3a→L4，30405 烽火 L4→L5，30406 运河 L5→L6
  - [x] SubTask 12.6: 同步 spec.md（26 科技、34 技术、Engineering 8 科技）
  - [x] SubTask 12.7: 同步 checklist.md（新增 v2.3 检查点）

# Task Dependencies
- [Task 5] 与 [Task 6] 可并行（不同文件）
- [Task 7] depends on [Task 5]（需要新字段 Category）和 [Task 6]（需要 SkillIds 对应）
- 建议 [Task 5] + [Task 6] 先行，[Task 7] 跟随
- [Task 8] depends on [Task 7]（在已恢复的 v2 样例数据基础上融入三11设计点）
- [Task 9] 与 [Task 10] 在 [Task 8] 完成后执行
- [Task 11] 在 [Task 10] 完成后执行（在 v2.1 基础上裁剪 Economy 系）
