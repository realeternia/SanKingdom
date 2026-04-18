# CityPanel委派数据持久化 Spec

## Why
当前CityPanel的委派数据（hero到devNode的映射）仅存在于内存中，游戏关闭或场景切换后会丢失。需要在SaveCityData中持久化存储委派数据，并在特定事件（hero移动、被俘虏、城市被攻占等）时清空委派队列。

## What Changes
- 在SaveCityData中添加委派数据存储字段（heroId到devId的映射）
- CityPanel打开时从SaveCityData读取委派数据初始化面板
- 委派数据变更时保存到SaveCityData并触发存档
- hero移动、被俘虏、俘虏逃跑、城市被攻占时清空对应城市的委派队列

## Impact
- Affected specs: SaveCityData, CityPanelManager, GameManager
- Affected code: 
  - `SaveCityData.cs` - 添加委派数据字段和相关方法
  - `CityPanelManager.cs` - 初始化时读取委派数据，变更时保存
  - `GameManager.cs` - 在ProcessHeros中处理俘虏逃跑时清空委派记录

## ADDED Requirements

### Requirement: 委派数据存储
系统应当在SaveCityData中存储每个城市的委派映射关系（heroId -> devId）。

#### Scenario: 存储委派数据
- **WHEN** 玩家将hero委派到某个devNode
- **THEN** 系统将heroId和devId的映射关系保存到对应城市的SaveCityData中
- **AND** 触发游戏存档

### Requirement: 委派数据读取
系统应当在CityPanel打开时从SaveCityData读取委派数据并初始化面板。

#### Scenario: 初始化面板
- **WHEN** CityPanel打开
- **THEN** 系统从当前城市的SaveCityData读取委派映射
- **AND** 根据映射关系初始化各个CityDevNodeNew的hero显示

### Requirement: 委派队列清空
系统应当在特定事件发生时清空城市的委派队列。

#### Scenario: Hero移动
- **WHEN** hero从一个城市移动到另一个城市（通过`Player.MoveHeroToCity()`或`SaveCityData.MoveHeroTo()`）
- **THEN** 系统从原城市的委派队列中移除该hero的委派记录

#### Scenario: Hero被俘虏
- **WHEN** hero状态变为Catched（被俘虏），发生在`SaveCityData.Occupy()`中
- **THEN** 系统从该hero所在城市的委派队列中移除该hero的委派记录

#### Scenario: 俘虏逃跑
- **WHEN** 被俘虏的hero成功逃跑（在`GameManager.ProcessHeros()`中，hero.state从Catched变为Normal，且cityId改变）
- **THEN** 系统从原城市的委派队列中移除该hero的委派记录

#### Scenario: 城市被攻占
- **WHEN** 城市被攻占（`SaveCityData.Occupy()`方法执行）
- **THEN** 系统清空该城市的所有委派记录
