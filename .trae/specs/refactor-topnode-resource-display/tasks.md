# Tasks

* [x] Task 1: 修复 CityAttrConfig force 属性 ID bug

  * [x] SubTask 1.1: 将 steel ID 从 12 改为 13，horse ID 改为 14，wood ID 改为 15，stone ID 改为 16

  * [x] SubTask 1.2: 更新 idxname 和 idxCname 字典中 steel/horse/wood/stone 的映射值

* [x] Task 2: 删除 MainPanelManager 的 topNode 相关逻辑

  * [x] SubTask 2.1: 删除 MainPanelManager 中的 `public GameObject topNode` 字段

  * [x] SubTask 2.2: 删除 `InitForceControls()` 方法

  * [x] SubTask 2.3: 移除 Start() 中对 InitForceControls() 的调用

  * [x] SubTask 2.4: 移除 SendSignal 中 CityForceChange 分支对 InitForceControls() 的调用

* [x] Task 3: 删除 PlayerInfoControl.cs 文件

* [x] Task 4: 扩展 ResItem 添加 attrName 字段

  * [x] SubTask 4.1: 在 ResItem 中添加 `public string attrName` 字段

  * [x] SubTask 4.2: 在 SetItem 方法中记录 `this.attrName = name`

* [x] Task 5: 实现 PanelManager.topNode 资源初始化逻辑

  * [x] SubTask 5.1: 实现 `InitTopNodeResItems()` 方法：清除 topNode 子对象，遍历 CityAttrConfig.ConfigList 中 IsForceAttr=true 的配置，实例化 ResBase.prefab，添加 ResItem 组件，调用 SetItem 设置图标和玩家势力资源值

  * [x] SubTask 5.2: 在 ShowWorld() 中调用 InitTopNodeResItems()

* [x] Task 6: 实现 SaveForceData.AddAttr 资源变化信号

  * [x] SubTask 6.1: 在 SaveForceData.AddAttr() 方法的 switch 语句之后，检查 isPlayer 为 true 时调用 PanelManager.Instance.SendSignal("ForceResChange", type.ToLower(), GetAttr(type.ToLower()))

* [x] Task 7: 实现 PanelManager 监听 ForceResChange 信号刷新资源

  * [x] SubTask 7.1: 在 PanelManager.SendSignal() 中添加对 ForceResChange 信号的处理：遍历 topNode 子对象的 ResItem 组件，找到 attrName 与 parm1 匹配的 ResItem，调用 SetItem(parm1, parm2) 刷新

# Task Dependencies

* \[Task 5] depends on \[Task 1] (需要正确的 CityAttrConfig 才能遍历 force 属性)

* \[Task 5] depends on \[Task 4] (需要 ResItem 有 attrName 字段)

* \[Task 7] depends on \[Task 4] (需要 ResItem 有 attrName 字段用于匹配)

* \[Task 6] depends on \[Task 1] (需要正确的 CityAttrConfig 才能发送信号)

* \[Task 2] 和 \[Task 3] 可独立执行

