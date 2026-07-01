# 实现 CityDevSearchConfig 驱动的走访搜索系统

## Context

走访（Search, devId 21202）目前只给随机金币（1-10/人），无法发现武将或资源。`CityDevSearchConfig_s.cs` 已创建但未被任何代码引用。旧的 `CheckFindAction` 武将发现逻辑是死代码（devId 21202 的 `Type="run"` 导致 `ExecuteCityDev` 永远不会被调用）。本计划将 `CityDevSearchConfig` 接入 `ExecuteCitySearch`，实现配置驱动的资源/武将发现系统，并清理死代码。

## 设计决策

- **保留基线金币**：每个搜索武将仍获得 1-10 金币（`CalculateSearchGoldAmount`），配置驱动的发现作为额外奖励
- **ResType 拆分**：`herofind`（普通武将）和 `herostar`（名将）分开，各自有不同概率
- **新增 AttrName 字段**：指定具体资源名（如 "gold"、"food"），herofind/herostar 类型留空。这是功能必需字段
- **Probability × Weight = 实际触发概率**：每个配置项独立判定，多个结果可同时触发
- **Condition 检查**：搜索武将的属性需满足条件（如 "inte>=90"），空条件视为无条件
- **武将发现**：沿用 `CheckFindAction` 的 BornCity 匹配逻辑，但区分名将/普通武将

## 修改文件清单

### 1. CityDevSearchConfig_s.cs
**新增 3 个字段**：`AttrName`（string）、`AttrValMin`（int）、`AttrValMax`（int）

**ResType 值调整**：
- `cityattr` — 城市属性资源（gold/food/soldier/wall/happy）
- `forceattr` — 势力属性资源（steel/horse/wood/stone/elephant/salt/fish）
- `herofind` — 发现普通武将（StarHero=false）
- `herostar` — 发现名将（StarHero=true）

**数据更新**（保留金币相关条目但改为额外发现，新增 AttrName/AttrValMin/AttrValMax）：
```
22001 发现金钱  cityattr  gold     1.0  0.3  charm>=70   5  15
22002 发现粮食  cityattr  food     1.0  0.3  fair>=70    5  15
22003 发现士兵  cityattr  soldier  0.8  0.2  leadShip>=70 5  12
22004 发现钢材  forceattr steel    0.5  0.15 inte>=70    1  5
22005 发现战马  forceattr horse    0.5  0.15 inte>=70    1  5
22006 发现木材  forceattr wood     0.5  0.15 fair>=70    1  5
22007 发现石料  forceattr stone    0.4  0.1  fair>=75    1  4
22008 发现名将  herostar  ""       0.1  0.05 inte>=90   0  0
22009 发现将领  herofind  ""       0.3  0.1  charm>=75   0  0
22010 发现贤才  herofind  ""       0.2  0.08 fair>=85   0  0
```

更新 fieldMeta、构造函数、Load 数据。

### 2. SysFormula.cs — SysFormula.Hero 新增方法

```csharp
/// 解析条件字符串（如 "inte>=90"）并检查英雄属性是否满足
/// 支持运算符：>=, <=, >, <, ==, !=
/// 空条件返回 true
public static bool CheckHeroCondition(string condition, SaveHeroData hero)
```

实现逻辑：
1. 空字符串 → return true
2. 按 `>=`、`<=`、`==`、`!=`、`>`、`<` 顺序查找运算符
3. 拆分出属性名和阈值
4. `hero.GetAttr(attrName)` 获取属性值
5. 比较返回 bool

### 3. SaveForceData.cs — 核心改动

#### 3a. 重写 ExecuteCitySearch（约 881-942 行）

新流程：
```
1. 验证 heroIds（保留现有逻辑）
2. 对每个搜索武将：
   a. 基线金币：CalculateSearchGoldAmount()（保留现有逻辑）
   b. 遍历 CityDevSearchConfig.ConfigList：
      - CheckHeroCondition(entry.Condition, hero) → 跳过不满足的
      - 实际概率 = entry.Probability * entry.Weight
      - SysRandom.Value < 实际概率 → 触发
      - 触发后按 ResType 处理：
        · cityattr: amount=SysRandom.Range(AttrValMin, AttrValMax+1)
                    cityData.AddAttr(entry.AttrName, amount, "走访发现")
        · forceattr: amount=SysRandom.Range(AttrValMin, AttrValMax+1)
                     AddAttr(entry.AttrName, amount, "走访发现")
        · herofind: FindUndiscoveredHero(cityId, false)
        · herostar: FindUndiscoveredHero(cityId, true)
      - 添加对应 AttrData 到结果列表
3. 汇总金币（保留现有逻辑）
4. cityData.AddAction / AddKingActionCount / MarkHeroesActed（保留现有逻辑）
```

#### 3b. 新增私有方法 FindUndiscoveredHero

```csharp
private SaveHeroData FindUndiscoveredHero(int cityId, bool starHero)
```

逻辑（从 CheckFindAction 提取并改进）：
1. `WorldConfig.GetConfig(cityId).Cname` 获取城市中文名
2. `GameManager.Instance.GetCurrentYear()` 获取当前年份
3. 收集已存在英雄 ID 集合
4. 遍历 HeroConfig.ConfigList：
   - `heroConfig.StarHero != starHero` → 跳过
   - `heroConfig.BornCity != cityName` → 跳过
   - `currentYear - BornYear < BORN_AGE` → 跳过
   - 已在游戏中 → 跳过
5. 匹配则调用 `SaveHeroData.CreateWildHero(heroConfig.Id, cityId)`
6. 添加到 `SaveData.heros`
7. `cityData.RecalculateHeros()`
8. 返回新英雄；无匹配返回 null 并日志

#### 3c. 删除死代码

- 删除 `CheckFindAction` 方法（462-510 行）
- 删除 `ExecuteCityDev` 中的 `if (devConfig.ActionName == "find")` 分支（454-457 行）

### 4. CityDevConfig_s.cs

- devId 21202 的 `ActionName` 从 `"find"` 改为 `"search"`（201 行）

### 5. CitySearchPanelManager.cs

#### 5a. GetHeroAttText（223-226 行）
当前返回固定金币范围 `+1~10`。改为显示搜索相关属性：
```csharp
private string GetHeroAttText(int heroId)
{
    var hero = GameManager.Instance.GetHero(heroId);
    if (hero == null) return "";
    return $"智{hero.inte} 魅{hero.charm}";
}
```

#### 5b. RefreshGoldDisplay（47-66 行）
保留金币范围显示不变（基线金币逻辑未变）。selectedCount=0 时显示当前金币，选中时显示 `gold(+min~max)`。

### 6. Assembly-CSharp.csproj
无需修改（CityDevSearchConfig_s.cs 已注册）。

## 不需要修改的文件

- **AIToolSearch.cs** — 调用 `ExecuteCitySearch`，自动受益于新逻辑
- **SystemConst.cs** — 无新常量需求（概率/数值均在配置表中）
- **SaveHeroData.cs** — `CreateWildHero` 保留不变（仍被新逻辑调用）
- **PopResultPanelManager / PopResultCell** — 通用结果展示组件，AttrData 结构不变

## 验证

1. 打开走访面板，确认英雄列表显示 `智X 魅Y` 而非金币范围
2. 选择高智力武将执行走访，确认结果弹窗中有金币 + 可能的资源/武将发现
3. 在有未发现武将的城市（BornCity 匹配）反复走访，确认能发现武将
4. 验证名将（herostar）触发概率明显低于普通武将（herofind）
5. 验证 Condition 生效：低属性武将不触发高门槛条目
6. 确认 devId 21202 的 ActionName 已改为 "search"
7. 全局搜索确认无 "CheckFindAction" 残留引用
8. 确认 `ExecuteCityDev` 中无 `"find"` 分支残留
