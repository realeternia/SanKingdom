# SignalData 继承体系重构 Spec

## Why

当前 `SendSignal(string name, string parm1, int parm2)` 使用松散参数传递信号数据，参数含义不明确且难以扩展。将信号参数封装为继承体系，基类持有信号名称，派生类持有各自专属字段，提升类型安全性和可读性。

## What Changes

- **BREAKING**: `IPanelEvent.SendSignal` 签名从 `void SendSignal(string name, string parm1, int parm2)` 改为 `void SendSignal(SignalData data)`
- 新建 `SignalData` 基类（含 `Name` 字段），放置于 PO 目录
- 新建 7 个派生类，每个对应一种现有信号，在构造函数中设置 `Name`
- 所有实现类和调用站点同步更新

## Impact

- Affected code: `PO/PanelEvent.cs`, `Controls/PanelManager.cs`, `CityPanelManager.cs`, `MainPanelManager.cs`, `CityDetail.cs`, `PopHeroBattleSelectPanelManager.cs`, `SaveCityData.cs`, `SaveForceData.cs`, `GameManager.cs`, `BattleUIManager.cs`, `PopArmySetManager.cs`, `PopResultPanelManager.cs`

## ADDED Requirements

### Requirement: SignalData 继承体系

系统 SHALL 提供以下类层次，所有类放置于 `Assets/Resources/Scripts/PO/SignalData.cs`：

```
SignalData (基类)
├── CityResChangeSignal      — 城市资源变化
├── CityForceChangeSignal    — 城市势力归属变化
├── ForceResChangeSignal     — 势力资源变化
├── PhaseChangeSignal        — 回合阶段变化
├── AICheckSignal            — AI 行为检查
├── RoundChangeSignal        — 回合数变化
└── CityAttrChangeSignal     — 城市属性变化
```

#### 基类 SignalData

```csharp
public class SignalData
{
    public string Name;
}
```

#### 派生类定义

| 派生类 | Name 常量值 | 专属字段 | 字段来源 |
|--------|------------|---------|---------|
| CityResChangeSignal | "CityResChange" | `string ResType; int Value;` | 原 parm1=资源类型, parm2=资源值 |
| CityForceChangeSignal | "CityForceChange" | `int CityId;` | 原 parm2=cityId |
| ForceResChangeSignal | "ForceResChange" | `string ResType; int Value;` | 原 parm1=资源类型, parm2=资源值 |
| PhaseChangeSignal | "PhaseChange" | `string PhaseName; int ForceId;` | 原 parm1=阶段名, parm2=forceId |
| AICheckSignal | "AICheck" | `string AIName; int ForceId;` | 原 parm1=AI名称, parm2=forceId |
| RoundChangeSignal | "RoundChange" | `int Round;` | 原 parm2=回合数 |
| CityAttrChangeSignal | "CityAttrChange" | `int CityId;` | 原 parm2=cityId |

每个派生类在无参构造函数中设置 `Name` 为对应常量值。

#### Scenario: 调用方构造信号

- **WHEN** 城市资源变化时
- **THEN** 使用 `new CityResChangeSignal { ResType = type.ToLower(), Value = GetAttr(type.ToLower()) }` 构造信号

#### Scenario: 接收方处理信号

- **WHEN** 面板收到 SignalData
- **THEN** 先检查 `data.Name` 判断是否需要处理，若需要则 `as` 转换为具体派生类后使用专属字段

示例：
```csharp
public void SendSignal(SignalData data)
{
    if (data.Name == "CityResChange")
    {
        var signal = data as CityResChangeSignal;
        RefreshTopNodeResItem(signal.ResType, signal.Value);
    }
}
```

### Requirement: IPanelEvent 接口变更

接口签名 SHALL 变更为：
```csharp
public interface IPanelEvent
{
    void SendSignal(SignalData data);
}
```

## MODIFIED Requirements

### Requirement: PanelManager.SendSignal

签名改为 `public void SendSignal(SignalData data)`，逻辑等价替换：
- `name` → `data.Name`
- `parm1` → 根据 `data.Name` 进行 `as` 转换后访问专属字段
- `parm2` → 同上
- 日志输出使用 `data.Name` 及转换后的字段

ForceResChange 处理：
```csharp
if (data.Name == "ForceResChange")
{
    var signal = data as ForceResChangeSignal;
    RefreshTopNodeResItem(signal.ResType, signal.Value);
}
```

### Requirement: CityPanelManager.SendSignal

签名改为 `public void SendSignal(SignalData data)`，CityResChange 处理：
```csharp
if (data.Name == "CityResChange")
{
    var signal = data as CityResChangeSignal;
    RefreshTopNodeResItem(signal.ResType, signal.Value);
}
```

### Requirement: MainPanelManager.SendSignal

签名改为 `public void SendSignal(SignalData data)`，各信号处理：
- PhaseChange → `data as PhaseChangeSignal` → 使用 `signal.PhaseName`, `signal.ForceId`
- CityForceChange → `data as CityForceChangeSignal` → 使用 `signal.CityId`
- RoundChange → `data as RoundChangeSignal` → 使用 `signal.Round`
- AICheck → `data as AICheckSignal` → 使用 `signal.AIName`, `signal.ForceId`

### Requirement: CityDetail.SendSignal

签名改为 `public void SendSignal(SignalData data)`，CityAttrChange 处理不变（仍使用自身 cityId）。

### Requirement: PopHeroBattleSelectPanelManager.SendSignal

签名改为 `public void SendSignal(SignalData data)`，CityAttrChange 处理不变。

### Requirement: 所有调用站点

14 处调用站点从 `SendSignal("SignalName", parm1, parm2)` 改为 `SendSignal(new XxxSignal { ... })`：

| 文件 | 原调用 | 新调用 |
|------|--------|--------|
| SaveCityData.cs:230 | `SendSignal("CityResChange", type.ToLower(), GetAttr(...))` | `SendSignal(new CityResChangeSignal { ResType = type.ToLower(), Value = GetAttr(...) })` |
| SaveCityData.cs:361 | `SendSignal("CityForceChange", "", cityId)` | `SendSignal(new CityForceChangeSignal { CityId = cityId })` |
| SaveForceData.cs:61 | `SendSignal("ForceResChange", type.ToLower(), GetAttr(...))` | `SendSignal(new ForceResChangeSignal { ResType = type.ToLower(), Value = GetAttr(...) })` |
| SaveForceData.cs:149 | `SendSignal("PhaseChange", "Planning", forceId)` | `SendSignal(new PhaseChangeSignal { PhaseName = "Planning", ForceId = forceId })` |
| SaveForceData.cs:150 | `SendSignal("AICheck", "", 0)` | `SendSignal(new AICheckSignal { ForceId = 0 })` |
| SaveForceData.cs:154 | `SendSignal("AICheck", Name, forceId)` | `SendSignal(new AICheckSignal { AIName = Name, ForceId = forceId })` |
| SaveForceData.cs:408 | `SendSignal("CityAttrChange", "", destCityId)` | `SendSignal(new CityAttrChangeSignal { CityId = destCityId })` |
| GameManager.cs:220 | `SendSignal("RoundChange", "", SaveData.round)` | `SendSignal(new RoundChangeSignal { Round = SaveData.round })` |
| GameManager.cs:284 | `SendSignal("PhaseChange", "Execution", force.forceId)` | `SendSignal(new PhaseChangeSignal { PhaseName = "Execution", ForceId = force.forceId })` |
| GameManager.cs:291 | `SendSignal("PhaseChange", "Battle", force.forceId)` | `SendSignal(new PhaseChangeSignal { PhaseName = "Battle", ForceId = force.forceId })` |
| GameManager.cs:367 | `SendSignal("AICheck", "", 0)` | `SendSignal(new AICheckSignal { ForceId = 0 })` |
| BattleUIManager.cs:85 | `SendSignal("CityAttrChange", "", 0)` | `SendSignal(new CityAttrChangeSignal { CityId = 0 })` |
| PopArmySetManager.cs:55 | `SendSignal("CityAttrChange", "", 0)` | `SendSignal(new CityAttrChangeSignal { CityId = 0 })` |
| PopResultPanelManager.cs:274 | `SendSignal("CityAttrChange", "", 0)` | `SendSignal(new CityAttrChangeSignal { CityId = 0 })` |
