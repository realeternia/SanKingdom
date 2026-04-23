# 重构 topNode 资源显示 Spec

## Why
MainPanelManager 的 topNode 当前用于显示各势力信息（PlayerInfoCell），但需求变更为在 PanelManager.topNode 显示玩家势力的资源信息。需要清理旧逻辑，实现新的资源显示系统，并支持资源变化时自动刷新。

## What Changes
- **删除** MainPanelManager 中的 `topNode` 字段和 `InitForceControls()` 方法
- **删除** MainPanelManager.SendSignal 中 `CityForceChange` 对 `InitForceControls()` 的调用
- **删除** `PlayerInfoControl.cs` 文件
- **修复** `CityAttrConfig` 中 force 属性 ID 全部为 12 的 bug，为每个属性分配唯一 ID
- **扩展** `ResItem` 添加 `attrName` 字段，用于信号匹配
- **实现** PanelManager.topNode 初始化逻辑：遍历 CityAttrConfig 中所有 IsForceAttr=true 的配置，实例化 ResBase.prefab 并挂载 ResItem 组件
- **实现** 资源显示始终展示玩家（isPlayer=true）势力的对应属性值
- **实现** 资源变化时发送 `ForceResChange` 信号（parm1=属性名，parm2=当前值），PanelManager 监听并刷新 topNode 中的 ResItem 显示
- **更新** SaveForceData.AddAttr() 方法，在修改资源后发送 `ForceResChange` 信号

## Impact
- Affected specs: 势力管理、UI 显示系统、信号系统
- Affected code:
  - `MainPanelManager.cs` - 移除 topNode 和 InitForceControls 逻辑
  - `PanelManager.cs` - 新增 topNode 初始化和资源刷新逻辑
  - `PlayerInfoControl.cs` - 删除
  - `CityAttrConfig_s.cs` - 修复 force 属性 ID
  - `SaveForceData.cs` - AddAttr 中发送 ForceResChange 信号
  - `ResItem.cs` - 添加 attrName 字段

## ADDED Requirements

### Requirement: PanelManager topNode 资源初始化
PanelManager SHALL 在初始化时遍历 CityAttrConfig 中所有 IsForceAttr=true 的配置项，为每个配置实例化一个 ResBase.prefab 作为 topNode 的子对象，并添加 ResItem 组件。每个 ResItem 通过 `SetItem(attrName, value)` 设置图标和数值，数值取自玩家势力（isPlayer=true）的对应属性。

#### Scenario: 游戏启动时 topNode 初始化
- **WHEN** PanelManager 初始化（Start 或 ShowWorld 时）
- **THEN** topNode 下包含所有 force 属性对应的 ResBase 实例，每个显示玩家势力的资源图标和数值

#### Scenario: 没有 PlayerInfoControl 的引用
- **WHEN** 代码中搜索 PlayerInfoControl 引用
- **THEN** 不存在任何对 PlayerInfoControl 的引用

### Requirement: ResItem 存储属性名
ResItem SHALL 添加 `private string attrName` 字段，在 `SetItem(string name, int num)` 调用时记录 name 到 attrName，用于信号匹配时识别对应 ResItem。

### Requirement: 资源变化信号通知
SaveForceData.AddAttr() SHALL 在修改资源值后，如果该势力为玩家势力（isPlayer=true），通过 PanelManager.SendSignal 发送 `ForceResChange` 信号，信号参数 parm1 为属性名称（如 "gold"），parm2 为修改后的当前值。

#### Scenario: 玩家资源变化时发送信号
- **WHEN** 玩家势力的 AddAttr() 被调用修改了资源
- **THEN** PanelManager.SendSignal("ForceResChange", attrName, currentValue) 被调用，parm2 为修改后的值

#### Scenario: AI 势力资源变化时不发送信号
- **WHEN** 非玩家势力的 AddAttr() 被调用修改了资源
- **THEN** 不发送 ForceResChange 信号

### Requirement: PanelManager 监听资源变化刷新
PanelManager SHALL 监听 `ForceResChange` 信号，收到信号后遍历 topNode 子对象的 ResItem 组件，找到 attrName 与 parm1 匹配的 ResItem，调用 `SetItem(parm1, parm2)` 刷新显示。

#### Scenario: 收到 ForceResChange 信号
- **WHEN** PanelManager 收到 ForceResChange 信号，parm1="gold"，parm2=150
- **THEN** 找到 attrName="gold" 的 ResItem，更新其显示为 150

### Requirement: CityAttrConfig force 属性 ID 修复
CityAttrConfig.Load() 中所有 IsForceAttr=true 的配置项 SHALL 拥有唯一的 ID，而非全部使用 ID=12。gold=12, steel=13, horse=14, wood=15, stone=16。同时 idxname 和 idxCname 字典的映射 SHALL 同步更新。

#### Scenario: 通过 GetConfigByname 查询 force 属性
- **WHEN** 调用 CityAttrConfig.GetConfigByname("gold")
- **THEN** 返回 gold 的配置（Id=12, name="gold", Cname="金钱", IsForceAttr=true）

#### Scenario: 通过 GetConfigByname 查询 steel 属性
- **WHEN** 调用 CityAttrConfig.GetConfigByname("steel")
- **THEN** 返回 steel 的配置（Id=13, name="steel", Cname="铁", IsForceAttr=true），而非 stone 的配置

## MODIFIED Requirements

### Requirement: MainPanelManager 移除 topNode 逻辑
MainPanelManager SHALL 不再包含 `topNode` 字段和 `InitForceControls()` 方法。SendSignal 中的 `CityForceChange` 分支不再调用 InitForceControls()。

### Requirement: PanelManager.topNode 用途变更
PanelManager.topNode 从原来传递给 MainPanelManager 使用的势力信息容器，变更为 PanelManager 自行管理的资源显示容器。

## REMOVED Requirements

### Requirement: PlayerInfoControl 势力信息显示
**Reason**: topNode 不再显示各势力信息卡片，改为显示玩家资源。PlayerInfoControl 及其 PlayerInfoCell prefab 引用不再需要。
**Migration**: PlayerInfoControl 的点击选择势力功能（SelectPlayer 信号）暂不迁移，后续如需恢复可另行实现。
