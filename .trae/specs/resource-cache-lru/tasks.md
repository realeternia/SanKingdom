# Tasks

- [x] Task 1: 设计并实现 LFU 缓存核心类
  - [x] SubTask 1.1: 创建 `LFUCache<TKey, TValue>` 泛型类，支持 LFU 淘汰算法
  - [x] SubTask 1.2: 实现频率计数器 + 频率分组结构（Dictionary + LinkedList）
  - [x] SubTask 1.3: 实现 `Get`、`Add`、`Remove` 方法，维护访问频率
  - [x] SubTask 1.4: 频率相同时使用 FIFO 作为二级排序
  - [x] SubTask 1.5: 实现内存大小估算接口

- [x] Task 2: 重构 ResourceCache 为双层架构
  - [x] SubTask 2.1: 创建 `ResourceCacheInstance` 类，封装单个缓存实例
  - [x] SubTask 2.2: 实现 `UICache` 和 `BattleCache` 静态实例
  - [x] SubTask 2.3: 实现数量上限和内存上限检查
  - [x] SubTask 2.4: 实现 LFU 淘汰逻辑

- [x] Task 3: 添加缓存配置到 SystemConst
  - [x] SubTask 3.1: 在 `SystemConst` 中添加 `ResourceCache` 嵌套类
  - [x] SubTask 3.2: 定义 UI 缓存配置（数量上限 200，内存上限 100MB）
  - [x] SubTask 3.3: 定义战斗缓存配置（数量上限 100，内存上限 50MB）

- [x] Task 4: 替换战略层 UI 资源加载
  - [x] SubTask 4.1: 替换 `Panels/` 目录下的 Resources.Load 调用
  - [x] SubTask 4.2: 替换 `UIScripts/` 目录下的 Resources.Load 调用
  - [x] SubTask 4.3: 替换 `MainPanelManager.cs` 中的 Resources.Load 调用
  - [x] SubTask 4.4: 替换 `SideBar.cs` 中的 Resources.Load 调用
  - [x] SubTask 4.5: 替换 `Controls/PanelManager.cs` 中的 Resources.Load 调用

- [x] Task 5: 替换战斗层资源加载
  - [x] SubTask 5.1: 替换 `Combat/` 目录下的 Resources.Load 调用
  - [x] SubTask 5.2: 替换 `BattleHeroInfo.cs` 和 `BattleHeroInfoGroup.cs` 中的调用
  - [x] SubTask 5.3: 替换 `Controls/BattleManager.cs` 和 `BattleUIManager.cs` 中的调用

- [x] Task 6: 实现战斗结束清理机制
  - [x] SubTask 6.1: 在 `BattleManager` 战斗结束时调用 `BattleCache.Clear()`

- [x] Task 7: 添加缓存统计和调试接口
  - [x] SubTask 7.1: 实现 `GetStats()` 方法返回缓存统计信息
  - [x] SubTask 7.2: 添加缓存命中/未命中计数
  - [x] SubTask 7.3: 添加各资源访问频率统计

# Task Dependencies
- Task 2 depends on Task 1, Task 3
- Task 4 depends on Task 2
- Task 5 depends on Task 2
- Task 6 depends on Task 2
- Task 7 depends on Task 2
