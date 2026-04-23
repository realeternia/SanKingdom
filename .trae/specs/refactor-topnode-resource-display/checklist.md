# Checklist

- [x] CityAttrConfig 中 gold/steel/horse/wood/stone 拥有唯一 ID（12/13/14/15/16），idxname 和 idxCname 映射正确
- [x] CityAttrConfig.GetConfigByname("gold") 返回 gold 配置而非 stone 配置
- [x] CityAttrConfig.GetConfigByname("steel") 返回 steel 配置而非 stone 配置
- [x] MainPanelManager 不再包含 topNode 字段
- [x] MainPanelManager 不再包含 InitForceControls() 方法
- [x] MainPanelManager.Start() 不再调用 InitForceControls()
- [x] MainPanelManager.SendSignal 中 CityForceChange 不再调用 InitForceControls()
- [x] PlayerInfoControl.cs 文件已删除
- [x] 代码中不存在对 PlayerInfoControl 的引用
- [x] ResItem 包含 attrName 字段，SetItem 时记录属性名
- [x] PanelManager.InitTopNodeResItems() 能遍历 CityAttrConfig 中 IsForceAttr=true 的配置并创建 ResBase 实例
- [x] topNode 下的 ResItem 显示玩家势力的资源图标和数值
- [x] SaveForceData.AddAttr() 在 isPlayer=true 时发送 ForceResChange 信号，parm2 为修改后的当前值
- [x] PanelManager.SendSignal 能处理 ForceResChange 信号，遍历 topNode 子对象匹配 attrName 并刷新 ResItem
- [x] 资源变化后 topNode 中的数值自动更新
