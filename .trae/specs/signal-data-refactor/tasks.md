# Tasks

- [x] T1: 创建 SignalData 基类及 7 个派生类（PO/SignalData.cs）
  - SignalData 基类：`public string Name;`
  - CityResChangeSignal：Name="CityResChange", `string ResType; int Value;`
  - CityForceChangeSignal：Name="CityForceChange", `int CityId;`
  - ForceResChangeSignal：Name="ForceResChange", `string ResType; int Value;`
  - PhaseChangeSignal：Name="PhaseChange", `string PhaseName; int ForceId;`
  - AICheckSignal：Name="AICheck", `string AIName; int ForceId;`
  - RoundChangeSignal：Name="RoundChange", `int Round;`
  - CityAttrChangeSignal：Name="CityAttrChange", `int CityId;`
- [x] T2: 修改 IPanelEvent 接口签名（PO/PanelEvent.cs）
  - `void SendSignal(string name, string parm1, int parm2)` → `void SendSignal(SignalData data)`
- [x] T3: 修改 PanelManager.SendSignal 实现（Controls/PanelManager.cs:423-445）
  - 签名改为 `void SendSignal(SignalData data)`
  - `name` → `data.Name`
  - ForceResChange 分支：`as ForceResChangeSignal`，使用 `signal.ResType`, `signal.Value`
  - 转发调用改为 `SendSignal(data)`
  - 日志更新
- [x] T4: 修改 CityPanelManager.SendSignal 实现（CityPanelManager.cs:499-505）
  - 签名改为 `void SendSignal(SignalData data)`
  - CityResChange 分支：`as CityResChangeSignal`，使用 `signal.ResType`, `signal.Value`
- [x] T5: 修改 MainPanelManager.SendSignal 实现（MainPanelManager.cs:307-368）
  - 签名改为 `void SendSignal(SignalData data)`
  - PhaseChange：`as PhaseChangeSignal`，使用 `signal.PhaseName`, `signal.ForceId`
  - CityForceChange：`as CityForceChangeSignal`，使用 `signal.CityId`
  - RoundChange：`as RoundChangeSignal`，使用 `signal.Round`
  - AICheck：`as AICheckSignal`，使用 `signal.AIName`, `signal.ForceId`
  - 转发给 cityDetail 改为 `SendSignal(data)`
  - 日志更新
- [x] T6: 修改 CityDetail.SendSignal 实现（CityDetail.cs:173-179）
  - 签名改为 `void SendSignal(SignalData data)`
  - CityAttrChange 分支：`as CityAttrChangeSignal`（逻辑不变，仍用自身 cityId）
- [x] T7: 修改 PopHeroBattleSelectPanelManager.SendSignal 实现（PopHeroBattleSelectPanelManager.cs:145-154）
  - 签名改为 `void SendSignal(SignalData data)`
  - CityAttrChange 分支：`as CityAttrChangeSignal`
- [x] T8: 修改 SaveCityData.cs 中的 2 处调用（行 230, 361）
- [x] T9: 修改 SaveForceData.cs 中的 5 处调用（行 61, 149, 150, 154, 408）
- [x] T10: 修改 GameManager.cs 中的 4 处调用（行 220, 284, 291, 367）
- [x] T11: 修改 BattleUIManager.cs 中的 1 处调用（行 85）
- [x] T12: 修改 PopArmySetManager.cs 中的 1 处调用（行 55）
- [x] T13: 修改 PopResultPanelManager.cs 中的 1 处调用（行 274）

# Task Dependencies

- T2 依赖 T1（接口引用 SignalData 类型）
- T3-T7 依赖 T2（实现类需满足接口签名）
- T8-T13 依赖 T3（调用站点需匹配新签名）
