# Spec: SendSignal 参数封装重构

## 目标

将 `SendSignal(string name, string parm1, int parm2)` 中的 `parm1` 和 `parm2` 封装为一个数据类，放置于 PO 目录下，提升信号参数的可读性和可扩展性。

## 背景

当前 `SendSignal` 方法使用 `string parm1, int parm2` 两个松散参数传递信号数据，存在以下问题：
- 参数含义不明确，需要查阅调用处才能理解 `parm1`/`parm2` 的语义
- 未来如需新增参数，需修改所有调用链的签名
- 与项目 PO 目录"简单数据结构封装"的定位一致，应将参数封装为数据类

## 方案

### 新建类：SignalData

在 `Assets/Resources/Scripts/PO/SignalData.cs` 中创建：

```csharp
public class SignalData
{
    public string Parm1;
    public int Parm2;
}
```

遵循 PO 目录现有风格：
- 无命名空间
- 公共字段 PascalCase
- 不继承基类
- 不标注 `[Serializable]`

### 接口变更

`IPanelEvent` 接口签名从：
```csharp
void SendSignal(string name, string parm1, int parm2);
```
改为：
```csharp
void SendSignal(string name, SignalData data);
```

### 影响范围

#### 接口定义（1处）
- `PO/PanelEvent.cs` — IPanelEvent 接口

#### 实现类（5处）
- `Controls/PanelManager.cs:423` — 信号分发中心
- `CityPanelManager.cs:499` — 城市面板
- `MainPanelManager.cs:307` — 主界面面板
- `CityDetail.cs:173` — 城市详情面板
- `PopHeroBattleSelectPanelManager.cs:145` — 英雄战斗选择弹窗

#### 调用站点（14处）
- `SaveCityData.cs:230` — CityResChange
- `SaveCityData.cs:361` — CityForceChange
- `SaveForceData.cs:61` — ForceResChange
- `SaveForceData.cs:149` — PhaseChange (Planning)
- `SaveForceData.cs:150` — AICheck
- `SaveForceData.cs:154` — AICheck
- `SaveForceData.cs:408` — CityAttrChange
- `GameManager.cs:220` — RoundChange
- `GameManager.cs:284` — PhaseChange (Execution)
- `GameManager.cs:291` — PhaseChange (Battle)
- `GameManager.cs:367` — AICheck
- `BattleUIManager.cs:85` — CityAttrChange
- `PopArmySetManager.cs:55` — CityAttrChange
- `PopResultPanelManager.cs:274` — CityAttrChange

## 不变项

- `string name` 参数保持不变，仍作为第一个参数
- 信号名称字符串（如 "CityResChange"）不变
- 各实现类的业务逻辑不变，仅将 `parm1` → `data.Parm1`，`parm2` → `data.Parm2`
