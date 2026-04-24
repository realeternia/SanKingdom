# Checklist: SendSignal 参数封装重构

## 编译验证
- [ ] 项目无编译错误

## 接口一致性
- [ ] IPanelEvent 接口签名已更新为 `void SendSignal(string name, SignalData data)`
- [ ] 所有 IPanelEvent 实现类的签名已同步更新

## 调用站点完整性
- [ ] SaveCityData.cs 2 处调用已更新
- [ ] SaveForceData.cs 4 处调用已更新
- [ ] GameManager.cs 4 处调用已更新
- [ ] BattleUIManager.cs 1 处调用已更新
- [ ] PopArmySetManager.cs 1 处调用已更新
- [ ] PopResultPanelManager.cs 1 处调用已更新

## 逻辑等价性
- [ ] PanelManager 中 parm1 → data.Parm1, parm2 → data.Parm2 替换正确
- [ ] CityPanelManager 中 parm1 → data.Parm1, parm2 → data.Parm2 替换正确
- [ ] MainPanelManager 中 parm1 → data.Parm1, parm2 → data.Parm2 替换正确
- [ ] CityDetail 中逻辑不变
- [ ] PopHeroBattleSelectPanelManager 中逻辑不变
- [ ] 日志输出中 parm1/parm2 引用已更新为 data.Parm1/data.Parm2

## 代码风格
- [ ] SignalData 类遵循 PO 目录风格（PascalCase 公共字段、无命名空间、无基类）
- [ ] 无多余注释
