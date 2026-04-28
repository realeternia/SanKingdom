# ArmsConfig 兵种攻防与 HeroConfig 兵种驾驭能力规范

## Why

当前战斗伤害计算仅使用英雄的武力(str)作为攻击力、统帅(leadShip)作为防御力，未考虑兵种本身的攻防属性，也未体现英雄对不同兵种的驾驭能力差异。需要为 ArmsConfig 增加攻防属性和兵种类型，为 HeroConfig 增加兵种驾驭能力属性，并在战斗伤害计算中综合体现这些因素。

## What Changes

- 在 PO 目录下新增 `ArmsType` 枚举（SodWalk, SodHorse, SodBow, SodWater, SodTank）
- ArmsConfig 在 `NameS` 后、`MoveSpeed` 前增加 `Type`（ArmsType 枚举）、`Atk`（int）、`Def`（int）三个字段
- HeroConfig 在 `Charm` 后增加 `SodWalk`、`SodHorse`、`SodBow`、`SodWater`、`SodTank` 五个 int 字段（1-10，10最强）
- 填充所有 ArmsConfig 数据的 Type、Atk、Def 值
- 填充所有 HeroConfig 数据的 SodXxx 值
- Chess 攻击力计算时加上兵种的 Atk，防御力计算时加上兵种的 Def
- Chess 攻击时根据英雄对应兵种类型的 SodXxx 属性，增加攻方 1%-30% 的伤害加成
- 在 SystemConst.Battle 中增加兵种驾驭加成相关常量
- 在 SysFormula.Battle 中增加兵种驾驭加成计算公式

## Impact

- Affected specs: BattleDamageCalculation（伤害计算公式需扩展）
- Affected code: ArmsType.cs(新增), ArmsConfig_s.cs, HeroConfig_s.cs, Chess.cs, SysFormula.cs, SystemConst.cs, Assembly-CSharp.csproj

## ADDED Requirements

### Requirement: ArmsType 枚举

在 PO 目录下新增 `ArmsType` 枚举，定义五种兵种类型，与 HeroConfig 的 SodXxx 属性一一对应：

```csharp
public enum ArmsType
{
    SodWalk = 0,
    SodHorse = 1,
    SodBow = 2,
    SodWater = 3,
    SodTank = 4
}
```

#### Scenario: 枚举值与 SodXxx 对应
- **WHEN** ArmsType 为 SodWalk
- **THEN** 对应 HeroConfig.SodWalk 属性

### Requirement: ArmsConfig Type 字段

ArmsConfig 类 SHALL 包含 `Type` 字段（ArmsType 枚举类型），表示该兵种对应的英雄驾驭属性类型。

#### Scenario: 获取兵种类型
- **WHEN** 查询 ArmsConfig 的 Type 字段
- **THEN** 返回 ArmsType 枚举值

### Requirement: ArmsConfig Atk 字段

ArmsConfig 类 SHALL 包含 `Atk` 字段（int 类型），表示该兵种的攻击力加成。

#### Scenario: 获取兵种攻击力
- **WHEN** 查询 ArmsConfig 的 Atk 字段
- **THEN** 返回该兵种的攻击力加成值

### Requirement: ArmsConfig Def 字段

ArmsConfig 类 SHALL 包含 `Def` 字段（int 类型），表示该兵种的防御力加成。

#### Scenario: 获取兵种防御力
- **WHEN** 查询 ArmsConfig 的 Def 字段
- **THEN** 返回该兵种的防御力加成值

### Requirement: HeroConfig SodXxx 字段

HeroConfig 类 SHALL 包含 `SodWalk`、`SodHorse`、`SodBow`、`SodWater`、`SodTank` 五个 int 字段，表示英雄对各兵种的驾驭能力，取值范围 1-10，10 为最强。

#### Scenario: 获取英雄兵种驾驭能力
- **WHEN** 查询 HeroConfig 的 SodWalk/SodHorse/SodBow/SodWater/SodTank 字段
- **THEN** 返回 1-10 之间的整数

### Requirement: 兵种攻防参与战斗计算

Chess 的攻击力 SHALL 在英雄原有属性基础上加上兵种的 Atk，防御力 SHALL 在英雄原有属性基础上加上兵种的 Def。

#### Scenario: Hero Chess 攻击力计算
- **WHEN** Hero Chess 初始化
- **THEN** atk = str + ArmsConfig.Atk

#### Scenario: Hero Chess 防御力计算
- **WHEN** Hero Chess 初始化
- **THEN** def = leadShip + ArmsConfig.Def

### Requirement: 兵种驾驭加成

Chess 攻击时 SHALL 根据攻击方英雄对应兵种类型的 SodXxx 属性，增加攻方 1%-30% 的伤害加成。SodXxx 值为 1 时加成 1%，为 10 时加成 30%，线性映射。

#### Scenario: 英雄驾驭加成计算
- **WHEN** 英雄攻击时
- **THEN** 根据 ArmsConfig.Type 获取对应的 HeroConfig.SodXxx 值
- **THEN** 加成率 = SodXxx * 3%（即 SodXxx=1 时 3%，SodXxx=10 时 30%）

#### Scenario: 非英雄单位
- **WHEN** 非 Hero 的 Chess 攻击时
- **THEN** 不应用兵种驾驭加成

## MODIFIED Requirements

### Requirement: ArmsConfig 数据结构

ArmsConfig 字段顺序变更为：Id, Name, NameS, Type, Atk, Def, MoveSpeed, Range, MissileSpeed, MissileHight, HitEffect, Model, ModelCountFactor, OvercomeStrong, OvercomeWeak, HorseCost, SteelCost, WoodCost, StoneCost

构造函数参数同步调整。

### Requirement: HeroConfig 数据结构

HeroConfig 字段在 Charm 后增加 SodWalk, SodHorse, SodBow, SodWater, SodTank，Total 字段重新计算为 LeadShip+Str+Inte+Fair+Charm+SodWalk+SodHorse+SodBow+SodWater+SodTank。

构造函数参数同步调整。

### Requirement: Chess.CreateChessView 方法

Hero 分支的攻击力/防御力计算修改为：

```csharp
if(heroId > 0)
{
    var heroConfig = HeroConfig.GetConfig(heroId);
    chessName = heroConfig.Icon;
    var armsConfig = ArmsConfig.GetConfig(armsId);
    atk = str + armsConfig.Atk;
    def = leadShip + armsConfig.Def;
}
```

### Requirement: 战斗伤害计算

CalculateDamage 方法增加兵种驾驭加成参数，攻击方伤害乘以 (1 + 驾驭加成率)：

```csharp
private static int CalculateDamage(Chess attacker, Chess defender)
{
    float sodBonus = 0f;
    if (attacker.isHero && attacker.heroId > 0)
    {
        var heroConfig = HeroConfig.GetConfig(attacker.heroId);
        var armsConfig = ArmsConfig.GetConfig(attacker.armsId);
        sodBonus = SysFormula.Battle.CalculateSodBonus(heroConfig, armsConfig.Type);
    }
    int baseDamage = SysFormula.Battle.CalculateDamage(attacker.atk, attacker.hp, defender.def);
    return (int)(baseDamage * (1f + sodBonus));
}
```

其中 `CalculateSodBonus` 方法根据 ArmsType 枚举值获取对应 SodXxx 属性：

```csharp
public static float CalculateSodBonus(HeroConfig heroConfig, ArmsType armsType)
{
    int sodValue = armsType switch
    {
        ArmsType.SodWalk => heroConfig.SodWalk,
        ArmsType.SodHorse => heroConfig.SodHorse,
        ArmsType.SodBow => heroConfig.SodBow,
        ArmsType.SodWater => heroConfig.SodWater,
        ArmsType.SodTank => heroConfig.SodTank,
        _ => 1
    };
    float bonus = sodValue * SystemConst.Battle.SOD_BONUS_RATE_PER_POINT;
    return Math.Clamp(bonus, SystemConst.Battle.SOD_BONUS_MIN, SystemConst.Battle.SOD_BONUS_MAX);
}
```

## REMOVED Requirements

无移除的需求。

## ArmsConfig 数据设计

| Id | Name | NameS | Type | Atk | Def |
|----|------|-------|------|-----|-----|
| 101 | ma | 马 | ArmsType.SodHorse | 15 | 10 |
| 102 | che | 车 | ArmsType.SodTank | 20 | 25 |
| 201 | gong | 弓 | ArmsType.SodBow | 18 | 5 |
| 202 | pao | 炮 | ArmsType.SodTank | 25 | 8 |
| 601 | dao | 刀 | ArmsType.SodWalk | 10 | 10 |
| 602 | daoqiang | 枪 | ArmsType.SodWalk | 12 | 12 |
| 603 | daoji | 戟 | ArmsType.SodWalk | 14 | 14 |
| 701 | shan | 扇 | ArmsType.SodWalk | 5 | 8 |
| 702 | mou | 谋 | ArmsType.SodWalk | 3 | 5 |

## HeroConfig SodXxx 数据设计原则

### SodWalk（步兵驾驭）
- 近战武将（str≥80）：7-10
- 中等武将（str 60-79）：5-7
- 高统帅谋士（leadShip≥85，如司马懿、诸葛亮）：6-8
- 一般谋士文臣（inte≥70, leadShip<85）：3-5
- 低属性角色：1-3

### SodHorse（骑兵驾驭）
- 骑兵名将（如马超、张辽、夏侯惇）：8-10
- 西凉武将（forceId=6 或西凉系）：8-10
- 一般武将：4-6
- 高统帅谋士（leadShip≥85）：4-6
- 文臣（inte≥70, leadShip<85）：2-4
- 低属性角色：1-2

### SodBow（弓兵驾驭）
- 弓箭名将（如黄忠、太史慈、夏侯渊）：8-10
- 善射武将（爱好含"射箭"或"射猎"）：6-8
- 一般武将：3-5
- 高统帅谋士（leadShip≥85）：3-5
- 文臣：2-3
- 低属性角色：1-2

### SodWater（水军驾驭）
- 东吴将领（forceId=3）：7-10
- 荆州水将（forceId=7 且与水战相关）：5-7
- 蔡瑁、张允等水战专长：7-9
- 北方/西凉武将：2-4
- 高统帅谋士（leadShip≥85）：3-5
- 一般文臣：1-3

### SodTank（车炮驾驭）
- 发明家（如诸葛亮、黄月英、刘晔，爱好含"发明"）：7-9
- 攻城专家（如郝昭、高顺）：6-8
- 高统帅谋士（leadShip≥85，如司马懿、曹操）：5-7
- 一般武将：3-5
- 一般文臣：2-4
- 低属性角色：1-2

### 特殊角色示例
| 英雄 | SodWalk | SodHorse | SodBow | SodWater | SodTank | 说明 |
|------|---------|----------|--------|----------|---------|------|
| 诸葛亮 | 7 | 5 | 4 | 3 | 9 | 高统帅+发明家，SodTank极高 |
| 司马懿 | 7 | 5 | 4 | 3 | 7 | 高统帅谋士，SodTank较高 |
| 黄月英 | 3 | 2 | 3 | 2 | 8 | 发明家，SodTank高 |
| 刘晔 | 3 | 2 | 4 | 2 | 8 | 发明家（投石车），SodTank高 |
| 曹操 | 8 | 7 | 5 | 3 | 6 | 高统帅君主，全面较高 |
| 刘备 | 7 | 5 | 3 | 2 | 4 | 君主，步兵和骑兵尚可 |
| 吕布 | 9 | 9 | 5 | 1 | 3 | 纯武力型，步骑极强 |
| 马超 | 8 | 10 | 4 | 1 | 2 | 西凉骑兵，SodHorse满 |
| 甘宁 | 7 | 5 | 7 | 8 | 3 | 东吴水战+弓兵 |
| 蔡瑁 | 5 | 4 | 4 | 9 | 3 | 水战专长 |
| 高顺 | 8 | 4 | 3 | 2 | 7 | 攻城专家（陷阵营） |
| 郝昭 | 8 | 4 | 4 | 2 | 8 | 守城/攻城专家 |
