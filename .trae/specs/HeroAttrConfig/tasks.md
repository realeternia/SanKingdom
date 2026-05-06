# HeroAttrConfig 任务清单

## 任务列表

### 任务 1: 创建 HeroAttrConfig 配置类
- **优先级**: 高
- **描述**: 参考 CityAttrConfig 创建 HeroAttrConfig_s.cs
- **文件**: `Assets/Resources/Scripts/Configs/HeroAttrConfig_s.cs`
- **详情**:
  - 定义 HeroAttrConfig 类结构（Id, name, Cname, ColorRule, Icon）
  - 定义 FieldMetaInfo 内部类
  - 实现 Load() 方法初始化配置数据
  - 添加 GetConfig/GetConfigByname 查询方法
  - 实现颜色规则解析方法（支持单值和范围阈值）
  - 实现 GetCName(string attrName) 静态方法
  - 实现 GetColorByValue(string attrName, int value) 静态方法
  - 实现 GetColoredText(string attrName, int value) 静态方法

- [x] **已完成**

### 任务 2: 更新 ConfigManager
- **优先级**: 高
- **描述**: 在 ConfigManager.Init() 中添加 HeroAttrConfig.Load() 调用
- **文件**: `Assets/Resources/Scripts/Configs/ConfigManager.cs`
- **详情**:
  - 在 Init() 方法中添加 HeroAttrConfig.Load()
  - 确保加载顺序正确

- [x] **已完成**

### 任务 3: 更新 Assembly-CSharp.csproj
- **优先级**: 高
- **描述**: 添加 HeroAttrConfig_s.cs 编译条目
- **文件**: `Assembly-CSharp.csproj`
- **详情**:
  - 添加 `<Compile Include="Assets\Resources\Scripts\Configs\HeroAttrConfig_s.cs" />`

- [x] **已完成**

### 任务 4: 修改 NameTransTool
- **优先级**: 中
- **描述**: 使用 HeroAttrConfig 替代硬编码的属性名称映射
- **文件**: `Assets/Resources/Scripts/SystemTool/NameTransTool.cs`
- **详情**:
  - 修改 GetAttrCName 方法使用 HeroAttrConfig.GetCName

- [x] **已完成**

### 任务 5: 修改 HeroInfoPanelManager
- **优先级**: 中
- **描述**: 使用 HeroAttrConfig 替换硬编码颜色逻辑
- **文件**: `Assets/Resources/Scripts/HeroInfoPanelManager.cs`
- **详情**:
  - 修改 GetColoredAttrValue 方法使用 HeroAttrConfig.GetColoredText

- [x] **已完成**

### 任务 6: 修改 PopHeroSelectPanelCell
- **优先级**: 中
- **描述**: 使用 HeroAttrConfig 替换硬编码颜色逻辑
- **文件**: `Assets/Resources/Scripts/PopHeroSelectPanelCell.cs`
- **详情**:
  - 修改 Init 方法中的属性颜色设置逻辑
  - 使用 HeroAttrConfig.GetColorByValue 或 GetColoredText

- [x] **已完成**

### 任务 7: 修改 RankCellInfo
- **优先级**: 中
- **描述**: 使用 HeroAttrConfig 替换硬编码颜色逻辑
- **文件**: `Assets/Resources/Scripts/RankCellInfo.cs`
- **详情**:
  - 修改 Init 方法中的属性颜色设置逻辑
  - 统一五个属性的颜色处理

- [x] **已完成**

### 任务 8: 修改 CityCellHero
- **优先级**: 中
- **描述**: 使用 HeroAttrConfig 替换硬编码颜色逻辑
- **文件**: `Assets/Resources/Scripts/CityCellHero.cs`
- **详情**:
  - 修改 UpdateThumbIcon 方法中的颜色逻辑
  - 添加 weightedAttr 配置项用于加权属性颜色

- [x] **已完成**

## 任务依赖关系

```
任务 1 ──┬──> 任务 2
         │
         ├──> 任务 3
         │
         └──> 任务 4, 5, 6, 7, 8 (并行)
```
