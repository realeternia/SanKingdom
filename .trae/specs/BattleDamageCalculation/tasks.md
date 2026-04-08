# 战斗伤害计算重构任务

## 任务列表

### 1. 修改攻击力计算方式
- **文件**: `Chess.cs`
- **位置**: `Init()` 方法，约 line 151
- **当前代码**: `attackDamage = leadShip / 3;`
- **新代码**: `attackDamage = str / 3;`
- **说明**: 攻击力改为由武力(Str)决定，移除兵种攻击加成

### 2. 重写伤害计算公式
- **文件**: `Chess.cs`
- **位置**: `CalculateDamage()` 静态方法，约 line 645-677
- **改动**:
  - 攻击力 = 武力(Str) + 兵力/50
  - 防御力 = 统帅(LeadShip)
  - 基础伤害 = 30 + (攻击力 - 防御力) / 2
- **返回值范围**: 约 10-60（取决于攻防差）

### 3. 调整伤害clamp范围
- **文件**: `Chess.cs`
- **位置**: `Attack()` 方法，约 line 564-565
- **当前代码**:
  ```csharp
  var minDamage = 10 + level / 2;
  var maxDamage = 50 + level;
  ```
- **新代码**:
  ```csharp
  var minDamage = 10;
  var maxDamage = 60;
  ```
- **说明**: 简化伤害区间，便于计算和平衡调整

### 4. 代码验证清单
- [ ] 编译检查无语法错误
- [ ] 确认 `str` 变量在 `CalculateDamage` 中可用
- [ ] 确认 `hp` 代表当前兵力/生命值
- [ ] 确认 `leadShip` 代表统帅属性
