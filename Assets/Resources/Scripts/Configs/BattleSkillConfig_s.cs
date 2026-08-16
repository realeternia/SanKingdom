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
            public int fieldWidth;
            public string fieldRule;
            public bool fieldIndex;
            public FieldMetaInfo(string name, string type, int width = 0, string rule = "", bool index = false)
            {
                fieldName = name;
                fieldType = type;
                fieldWidth = width;
                fieldRule = rule;
                fieldIndex = index;
            }
        }

        public class CellMeta
        {
            public int row;
            public int col;
            public int? foreColor;
            public int? backColor;
            public CellMeta(int row, int col, int? foreColor, int? backColor)
            {
                this.row = row;
                this.col = col;
                this.foreColor = foreColor;
                this.backColor = backColor;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Sname", new FieldMetaInfo("缩写", "string", 0)},
            {"Descript", new FieldMetaInfo("说明", "string", 0)},
            {"Type", new FieldMetaInfo("分类", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 0)},
            {"Rate", new FieldMetaInfo("发动概率", "float", 0)},
            {"RoundCd", new FieldMetaInfo("发动cd回合数", "int", 0)},
            {"ConditionParm", new FieldMetaInfo("条件参数", "float", 0)},
            {"Attr", new FieldMetaInfo("相关属性", "string", 0)},
            {"CheckAttrs", new FieldMetaInfo("判定属性", "string[]", 0)},
            {"Range", new FieldMetaInfo("范围", "float", 0)},
            {"RangeOut", new FieldMetaInfo("范围外", "bool", 0)},
            {"TargetType", new FieldMetaInfo("选取点", "string", 0)},
            {"TargetCount", new FieldMetaInfo("最大目标数", "int", 0)},
            {"Strength", new FieldMetaInfo("技能强度（恒定）", "float", 0)},
            {"StrengthInt", new FieldMetaInfo("技能强度（恒定）", "int", 0)},
            {"SkillAttrRate", new FieldMetaInfo("技能数值比例", "float", 0)},
            {"SkillDamageRate", new FieldMetaInfo("技能强度伤害比例", "float", 0)},
            {"SkillDamageAttrRate", new FieldMetaInfo("技能强度属性比例", "float", 0)},
            {"DoCount", new FieldMetaInfo("计数次数", "int", 0)},
            {"UnitHelpType", new FieldMetaInfo("效果范围(1横向，2纵向)", "int", 0)},
            {"HelpSkill", new FieldMetaInfo("光环技能", "string", 0)},
            {"HelpSkillJob", new FieldMetaInfo("职业限定", "string", 0)},
            {"BuffId", new FieldMetaInfo("BuffId", "int", 0)},
            {"NegBuff", new FieldMetaInfo("是否针对负面buff", "bool", 0)},
            {"BuffTime", new FieldMetaInfo("持续回合数", "int", 0)},
            {"SummonTag", new FieldMetaInfo("召唤物标签", "string", 0)},
            {"SummonCount", new FieldMetaInfo("技能场数", "int", 0)},
            {"SummonArea", new FieldMetaInfo("技能场范围", "float", 0)},
            {"SummonRoundCount", new FieldMetaInfo("持续回合数", "int", 0)},
            {"SummonHitInterval", new FieldMetaInfo("技能场间隔", "float", 0)},
            {"SummonSpeed", new FieldMetaInfo("技能场速度", "float", 0)},
            {"ScriptName", new FieldMetaInfo("脚本名", "string", 0)},
            {"Action", new FieldMetaInfo("动作", "string", 0)},
            {"EffectSelf", new FieldMetaInfo("自己", "string", 0)},
            {"EffectArea", new FieldMetaInfo("区域", "string", 0)},
            {"EffectHit", new FieldMetaInfo("hit", "string", 0)},
            {"EffectSize", new FieldMetaInfo("size", "float", 0)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

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
        ///发动cd回合数
        /// </summary>
        public int RoundCd;
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
        ///持续回合数
        /// </summary>
        public int BuffTime;
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
        ///持续回合数
        /// </summary>
        public int SummonRoundCount;
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
        ///图标
        /// </summary>
        public string Icon;


        public BattleSkillConfig(int Id, string Name, string Sname, string Descript, string Type, int Lv, float Rate, int RoundCd, float ConditionParm, string Attr, string[] CheckAttrs, float Range, bool RangeOut, string TargetType, int TargetCount, float Strength, int StrengthInt, float SkillAttrRate, float SkillDamageRate, float SkillDamageAttrRate, int DoCount, int UnitHelpType, string HelpSkill, string HelpSkillJob, int BuffId, bool NegBuff, int BuffTime, string SummonTag, int SummonCount, float SummonArea, int SummonRoundCount, float SummonHitInterval, float SummonSpeed, string ScriptName, string Action, string EffectSelf, string EffectArea, string EffectHit, float EffectSize, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Sname = Sname;
            this.Descript = Descript;
            this.Type = Type;
            this.Lv = Lv;
            this.Rate = Rate;
            this.RoundCd = RoundCd;
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
            this.UnitHelpType = UnitHelpType;
            this.HelpSkill = HelpSkill;
            this.HelpSkillJob = HelpSkillJob;
            this.BuffId = BuffId;
            this.NegBuff = NegBuff;
            this.BuffTime = BuffTime;
            this.SummonTag = SummonTag;
            this.SummonCount = SummonCount;
            this.SummonArea = SummonArea;
            this.SummonRoundCount = SummonRoundCount;
            this.SummonHitInterval = SummonHitInterval;
            this.SummonSpeed = SummonSpeed;
            this.ScriptName = ScriptName;
            this.Action = Action;
            this.EffectSelf = EffectSelf;
            this.EffectArea = EffectArea;
            this.EffectHit = EffectHit;
            this.EffectSize = EffectSize;
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
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[200002] = new BattleSkillConfig(200002, "羽扇", "扇", "击中目标时触发弹射", "职业", 1, 0f, 0, 0f, "inte", null, 2f, false, "", 1, 0f, 0, 0f, 0.15f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackReboundArrow", "sway", "", "", "", 0f, "shan");
            config[200003] = new BattleSkillConfig(200003, "刀兵", "刀", "攻击几率造成额外伤害", "职业", 1, 0.15f, 5, 0f, "leadShip", null, 0f, false, "", 0, 0f, 10, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackAddDamage", "sway", "", "", "SwordHitRedCritical", 0f, "dao");
            config[200004] = new BattleSkillConfig(200004, "坚韧", "士", "受击时几率发动减伤", "职业", 1, 0.35f, 7, 0f, "str", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0, "", "", 300002, false, 4, "", 0, 0f, 0, 0f, 0f, "AttackedBuff", "spin", "", "", "", 0f, "shi");
            config[200007] = new BattleSkillConfig(200007, "弓手", "弓", "远程射击单位", "职业", 1, 1f, 99, 0f, "leadShip", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "Dumb", "", "", "", "", 0f, "gong");
            config[200008] = new BattleSkillConfig(200008, "谋略", "谋", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4, 0f, "inte", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 301001, false, 2, "", 0, 0f, 0, 0f, 0f, "HitBuff", "throw", "", "", "MagicChargeYellow", 0f, "mou");
            config[200009] = new BattleSkillConfig(200009, "炮车", "炮", "攻击目标发生爆炸", "职业", 1, 0.5f, 0, 0f, "leadShip", null, 2f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HitArea", "throw", "", "", "MagicNovaYellow", 0f, "pao");
            config[200010] = new BattleSkillConfig(200010, "弩手", "弩", "射程非常远", "职业", 1, 1f, 99, 0f, "leadShip", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "Dumb", "", "", "", "", 0f, "nu");
            config[200012] = new BattleSkillConfig(200012, "声乐", "乐", "给与友军攻速祝福", "职业", 1, 1f, 3, 0f, "inte", null, 4f, false, "", 0, 0.45f, 0, 0f, 0f, 0f, 0, 0, "", "", 300005, false, 5, "", 0, 0f, 0, 0f, 0f, "HelpAidBuff", "sway", "", "", "MagicChargePink", 0f, "song");
            config[200013] = new BattleSkillConfig(200013, "治疗", "医", "给与友军治疗", "职业", 1, 1f, 3, 0f, "inte", null, 4f, false, "", 0, 0f, 0, 0.7f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HelpAidHeal", "sway", "", "", "MagicBuffGreen", 0f, "heal");
            config[200014] = new BattleSkillConfig(200014, "枪阵", "枪", "一定几率混乱目标单位2s", "职业", 1, 0.15f, 4, 0f, "leadShip", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 301001, false, 2, "", 0, 0f, 0, 0f, 0f, "HitBuff", "spin", "", "", "MagicChargeYellow", 0f, "qiang");
            config[200015] = new BattleSkillConfig(200015, "戟阵", "戟", "攻击目标时伤害周边敌人", "职业", 1, 0.35f, 4, 0f, "leadShip", null, 2f, false, "", 2, 0f, 0, 0f, 0.6f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HitAround", "throw", "", "", "SwordSlashMiniWhite", 0f, "ji");
            config[200016] = new BattleSkillConfig(200016, "战鼓", "鼓", "给与友军攻击力祝福", "职业", 1, 1f, 3, 0f, "inte", null, 4f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0, "", "", 300004, false, 5, "", 0, 0f, 0, 0f, 0f, "HelpAidBuff", "sway", "", "", "MagicChargeYellow", 0f, "gu");
            config[201004] = new BattleSkillConfig(201004, "瓦解小", "透", "提升20%暴击率", "攻击up", 1, 0f, 0, 0f, "str", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "InitAddCrit", "", "", "", "", 0f, "jie3");
            config[201005] = new BattleSkillConfig(201005, "瓦解", "解", "自己和同行队友提升20%暴击率", "攻击up", 1, 0f, 0, 0f, "str", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 1, "透", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "InitAddCrit", "", "", "", "", 0f, "jie");
            config[201006] = new BattleSkillConfig(201006, "连击", "连", "攻击时几率触发连续攻击", "攻击up", 1, 0.3f, 5, 0f, "leadShip", null, 0f, false, "", 0, 0.7f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackSpeedAttack", "flipspin", "", "", "", 0f, "lian");
            config[201008] = new BattleSkillConfig(201008, "箭雨", "雨", "攻击时30%发出2只箭", "", 1, 0.3f, 4, 0f, "leadShip", null, 2f, false, "", 1, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackMultiArrow", "flipspin", "", "", "", 0f, "duo");
            config[201009] = new BattleSkillConfig(201009, "共杀", "共", "击中目标时触发2次弹射", "", 1, 0.35f, 5, 0f, "inte", null, 2f, false, "", 3, 0f, 0, 0f, 0.25f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackReboundArrow", "flipspin", "", "", "", 0f, "gong2");
            config[201010] = new BattleSkillConfig(201010, "旋风斩", "旋", "攻击时几率对附近敌人造成伤害", "技", 1, 0.4f, 5, 0f, "str", null, 2f, false, "", 5, 0f, 0, 0f, 0.8f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackSpinAttack", "spin", "SwordWhirlwindWhite", "", "", 0f, "meng");
            config[201011] = new BattleSkillConfig(201011, "落雷", "天", "攻击召唤出持续伤害的雷电阵", "术", 1, 0.3f, 10, 0f, "inte", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.15f, 0, 0, "", "", 0, false, 0, "雷", 0, 2f, 3, 0.5f, 0f, "HitRegion", "spin", "", "SummonStorm", "", 5f, "tian");
            config[201012] = new BattleSkillConfig(201012, "火计", "火", "攻击时对目标放火", "术", 1, 0.3f, 3, 0f, "inte", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.1f, 0, 0, "", "", 0, false, 0, "火", 1, 1f, 3, 1f, 0f, "HitWall", "throw", "", "SoftFireBigRed", "", 1.6f, "huo");
            config[201013] = new BattleSkillConfig(201013, "火墙", "炎", "攻击召唤出持续伤害的火墙", "术", 1, 0.3f, 5, 0f, "inte", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.08f, 0, 0, "", "", 0, false, 0, "火", 5, 1f, 3, 1f, 0f, "HitWall", "spin", "", "SoftFireBigRed", "", 1.6f, "yan");
            config[201014] = new BattleSkillConfig(201014, "飞斧", "斧", "扔出飞斧攻击前方敌人", "技", 1, 1f, 6, 0f, "str", null, 3f, false, "", 4, 0f, 0, 0f, 0f, 0.4f, 0, 0, "", "", 0, false, 0, "武", 0, 1f, 2, 0f, 40f, "AidShockWave", "spin", "", "", "AxeExplosion", 3f, "fu3");
            config[201015] = new BattleSkillConfig(201015, "驰羽", "羽", "能够射出箭矢", "技", 1, 1f, 7, 0f, "leadShip", null, 3f, false, "", 0, 0f, 0, 0f, 0f, 0.75f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AidSuddenArrow", "sway", "", "", "BulletExplosionBlue", 3f, "yu");
            config[201016] = new BattleSkillConfig(201016, "惊雷", "雷", "召唤3个惊雷攻击前方敌人", "术", 1, 1f, 6, 0f, "inte", null, 4f, false, "", 4, 0f, 0, 0f, 0f, 0.25f, 0, 0, "", "", 0, false, 0, "雷", 0, 1f, 2, 0f, 40f, "AidShockWave", "spin", "", "", "NukeMissileFires", 4f, "lei");
            config[201019] = new BattleSkillConfig(201019, "魔神", "魔", "攻击时回复生命", "", 1, 0.35f, 6, 0f, "str", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 0, 0, "", "", 300003, false, 5, "", 0, 0f, 0, 0f, 0f, "AttackedBuff", "", "", "", "MagicBuffGreen", 0f, "mo");
            config[201020] = new BattleSkillConfig(201020, "埋伏", "伏", "被攻击时瞬移到远程攻击者附近", "", 1, 1f, 6, 0f, "leadShip", null, 2f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 301001, false, 1, "", 0, 0f, 0, 0f, 0f, "HitTeleport", "saw", "MagicNovaBlue", "", "", 0f, "fu");
            config[201021] = new BattleSkillConfig(201021, "火矢", "矢", "攻击时射出火箭", "技", 1, 0.35f, 3, 0f, "str", null, 0f, false, "", 1, 0f, 0, 0f, 0f, 0.12f, 0, 0, "", "", 0, false, 0, "火", 1, 1f, 3, 1f, 0f, "HitWall", "throw", "", "SoftFireBigRed", "", 1.6f, "shi3");
            config[201022] = new BattleSkillConfig(201022, "虎卫队", "虎", "攻击时几率对目标进行三连击", "技", 1, 0.15f, 6, 0f, "leadShip", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 2, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HitRepeat", "jumpspin", "", "", "SwordHitRedCritical", 0f, "hu");
            config[201023] = new BattleSkillConfig(201023, "青州兵", "青", "攻击时几率对目标进行2连击", "技", 1, 0.15f, 7, 0f, "leadShip", null, 0f, false, "", 0, 0f, 0, 0f, 1f, 0f, 1, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HitRepeat", "jumpspin", "", "", "SwordHitGreenCritical", 0f, "qing");
            config[201024] = new BattleSkillConfig(201024, "乱战", "乱", "攻击晕眩单位造成额外伤害", "", 1, 0f, 3, 0f, "leadShip", null, 0f, false, "", 0, 0f, 0, 0f, 0.7f, 0f, 0, 0, "", "", 301001, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackAddDamage", "saw", "", "", "SwordHitRedCritical", 0f, "luan");
            config[201025] = new BattleSkillConfig(201025, "虐袭", "虐", "攻击护盾敌人时造成额外伤害", "技", 1, 0f, 0, 0f, "str", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0, "", "", 300001, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackAntiShield", "sway", "", "", "", 0f, "nue");
            config[202001] = new BattleSkillConfig(202001, "刺甲", "刺", "反弹50%近战伤害", "防御up", 1, 0.3f, 0, 0f, "str", new string[]{"str，leadShip"}, 2f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "DefFeedback", "", "", "", "SwordHitBlue", 0f, "ci");
            config[202003] = new BattleSkillConfig(202003, "明镜", "镜", "自己和同行队友反弹智力伤害", "防御up", 1, 0.3f, 0, 0f, "leadShip", new string[]{"inte"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 1, "竟", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "DefFeedback", "", "", "", "SwordHitBlue", 0f, "jing");
            config[202004] = new BattleSkillConfig(202004, "明镜小", "竟", "反弹30%智力伤害", "防御up", 1, 0.3f, 0, 0f, "leadShip", new string[]{"inte"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "DefFeedback", "", "", "", "SwordHitBlue", 0f, "jing2");
            config[202005] = new BattleSkillConfig(202005, "护卫", "护", "给与友军护盾祝福", "", 1, 1f, 8, 0f, "str", null, 4f, false, "", 0, 0.18f, 0, 0f, 0f, 0f, 0, 0, "", "", 300001, false, 10, "", 0, 0f, 0, 0f, 0f, "HelpAidBuff", "sway", "", "", "MagicChargeYellow", 0f, "hu1");
            config[202006] = new BattleSkillConfig(202006, "坚毅", "坚", "生命值低时降低50%伤害", "防御up", 1, 0.3f, 0, 0.35f, "str", null, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "DefHpLow", "", "", "", "", 0f, "jian");
            config[202007] = new BattleSkillConfig(202007, "识破", "识", "同行降低智力类技能伤害", "防御up", 1, 0.3f, 0, 0f, "leadShip", new string[]{"inte"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 1, "实", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "DefSkillDamageReduce", "", "", "", "", 0f, "shi5");
            config[202008] = new BattleSkillConfig(202008, "识破小", "实", "降低智力类技能伤害", "防御up", 1, 0.3f, 0, 0f, "leadShip", new string[]{"inte"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "DefSkillDamageReduce", "", "", "", "", 0f, "shi4");
            config[202011] = new BattleSkillConfig(202011, "敏锐", "敏", "提升15%闪避率", "防御up", 1, 0f, 0, 0f, "str", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "InitAddDodge", "", "", "", "", 0f, "min");
            config[202012] = new BattleSkillConfig(202012, "空城", "空", "自己和同列队友提升15%闪避率", "防御up", 1, 0f, 0, 0f, "inte", null, 0f, false, "", 0, 0.2f, 0, 0f, 0f, 0f, 0, 2, "敏", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "InitAddDodge", "", "", "", "", 0f, "kong");
            config[202013] = new BattleSkillConfig(202013, "复原", "复", "提升5点生命回复", "防御up", 1, 0f, 0, 0f, "leadShip", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "InitAddRege", "", "", "", "", 0f, "fu2");
            config[202014] = new BattleSkillConfig(202014, "药仙", "药", "自己和所有队友提升5点生命回复", "防御up", 1, 0f, 0, 0f, "inte", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 3, "复", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "InitAddRege", "", "", "", "", 0f, "yao");
            config[203002] = new BattleSkillConfig(203002, "连锁", "锁", "锁定敌人目标,传递一半受到伤害", "术", 1, 0.5f, 3, 0f, "inte", null, 2f, false, "targetUnit", 1, 0f, 0, 0f, 0.5f, 0f, 0, 0, "", "", 301002, false, 8, "", 0, 0f, 0, 0f, 0f, "HitBuffArea", "throw", "", "", "MagicNovaYellow", 0f, "suo");
            config[203003] = new BattleSkillConfig(203003, "劫粮", "劫", "攻击几率获取对方粮食", "", 1, 0.2f, 4, 0f, "leadShip", null, 0f, false, "", 0, 0f, 2, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HitFood", "sway", "", "", "", 0f, "jie2");
            config[203004] = new BattleSkillConfig(203004, "威震", "威", "攻击时混乱周围目标", "", 1, 0.2f, 5, 0f, "str", null, 2f, false, "castUnit", 3, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 301001, false, 1, "", 0, 0f, 0, 0f, 0f, "HitBuffArea", "spin", "", "", "MagicNovaYellow", 0f, "wei");
            config[203005] = new BattleSkillConfig(203005, "击破", "破", "攻击几率使目标增伤40%", "", 1, 0.4f, 2, 0f, "str", null, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0, "", "", 301003, false, 3, "", 0, 0f, 0, 0f, 0f, "HitBuff", "jump", "", "", "SoftFireBigRed", 0f, "po");
            config[203006] = new BattleSkillConfig(203006, "延缓", "缓", "攻击几率使目标减速30%", "", 1, 0.4f, 3, 0f, "inte", null, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 0, "", "", 301004, false, 5, "", 0, 0f, 0, 0f, 0f, "HitBuff", "sway", "", "", "MagicNovaYellow", 0f, "huan");
            config[203007] = new BattleSkillConfig(203007, "陷阵", "陷", "攻击几率使目标陷阵", "", 1, 0.4f, 3, 0f, "inte", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 0, "", "", 301005, false, 4, "", 0, 0f, 0, 0f, 0f, "HitBuff", "sway", "", "", "MagicNovaYellow", 0f, "xian");
            config[203008] = new BattleSkillConfig(203008, "溃散", "溃", "攻击几率使目标溃败", "", 1, 0.4f, 4, 0f, "inte", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0.1f, 0, 0, "", "", 301006, false, 5, "", 0, 0f, 0, 0f, 0f, "HitBuff", "sway", "", "", "MagicNovaYellow", 0f, "kui");
            config[203009] = new BattleSkillConfig(203009, "分兵", "分", "被攻击时产生一只有伤害部队", "", 1, 0.4f, 15, 0f, "leadShip", null, 1f, false, "", 0, 0f, 0, 0.5f, 0.5f, 0f, 4, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", "", "MagicFieldGreen", 0f, "fen2");
            config[203010] = new BattleSkillConfig(203010, "分兵小", "纷", "被攻击时产生一只无伤害部队", "", 1, 0.4f, 15, 0f, "leadShip", null, 1f, false, "", 0, 0f, 0, 0.4f, 0.01f, 0f, 4, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "AttackedShadow", "sway", "MagicFieldGreen", "", "MagicFieldGreen", 0f, "fen3");
            config[208001] = new BattleSkillConfig(208001, "百出", "百", "降低自己和同行[扇谋相]技能CD时间", "智技up", 1, 0f, 0, 0f, "inte", new string[]{"inte"}, 0f, false, "", 0, 0.3f, 0, 0f, 0f, 0f, 0, 1, "白", "扇谋相", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "ModifySkillRateTime", "", "", "", "", 0f, "bai");
            config[208002] = new BattleSkillConfig(208002, "百出小", "白", "降低技能CD时间", "智技up", 1, 0f, 0, 0f, "inte", new string[]{"inte"}, 0f, false, "", 0, 0.4f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "ModifySkillRateTime", "", "", "", "", 0f, "bai2");
            config[208003] = new BattleSkillConfig(208003, "神算", "神", "提升技能命中率和持续时间", "智技up", 1, 0.3f, 0, 0f, "inte", new string[]{"inte"}, 0f, false, "", 0, 0.5f, 0, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "ModifySkillRateTime", "", "", "", "", 0f, "shen");
            config[208008] = new BattleSkillConfig(208008, "炽热", "炽", "提升本方火焰持续时间", "智技up", 1, 0f, 0, 0f, "inte", null, 0f, false, "", 0, 0f, 0, 0f, 0f, 0f, 0, 3, "炽", "", 0, false, 0, "火", 0, 0f, 3, 0f, 0f, "ModifySummonTime", "", "", "", "", 0f, "chi");
            config[209005] = new BattleSkillConfig(209005, "学习", "学", "攻击时几率提升自己的属性", "", 1, 0.3f, 0, 0f, "inte", null, 0f, false, "", 0, 0f, 5, 0f, 0f, 0f, 0, 0, "", "", 0, false, 0, "", 0, 0f, 0, 0f, 0f, "HitAttr", "", "MagicChargeYellow", "", "", 0f, "zhang");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
