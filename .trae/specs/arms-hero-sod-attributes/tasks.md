# Tasks

- [x] Task 1: 在 PO 目录下新增 ArmsType 枚举
  - [x] SubTask 1.1: 创建 Assets/Resources/Scripts/PO/ArmsType.cs，定义 ArmsType 枚举（SodWalk=0, SodHorse=1, SodBow=2, SodWater=3, SodTank=4）
  - [x] SubTask 1.2: 在 Assembly-CSharp.csproj 中添加 Compile Include 条目

- [x] Task 2: 修改 ArmsConfig_s.cs 结构
  - [x] SubTask 2.1: 在 fieldMeta 中 NameS 后添加 Type(ArmsType)、Atk(int)、Def(int) 三个元信息
  - [x] SubTask 2.2: 在字段声明中 NameS 后添加 Type(ArmsType)、Atk(int)、Def(int)
  - [x] SubTask 2.3: 修改构造函数，在 NameS 后添加 Type(ArmsType)、Atk(int)、Def(int) 三个参数
  - [x] SubTask 2.4: 更新所有 ArmsConfig.Load() 中的数据行，填充 Type(ArmsType枚举值)、Atk、Def 值

- [x] Task 3: 修改 HeroConfig_s.cs 结构
  - [x] SubTask 3.1: 在 fieldMeta 中 Charm 后添加 SodWalk、SodHorse、SodBow、SodWater、SodTank 五个元信息
  - [x] SubTask 3.2: 在字段声明中 Charm 后添加 SodWalk(int)、SodHorse(int)、SodBow(int)、SodWater(int)、SodTank(int)
  - [x] SubTask 3.3: 修改构造函数，在 Charm 后添加五个 SodXxx 参数
  - [x] SubTask 3.4: 更新所有 HeroConfig.Load() 中的数据行，填充 SodXxx 值（1-10）
  - [x] SubTask 3.5: 更新 Total 字段计算（包含 SodXxx 五个属性之和）

- [x] Task 4: 在 SystemConst.Battle 中添加兵种驾驭加成常量
  - [x] SubTask 4.1: 添加 SOD_BONUS_RATE_PER_POINT = 0.03f（每点驾驭能力3%加成）
  - [x] SubTask 4.2: 添加 SOD_BONUS_MIN = 0.01f（最低1%加成）
  - [x] SubTask 4.3: 添加 SOD_BONUS_MAX = 0.30f（最高30%加成）

- [x] Task 5: 在 SysFormula.Battle 中添加兵种驾驭加成计算公式
  - [x] SubTask 5.1: 添加 CalculateSodBonus(HeroConfig heroConfig, ArmsType armsType) 方法
  - [x] SubTask 5.2: 根据 armsType 枚举值用 switch 获取对应 SodXxx 值，计算加成率 = SodXxx * SOD_BONUS_RATE_PER_POINT，clamp 到 [SOD_BONUS_MIN, SOD_BONUS_MAX]

- [x] Task 6: 修改 Chess.cs 攻击力/防御力计算
  - [x] SubTask 6.1: 修改 CreateChessView 中 Hero 分支，atk = str + armsConfig.Atk，def = leadShip + armsConfig.Def
  - [x] SubTask 6.2: 修改 CalculateDamage 方法，增加兵种驾驭加成计算

# Task Dependencies
- Task 1 需最先完成（其他任务依赖 ArmsType 枚举）
- Task 2 和 Task 3 可并行执行（均依赖 Task 1）
- Task 4 和 Task 5 可并行执行（Task 5 依赖 Task 1 和 Task 3）
- Task 6 依赖 Task 2、Task 3、Task 5
