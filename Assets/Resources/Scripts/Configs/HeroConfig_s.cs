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
        ///出生
        /// </summary>
        public int BornYear;
        /// <summary>
        ///死亡
        /// </summary>
        public int DeadYear;
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
        ///阵营
        /// </summary>
        public int ForceId;
        /// <summary>
        ///所在
        /// </summary>
        public string City;
        /// <summary>
        ///出生
        /// </summary>
        public string BornCity;
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


        public HeroConfig(int Id, string Name, int BornYear, int DeadYear, int LeadShip, int Str, int Inte, int Fair, int Charm, int Total, int ForceId, string City, string BornCity, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int RateWeight, int RateAbs, int Pos, string Job, int[] Skills, string Skill1, string Skill2, string Group, string HitEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.BornYear = BornYear;
            this.DeadYear = DeadYear;
            this.LeadShip = LeadShip;
            this.Str = Str;
            this.Inte = Inte;
            this.Fair = Fair;
            this.Charm = Charm;
            this.Total = Total;
            this.ForceId = ForceId;
            this.City = City;
            this.BornCity = BornCity;
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
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "刘备", 161, 223, 80, 77, 74, 78, 99, 408, 1, "新野", "蓟", 10, 17, 0, 0, 0, 100, 1, "帅", null, "仁", "", "core", "SwordHitYellowCritical", "liubei");
            config[101001] = new HeroConfig(101001, "张飞", 165, 221, 92, 98, 30, 22, 45, 287, 1, "新野", "北平", 10, 17, 0, 0, 152, 0, 1, "枪", null, "威", "", "atk", "SwordHitYellowCritical", "zhangfei");
            config[101002] = new HeroConfig(101002, "关羽", 160, 220, 97, 97, 77, 62, 94, 427, 1, "新野", "洛阳", 10, 17, 0, 0, 1775, 0, 1, "车", null, "斩", "", "atk", "SwordHitGreenCritical", "guanyu");
            config[101003] = new HeroConfig(101003, "徐庶", 189, 234, 87, 65, 93, 82, 84, 411, 1, "新野", "许昌", 10, 17, 30, 0, 1341, 0, 3, "谋", null, "火", "共", "inte", "GasExplosionFire", "xusu");
            config[101004] = new HeroConfig(101004, "周仓", 178, 220, 63, 82, 42, 33, 60, 280, 1, "新野", "洛阳", 10, 17, 0, 0, 135, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "zhoucang");
            config[101005] = new HeroConfig(101005, "廖化", 184, 264, 74, 78, 64, 49, 66, 331, 1, "新野", "襄阳", 10, 17, 0, 0, 330, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "liaohua");
            config[101006] = new HeroConfig(101006, "简雍", 164, 221, 72, 65, 70, 75, 70, 352, 1, "新野", "北平", 10, 17, 40, 5f, 476, 0, 3, "弓", null, "破", "", "shoot", "BulletExplosionBlue", "jianyong");
            config[101007] = new HeroConfig(101007, "孙乾", 163, 215, 62, 54, 80, 84, 82, 362, 1, "新野", "北海", 10, 17, 58, 0, 568, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "sunqian");
            config[101009] = new HeroConfig(101009, "关平", 178, 220, 79, 82, 72, 71, 78, 382, 1, "新野", "洛阳", 10, 17, 0, 0, 806, 0, 1, "戟", null, "连", "", "def", "SwordHitYellowCritical", "guanping");
            config[101010] = new HeroConfig(101010, "胡班", 170, 220, 54, 54, 57, 49, 61, 275, 1, "新野", "陈留", 10, 17, 0, 0, 123, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "huban");
            config[101011] = new HeroConfig(101011, "糜竺", 157, 221, 33, 29, 77, 85, 85, 309, 1, "新野", "下邳", 10, 17, 0, 0, 224, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "mizhu");
            config[101012] = new HeroConfig(101012, "糜芳", 160, 223, 54, 61, 32, 23, 23, 193, 1, "新野", "下邳", 10, 17, 0, 0, 29, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "mifang");
            config[101013] = new HeroConfig(101013, "陈到", 178, 230, 76, 71, 63, 53, 69, 332, 1, "新野", "汝南", 10, 17, 0, 0, 335, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chendao");
            config[100002] = new HeroConfig(100002, "曹操", 155, 220, 98, 81, 91, 94, 96, 460, 2, "许昌", "陈留", 10, 17, 0, 0, 0, 100, 1, "帅", null, "识", "", "core", "SwordHitYellowCritical", "caocao");
            config[102001] = new HeroConfig(102001, "郭嘉", 170, 207, 72, 43, 98, 78, 82, 373, 2, "宛", "许昌", 10, 17, 30, 0, 689, 0, 3, "谋", null, "天", "", "inte", "LightningExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 155, 220, 91, 91, 57, 68, 80, 387, 2, "许昌", "陈留", 10, 17, 0, 0, 880, 0, 1, "车", null, "青", "", "atk", "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 163, 212, 67, 47, 95, 100, 93, 402, 2, "许昌", "许昌", 10, 17, 36, 0, 1145, 0, 3, "相", null, "国", "", "help", "FrostExplosionBlue", "xunyu");
            config[102004] = new HeroConfig(102004, "许褚", 169, 227, 65, 96, 36, 20, 59, 276, 2, "许昌", "陈留", 10, 17, 0, 0, 125, 0, 1, "士", null, "斧", "", "atk", "SwordHitYellowCritical", "xuchu");
            config[102005] = new HeroConfig(102005, "夏侯渊", 155, 219, 90, 89, 54, 40, 80, 353, 2, "小沛", "陈留", 10, 17, 40, 5f, 485, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan");
            config[102006] = new HeroConfig(102006, "典韦", 155, 197, 59, 95, 43, 38, 78, 313, 2, "许昌", "陈留", 10, 17, 0, 0, 240, 0, 1, "士", null, "护", "", "def", "SwordHitYellowCritical", "dianwei");
            config[102007] = new HeroConfig(102007, "徐晃", 155, 227, 90, 91, 74, 68, 71, 394, 2, "下邳", "洛阳", 10, 17, 40, 5f, 995, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "xuhuang");
            config[102008] = new HeroConfig(102008, "荀攸", 157, 214, 63, 53, 93, 91, 73, 373, 2, "许昌", "许昌", 10, 17, 30, 0, 689, 0, 3, "谋", null, "百", "米", "inte", "FrostExplosionBlue", "xunyou");
            config[102009] = new HeroConfig(102009, "于禁", 158, 219, 80, 75, 68, 55, 51, 329, 2, "下邳", "北海", 10, 17, 0, 0, 318, 0, 1, "戟", null, "青", "破", "def", "SwordHitYellowCritical", "yujin");
            config[102010] = new HeroConfig(102010, "曹仁", 168, 223, 90, 86, 62, 46, 76, 360, 2, "宛", "陈留", 10, 17, 0, 0, 548, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caoren");
            config[102011] = new HeroConfig(102011, "曹洪", 169, 232, 82, 83, 44, 35, 54, 298, 2, "小沛", "陈留", 10, 17, 0, 0, 185, 0, 1, "枪", null, "商", "", "atk", "SwordHitYellowCritical", "caohong");
            config[102012] = new HeroConfig(102012, "乐进", 158, 218, 80, 84, 50, 49, 63, 326, 2, "北海", "濮阳", 10, 17, 0, 0, 302, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "lejin");
            config[102013] = new HeroConfig(102013, "文聘", 170, 226, 80, 82, 65, 75, 78, 380, 2, "北海", "宛", 10, 17, 0, 0, 779, 0, 1, "戟", null, "透", "劫", "atk", "SwordHitYellowCritical", "wenpin");
            config[102014] = new HeroConfig(102014, "曹休", 170, 228, 73, 73, 58, 56, 67, 327, 2, "宛", "陈留", 10, 17, 40, 5f, 307, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "caoxiu");
            config[102015] = new HeroConfig(102015, "郝昭", 176, 228, 87, 79, 74, 59, 69, 368, 2, "北海", "晋阳", 10, 17, 0, 0, 631, 0, 1, "枪", null, "坚", "", "def", "SwordHitYellowCritical", "haozhao");
            config[102016] = new HeroConfig(102016, "程昱", 141, 220, 63, 54, 87, 79, 56, 339, 2, "濮阳", "濮阳", 10, 17, 30, 0, 379, 0, 3, "谋", null, "识", "火", "inte", "StormExplosion", "chengyu");
            config[102017] = new HeroConfig(102017, "杨修", 175, 219, 10, 4, 83, 80, 43, 220, 2, "小沛", "长安", 10, 17, 36, 0, 47, 0, 3, "相", null, "虐", "", "help", "SharpExplosionGreen", "yangxiu");
            config[102018] = new HeroConfig(102018, "牛金", 175, 220, 71, 77, 38, 40, 45, 271, 2, "宛", "宛", 10, 17, 0, 0, 115, 0, 1, "刀", null, "伏", "", "atk", "SwordHitYellowCritical", "niujin");
            config[102019] = new HeroConfig(102019, "陈群", 165, 237, 65, 45, 74, 98, 73, 355, 2, "许昌", "许昌", 10, 17, 30, 0, 502, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "chenqun");
            config[102020] = new HeroConfig(102020, "李典", 174, 215, 74, 73, 79, 74, 65, 365, 2, "宛", "陈留", 10, 17, 0, 0, 599, 0, 1, "枪", null, "伏", "坚", "def", "SwordHitYellowCritical", "lidian");
            config[102021] = new HeroConfig(102021, "曹丕", 187, 226, 78, 79, 78, 84, 77, 396, 2, "许昌", "陈留", 10, 17, 0, 0, 1031, 0, 1, "刀", null, "敏", "", "def", "SwordHitYellowCritical", "caopi");
            config[102022] = new HeroConfig(102022, "曹植", 192, 232, 64, 67, 75, 65, 74, 345, 2, "许昌", "陈留", 10, 17, 30, 0, 421, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "caozhi");
            config[102023] = new HeroConfig(102023, "刘晔", 175, 234, 65, 49, 92, 75, 69, 350, 2, "小沛", "寿春", 10, 17, 26, 8f, 460, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "liuye");
            config[102024] = new HeroConfig(102024, "朱灵", 168, 222, 73, 77, 67, 53, 42, 312, 2, "宛", "平原", 10, 17, 26, 8f, 236, 0, 3, "炮", null, "", "", "atk", "SwordHitYellowCritical", "zhuling");
            config[102025] = new HeroConfig(102025, "曹彰", 189, 223, 82, 90, 37, 32, 71, 312, 2, "小沛", "陈留", 10, 17, 0, 0, 236, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caozhang");
            config[102026] = new HeroConfig(102026, "满宠", 162, 242, 84, 64, 82, 84, 50, 364, 2, "下邳", "陈留", 10, 17, 0, 0, 588, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "manchong");
            config[102027] = new HeroConfig(102027, "曹冲", 196, 208, 31, 21, 79, 74, 78, 283, 2, "许昌", "陈留", 10, 17, 30, 0, 142, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "caochong");
            config[102028] = new HeroConfig(102028, "蒋济", 188, 249, 48, 43, 80, 73, 53, 297, 2, "北海", "寿春", 10, 17, 58, 0, 181, 0, 3, "鼓", null, "米", "", "help", "SoulExplosionOrange", "jiangji");
            config[102029] = new HeroConfig(102029, "甄宓", 183, 221, 14, 3, 69, 64, 94, 244, 2, "下邳", "北平", 10, 17, 58, 0, 71, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "zhenshi");
            config[102030] = new HeroConfig(102030, "戏志才", 169, 196, 66, 24, 88, 75, 70, 323, 2, "下邳", "许昌", 10, 17, 30, 0, 286, 0, 3, "谋", null, "陷", "", "inte", "StormExplosion", "xizhicai");
            config[102032] = new HeroConfig(102032, "曹真", 180, 231, 82, 74, 65, 69, 84, 374, 2, "北海", "陈留", 10, 17, 0, 0, 701, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "caozhen");
            config[102033] = new HeroConfig(102033, "郭淮", 187, 255, 87, 78, 76, 71, 73, 385, 2, "陈留", "晋阳", 10, 17, 0, 0, 850, 0, 1, "士", null, "", "", "def", "SwordHitYellowCritical", "guohuai");
            config[102034] = new HeroConfig(102034, "夏侯尚", 185, 225, 79, 75, 72, 63, 59, 348, 2, "陈留", "陈留", 10, 17, 0, 0, 444, 0, 1, "戟", null, "敏", "", "def", "SwordHitYellowCritical", "xiahoushang");
            config[102035] = new HeroConfig(102035, "钟繇", 151, 230, 70, 37, 72, 91, 76, 346, 2, "许昌", "许昌", 10, 17, 36, 0, 429, 0, 3, "相", null, "米", "", "help", "SharpExplosionGreen", "zhongyao");
            config[102036] = new HeroConfig(102036, "田予", 165, 227, 80, 72, 80, 78, 75, 385, 2, "北海", "吴", 10, 17, 0, 0, 850, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "tianyu");
            config[102037] = new HeroConfig(102037, "张燕", 165, 210, 79, 78, 51, 46, 61, 315, 2, "濮阳", "新野", 10, 17, 0, 0, 249, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangyan");
            config[102038] = new HeroConfig(102038, "陈登", 163, 201, 79, 64, 81, 82, 61, 367, 2, "小沛", "宛", 10, 17, 0, 0, 620, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chendeng");
            config[102039] = new HeroConfig(102039, "贾逵", 174, 228, 78, 61, 84, 85, 75, 383, 2, "北海", "陈留", 10, 17, 0, 0, 821, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "jiakui");
            config[102040] = new HeroConfig(102040, "曹纯", 170, 210, 75, 71, 60, 35, 72, 313, 2, "许昌", "濮阳", 10, 17, 0, 0, 240, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "caochun");
            config[102041] = new HeroConfig(102041, "王凌", 172, 251, 74, 64, 70, 82, 71, 361, 2, "下邳", "下邳", 10, 17, 0, 0, 558, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangling");
            config[102042] = new HeroConfig(102042, "张既", 168, 223, 74, 35, 75, 89, 81, 354, 2, "宛", "小沛", 10, 17, 0, 0, 493, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangji2");
            config[102043] = new HeroConfig(102043, "梁习", 160, 223, 73, 40, 73, 87, 80, 353, 2, "宛", "北海", 10, 17, 0, 0, 485, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liangxi");
            config[102044] = new HeroConfig(102044, "李通", 165, 209, 73, 81, 57, 63, 83, 357, 2, "下邳", "平原", 10, 17, 0, 0, 520, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "litong");
            config[102046] = new HeroConfig(102046, "孙观", 170, 213, 72, 78, 51, 39, 66, 306, 2, "陈留", "南皮", 10, 17, 0, 0, 213, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "sunguan");
            config[102047] = new HeroConfig(102047, "夏侯德", 170, 219, 69, 73, 32, 40, 52, 266, 2, "陈留", "晋阳", 10, 17, 0, 0, 105, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xiahoude");
            config[102048] = new HeroConfig(102048, "杜畿", 163, 224, 66, 32, 74, 87, 76, 335, 2, "陈留", "安定", 10, 17, 0, 0, 354, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "duji");
            config[102049] = new HeroConfig(102049, "刘馥", 174, 208, 64, 49, 73, 89, 83, 358, 2, "陈留", "天水", 10, 17, 0, 0, 529, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liufu");
            config[102050] = new HeroConfig(102050, "夏侯恩", 175, 219, 63, 71, 51, 44, 70, 299, 2, "陈留", "武威", 10, 17, 0, 0, 188, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xiahoueng");
            config[102051] = new HeroConfig(102051, "温恢", 171, 223, 62, 36, 73, 86, 76, 333, 2, "下邳", "武陵", 10, 17, 0, 0, 341, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wenhui");
            config[102052] = new HeroConfig(102052, "毛玠", 165, 216, 62, 38, 58, 79, 60, 297, 2, "下邳", "零陵", 10, 17, 0, 0, 181, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "maojie");
            config[102053] = new HeroConfig(102053, "陈矫", 169, 237, 61, 27, 76, 83, 64, 311, 2, "濮阳", "江陵", 10, 17, 0, 0, 232, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenjiao");
            config[102054] = new HeroConfig(102054, "吕虔", 168, 227, 57, 70, 58, 74, 60, 319, 2, "濮阳", "江夏", 10, 17, 0, 0, 267, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lvqian");
            config[102055] = new HeroConfig(102055, "徐邈", 171, 249, 55, 32, 67, 82, 79, 315, 2, "宛", "桂阳", 10, 17, 0, 0, 249, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xumiao");
            config[102056] = new HeroConfig(102056, "王修", 170, 217, 54, 27, 76, 79, 63, 299, 2, "许昌", "长沙", 10, 17, 0, 0, 188, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangxiu");
            config[102057] = new HeroConfig(102057, "国渊", 170, 215, 49, 18, 70, 85, 73, 295, 2, "濮阳", "庐江", 10, 17, 0, 0, 175, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "guoyuan");
            config[102058] = new HeroConfig(102058, "典满", 170, 220, 49, 71, 38, 25, 50, 233, 2, "陈留", "会稽", 10, 17, 0, 0, 59, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "dianman");
            config[102059] = new HeroConfig(102059, "娄圭", 165, 201, 48, 12, 81, 63, 14, 218, 2, "濮阳", "柴桑", 10, 17, 0, 0, 45, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lougui");
            config[102060] = new HeroConfig(102060, "王朗", 152, 228, 46, 34, 79, 84, 51, 294, 2, "宛", "永安", 10, 17, 0, 0, 172, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wanglang");
            config[102061] = new HeroConfig(102061, "卞氏", 159, 230, 35, 23, 74, 76, 87, 295, 2, "许昌", "江州", 10, 17, 0, 0, 175, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "nvbianshi2");
            config[102063] = new HeroConfig(102063, "李孚", 170, 230, 30, 35, 73, 72, 68, 278, 2, "下邳", "建宁", 10, 17, 0, 0, 130, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lifu");
            config[102065] = new HeroConfig(102065, "孔融", 153, 208, 29, 4, 69, 76, 63, 241, 2, "北海", "北海", 10, 17, 0, 0, 68, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "kongrong");
            config[102066] = new HeroConfig(102066, "司马朗", 171, 217, 20, 21, 71, 84, 81, 277, 2, "陈留", "梓潼", 10, 17, 0, 0, 128, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "simalang");
            config[102067] = new HeroConfig(102067, "郭奕", 185, 220, 19, 27, 66, 72, 44, 228, 2, "陈留", "上庸", 10, 17, 0, 0, 54, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "guoyi");
            config[102068] = new HeroConfig(102068, "董昭", 156, 236, 18, 24, 78, 83, 57, 260, 2, "小沛", "汉中", 10, 17, 0, 0, 95, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "dongzhao");
            config[102070] = new HeroConfig(102070, "崔琰", 163, 216, 17, 54, 69, 84, 74, 298, 2, "濮阳", "汝南", 10, 17, 0, 0, 185, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "cuiyan");
            config[102071] = new HeroConfig(102071, "吴质", 178, 230, 16, 29, 68, 57, 37, 207, 2, "小沛", "寿春", 10, 17, 0, 0, 37, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wuzhi");
            config[102072] = new HeroConfig(102072, "桓阶", 162, 221, 9, 25, 65, 76, 67, 242, 2, "濮阳", "北平", 10, 17, 0, 0, 69, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "huanjie");
            config[102073] = new HeroConfig(102073, "蒋干", 174, 212, 9, 6, 65, 64, 47, 191, 2, "濮阳", "蓟", 10, 17, 0, 0, 28, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "jianggan");
            config[100003] = new HeroConfig(100003, "孙权", 182, 252, 74, 65, 78, 87, 93, 397, 3, "吴", "吴", 10, 17, 0, 0, 0, 100, 1, "帅", null, "衡", "", "core", "SwordHitYellowCritical", "sunquan");
            config[103001] = new HeroConfig(103001, "甘宁", 175, 215, 93, 94, 76, 18, 58, 339, 3, "长沙", "江州", 10, 17, 40, 5f, 379, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "ganning");
            config[103002] = new HeroConfig(103002, "太史慈", 166, 206, 85, 93, 66, 58, 79, 381, 3, "吴", "北海", 10, 17, 40, 5f, 792, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "taishici");
            config[103003] = new HeroConfig(103003, "黄盖", 155, 210, 79, 83, 65, 65, 80, 372, 3, "桂阳", "零陵", 10, 17, 0, 0, 677, 0, 1, "士", null, "奋", "", "def", "SwordHitYellowCritical", "huanggai");
            config[103004] = new HeroConfig(103004, "周泰", 170, 225, 76, 91, 48, 38, 60, 313, 3, "长沙", "庐江", 10, 17, 0, 0, 240, 0, 1, "枪", null, "连", "", "atk", "SwordHitYellowCritical", "zhoutai");
            config[103005] = new HeroConfig(103005, "鲁肃", 172, 217, 80, 56, 92, 92, 89, 409, 3, "柴桑", "寿春", 10, 17, 36, 0, 1294, 0, 3, "相", null, "商", "雷", "help", "SharpExplosionGreen", "lusu");
            config[103006] = new HeroConfig(103006, "周瑜", 175, 210, 96, 71, 96, 86, 93, 442, 3, "柴桑", "庐江", 10, 17, 30, 0, 2308, 0, 3, "谋", null, "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu");
            config[103007] = new HeroConfig(103007, "蒋钦", 172, 220, 78, 84, 51, 42, 74, 329, 3, "桂阳", "寿春", 10, 17, 0, 0, 318, 0, 1, "戟", null, "", "", "atk", "SwordHitYellowCritical", "jiangqing");
            config[103008] = new HeroConfig(103008, "吕蒙", 178, 219, 91, 81, 84, 74, 82, 412, 3, "柴桑", "汝南", 10, 17, 0, 0, 1364, 0, 1, "马", null, "学", "羽", "def", "SwordHitYellowCritical", "lvmeng");
            config[103009] = new HeroConfig(103009, "陆逊", 183, 245, 96, 69, 92, 87, 90, 434, 3, "吴", "吴", 10, 17, 30, 0, 2006, 0, 3, "谋", null, "炎", "", "inte", "GasExplosionFire", "luxun");
            config[103010] = new HeroConfig(103010, "张昭", 156, 236, 32, 2, 83, 98, 79, 294, 3, "桂阳", "下邳", 10, 17, 36, 0, 172, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "zhangzhao");
            config[103011] = new HeroConfig(103011, "诸葛瑾", 174, 241, 72, 34, 81, 90, 90, 367, 3, "吴", "襄阳", 10, 17, 30, 0, 620, 0, 3, "扇", null, "励", "", "help", "FanExplosion", "zhugejin");
            config[103012] = new HeroConfig(103012, "孙尚香", 191, 222, 69, 83, 64, 61, 70, 347, 3, "吴", "吴", 10, 17, 40, 5f, 436, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "sunshangxiang");
            config[103013] = new HeroConfig(103013, "朱桓", 177, 238, 84, 82, 75, 56, 59, 356, 3, "桂阳", "吴", 10, 17, 0, 0, 511, 0, 1, "枪", null, "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan");
            config[103014] = new HeroConfig(103014, "大乔", 175, 221, 17, 11, 72, 78, 92, 270, 3, "吴", "庐江", 10, 17, 30, 0, 113, 0, 3, "乐", null, "碉", "陷", "help", "StormExplosion", "daqiao");
            config[103015] = new HeroConfig(103015, "小乔", 176, 223, 16, 12, 73, 77, 92, 270, 3, "柴桑", "庐江", 10, 17, 30, 0, 113, 0, 3, "乐", null, "曲", "陷", "help", "StormExplosion", "xiaoqiao");
            config[103016] = new HeroConfig(103016, "丁奉", 186, 271, 76, 95, 66, 51, 52, 340, 3, "长沙", "庐江", 10, 17, 26, 8f, 386, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "dingfeng");
            config[103017] = new HeroConfig(103017, "董袭", 170, 213, 72, 85, 50, 48, 60, 315, 3, "长沙", "会稽", 10, 17, 0, 0, 249, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "dongxi");
            config[103018] = new HeroConfig(103018, "凌统", 189, 237, 72, 83, 54, 37, 66, 312, 3, "建业", "吴", 10, 17, 50, 5f, 236, 0, 3, "弩", null, "虐", "", "shoot", "BulletExplosionBlue", "lingtong");
            config[103019] = new HeroConfig(103019, "潘璋", 176, 234, 76, 80, 70, 28, 12, 266, 3, "庐江", "濮阳", 10, 17, 0, 0, 105, 0, 1, "戟", null, "刺", "虐", "def", "SwordHitYellowCritical", "panzhang");
            config[103020] = new HeroConfig(103020, "朱治", 156, 224, 66, 78, 42, 39, 64, 289, 3, "柴桑", "会稽", 10, 17, 40, 5f, 158, 0, 3, "弓", null, "敏", "", "shoot", "BulletExplosionBlue", "zhuzhi");
            config[103021] = new HeroConfig(103021, "徐盛", 177, 228, 87, 81, 78, 65, 73, 384, 3, "庐江", "北海", 10, 17, 0, 0, 835, 0, 1, "士", null, "乱", "", "def", "SwordHitYellowCritical", "xusheng");
            config[103022] = new HeroConfig(103022, "程普", 138, 215, 80, 75, 73, 71, 81, 380, 3, "庐江", "北平", 10, 17, 0, 0, 779, 0, 1, "戟", null, "实", "奋", "def", "SwordHitYellowCritical", "chengpu");
            config[103023] = new HeroConfig(103023, "张纮", 153, 212, 23, 21, 83, 94, 79, 300, 3, "会稽", "寿春", 10, 17, 36, 0, 191, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "zhanghong");
            config[103024] = new HeroConfig(103024, "顾雍", 168, 243, 43, 18, 80, 92, 76, 309, 3, "桂阳", "吴", 10, 17, 30, 0, 224, 0, 3, "扇", null, "连", "", "inte", "FanExplosion", "guyong");
            config[103025] = new HeroConfig(103025, "步骘", 177, 247, 72, 51, 84, 87, 65, 359, 3, "长沙", "寿春", 10, 17, 58, 0, 539, 0, 3, "鼓", null, "励", "", "help", "SoulExplosionOrange", "buzhi");
            config[103026] = new HeroConfig(103026, "阚泽", 170, 243, 42, 48, 87, 85, 71, 333, 3, "长沙", "会稽", 10, 17, 30, 0, 341, 0, 3, "谋", null, "炽", "", "inte", "StormExplosion", "kanze");
            config[103027] = new HeroConfig(103027, "韩当", 156, 223, 75, 84, 54, 49, 67, 329, 3, "柴桑", "北平", 10, 17, 0, 0, 318, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "handang");
            config[103029] = new HeroConfig(103029, "苏飞", 170, 215, 69, 63, 66, 72, 70, 340, 3, "长沙", "庐江", 10, 17, 0, 0, 386, 0, 1, "刀", null, "复", "", "def", "SwordHitYellowCritical", "sufei");
            config[103031] = new HeroConfig(103031, "陈武", 178, 215, 74, 87, 43, 40, 62, 306, 3, "桂阳", "会稽", 10, 17, 0, 0, 213, 0, 1, "枪", null, "劫", "", "def", "SwordHitYellowCritical", "chengwu");
            config[103032] = new HeroConfig(103032, "朱然", 182, 249, 77, 67, 69, 58, 73, 344, 3, "吴", "柴桑", 10, 17, 0, 0, 414, 0, 1, "枪", null, "竟", "", "def", "SwordHitYellowCritical", "zhuran");
            config[103033] = new HeroConfig(103033, "孙韶", 188, 241, 76, 75, 71, 65, 70, 357, 3, "吴", "永安", 10, 17, 0, 0, 520, 0, 1, "戟", null, "破", "", "def", "SwordHitYellowCritical", "sunshao");
            config[103035] = new HeroConfig(103035, "严畯", 183, 243, 13, 2, 70, 85, 72, 242, 3, "长沙", "江州", 10, 17, 30, 0, 69, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "yanjun");
            config[103036] = new HeroConfig(103036, "吕范", 161, 228, 73, 63, 74, 77, 69, 356, 3, "庐江", "建宁", 10, 17, 0, 0, 511, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lvfan");
            config[103037] = new HeroConfig(103037, "马忠", 183, 249, 64, 73, 61, 34, 36, 268, 3, "桂阳", "云南", 10, 17, 0, 0, 109, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "mazhong");
            config[103038] = new HeroConfig(103038, "贾华", 178, 234, 49, 65, 71, 29, 52, 266, 3, "建业", "梓潼", 10, 17, 0, 0, 105, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "jiahua");
            config[103039] = new HeroConfig(103039, "贺齐", 170, 227, 83, 78, 42, 64, 73, 340, 3, "会稽", "上庸", 10, 17, 0, 0, 386, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "heqi");
            config[103040] = new HeroConfig(103040, "留赞", 183, 255, 78, 75, 64, 57, 62, 336, 3, "会稽", "汉中", 10, 17, 0, 0, 360, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liuzhan");
            config[103041] = new HeroConfig(103041, "虞翻", 164, 233, 43, 46, 86, 83, 46, 304, 3, "会稽", "汝南", 10, 17, 0, 0, 205, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lufan");
            config[103042] = new HeroConfig(103042, "陆绩", 187, 219, 19, 48, 61, 69, 41, 238, 3, "会稽", "寿春", 10, 17, 0, 0, 64, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "luji");
            config[103043] = new HeroConfig(103043, "程秉", 168, 225, 16, 15, 71, 73, 65, 240, 3, "庐江", "北平", 10, 17, 0, 0, 67, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenbing");
            config[103044] = new HeroConfig(103044, "吕岱", 161, 256, 81, 70, 68, 74, 62, 355, 3, "柴桑", "蓟", 10, 17, 0, 0, 502, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lvdai");
            config[103045] = new HeroConfig(103045, "孙瑜", 177, 215, 77, 70, 68, 69, 78, 362, 3, "柴桑", "襄平", 10, 17, 0, 0, 568, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "sunyu");
            config[103046] = new HeroConfig(103046, "吾粲", 190, 245, 66, 40, 76, 73, 70, 325, 3, "柴桑", "洛阳", 10, 17, 0, 0, 297, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wucan");
            config[103047] = new HeroConfig(103047, "李异", 180, 231, 56, 73, 18, 17, 22, 186, 3, "庐江", "长安", 10, 17, 0, 0, 26, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liyi");
            config[103048] = new HeroConfig(103048, "张承", 178, 244, 77, 70, 75, 74, 74, 370, 3, "建业", "许昌", 10, 17, 0, 0, 653, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangcheng");
            config[103049] = new HeroConfig(103049, "孙皎", 183, 219, 75, 70, 64, 69, 71, 349, 3, "建业", "邺", 10, 17, 0, 0, 452, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "sunjiao");
            config[103050] = new HeroConfig(103050, "宋谦", 182, 235, 69, 54, 70, 73, 73, 339, 3, "建业", "襄阳", 10, 17, 0, 0, 379, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "songqian");
            config[103052] = new HeroConfig(103052, "孙匡", 175, 204, 49, 44, 45, 62, 53, 253, 3, "吴", "襄阳", 10, 17, 0, 0, 84, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "sunkuang");
            config[103054] = new HeroConfig(103054, "薛综", 176, 243, 32, 15, 68, 77, 59, 251, 3, "柴桑", "建业", 10, 17, 0, 0, 81, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xuezong");
            config[103055] = new HeroConfig(103055, "孙朗", 185, 223, 32, 40, 28, 38, 42, 180, 3, "柴桑", "吴", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "sunlang");
            config[103056] = new HeroConfig(103056, "吴国太", 156, 216, 29, 20, 70, 74, 81, 274, 3, "吴", "新野", 10, 17, 0, 0, 121, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wuguotai");
            config[100004] = new HeroConfig(100004, "袁绍", 154, 202, 86, 73, 70, 73, 90, 392, 4, "邺", "汝南", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshao");
            config[104001] = new HeroConfig(104001, "张郃", 167, 231, 89, 90, 69, 56, 70, 374, 4, "邺", "南皮", 10, 17, 0, 0, 701, 0, 1, "车", null, "分", "", "def", "SwordHitYellowCritical", "zhanghe");
            config[104002] = new HeroConfig(104002, "颜良", 165, 200, 78, 93, 42, 32, 53, 298, 4, "南皮", "平原", 10, 17, 0, 0, 185, 0, 1, "车", null, "破", "", "atk", "SwordHitYellowCritical", "yanliang");
            config[104003] = new HeroConfig(104003, "文丑", 163, 200, 79, 92, 48, 52, 68, 339, 4, "晋阳", "平原", 10, 17, 0, 0, 379, 0, 1, "车", null, "刺", "", "def", "SwordHitYellowCritical", "wenchou");
            config[104004] = new HeroConfig(104004, "田丰", 170, 200, 72, 33, 93, 89, 64, 351, 4, "晋阳", "南皮", 10, 17, 30, 0, 468, 0, 3, "谋", null, "雷", "", "inte", "StormExplosion", "tianfeng");
            config[104005] = new HeroConfig(104005, "鞠义", 158, 191, 72, 78, 55, 18, 37, 260, 4, "平原", "武威", 10, 17, 40, 5f, 95, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "juyi");
            config[104006] = new HeroConfig(104006, "许攸", 155, 204, 39, 29, 80, 57, 23, 228, 4, "邺", "宛", 10, 17, 30, 0, 54, 0, 3, "谋", null, "火", "", "inte", "StormExplosion", "xuyou");
            config[104007] = new HeroConfig(104007, "高览", 168, 200, 76, 82, 66, 55, 62, 341, 4, "邺", "汝南", 10, 17, 0, 0, 393, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "gaolan");
            config[104008] = new HeroConfig(104008, "沮授", 169, 200, 78, 35, 90, 91, 74, 368, 4, "邺", "南皮", 10, 17, 30, 0, 631, 0, 3, "谋", null, "静", "", "inte", "StormExplosion", "jushou");
            config[104009] = new HeroConfig(104009, "郭图", 170, 205, 52, 50, 82, 70, 37, 291, 4, "南皮", "许昌", 10, 17, 30, 0, 163, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "guotu");
            config[104011] = new HeroConfig(104011, "焦触", 172, 206, 65, 72, 33, 32, 39, 241, 4, "平原", "天水", 10, 17, 0, 0, 68, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "jiaochu");
            config[104012] = new HeroConfig(104012, "吕翔", 175, 204, 54, 71, 12, 19, 28, 184, 4, "邺", "武威", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lvxiang");
            config[104013] = new HeroConfig(104013, "吕旷", 174, 204, 56, 70, 13, 22, 29, 190, 4, "南皮", "武陵", 10, 17, 0, 0, 27, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lvkuang");
            config[104014] = new HeroConfig(104014, "张南", 173, 207, 55, 69, 45, 33, 33, 235, 4, "南皮", "零陵", 10, 17, 0, 0, 61, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangnan");
            config[104015] = new HeroConfig(104015, "眭元进", 170, 207, 52, 68, 45, 32, 49, 246, 4, "邺", "江陵", 10, 17, 0, 0, 74, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "kuiyuanjin");
            config[104016] = new HeroConfig(104016, "牵招", 171, 231, 70, 67, 71, 73, 71, 352, 4, "平原", "江夏", 10, 17, 0, 0, 476, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "qianzhao");
            config[104017] = new HeroConfig(104017, "袁尚", 176, 207, 58, 66, 39, 35, 66, 264, 4, "南皮", "桂阳", 10, 17, 0, 0, 102, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yuanshang");
            config[104018] = new HeroConfig(104018, "袁谭", 173, 205, 57, 66, 28, 33, 56, 240, 4, "晋阳", "长沙", 10, 17, 0, 0, 67, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yuantan");
            config[104019] = new HeroConfig(104019, "周昂", 175, 201, 74, 65, 62, 50, 62, 313, 4, "晋阳", "庐江", 10, 17, 0, 0, 240, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhouang");
            config[104020] = new HeroConfig(104020, "田畴", 169, 215, 64, 64, 70, 75, 72, 345, 4, "晋阳", "会稽", 10, 17, 0, 0, 421, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "tianchou");
            config[104021] = new HeroConfig(104021, "淳于琼", 163, 200, 70, 64, 28, 28, 35, 225, 4, "晋阳", "柴桑", 10, 17, 0, 0, 51, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chunyuqiong");
            config[104022] = new HeroConfig(104022, "吕威璜", 172, 200, 58, 63, 29, 39, 44, 233, 4, "平原", "永安", 10, 17, 0, 0, 59, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lvweihuang");
            config[104023] = new HeroConfig(104023, "审配", 165, 204, 84, 60, 83, 73, 70, 370, 4, "平原", "邺", 10, 17, 0, 0, 653, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "shengpei");
            config[104024] = new HeroConfig(104024, "韩莒子", 170, 200, 51, 59, 51, 46, 52, 259, 4, "邺", "建宁", 10, 17, 0, 0, 93, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "hanlvzi");
            config[104025] = new HeroConfig(104025, "苏由", 173, 206, 50, 59, 49, 41, 49, 248, 4, "晋阳", "云南", 10, 17, 0, 0, 77, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "suyou");
            config[104026] = new HeroConfig(104026, "蒋义渠", 174, 202, 71, 58, 57, 51, 55, 292, 4, "邺", "梓潼", 10, 17, 0, 0, 166, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "jiangyiqu");
            config[104027] = new HeroConfig(104027, "高干", 170, 206, 72, 54, 47, 57, 64, 294, 4, "晋阳", "上庸", 10, 17, 0, 0, 172, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gaogan");
            config[104028] = new HeroConfig(104028, "袁熙", 170, 207, 62, 47, 59, 61, 60, 289, 4, "平原", "汉中", 10, 17, 0, 0, 158, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yuanxi");
            config[104029] = new HeroConfig(104029, "陈震", 178, 235, 44, 44, 65, 73, 70, 296, 4, "南皮", "汝南", 10, 17, 0, 0, 178, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenzhen");
            config[104030] = new HeroConfig(104030, "辛评", 175, 204, 69, 43, 76, 75, 68, 331, 4, "邺", "寿春", 10, 17, 0, 0, 330, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xinping");
            config[104031] = new HeroConfig(104031, "高柔", 174, 263, 54, 40, 67, 75, 70, 306, 4, "南皮", "北平", 10, 17, 0, 0, 213, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gaorou");
            config[104032] = new HeroConfig(104032, "荀谌", 172, 214, 19, 25, 77, 79, 64, 264, 4, "平原", "蓟", 10, 17, 0, 0, 102, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xunshen");
            config[104033] = new HeroConfig(104033, "辛毗", 171, 235, 37, 23, 75, 77, 69, 281, 4, "南皮", "襄平", 10, 17, 0, 0, 137, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xinpi");
            config[104034] = new HeroConfig(104034, "逢纪", 175, 202, 27, 21, 84, 72, 39, 243, 4, "南皮", "宛", 10, 17, 0, 0, 70, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "fengji");
            config[104035] = new HeroConfig(104035, "陈琳", 160, 217, 10, 9, 74, 79, 72, 244, 4, "平原", "长安", 10, 17, 0, 0, 71, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenlin");
            config[100005] = new HeroConfig(100005, "董卓", 139, 192, 77, 87, 67, 18, 36, 285, 5, "洛阳", "安定", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "dongzhuo");
            config[105001] = new HeroConfig(105001, "张辽", 169, 222, 95, 92, 78, 56, 76, 397, 5, "长安", "邺", 10, 17, 0, 0, 1049, 0, 1, "马", null, "旋", "", "def", "SwordHitYellowCritical", "zhangliao");
            config[105002] = new HeroConfig(105002, "吕布", 156, 199, 91, 100, 26, 13, 40, 270, 5, "洛阳", "晋阳", 10, 17, 0, 0, 113, 0, 1, "车", null, "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu");
            config[105003] = new HeroConfig(105003, "华雄", 154, 191, 82, 90, 56, 40, 57, 325, 5, "洛阳", "长安", 10, 17, 0, 0, 297, 0, 1, "车", null, "纷", "", "atk", "SwordHitYellowCritical", "huaxiong");
            config[105004] = new HeroConfig(105004, "贾诩", 147, 223, 86, 50, 97, 85, 57, 375, 5, "安定", "武威", 10, 17, 30, 0, 713, 0, 3, "谋", null, "延", "", "inte", "StormExplosion", "jiaxu");
            config[105005] = new HeroConfig(105005, "貂蝉", 169, 195, 27, 65, 81, 70, 95, 338, 5, "洛阳", "晋阳", 10, 17, 30, 0, 373, 0, 3, "乐", null, "曲", "", "help", "StormExplosion", "diaochan");
            config[105006] = new HeroConfig(105006, "臧霸", 165, 227, 78, 75, 53, 56, 71, 333, 5, "长安", "北海", 10, 17, 0, 0, 341, 0, 1, "马", null, "虐", "", "atk", "SwordHitYellowCritical", "zangba");
            config[105007] = new HeroConfig(105007, "高顺", 158, 198, 85, 86, 54, 45, 68, 338, 5, "长安", "晋阳", 10, 17, 26, 8f, 373, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "gaoshun");
            config[105008] = new HeroConfig(105008, "李儒", 164, 192, 63, 43, 91, 78, 38, 313, 5, "洛阳", "长安", 10, 17, 30, 0, 240, 0, 3, "谋", null, "火", "", "inte", "ShadowExplosion", "liru");
            config[105009] = new HeroConfig(105009, "李傕", 160, 198, 69, 74, 24, 1, 17, 185, 5, "安定", "安定", 10, 17, 0, 0, 25, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "lijue");
            config[105010] = new HeroConfig(105010, "郭汜", 160, 197, 64, 76, 13, 14, 13, 180, 5, "安定", "武威", 10, 17, 0, 0, 25, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guosi");
            config[105011] = new HeroConfig(105011, "陈宫", 154, 199, 84, 55, 89, 82, 70, 380, 5, "洛阳", "濮阳", 10, 17, 30, 0, 779, 0, 3, "谋", null, "励", "溃", "inte", "ShadowExplosion", "chengong");
            config[105012] = new HeroConfig(105012, "胡车儿", 172, 198, 25, 80, 40, 1, 29, 175, 5, "安定", "北海", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "hucheer");
            config[105013] = new HeroConfig(105013, "魏续", 170, 198, 67, 78, 31, 32, 39, 247, 5, "洛阳", "平原", 10, 17, 0, 0, 75, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "weixu");
            config[105014] = new HeroConfig(105014, "宋宪", 171, 198, 68, 77, 38, 27, 31, 241, 5, "洛阳", "南皮", 10, 17, 0, 0, 68, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "songxian");
            config[105015] = new HeroConfig(105015, "徐荣", 158, 192, 80, 76, 57, 43, 42, 298, 5, "洛阳", "晋阳", 10, 17, 0, 0, 185, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xurong");
            config[105016] = new HeroConfig(105016, "侯成", 170, 198, 74, 75, 63, 56, 60, 328, 5, "洛阳", "安定", 10, 17, 0, 0, 313, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "houcheng");
            config[105017] = new HeroConfig(105017, "胡轸", 168, 191, 65, 74, 12, 15, 21, 187, 5, "洛阳", "天水", 10, 17, 0, 0, 26, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "huzhen");
            config[105018] = new HeroConfig(105018, "张绣", 167, 207, 80, 73, 60, 45, 59, 317, 5, "安定", "武威", 10, 17, 0, 0, 258, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangxiu");
            config[105019] = new HeroConfig(105019, "樊稠", 165, 197, 66, 73, 31, 24, 39, 233, 5, "安定", "武陵", 10, 17, 0, 0, 59, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "fanchou");
            config[105020] = new HeroConfig(105020, "曹性", 170, 198, 53, 73, 37, 26, 38, 227, 5, "洛阳", "零陵", 10, 17, 0, 0, 53, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "caoxing");
            config[105021] = new HeroConfig(105021, "李肃", 171, 192, 46, 69, 59, 15, 36, 225, 5, "洛阳", "江陵", 10, 17, 0, 0, 51, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lisu");
            config[105022] = new HeroConfig(105022, "张济", 176, 196, 69, 65, 51, 52, 54, 291, 5, "安定", "武威", 10, 17, 0, 0, 163, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangji");
            config[105023] = new HeroConfig(105023, "朱儁", 156, 195, 78, 65, 70, 74, 73, 360, 5, "洛阳", "桂阳", 10, 17, 0, 0, 548, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhujun");
            config[105024] = new HeroConfig(105024, "杨奉", 172, 215, 66, 65, 31, 14, 58, 234, 5, "洛阳", "长沙", 10, 17, 0, 0, 60, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yangfeng");
            config[105025] = new HeroConfig(105025, "牛辅", 165, 192, 38, 60, 21, 26, 37, 182, 5, "安定", "庐江", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "niufu");
            config[105026] = new HeroConfig(105026, "董旻", 165, 192, 49, 60, 25, 12, 23, 169, 5, "长安", "会稽", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "dongmin");
            config[105027] = new HeroConfig(105027, "皇甫嵩", 135, 195, 83, 58, 70, 48, 69, 328, 5, "安定", "柴桑", 10, 17, 0, 0, 313, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "huangpusong");
            config[105028] = new HeroConfig(105028, "董承", 167, 200, 56, 53, 65, 63, 75, 312, 5, "长安", "永安", 10, 17, 0, 0, 236, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "dongcheng");
            config[105029] = new HeroConfig(105029, "华歆", 157, 232, 18, 33, 82, 84, 17, 234, 5, "长安", "江州", 10, 17, 0, 0, 60, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "huaxin");
            config[105030] = new HeroConfig(105030, "王允", 137, 192, 25, 5, 67, 83, 73, 253, 5, "洛阳", "建宁", 10, 17, 0, 0, 84, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangyun");
            config[100006] = new HeroConfig(100006, "马腾", 155, 212, 82, 80, 51, 58, 88, 359, 6, "武威", "武威", 10, 17, 0, 0, 539, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "mateng");
            config[106001] = new HeroConfig(106001, "马超", 176, 222, 92, 97, 42, 25, 80, 336, 6, "武威", "武威", 10, 17, 0, 0, 360, 0, 1, "马", null, "铁", "", "atk", "SwordHitWhiteCritical", "machao");
            config[106002] = new HeroConfig(106002, "马岱", 183, 235, 80, 85, 54, 52, 71, 342, 6, "武威", "武威", 10, 17, 0, 0, 400, 0, 1, "马", null, "坚", "羽", "atk", "SwordHitYellowCritical", "madai");
            config[106003] = new HeroConfig(106003, "庞德", 176, 219, 89, 94, 67, 43, 67, 360, 6, "武威", "天水", 10, 17, 0, 0, 548, 0, 1, "枪", null, "坚", "", "atk", "SwordHitYellowCritical", "pangde");
            config[106004] = new HeroConfig(106004, "成公英", 170, 220, 70, 68, 76, 60, 65, 339, 6, "天水", "武威", 10, 17, 0, 0, 379, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenggongying");
            config[106005] = new HeroConfig(106005, "成宜", 175, 211, 72, 70, 40, 47, 54, 283, 6, "天水", "寿春", 10, 17, 0, 0, 142, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chengyi");
            config[106006] = new HeroConfig(106006, "侯选", 170, 211, 59, 62, 32, 52, 49, 254, 6, "天水", "北平", 10, 17, 0, 0, 85, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "houxuan");
            config[106007] = new HeroConfig(106007, "马休", 178, 212, 63, 67, 44, 41, 63, 278, 6, "武威", "蓟", 10, 17, 0, 0, 130, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "maxiu");
            config[106008] = new HeroConfig(106008, "马玩", 175, 211, 68, 71, 15, 22, 35, 211, 6, "天水", "蓟", 10, 17, 0, 0, 40, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "mawan");
            config[106009] = new HeroConfig(106009, "马铁", 179, 212, 65, 57, 52, 48, 57, 279, 6, "武威", "蓟", 10, 17, 0, 0, 132, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "matie");
            config[106010] = new HeroConfig(106010, "梁兴", 172, 211, 59, 63, 18, 21, 25, 186, 6, "天水", "长安", 10, 17, 0, 0, 26, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liangxing");
            config[106011] = new HeroConfig(106011, "程银", 170, 211, 67, 71, 39, 35, 49, 261, 6, "天水", "许昌", 10, 17, 0, 0, 96, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenying");
            config[106012] = new HeroConfig(106012, "杨秋", 175, 220, 64, 61, 55, 61, 40, 281, 6, "天水", "邺", 10, 17, 0, 0, 137, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yangqiu");
            config[106013] = new HeroConfig(106013, "阎行", 175, 215, 72, 84, 61, 58, 69, 344, 6, "天水", "天水", 10, 17, 0, 0, 414, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yanxing");
            config[106014] = new HeroConfig(106014, "韩遂", 158, 215, 84, 75, 77, 61, 80, 377, 6, "天水", "天水", 10, 17, 0, 0, 739, 0, 1, "马", null, "乱", "", "def", "SwordHitYellowCritical", "hansui");
            config[100007] = new HeroConfig(100007, "刘表", 142, 208, 46, 31, 68, 81, 80, 306, 7, "襄阳", "陈留", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "liubiao");
            config[107001] = new HeroConfig(107001, "魏延", 175, 234, 84, 91, 68, 49, 51, 343, 7, "武陵", "襄阳", 10, 17, 0, 0, 407, 0, 1, "戟", null, "破", "乱", "atk", "SwordHitYellowCritical", "weiyan");
            config[107002] = new HeroConfig(107002, "黄忠", 145, 220, 88, 93, 64, 52, 75, 372, 7, "武陵", "宛", 10, 17, 40, 5f, 677, 0, 3, "弓", null, "矢", "速", "shoot", "BulletExplosionFire", "huangzhong");
            config[107003] = new HeroConfig(107003, "王威", 165, 208, 60, 70, 59, 52, 66, 307, 7, "江夏", "宛", 10, 17, 0, 0, 216, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangwei");
            config[107004] = new HeroConfig(107004, "王粲", 177, 217, 5, 2, 79, 81, 52, 219, 7, "江夏", "陈留", 10, 17, 0, 0, 46, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangcan");
            config[107005] = new HeroConfig(107005, "黄祖", 158, 208, 73, 65, 52, 37, 31, 258, 7, "江夏", "江夏", 10, 17, 0, 0, 91, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "huangzu");
            config[107006] = new HeroConfig(107006, "韩嵩", 159, 215, 25, 15, 70, 78, 61, 249, 7, "襄阳", "襄阳", 10, 17, 0, 0, 78, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "hansong");
            config[107007] = new HeroConfig(107007, "苏飞", 162, 209, 66, 60, 63, 59, 60, 308, 7, "零陵", "小沛", 10, 17, 0, 0, 220, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "sufei");
            config[107008] = new HeroConfig(107008, "吴巨", 165, 211, 49, 61, 23, 51, 54, 238, 7, "江陵", "北海", 10, 17, 0, 0, 64, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wuju");
            config[107009] = new HeroConfig(107009, "刘磐", 168, 208, 67, 74, 46, 42, 53, 282, 7, "江陵", "平原", 10, 17, 0, 0, 139, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liupan");
            config[107010] = new HeroConfig(107010, "伊籍", 168, 223, 29, 24, 80, 86, 84, 303, 7, "零陵", "襄阳", 10, 17, 0, 0, 202, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yiji");
            config[107011] = new HeroConfig(107011, "张允", 168, 210, 72, 67, 42, 56, 48, 285, 7, "襄阳", "晋阳", 10, 17, 0, 0, 147, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangyun");
            config[107012] = new HeroConfig(107012, "蒯良", 162, 203, 68, 33, 88, 83, 71, 343, 7, "襄阳", "襄阳", 10, 17, 0, 0, 407, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "kuailiang");
            config[107013] = new HeroConfig(107013, "蒯越", 163, 214, 47, 27, 82, 89, 73, 318, 7, "襄阳", "襄阳", 10, 17, 0, 0, 262, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "kuaiyue");
            config[107015] = new HeroConfig(107015, "刘琦", 171, 209, 49, 11, 58, 68, 69, 255, 7, "襄阳", "武威", 10, 17, 0, 0, 87, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liuqi");
            config[107016] = new HeroConfig(107016, "蔡中", 170, 208, 39, 52, 1, 21, 42, 155, 7, "襄阳", "武陵", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "caizhong");
            config[107017] = new HeroConfig(107017, "蔡氏", 165, 208, 8, 7, 69, 58, 66, 208, 7, "襄阳", "零陵", 10, 17, 0, 0, 38, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "nvcaishi");
            config[107018] = new HeroConfig(107018, "蔡和", 172, 208, 38, 49, 1, 25, 44, 157, 7, "襄阳", "江陵", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "caihe");
            config[107019] = new HeroConfig(107019, "蔡瑁", 155, 208, 77, 70, 77, 72, 62, 358, 7, "襄阳", "襄阳", 10, 17, 0, 0, 529, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "caimao");
            config[100008] = new HeroConfig(100008, "刘璋", 165, 221, 15, 4, 8, 37, 64, 128, 8, "成都", "江夏", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "liuzhang");
            config[108001] = new HeroConfig(108001, "严颜", 158, 214, 79, 83, 69, 64, 79, 374, 8, "建宁", "长沙", 10, 17, 0, 0, 701, 0, 1, "士", null, "敏", "", "def", "SwordHitYellowCritical", "yanyan");
            config[108002] = new HeroConfig(108002, "李严", 171, 234, 82, 83, 72, 71, 50, 358, 8, "建宁", "庐江", 10, 17, 0, 0, 529, 0, 1, "士", null, "实", "", "def", "SwordHitYellowCritical", "liyan");
            config[108003] = new HeroConfig(108003, "张松", 165, 212, 15, 6, 88, 83, 19, 211, 8, "成都", "成都", 10, 17, 30, 0, 40, 0, 3, "扇", null, "", "", "inte", "FanExplosion", "zhangsong");
            config[108004] = new HeroConfig(108004, "董允", 185, 246, 67, 65, 78, 94, 79, 383, 8, "江州", "柴桑", 10, 17, 30, 0, 821, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "dongyun");
            config[108005] = new HeroConfig(108005, "孟获", 175, 225, 87, 87, 51, 55, 75, 355, 8, "云南", "云南", 10, 17, 0, 0, 502, 0, 1, "刀", null, "藤", "", "atk", "SwordHitYellowCritical", "menghuo");
            config[108006] = new HeroConfig(108006, "祝融", 180, 225, 77, 85, 43, 50, 78, 333, 8, "云南", "云南", 10, 17, 0, 0, 341, 0, 1, "刀", null, "藤", "", "def", "SwordHitYellowCritical", "zhurong");
            config[108007] = new HeroConfig(108007, "法正", 176, 220, 83, 52, 94, 79, 55, 363, 8, "江州", "长安", 10, 17, 30, 0, 578, 0, 3, "谋", null, "溃", "", "inte", "GasExplosionFire", "fazheng");
            config[108008] = new HeroConfig(108008, "黄权", 171, 240, 75, 59, 82, 81, 78, 375, 8, "建宁", "云南", 10, 17, 0, 0, 713, 0, 1, "枪", null, "缓", "", "atk", "SwordHitYellowCritical", "huangquan");
            config[108009] = new HeroConfig(108009, "孟达", 170, 228, 75, 73, 74, 67, 72, 361, 8, "江州", "梓潼", 10, 17, 40, 5f, 558, 0, 3, "弓", null, "乱", "", "shoot", "BulletExplosionBlue", "mengda");
            config[108010] = new HeroConfig(108010, "李恢", 174, 231, 79, 65, 79, 81, 78, 382, 8, "建宁", "上庸", 10, 17, 0, 0, 806, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "lihui");
            config[108011] = new HeroConfig(108011, "张任", 169, 214, 88, 84, 78, 59, 76, 385, 8, "建宁", "汉中", 10, 17, 40, 5f, 850, 0, 3, "弓", null, "复", "", "shoot", "BulletExplosionBlue", "zhangren");
            config[108012] = new HeroConfig(108012, "雷铜", 165, 218, 69, 78, 51, 37, 53, 288, 8, "梓潼", "汝南", 10, 17, 0, 0, 155, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "leitong");
            config[108013] = new HeroConfig(108013, "高沛", 168, 214, 66, 61, 69, 57, 52, 305, 8, "梓潼", "寿春", 10, 17, 0, 0, 209, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gaopei");
            config[108014] = new HeroConfig(108014, "杨怀", 166, 212, 62, 68, 68, 62, 53, 313, 8, "成都", "北平", 10, 17, 0, 0, 240, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yanghuai");
            config[108015] = new HeroConfig(108015, "吴兰", 168, 218, 62, 80, 35, 36, 50, 263, 8, "梓潼", "蓟", 10, 17, 0, 0, 100, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wulan");
            config[108016] = new HeroConfig(108016, "庞羲", 165, 214, 58, 36, 65, 71, 55, 285, 8, "梓潼", "襄平", 10, 17, 0, 0, 147, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "pangyi");
            config[108017] = new HeroConfig(108017, "王甫", 173, 222, 62, 41, 79, 79, 73, 334, 8, "江州", "洛阳", 10, 17, 0, 0, 347, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangfu");
            config[108018] = new HeroConfig(108018, "董和", 162, 220, 57, 34, 74, 88, 76, 329, 8, "江州", "长安", 10, 17, 0, 0, 318, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "donghe");
            config[108019] = new HeroConfig(108019, "吴懿", 166, 237, 83, 73, 68, 70, 77, 371, 8, "成都", "许昌", 10, 17, 0, 0, 665, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wuyi");
            config[108020] = new HeroConfig(108020, "吴班", 170, 231, 74, 71, 56, 45, 66, 312, 8, "成都", "邺", 10, 17, 0, 0, 236, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wuban");
            config[108021] = new HeroConfig(108021, "冷苞", 168, 214, 71, 82, 68, 37, 23, 281, 8, "建宁", "襄阳", 10, 17, 0, 0, 137, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lengbao");
            config[108022] = new HeroConfig(108022, "刘璝", 166, 214, 71, 73, 66, 44, 62, 316, 8, "永安", "襄阳", 10, 17, 0, 0, 253, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liukui");
            config[108023] = new HeroConfig(108023, "刘循", 178, 220, 61, 44, 39, 48, 55, 247, 8, "永安", "建业", 10, 17, 0, 0, 75, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liuxun");
            config[108024] = new HeroConfig(108024, "王累", 168, 214, 28, 30, 78, 81, 73, 290, 8, "成都", "吴", 10, 17, 0, 0, 160, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wanglei");
            config[108025] = new HeroConfig(108025, "秦宓", 174, 226, 15, 6, 71, 77, 75, 244, 8, "成都", "新野", 10, 17, 0, 0, 71, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "qinse");
            config[108026] = new HeroConfig(108026, "费诗", 170, 250, 15, 28, 64, 75, 66, 248, 8, "成都", "宛", 10, 17, 0, 0, 77, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "feishi");
            config[108027] = new HeroConfig(108027, "许靖", 155, 222, 2, 4, 64, 77, 65, 212, 8, "成都", "陈留", 10, 17, 0, 0, 41, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "xujing");
            config[100009] = new HeroConfig(100009, "张鲁", 155, 216, 51, 26, 74, 78, 75, 304, 9, "汉中", "濮阳", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "zhanglu");
            config[109001] = new HeroConfig(109001, "张卫", 170, 215, 71, 63, 43, 42, 58, 277, 9, "汉中", "下邳", 10, 17, 0, 0, 128, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangwei");
            config[109002] = new HeroConfig(109002, "杨任", 170, 215, 67, 75, 51, 38, 54, 285, 9, "上庸", "小沛", 10, 17, 0, 0, 147, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yangren");
            config[109003] = new HeroConfig(109003, "杨昂", 172, 215, 65, 69, 36, 33, 40, 243, 9, "汉中", "北海", 10, 17, 0, 0, 70, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yangang2");
            config[109004] = new HeroConfig(109004, "杨松", 168, 215, 1, 4, 27, 34, 4, 70, 9, "上庸", "平原", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yangsong");
            config[109005] = new HeroConfig(109005, "杨柏", 170, 215, 42, 43, 18, 25, 20, 148, 9, "上庸", "南皮", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yangbai");
            config[109006] = new HeroConfig(109006, "阎圃", 170, 215, 29, 25, 82, 80, 70, 286, 9, "上庸", "晋阳", 10, 17, 0, 0, 150, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yanpu");
            config[100010] = new HeroConfig(100010, "袁术", 155, 199, 67, 65, 65, 60, 45, 302, 10, "寿春", "汝南", 10, 17, 0, 0, 198, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshu");
            config[110001] = new HeroConfig(110001, "李丰", 165, 199, 69, 74, 50, 22, 47, 262, 10, "寿春", "天水", 10, 17, 0, 0, 98, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lifeng");
            config[110002] = new HeroConfig(110002, "纪灵", 166, 199, 78, 83, 51, 48, 55, 315, 10, "寿春", "武威", 10, 17, 0, 0, 249, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "jiling");
            config[110003] = new HeroConfig(110003, "袁胤", 168, 199, 17, 14, 39, 43, 46, 159, 10, "寿春", "武陵", 10, 17, 0, 0, 25, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yuanyin");
            config[110004] = new HeroConfig(110004, "袁涣", 170, 215, 30, 17, 68, 79, 83, 277, 10, "寿春", "零陵", 10, 17, 0, 0, 128, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yuanhuan");
            config[110005] = new HeroConfig(110005, "袁燿", 170, 230, 38, 47, 38, 48, 49, 220, 10, "寿春", "江陵", 10, 17, 0, 0, 47, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yuanyao");
            config[110006] = new HeroConfig(110006, "张勋", 166, 199, 72, 68, 41, 39, 59, 279, 10, "汝南", "江夏", 10, 17, 0, 0, 132, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zhangxun");
            config[110007] = new HeroConfig(110007, "梁纲", 165, 199, 60, 69, 41, 22, 46, 238, 10, "寿春", "桂阳", 10, 17, 0, 0, 64, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lianggang");
            config[110008] = new HeroConfig(110008, "陈纪", 164, 199, 58, 65, 43, 48, 32, 246, 10, "汝南", "长沙", 10, 17, 0, 0, 74, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenji");
            config[110009] = new HeroConfig(110009, "陈兰", 167, 199, 66, 69, 40, 24, 38, 237, 10, "寿春", "庐江", 10, 17, 0, 0, 63, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "chenlan");
            config[110010] = new HeroConfig(110010, "杨弘", 165, 200, 18, 15, 76, 62, 45, 216, 10, "寿春", "会稽", 10, 17, 0, 0, 44, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yanghong");
            config[110011] = new HeroConfig(110011, "雷薄", 168, 200, 62, 70, 36, 11, 15, 194, 10, "寿春", "柴桑", 10, 17, 0, 0, 29, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "leibo");
            config[110012] = new HeroConfig(110012, "刘勋", 165, 200, 47, 63, 35, 16, 32, 193, 10, "寿春", "永安", 10, 17, 0, 0, 29, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "liuxun2");
            config[110013] = new HeroConfig(110013, "乐就", 166, 199, 53, 66, 58, 42, 53, 272, 10, "汝南", "江州", 10, 17, 0, 0, 117, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "lejiu");
            config[110014] = new HeroConfig(110014, "桥蕤", 164, 199, 62, 67, 37, 40, 56, 262, 10, "寿春", "建宁", 10, 17, 0, 0, 98, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "qiaorui");
            config[110015] = new HeroConfig(110015, "阎象", 166, 199, 30, 27, 70, 75, 51, 253, 10, "寿春", "寿春", 10, 17, 0, 0, 84, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "yanxiang");
            config[110016] = new HeroConfig(110016, "韩胤", 168, 199, 26, 29, 64, 55, 44, 218, 10, "汝南", "梓潼", 10, 17, 0, 0, 45, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "hanyin");
            config[110017] = new HeroConfig(110017, "韩浩", 170, 215, 69, 72, 68, 88, 62, 359, 10, "寿春", "上庸", 10, 17, 0, 0, 539, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "hanhao");
            config[100011] = new HeroConfig(100011, "公孙瓒", 160, 199, 83, 81, 75, 46, 77, 362, 11, "北平", "汉中", 10, 17, 0, 0, 568, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "gongsunzan");
            config[111001] = new HeroConfig(111001, "公孙范", 165, 195, 73, 69, 64, 62, 61, 329, 11, "北平", "汝南", 10, 17, 0, 0, 318, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gongsunfan");
            config[111002] = new HeroConfig(111002, "公孙续", 170, 199, 59, 63, 50, 59, 62, 293, 11, "北平", "寿春", 10, 17, 0, 0, 169, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gongsunxu");
            config[111003] = new HeroConfig(111003, "王门", 168, 190, 65, 64, 31, 41, 49, 250, 11, "蓟", "北平", 10, 17, 0, 0, 79, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "wangmen");
            config[111004] = new HeroConfig(111004, "田豫", 171, 252, 77, 69, 77, 75, 72, 370, 11, "蓟", "蓟", 10, 17, 0, 0, 653, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "tianyu");
            config[111005] = new HeroConfig(111005, "田楷", 165, 204, 68, 65, 56, 61, 63, 313, 11, "蓟", "襄平", 10, 17, 0, 0, 240, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "tiankai");
            config[111006] = new HeroConfig(111006, "单经", 166, 199, 71, 68, 43, 49, 54, 285, 11, "北平", "洛阳", 10, 17, 0, 0, 147, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "shanjing");
            config[111007] = new HeroConfig(111007, "邹丹", 165, 195, 60, 63, 33, 36, 38, 230, 11, "北平", "长安", 10, 17, 0, 0, 56, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "zoudan");
            config[111008] = new HeroConfig(111008, "关靖", 160, 199, 36, 52, 72, 65, 42, 267, 11, "北平", "许昌", 10, 17, 0, 0, 107, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "guanjing");
            config[111009] = new HeroConfig(111009, "赵云", 168, 229, 91, 96, 76, 65, 81, 409, 11, "北平", "晋阳", 10, 17, 0, 0, 1294, 0, 1, "马", null, "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun");
            config[100012] = new HeroConfig(100012, "公孙度", 150, 204, 67, 71, 66, 51, 55, 310, 12, "襄平", "襄阳", 10, 17, 0, 0, 228, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "gongsundu");
            config[112001] = new HeroConfig(112001, "公孙恭", 170, 230, 37, 16, 64, 57, 39, 213, 12, "襄平", "襄阳", 10, 17, 0, 0, 41, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gongsungong");
            config[112002] = new HeroConfig(112002, "公孙康", 170, 221, 69, 63, 58, 55, 53, 298, 12, "襄平", "建业", 10, 17, 0, 0, 185, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "gongsunkang");
            config[100020] = new HeroConfig(100020, "司马炎", 236, 290, 69, 59, 77, 85, 75, 365, 99, "", "吴", 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "simayan");
            config[120001] = new HeroConfig(120001, "邓艾", 197, 264, 94, 87, 89, 88, 75, 433, 99, "", "新野", 10, 17, 0, 0, 1971, 0, 1, "枪", null, "奇", "", "def", "SwordHitYellowCritical", "dengai");
            config[120002] = new HeroConfig(120002, "司马师", 208, 255, 80, 67, 87, 82, 70, 386, 99, "", "宛", 10, 17, 36, 0, 865, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "simashi");
            config[120003] = new HeroConfig(120003, "司马昭", 211, 265, 78, 57, 87, 84, 65, 371, 99, "", "陈留", 10, 17, 36, 0, 665, 0, 3, "相", null, "溃", "", "help", "SharpExplosionGreen", "simazhao");
            config[120004] = new HeroConfig(120004, "羊祜", 221, 278, 90, 64, 84, 90, 92, 420, 99, "", "濮阳", 10, 17, 0, 0, 1570, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "yangku");
            config[120005] = new HeroConfig(120005, "钟会", 225, 264, 82, 58, 92, 85, 65, 382, 99, "", "下邳", 10, 17, 30, 0, 806, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhonghui");
            config[120006] = new HeroConfig(120006, "陈泰", 200, 260, 86, 77, 84, 72, 71, 390, 99, "", "小沛", 10, 17, 0, 0, 928, 0, 1, "士", null, "虐", "", "def", "SwordHitYellowCritical", "chentai");
            config[120007] = new HeroConfig(120007, "杜预", 222, 285, 84, 30, 85, 89, 78, 366, 99, "", "北海", 10, 17, 36, 0, 609, 0, 3, "相", null, "米", "", "inte", "SharpExplosionGreen", "duyu");
            config[120008] = new HeroConfig(120008, "王濬", 206, 286, 80, 52, 79, 82, 75, 368, 99, "", "平原", 10, 17, 0, 0, 631, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "wangrui");
            config[120009] = new HeroConfig(120009, "辛宪英", 191, 269, 42, 28, 84, 80, 82, 316, 99, "", "南皮", 10, 17, 30, 0, 253, 0, 3, "扇", null, "缓", "", "inte", "FanExplosion", "xinxianying");
            config[199001] = new HeroConfig(199001, "孙策", 175, 200, 96, 93, 74, 75, 94, 432, 99, "", "吴", 10, 17, 0, 0, 1937, 0, 1, "车", null, "虎", "", "atk", "SwordHitYellowCritical", "sunce");
            config[199002] = new HeroConfig(199002, "诸葛亮", 181, 234, 98, 45, 100, 99, 92, 434, 99, "", "襄阳", 10, 17, 36, 0, 2006, 0, 3, "谋", null, "神", "空", "inte", "LightningExplosionYellow", "zhugeliang");
            config[199003] = new HeroConfig(199003, "姜维", 202, 264, 92, 89, 90, 66, 79, 416, 99, "", "天水", 10, 17, 0, 0, 1463, 0, 1, "车", null, "解", "", "def", "SwordHitYellowCritical", "jiangwei");
            config[199004] = new HeroConfig(199004, "关兴", 199, 220, 80, 85, 60, 56, 73, 354, 99, "", "武威", 10, 17, 0, 0, 493, 0, 1, "马", null, "奋", "", "atk", "SwordHitYellowCritical", "guanxing");
            config[199006] = new HeroConfig(199006, "庞统", 179, 214, 85, 47, 98, 86, 65, 381, 99, "", "襄阳", 10, 17, 30, 0, 792, 0, 3, "谋", null, "锁", "火", "inte", "ExplosionFireballFire", "pangtong");
            config[199007] = new HeroConfig(199007, "张苞", 199, 231, 75, 87, 47, 45, 67, 321, 99, "", "零陵", 10, 17, 0, 0, 277, 0, 1, "枪", null, "乱", "", "atk", "SwordHitYellowCritical", "zhangbao");
            config[199008] = new HeroConfig(199008, "关索", 200, 263, 74, 81, 50, 46, 72, 323, 99, "", "江陵", 10, 17, 50, 5f, 286, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "guansuo");
            config[199009] = new HeroConfig(199009, "黄月英", 183, 234, 58, 34, 86, 85, 70, 333, 99, "", "江夏", 10, 17, 26, 6f, 341, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "huangyueying");
            config[199010] = new HeroConfig(199010, "刘禅", 207, 271, 12, 15, 27, 40, 52, 146, 99, "", "新野", 10, 17, 58, 0, 25, 0, 3, "鼓", null, "碉", "", "help", "SoulExplosionOrange", "liushan");
            config[199011] = new HeroConfig(199011, "刘巴", 170, 222, 33, 24, 78, 85, 65, 285, 99, "", "长沙", 10, 17, 30, 0, 147, 0, 3, "扇", null, "纷", "", "inte", "FanExplosion", "liuba");
            config[199012] = new HeroConfig(199012, "司马懿", 179, 251, 98, 63, 98, 93, 87, 439, 99, "", "庐江", 10, 17, 30, 0, 2190, 0, 3, "谋", null, "鬼", "", "inte", "ShadowExplosion", "simayi");
            config[199013] = new HeroConfig(199013, "夏侯霸", 200, 259, 82, 77, 69, 53, 68, 349, 99, "", "会稽", 10, 17, 0, 0, 452, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "xiahouba");
            config[199014] = new HeroConfig(199014, "王双", 195, 231, 68, 88, 19, 22, 27, 224, 99, "", "柴桑", 10, 17, 0, 0, 50, 0, 1, "枪", null, "透", "", "atk", "SwordHitYellowCritical", "wangshuang");
            config[199015] = new HeroConfig(199015, "文鸯", 238, 291, 76, 91, 64, 65, 68, 364, 99, "", "永安", 10, 17, 40, 5f, 588, 0, 3, "弓", null, "速", "", "shoot", "BulletExplosionBlue", "wenyuan");
            config[199017] = new HeroConfig(199017, "毌丘俭", 195, 255, 78, 75, 50, 54, 52, 309, 99, "", "江州", 10, 17, 0, 0, 224, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guanqiujian");
            config[199021] = new HeroConfig(199021, "孙坚", 155, 191, 93, 90, 77, 78, 92, 430, 99, "", "吴", 10, 17, 0, 0, 1870, 0, 1, "枪", null, "旋", "", "atk", "SwordHitYellowCritical", "sunjian");
            config[199022] = new HeroConfig(199022, "诸葛恪", 203, 253, 72, 47, 90, 80, 57, 346, 99, "", "北海", 10, 17, 30, 0, 429, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhugege");
            config[199023] = new HeroConfig(199023, "华佗", 145, 208, 60, 34, 77, 70, 85, 326, 99, "", "梓潼", 10, 17, 28, 0, 302, 0, 3, "医", null, "药", "", "help", "ShadowExplosionGreen", "huatuo");
            config[199024] = new HeroConfig(199024, "于吉", 164, 200, 47, 41, 73, 65, 70, 296, 99, "", "上庸", 10, 17, 28, 0, 178, 0, 3, "医", null, "调", "", "help", "ShadowExplosionGreen", "yuji");
            config[199025] = new HeroConfig(199025, "张角", 156, 184, 87, 29, 86, 82, 88, 372, 99, "", "汉中", 10, 17, 30, 0, 677, 0, 3, "谋", null, "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao");
            config[199026] = new HeroConfig(199026, "张宝", 155, 184, 83, 71, 81, 78, 75, 388, 99, "", "汝南", 10, 17, 0, 0, 896, 0, 1, "枪", null, "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2");
            config[199027] = new HeroConfig(199027, "张梁", 156, 184, 78, 80, 74, 75, 70, 377, 99, "", "寿春", 10, 17, 26, 8f, 739, 0, 3, "炮", null, "", "", "def", "SwordHitYellowCritical", "zhangliang");
            config[199029] = new HeroConfig(199029, "王异", 170, 214, 73, 51, 82, 75, 78, 359, 99, "", "北平", 10, 17, 0, 0, 539, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "wangyi");
            config[199030] = new HeroConfig(199030, "蔡琰", 177, 249, 61, 13, 77, 75, 82, 308, 99, "", "蓟", 10, 17, 30, 0, 220, 0, 3, "乐", null, "碉", "", "help", "StormExplosion", "caiyan");
            config[199031] = new HeroConfig(199031, "马谡", 190, 228, 70, 72, 88, 70, 65, 365, 99, "", "襄阳", 99, 17, 30, 0, 599, 0, 3, "谋", null, "百", "", "inte", "StormExplosion", "masu");
            config[199032] = new HeroConfig(199032, "马良", 187, 222, 68, 60, 93, 87, 86, 394, 99, "", "襄阳", 99, 17, 36, 0, 995, 0, 3, "相", null, "静", "", "help", "SharpExplosionGreen", "maliang");
            config[199033] = new HeroConfig(199033, "蒋琬", 193, 246, 64, 52, 85, 97, 81, 379, 99, "", "零陵", 10, 17, 36, 0, 765, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "jiangwan");
            config[199034] = new HeroConfig(199034, "费祎", 185, 253, 68, 42, 83, 95, 83, 371, 99, "", "江夏", 10, 17, 36, 0, 665, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "feiyi");
            config[199035] = new HeroConfig(199035, "郭攸之", 180, 243, 63, 48, 82, 80, 75, 348, 99, "", "邺", 10, 17, 58, 0, 444, 0, 3, "鼓", null, "陷", "", "help", "SoulExplosionOrange", "guoyouzhi");
            config[199036] = new HeroConfig(199036, "邓芝", 178, 251, 70, 71, 80, 89, 87, 397, 99, "", "新野", 10, 17, 0, 0, 1049, 0, 1, "枪", null, "境", "", "def", "SwordHitYellowCritical", "dengzhi");
            config[199037] = new HeroConfig(199037, "王平", 175, 248, 83, 78, 75, 58, 51, 345, 99, "", "汉中", 10, 17, 0, 0, 421, 0, 1, "戟", null, "伏", "坚", "def", "SwordHitYellowCritical", "wangping");
            config[199038] = new HeroConfig(199038, "陆抗", 226, 274, 91, 63, 87, 88, 86, 415, 99, "", "建业", 10, 17, 50, 5f, 1438, 0, 3, "弩", null, "透", "", "shoot", "BulletExplosionBlue", "lukang");
            config[199039] = new HeroConfig(199039, "刘封", 192, 220, 75, 79, 44, 55, 50, 303, 99, "", "吴", 10, 17, 50, 5f, 202, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "liufeng");
            config[199040] = new HeroConfig(199040, "孙桓", 198, 222, 82, 73, 76, 75, 76, 382, 99, "", "新野", 10, 17, 0, 0, 806, 0, 1, "戟", null, "竟", "", "def", "SwordHitYellowCritical", "sunhuan");
            config[199041] = new HeroConfig(199041, "太史享", 190, 256, 53, 62, 45, 55, 56, 271, 99, "", "宛", 10, 17, 0, 0, 115, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "taishixiang");
            config[199042] = new HeroConfig(199042, "全琮", 198, 249, 75, 69, 68, 59, 64, 335, 99, "", "陈留", 10, 17, 0, 0, 354, 0, 1, "戟", null, "实", "", "def", "SwordHitYellowCritical", "quanzong");
            config[199043] = new HeroConfig(199043, "骆统", 193, 236, 69, 53, 69, 70, 70, 331, 99, "", "濮阳", 10, 17, 0, 0, 330, 0, 1, "刀", null, "", "", "def", "SwordHitWhiteCritical", "luotong");



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
