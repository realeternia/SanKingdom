using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class BattleSkillConfig
    {
        public class FieldMetaInfo
        {
            public string fieldName;
            public string fieldType;
            public FieldMetaInfo(string name, string type)
            {
                fieldName = name;
                fieldType = type;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int")},
            {"Name", new FieldMetaInfo("名字", "string")},
            {"Sname", new FieldMetaInfo("缩写", "string")},
            {"Descript", new FieldMetaInfo("说明", "string")},
            {"Type", new FieldMetaInfo("分类", "string")},
            {"Lv", new FieldMetaInfo("等级", "int")},
            {"Rate", new FieldMetaInfo("发动概率", "float")},
            {"CD", new FieldMetaInfo("发动cd", "float")},
            {"AttackPointReduce", new FieldMetaInfo("攻击时间惩罚", "float")},
            {"ConditionParm", new FieldMetaInfo("条件参数", "float")},
            {"Attr", new FieldMetaInfo("相关属性", "string")},
            {"CheckAttrs", new FieldMetaInfo("判定属性", "string[]")},
            {"Range", new FieldMetaInfo("范围", "float")},
            {"RangeOut", new FieldMetaInfo("范围外", "bool")},
            {"TargetType", new FieldMetaInfo("选取点", "string")},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int")},
            {"Strength", new FieldMetaInfo("技能强度（恒定）", "float")},
            {"StrengthInt", new FieldMetaInfo("技能强度（恒定）", "int")},
            {"SkillAttrRate", new FieldMetaInfo("技能数值比例", "float")},
            {"SkillDamageRate", new FieldMetaInfo("技能强度伤害比例", "float")},
            {"SkillDamageAttrRate", new FieldMetaInfo("技能强度属性比例", "float")},
            {"DoCount", new FieldMetaInfo("计数次数", "int")},
            {"TimeDelay", new FieldMetaInfo("计数延迟", "float")},
            {"UnitHelpType", new FieldMetaInfo("效果范围(1横向，2纵向)", "int")},
            {"HelpSkill", new FieldMetaInfo("光环技能", "string")},
            {"HelpSkillJob", new FieldMetaInfo("职业限定", "string")},
            {"BuffId", new FieldMetaInfo("BuffId", "int")},
            {"NegBuff", new FieldMetaInfo("是否针对负面buff", "bool")},
            {"BuffTime", new FieldMetaInfo("Buff持续", "float")},
            {"SummonTag", new FieldMetaInfo("召唤物标签", "string")},
            {"SummonCount", new FieldMetaInfo("技能场数", "int")},
            {"SummonArea", new FieldMetaInfo("技能场范围", "float")},
            {"SummonTime", new FieldMetaInfo("技能场持续", "float")},
            {"SummonHitInterval", new FieldMetaInfo("技能场间隔", "float")},
            {"SummonSpeed", new FieldMetaInfo("技能场速度", "float")},
            {"ScriptName", new FieldMetaInfo("脚本名", "string")},
            {"Action", new FieldMetaInfo("动作", "string")},
            {"EffectSelf", new FieldMetaInfo("自己", "string")},
            {"EffectArea", new FieldMetaInfo("区域", "string")},
            {"EffectHit", new FieldMetaInfo("hit", "string")},
            {"EffectSize", new FieldMetaInfo("size", "float")},
            {"Price", new FieldMetaInfo("价值", "int")},
            {"Icon", new FieldMetaInfo("图标", "string")},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///缩写
        /// </summary>
        public string Sname;
        /// <summary>
        ///说明
        /// </summary>
        public string Descript;
        /// <summary>
        ///分类
        /// </summary>
        public string Type;
        /// <summary>
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///发动概率
        /// </summary>
        public float Rate;
        /// <summary>
        ///发动cd
        /// </summary>
        public float CD;
        /// <summary>
        ///攻击时间惩罚
        /// </summary>
        public float AttackPointReduce;
        /// <summary>
        ///条件参数
        /// </summary>
        public float ConditionParm;
        /// <summary>
        ///相关属性
        /// </summary>
        public string Attr;
        /// <summary>
        ///判定属性
        /// </summary>
        public string[] CheckAttrs;
        /// <summary>
        ///范围
        /// </summary>
        public float Range;
        /// <summary>
        ///范围外
        /// </summary>
        public bool RangeOut;
        /// <summary>
        ///选取点
        /// </summary>
        public string TargetType;
        /// <summary>
        ///最大目标数
        /// </summary>
        public int TargetCount;
        /// <summary>
        ///技能强度（恒定）
        /// </summary>
        public float Strength;
        /// <summary>
        ///技能强度（恒定）
        /// </summary>
        public int StrengthInt;
        /// <summary>
        ///技能数值比例
        /// </summary>
        public float SkillAttrRate;
        /// <summary>
        ///技能强度伤害比例
        /// </summary>
        public float SkillDamageRate;
        /// <summary>
        ///技能强度属性比例
        /// </summary>
        public float SkillDamageAttrRate;
        /// <summary>
        ///计数次数
        /// </summary>
        public int DoCount;
        /// <summary>
        ///计数延迟
        /// </summary>
        public float TimeDelay;
        /// <summary>
        ///效果范围(1横向，2纵向)
        /// </summary>
        public int UnitHelpType;
        /// <summary>
        ///光环技能
        /// </summary>
        public string HelpSkill;
        /// <summary>
        ///职业限定
        /// </summary>
        public string HelpSkillJob;
        /// <summary>
        ///BuffId
        /// </summary>
        public int BuffId;
        /// <summary>
        ///是否针对负面buff
        /// </summary>
        public bool NegBuff;
        /// <summary>
        ///Buff持续
        /// </summary>
        public float BuffTime;
        /// <summary>
        ///召唤物标签
        /// </summary>
        public string SummonTag;
        /// <summary>
        ///技能场数
        /// </summary>
        public int SummonCount;
        /// <summary>
        ///技能场范围
        /// </summary>
        public float SummonArea;
        /// <summary>
        ///技能场持续
        /// </summary>
        public float SummonTime;
        /// <summary>
        ///技能场间隔
        /// </summary>
        public float SummonHitInterval;
        /// <summary>
        ///技能场速度
        /// </summary>
        public float SummonSpeed;
        /// <summary>
        ///脚本名
        /// </summary>
        public string ScriptName;
        /// <summary>
        ///动作
        /// </summary>
        public string Action;
        /// <summary>
        ///自己
        /// </summary>
        public string EffectSelf;
        /// <summary>
        ///区域
        /// </summary>
        public string EffectArea;
        /// <summary>
        ///hit
        /// </summary>
        public string EffectHit;
        /// <summary>
        ///size
        /// </summary>
        public float EffectSize;
        /// <summary>
        ///价值
        /// </summary>
        public int Price;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;


        public BattleSkillConfig(int Id, string Name, string Sname, string Descript, string Type, int Lv, float Rate, float CD, float AttackPointReduce, float ConditionParm, string Attr, string[] CheckAttrs, float Range, bool RangeOut, string TargetType, int TargetCount, float Strength, int StrengthInt, float SkillAttrRate, float SkillDamageRate, float SkillDamageAttrRate, int DoCount, float TimeDelay, int UnitHelpType, string HelpSkill, string HelpSkillJob, int BuffId, bool NegBuff, float BuffTime, string SummonTag, int SummonCount, float SummonArea, float SummonTime, float SummonHitInterval, float SummonSpeed, string ScriptName, string Action, string EffectSelf, string EffectArea, string EffectHit, float EffectSize, int Price, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.Type = Type;
            this.Lv = Lv;
            this.Rate = Rate;
            this.CD = CD;
            this.AttackPointReduce = AttackPointReduce;
            this.ConditionParm = ConditionParm;
            this.Attr = Attr;
            this.CheckAttrs = CheckAttrs;
            this.Range = Range;
            this.RangeOut = RangeOut;
            this.TargetType = TargetType;
            this.TargetCount = TargetCount;
            this.Strength = Strength;
            this.StrengthInt = StrengthInt;
            this.SkillAttrRate = SkillAttrRate;
            this.SkillDamageRate = SkillDamageRate;
            this.SkillDamageAttrRate = SkillDamageAttrRate;
            this.DoCount = DoCount;
            this.TimeDelay = TimeDelay;
            this.UnitHelpType = UnitHelpType;
            this.HelpSkill = HelpSkill;
            this.HelpSkillJob = HelpSkillJob;
            this.BuffId = BuffId;
            this.NegBuff = NegBuff;
            this.BuffTime = BuffTime;
            this.SummonTag = SummonTag;
            this.SummonCount = SummonCount;
            this.SummonArea = SummonArea;
            this.SummonTime = SummonTime;
            this.SummonHitInterval = SummonHitInterval;
            this.SummonSpeed = SummonSpeed;
            this.ScriptName = ScriptName;
            this.Action = Action;
            this.EffectSelf = EffectSelf;
            this.EffectArea = EffectArea;
            this.EffectHit = EffectHit;
            this.EffectSize = EffectSize;
            this.Price = Price;
            this.Icon = Icon;

        }

        public BattleSkillConfig() { }

        private static Dictionary<int, BattleSkillConfig> config = new Dictionary<int, BattleSkillConfig>();
        public static Dictionary<int, BattleSkillConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, BattleSkillConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[200001] = new BattleSkillConfig(200001, "王", "帅", "给与我方同阵营单位17%生命值护盾", "职业", 1, 0, 0, 0, 0, "", null, 0, false, "", 0, 0, 0, 0.17f, 0, 0, 0, 0, 0, "", "", 300001, false, 999f, "", 0, 0, 0, 0, 0, "InitMasterShield", "", "", "", "", 0, 4, "shuai");
            config[200002] = new BattleSkillConfig(200002, "羽扇", "扇", "击中目标时触发弹射", "职业", 1, 0, 0, 0, 0, "inte", null, 30f, false, "", 1, 0, 0, 0, 0.15f, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackReboundArrow", "sway", "", "", "", 0, 2, "shan");
            config[200003] = new BattleSkillConfig(200003, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 1, 0.15f, 5f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 10, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackAddDamage", "sway", "", "", "SwordHitRedCritical", 0, 1, "dao");
            config[200004] = new BattleSkillConfig(200004, "坚韧", "士", "受击时几率发动减伤", "职业", 1, 0.35f, 7f, 0, 0, "str", null, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 300002, false, 4.5f, "", 0, 0, 0, 0, 0, "AttackedBuff", "spin", "", "", "", 0, 3, "shi");
            config[200005] = new BattleSkillConfig(200005, "突破", "马", "移动时穿越敌人,造成额外伤害", "职业", 1, 1f, 7f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0.5f, 0, 0, 0, 0, "", "", 301003, false, 0f, "", 0, 0, 0, 0, 0, "AttackRunCross", "", "", "", "LightningExplosionBlue", 0, 2, "ma");
            config[200006] = new BattleSkillConfig(200006, "运筹", "相", "提升士兵等级", "职业", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 0, 0.25f, 0.05f, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitSoldierUp", "", "", "", "MagicChargeYellow", 0, 2, "xiang");
            config[200007] = new BattleSkillConfig(200007, "弓手", "弓", "远程射击单位", "职业", 1, 1f, 99f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "Dumb", "", "", "", "", 0, 2, "gong");
            config[200008] = new BattleSkillConfig(200008, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4f, 0, 0, "inte", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 301001, false, 2f, "", 0, 0, 0, 0, 0, "HitBuff", "throw", "", "", "MagicChargeYellow", 0, 2, "mou");
            config[200009] = new BattleSkillConfig(200009, "炮车", "炮", "攻击目标发生爆炸", "职业", 1, 0.5f, 0, 0, 0, "leadShip", null, 20f, false, "", 2, 0, 0, 0, 0.6f, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HitArea", "throw", "", "", "MagicNovaYellow", 0, 3, "pao");
            config[200010] = new BattleSkillConfig(200010, "弩手", "弩", "射程非常远", "职业", 1, 1f, 99f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "Dumb", "", "", "", "", 0, 3, "nu");
            config[200011] = new BattleSkillConfig(200011, "冲锋", "车", "移动时穿越敌人,降低目标防御", "职业", 1, 1f, 7f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 301003, false, 3f, "", 0, 0, 0, 0, 0, "AttackRunCrossPlus", "", "LightningExplosionRed", "", "", 0, 2, "che");
            config[200012] = new BattleSkillConfig(200012, "声乐", "乐", "给与友军攻速祝福", "职业", 1, 1f, 3.5f, 1f, 0, "inte", null, 50f, false, "", 0, 0.45f, 0, 0, 0, 0, 0, 0, 0, "", "", 300005, false, 5f, "", 0, 0, 0, 0, 0, "HelpAidBuff", "sway", "", "", "MagicChargePink", 0, 2, "song");
            config[200013] = new BattleSkillConfig(200013, "治疗", "医", "给与友军治疗", "职业", 1, 1f, 3f, 1f, 0, "inte", null, 50f, false, "", 0, 0, 0, 0.7f, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HelpAidHeal", "sway", "", "", "MagicBuffGreen", 0, 2, "heal");
            config[200014] = new BattleSkillConfig(200014, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 301001, false, 2f, "", 0, 0, 0, 0, 0, "HitBuff", "spin", "", "", "MagicChargeYellow", 0, 2, "qiang");
            config[200015] = new BattleSkillConfig(200015, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 1, 0.35f, 4f, 0, 0, "leadShip", null, 25f, false, "", 2, 0, 0, 0, 0.6f, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HitAround", "throw", "", "", "SwordSlashMiniWhite", 0, 2, "ji");
            config[200016] = new BattleSkillConfig(200016, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 1, 1f, 3.5f, 1f, 0, "inte", null, 50f, false, "", 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "", "", 300004, false, 5f, "", 0, 0, 0, 0, 0, "HelpAidBuff", "sway", "", "", "MagicChargeYellow", 0, 2, "gu");
            config[201002] = new BattleSkillConfig(201002, "铁骑", "铁", "自己和同行[马车]技能附带混乱效果", "攻击up", 1, 0, 0, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 1, "铁", "马车", 301001, false, 2f, "", 0, 0, 0, 0, 0, "BuffTieqi", "", "", "", "", 0, 2, "tie");
            config[201003] = new BattleSkillConfig(201003, "奇袭", "奇", "自己和同行队友统帅技能造成的混乱时间增加50%", "攻击up", 1, 0, 0, 0, 0, "leadShip", new string[]{"leadShip"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 1, "奇", "", 301001, true, 0, "", 0, 0, 0, 0, 0, "ModifyBuffTime", "", "", "", "", 0, 2, "qi");
            config[201004] = new BattleSkillConfig(201004, "瓦解小", "透", "提升20%暴击率", "攻击up", 1, 0, 0, 0, 0, "str", null, 0, false, "", 0, 0.2f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAddCrit", "", "", "", "", 0, 2, "jie3");
            config[201005] = new BattleSkillConfig(201005, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 1, 0, 0, 0, 0, "str", null, 0, false, "", 0, 0.2f, 0, 0, 0, 0, 0, 0, 1, "透", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAddCrit", "", "", "", "", 0, 4, "jie");
            config[201006] = new BattleSkillConfig(201006, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 1, 0.3f, 5f, 0, 0, "leadShip", null, 0, false, "", 0, 0.7f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackSpeedAttack", "flipspin", "", "", "", 0, 2, "lian");
            config[201007] = new BattleSkillConfig(201007, "速射", "速", "自己和同行[弓弩]箭矢飞行速度提升", "攻击up", 1, 0, 0, 0, 0, "leadShip", null, 0, false, "", 0, 2.5f, 0, 0, 0, 0, 0, 0, 1, "速", "弓弩", 0, false, 0, "", 0, 0, 0, 0, 0, "ModifyShootSpeed", "", "", "", "", 0, 1, "su");
            config[201008] = new BattleSkillConfig(201008, "箭雨", "雨", "攻击时30%发出2只箭", "", 1, 0.3f, 4f, 0, 0, "leadShip", null, 30f, false, "", 1, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackMultiArrow", "flipspin", "", "", "", 0, 3, "duo");
            config[201009] = new BattleSkillConfig(201009, "共杀", "共", "击中目标时触发2次弹射", "", 1, 0.35f, 5f, 0, 0, "inte", null, 30f, false, "", 3, 0, 0, 0, 0.25f, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackReboundArrow", "flipspin", "", "", "", 0, 4, "gong2");
            config[201010] = new BattleSkillConfig(201010, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 1, 0.4f, 5f, 0, 0, "str", null, 25f, false, "", 5, 0, 0, 0, 0.8f, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", "", "", 0, 3, "meng");
            config[201011] = new BattleSkillConfig(201011, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 1, 0.3f, 10f, 0, 0, "inte", null, 0, false, "", 1, 0, 0, 0, 0, 0.15f, 0, 0, 0, "", "", 0, false, 0, "雷", 0, 25f, 5.2f, 0.5f, 0, "HitRegion", "spin", "", "SummonStorm", "", 5f, 4, "tian");
            config[201012] = new BattleSkillConfig(201012, "火计", "火", "攻击时对目标放火", "术", 1, 0.3f, 5f, 0, 0, "inte", null, 0, false, "", 1, 0, 0, 0, 0, 0.1f, 0, 0, 0, "", "", 0, false, 0, "火", 1, 8f, 3.2f, 1f, 0, "HitWall", "throw", "", "SoftFireBigRed", "", 1.6f, 2, "huo");
            config[201013] = new BattleSkillConfig(201013, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 1, 0.3f, 8.5f, 0, 0, "inte", null, 0, false, "", 1, 0, 0, 0, 0, 0.08f, 0, 0, 0, "", "", 0, false, 0, "火", 5, 8f, 3.2f, 1f, 0, "HitWall", "spin", "", "SoftFireBigRed", "", 1.6f, 4, "yan");
            config[201014] = new BattleSkillConfig(201014, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 1, 1f, 6.7f, 2f, 0, "str", null, 45f, false, "", 4, 0, 0, 0, 0, 0.4f, 0, 0, 0, "", "", 0, false, 0, "武", 0, 9f, 1.5f, 0, 40f, "AidShockWave", "spin", "", "", "AxeExplosion", 3f, 2, "fu3");
            config[201015] = new BattleSkillConfig(201015, "驰羽", "羽", "能够射出箭矢", "技", 1, 1f, 7f, 2f, 0, "leadShip", null, 42f, false, "", 0, 0, 0, 0, 0, 0.75f, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AidSuddenArrow", "sway", "", "", "BulletExplosionBlue", 3f, 2, "yu");
            config[201016] = new BattleSkillConfig(201016, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 1, 1f, 6.7f, 2f, 0, "inte", null, 60f, false, "", 4, 0, 0, 0, 0, 0.25f, 0, 0, 0, "", "", 0, false, 0, "雷", 0, 11f, 1.5f, 0, 40f, "AidShockWave", "spin", "", "", "NukeMissileFires", 4f, 3, "lei");
            config[201017] = new BattleSkillConfig(201017, "鬼谋", "鬼", "攻击造成生命值比例伤害", "", 1, 0.25f, 2f, 0, 1f, "inte", null, 0, false, "", 0, 0.2f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DamageReal", "sway", "", "", "", 0, 3, "gui");
            config[201018] = new BattleSkillConfig(201018, "斩", "斩", "直接杀死低生命值单位", "", 1, 0, 7f, 0, 0.3f, "str", null, 0, false, "", 0, 1f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DamageReal", "spin", "", "", "SwordHitRedCritical", 0, 3, "zhan");
            config[201019] = new BattleSkillConfig(201019, "魔神", "魔", "攻击时回复生命", "", 1, 0.35f, 6f, 0, 0, "str", null, 0, false, "", 0, 0, 0, 0, 1f, 0, 0, 0, 0, "", "", 300003, false, 5f, "", 0, 0, 0, 0, 0, "AttackedBuff", "", "", "", "MagicBuffGreen", 0, 2, "mo");
            config[201020] = new BattleSkillConfig(201020, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 1, 1f, 6f, 0, 0, "leadShip", null, 30f, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 301001, false, 1f, "", 0, 0, 0, 0, 0, "HitTeleport", "saw", "MagicNovaBlue", "", "", 0, 1, "fu");
            config[201021] = new BattleSkillConfig(201021, "火矢", "矢", "攻击时射出火箭", "技", 1, 0.35f, 3f, 0, 0, "str", null, 0, false, "", 1, 0, 0, 0, 0, 0.12f, 0, 0, 0, "", "", 0, false, 0, "火", 1, 8f, 3.2f, 1f, 0, "HitWall", "throw", "", "SoftFireBigRed", "", 1.6f, 2, "shi3");
            config[201022] = new BattleSkillConfig(201022, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 1, 0.15f, 6f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 1f, 0, 2, 0.3f, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HitRepeat", "jumpspin", "", "", "SwordHitRedCritical", 0, 4, "hu");
            config[201023] = new BattleSkillConfig(201023, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 1, 0.15f, 7f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 1f, 0, 1, 0.4f, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HitRepeat", "jumpspin", "", "", "SwordHitGreenCritical", 0, 3, "qing");
            config[201024] = new BattleSkillConfig(201024, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 1, 0, 3f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0, 0.7f, 0, 0, 0, 0, "", "", 301001, false, 0, "", 0, 0, 0, 0, 0, "AttackAddDamage", "saw", "", "", "SwordHitRedCritical", 0, 1, "luan");
            config[201025] = new BattleSkillConfig(201025, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 1, 0, 0, 0, 0, "str", null, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 300001, false, 0, "", 0, 0, 0, 0, 0, "AttackAntiShield", "sway", "", "", "", 0, 1, "nue");
            config[202001] = new BattleSkillConfig(202001, "刺甲", "刺", "反弹50%近战伤害", "防御up", 1, 0.3f, 0, 0, 0, "str", new string[]{"str，leadShip"}, 20f, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefFeedback", "", "", "", "SwordHitBlue", 0, 2, "ci");
            config[202002] = new BattleSkillConfig(202002, "藤甲", "藤", "受非智力攻击时高几率发动70%减伤", "", 1, 0.5f, 0.5f, 0, 0, "str", new string[]{"str，leadShip"}, 0, false, "", 0, 0.7f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefPlantSkin", "", "", "", "", 0, 3, "teng");
            config[202003] = new BattleSkillConfig(202003, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 1, 0.3f, 0, 0, 0, "leadShip", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 1, "竟", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefFeedback", "", "", "", "SwordHitBlue", 0, 3, "jing");
            config[202004] = new BattleSkillConfig(202004, "明镜小", "竟", "反弹30%智力伤害", "防御up", 1, 0.3f, 0, 0, 0, "leadShip", new string[]{"inte"}, 0, false, "", 0, 0.3f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefFeedback", "", "", "", "SwordHitBlue", 0, 1, "jing2");
            config[202005] = new BattleSkillConfig(202005, "护卫", "护", "给与友军护盾祝福", "", 1, 1f, 8f, 1.5f, 0, "str", null, 50f, false, "", 0, 0.18f, 0, 0, 0, 0, 0, 0, 0, "", "", 300001, false, 10f, "", 0, 0, 0, 0, 0, "HelpAidBuff", "sway", "", "", "MagicChargeYellow", 0, 1, "hu1");
            config[202006] = new BattleSkillConfig(202006, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 1, 0.3f, 0.5f, 0, 0.35f, "str", null, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefHpLow", "", "", "", "", 0, 3, "jian");
            config[202007] = new BattleSkillConfig(202007, "识破", "识", "同行降低智力类技能伤害", "防御up", 1, 0.3f, 0.5f, 0, 0, "leadShip", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 1, "实", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefSkillDamageReduce", "", "", "", "", 0, 3, "shi5");
            config[202008] = new BattleSkillConfig(202008, "识破小", "实", "降低智力类技能伤害", "防御up", 1, 0.3f, 0.5f, 0, 0, "leadShip", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "DefSkillDamageReduce", "", "", "", "", 0, 1, "shi4");
            config[202009] = new BattleSkillConfig(202009, "冷静", "静", "降低同行智力类技能造成的负面状态时间", "防御up", 1, 0, 0, 0, 0, "leadShip", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 1, "境", "", 0, true, 0, "", 0, 0, 0, 0, 0, "ModifyBeBuffTime", "", "", "", "", 0, 3, "jing3");
            config[202010] = new BattleSkillConfig(202010, "冷静小", "境", "降低智力类技能造成的负面状态时间", "防御up", 1, 0, 0, 0, 0, "leadShip", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, true, 0, "", 0, 0, 0, 0, 0, "ModifyBeBuffTime", "", "", "", "", 0, 1, "jing4");
            config[202011] = new BattleSkillConfig(202011, "敏锐", "敏", "提升15%闪避率", "防御up", 1, 0, 0, 0, 0, "str", null, 0, false, "", 0, 0.2f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAddDodge", "", "", "", "", 0, 1, "min");
            config[202012] = new BattleSkillConfig(202012, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0.2f, 0, 0, 0, 0, 0, 0, 2, "敏", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAddDodge", "", "", "", "", 0, 2, "kong");
            config[202013] = new BattleSkillConfig(202013, "复原", "复", "提升5点生命回复", "防御up", 1, 0, 0, 0, 0, "leadShip", null, 0, false, "", 0, 0, 5, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAddRege", "", "", "", "", 0, 1, "fu2");
            config[202014] = new BattleSkillConfig(202014, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 5, 0, 0, 0, 0, 0, 3, "复", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAddRege", "", "", "", "", 0, 2, "yao");
            config[203002] = new BattleSkillConfig(203002, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 1, 0.5f, 3f, 0, 0, "inte", null, 25f, false, "targetUnit", 1, 0, 0, 0, 0.5f, 0, 0, 0, 0, "", "", 301002, false, 8f, "", 0, 0, 0, 0, 0, "HitBuffArea", "throw", "", "", "MagicNovaYellow", 0, 4, "suo");
            config[203003] = new BattleSkillConfig(203003, "劫粮", "劫", "攻击几率获取对方粮食", "", 1, 0.2f, 4f, 0, 0, "leadShip", null, 0, false, "", 0, 0, 2, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HitFood", "sway", "", "", "", 0, 2, "jie2");
            config[203004] = new BattleSkillConfig(203004, "威震", "威", "攻击时混乱周围目标", "", 1, 0.2f, 5f, 0, 0, "str", null, 20f, false, "castUnit", 3, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 301001, false, 1.5f, "", 0, 0, 0, 0, 0, "HitBuffArea", "spin", "", "", "MagicNovaYellow", 0, 4, "wei");
            config[203005] = new BattleSkillConfig(203005, "击破", "破", "攻击几率使目标增伤40%", "", 1, 0.4f, 2f, 0, 0, "str", null, 0, false, "", 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "", "", 301003, false, 3f, "", 0, 0, 0, 0, 0, "HitBuff", "jump", "", "", "SoftFireBigRed", 0, 2, "po");
            config[203006] = new BattleSkillConfig(203006, "延缓", "缓", "攻击几率使目标减速30%", "", 1, 0.4f, 3f, 0, 0, "inte", null, 0, false, "", 0, 0.3f, 0, 0, 0, 0, 0, 0, 0, "", "", 301004, false, 5f, "", 0, 0, 0, 0, 0, "HitBuff", "sway", "", "", "MagicNovaYellow", 0, 2, "huan");
            config[203007] = new BattleSkillConfig(203007, "陷阵", "陷", "攻击几率使目标陷阵", "", 1, 0.4f, 3f, 0, 0, "inte", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 301005, false, 4f, "", 0, 0, 0, 0, 0, "HitBuff", "sway", "", "", "MagicNovaYellow", 0, 2, "xian");
            config[203008] = new BattleSkillConfig(203008, "溃散", "溃", "攻击几率使目标溃败", "", 1, 0.4f, 4f, 0, 0, "inte", null, 0, false, "", 0, 0, 0, 0, 0, 0.1f, 0, 0, 0, "", "", 301006, false, 5.2f, "", 0, 0, 0, 0, 0, "HitBuff", "sway", "", "", "MagicNovaYellow", 0, 2, "kui");
            config[203009] = new BattleSkillConfig(203009, "分兵", "分", "被攻击时产生一只有伤害部队", "", 1, 0.4f, 15f, 0, 0, "leadShip", null, 15f, false, "", 0, 0, 0, 0.5f, 0.5f, 0, 4, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackedShadow", "sway", "MagicFieldGreen", "", "MagicFieldGreen", 0, 3, "fen2");
            config[203010] = new BattleSkillConfig(203010, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 1, 0.4f, 15f, 0, 0, "leadShip", null, 15f, false, "", 0, 0, 0, 0.4f, 0.01f, 0, 4, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "AttackedShadow", "sway", "MagicFieldGreen", "", "MagicFieldGreen", 0, 2, "fen3");
            config[208001] = new BattleSkillConfig(208001, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 1, 0, 0, 0, 0, "inte", new string[]{"inte"}, 0, false, "", 0, 0.3f, 0, 0, 0, 0, 0, 0, 1, "白", "扇谋相", 0, false, 0, "", 0, 0, 0, 0, 0, "ModifySkillRateTime", "", "", "", "", 0, 4, "bai");
            config[208002] = new BattleSkillConfig(208002, "百出小", "白", "降低技能CD时间", "智技up", 1, 0, 0, 0, 0, "inte", new string[]{"inte"}, 0, false, "", 0, 0.4f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "ModifySkillRateTime", "", "", "", "", 0, 2, "bai2");
            config[208003] = new BattleSkillConfig(208003, "神算", "神", "提升技能命中率和持续时间", "智技up", 1, 0.3f, 0, 0, 0, "inte", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0.5f, "", 0, 0, 0, 0, 0, "ModifySkillRateTime", "", "", "", "", 0, 3, "shen");
            config[208004] = new BattleSkillConfig(208004, "蔓延", "延", "自己和同行[扇谋相]技能负面状态扩散", "智技up", 1, 0.5f, 3f, 0, 0, "inte", new string[]{"inte"}, 30f, false, "", 2, 0, 0, 0, 0, 0, 0, 0, 1, "筵", "扇谋相", 0, false, 0, "", 0, 0, 0, 0, 0, "BuffExpand", "", "", "", "", 0, 4, "yan2");
            config[208005] = new BattleSkillConfig(208005, "蔓延小", "筵", "技能负面状态概率扩散", "智技up", 1, 0.5f, 3f, 0, 0, "inte", new string[]{"inte"}, 30f, false, "", 2, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "BuffExpand", "", "", "", "", 0, 2, "yan3");
            config[208006] = new BattleSkillConfig(208006, "同调", "调", "自己和同行[鼓乐医]技能正面状态扩散", "智技up", 1, 0.5f, 3f, 0, 0, "inte", new string[]{"inte"}, 30f, false, "", 2, 0, 0, 0, 0, 0, 0, 0, 1, "碉", "鼓乐医", 0, false, 0, "", 0, 0, 0, 0, 0, "BuffExpandPos", "", "", "", "", 0, 3, "diao");
            config[208007] = new BattleSkillConfig(208007, "同调小", "碉", "技能正面状态扩散", "智技up", 1, 0.5f, 3f, 0, 0, "inte", new string[]{"inte"}, 30f, false, "", 2, 0, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "BuffExpandPos", "", "", "", "", 0, 2, "diao2");
            config[208008] = new BattleSkillConfig(208008, "炽热", "炽", "提升本方火焰持续时间", "智技up", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 0, 0, 0, 0, 0, 0, 3, "炽", "", 0, false, 0, "火", 0, 0, 3f, 0, 0, "ModifySummonTime", "", "", "", "", 0, 2, "chi");
            config[208009] = new BattleSkillConfig(208009, "曲扬", "曲", "正面祝福状态时间增加50%", "智技up", 1, 0, 0, 0, 0, "inte", new string[]{"inte"}, 0, false, "", 0, 0.5f, 0, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "ModifyBuffTime", "", "", "", "", 0, 1, "qu");
            config[209001] = new BattleSkillConfig(209001, "富甲", "商", "战斗开始时获得金币", "", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 5, 0.05f, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitGold", "", "MagicChargeYellow", "", "", 0, 3, "gold");
            config[209002] = new BattleSkillConfig(209002, "国士", "国", "增加一个远程士兵并提升射程", "", 1, 0, 0, 0, 0, "inte", null, 15f, false, "", 0, 0, 0, 0.075f, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitSoldierSummon", "", "", "", "MagicChargeGreen", 0, 4, "guo");
            config[209003] = new BattleSkillConfig(209003, "仁德", "仁", "给与我方前排士兵12%生命值护盾", "", 1, 0, 0, 0, 0, "leadShip", null, 0, false, "", 0, 0, 0, 0.12f, 0, 0, 0, 0, 0, "", "", 300001, false, 999f, "", 0, 0, 0, 0, 0, "InitSoldierShield", "", "", "", "", 0, 3, "ren");
            config[209004] = new BattleSkillConfig(209004, "激励", "励", "提升同列队友智力", "", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 0, 0.7f, 0, 0, 0, 0, 2, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HelpInitTeach", "", "", "", "MagicChargeYellow", 0, 2, "li");
            config[209005] = new BattleSkillConfig(209005, "学习", "学", "攻击时几率提升自己的属性", "", 1, 0.3f, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 5, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HitAttr", "", "MagicChargeYellow", "", "", 0, 3, "zhang");
            config[209006] = new BattleSkillConfig(209006, "制衡", "衡", "初始获得我方兵种数为3,4,5时,提升属性10,20,30", "", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 5, 0, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitAttrZhiheng", "", "MagicChargeYellow", "", "", 0, 2, "heng");
            config[209007] = new BattleSkillConfig(209007, "米道", "米", "战斗开始时获得粮食", "", 1, 0, 0, 0, 0, "inte", null, 0, false, "", 0, 0, 10, 0.1f, 0, 0, 0, 0, 0, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "InitFood", "", "MagicChargeYellow", "", "", 0, 2, "food");
            config[209008] = new BattleSkillConfig(209008, "奋进", "奋", "提升同列队友武力", "", 1, 0, 0, 0, 0, "str", null, 0, false, "", 0, 0, 0, 0.7f, 0, 0, 0, 0, 2, "", "", 0, false, 0, "", 0, 0, 0, 0, 0, "HelpInitTeach", "", "", "", "MagicChargePink", 0, 2, "fen");



        }

        public static BattleSkillConfig GetConfig(int id)
        {
            BattleSkillConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表BattleSkillConfig不存在id={0}", id));
        }



        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, BattleSkillConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, BattleSkillConfig configData)
        {
            if (!config.ContainsKey(id))
            {
                config.Add(id, configData);
            }
        }

        public static void Remove(int id)
        {
            if (config.ContainsKey(id))
            {
                config.Remove(id);
            }
        }
    }
}
