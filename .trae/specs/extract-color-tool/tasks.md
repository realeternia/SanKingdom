# Tasks

- [x] Task 1: 创建 `SystemTool/SysColor.cs` 静态工具类
  - [x] SubTask 1.1: 创建 SysColor 静态类骨架，包含核心方法：`GetArmsLevelColor`、`GetForceColor`、`GetColorByValue`、`GetColoredText`、`GetColoredTextWithRule`、`GetTextColorOnBackground`，以及 `ParseColorRule`/`TryMatchThreshold` 私有方法
  - [x] SubTask 1.2: 添加 `SysColor.UI` 嵌套类（NormalColor、SelectedColor、DisabledColor、DropDownNormal、DropDownSelected、DropDownHover、ListItemSelected、ListItemNormal、BorderColor、BorderSelectedColor）
  - [x] SubTask 1.3: 添加 `SysColor.Battle` 嵌套类（DamageColor、FoodLossColor、FoodGainColor、DeadColor、HealthLowColor、HealthNormalColor、AttackFailColor、AttackSuccessColor）
  - [x] SubTask 1.4: 添加 `SysColor.Chess` 嵌套类（GoldMain、GoldEmission、GoldOutline、GoldSpec、SilverMain、SilverEmission、SilverOutline、SilverSpec）
  - [x] SubTask 1.5: 添加 `SysColor.City` 嵌套类（HeroOverlayColor、WildHeroBorderColor、CapturedHeroBorderColor）
  - [x] SubTask 1.6: 在 `Assembly-CSharp.csproj` 中添加 `<Compile Include="Assets\Resources\Scripts\SystemTool\SysColor.cs" />`

- [x] Task 2: 迁移 SystemConst.Arms 到 SysColor
  - [x] SubTask 2.1: 将 `SystemConst.Arms.LevelColors` 和 `GetColorByLevel` 逻辑复制到 `SysColor.GetArmsLevelColor`
  - [x] SubTask 2.2: 删除 `SystemConst.Arms` 嵌套类

- [x] Task 3: 迁移 HeroAttrTool 颜色方法到 SysColor
  - [x] SubTask 3.1: 将 `GetColorByValue`、`GetColoredText`、`GetColoredTextWithRule`、`ParseColorRule`、`TryMatchThreshold` 复制到 SysColor
  - [x] SubTask 3.2: 从 HeroAttrTool 中删除这些颜色方法

- [x] Task 4: 更新战略层引用
  - [x] SubTask 4.1: `WorldPieceControl.cs` — 势力颜色解析改用 `SysColor.GetForceColor`，亮度自适应改用 `SysColor.GetTextColorOnBackground`，兵力/金币颜色改用 `SysColor.Battle` 常量
  - [x] SubTask 4.2: `CityDetail.cs` — 覆盖层/描边颜色改用 `SysColor.City` 常量，灰色改用 `SysColor.Battle.DeadColor`
  - [x] SubTask 4.3: `SaveForceData.cs` — `LineColor` 属性改用 `SysColor.GetForceColor(forceId)`
  - [x] SubTask 4.4: `PickPanelControl.cs` — 势力颜色改用 `SysColor.GetForceColor`

- [x] Task 5: 更新战斗层引用
  - [x] SubTask 5.1: `ChessViewObj.cs` — 金色/银色材质颜色改用 `SysColor.Chess` 常量
  - [x] SubTask 5.2: `BattleHeroInfo.cs` — 硬编码属性值颜色改用 `SysColor.GetColorByValue`，血量条/死亡颜色改用 `SysColor.Battle` 常量
  - [x] SubTask 5.3: `BattleTopInfo.cs` — 势力颜色改用 `SysColor.GetForceColor`
  - [x] SubTask 5.4: `SkillDamageAction.cs` — 伤害飘字颜色改用 `SysColor.Battle.DamageColor`
  - [x] SubTask 5.5: `SkillHitFood.cs` — 粮草颜色改用 `SysColor.Battle.FoodLossColor` / `FoodGainColor`

- [x] Task 6: 更新面板/列表项引用
  - [x] SubTask 6.1: `HeroInfoPanelManager.cs` — `SystemConst.Arms.GetColorByLevel` 改用 `SysColor.GetArmsLevelColor`，灰色/白色改用 `SysColor.Battle`
  - [x] SubTask 6.2: `PopHeroBattleSelectPanelCell.cs` — 硬编码属性值颜色改用 `SysColor.GetColorByValue`，UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.3: `CityBattleItem.cs` — 颜色值与 SysColor.UI 不匹配，保留原样
  - [x] SubTask 6.4: `CityDevPanelCell.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.5: `CityDevItem.cs` — 描边颜色改用 `SysColor.UI`，英雄等级颜色映射改用 `SysColor`
  - [x] SubTask 6.6: `ArmsItemControl.cs` — `HeroAttrTool.GetColorByValue` 改用 `SysColor.GetColorByValue`，匹配颜色改用 `SysColor.UI`
  - [x] SubTask 6.7: `SideArmsItem.cs` — `SystemConst.Arms.GetColorByLevel` 改用 `SysColor.GetArmsLevelColor`，UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.8: `PopHeroSelectPanelCell.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.9: `PopCitySelectPanelCell.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.10: `CityCellHero.cs` — `HeroAttrTool.GetColorByValue` 改用 `SysColor.GetColorByValue`，UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.11: `CityCellCity.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.12: `HeroInfoCell.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.13: `RankCellMode.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.14: `RankCellForce.cs` — UI 状态颜色改用 `SysColor.UI`
  - [x] SubTask 6.15: `RankCellInfo.cs` — 势力颜色改用 `SysColor.GetForceColor`，`HeroAttrTool.GetColoredText` 改用 `SysColor.GetColoredText`
  - [x] SubTask 6.16: `ReplayCellControl.cs` — 势力颜色改用 `SysColor.GetForceColor`，结果颜色改用 `SysColor.Battle`
  - [x] SubTask 6.17: `BattleResultHeroCellControl.cs` — 死亡/描边颜色改用 `SysColor.Battle`
  - [x] SubTask 6.18: `CityDevNodeMove.cs` — 食物颜色改用 `SysColor.Battle`
  - [x] SubTask 6.19: `TroopsHeroSlot.cs` — 拖拽颜色改用 `SysColor.UI`
  - [x] SubTask 6.20: `NLDropDownItem.cs` — 下拉框颜色改用 `SysColor.UI`

# Task Dependencies
- [Task 2] depends on [Task 1]
- [Task 3] depends on [Task 1]
- [Task 4] depends on [Task 1, Task 2, Task 3]
- [Task 5] depends on [Task 1, Task 2, Task 3]
- [Task 6] depends on [Task 1, Task 2, Task 3]
- [Task 4, Task 5, Task 6] 可并行执行
