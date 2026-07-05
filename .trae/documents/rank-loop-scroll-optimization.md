# 排行榜循环列表优化方案

## Context

排行榜当前在 `LoadHeroCells()` 中采用「全量 Instantiate + Destroy」模式：每次切换模式/势力都把 `rankRegionMain` 下所有子物体 `Destroy`，然后逐项 `Instantiate` 预制体。全武将模式下武将数量可达数百个，单次刷新会实例化数百个 GameObject，造成主线程卡顿。

项目目前**没有任何对象池或循环列表基础设施**，CityPanelManager / CityUseHeroPanelManager 等其他面板也都是全量实例化模式。

目标：为排行榜主列表 `scrollRectMain` 实现循环列表（LoopScrollRect），只实例化视口可见 + 缓冲区域的项（约 15 个），滚动时复用 GameObject 重新绑定数据。先解决排行榜卡顿，工具类放入 `SystemTool/` 后续可复用到其他面板。

## 方案概览

| 模式 | 数据量 | 是否走循环列表 |
|------|--------|---------------|
| 势力武将 | 少量（单势力武将）| 否，保留原逻辑 |
| 全武将 | 大量（全图武将，数百）| 是 |
| 势力战力 | 少量（未灭亡势力数）| 否，保留原逻辑 |
| 势力城市 | 少量（单势力城市）| 否，保留原逻辑 |
| 全城市 | 中等（全图城市，数十）| 是 |

只有数据量可能过百的「全武将」「全城市」两个模式走循环列表，其他模式保留原全量实例化逻辑，减小改动面与风险。

## 新增文件

### 1. `OOs/ILoopScrollItem.cs` — 循环列表项接口

```csharp
public interface ILoopScrollItem
{
    // 用数据源第 index 项刷新自身（重复调用以复用 GameObject）
    void BindData(int index);
    // 归还到对象池前清理（取消高亮、隐藏头像等可视状态，不销毁对象）
    void OnReturnToPool();
}
```

### 2. `SystemTool/LoopScrollRect.cs` — 通用循环列表工具

挂在 ScrollRect 所在 GameObject 上的 MonoBehaviour 组件，负责：
- 持有数据源 `IList dataSource`（外部传入的 `List<SaveHeroData>` 或 `List<SaveCityData>`）
- 持有项对象池 `Queue<ILoopScrollItem>`
- 监听 `ScrollRect.onValueChanged`，计算可见索引区间并刷新项
- 项用 `RectTransform.anchoredPosition` 直接定位（禁用 Content 上的 `VerticalLayoutGroup`，避免布局冲突）
- 设置 `Content.sizeDelta.y = itemHeight * totalCount`

关键参数：
- `itemHeight`：由调用方传入（取自 prefab 的 RectTransform 高度）
- `buffer`：视口上下各额外渲染的项数，默认 3

对外 API：
- `Initialize(IList dataSource, GameObject prefab, float itemHeight)` — 初始化并填充首屏
- `Clear()` — 回收所有项到池并销毁 GameObject，移除滚动监听
- `GetTotalCount()` — 返回数据源总数，供外部排序使用
- `GetData(int index)` — 返回数据源第 index 项
- `SortItems(string rankType)` — 对数据源排序后刷新可见项（不再操作 SiblingIndex）
- `ForceRefresh()` — 强制刷新当前可见项（用于排序后）

## 修改文件

### 3. `Panels/ListItem/RankCellInfo.cs` 实现 `ILoopScrollItem`

- 新增字段 `private int currentIndex` 保存当前绑定的数据索引
- 实现 `BindData(int index)`：缓存 `currentIndex`，调用现有 `Init(SaveHeroData)`（数据从外部传入的数据源取）
- 实现 `OnReturnToPool()`：调用 `OnSelectHero(false)` 清除选中高亮，并把 `heroPic.gameObject.SetActive(false)`
- 把 `Init(SaveHeroData)` 改为可接收外部传入的 `SaveHeroData`（已是该签名，无需改动）

`viewButton.onClick` 监听在 `Start()` 中添加一次，循环复用时不重复添加。

### 4. `Panels/ListItem/RankCellInfoCity.cs` 实现 `ILoopScrollItem`

- 新增字段 `private int currentIndex`
- 实现 `BindData(int index)`：缓存 `currentIndex`，调用现有 `Init(int cityId)`
- 实现 `OnReturnToPool()`：无额外可视状态需清理，空实现即可

### 5. `Panels/RankPanelManager.cs` 集成 LoopScrollRect

修改 `LoadHeroCells()`：

- 新增字段 `private LoopScrollRect loopScrollMain`（Inspector 引用，挂在 `scrollRectMain` 所在 GameObject 上）
- 「全武将」「全城市」模式不再循环 `Instantiate`，改为：
  1. 构建数据源 `List<SaveHeroData>` / `List<SaveCityData>`
  2. 调用 `loopScrollMain.Initialize(dataSource, rankCellInfoPrefab, cellHeight)`
- 其他模式（势力武将/势力城市/势力战力）保留原有全量 Instantiate 逻辑
- `SortItems(rankType)` 改为：若 `loopScrollMain` 已初始化则调用 `loopScrollMain.SortItems(rankType)`，否则保留原 SiblingIndex 排序
- `mHeroList` 改为从数据源构建（全武将模式下包含所有数据源 heroId，不再只含可见项）
- 切换模式/势力时，先 `loopScrollMain.Clear()` 再走各自加载分支

### 6. `Assembly-CSharp.csproj`

注册两个新文件：
- `<Compile Include="Assets\Resources\Scripts\OOs\ILoopScrollItem.cs" />`
- `<Compile Include="Assets\Resources\Scripts\SystemTool\LoopScrollRect.cs" />`

## 关键设计点

### 视口计算
- `viewportHeight = scrollRectMain.viewport.rect.height`（`m_Viewport` 为 0 时 Unity 自动用 ScrollRect 自身 RectTransform）
- `firstIdx = max(0, floor((-content.anchoredPosition.y) / itemHeight) - buffer)`
- `lastIdx = min(totalCount - 1, floor((-content.anchoredPosition.y + viewportHeight) / itemHeight) + buffer)`

### 项定位
```csharp
var rt = cell.transform as RectTransform;
rt.anchoredPosition = new Vector2(0, -index * itemHeight);
```
项 prefab 的 anchor 应为 `(0, 1)`（顶部对齐），pivot 为 `(0.5, 1)`。当前 prefab 已是顶部对齐布局，保持兼容。

### VerticalLayoutGroup 处理
- `Initialize` 时检测 Content 上的 `VerticalLayoutGroup`，若存在则 `enabled = false`
- `Clear` 时恢复 `enabled = true`，让其他模式（势力武将等）仍能用原布局

### 对象池
- `GetFromPool()`：池非空则 `Dequeue` 并 `SetActive(true)`，否则 `Instantiate` 新项
- `ReturnToPool(cell)`：调用 `cell.OnReturnToPool()`，`SetActive(false)`，`Enqueue`

### 排序刷新
- `SortItems(rankType)`：对数据源 `dataSource` 按 `GetValInt(rankType)` 降序排序，然后 `ForceRefresh()` 重新绑定所有可见项
- 排序后清空 `lastSelectedHero`，避免引用陈旧项

### mHeroList 构建
- 「全武将」模式：`mHeroList = dataSource.Cast<SaveHeroData>().Select(h => h.heroId).ToArray()`
- 其他模式：保留原构建逻辑
- `viewButton.onClick` 仍使用 `rankPanelManager.mHeroList`，全武将模式下数据完整

## 验证

1. 进入游戏，打开排行榜
2. 切换到「全武将」模式：列表流畅加载，无明显卡顿；下方势力选择面板隐藏
3. 滚动列表：项正常显示，无空白/重叠；快速滚动时短暂空白可接受（buffer=3）
4. 点击表头排序按钮（Str/Inte/LeadShip 等）：列表按字段降序，可见项立即刷新
5. 点击武将行 `viewButton`：弹出 `HeroInfoPanel`，可正常翻页查看（验证 mHeroList 完整）
6. 切换到「全城市」模式：列表流畅，城市行 `cityOwner` 显示势力名+颜色
7. 切换到「势力武将」/「势力城市」/「势力战力」：仍走原全量加载逻辑，行为不变
8. 关闭重开排行榜：无 GameObject 泄漏（Clear 时销毁所有项）

## 风险与限制

- 循环列表只优化「全武将」「全城市」两个模式；其他模式仍全量实例化，但项数少（< 30）无性能问题
- 项 prefab 的 anchor/pivot 必须保持顶部对齐，否则 anchoredPosition 定位错位（已确认当前 prefab 符合）
- 排序后会清空 `lastSelectedHero`，选中状态丢失（可接受，排序后本就需要重新选择）
- 不修改 `RankInfoPanel.prefab`，运行时禁用 VerticalLayoutGroup，关闭面板时恢复
