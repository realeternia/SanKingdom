using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string Name;
        /// <summary>
        ///统帅
        /// </summary>
        public int LeadShip;
        /// <summary>
        ///武力
        /// </summary>
        public int Str;
        /// <summary>
        ///智力
        /// </summary>
        public int Inte;
        /// <summary>
        ///内政
        /// </summary>
        public int Fair;
        /// <summary>
        ///魅力
        /// </summary>
        public int Charm;
        /// <summary>
        ///总属性
        /// </summary>
        public int Total;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
        /// <summary>
        ///阵营
        /// </summary>
        public int ForceId;
        /// <summary>
        ///所在
        /// </summary>
        public string City;
        /// <summary>
        ///移动速度
        /// </summary>
        public int MoveSpeed;
        /// <summary>
        ///攻击距离
        /// </summary>
        public int Range;
        /// <summary>
        ///导弹速度
        /// </summary>
        public int MissileSpeed;
        /// <summary>
        ///导弹高度
        /// </summary>
        public float MissileHight;
        /// <summary>
        ///出场概率
        /// </summary>
        public int RateWeight;
        /// <summary>
        ///出场概率，绝对
        /// </summary>
        public int RateAbs;
        /// <summary>
        ///站位
        /// </summary>
        public int Pos;
        /// <summary>
        ///职业
        /// </summary>
        public string Job;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///技能
        /// </summary>
        public string Skill1;
        /// <summary>
        ///技能2
        /// </summary>
        public string Skill2;
        /// <summary>
        ///团队
        /// </summary>
        public string Group;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///背景图
        /// </summary>
        public string Icon;


        public HeroConfig(int Id, string Name, int LeadShip, int Str, int Inte, int Fair, int Charm, int Total, int Hp, int ForceId, string City, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int RateWeight, int RateAbs, int Pos, string Job, int[] Skills, string Skill1, string Skill2, string Group, string HitEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.LeadShip = LeadShip;
            this.Str = Str;
            this.Inte = Inte;
            this.Fair = Fair;
            this.Charm = Charm;
            this.Total = Total;
            this.Hp = Hp;
            this.ForceId = ForceId;
            this.City = City;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.RateWeight = RateWeight;
            this.RateAbs = RateAbs;
            this.Pos = Pos;
            this.Job = Job;
            this.Skills = Skills;
            this.Skill1 = Skill1;
            this.Skill2 = Skill2;
            this.Group = Group;
            this.HitEffect = HitEffect;
            this.Icon = Icon;

        }

        public HeroConfig() { }

        private static Dictionary<int, HeroConfig> config = new Dictionary<int, HeroConfig>();
        public static Dictionary<int, HeroConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "刘备", 80, 77, 78, 82, 99, 416, 330, 1, "新野", 10, 17, 0, 0, 0, 100, 1, "帅", null, "仁", "", "core", "SwordHitYellowCritical", "liubei");
            config[101001] = new HeroConfig(101001, "张飞", 92, 98, 48, 45, 70, 353, 310, 1, "新野", 10, 17, 0, 0, 485, 0, 1, "枪", null, "威", "", "atk", "SwordHitYellowCritical", "zhangfei");
            config[101002] = new HeroConfig(101002, "关羽", 97, 97, 77, 75, 94, 440, 260, 1, "新野", 10, 17, 0, 0, 2229, 0, 1, "车", null, "斩", "", "atk", "SwordHitGreenCritical", "guanyu");
            config[101003] = new HeroConfig(101003, "徐庶", 87, 65, 93, 82, 84, 411, 230, 1, "新野", 10, 17, 15, 0, 1341, 0, 3, "谋", null, "火", "共", "inte", "GasExplosionFire", "xusu");
            config[101004] = new HeroConfig(101004, "周仓", 63, 82, 55, 58, 65, 323, 335, 1, "新野", 10, 17, 0, 0, 286, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "zhoucang");
            config[101005] = new HeroConfig(101005, "廖化", 74, 78, 68, 70, 72, 362, 330, 1, "新野", 10, 17, 0, 0, 568, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "liaohua");
            config[101006] = new HeroConfig(101006, "简雍", 72, 65, 69, 75, 70, 351, 240, 1, "新野", 10, 17, 22, 1.5f, 468, 0, 3, "弓", null, "破", "", "shoot", "BulletExplosionBlue", "jianyong");
            config[101007] = new HeroConfig(101007, "孙乾", 62, 54, 80, 84, 82, 362, 250, 1, "新野", 10, 17, 35, 0, 568, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "sunqian");
            config[101008] = new HeroConfig(101008, "刘封", 75, 79, 44, 55, 50, 303, 220, 1, "新野", 10, 17, 30, 1.5f, 202, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "liufeng");
            config[101009] = new HeroConfig(101009, "关平", 79, 82, 72, 71, 78, 382, 290, 1, "新野", 10, 17, 0, 0, 806, 0, 1, "戟", null, "连", "", "def", "SwordHitYellowCritical", "guanping");
            config[111009] = new HeroConfig(111009, "赵云", 91, 96, 76, 65, 81, 409, 300, 11, "北平", 10, 17, 0, 0, 1294, 0, 1, "马", null, "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun");
            config[106013] = new HeroConfig(106013, "阎行", 72, 84, 61, 58, 69, 344, 300, 6, "天水", 10, 17, 0, 0, 414, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110002] = new HeroConfig(110002, "纪灵", 78, 83, 51, 48, 55, 315, 300, 10, "寿春", 10, 17, 0, 0, 249, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108021] = new HeroConfig(108021, "冷苞", 71, 82, 68, 37, 23, 281, 250, 8, "建宁", 10, 17, 0, 0, 137, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100002] = new HeroConfig(100002, "曹操", 98, 81, 91, 94, 96, 460, 240, 2, "许昌", 10, 17, 0, 0, 0, 100, 1, "帅", null, "识", "", "core", "SwordHitYellowCritical", "caocao");
            config[102001] = new HeroConfig(102001, "郭嘉", 72, 43, 98, 78, 82, 373, 270, 2, "宛", 10, 17, 15, 0, 689, 0, 3, "谋", null, "天", "", "inte", "LightningExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 91, 91, 57, 68, 80, 387, 305, 2, "许昌", 10, 17, 0, 0, 880, 0, 1, "车", null, "青", "", "atk", "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 67, 47, 95, 100, 93, 402, 285, 2, "许昌", 10, 17, 18, 0, 1145, 0, 3, "相", null, "国", "", "help", "FrostExplosionBlue", "xunyu");
            config[102004] = new HeroConfig(102004, "许褚", 65, 96, 36, 20, 59, 276, 390, 2, "许昌", 10, 17, 0, 0, 125, 0, 1, "士", null, "斧", "", "atk", "SwordHitYellowCritical", "xuchu");
            config[102005] = new HeroConfig(102005, "夏侯渊", 90, 89, 54, 40, 80, 353, 230, 2, "小沛", 10, 17, 22, 1.5f, 485, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan");
            config[102006] = new HeroConfig(102006, "典韦", 59, 95, 43, 38, 78, 313, 415, 2, "许昌", 10, 17, 0, 0, 240, 0, 1, "士", null, "护", "", "def", "SwordHitYellowCritical", "dianwei");
            config[102007] = new HeroConfig(102007, "徐晃", 90, 91, 74, 68, 71, 394, 215, 2, "下邳", 10, 17, 22, 1.5f, 995, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "xuhuang");
            config[102008] = new HeroConfig(102008, "荀攸", 63, 53, 93, 91, 73, 373, 270, 2, "许昌", 10, 17, 15, 0, 689, 0, 3, "谋", null, "百", "米", "inte", "FrostExplosionBlue", "xunyou");
            config[102009] = new HeroConfig(102009, "于禁", 80, 75, 68, 55, 51, 329, 280, 2, "下邳", 10, 17, 0, 0, 318, 0, 1, "戟", null, "青", "破", "def", "SwordHitYellowCritical", "yujin");
            config[102010] = new HeroConfig(102010, "曹仁", 90, 86, 62, 46, 76, 360, 300, 2, "宛", 10, 17, 0, 0, 548, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caoren");
            config[102011] = new HeroConfig(102011, "曹洪", 82, 83, 44, 35, 54, 298, 310, 2, "小沛", 10, 17, 0, 0, 185, 0, 1, "枪", null, "商", "", "atk", "SwordHitYellowCritical", "caohong");
            config[102012] = new HeroConfig(102012, "乐进", 80, 84, 50, 49, 63, 326, 280, 2, "北海", 10, 17, 0, 0, 302, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "lejin");
            config[102013] = new HeroConfig(102013, "文聘", 80, 82, 65, 75, 78, 380, 335, 2, "北海", 10, 17, 0, 0, 779, 0, 1, "戟", null, "透", "劫", "atk", "SwordHitYellowCritical", "wenpin");
            config[102014] = new HeroConfig(102014, "曹休", 73, 73, 58, 56, 67, 327, 250, 2, "宛", 10, 17, 22, 1.5f, 307, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "caoxiu");
            config[102015] = new HeroConfig(102015, "郝昭", 87, 79, 74, 59, 69, 368, 300, 2, "北海", 10, 17, 0, 0, 631, 0, 1, "枪", null, "坚", "", "def", "SwordHitYellowCritical", "haozhao");
            config[102016] = new HeroConfig(102016, "程昱", 63, 54, 87, 79, 56, 339, 240, 2, "濮阳", 10, 17, 15, 0, 379, 0, 3, "谋", null, "识", "火", "inte", "StormExplosion", "chengyu");
            config[102017] = new HeroConfig(102017, "杨修", 10, 4, 83, 80, 43, 220, 260, 2, "小沛", 10, 17, 18, 0, 47, 0, 3, "相", null, "虐", "", "help", "SharpExplosionGreen", "yangxiu");
            config[102018] = new HeroConfig(102018, "牛金", 71, 77, 38, 40, 45, 271, 330, 2, "宛", 10, 17, 0, 0, 115, 0, 1, "刀", null, "伏", "", "atk", "SwordHitYellowCritical", "niujin");
            config[102019] = new HeroConfig(102019, "陈群", 65, 45, 74, 98, 73, 355, 270, 2, "许昌", 10, 17, 15, 0, 502, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "chenqun");
            config[102020] = new HeroConfig(102020, "李典", 74, 73, 79, 74, 65, 365, 300, 2, "宛", 10, 17, 0, 0, 599, 0, 1, "枪", null, "伏", "坚", "def", "SwordHitYellowCritical", "lidian");
            config[102021] = new HeroConfig(102021, "曹丕", 78, 79, 78, 84, 77, 396, 320, 2, "许昌", 10, 17, 0, 0, 1031, 0, 1, "刀", null, "敏", "", "def", "SwordHitYellowCritical", "caopi");
            config[102022] = new HeroConfig(102022, "曹植", 64, 67, 75, 65, 74, 345, 300, 2, "许昌", 10, 17, 15, 0, 421, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "caozhi");
            config[102023] = new HeroConfig(102023, "刘晔", 65, 49, 92, 75, 69, 350, 200, 2, "小沛", 10, 17, 13, 2.5f, 460, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "liuye");
            config[102024] = new HeroConfig(102024, "朱灵", 73, 77, 67, 53, 42, 312, 210, 2, "宛", 10, 17, 13, 2.5f, 236, 0, 3, "炮", null, "", "", "atk", "SwordHitYellowCritical", "zhuling");
            config[102025] = new HeroConfig(102025, "曹彰", 82, 90, 37, 32, 71, 312, 320, 2, "小沛", 10, 17, 0, 0, 236, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caozhang");
            config[102026] = new HeroConfig(102026, "满宠", 84, 64, 82, 84, 50, 364, 280, 2, "下邳", 10, 17, 0, 0, 588, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "manchong");
            config[102027] = new HeroConfig(102027, "曹冲", 31, 21, 79, 74, 78, 283, 260, 2, "许昌", 10, 17, 15, 0, 142, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "caochong");
            config[102028] = new HeroConfig(102028, "蒋济", 48, 43, 80, 73, 53, 297, 310, 2, "北海", 10, 17, 35, 0, 181, 0, 3, "鼓", null, "米", "", "help", "SoulExplosionOrange", "jiangji");
            config[102029] = new HeroConfig(102029, "甄宓", 14, 3, 69, 64, 94, 244, 290, 2, "下邳", 10, 17, 35, 0, 71, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "zhenshi");
            config[102030] = new HeroConfig(102030, "戏志才", 66, 24, 88, 75, 70, 323, 270, 2, "下邳", 10, 17, 15, 0, 286, 0, 3, "谋", null, "陷", "", "inte", "StormExplosion", "xizhicai");
            config[102032] = new HeroConfig(102032, "曹真", 82, 74, 65, 69, 84, 374, 310, 2, "北海", 10, 17, 0, 0, 701, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "caozhen");
            config[102033] = new HeroConfig(102033, "郭淮", 87, 78, 76, 71, 73, 385, 320, 2, "陈留", 10, 17, 0, 0, 850, 0, 1, "士", null, "", "", "def", "SwordHitYellowCritical", "guohuai");
            config[102034] = new HeroConfig(102034, "夏侯尚", 79, 75, 72, 63, 59, 348, 315, 2, "陈留", 10, 17, 0, 0, 444, 0, 1, "戟", null, "敏", "", "def", "SwordHitYellowCritical", "xiahoushang");
            config[102035] = new HeroConfig(102035, "钟繇", 70, 37, 72, 91, 76, 346, 290, 2, "许昌", 10, 17, 18, 0, 429, 0, 3, "相", null, "米", "", "help", "SharpExplosionGreen", "zhongyao");
            config[102044] = new HeroConfig(102044, "李通", 73, 81, 57, 63, 83, 357, 310, 2, "下邳", 10, 17, 0, 0, 520, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105012] = new HeroConfig(105012, "胡车儿", 25, 80, 40, 1, 29, 175, 260, 5, "安定", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108015] = new HeroConfig(108015, "吴兰", 62, 80, 35, 36, 50, 263, 250, 8, "梓潼", 10, 17, 0, 0, 100, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102037] = new HeroConfig(102037, "张燕", 79, 78, 51, 46, 61, 315, 310, 2, "濮阳", 10, 17, 0, 0, 249, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102046] = new HeroConfig(102046, "孙观", 72, 78, 51, 39, 66, 306, 310, 2, "陈留", 10, 17, 0, 0, 213, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103039] = new HeroConfig(103039, "贺齐", 83, 78, 42, 64, 73, 340, 310, 3, "会稽", 10, 17, 0, 0, 386, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105013] = new HeroConfig(105013, "魏续", 67, 78, 31, 32, 39, 247, 260, 5, "洛阳", 10, 17, 0, 0, 75, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108012] = new HeroConfig(108012, "雷铜", 69, 78, 51, 37, 53, 288, 250, 8, "梓潼", 10, 17, 0, 0, 155, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105014] = new HeroConfig(105014, "宋宪", 68, 77, 38, 27, 31, 241, 260, 5, "洛阳", 10, 17, 0, 0, 68, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105015] = new HeroConfig(105015, "徐荣", 80, 76, 57, 43, 42, 298, 260, 5, "洛阳", 10, 17, 0, 0, 185, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103040] = new HeroConfig(103040, "留赞", 78, 75, 64, 57, 62, 336, 310, 3, "会稽", 10, 17, 0, 0, 360, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105016] = new HeroConfig(105016, "侯成", 74, 75, 63, 56, 60, 328, 260, 5, "洛阳", 10, 17, 0, 0, 313, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[109002] = new HeroConfig(109002, "杨任", 67, 75, 51, 38, 54, 285, 300, 9, "上庸", 10, 17, 0, 0, 147, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105017] = new HeroConfig(105017, "胡轸", 65, 74, 12, 15, 21, 187, 260, 5, "洛阳", 10, 17, 0, 0, 26, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107009] = new HeroConfig(107009, "刘磐", 67, 74, 46, 42, 53, 282, 300, 7, "江陵", 10, 17, 0, 0, 139, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110001] = new HeroConfig(110001, "李丰", 69, 74, 50, 22, 47, 262, 300, 10, "寿春", 10, 17, 0, 0, 98, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102047] = new HeroConfig(102047, "夏侯德", 69, 73, 32, 40, 52, 266, 310, 2, "陈留", 10, 17, 0, 0, 105, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103047] = new HeroConfig(103047, "李异", 56, 73, 18, 17, 22, 186, 310, 3, "庐江", 10, 17, 0, 0, 26, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103037] = new HeroConfig(103037, "马忠", 64, 73, 61, 34, 36, 268, 310, 3, "桂阳", 10, 17, 0, 0, 109, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105018] = new HeroConfig(105018, "张绣", 80, 73, 60, 45, 59, 317, 260, 5, "安定", 10, 17, 0, 0, 258, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105019] = new HeroConfig(105019, "樊稠", 66, 73, 31, 24, 39, 233, 260, 5, "安定", 10, 17, 0, 0, 59, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105020] = new HeroConfig(105020, "曹性", 53, 73, 37, 26, 38, 227, 260, 5, "洛阳", 10, 17, 0, 0, 53, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108019] = new HeroConfig(108019, "吴懿", 83, 73, 68, 70, 77, 371, 250, 8, "成都", 10, 17, 0, 0, 665, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108022] = new HeroConfig(108022, "刘璝", 71, 73, 66, 44, 62, 316, 250, 8, "永安", 10, 17, 0, 0, 253, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102036] = new HeroConfig(102036, "田予", 80, 72, 80, 78, 75, 385, 310, 2, "北海", 10, 17, 0, 0, 850, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104011] = new HeroConfig(104011, "焦触", 65, 72, 33, 32, 39, 241, 310, 4, "平原", 10, 17, 0, 0, 68, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110017] = new HeroConfig(110017, "韩浩", 69, 72, 68, 88, 62, 359, 300, 10, "寿春", 10, 17, 0, 0, 539, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[101013] = new HeroConfig(101013, "陈到", 76, 71, 63, 53, 69, 332, 310, 1, "新野", 10, 17, 0, 0, 335, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102040] = new HeroConfig(102040, "曹纯", 75, 71, 60, 35, 72, 313, 310, 2, "许昌", 10, 17, 0, 0, 240, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102050] = new HeroConfig(102050, "夏侯恩", 63, 71, 51, 44, 70, 299, 310, 2, "陈留", 10, 17, 0, 0, 188, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102058] = new HeroConfig(102058, "典满", 49, 71, 38, 25, 50, 233, 310, 2, "陈留", 10, 17, 0, 0, 59, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104012] = new HeroConfig(104012, "吕翔", 54, 71, 12, 19, 28, 184, 310, 4, "邺", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106008] = new HeroConfig(106008, "马玩", 68, 71, 15, 22, 35, 211, 300, 6, "天水", 10, 17, 0, 0, 40, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106011] = new HeroConfig(106011, "程银", 67, 71, 39, 35, 49, 261, 300, 6, "天水", 10, 17, 0, 0, 96, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108020] = new HeroConfig(108020, "吴班", 74, 71, 56, 45, 66, 312, 250, 8, "成都", 10, 17, 0, 0, 236, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103025] = new HeroConfig(103025, "步骘", 72, 51, 84, 87, 65, 359, 315, 3, "长沙", 10, 17, 35, 0, 539, 0, 3, "鼓", null, "励", "", "help", "SoulExplosionOrange", "buzhi");
            config[103031] = new HeroConfig(103031, "陈武", 74, 87, 43, 40, 62, 306, 305, 3, "桂阳", 10, 17, 0, 0, 213, 0, 1, "枪", null, "劫", "", "def", "SwordHitYellowCritical", "chengwu");
            config[102054] = new HeroConfig(102054, "吕虔", 57, 70, 58, 74, 60, 319, 310, 2, "濮阳", 10, 17, 0, 0, 267, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103022] = new HeroConfig(103022, "程普", 80, 75, 73, 71, 81, 380, 300, 3, "庐江", 10, 17, 0, 0, 779, 0, 1, "戟", null, "实", "奋", "def", "SwordHitYellowCritical", "chengpu");
            config[103014] = new HeroConfig(103014, "大乔", 17, 11, 72, 78, 92, 270, 280, 3, "吴", 10, 17, 15, 0, 113, 0, 3, "乐", null, "碉", "陷", "help", "StormExplosion", "daqiao");
            config[103016] = new HeroConfig(103016, "丁奉", 76, 95, 66, 51, 52, 340, 205, 3, "长沙", 10, 17, 13, 2.5f, 386, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "dingfeng");
            config[103017] = new HeroConfig(103017, "董袭", 72, 85, 50, 48, 60, 315, 325, 3, "长沙", 10, 17, 0, 0, 249, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "dongxi");
            config[103001] = new HeroConfig(103001, "甘宁", 93, 94, 76, 70, 80, 413, 180, 3, "长沙", 10, 17, 22, 1.5f, 1388, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "ganning");
            config[103024] = new HeroConfig(103024, "顾雍", 43, 18, 80, 92, 76, 309, 300, 3, "桂阳", 10, 17, 15, 0, 224, 0, 3, "扇", null, "连", "", "inte", "FanExplosion", "guyong");
            config[103027] = new HeroConfig(103027, "韩当", 75, 84, 54, 49, 67, 329, 300, 3, "柴桑", 10, 17, 0, 0, 318, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "handang");
            config[103044] = new HeroConfig(103044, "吕岱", 81, 70, 68, 74, 62, 355, 310, 3, "柴桑", 10, 17, 0, 0, 502, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103003] = new HeroConfig(103003, "黄盖", 79, 83, 65, 65, 80, 372, 320, 3, "桂阳", 10, 17, 0, 0, 677, 0, 1, "士", null, "奋", "", "def", "SwordHitYellowCritical", "huanggai");
            config[103049] = new HeroConfig(103049, "孙皎", 75, 70, 64, 69, 71, 349, 310, 3, "建业", 10, 17, 0, 0, 452, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103007] = new HeroConfig(103007, "蒋钦", 78, 84, 51, 42, 74, 329, 310, 3, "桂阳", 10, 17, 0, 0, 318, 0, 1, "戟", null, "", "", "atk", "SwordHitYellowCritical", "jiangqing");
            config[103026] = new HeroConfig(103026, "阚泽", 42, 48, 87, 85, 71, 333, 260, 3, "长沙", 10, 17, 15, 0, 341, 0, 3, "谋", null, "炽", "", "inte", "StormExplosion", "kanze");
            config[103045] = new HeroConfig(103045, "孙瑜", 77, 70, 68, 69, 78, 362, 310, 3, "柴桑", 10, 17, 0, 0, 568, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103018] = new HeroConfig(103018, "凌统", 72, 83, 54, 37, 66, 312, 205, 3, "建业", 10, 17, 30, 1.5f, 236, 0, 3, "弩", null, "虐", "", "shoot", "BulletExplosionBlue", "lingtong");
            config[103048] = new HeroConfig(103048, "张承", 77, 70, 75, 74, 74, 370, 310, 3, "建业", 10, 17, 0, 0, 653, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103005] = new HeroConfig(103005, "鲁肃", 80, 56, 92, 92, 89, 409, 250, 3, "柴桑", 10, 17, 18, 0, 1294, 0, 3, "相", null, "商", "雷", "help", "SharpExplosionGreen", "lusu");
            config[104013] = new HeroConfig(104013, "吕旷", 56, 70, 13, 22, 29, 190, 310, 4, "南皮", 10, 17, 0, 0, 27, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103009] = new HeroConfig(103009, "陆逊", 96, 69, 92, 87, 90, 434, 220, 3, "吴", 10, 17, 15, 0, 2006, 0, 3, "谋", null, "炎", "", "inte", "GasExplosionFire", "luxun");
            config[106005] = new HeroConfig(106005, "成宜", 72, 70, 40, 47, 54, 283, 300, 6, "天水", 10, 17, 0, 0, 142, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107003] = new HeroConfig(107003, "王威", 60, 70, 59, 52, 66, 307, 300, 7, "江夏", 10, 17, 0, 0, 216, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107019] = new HeroConfig(107019, "蔡瑁", 77, 70, 77, 72, 62, 358, 300, 7, "襄阳", 10, 17, 0, 0, 529, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103008] = new HeroConfig(103008, "吕蒙", 91, 81, 84, 74, 82, 412, 250, 3, "柴桑", 10, 17, 0, 0, 1364, 0, 1, "马", null, "学", "羽", "def", "SwordHitYellowCritical", "lvmeng");
            config[110011] = new HeroConfig(110011, "雷薄", 62, 70, 36, 11, 15, 194, 300, 10, "寿春", 10, 17, 0, 0, 29, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103019] = new HeroConfig(103019, "潘璋", 76, 80, 70, 28, 12, 266, 295, 3, "庐江", 10, 17, 0, 0, 105, 0, 1, "戟", null, "刺", "虐", "def", "SwordHitYellowCritical", "panzhang");
            config[103030] = new HeroConfig(103030, "全琮", 75, 69, 68, 59, 64, 335, 310, 3, "吴", 10, 17, 0, 0, 354, 0, 1, "戟", null, "实", "", "def", "SwordHitYellowCritical", "quanzong");
            config[104014] = new HeroConfig(104014, "张南", 55, 69, 45, 33, 33, 235, 310, 4, "南皮", 10, 17, 0, 0, 61, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103029] = new HeroConfig(103029, "苏飞", 69, 63, 66, 72, 70, 340, 330, 3, "长沙", 10, 17, 0, 0, 386, 0, 1, "刀", null, "复", "", "def", "SwordHitYellowCritical", "sufei");
            config[103034] = new HeroConfig(103034, "孙桓", 82, 73, 76, 75, 76, 382, 315, 3, "吴", 10, 17, 0, 0, 806, 0, 1, "戟", null, "竟", "", "def", "SwordHitYellowCritical", "sunhuan");
            config[105021] = new HeroConfig(105021, "李肃", 46, 69, 59, 15, 36, 225, 260, 5, "洛阳", 10, 17, 0, 0, 51, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[109003] = new HeroConfig(109003, "杨昂", 65, 69, 36, 33, 40, 243, 300, 9, "汉中", 10, 17, 0, 0, 70, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110007] = new HeroConfig(110007, "梁纲", 60, 69, 41, 22, 46, 238, 300, 10, "寿春", 10, 17, 0, 0, 64, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100003] = new HeroConfig(100003, "孙权", 74, 65, 78, 87, 93, 397, 350, 3, "吴", 10, 17, 0, 0, 0, 100, 1, "帅", null, "衡", "", "core", "SwordHitYellowCritical", "sunquan");
            config[103012] = new HeroConfig(103012, "孙尚香", 69, 83, 64, 61, 70, 347, 240, 3, "吴", 10, 17, 22, 1.5f, 436, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "sunshangxiang");
            config[103033] = new HeroConfig(103033, "孙韶", 76, 75, 71, 65, 70, 357, 310, 3, "吴", 10, 17, 0, 0, 520, 0, 1, "戟", null, "破", "", "def", "SwordHitYellowCritical", "sunshao");
            config[110009] = new HeroConfig(110009, "陈兰", 66, 69, 40, 24, 38, 237, 300, 10, "寿春", 10, 17, 0, 0, 63, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103002] = new HeroConfig(103002, "太史慈", 85, 93, 66, 58, 79, 381, 230, 3, "吴", 10, 17, 22, 1.5f, 792, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "taishici");
            config[111001] = new HeroConfig(111001, "公孙范", 73, 69, 64, 62, 61, 329, 300, 11, "北平", 10, 17, 0, 0, 318, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[111004] = new HeroConfig(111004, "田豫", 77, 69, 77, 75, 72, 370, 300, 11, "蓟", 10, 17, 0, 0, 653, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104015] = new HeroConfig(104015, "眭元进", 52, 68, 45, 32, 49, 246, 310, 4, "邺", 10, 17, 0, 0, 74, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103015] = new HeroConfig(103015, "小乔", 16, 12, 73, 77, 92, 270, 270, 3, "柴桑", 10, 17, 15, 0, 113, 0, 3, "乐", null, "曲", "陷", "help", "StormExplosion", "xiaoqiao");
            config[103021] = new HeroConfig(103021, "徐盛", 87, 81, 78, 65, 73, 384, 290, 3, "庐江", 10, 17, 0, 0, 835, 0, 1, "士", null, "乱", "", "def", "SwordHitYellowCritical", "xusheng");
            config[106004] = new HeroConfig(106004, "成公英", 70, 68, 76, 60, 65, 339, 300, 6, "天水", 10, 17, 0, 0, 379, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103035] = new HeroConfig(103035, "严畯", 13, 2, 70, 85, 72, 242, 290, 3, "长沙", 10, 17, 15, 0, 69, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "yanjun");
            config[108014] = new HeroConfig(108014, "杨怀", 62, 68, 68, 62, 53, 313, 250, 8, "成都", 10, 17, 0, 0, 240, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110006] = new HeroConfig(110006, "张勋", 72, 68, 41, 39, 59, 279, 300, 10, "汝南", 10, 17, 0, 0, 132, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103023] = new HeroConfig(103023, "张纮", 23, 21, 83, 94, 79, 300, 250, 3, "会稽", 10, 17, 18, 0, 191, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "zhanghong");
            config[103010] = new HeroConfig(103010, "张昭", 32, 2, 83, 98, 79, 294, 275, 3, "桂阳", 10, 17, 18, 0, 172, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "zhangzhao");
            config[103004] = new HeroConfig(103004, "周泰", 76, 91, 48, 38, 60, 313, 350, 3, "长沙", 10, 17, 0, 0, 240, 0, 1, "枪", null, "连", "", "atk", "SwordHitYellowCritical", "zhoutai");
            config[103006] = new HeroConfig(103006, "周瑜", 96, 71, 96, 86, 93, 442, 210, 3, "柴桑", 10, 17, 15, 0, 2308, 0, 3, "谋", null, "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu");
            config[103013] = new HeroConfig(103013, "朱桓", 84, 82, 75, 56, 59, 356, 320, 3, "桂阳", 10, 17, 0, 0, 511, 0, 1, "枪", null, "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan");
            config[103032] = new HeroConfig(103032, "朱然", 77, 67, 69, 58, 73, 344, 315, 3, "吴", 10, 17, 0, 0, 414, 0, 1, "枪", null, "竟", "", "def", "SwordHitYellowCritical", "zhuran");
            config[103020] = new HeroConfig(103020, "朱治", 66, 78, 42, 39, 64, 289, 235, 3, "柴桑", 10, 17, 22, 1.5f, 158, 0, 3, "弓", null, "敏", "", "shoot", "BulletExplosionBlue", "zhuzhi");
            config[103011] = new HeroConfig(103011, "诸葛瑾", 72, 34, 81, 90, 90, 367, 290, 3, "吴", 10, 17, 15, 0, 620, 0, 3, "扇", null, "励", "", "help", "FanExplosion", "zhugejin");
            config[100004] = new HeroConfig(100004, "袁绍", 86, 73, 70, 73, 90, 392, 310, 4, "邺", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshao");
            config[104001] = new HeroConfig(104001, "张郃", 89, 90, 69, 56, 70, 374, 300, 4, "邺", 10, 17, 0, 0, 701, 0, 1, "车", null, "分", "", "def", "SwordHitYellowCritical", "zhanghe");
            config[104002] = new HeroConfig(104002, "颜良", 78, 93, 42, 32, 53, 298, 340, 4, "南皮", 10, 17, 0, 0, 185, 0, 1, "车", null, "破", "", "atk", "SwordHitYellowCritical", "yanliang");
            config[104003] = new HeroConfig(104003, "文丑", 79, 92, 48, 52, 68, 339, 355, 4, "晋阳", 10, 17, 0, 0, 379, 0, 1, "车", null, "刺", "", "def", "SwordHitYellowCritical", "wenchou");
            config[104004] = new HeroConfig(104004, "田丰", 72, 33, 93, 89, 64, 351, 250, 4, "晋阳", 10, 17, 15, 0, 468, 0, 3, "谋", null, "雷", "", "inte", "StormExplosion", "tianfeng");
            config[104005] = new HeroConfig(104005, "鞠义", 72, 78, 55, 18, 37, 260, 250, 4, "平原", 10, 17, 22, 1.5f, 95, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "juyi");
            config[104006] = new HeroConfig(104006, "许攸", 39, 29, 80, 57, 23, 228, 285, 4, "邺", 10, 17, 15, 0, 54, 0, 3, "谋", null, "火", "", "inte", "StormExplosion", "xuyou");
            config[104007] = new HeroConfig(104007, "高览", 76, 82, 66, 55, 62, 341, 305, 4, "邺", 10, 17, 0, 0, 393, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "gaolan");
            config[104008] = new HeroConfig(104008, "沮授", 78, 35, 90, 91, 74, 368, 260, 4, "邺", 10, 17, 15, 0, 631, 0, 3, "谋", null, "静", "", "inte", "StormExplosion", "jushou");
            config[104009] = new HeroConfig(104009, "郭图", 52, 50, 82, 70, 37, 291, 290, 4, "南皮", 10, 17, 15, 0, 163, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "guotu");
            config[111006] = new HeroConfig(111006, "单经", 71, 68, 43, 49, 54, 285, 300, 11, "北平", 10, 17, 0, 0, 147, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104016] = new HeroConfig(104016, "牵招", 70, 67, 71, 73, 71, 352, 310, 4, "平原", 10, 17, 0, 0, 476, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106007] = new HeroConfig(106007, "马休", 63, 67, 44, 41, 63, 278, 300, 6, "武威", 10, 17, 0, 0, 130, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107011] = new HeroConfig(107011, "张允", 72, 67, 42, 56, 48, 285, 300, 7, "襄阳", 10, 17, 0, 0, 147, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110014] = new HeroConfig(110014, "桥蕤", 62, 67, 37, 40, 56, 262, 300, 10, "寿春", 10, 17, 0, 0, 98, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104017] = new HeroConfig(104017, "袁尚", 58, 66, 39, 35, 66, 264, 310, 4, "南皮", 10, 17, 0, 0, 102, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104018] = new HeroConfig(104018, "袁谭", 57, 66, 28, 33, 56, 240, 310, 4, "晋阳", 10, 17, 0, 0, 67, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110013] = new HeroConfig(110013, "乐就", 53, 66, 58, 42, 53, 272, 300, 10, "汝南", 10, 17, 0, 0, 117, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103038] = new HeroConfig(103038, "贾华", 49, 65, 71, 29, 52, 266, 310, 3, "建业", 10, 17, 0, 0, 105, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104019] = new HeroConfig(104019, "周昂", 74, 65, 62, 50, 62, 313, 310, 4, "晋阳", 10, 17, 0, 0, 240, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105022] = new HeroConfig(105022, "张济", 69, 65, 51, 52, 54, 291, 260, 5, "安定", 10, 17, 0, 0, 163, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105023] = new HeroConfig(105023, "朱儁", 78, 65, 70, 74, 73, 360, 260, 5, "洛阳", 10, 17, 0, 0, 548, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105024] = new HeroConfig(105024, "杨奉", 66, 65, 31, 14, 58, 234, 260, 5, "洛阳", 10, 17, 0, 0, 60, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107005] = new HeroConfig(107005, "黄祖", 73, 65, 52, 37, 31, 258, 300, 7, "江夏", 10, 17, 0, 0, 91, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110008] = new HeroConfig(110008, "陈纪", 58, 65, 43, 48, 32, 246, 300, 10, "汝南", 10, 17, 0, 0, 74, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[111005] = new HeroConfig(111005, "田楷", 68, 65, 56, 61, 63, 313, 300, 11, "蓟", 10, 17, 0, 0, 240, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102038] = new HeroConfig(102038, "陈登", 79, 64, 81, 82, 61, 367, 310, 2, "小沛", 10, 17, 0, 0, 620, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102041] = new HeroConfig(102041, "王凌", 74, 64, 70, 82, 71, 361, 310, 2, "下邳", 10, 17, 0, 0, 558, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104020] = new HeroConfig(104020, "田畴", 64, 64, 70, 75, 72, 345, 310, 4, "晋阳", 10, 17, 0, 0, 421, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104021] = new HeroConfig(104021, "淳于琼", 70, 64, 28, 28, 35, 225, 310, 4, "晋阳", 10, 17, 0, 0, 51, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[111003] = new HeroConfig(111003, "王门", 65, 64, 31, 41, 49, 250, 300, 11, "蓟", 10, 17, 0, 0, 79, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103036] = new HeroConfig(103036, "吕范", 73, 63, 74, 77, 69, 356, 310, 3, "庐江", 10, 17, 0, 0, 511, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104022] = new HeroConfig(104022, "吕威璜", 58, 63, 29, 39, 44, 233, 310, 4, "平原", 10, 17, 0, 0, 59, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106010] = new HeroConfig(106010, "梁兴", 59, 63, 18, 21, 25, 186, 300, 6, "天水", 10, 17, 0, 0, 26, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[109001] = new HeroConfig(109001, "张卫", 71, 63, 43, 42, 58, 277, 300, 9, "汉中", 10, 17, 0, 0, 128, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100005] = new HeroConfig(100005, "董卓", 77, 87, 67, 18, 36, 285, 330, 5, "洛阳", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "dongzhuo");
            config[105001] = new HeroConfig(105001, "张辽", 95, 92, 78, 56, 76, 397, 295, 5, "长安", 10, 17, 0, 0, 1049, 0, 1, "马", null, "旋", "", "def", "SwordHitYellowCritical", "zhangliao");
            config[105002] = new HeroConfig(105002, "吕布", 91, 100, 26, 13, 40, 270, 310, 5, "洛阳", 10, 17, 0, 0, 113, 0, 1, "车", null, "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu");
            config[105003] = new HeroConfig(105003, "华雄", 82, 90, 56, 40, 57, 325, 310, 5, "洛阳", 10, 17, 0, 0, 297, 0, 1, "车", null, "纷", "", "atk", "SwordHitYellowCritical", "huaxiong");
            config[105004] = new HeroConfig(105004, "贾诩", 86, 50, 97, 85, 57, 375, 260, 5, "安定", 10, 17, 15, 0, 713, 0, 3, "谋", null, "延", "", "inte", "StormExplosion", "jiaxu");
            config[105005] = new HeroConfig(105005, "貂蝉", 27, 65, 81, 70, 95, 338, 280, 5, "洛阳", 10, 17, 15, 0, 373, 0, 3, "乐", null, "曲", "", "help", "StormExplosion", "diaochan");
            config[105006] = new HeroConfig(105006, "臧霸", 78, 75, 53, 70, 75, 351, 330, 5, "长安", 10, 17, 0, 0, 468, 0, 1, "马", null, "虐", "", "atk", "SwordHitYellowCritical", "zangba");
            config[105007] = new HeroConfig(105007, "高顺", 85, 86, 54, 45, 68, 338, 215, 5, "长安", 10, 17, 13, 2.5f, 373, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "gaoshun");
            config[105008] = new HeroConfig(105008, "李儒", 63, 43, 91, 78, 38, 313, 250, 5, "洛阳", 10, 17, 15, 0, 240, 0, 3, "谋", null, "火", "", "inte", "ShadowExplosion", "liru");
            config[105009] = new HeroConfig(105009, "李傕", 69, 74, 24, 1, 17, 185, 370, 5, "安定", 10, 17, 0, 0, 25, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "lijue");
            config[105010] = new HeroConfig(105010, "郭汜", 64, 76, 13, 14, 13, 180, 390, 5, "安定", 10, 17, 0, 0, 25, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guosi");
            config[105011] = new HeroConfig(105011, "陈宫", 84, 55, 89, 82, 70, 380, 260, 5, "洛阳", 10, 17, 15, 0, 779, 0, 3, "谋", null, "励", "溃", "inte", "ShadowExplosion", "chengong");
            config[110012] = new HeroConfig(110012, "刘勋", 47, 63, 35, 16, 32, 193, 300, 10, "寿春", 10, 17, 0, 0, 29, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[111002] = new HeroConfig(111002, "公孙续", 59, 63, 50, 59, 62, 293, 300, 11, "北平", 10, 17, 0, 0, 169, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[111007] = new HeroConfig(111007, "邹丹", 60, 63, 33, 36, 38, 230, 300, 11, "北平", 10, 17, 0, 0, 56, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[112002] = new HeroConfig(112002, "公孙康", 69, 63, 58, 55, 53, 298, 300, 12, "襄平", 10, 17, 0, 0, 185, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103051] = new HeroConfig(103051, "太史享", 53, 62, 45, 55, 56, 271, 310, 3, "建业", 10, 17, 0, 0, 115, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106006] = new HeroConfig(106006, "侯选", 59, 62, 32, 52, 49, 254, 300, 6, "天水", 10, 17, 0, 0, 85, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[101012] = new HeroConfig(101012, "糜芳", 54, 61, 32, 23, 23, 193, 310, 1, "新野", 10, 17, 0, 0, 29, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102039] = new HeroConfig(102039, "贾逵", 78, 61, 84, 85, 75, 383, 310, 2, "北海", 10, 17, 0, 0, 821, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106012] = new HeroConfig(106012, "杨秋", 64, 61, 55, 61, 40, 281, 300, 6, "天水", 10, 17, 0, 0, 137, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107008] = new HeroConfig(107008, "吴巨", 49, 61, 23, 51, 54, 238, 300, 7, "江陵", 10, 17, 0, 0, 64, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108013] = new HeroConfig(108013, "高沛", 66, 61, 69, 57, 52, 305, 250, 8, "梓潼", 10, 17, 0, 0, 209, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104023] = new HeroConfig(104023, "审配", 84, 60, 83, 73, 70, 370, 310, 4, "平原", 10, 17, 0, 0, 653, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105025] = new HeroConfig(105025, "牛辅", 38, 60, 21, 26, 37, 182, 260, 5, "安定", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105026] = new HeroConfig(105026, "董旻", 49, 60, 25, 12, 23, 169, 260, 5, "长安", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107007] = new HeroConfig(107007, "苏飞", 66, 60, 63, 59, 60, 308, 300, 7, "零陵", 10, 17, 0, 0, 220, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104024] = new HeroConfig(104024, "韩莒子", 51, 59, 51, 46, 52, 259, 310, 4, "邺", 10, 17, 0, 0, 93, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104025] = new HeroConfig(104025, "苏由", 50, 59, 49, 41, 49, 248, 310, 4, "晋阳", 10, 17, 0, 0, 77, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104026] = new HeroConfig(104026, "蒋义渠", 71, 58, 57, 51, 55, 292, 310, 4, "邺", 10, 17, 0, 0, 166, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105027] = new HeroConfig(105027, "皇甫嵩", 83, 58, 70, 48, 69, 328, 260, 5, "安定", 10, 17, 0, 0, 313, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100006] = new HeroConfig(100006, "马腾", 82, 80, 51, 58, 88, 359, 290, 6, "武威", 10, 17, 0, 0, 539, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "mateng");
            config[106001] = new HeroConfig(106001, "马超", 92, 97, 42, 25, 80, 336, 335, 6, "武威", 10, 17, 0, 0, 360, 0, 1, "马", null, "铁", "", "atk", "SwordHitWhiteCritical", "machao");
            config[106002] = new HeroConfig(106002, "马岱", 80, 85, 66, 45, 68, 344, 300, 6, "武威", 10, 17, 0, 0, 414, 0, 1, "马", null, "坚", "羽", "atk", "SwordHitYellowCritical", "madai");
            config[106003] = new HeroConfig(106003, "庞德", 89, 94, 67, 43, 67, 360, 280, 6, "武威", 10, 17, 0, 0, 548, 0, 1, "枪", null, "坚", "", "atk", "SwordHitYellowCritical", "pangde");
            config[106009] = new HeroConfig(106009, "马铁", 65, 57, 52, 48, 57, 279, 300, 6, "武威", 10, 17, 0, 0, 132, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[101010] = new HeroConfig(101010, "胡班", 54, 54, 57, 49, 61, 275, 310, 1, "新野", 10, 17, 0, 0, 123, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102070] = new HeroConfig(102070, "崔琰", 17, 54, 69, 84, 74, 298, 310, 2, "濮阳", 10, 17, 0, 0, 185, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103050] = new HeroConfig(103050, "宋谦", 69, 54, 70, 73, 73, 339, 310, 3, "建业", 10, 17, 0, 0, 379, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104027] = new HeroConfig(104027, "高干", 72, 54, 47, 57, 64, 294, 310, 4, "晋阳", 10, 17, 0, 0, 172, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103053] = new HeroConfig(103053, "骆统", 69, 53, 69, 70, 70, 331, 310, 3, "吴", 10, 17, 0, 0, 330, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105028] = new HeroConfig(105028, "董承", 56, 53, 65, 63, 75, 312, 260, 5, "长安", 10, 17, 0, 0, 236, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107016] = new HeroConfig(107016, "蔡中", 39, 52, 1, 21, 42, 155, 300, 7, "襄阳", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[111008] = new HeroConfig(111008, "关靖", 36, 52, 72, 65, 42, 267, 300, 11, "北平", 10, 17, 0, 0, 107, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102049] = new HeroConfig(102049, "刘馥", 64, 49, 73, 89, 83, 358, 310, 2, "陈留", 10, 17, 0, 0, 529, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[106014] = new HeroConfig(106014, "韩遂", 84, 75, 77, 61, 80, 377, 290, 6, "天水", 10, 17, 0, 0, 739, 0, 1, "马", null, "乱", "", "def", "SwordHitYellowCritical", "hansui");
            config[100007] = new HeroConfig(100007, "刘表", 46, 31, 68, 81, 80, 306, 310, 7, "襄阳", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "liubiao");
            config[107001] = new HeroConfig(107001, "魏延", 84, 91, 67, 68, 55, 365, 300, 7, "武陵", 10, 17, 0, 0, 599, 0, 1, "戟", null, "破", "乱", "atk", "SwordHitYellowCritical", "weiyan");
            config[107002] = new HeroConfig(107002, "黄忠", 88, 93, 64, 52, 75, 372, 210, 7, "武陵", 10, 17, 22, 1.5f, 677, 0, 3, "弓", null, "矢", "速", "shoot", "BulletExplosionFire", "huangzhong");
            config[107018] = new HeroConfig(107018, "蔡和", 38, 49, 1, 25, 44, 157, 300, 7, "襄阳", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103042] = new HeroConfig(103042, "陆绩", 19, 48, 61, 69, 41, 238, 310, 3, "会稽", 10, 17, 0, 0, 64, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104028] = new HeroConfig(104028, "袁熙", 62, 47, 59, 61, 60, 289, 310, 4, "平原", 10, 17, 0, 0, 158, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110005] = new HeroConfig(110005, "袁燿", 38, 47, 38, 48, 49, 220, 300, 10, "寿春", 10, 17, 0, 0, 47, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103041] = new HeroConfig(103041, "虞翻", 43, 46, 86, 83, 46, 304, 310, 3, "会稽", 10, 17, 0, 0, 205, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103052] = new HeroConfig(103052, "孙匡", 49, 44, 45, 62, 53, 253, 310, 3, "吴", 10, 17, 0, 0, 84, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104029] = new HeroConfig(104029, "陈震", 44, 44, 65, 73, 70, 296, 310, 4, "南皮", 10, 17, 0, 0, 178, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108023] = new HeroConfig(108023, "刘循", 61, 44, 39, 48, 55, 247, 250, 8, "永安", 10, 17, 0, 0, 75, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104030] = new HeroConfig(104030, "辛评", 69, 43, 76, 75, 68, 331, 310, 4, "邺", 10, 17, 0, 0, 330, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[109005] = new HeroConfig(109005, "杨柏", 42, 43, 18, 25, 20, 148, 300, 9, "上庸", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108017] = new HeroConfig(108017, "王甫", 62, 41, 79, 79, 73, 334, 250, 8, "江州", 10, 17, 0, 0, 347, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102043] = new HeroConfig(102043, "梁习", 73, 40, 73, 87, 80, 353, 310, 2, "宛", 10, 17, 0, 0, 485, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103055] = new HeroConfig(103055, "孙朗", 32, 40, 28, 38, 42, 180, 310, 3, "柴桑", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103046] = new HeroConfig(103046, "吾粲", 66, 40, 76, 73, 70, 325, 310, 3, "柴桑", 10, 17, 0, 0, 297, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104031] = new HeroConfig(104031, "高柔", 54, 40, 67, 75, 70, 306, 310, 4, "南皮", 10, 17, 0, 0, 213, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102052] = new HeroConfig(102052, "毛玠", 62, 38, 58, 79, 60, 297, 310, 2, "下邳", 10, 17, 0, 0, 181, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100008] = new HeroConfig(100008, "刘璋", 15, 4, 8, 37, 64, 128, 310, 8, "成都", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "liuzhang");
            config[108001] = new HeroConfig(108001, "严颜", 79, 83, 69, 64, 79, 374, 280, 8, "建宁", 10, 17, 0, 0, 701, 0, 1, "士", null, "敏", "", "def", "SwordHitYellowCritical", "yanyan");
            config[108002] = new HeroConfig(108002, "李严", 82, 83, 75, 80, 70, 390, 290, 8, "建宁", 10, 17, 0, 0, 928, 0, 1, "士", null, "实", "", "def", "SwordHitYellowCritical", "liyan");
            config[108003] = new HeroConfig(108003, "张松", 15, 6, 88, 83, 19, 211, 240, 8, "成都", 10, 17, 15, 0, 40, 0, 3, "扇", null, "", "", "inte", "FanExplosion", "zhangsong");
            config[108004] = new HeroConfig(108004, "董允", 67, 65, 85, 88, 83, 388, 280, 8, "江州", 10, 17, 15, 0, 896, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "dongyun");
            config[108005] = new HeroConfig(108005, "孟获", 87, 87, 51, 55, 75, 355, 310, 8, "云南", 10, 17, 0, 0, 502, 0, 1, "刀", null, "藤", "", "atk", "SwordHitYellowCritical", "menghuo");
            config[108006] = new HeroConfig(108006, "祝融", 77, 85, 43, 50, 78, 333, 290, 8, "云南", 10, 17, 0, 0, 341, 0, 1, "刀", null, "藤", "", "def", "SwordHitYellowCritical", "zhurong");
            config[108007] = new HeroConfig(108007, "法正", 83, 52, 94, 79, 55, 363, 275, 8, "江州", 10, 17, 15, 0, 578, 0, 3, "谋", null, "溃", "", "inte", "GasExplosionFire", "fazheng");
            config[108008] = new HeroConfig(108008, "黄权", 75, 59, 82, 81, 78, 375, 310, 8, "建宁", 10, 17, 0, 0, 713, 0, 1, "枪", null, "缓", "", "atk", "SwordHitYellowCritical", "huangquan");
            config[108009] = new HeroConfig(108009, "孟达", 75, 73, 74, 67, 72, 361, 250, 8, "江州", 10, 17, 22, 1.5f, 558, 0, 3, "弓", null, "乱", "", "shoot", "BulletExplosionBlue", "mengda");
            config[108010] = new HeroConfig(108010, "李恢", 79, 65, 79, 81, 78, 382, 315, 8, "建宁", 10, 17, 0, 0, 806, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "lihui");
            config[108011] = new HeroConfig(108011, "张任", 88, 84, 78, 59, 76, 385, 210, 8, "建宁", 10, 17, 22, 1.5f, 850, 0, 3, "弓", null, "复", "", "shoot", "BulletExplosionBlue", "zhangren");
            config[102051] = new HeroConfig(102051, "温恢", 62, 36, 73, 86, 76, 333, 310, 2, "下邳", 10, 17, 0, 0, 341, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108016] = new HeroConfig(108016, "庞羲", 58, 36, 65, 71, 55, 285, 250, 8, "梓潼", 10, 17, 0, 0, 147, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102042] = new HeroConfig(102042, "张既", 74, 35, 75, 89, 81, 354, 310, 2, "宛", 10, 17, 0, 0, 493, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102063] = new HeroConfig(102063, "李孚", 30, 35, 73, 72, 68, 278, 310, 2, "下邳", 10, 17, 0, 0, 130, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102060] = new HeroConfig(102060, "王朗", 46, 34, 79, 84, 51, 294, 310, 2, "宛", 10, 17, 0, 0, 172, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108018] = new HeroConfig(108018, "董和", 57, 34, 74, 88, 76, 329, 250, 8, "江州", 10, 17, 0, 0, 318, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105029] = new HeroConfig(105029, "华歆", 18, 33, 82, 84, 17, 234, 260, 5, "长安", 10, 17, 0, 0, 60, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107012] = new HeroConfig(107012, "蒯良", 68, 33, 88, 83, 71, 343, 300, 7, "襄阳", 10, 17, 0, 0, 407, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102048] = new HeroConfig(102048, "杜畿", 66, 32, 74, 87, 76, 335, 310, 2, "陈留", 10, 17, 0, 0, 354, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102055] = new HeroConfig(102055, "徐邈", 55, 32, 67, 82, 79, 315, 310, 2, "宛", 10, 17, 0, 0, 249, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102069] = new HeroConfig(102069, "华歆", 17, 32, 80, 85, 16, 230, 310, 2, "濮阳", 10, 17, 0, 0, 56, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108024] = new HeroConfig(108024, "王累", 28, 30, 78, 81, 73, 290, 250, 8, "成都", 10, 17, 0, 0, 160, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[101011] = new HeroConfig(101011, "糜竺", 33, 29, 77, 85, 85, 309, 310, 1, "新野", 10, 17, 0, 0, 224, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102071] = new HeroConfig(102071, "吴质", 16, 29, 68, 57, 37, 207, 310, 2, "小沛", 10, 17, 0, 0, 37, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110016] = new HeroConfig(110016, "韩胤", 26, 29, 64, 55, 44, 218, 300, 10, "汝南", 10, 17, 0, 0, 45, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108026] = new HeroConfig(108026, "费诗", 15, 28, 64, 75, 66, 248, 250, 8, "成都", 10, 17, 0, 0, 77, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100009] = new HeroConfig(100009, "张鲁", 51, 26, 74, 78, 75, 304, 310, 9, "汉中", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "zhanglu");
            config[102053] = new HeroConfig(102053, "陈矫", 61, 27, 76, 83, 64, 311, 310, 2, "濮阳", 10, 17, 0, 0, 232, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102056] = new HeroConfig(102056, "王修", 54, 27, 76, 79, 63, 299, 310, 2, "许昌", 10, 17, 0, 0, 188, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102067] = new HeroConfig(102067, "郭奕", 19, 27, 66, 72, 44, 228, 310, 2, "陈留", 10, 17, 0, 0, 54, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107013] = new HeroConfig(107013, "蒯越", 47, 27, 82, 89, 73, 318, 300, 7, "襄阳", 10, 17, 0, 0, 262, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110015] = new HeroConfig(110015, "阎象", 30, 27, 70, 75, 51, 253, 300, 10, "寿春", 10, 17, 0, 0, 84, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102072] = new HeroConfig(102072, "桓阶", 9, 25, 65, 76, 67, 242, 310, 2, "濮阳", 10, 17, 0, 0, 69, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100010] = new HeroConfig(100010, "袁术", 67, 65, 65, 60, 45, 302, 335, 10, "寿春", 10, 17, 0, 0, 198, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshu");
            config[104032] = new HeroConfig(104032, "荀谌", 19, 25, 77, 79, 64, 264, 310, 4, "平原", 10, 17, 0, 0, 102, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[109006] = new HeroConfig(109006, "阎圃", 29, 25, 82, 80, 70, 286, 300, 9, "上庸", 10, 17, 0, 0, 150, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102068] = new HeroConfig(102068, "董昭", 18, 24, 78, 83, 57, 260, 310, 2, "小沛", 10, 17, 0, 0, 95, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107010] = new HeroConfig(107010, "伊籍", 29, 24, 80, 86, 84, 303, 300, 7, "零陵", 10, 17, 0, 0, 202, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102061] = new HeroConfig(102061, "卞氏", 35, 23, 74, 76, 87, 295, 310, 2, "许昌", 10, 17, 0, 0, 175, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104033] = new HeroConfig(104033, "辛毗", 37, 23, 75, 77, 69, 281, 310, 4, "南皮", 10, 17, 0, 0, 137, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102066] = new HeroConfig(102066, "司马朗", 20, 21, 71, 84, 81, 277, 310, 2, "陈留", 10, 17, 0, 0, 128, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104034] = new HeroConfig(104034, "逢纪", 27, 21, 84, 72, 39, 243, 310, 4, "南皮", 10, 17, 0, 0, 70, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103056] = new HeroConfig(103056, "吴国太", 29, 20, 70, 74, 81, 274, 310, 3, "吴", 10, 17, 0, 0, 121, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102057] = new HeroConfig(102057, "国渊", 49, 18, 70, 85, 73, 295, 310, 2, "濮阳", 10, 17, 0, 0, 175, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110004] = new HeroConfig(110004, "袁涣", 30, 17, 68, 79, 83, 277, 300, 10, "寿春", 10, 17, 0, 0, 128, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[112001] = new HeroConfig(112001, "公孙恭", 37, 16, 64, 57, 39, 213, 300, 12, "襄平", 10, 17, 0, 0, 41, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103043] = new HeroConfig(103043, "程秉", 16, 15, 71, 73, 65, 240, 310, 3, "庐江", 10, 17, 0, 0, 67, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[103054] = new HeroConfig(103054, "薛综", 32, 15, 68, 77, 59, 251, 310, 3, "柴桑", 10, 17, 0, 0, 81, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107006] = new HeroConfig(107006, "韩嵩", 25, 15, 70, 78, 61, 249, 300, 7, "襄阳", 10, 17, 0, 0, 78, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110010] = new HeroConfig(110010, "杨弘", 18, 15, 76, 62, 45, 216, 300, 10, "寿春", 10, 17, 0, 0, 44, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[110003] = new HeroConfig(110003, "袁胤", 17, 14, 39, 43, 46, 159, 300, 10, "寿春", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100011] = new HeroConfig(100011, "公孙瓒", 83, 81, 75, 46, 77, 362, 295, 11, "北平", 10, 17, 0, 0, 568, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "gongsunzan");
            config[102059] = new HeroConfig(102059, "娄圭", 48, 12, 81, 63, 14, 218, 310, 2, "濮阳", 10, 17, 0, 0, 45, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107015] = new HeroConfig(107015, "刘琦", 49, 11, 58, 68, 69, 255, 300, 7, "襄阳", 10, 17, 0, 0, 87, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[104035] = new HeroConfig(104035, "陈琳", 10, 9, 74, 79, 72, 244, 310, 4, "平原", 10, 17, 0, 0, 71, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107017] = new HeroConfig(107017, "蔡氏", 8, 7, 69, 58, 66, 208, 300, 7, "襄阳", 10, 17, 0, 0, 38, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102073] = new HeroConfig(102073, "蒋干", 9, 6, 65, 64, 47, 191, 310, 2, "濮阳", 10, 17, 0, 0, 28, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108025] = new HeroConfig(108025, "秦宓", 15, 6, 71, 77, 75, 244, 250, 8, "成都", 10, 17, 0, 0, 71, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[105030] = new HeroConfig(105030, "王允", 25, 5, 67, 83, 73, 253, 260, 5, "洛阳", 10, 17, 0, 0, 84, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[102065] = new HeroConfig(102065, "孔融", 29, 4, 69, 76, 63, 241, 310, 2, "北海", 10, 17, 0, 0, 68, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[108027] = new HeroConfig(108027, "许靖", 2, 4, 64, 77, 65, 212, 250, 8, "成都", 10, 17, 0, 0, 41, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100012] = new HeroConfig(100012, "公孙度", 67, 71, 66, 51, 55, 310, 295, 12, "襄平", 10, 17, 0, 0, 228, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "gongsundu");
            config[109004] = new HeroConfig(109004, "杨松", 1, 4, 27, 34, 4, 70, 300, 9, "上庸", 10, 17, 0, 0, 25, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[107004] = new HeroConfig(107004, "王粲", 5, 2, 79, 81, 52, 219, 300, 7, "江夏", 10, 17, 0, 0, 46, 0, 1, "马", null, "", "", "def", "SwordHitWhiteCritical", "default");
            config[100020] = new HeroConfig(100020, "司马炎", 69, 59, 77, 85, 75, 365, 385, 99, "", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "simayan");
            config[120001] = new HeroConfig(120001, "邓艾", 94, 87, 89, 88, 75, 433, 295, 99, "", 10, 17, 0, 0, 1971, 0, 1, "枪", null, "奇", "", "def", "SwordHitYellowCritical", "dengai");
            config[120002] = new HeroConfig(120002, "司马师", 80, 67, 87, 82, 70, 386, 280, 99, "", 10, 17, 18, 0, 865, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "simashi");
            config[120003] = new HeroConfig(120003, "司马昭", 78, 57, 87, 84, 65, 371, 290, 99, "", 10, 17, 18, 0, 665, 0, 3, "相", null, "溃", "", "help", "SharpExplosionGreen", "simazhao");
            config[120004] = new HeroConfig(120004, "羊祜", 90, 64, 84, 90, 92, 420, 320, 99, "", 10, 17, 0, 0, 1570, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "yangku");
            config[120005] = new HeroConfig(120005, "钟会", 82, 58, 92, 85, 65, 382, 290, 99, "", 10, 17, 15, 0, 806, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhonghui");
            config[120006] = new HeroConfig(120006, "陈泰", 86, 77, 84, 86, 80, 413, 280, 99, "", 10, 17, 0, 0, 1388, 0, 1, "士", null, "虐", "", "def", "SwordHitYellowCritical", "chentai");
            config[120007] = new HeroConfig(120007, "杜预", 84, 30, 85, 89, 78, 366, 320, 99, "", 10, 17, 18, 0, 609, 0, 3, "相", null, "米", "", "inte", "SharpExplosionGreen", "duyu");
            config[120008] = new HeroConfig(120008, "王濬", 80, 52, 79, 82, 75, 368, 340, 99, "", 10, 17, 0, 0, 631, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "wangrui");
            config[120009] = new HeroConfig(120009, "辛宪英", 42, 28, 84, 80, 82, 316, 310, 99, "", 10, 17, 15, 0, 253, 0, 3, "扇", null, "缓", "", "inte", "FanExplosion", "xinxianying");
            config[199001] = new HeroConfig(199001, "孙策", 96, 93, 74, 75, 94, 432, 260, 99, "", 10, 17, 0, 0, 1937, 0, 1, "车", null, "虎", "", "atk", "SwordHitYellowCritical", "sunce");
            config[199002] = new HeroConfig(199002, "诸葛亮", 98, 45, 100, 99, 92, 434, 245, 99, "", 10, 17, 18, 0, 2006, 0, 3, "谋", null, "神", "空", "inte", "LightningExplosionYellow", "zhugeliang");
            config[199003] = new HeroConfig(199003, "姜维", 92, 89, 90, 85, 88, 444, 295, 99, "", 10, 17, 0, 0, 2390, 0, 1, "车", null, "解", "", "def", "SwordHitYellowCritical", "jiangwei");
            config[199004] = new HeroConfig(199004, "关兴", 80, 85, 72, 70, 80, 387, 300, 99, "", 10, 17, 0, 0, 880, 0, 1, "马", null, "奋", "", "atk", "SwordHitYellowCritical", "guanxing");
            config[199006] = new HeroConfig(199006, "庞统", 85, 47, 98, 86, 65, 381, 250, 99, "", 10, 17, 15, 0, 792, 0, 3, "谋", null, "锁", "火", "inte", "ExplosionFireballFire", "pangtong");
            config[199007] = new HeroConfig(199007, "张苞", 75, 87, 82, 65, 76, 385, 305, 99, "", 10, 17, 0, 0, 850, 0, 1, "枪", null, "乱", "", "atk", "SwordHitYellowCritical", "zhangbao");
            config[199008] = new HeroConfig(199008, "关索", 74, 81, 69, 68, 75, 367, 215, 99, "", 10, 17, 30, 1.5f, 620, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "guansuo");
            config[199009] = new HeroConfig(199009, "黄月英", 58, 34, 86, 85, 70, 333, 190, 99, "", 10, 17, 13, 2.5f, 341, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "huangyueying");
            config[199010] = new HeroConfig(199010, "刘禅", 27, 41, 35, 40, 60, 203, 360, 99, "", 10, 17, 35, 0, 35, 0, 3, "鼓", null, "碉", "", "help", "SoulExplosionOrange", "liushan");
            config[199011] = new HeroConfig(199011, "刘巴", 33, 24, 78, 85, 65, 285, 290, 99, "", 10, 17, 15, 0, 147, 0, 3, "扇", null, "纷", "", "inte", "FanExplosion", "liuba");
            config[199012] = new HeroConfig(199012, "司马懿", 98, 63, 98, 95, 82, 436, 220, 99, "", 10, 17, 15, 0, 2078, 0, 3, "谋", null, "鬼", "", "inte", "ShadowExplosion", "simayi");
            config[199013] = new HeroConfig(199013, "夏侯霸", 82, 77, 69, 72, 75, 375, 320, 99, "", 10, 17, 0, 0, 713, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "xiahouba");
            config[199014] = new HeroConfig(199014, "王双", 68, 88, 38, 50, 65, 309, 380, 99, "", 10, 17, 0, 0, 224, 0, 1, "枪", null, "透", "", "atk", "SwordHitYellowCritical", "wangshuang");
            config[199015] = new HeroConfig(199015, "文鸯", 76, 91, 64, 65, 75, 371, 240, 99, "", 10, 17, 22, 1.5f, 665, 0, 3, "弓", null, "速", "", "shoot", "BulletExplosionBlue", "wenyuan");
            config[199017] = new HeroConfig(199017, "毌丘俭", 78, 75, 55, 72, 70, 350, 340, 99, "", 10, 17, 0, 0, 460, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guanqiujian");
            config[199021] = new HeroConfig(199021, "孙坚", 93, 90, 77, 78, 92, 430, 290, 99, "", 10, 17, 0, 0, 1870, 0, 1, "枪", null, "旋", "", "atk", "SwordHitYellowCritical", "sunjian");
            config[199022] = new HeroConfig(199022, "诸葛恪", 72, 47, 90, 80, 60, 349, 235, 99, "", 10, 17, 15, 0, 452, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhugege");
            config[199023] = new HeroConfig(199023, "华佗", 60, 34, 77, 70, 85, 326, 330, 99, "", 10, 17, 14, 0, 302, 0, 3, "医", null, "药", "", "help", "ShadowExplosionGreen", "huatuo");
            config[199024] = new HeroConfig(199024, "于吉", 47, 41, 73, 65, 70, 296, 310, 99, "", 10, 17, 14, 0, 178, 0, 3, "医", null, "调", "", "help", "ShadowExplosionGreen", "yuji");
            config[199025] = new HeroConfig(199025, "张角", 87, 29, 86, 82, 88, 372, 280, 99, "", 10, 17, 15, 0, 677, 0, 3, "谋", null, "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao");
            config[199026] = new HeroConfig(199026, "张宝", 83, 71, 81, 78, 75, 388, 280, 99, "", 10, 17, 0, 0, 896, 0, 1, "枪", null, "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2");
            config[199027] = new HeroConfig(199027, "张梁", 78, 80, 74, 75, 70, 377, 225, 99, "", 10, 17, 13, 2.5f, 739, 0, 3, "炮", null, "", "", "def", "SwordHitYellowCritical", "zhangliang");
            config[199029] = new HeroConfig(199029, "王异", 73, 51, 82, 75, 78, 359, 290, 99, "", 10, 17, 0, 0, 539, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "wangyi");
            config[199030] = new HeroConfig(199030, "蔡琰", 61, 13, 77, 75, 82, 308, 270, 99, "", 10, 17, 15, 0, 220, 0, 3, "乐", null, "碉", "", "help", "StormExplosion", "caiyan");
            config[199031] = new HeroConfig(199031, "马谡", 70, 72, 88, 78, 65, 373, 235, 99, "", 99, 17, 15, 0, 689, 0, 3, "谋", null, "百", "", "inte", "StormExplosion", "masu");
            config[199032] = new HeroConfig(199032, "马良", 68, 60, 93, 87, 86, 394, 270, 99, "", 99, 17, 18, 0, 995, 0, 3, "相", null, "静", "", "help", "SharpExplosionGreen", "maliang");
            config[199033] = new HeroConfig(199033, "蒋琬", 64, 52, 85, 92, 85, 378, 240, 99, "", 10, 17, 18, 0, 752, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "jiangwan");
            config[199034] = new HeroConfig(199034, "费祎", 68, 42, 86, 90, 88, 374, 250, 99, "", 10, 17, 18, 0, 701, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "feiyi");
            config[199035] = new HeroConfig(199035, "郭攸之", 63, 48, 82, 80, 75, 348, 260, 99, "", 10, 17, 35, 0, 444, 0, 3, "鼓", null, "陷", "", "help", "SoulExplosionOrange", "guoyouzhi");
            config[199036] = new HeroConfig(199036, "邓芝", 70, 71, 80, 79, 84, 384, 290, 99, "", 10, 17, 0, 0, 835, 0, 1, "枪", null, "境", "", "def", "SwordHitYellowCritical", "dengzhi");
            config[199037] = new HeroConfig(199037, "王平", 83, 78, 75, 72, 75, 383, 300, 99, "", 10, 17, 0, 0, 821, 0, 1, "戟", null, "伏", "坚", "def", "SwordHitYellowCritical", "wangping");
            config[199038] = new HeroConfig(199038, "陆抗", 91, 63, 87, 88, 86, 415, 195, 99, "", 10, 17, 30, 1.5f, 1438, 0, 3, "弩", null, "透", "", "shoot", "BulletExplosionBlue", "lukang");

        }

        public static HeroConfig GetConfig(int id)
        {
            HeroConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroConfig configData)
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
