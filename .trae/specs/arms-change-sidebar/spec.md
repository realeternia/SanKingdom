# 兵种切换与 ArmsItemControl 背景色规范

## Why

HeroInfoPanel 中的 armsChangeBtn 已声明但未绑定点击事件，SideArmysSelector 的 confirmButton 也未绑定确认逻辑。需要打通从"点击换兵按钮 → 弹出侧边栏选择兵种 → 确认时校验资源 → 更新英雄兵种"的完整流程，并在 ArmsItemControl 中根据当前兵种类型高亮匹配的属性项。

## What Changes

- HeroInfoPanelManager 中为 armsChangeBtn 绑定点击事件，打开 SideArmsSelector 侧边栏
- SideArmysSelector 增加上下文传递机制，接收当前英雄 heroId 和原 armsId
- SideArmysSelector 的 confirmButton 绑定确认逻辑：对比新旧兵种资源消耗增量，校验 SaveForceData 资源是否足够
- 资源不足时通过 SystemTip 飘字提示
- 资源足够时调用 SaveHeroData.SetArmsId 更新兵种，刷新 ArmsItemControl 背景色
- ArmsItemControl.Init 增加当前 armsId 参数，根据 ArmsType 匹配设置 BG 颜色（匹配=绿色，不匹配=黑色）
- HeroInfoPanelManager.UpdateArmsPanel 传递当前英雄的 armsId 给 ArmsItemControl

## Impact

- Affected code: HeroInfoPanelManager.cs, SideArmsSelector.cs, ArmsItemControl.cs
- 依赖已有逻辑: SaveHeroData.SetArmsId(), SaveForceData.CanAffordArms(), SystemTip.ShowTip()

## ADDED Requirements

### Requirement: armsChangeBtn 点击弹出兵种选择侧边栏

HeroInfoPanelManager SHALL 在 armsChangeBtn 点击时，将当前英雄 heroId 传递给 SideArmysSelector 并打开侧边栏。

#### Scenario: 点击换兵按钮
- **WHEN** 玩家在英雄信息面板点击 armsChangeBtn
- **THEN** 打开 SideArmsSelector 侧边栏，SideArmysSelector 获知当前编辑的英雄 heroId

### Requirement: SideArmysSelector 确认按钮校验资源并更新兵种

SideArmysSelector 的 confirmButton 点击时 SHALL 校验势力资源是否足够负担新兵种（排除当前英雄的原兵种消耗），不足时飘字提示，足够时更新英雄兵种。

#### Scenario: 资源充足，成功切换兵种
- **WHEN** 玩家在侧边栏选择新兵种并点击确认按钮
- **AND** SaveForceData.CanAffordArms(newArmsId, heroId) 返回 true
- **THEN** 调用 SaveHeroData.SetArmsId(newArmsId) 更新兵种
- **AND** 关闭侧边栏
- **AND** 刷新 HeroInfoPanel 中的 ArmsItemControl 背景色

#### Scenario: 资源不足，切换失败
- **WHEN** 玩家在侧边栏选择新兵种并点击确认按钮
- **AND** SaveForceData.CanAffordArms(newArmsId, heroId) 返回 false
- **THEN** 通过 SystemTip.Instance.ShowTip("资源不足") 飘字提示
- **AND** 不关闭侧边栏，不修改兵种

#### Scenario: 未选择兵种点击确认
- **WHEN** 玩家未选择任何兵种就点击确认按钮
- **THEN** 不执行任何操作

### Requirement: ArmsItemControl 根据兵种类型设置背景色

ArmsItemControl SHALL 根据当前英雄的 armsId 对应的 ArmsType，将匹配的属性项 BG 设为绿色，其余为黑色。

#### Scenario: 英雄兵种为骑兵（ArmsType.SodHorse）
- **WHEN** ArmsItemControl 初始化时传入 armsId，该 armsId 的 Type 为 SodHorse
- **THEN** name 为 "SodHorse" 的 ArmsItemControl 的 BG 颜色设为绿色
- **AND** 其余 ArmsItemControl 的 BG 颜色为黑色

#### Scenario: 切换兵种后刷新背景色
- **WHEN** 英雄的 armsId 被修改
- **THEN** HeroInfoPanel 中所有 ArmsItemControl 的 BG 颜色根据新 armsId 重新设置

## MODIFIED Requirements

### Requirement: ArmsItemControl.Init 方法签名

ArmsItemControl.Init 增加 armsId 参数：

```csharp
public void Init(HeroAttrConfig attrConfig, HeroConfig heroConfig, int armsId)
```

### Requirement: HeroInfoPanelManager.UpdateArmsPanel 传递 armsId

UpdateArmsPanel 方法在调用 ArmsItemControl.Init 时传入当前英雄的 armsId。

### Requirement: SideArmysSelector 增加上下文

SideArmysSelector 增加静态方法 SetContext(int heroId) 用于在打开侧边栏前设置当前编辑的英雄。
