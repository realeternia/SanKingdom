# HeroAttrConfig 规格说明

## 1. 概述

参考 `CityAttrConfig` 的设计模式，创建 `HeroAttrConfig` 配置表，用于统一管理 Hero 属性的元信息，包括：
- 属性名称映射（替代 `NameTransTool.GetAttrCName`）
- 属性颜色显示规则（从 `HeroConfig.FieldMetaInfo.fieldRule` 读取作为初始值）

## 2. 背景

### 2.1 属性名称映射分散

当前属性名称映射存在于两个地方：

**NameTransTool.cs** (使用中):
```csharp
case "str": return "武力";
case "inte": return "智力";
case "fair": return "政治";      // 注意：与 HeroConfig 不同
case "leadship": return "统率";  // 注意：与 HeroConfig 不同
case "charm": return "魅力";
```

**HeroConfig.FieldMetaInfo**:
```csharp
{"LeadShip", new FieldMetaInfo("统帅", "int", 60)},  // 统帅 vs 统率
{"Str", new FieldMetaInfo("武力", "int", 60)},
{"Inte", new FieldMetaInfo("智力", "int", 60)},
{"Fair", new FieldMetaInfo("内政", "int", 60)},      // 内政 vs 政治
{"Charm", new FieldMetaInfo("魅力", "int", 60)},
```

### 2.2 属性颜色硬编码

属性颜色判断逻辑分散在多个文件中：

| 文件 | 颜色逻辑 |
|------|----------|
| `HeroInfoPanelManager.cs` | >=95 红色, >=90 黄色 |
| `PopHeroSelectPanelCell.cs` | >=95 红色, >=90 橙色 |
| `RankCellInfo.cs` | >=95 红色, >=90 黄色 |
| `CityCellHero.cs` | >=90 红色, >=80 黄色, >=70 绿色 |

### 2.3 HeroConfig 已有的 fieldRule 格式

HeroConfig 中兵种驾驭属性已有颜色规则：
```
"10:#FF9900,8-9:#995500,6-7:#33CC33,4-5:#3333CC"
```
格式为：`阈值或范围:颜色`，多个规则用逗号分隔。

## 3. 设计方案

### 3.1 HeroAttrConfig 结构

```csharp
public class HeroAttrConfig
{
    public int Id;                    // 属性ID
    public string name;               // 属性英文名 (str, inte, fair, charm, leadShip, loyalty)
    public string Cname;              // 属性中文名 (武力, 智力, 内政, 魅力, 统帅, 忠诚度)
    public string ColorRule;          // 颜色规则，格式: "95:#FF0000,90:#FFFF00,0:#FFFFFF"
    public string Icon;               // 属性图标名称（预留）
}
```

### 3.2 颜色规则格式

参考 HeroConfig.FieldMetaInfo.fieldRule 格式，支持两种格式：
- 单值阈值：`95:#FF0000` 表示 >=95 使用红色
- 范围阈值：`8-9:#995500` 表示 8-9 之间使用该颜色

规则从高到低匹配，第一个匹配的规则生效。

### 3.3 属性列表

| Id | name | Cname | ColorRule | 说明 |
|----|------|-------|-----------|------|
| 1 | str | 武力 | 95:#FF0000,90:#FFFF00,0:#FFFFFF | >=95红, >=90黄 |
| 2 | inte | 智力 | 95:#FF0000,90:#FFFF00,0:#FFFFFF | >=95红, >=90黄 |
| 3 | fair | 内政 | 95:#FF0000,90:#FFFF00,0:#FFFFFF | >=95红, >=90黄 |
| 4 | charm | 魅力 | 95:#FF0000,90:#FFFF00,0:#FFFFFF | >=95红, >=90黄 |
| 5 | leadShip | 统帅 | 95:#FF0000,90:#FFFF00,0:#FFFFFF | >=95红, >=90黄 |
| 6 | loyalty | 忠诚度 | 80:#FFFFFF,50:#FFA500,0:#FF0000 | >=80白, >=50橙, <50红 |

**注意**: 中文名称统一使用 HeroConfig.FieldMetaInfo 中的定义（统帅、内政），保持一致性。

### 3.4 辅助方法

在 `HeroAttrConfig` 中提供以下静态方法：
- `GetCName(string attrName)` - 获取属性中文名（替代 NameTransTool.GetAttrCName）
- `GetColorByValue(string attrName, int value)` - 根据属性名和值获取颜色
- `GetColoredText(string attrName, int value)` - 根据属性名和值获取带颜色标签的文本

## 4. 影响范围

### 4.1 需要修改的文件

1. **新增文件**
   - `Assets/Resources/Scripts/Configs/HeroAttrConfig_s.cs` - HeroAttrConfig 配置类

2. **修改文件**
   - `Assets/Resources/Scripts/Configs/ConfigManager.cs` - 添加 HeroAttrConfig.Load() 调用
   - `Assets/Resources/Scripts/SystemTool/NameTransTool.cs` - 使用 HeroAttrConfig.GetCName 替代硬编码
   - `Assets/Resources/Scripts/HeroInfoPanelManager.cs` - 使用 HeroAttrConfig 获取属性颜色
   - `Assets/Resources/Scripts/PopHeroSelectPanelCell.cs` - 使用 HeroAttrConfig 获取属性颜色
   - `Assets/Resources/Scripts/RankCellInfo.cs` - 使用 HeroAttrConfig 获取属性颜色
   - `Assets/Resources/Scripts/CityCellHero.cs` - 使用 HeroAttrConfig 获取属性颜色

3. **项目文件**
   - `Assembly-CSharp.csproj` - 添加 HeroAttrConfig_s.cs 编译条目

### 4.2 不修改的逻辑

- 属性值的获取和计算逻辑保持不变
- SaveHeroData.GetAttr() 方法保持不变
- HeroConfig 中的属性定义保持不变

## 5. 实现优先级

1. 创建 HeroAttrConfig_s.cs 配置类
2. 在 ConfigManager 中添加加载调用
3. 修改 NameTransTool 使用 HeroAttrConfig
4. 修改各 UI 文件使用新的颜色获取方法
5. 更新 Assembly-CSharp.csproj

## 6. 验收标准

1. HeroAttrConfig 配置类正确加载
2. NameTransTool.GetAttrCName 使用 HeroAttrConfig 获取中文名
3. 各 UI 界面的属性颜色显示正确
4. 修改颜色规则时只需修改配置表，无需修改业务代码
5. 代码中不再有硬编码的属性颜色阈值
