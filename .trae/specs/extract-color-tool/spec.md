# 提取颜色工具类 Spec

## Why
项目中颜色相关逻辑分散在 30+ 个文件中，存在大量重复的颜色常量定义（如 normalColor/selectedColor/disabledColor 在多个文件中重复定义）、重复的势力颜色解析模式（`ColorUtility.TryParseHtmlString(forceCfg.Color, out color) ? color : Color.white` 出现 6+ 次）、以及硬编码的属性值颜色逻辑（违反"禁止硬编码数值"规则）。需要将这些逻辑统一提取到 `SystemTool/SysColor.cs`，实现颜色逻辑的集中管理和复用。

## What Changes
- 新建 `SystemTool/SysColor.cs` 静态工具类，集中管理所有颜色常量和颜色查询逻辑
- 将 `SystemConst.Arms` 中的等级颜色映射和 `GetColorByLevel` 迁移到 `SysColor`
- 将 `HeroAttrTool` 中的颜色相关方法（`GetColorByValue`、`GetColoredText`、`GetColoredTextWithRule`、`ParseColorRule`、`TryMatchThreshold`）迁移到 `SysColor`
- 提取势力颜色解析为 `SysColor.GetForceColor(forceId)` 方法，消除 6+ 处重复代码
- 提取 UI 状态颜色常量（normalColor/selectedColor/disabledColor/hoverColor）为 `SysColor.UI` 嵌套类
- 提取战斗层颜色常量（伤害飘字红、粮草绿、死亡灰、血量条颜色等）为 `SysColor.Battle` 嵌套类
- 提取棋子材质颜色常量（金色/银色）为 `SysColor.Chess` 嵌套类
- 提取亮度自适应文字颜色逻辑为 `SysColor.GetTextColorOnBackground(Color bgColor)` 方法
- 修复 `BattleHeroInfo.cs` 和 `PopHeroBattleSelectPanelCell.cs` 中的硬编码属性值颜色，改用 `SysColor.GetColorByValue`
- 更新所有引用点，改为调用 `SysColor` 中的方法
- 删除 `SystemConst.Arms` 嵌套类和 `HeroAttrTool` 中已迁移的颜色方法
- 在 `Assembly-CSharp.csproj` 中添加新文件引用

## Impact
- Affected specs: 无
- Affected code:
  - `SystemTool/SystemConst.cs` — 删除 Arms 嵌套类
  - `SystemTool/HeroAttrTool.cs` — 删除颜色相关方法
  - `SystemTool/SysColor.cs` — 新建
  - `Combat/ChessViewObj.cs` — 引用 SysColor.Chess
  - `BattleHeroInfo.cs` — 引用 SysColor.Battle + SysColor.GetColorByValue
  - `Combat/BattleTopInfo.cs` — 引用 SysColor.GetForceColor
  - `Combat/Actions/SkillDamageAction.cs` — 引用 SysColor.Battle
  - `Combat/Skills/SkillHitFood.cs` — 引用 SysColor.Battle
  - `WorldPieceControl.cs` — 引用 SysColor.GetForceColor + SysColor.GetTextColorOnBackground
  - `CityDetail.cs` — 引用 SysColor.UI
  - `SaveForceData.cs` — 引用 SysColor.GetForceColor
  - `Panels/HeroInfoPanelManager.cs` — 引用 SysColor
  - `Panels/ListItem/PopHeroBattleSelectPanelCell.cs` — 引用 SysColor
  - `Panels/ListItem/CityBattleItem.cs` — 引用 SysColor.UI
  - `Panels/CityDevPanelCell.cs` — 引用 SysColor.UI
  - `Panels/ListItem/CityDevItem.cs` — 引用 SysColor
  - `Panels/Gismo/ArmsItemControl.cs` — 引用 SysColor
  - `Panels/ListItem/SideArmsItem.cs` — 引用 SysColor
  - `Panels/ListItem/PopHeroSelectPanelCell.cs` — 引用 SysColor.UI
  - `Panels/ListItem/PopCitySelectPanelCell.cs` — 引用 SysColor.UI
  - `Panels/ListItem/CityCellHero.cs` — 引用 SysColor
  - `Panels/ListItem/CityCellCity.cs` — 引用 SysColor.UI
  - `Panels/ListItem/HeroInfoCell.cs` — 引用 SysColor.UI
  - `Panels/ListItem/RankCellMode.cs` — 引用 SysColor.UI
  - `Panels/ListItem/RankCellForce.cs` — 引用 SysColor.UI
  - `Panels/ListItem/RankCellInfo.cs` — 引用 SysColor.GetForceColor + SysColor.GetColoredText
  - `Panels/ListItem/ReplayCellControl.cs` — 引用 SysColor.GetForceColor + SysColor.Battle
  - `Panels/ListItem/BattleResultHeroCellControl.cs` — 引用 SysColor.Battle
  - `Panels/PickPanelControl.cs` — 引用 SysColor.GetForceColor
  - `Panels/CityDevNodeMove.cs` — 引用 SysColor.Battle
  - `Panels/ListItem/TroopsHeroSlot.cs` — 引用 SysColor.UI
  - `UIScripts/NLDropDownItem.cs` — 引用 SysColor.UI

## ADDED Requirements

### Requirement: SysColor 颜色工具类
系统 SHALL 提供一个 `SysColor` 静态工具类（位于 `SystemTool/SysColor.cs`），集中管理所有颜色常量和颜色查询逻辑。

#### Scenario: 兵种等级颜色查询
- **WHEN** 调用 `SysColor.GetArmsLevelColor(level)`
- **THEN** 返回对应等级的颜色，等级越界时返回最高等级颜色

#### Scenario: 势力颜色查询
- **WHEN** 调用 `SysColor.GetForceColor(forceId)`
- **THEN** 从 ForceConfig 解析该势力的十六进制颜色，解析失败时返回 Color.white

#### Scenario: 属性值颜色查询
- **WHEN** 调用 `SysColor.GetColorByValue(attrName, value)`
- **THEN** 根据 HeroAttrConfig 的 ColorRule 解析并返回对应颜色，无规则时返回 Color.white

#### Scenario: 带颜色的富文本
- **WHEN** 调用 `SysColor.GetColoredText(attrName, value)` 或 `SysColor.GetColoredTextWithRule(attrName, value)`
- **THEN** 返回包含 `<color=#XXXXXX>` 标签的富文本字符串

#### Scenario: 亮度自适应文字颜色
- **WHEN** 调用 `SysColor.GetTextColorOnBackground(bgColor)`
- **THEN** 背景亮度 > 0.65 时返回深色文字，否则返回白色文字

### Requirement: SysColor.UI 嵌套类 — UI 状态颜色常量
系统 SHALL 在 `SysColor.UI` 嵌套类中提供 UI 状态颜色常量。

#### Scenario: 通用 UI 状态颜色
- **WHEN** 引用 `SysColor.UI.NormalColor` / `SelectedColor` / `DisabledColor`
- **THEN** 返回项目统一的 UI 状态颜色（深灰/黄绿/暗灰半透明）

#### Scenario: 下拉框 UI 状态颜色
- **WHEN** 引用 `SysColor.UI.DropDownNormal` / `DropDownSelected` / `DropDownHover`
- **THEN** 返回下拉框专用状态颜色

#### Scenario: 列表项选中颜色
- **WHEN** 引用 `SysColor.UI.ListItemSelected` / `ListItemNormal`
- **THEN** 返回列表项选中/正常颜色（绿色/黑色）

#### Scenario: 描边颜色
- **WHEN** 引用 `SysColor.UI.BorderColor` / `BorderSelectedColor`
- **THEN** 返回描边正常/选中颜色

### Requirement: SysColor.Battle 嵌套类 — 战斗层颜色常量
系统 SHALL 在 `SysColor.Battle` 嵌套类中提供战斗层颜色常量。

#### Scenario: 伤害飘字颜色
- **WHEN** 引用 `SysColor.Battle.DamageColor`
- **THEN** 返回红色 (1,0,0)

#### Scenario: 粮草颜色
- **WHEN** 引用 `SysColor.Battle.FoodLossColor` / `FoodGainColor`
- **THEN** 返回粮草损失红色和粮草获取绿色

#### Scenario: 死亡/灰色
- **WHEN** 引用 `SysColor.Battle.DeadColor`
- **THEN** 返回灰色 (0.3, 0.3, 0.3, 1)

#### Scenario: 血量条颜色
- **WHEN** 引用 `SysColor.Battle.HealthLowColor` / `HealthNormalColor`
- **THEN** 返回低血量棕色和正常血量绿色

#### Scenario: 攻城结果颜色
- **WHEN** 引用 `SysColor.Battle.AttackFailColor` / `AttackSuccessColor`
- **THEN** 返回攻城失败绿色和攻城成功红色

### Requirement: SysColor.Chess 嵌套类 — 棋子材质颜色常量
系统 SHALL 在 `SysColor.Chess` 嵌套类中提供棋子材质颜色常量。

#### Scenario: 金色材质颜色
- **WHEN** 引用 `SysColor.Chess.GoldMain` / `GoldEmission` / `GoldOutline` / `GoldSpec`
- **THEN** 返回金色材质的四种颜色

#### Scenario: 银色材质颜色
- **WHEN** 引用 `SysColor.Chess.SilverMain` / `SilverEmission` / `SilverOutline` / `SilverSpec`
- **THEN** 返回银色材质的四种颜色

### Requirement: SysColor.City 嵌套类 — 城市相关颜色常量
系统 SHALL 在 `SysColor.City` 嵌套类中提供城市相关颜色常量。

#### Scenario: 英雄覆盖层颜色
- **WHEN** 引用 `SysColor.City.HeroOverlayColor`
- **THEN** 返回半透明黑色 (0, 0, 0, 0.92)

#### Scenario: 英雄描边颜色
- **WHEN** 引用 `SysColor.City.WildHeroBorderColor` / `CapturedHeroBorderColor`
- **THEN** 返回野外英雄黄色描边和俘虏英雄红色描边

## MODIFIED Requirements

### Requirement: 兵种等级颜色查询
原 `SystemConst.Arms.GetColorByLevel(level)` 迁移到 `SysColor.GetArmsLevelColor(level)`，`SystemConst.Arms` 嵌套类删除。

### Requirement: 属性值颜色查询
原 `HeroAttrTool.GetColorByValue` / `GetColoredText` / `GetColoredTextWithRule` / `ParseColorRule` / `TryMatchThreshold` 迁移到 `SysColor`，`HeroAttrTool` 中删除这些方法。`HeroAttrTool.GetTextByValue` 保留在原处。

### Requirement: 势力颜色解析
原分散在 6+ 处的 `ColorUtility.TryParseHtmlString(ForceConfig.GetConfig(id).Color, out color) ? color : Color.white` 模式统一替换为 `SysColor.GetForceColor(forceId)`。

### Requirement: 属性值颜色显示
`BattleHeroInfo.cs` 和 `PopHeroBattleSelectPanelCell.cs` 中的硬编码属性值颜色逻辑（`>=95` 红色等）替换为 `SysColor.GetColorByValue` 调用，消除与 HeroAttrConfig.ColorRule 的不一致。

## REMOVED Requirements

### Requirement: SystemConst.Arms 嵌套类
**Reason**: 颜色逻辑统一迁移到 SysColor
**Migration**: `SystemConst.Arms.GetColorByLevel(level)` → `SysColor.GetArmsLevelColor(level)`

### Requirement: HeroAttrTool 颜色方法
**Reason**: 颜色逻辑统一迁移到 SysColor
**Migration**: `HeroAttrTool.GetColorByValue` → `SysColor.GetColorByValue`，`HeroAttrTool.GetColoredText` → `SysColor.GetColoredText`，`HeroAttrTool.GetColoredTextWithRule` → `SysColor.GetColoredTextWithRule`
