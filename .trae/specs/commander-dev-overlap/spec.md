# 主将与委派允许同一英雄兼任 Spec

## Why
当前系统不允许主将同时担任委派（dev）工作，玩家将英雄设为主将时会自动移除其委派，AI组建军团时也排除已委派的英雄。需要解除这一互斥限制，允许同一英雄同时担任主将和委派工作，提升游戏策略灵活性。

## What Changes
- 移除玩家侧主将与委派的互斥校验（`IsCommander` 检查和 `RemoveHeroFromDev` 调用）
- 移除AI侧组建军团时对已委派英雄的排除逻辑
- 修改英雄工作状态UI显示，支持同时显示主将图标和委派图标

## Impact
- Affected specs: 英雄工作状态显示、AI军团组建逻辑
- Affected code:
  - [CityTroopsItem.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Panels/ListItem/CityTroopsItem.cs) — 移除 `RemoveHeroFromDev` 调用
  - [CityPanelManager.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Panels/CityPanelManager.cs) — 移除 `IsCommander` 校验
  - [AI.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Controls/AI/AI.cs) — 移除 `FormCityTroops` 中对dev英雄的排除
  - [CityCellHero.cs](file:///d:/U3dPrj/SanKingdom/Assets/Resources/Scripts/Panels/ListItem/CityCellHero.cs) — 修改工作状态图标显示逻辑

## ADDED Requirements

### Requirement: 主将与委派兼任
系统应当允许同一英雄同时担任军团主将和委派工作。

#### Scenario: 玩家将英雄设为主将时保留委派
- **WHEN** 玩家将英雄拖入军团主将槽位
- **THEN** 系统不再自动移除该英雄的委派指派
- **AND** 英雄同时保持主将身份和委派工作状态

#### Scenario: 玩家将主将委派到开发位置
- **WHEN** 玩家将已担任主将的英雄拖到委派节点
- **THEN** 系统允许该操作，不再提示"主将不能派遣到开发位置"
- **AND** 英雄同时保持主将身份和委派工作状态

#### Scenario: AI组建军团时不再排除已委派英雄
- **WHEN** AI执行 `FormCityTroops` 组建军团
- **THEN** 已委派到dev的英雄仍可作为主将候选人
- **AND** AI无需考虑英雄的委派状态来决定是否选为主将

### Requirement: 工作状态图标双显
系统应当在英雄同时担任主将和委派工作时，同时显示两种工作状态图标。

#### Scenario: 英雄同时为主将和委派
- **WHEN** 英雄同时担任军团主将和委派工作
- **THEN** job1图标显示主将图标（citytroop1）
- **AND** job2图标显示委派工作对应属性图标

#### Scenario: 英雄仅为主将
- **WHEN** 英雄仅担任军团主将，无委派工作
- **THEN** job1图标显示主将图标（citytroop1）
- **AND** job2图标隐藏

#### Scenario: 英雄仅为委派
- **WHEN** 英雄仅担任委派工作，非主将
- **THEN** job1图标显示委派工作对应属性图标
- **AND** job2图标隐藏

## MODIFIED Requirements

### Requirement: 英雄工作状态显示
原 `CityCellHero.UpdateWorkState` 中主将与委派互斥显示逻辑改为可同时显示。job1优先显示主将图标（如有），其次显示委派图标；job2在英雄同时有委派工作时显示委派图标。

## REMOVED Requirements

### Requirement: 主将与委派互斥
**Reason**: 游戏设计变更，允许同一英雄同时担任主将和委派工作
**Migration**: 移除所有互斥校验和自动移除逻辑
