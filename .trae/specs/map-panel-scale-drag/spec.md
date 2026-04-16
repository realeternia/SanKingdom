# 地图面板缩放与拖动功能规格说明

## 1. 概述

本规格说明描述了对地图面板(bgPanel)进行缩放和拖动功能的实现。由于 bgPanel 尺寸从 1204×1024 放大到 2560×2560，需要相应调整地图块(mapPiece)的尺寸和位置，并添加拖动功能以便用户能够查看完整的地图。

## 2. 当前实现分析

### 2.1 现有代码结构
- **文件位置**: `Assets/Resources/Scripts/MainPanelManager.cs`
- **关键方法**: `LoadMapPieces()` (第97-163行)
- **地图块预制体**: `Assets/Resources/Prefabs/WorldPiece.prefab`

### 2.2 当前尺寸计算逻辑
```csharp
// 位置计算 (第147行)
rectTransform.anchoredPosition = new Vector2(worldConfig.X/2+texture.width/2/2, -worldConfig.Y/2-texture.height/2/2);

// 尺寸计算 (第150行)
rectTransform.sizeDelta = new Vector2(texture.width/2, texture.height/2);
```

### 2.3 问题分析
- 当前尺寸计算使用 `/2` 作为缩放因子
- bgPanel 放大后，原有的缩放比例不再适用
- 用户无法查看超出可视区域的地图内容

## 3. 需求规格

### 3.1 功能需求

#### FR-1: 地图块尺寸调整
- **描述**: 将所有地图块的尺寸扩大 2.5 倍
- **输入**: 原始纹理尺寸
- **输出**: 扩大后的地图块尺寸
- **计算公式**: `新尺寸 = 原始纹理尺寸 * 2.5`

#### FR-2: 地图块位置调整
- **描述**: 相应调整地图块的位置偏移
- **输入**: WorldConfig 中的 X, Y 坐标
- **输出**: 调整后的 anchoredPosition
- **计算公式**: 
  - `x = worldConfig.X * 2.5 + texture.width * 2.5 / 2`
  - `y = -worldConfig.Y * 2.5 - texture.height * 2.5 / 2`

#### FR-3: 地图面板拖动功能
- **描述**: 允许用户通过鼠标/触摸拖动查看完整地图
- **输入**: 用户拖动操作
- **输出**: bgPanel 位置更新
- **约束**: 
  - 拖动范围限制在地图边界内
  - 支持鼠标和触摸输入

### 3.2 非功能需求

#### NFR-1: 性能
- 拖动操作应流畅，无明显卡顿
- 地图块加载不应阻塞主线程

#### NFR-2: 兼容性
- 保持与现有 WorldPieceControl 的兼容性
- 不影响地图块的点击交互功能

## 4. 技术设计

### 4.1 缩放因子定义
```csharp
private const float MAP_SCALE_FACTOR = 2.5f;
```

### 4.2 尺寸计算修改
```csharp
rectTransform.sizeDelta = new Vector2(texture.width * MAP_SCALE_FACTOR, texture.height * MAP_SCALE_FACTOR);
```

### 4.3 位置计算修改
```csharp
rectTransform.anchoredPosition = new Vector2(
    worldConfig.X * MAP_SCALE_FACTOR + texture.width * MAP_SCALE_FACTOR / 2,
    -worldConfig.Y * MAP_SCALE_FACTOR - texture.height * MAP_SCALE_FACTOR / 2
);
```

### 4.4 拖动功能实现方案

#### 方案选择: 使用 Unity EventSystem 实现

**优点**:
- 原生支持，无需额外依赖
- 同时支持鼠标和触摸输入
- 与现有 UI 系统兼容

**实现步骤**:
1. 创建 `MapDragHandler` 组件
2. 实现 `IDragHandler`, `IBeginDragHandler`, `IEndDragHandler` 接口
3. 添加边界限制逻辑

**核心代码结构**:
```csharp
public class MapDragHandler : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform bgPanelRect;
    private Vector2 dragOffset;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录拖动起始位置
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // 更新面板位置，限制在边界内
    }
}
```

### 4.5 边界限制计算
```csharp
// 假设可视区域为 viewportRect
// bgPanel 尺寸为 2560×2560
// 需要限制 bgPanel 的 anchoredPosition 在合理范围内

Vector2 minPos = new Vector2(
    viewportRect.width - bgPanelRect.width,
    viewportRect.height - bgPanelRect.height
);
Vector2 maxPos = Vector2.zero;

clampedPos = new Vector2(
    Mathf.Clamp(newPos.x, minPos.x, maxPos.x),
    Mathf.Clamp(newPos.y, minPos.y, maxPos.y)
);
```

## 5. 实现计划

### 5.1 修改文件清单
| 文件 | 修改类型 | 说明 |
|------|----------|------|
| `MainPanelManager.cs` | 修改 | 更新尺寸和位置计算逻辑 |
| `MapDragHandler.cs` | 新增 | 实现拖动功能组件 |

### 5.2 依赖关系
- 无新增外部依赖
- 依赖 Unity UI 系统 (UnityEngine.UI)
- 依赖 Unity EventSystem

## 6. 测试要点

### 6.1 功能测试
- [ ] 地图块尺寸正确放大 2.5 倍
- [ ] 地图块位置正确调整
- [ ] 拖动功能正常工作
- [ ] 边界限制正确生效
- [ ] 地图块点击功能不受影响

### 6.2 兼容性测试
- [ ] 现有 WorldPieceControl 功能正常
- [ ] 城市详情面板正常显示
- [ ] 势力信息显示正常

## 7. 风险与缓解

| 风险 | 影响 | 缓解措施 |
|------|------|----------|
| 缩放比例不准确 | 地图显示异常 | 使用常量定义缩放因子，便于调整 |
| 拖动与点击冲突 | 交互异常 | 添加拖动距离阈值判断 |
| 性能问题 | 卡顿 | 使用对象池优化地图块管理 |
