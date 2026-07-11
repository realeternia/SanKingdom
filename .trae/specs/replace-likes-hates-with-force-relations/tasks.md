# Tasks

- [x] Task 1: 修改 HeroConfig_s.cs 字段定义和元数据
  - [x] SubTask 1.1: 将 fieldMeta 中 "Likes" → "LikeForces"（中文标签"喜爱势力"），"Hates" → "HateForces"（中文标签"厌恶势力"）
  - [x] SubTask 1.2: 将公共字段 `string[] Likes` → `string[] LikeForces`，`string[] Hates` → `string[] HateForces`，更新注释
  - [x] SubTask 1.3: 更新构造函数参数名和赋值
  - [x] SubTask 1.4: 更新 Load() 中所有英雄数据，将 Likes/Hates 从英雄名数组转换为 forceId;degree 格式

- [x] Task 2: 修改 SystemConst.cs 关系加成常量
  - [x] SubTask 2.1: 替换 RECRUIT_LIKE_EXECUTOR_BONUS / RECRUIT_LIKE_KING_BONUS 为 RECRUIT_LIKE_BONUS_PER_DEGREE = 5（每级5%）
  - [x] SubTask 2.2: 替换 RECRUIT_HATE_EXECUTOR_PENALTY / RECRUIT_HATE_KING_PENALTY 为 RECRUIT_HATE_PENALTY_PER_DEGREE = -8（每级-8%）
  - [x] SubTask 2.3: 删除旧的4个常量

- [x] Task 3: 修改 SysFormula.cs 关系加成计算逻辑
  - [x] SubTask 3.1: 删除 ContainsName 方法，新增 GetForceDegree 方法（解析 forceId;degree，返回 degree，0 表示未找到）
  - [x] SubTask 3.2: 重写 GetRelationBonusPercent 方法，使用 executorConfig.ForceId 匹配 LikeForces/HateForces，按 degree * PER_DEGREE 常量计算加成（executor和king取较大degree）

- [x] Task 4: 修改 HeroInfoPanelManager.cs UI 展示
  - [x] SubTask 4.1: 将 likesText/hatesText 展示逻辑改为遍历 LikeForces/HateForces，解析 forceId 获取 ForceConfig.Cname + "(degree)" 显示

# Task Dependencies
- [Task 2] depends on [Task 1]（常量引用需与字段变更一起理解）
- [Task 3] depends on [Task 1] and [Task 2]（公式逻辑依赖新字段和常量）
- [Task 4] depends on [Task 1]（UI 依赖新字段）
