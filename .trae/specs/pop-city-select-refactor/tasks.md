# Tasks: PopCitySelectPanel 改造任务分解

## Task 1: MapTool 新增城市列表获取方法
- **文件**: `Assets/Resources/Scripts/SystemTool/MapTool.cs`
- **操作**: 修改
- **内容**:
  1. 添加 `using System.Collections.Generic;`、`using CommonConfig;` 引用
  2. 实现 `GetOwnCityIds(int forceId)` — 返回己方所有城市ID列表
  3. 实现 `GetAdjacentEnemyCityIds(int forceId)` — 返回己方所有城市的相邻敌方城市并集
- **依赖**: 无

## Task 2: PopCitySelectPanelManager 签名改造
- **文件**: `Assets/Resources/Scripts/Panels/PopCitySelectPanelManager.cs`
- **操作**: 修改
- **内容**:
  1. 修改 `Init` 方法签名：`Init(int cityId, bool findEnemy, Action<int>)` → `Init(List<int> cityIds, Action<int>)`
  2. 修改 `OnShow` 方法签名：`OnShow(int cityId, bool findEnemy, Action<int>)` → `OnShow(List<int> cityIds, Action<int>)`
  3. 移除 `mCityId` 字段
  4. 移除 `findEnemy` 分支逻辑，改为直接遍历 `cityIds` 创建 cell
- **依赖**: 无

## Task 3: PanelManager 签名同步修改
- **文件**: `Assets/Resources/Scripts/Controls/PanelManager.cs`
- **操作**: 修改
- **内容**:
  1. 修改 `ShowPopCitySelectPanel` 签名：`(int cityId, bool findEnemy, Action<int>)` → `(List<int> cityIds, Action<int>)`
  2. 同步修改内部调用 `OnShow` 的参数
- **依赖**: Task 2

## Task 4: CityDevNodeMove 调用方修改
- **文件**: `Assets/Resources/Scripts/Panels/CityDevNodeMove.cs`
- **操作**: 修改
- **内容**:
  1. 修改 `destButton.onClick` 中的调用
  2. 使用 `MapTool.GetOwnCityIds()` 获取城市列表
  3. 传入 `ShowPopCitySelectPanel(cityIds, callback)`
- **依赖**: Task 1, Task 3

## Task 5: CityBattlePanelManager 调用方修改 + Bug 修复
- **文件**: `Assets/Resources/Scripts/Panels/CityBattlePanelManager.cs`
- **操作**: 修改
- **内容**:
  1. 添加 `forceId` 字段（在 `Init` 中赋值），用于 `destButton` 回调
  2. 添加 `attrVal1Text` 字段声明（修复未声明 bug），或调整逻辑移除对它的引用
  3. 修改 `destButton.onClick` 中的调用
  4. 使用 `MapTool.GetAdjacentEnemyCityIds()` 获取城市列表
  5. 传入 `ShowPopCitySelectPanel(cityIds, callback)`
- **依赖**: Task 1, Task 3

## Task 6: Assembly-CSharp.csproj 检查
- **文件**: `Assembly-CSharp.csproj`
- **操作**: 可能修改
- **内容**:
  1. 检查 `MapTool.cs` 是否已有 `<Compile Include>` 条目
  2. 若无则添加
- **依赖**: 无
