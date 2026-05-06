# Checklist: PopCitySelectPanel 改造检查清单

## 编译与结构检查
- [ ] MapTool.cs 新增 `GetOwnCityIds` 方法，返回 `List<int>`
- [ ] MapTool.cs 新增 `GetAdjacentEnemyCityIds` 方法，返回 `List<int>`（去重并集）
- [ ] MapTool.cs 在 Assembly-CSharp.csproj 中有 `<Compile Include>` 条目
- [ ] PopCitySelectPanelManager.Init 签名改为 `Init(List<int> cityIds, Action<int> callback)`
- [ ] PopCitySelectPanelManager.OnShow 签名改为 `OnShow(List<int> cityIds, Action<int> callback)`
- [ ] PopCitySelectPanelManager 内部不再有 findEnemy 分支逻辑
- [ ] PanelManager.ShowPopCitySelectPanel 签名改为 `ShowPopCitySelectPanel(List<int> cityIds, Action<int> callback)`
- [ ] CityDevNodeMove 使用 `MapTool.GetOwnCityIds()` 获取城市列表
- [ ] CityBattlePanelManager 使用 `MapTool.GetAdjacentEnemyCityIds()` 获取城市列表
- [ ] CityBattlePanelManager 中 `cityId` 和 `attrVal1Text` 引用错误已修复

## 规范检查
- [ ] 不使用 `UnityEngine.Random`，使用 `SysRandom` 或 `BattleRandom`
- [ ] 不使用 `UnityEngine.Debug.Log`，使用 `GameLog`
- [ ] 无魔法数字，常量提取到 `SystemConst`
- [ ] 无内联计算公式，提取到 `SysFormula`
- [ ] 公共字段使用 camelCase
- [ ] 方法名使用 PascalCase
- [ ] 不在业务代码中添加注释

## 功能验证
- [ ] PopCitySelectPanel 传入空列表时不崩溃
- [ ] GetOwnCityIds 返回的城市均为指定 forceId 的城市
- [ ] GetAdjacentEnemyCityIds 返回的城市均为非己方城市且与己方城市相邻
- [ ] GetAdjacentEnemyCityIds 结果无重复城市ID
