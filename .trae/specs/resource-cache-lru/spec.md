# ResourceCache 双层缓存系统 Spec

## Why
当前项目中存在大量 `Resources.Load` 调用，每次加载都需要进行磁盘 I/O 操作，导致性能问题。已有的简单 `ResourceCache` 缺乏内存管理机制，可能导致内存无限增长。需要建立一个分层、有内存限制的缓存系统。

## What Changes
- 重构 `ResourceCache` 为双层缓存架构（UI 缓存 + 战斗缓存）
- 实现 LFU (Least Frequently Used) 淘汰算法
- 添加缓存数量上限和内存大小上限配置
- 扫描并替换项目中所有 `Resources.Load` 调用为缓存调用

## Impact
- Affected specs: 资源加载系统
- Affected code:
  - `SystemTool/ResourceCache.cs` - 核心缓存实现
  - `SystemConst.cs` - 缓存配置常量
  - 所有使用 `Resources.Load` 的文件（约 40+ 个文件）

## ADDED Requirements

### Requirement: 双层缓存架构
系统 SHALL 提供两个独立的资源缓存实例：
- **UICache**: 用于战略层 UI 资源（面板、图标、纹理等）
- **BattleCache**: 用于战斗层资源（战斗预制体、特效、材质等）

#### Scenario: UI 资源加载
- **WHEN** 战略层代码请求加载 UI 资源
- **THEN** 系统使用 UICache 进行缓存和加载

#### Scenario: 战斗资源加载
- **WHEN** 战斗层代码请求加载战斗资源
- **THEN** 系统使用 BattleCache 进行缓存和加载

### Requirement: LFU 淘汰机制
系统 SHALL 实现 LFU 淘汰算法，当缓存超过限制时自动淘汰使用频率最低的资源。

#### Scenario: 缓存数量超限
- **GIVEN** 缓存数量达到上限
- **WHEN** 加载新资源
- **THEN** 系统淘汰使用频率最低的资源

#### Scenario: 缓存内存超限
- **GIVEN** 缓存内存占用达到上限
- **WHEN** 加载新资源
- **THEN** 系统淘汰使用频率最低的资源直到内存低于上限

#### Scenario: 频率相同处理
- **GIVEN** 多个资源使用频率相同且最低
- **WHEN** 需要淘汰资源
- **THEN** 淘汰其中最早加入缓存的资源（FIFO 策略作为二级排序）

### Requirement: 缓存配置
系统 SHALL 通过 `SystemConst` 提供缓存配置：

| 配置项 | UI 缓存 | 战斗缓存 |
|--------|---------|----------|
| 数量上限 | 200 | 100 |
| 内存上限 | 100MB | 50MB |

#### Scenario: 配置读取
- **WHEN** 缓存初始化
- **THEN** 从 `SystemConst.ResourceCache` 读取配置值

### Requirement: 缓存统计接口
系统 SHALL 提供缓存统计接口用于调试：
- 当前缓存数量
- 当前内存占用
- 缓存命中率
- 各资源访问频率

#### Scenario: 获取统计信息
- **WHEN** 调用统计接口
- **THEN** 返回当前缓存的详细统计信息

### Requirement: 战斗结束清理
系统 SHALL 在战斗结束时清空 BattleCache。

#### Scenario: 战斗结束
- **WHEN** 战斗结束
- **THEN** 清空 BattleCache 释放内存

## MODIFIED Requirements

### Requirement: 资源加载统一入口
原有 `Resources.Load` 调用 SHALL 替换为缓存调用：
- UI 层代码使用 `UICache.Load<T>()`
- 战斗层代码使用 `BattleCache.Load<T>()`
- 通用代码根据上下文选择合适的缓存
