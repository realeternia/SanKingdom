# Hero ArmsId 集成规范

## Why
HeroConfig 不再包含 MoveSpeed、Range、MissileSpeed、MissileHight 和 Job 等战斗属性，这些属性已迁移到 ArmsConfig 中。Hero 和 Chess 需要关联一个 ArmsId 来获取这些战斗属性，与 BattleUnitConfig 的设计保持一致。

## What Changes
- SaveHeroData 添加 armsId 字段
- Chess 类添加 armsId 字段
- BattleCardData 添加 armsId 字段
- CreateChessAction 添加 armsId 字段
- Hero 初始化时默认赋值 armsId = 601（刀兵）
- Chess 从 ArmsConfig 获取战斗属性（MoveSpeed、Range、MissileSpeed、MissileHight）
- 删除 JobConfig 及相关代码

## Impact
- Affected specs: SaveHeroData 数据结构, Chess 类结构, BattleCardData 数据结构, CreateChessAction 数据结构
- Affected code: SaveHeroData.cs, Chess.cs, BattleCardData.cs, CreateChessAction.cs, BattleManager.cs, SaveCityData.cs, GameManager.cs, ConfigManager.cs, Tooltip.cs, SkillManager.cs

## ADDED Requirements
### Requirement: SaveHeroData ArmsId 存储
SaveHeroData 类 SHALL 包含 armsId 字段用于存储英雄的兵种配置ID。

#### Scenario: 新建英雄默认兵种
- **WHEN** 创建新的 SaveHeroData 实例
- **THEN** armsId 默认值为 601（刀兵）

### Requirement: Chess ArmsId 存储
Chess 类 SHALL 包含 armsId 字段用于存储战斗单位的兵种配置ID，供UI表现使用。

#### Scenario: Hero Chess 初始化
- **WHEN** 创建 Hero 类型的 Chess
- **THEN** armsId 从 BattleCardData 获取

#### Scenario: BattleUnit Chess 初始化
- **WHEN** 创建 BattleUnit 类型的 Chess
- **THEN** armsId 从 BattleUnitConfig.ArmsId 获取

### Requirement: BattleCardData ArmsId 存储
BattleCardData 类 SHALL 包含 armsId 字段用于传递英雄的兵种配置ID。

#### Scenario: 从 SaveHeroData 创建 BattleCardData
- **WHEN** 创建 BattleCardData
- **THEN** armsId 从 SaveHeroData.armsId 获取，若为0则默认601

### Requirement: CreateChessAction ArmsId 存储
CreateChessAction 类 SHALL 包含 armsId 字段并赋值给 Chess。

### Requirement: Chess 从 ArmsConfig 获取战斗属性
Chess 类 SHALL 从 ArmsConfig 获取 MoveSpeed、Range、MissileSpeed、MissileHight 等战斗属性。

#### Scenario: Hero Chess 获取战斗属性
- **WHEN** Hero Chess 初始化
- **THEN** 通过 armsId 从 ArmsConfig 获取 moveSpeed、attackRange、missileSpeed、missileHeight

## MODIFIED Requirements
### Requirement: SaveHeroData 数据结构
SaveHeroData 类新增 armsId 字段：
```csharp
public int armsId;  // 兵种配置ID，默认601（刀兵）
```

### Requirement: Chess 类结构
Chess 类新增 armsId 字段：
```csharp
public int armsId;  // 兵种配置ID，用于UI表现和战斗属性获取
```

### Requirement: BattleCardData 数据结构
BattleCardData 类新增 armsId 字段：
```csharp
public int ArmsId;  // 兵种配置ID
```

### Requirement: CreateChessAction 数据结构
CreateChessAction 类新增 armsId 字段，Hero 构造函数添加 armsId 参数。

### Requirement: Chess CreateChessView 方法
修改 CreateChessView 方法，Hero 分支从 ArmsConfig 获取战斗属性：
```csharp
if(heroId > 0)
{
    var heroConfig = HeroConfig.GetConfig(heroId);
    chessName = heroConfig.Icon;
    
    var armsConfig = ArmsConfig.GetConfig(armsId);
    hitEffect = armsConfig.HitEffect;
    missileSpeed = armsConfig.MissileSpeed;
    missileHeight = armsConfig.MissileHight;
    moveSpeed = armsConfig.MoveSpeed;
    attackRange = armsConfig.Range;
    atk = str;
    def = leadShip;
}
```

## REMOVED Requirements
### Requirement: JobConfig
**Reason**: Job 相关属性已迁移到 ArmsConfig，JobConfig 不再需要
**Migration**: 
- 删除 JobConfig_s.cs 文件
- 移除 ConfigManager 中 JobConfig 相关代码
- 移除 Chess.Attack 中兵种克制逻辑
- 移除 Tooltip 中 Job 克制显示
- 移除 SkillManager 中 Job 检查逻辑
