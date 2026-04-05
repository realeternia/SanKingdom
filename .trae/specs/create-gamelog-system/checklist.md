# Checklist

## GameLog 核心功能
- [x] GameLog.cs 文件创建在 Controls/Utils 目录下
- [x] 支持 Debug、Info、Warn、Error 四种日志级别
- [x] 所有日志级别方法正确封装 UnityEngine.Debug 对应接口
- [x] 日志格式为 `[时间戳][级别][标签] 消息内容`

## 日志文件功能
- [x] 日志文件正确写入到文件系统
- [x] 日志文件按日期自动轮转，文件名包含日期
- [x] 日志目录正确创建

## 标签分类功能
- [x] SetTag 方法正确返回带标签的日志实例
- [x] 标签日志同时写入主日志文件和标签文件
- [x] 标签文件命名为 `log.{tag}`

## 日志迁移
- [x] AI.cs 中所有 UnityEngine.Debug 调用已迁移
- [x] BattleManager.cs 中所有 UnityEngine.Debug 调用已迁移
- [x] StrategicDecider.cs 中所有 UnityEngine.Debug 调用已迁移
- [x] GameManager.cs 中所有 UnityEngine.Debug 调用已迁移
- [x] HeroDispatcher.cs 中所有 UnityEngine.Debug 调用已迁移
- [x] 其他所有文件中的 UnityEngine.Debug 调用已迁移
- [x] 项目中不再有直接的 UnityEngine.Debug.Log/LogWarning/LogError 调用（注释除外）

## 代码质量
- [x] GameLog 类有完整的 XML 文档注释
- [x] 代码符合项目现有代码风格
