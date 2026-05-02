# Tasks

- [x] Task 1: ArmsItemControl 增加兵种类型匹配背景色逻辑
  - [x] SubTask 1.1: 修改 Init 方法签名，增加 int armsId 参数
  - [x] SubTask 1.2: 根据 armsId 获取 ArmsConfig.Type，与 attrConfig.name 对比，匹配时 BG 设为绿色，不匹配设为黑色

- [x] Task 2: HeroInfoPanelManager 绑定 armsChangeBtn 点击事件并传递 armsId
  - [x] SubTask 2.1: 在 Start 中为 armsChangeBtn.onClick 添加监听，调用 SideArmysSelector.SetContext(heroId) 并 ShowSideBar
  - [x] SubTask 2.2: 修改 UpdateArmsPanel，获取当前英雄的 armsId，传给 ArmsItemControl.Init

- [x] Task 3: SideArmysSelector 增加上下文传递和确认逻辑
  - [x] SubTask 3.1: 增加静态字段 currentHeroId 和静态方法 SetContext(int heroId, Action callback)
  - [x] SubTask 3.2: 在 Start 中为 confirmButton.onClick 添加监听
  - [x] SubTask 3.3: 确认逻辑：检查 selectedItem 是否为空，为空则不操作
  - [x] SubTask 3.4: 确认逻辑：获取选中兵种 ID，调用 SaveForceData.CanAffordArms 校验资源
  - [x] SubTask 3.5: 确认逻辑：资源不足时调用 SystemTip.Instance.ShowTip("资源不足")
  - [x] SubTask 3.6: 确认逻辑：资源充足时调用 SaveHeroData.SetArmsId，关闭侧边栏，通知 HeroInfoPanelManager 刷新

- [x] Task 4: HeroInfoPanelManager 支持兵种变更后刷新 ArmsItemControl 背景色
  - [x] SubTask 4.1: 添加 RefreshArmsBG 方法，遍历 armsItems 调用 UpdateBGColor

- [x] Task 5: SelectArmsItem 增加 GetArmsId 方法
  - [x] SubTask 5.1: 添加 armsId 私有字段，在 SetData 中赋值
  - [x] SubTask 5.2: 添加 GetArmsId() 公共方法

# Task Dependencies
- Task 1 和 Task 2 可并行
- Task 3 依赖 Task 2（需要 HeroInfoPanelManager 传递 heroId）
- Task 4 依赖 Task 1 和 Task 3
- Task 5 与 Task 3 配合
