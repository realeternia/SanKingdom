# Checklist

## 核心实现
- [x] LFUCache 类正确实现频率计数器 + 频率分组结构
- [x] LFU 淘汰算法在数量超限时正确工作
- [x] LFU 淘汰算法在内存超限时正确工作
- [x] 频率相同时使用 FIFO 作为二级排序
- [x] 内存大小估算接口正确实现

## 双层架构
- [x] UICache 和 BattleCache 静态实例正确创建
- [x] UICache 配置：数量上限 200，内存上限 100MB
- [x] BattleCache 配置：数量上限 100，内存上限 50MB
- [x] 两个缓存实例相互独立，不共享数据

## 资源替换
- [x] 所有 Panels/ 目录下的 Resources.Load 已替换
- [x] 所有 UIScripts/ 目录下的 Resources.Load 已替换
- [x] 所有 Combat/ 目录下的 Resources.Load 已替换
- [x] MainPanelManager.cs 中的 Resources.Load 已替换
- [x] SideBar.cs 中的 Resources.Load 已替换
- [x] Controls/PanelManager.cs 中的 Resources.Load 已替换
- [x] Controls/BattleManager.cs 中的 Resources.Load 已替换
- [x] Controls/BattleUIManager.cs 中的 Resources.Load 已替换
- [x] SystemTool/BGMPlayer.cs 中的 Resources.Load 已替换

## 清理机制
- [x] 战斗结束时 BattleCache.Clear() 被正确调用
- [x] 战斗结束后内存正确释放

## 统计接口
- [x] GetStats() 方法返回正确的缓存统计信息
- [x] 缓存命中/未命中计数正确累加
- [x] 各资源访问频率统计正确

## 编译验证
- [x] 项目编译无错误
- [x] 无新增编译警告
