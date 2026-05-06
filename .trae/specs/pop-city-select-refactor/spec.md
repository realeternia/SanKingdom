# Spec: 改造 PopCitySelectPanel 为通用城市列表面板

## 概述

将 `PopCitySelectPanel` 从内部计算城市列表的耦合设计，改造为接受外部传入城市列表的通用选择面板。同时在 `MapTool` 中新增两个静态方法，分别提供"己方城市列表"和"己方城市相邻敌方城市并集列表"的获取能力。

## 当前问题

1. **PopCitySelectPanelManager** 的 `Init(int cityId, bool findEnemy, Action<int>)` 方法内部硬编码了两种城市筛选逻辑（findEnemy=true 查相邻敌方城市，findEnemy=false 查己方城市），导致面板与业务逻辑强耦合。
2. **CityBattlePanelManager** 中引用了未声明的 `cityId` 和 `attrVal1Text` 字段，存在编译错误。
3. **MapTool** 为空类，城市列表获取逻辑散落在各面板中，无法复用。

## 改造方案

### 1. PopCitySelectPanelManager 改造

**改造前签名：**
```csharp
private void Init(int cityId, bool findEnemy, System.Action<int> callback)
public void OnShow(int cityId, bool findEnemy, System.Action<int> callback)
```

**改造后签名：**
```csharp
private void Init(List<int> cityIds, System.Action<int> callback)
public void OnShow(List<int> cityIds, System.Action<int> callback)
```

**改造要点：**
- 移除 `cityId`、`findEnemy` 参数，改为接受 `List<int> cityIds`
- 移除内部的城市筛选逻辑（findEnemy 分支），直接遍历 `cityIds` 创建 cell
- 保留 `mCityId` 字段的移除（不再需要）
- `PopCitySelectPanelCell.Init(int cityId)` 保持不变

### 2. PanelManager 签名同步修改

**改造前：**
```csharp
public void ShowPopCitySelectPanel(int cityId, bool findEnemy, System.Action<int> callback)
```

**改造后：**
```csharp
public void ShowPopCitySelectPanel(List<int> cityIds, System.Action<int> callback)
```

### 3. MapTool 新增两个静态方法

#### 方法1：获取己方所有城市
```csharp
public static List<int> GetOwnCityIds(int forceId)
```
- 遍历 `GameManager.Instance.GetCitiesByForce(forceId)` 返回所有 `cityId`
- 供 CityDevNodeMove 调用

#### 方法2：获取己方所有城市的相邻敌方城市并集
```csharp
public static List<int> GetAdjacentEnemyCityIds(int forceId)
```
- 遍历己方所有城市
- 对每个己方城市，通过 `WorldConfig.GetConfig(cityId).WorldNearIds` 获取相邻城市
- 筛选 `forceId != 己方forceId` 的城市
- 使用 `HashSet<int>` 去重，返回并集列表
- 供 CityBattlePanelManager 调用

### 4. 调用方修改

#### CityDevNodeMove
**改造前：**
```csharp
PanelManager.Instance.ShowPopCitySelectPanel(cityId, true, callback);
```
**改造后：**
```csharp
var cityIds = MapTool.GetOwnCityIds(GameManager.Instance.GetCity(cityId).forceId);
PanelManager.Instance.ShowPopCitySelectPanel(cityIds, callback);
```

#### CityBattlePanelManager
**改造前：**
```csharp
PanelManager.Instance.ShowPopCitySelectPanel(cityId, true, callback);
```
**改造后：**
```csharp
var cityIds = MapTool.GetAdjacentEnemyCityIds(forceId);
PanelManager.Instance.ShowPopCitySelectPanel(cityIds, callback);
```
- 同时修复 `cityId` 和 `attrVal1Text` 未声明的 bug（需要补充字段声明或调整逻辑）

### 5. Assembly-CSharp.csproj

- 确认 `MapTool.cs` 已有 `<Compile Include>` 条目，若无则添加

## 涉及文件

| 文件 | 修改类型 |
|------|---------|
| `Assets/Resources/Scripts/Panels/PopCitySelectPanelManager.cs` | 修改 |
| `Assets/Resources/Scripts/Controls/PanelManager.cs` | 修改 |
| `Assets/Resources/Scripts/SystemTool/MapTool.cs` | 修改 |
| `Assets/Resources/Scripts/Panels/CityDevNodeMove.cs` | 修改 |
| `Assets/Resources/Scripts/Panels/CityBattlePanelManager.cs` | 修改 |
| `Assembly-CSharp.csproj` | 可能修改 |
