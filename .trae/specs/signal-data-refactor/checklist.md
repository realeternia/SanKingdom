# Checklist

## 编译验证
- [x] 项目无编译错误

## SignalData 继承体系
- [x] SignalData 基类包含 `public string Name;` 字段
- [x] 7 个派生类均继承 SignalData
- [x] 每个派生类在无参构造函数中设置正确的 Name 常量值
- [x] 派生类字段命名语义化（ResType/Value/CityId/PhaseName/ForceId/AIName/Round）
- [x] 遵循 PO 目录风格（PascalCase 公共字段、无命名空间、无基类继承除 SignalData 外）

## 接口一致性
- [x] IPanelEvent 接口签名已更新为 `void SendSignal(SignalData data)`
- [x] 所有 5 个实现类签名已同步更新

## 实现类 as 转换正确性
- [x] PanelManager：ForceResChange → `as ForceResChangeSignal`，使用 signal.ResType, signal.Value
- [x] CityPanelManager：CityResChange → `as CityResChangeSignal`，使用 signal.ResType, signal.Value
- [x] MainPanelManager：PhaseChange → `as PhaseChangeSignal`
- [x] MainPanelManager：CityForceChange → `as CityForceChangeSignal`，使用 signal.CityId
- [x] MainPanelManager：RoundChange → `as RoundChangeSignal`，使用 signal.Round
- [x] MainPanelManager：AICheck → `as AICheckSignal`，使用 signal.AIName, signal.ForceId
- [x] CityDetail：CityAttrChange → `as CityAttrChangeSignal`
- [x] PopHeroBattleSelectPanelManager：CityAttrChange → `as CityAttrChangeSignal`

## 调用站点完整性
- [x] SaveCityData.cs 2 处调用已更新
- [x] SaveForceData.cs 5 处调用已更新
- [x] GameManager.cs 4 处调用已更新
- [x] BattleUIManager.cs 1 处调用已更新
- [x] PopArmySetManager.cs 1 处调用已更新
- [x] PopResultPanelManager.cs 1 处调用已更新

## 逻辑等价性
- [x] 所有信号处理逻辑与重构前等价
- [x] 日志输出使用 data.Name 及转换后的字段

## 代码风格
- [x] 无多余注释
- [x] 不使用 UnityEngine.Debug.Log，统一使用 GameLog
