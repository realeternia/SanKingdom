# 替换 HeroConfig Likes/Hates 为 LikeForces/HateForces Spec

## Why
当前 Likes/Hates 字段以英雄名字（string）存储关系，粒度粗且无法表达程度差异。改为基于 forceId+程度的方式，可以更细粒度地表达英雄对各势力的好恶程度（1-5分），同时更利于 AI 决策和登庸公式计算。

## What Changes
- HeroConfig 字段 `Likes` (string[]) → `LikeForces` (string[])，每项格式 "forceId;degree"
- HeroConfig 字段 `Hates` (string[]) → `HateForces` (string[])，每项格式 "forceId;degree"
- **BREAKING**: 字段名和格式均变更，所有引用方需适配
- SysFormula.GetRelationBonusPercent 从按英雄名匹配改为按 forceId+程度计算
- HeroInfoPanelManager UI 展示改为显示势力名+程度
- SystemConst 中关系加成常量从固定值改为按程度分级的公式
- 删除 ContainsName 辅助方法，替换为 ContainsForce 辅助方法
- HeroConfig_s.cs Load 方法中的所有数据需从英雄名格式转换为 forceId;degree 格式

## Impact
- Affected code: HeroConfig_s.cs, SysFormula.cs, HeroInfoPanelManager.cs, SystemConst.cs
- Affected data: HeroConfig.Load() 中所有英雄的 Likes/Hates 数据需重写

## ADDED Requirements

### Requirement: ForceRelation 数据格式
系统 SHALL 支持 LikeForces/HateForces 字段的 "forceId;degree" 格式，其中 forceId 为 int，degree 为 1-5 整数，5 表示非常喜欢/非常厌恶。

#### Scenario: 解析 forceId;degree 格式
- **WHEN** LikeForces 或 HateForces 包含 "2;5"
- **THEN** 解析出 forceId=2，degree=5

#### Scenario: 多个势力关系
- **WHEN** LikeForces 包含 ["1;3","3;5"]
- **THEN** 英雄对势力1好感度为3，对势力3好感度为5

### Requirement: 按程度计算关系加成
系统 SHALL 根据好感/厌恶程度（1-5）分级计算登庸加成，而非固定值。

#### Scenario: 好感度加成
- **WHEN** 目标英雄 LikeForces 包含执行人所属势力的 forceId，degree=N
- **THEN** 加成 = N * LIKE_BONUS_PER_DEGREE

#### Scenario: 厌恶程度惩罚
- **WHEN** 目标英雄 HateForces 包含执行人所属势力的 forceId，degree=N
- **THEN** 惩罚 = N * HATE_PENALTY_PER_DEGREE

## MODIFIED Requirements

### Requirement: HeroConfig 关系字段
HeroConfig 的关系字段从 Likes(string[] 英雄名列表)/Hates(string[] 英雄名列表) 变更为 LikeForces(string[] forceId;degree 列表)/HateForces(string[] forceId;degree 列表)。

## REMOVED Requirements

### Requirement: ContainsName 辅助方法
**Reason**: 不再按英雄名匹配关系，改为按 forceId 匹配
**Migration**: 替换为 ContainsForce 方法，按 forceId 在 LikeForces/HateForces 中查找并返回 degree
