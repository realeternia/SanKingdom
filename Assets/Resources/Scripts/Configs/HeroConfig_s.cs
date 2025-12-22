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
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///攻击力
        /// </summary>
        public int Atk;
        /// <summary>
        ///统帅
        /// </summary>
        public int LeadShip;
        /// <summary>
        ///智力
        /// </summary>
        public int Inte;
        /// <summary>
        ///武力
        /// </summary>
        public int Str;
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
        public int Side;
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


        public HeroConfig(int Id, string Name, int Lv, int Atk, int LeadShip, int Inte, int Str, int Fair, int Charm, int Total, int Hp, int Side, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, int RateWeight, int RateAbs, int Pos, string Job, int[] Skills, string Skill1, string Skill2, string Group, string HitEffect, string Icon)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.Atk = Atk;
            this.LeadShip = LeadShip;
            this.Inte = Inte;
            this.Str = Str;
            this.Fair = Fair;
            this.Charm = Charm;
            this.Total = Total;
            this.Hp = Hp;
            this.Side = Side;
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
            config[100001] = new HeroConfig(100001, "刘备", 1, 26, 80, 78, 77, 82, 99, 416, 330, 1, 10, 17, 0, 0, 0, 100, 1, "帅", null, "仁", "", "core", "SwordHitYellowCritical", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 1, 32, 98, 92, 81, 96, 95, 462, 240, 2, 10, 17, 0, 0, 0, 100, 1, "帅", null, "识", "", "core", "SwordHitYellowCritical", "caocao");
            config[100003] = new HeroConfig(100003, "孙权", 1, 26, 79, 80, 70, 88, 92, 409, 350, 3, 10, 17, 0, 0, 0, 100, 1, "帅", null, "衡", "", "core", "SwordHitYellowCritical", "sunquan");
            config[100004] = new HeroConfig(100004, "董卓", 1, 28, 85, 75, 87, 68, 35, 350, 330, 4, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "dongzhuo");
            config[100005] = new HeroConfig(100005, "司马炎", 1, 23, 69, 77, 59, 85, 75, 365, 385, 5, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "simayan");
            config[100006] = new HeroConfig(100006, "袁绍", 1, 28, 86, 82, 73, 79, 86, 406, 310, 6, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshao");
            config[100007] = new HeroConfig(100007, "刘表", 1, 10, 31, 72, 31, 81, 69, 284, 310, 6, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "liubiao");
            config[100008] = new HeroConfig(100008, "刘璋", 1, 10, 31, 34, 15, 38, 65, 183, 310, 6, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "liuzhang");
            config[100009] = new HeroConfig(100009, "张鲁", 1, 17, 51, 74, 26, 78, 75, 304, 310, 6, 10, 17, 0, 0, 0, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "zhanglu");
            config[100010] = new HeroConfig(100010, "袁术", 1, 22, 67, 65, 65, 60, 45, 302, 335, 10, 10, 17, 0, 0, 198, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "yuanshu");
            config[100011] = new HeroConfig(100011, "马腾", 1, 27, 82, 51, 80, 72, 80, 365, 290, 10, 10, 17, 0, 0, 599, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "mateng");
            config[100012] = new HeroConfig(100012, "公孙瓒", 1, 27, 83, 75, 82, 75, 78, 393, 295, 10, 10, 17, 0, 0, 978, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "gongsunzan");
            config[100013] = new HeroConfig(100013, "公孙度", 1, 22, 67, 66, 71, 51, 55, 310, 295, 10, 10, 17, 0, 0, 228, 0, 1, "帅", null, "", "", "core", "SwordHitYellowCritical", "gongsundu");
            config[101001] = new HeroConfig(101001, "赵云", 1, 30, 91, 76, 96, 75, 93, 431, 300, 1, 10, 17, 0, 0, 1903, 0, 1, "马", null, "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun");
            config[101002] = new HeroConfig(101002, "张飞", 1, 30, 92, 48, 98, 45, 70, 353, 310, 1, 10, 17, 0, 0, 485, 0, 1, "枪", null, "威", "", "atk", "SwordHitYellowCritical", "zhangfei");
            config[101003] = new HeroConfig(101003, "马超", 1, 30, 92, 54, 97, 50, 80, 373, 335, 1, 10, 17, 0, 0, 689, 0, 1, "马", null, "铁", "", "atk", "SwordHitWhiteCritical", "machao");
            config[101004] = new HeroConfig(101004, "诸葛亮", 1, 32, 98, 100, 45, 99, 92, 434, 245, 1, 10, 17, 18, 0, 2006, 0, 3, "谋", null, "神", "空", "inte", "LightningExplosionYellow", "zhugeliang");
            config[101005] = new HeroConfig(101005, "关羽", 1, 32, 97, 77, 97, 75, 94, 440, 260, 1, 10, 17, 0, 0, 2229, 0, 1, "车", null, "斩", "", "atk", "SwordHitGreenCritical", "guanyu");
            config[101006] = new HeroConfig(101006, "徐庶", 1, 29, 87, 93, 65, 82, 84, 411, 230, 1, 10, 17, 15, 0, 1341, 0, 3, "谋", null, "火", "共", "inte", "GasExplosionFire", "xusu");
            config[101007] = new HeroConfig(101007, "魏延", 1, 28, 84, 67, 91, 68, 55, 365, 300, 1, 10, 17, 0, 0, 599, 0, 1, "戟", null, "破", "乱", "atk", "SwordHitYellowCritical", "weiyan");
            config[101008] = new HeroConfig(101008, "黄忠", 1, 30, 90, 64, 93, 70, 82, 399, 210, 1, 10, 17, 22, 1.5f, 1086, 0, 3, "弓", null, "矢", "速", "shoot", "BulletExplosionFire", "huangzhong");
            config[101009] = new HeroConfig(101009, "周仓", 1, 21, 63, 55, 82, 58, 65, 323, 335, 1, 10, 17, 0, 0, 286, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "zhoucang");
            config[101010] = new HeroConfig(101010, "姜维", 1, 30, 92, 90, 89, 85, 88, 444, 295, 1, 10, 17, 0, 0, 2390, 0, 1, "车", null, "解", "", "def", "SwordHitYellowCritical", "jiangwei");
            config[101011] = new HeroConfig(101011, "马岱", 1, 26, 80, 66, 85, 72, 78, 381, 300, 1, 10, 17, 0, 0, 792, 0, 1, "马", null, "坚", "羽", "atk", "SwordHitYellowCritical", "madai");
            config[101012] = new HeroConfig(101012, "关兴", 1, 26, 80, 72, 85, 70, 80, 387, 300, 1, 10, 17, 0, 0, 880, 0, 1, "马", null, "奋", "", "atk", "SwordHitYellowCritical", "guanxing");
            config[101013] = new HeroConfig(101013, "关平", 1, 26, 79, 72, 82, 71, 78, 382, 290, 1, 10, 17, 0, 0, 806, 0, 1, "戟", null, "连", "", "def", "SwordHitYellowCritical", "guanping");
            config[101014] = new HeroConfig(101014, "严颜", 1, 26, 79, 71, 83, 73, 79, 385, 280, 1, 10, 17, 0, 0, 850, 0, 1, "士", null, "敏", "", "def", "SwordHitYellowCritical", "yanyan");
            config[101015] = new HeroConfig(101015, "廖化", 1, 24, 74, 68, 78, 70, 72, 362, 330, 1, 10, 17, 0, 0, 568, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "liaohua");
            config[101016] = new HeroConfig(101016, "孟获", 1, 29, 87, 51, 87, 55, 75, 355, 310, 1, 10, 17, 0, 0, 502, 0, 1, "刀", null, "藤", "", "atk", "SwordHitYellowCritical", "menghuo");
            config[101017] = new HeroConfig(101017, "庞统", 1, 28, 85, 98, 47, 86, 65, 381, 250, 1, 10, 17, 15, 0, 792, 0, 3, "谋", null, "锁", "火", "inte", "ExplosionFireballFire", "pangtong");
            config[101018] = new HeroConfig(101018, "李严", 1, 27, 82, 75, 83, 80, 70, 390, 290, 1, 10, 17, 0, 0, 928, 0, 1, "士", null, "实", "", "def", "SwordHitYellowCritical", "liyan");
            config[101019] = new HeroConfig(101019, "张苞", 1, 25, 75, 82, 87, 65, 76, 385, 305, 1, 10, 17, 0, 0, 850, 0, 1, "枪", null, "乱", "", "atk", "SwordHitYellowCritical", "zhangbao");
            config[101020] = new HeroConfig(101020, "张松", 1, 21, 65, 89, 46, 82, 30, 312, 240, 1, 10, 17, 15, 0, 236, 0, 3, "扇", null, "", "", "inte", "FanExplosion", "zhangsong");
            config[101021] = new HeroConfig(101021, "关索", 1, 24, 74, 69, 81, 68, 75, 367, 215, 1, 10, 17, 30, 1.5f, 620, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "guansuo");
            config[101022] = new HeroConfig(101022, "简雍", 1, 24, 72, 69, 65, 75, 70, 351, 240, 1, 10, 17, 22, 1.5f, 468, 0, 3, "弓", null, "破", "", "shoot", "BulletExplosionBlue", "jianyong");
            config[101023] = new HeroConfig(101023, "蒋琬", 1, 21, 64, 85, 52, 92, 85, 378, 240, 1, 10, 17, 18, 0, 752, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "jiangwan");
            config[101024] = new HeroConfig(101024, "孙乾", 1, 20, 62, 80, 54, 84, 82, 362, 250, 1, 10, 17, 35, 0, 568, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "sunqian");
            config[101025] = new HeroConfig(101025, "费祎", 1, 22, 68, 86, 42, 90, 88, 374, 250, 1, 10, 17, 18, 0, 701, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "feiyi");
            config[101026] = new HeroConfig(101026, "马谡", 1, 23, 70, 88, 72, 78, 65, 373, 235, 1, 10, 17, 15, 0, 689, 0, 3, "谋", null, "百", "", "inte", "StormExplosion", "masu");
            config[101027] = new HeroConfig(101027, "董允", 1, 22, 67, 85, 65, 88, 83, 388, 280, 1, 10, 17, 15, 0, 896, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "dongyun");
            config[101028] = new HeroConfig(101028, "王平", 1, 27, 83, 75, 78, 72, 75, 383, 300, 1, 10, 17, 0, 0, 821, 0, 1, "戟", null, "伏", "坚", "def", "SwordHitYellowCritical", "wangping");
            config[101029] = new HeroConfig(101029, "邓芝", 1, 23, 70, 80, 71, 79, 84, 384, 290, 1, 10, 17, 0, 0, 835, 0, 1, "枪", null, "境", "", "def", "SwordHitYellowCritical", "dengzhi");
            config[101030] = new HeroConfig(101030, "郭攸之", 1, 21, 63, 82, 48, 80, 75, 348, 260, 1, 10, 17, 35, 0, 444, 0, 3, "鼓", null, "陷", "", "help", "SoulExplosionOrange", "guoyouzhi");
            config[101031] = new HeroConfig(101031, "马良", 1, 22, 68, 93, 60, 87, 86, 394, 270, 1, 10, 17, 18, 0, 995, 0, 3, "相", null, "静", "", "help", "SharpExplosionGreen", "maliang");
            config[101032] = new HeroConfig(101032, "黄月英", 1, 19, 58, 86, 34, 85, 70, 333, 190, 1, 10, 17, 13, 2.5f, 341, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "huangyueying");
            config[101033] = new HeroConfig(101033, "法正", 1, 27, 83, 94, 52, 81, 62, 372, 275, 1, 10, 17, 15, 0, 677, 0, 3, "谋", null, "溃", "", "inte", "GasExplosionFire", "fazheng");
            config[101034] = new HeroConfig(101034, "刘禅", 1, 9, 27, 35, 41, 40, 60, 203, 360, 1, 10, 17, 35, 0, 35, 0, 3, "鼓", null, "碉", "", "help", "SoulExplosionOrange", "liushan");
            config[101035] = new HeroConfig(101035, "祝融", 1, 25, 77, 43, 85, 50, 78, 333, 290, 1, 10, 17, 0, 0, 341, 0, 1, "刀", null, "藤", "", "def", "SwordHitYellowCritical", "zhurong");
            config[101036] = new HeroConfig(101036, "黄权", 1, 25, 75, 82, 59, 83, 80, 379, 310, 1, 10, 17, 0, 0, 765, 0, 1, "枪", null, "缓", "", "atk", "SwordHitYellowCritical", "huangquan");
            config[101037] = new HeroConfig(101037, "孟达", 1, 25, 75, 68, 73, 70, 45, 331, 250, 1, 10, 17, 22, 1.5f, 330, 0, 3, "弓", null, "乱", "", "shoot", "BulletExplosionBlue", "mengda");
            config[101038] = new HeroConfig(101038, "刘封", 1, 25, 75, 44, 79, 55, 50, 303, 220, 1, 10, 17, 30, 1.5f, 202, 0, 3, "弩", null, "", "", "shoot", "BulletExplosionBlue", "liufeng");
            config[101039] = new HeroConfig(101039, "李恢", 1, 26, 79, 79, 65, 81, 78, 382, 315, 1, 10, 17, 0, 0, 806, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "lihui");
            config[101040] = new HeroConfig(101040, "刘巴", 1, 11, 33, 78, 24, 85, 65, 285, 290, 1, 10, 17, 15, 0, 147, 0, 3, "扇", null, "纷", "", "inte", "FanExplosion", "liuba");
            config[102001] = new HeroConfig(102001, "郭嘉", 1, 24, 72, 98, 43, 78, 82, 373, 270, 2, 10, 17, 15, 0, 689, 0, 3, "谋", null, "天", "", "inte", "LightningExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 1, 30, 91, 63, 91, 75, 85, 405, 305, 2, 10, 17, 0, 0, 1207, 0, 1, "车", null, "青", "", "atk", "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 1, 22, 67, 96, 47, 98, 95, 403, 285, 2, 10, 17, 18, 0, 1165, 0, 3, "相", null, "国", "", "help", "FrostExplosionBlue", "xunyu");
            config[102004] = new HeroConfig(102004, "张辽", 1, 31, 95, 78, 92, 80, 88, 433, 295, 2, 10, 17, 0, 0, 1971, 0, 1, "马", null, "旋", "", "def", "SwordHitYellowCritical", "zhangliao");
            config[102005] = new HeroConfig(102005, "许褚", 1, 21, 65, 44, 96, 40, 70, 315, 390, 2, 10, 17, 0, 0, 249, 0, 1, "士", null, "斧", "", "atk", "SwordHitYellowCritical", "xuchu");
            config[102006] = new HeroConfig(102006, "夏侯渊", 1, 30, 90, 68, 89, 72, 80, 399, 230, 2, 10, 17, 22, 1.5f, 1086, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan");
            config[102007] = new HeroConfig(102007, "典韦", 1, 19, 59, 43, 95, 38, 78, 313, 415, 2, 10, 17, 0, 0, 240, 0, 1, "士", null, "护", "", "def", "SwordHitYellowCritical", "dianwei");
            config[102008] = new HeroConfig(102008, "张郃", 1, 29, 89, 75, 90, 76, 80, 410, 300, 2, 10, 17, 0, 0, 1317, 0, 1, "车", null, "分", "", "def", "SwordHitYellowCritical", "zhanghe");
            config[102009] = new HeroConfig(102009, "徐晃", 1, 30, 90, 77, 91, 78, 82, 418, 215, 2, 10, 17, 22, 1.5f, 1516, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "xuhuang");
            config[102010] = new HeroConfig(102010, "荀攸", 1, 21, 63, 92, 53, 85, 80, 373, 270, 2, 10, 17, 15, 0, 689, 0, 3, "谋", null, "百", "米", "inte", "FrostExplosionBlue", "xunyou");
            config[102011] = new HeroConfig(102011, "于禁", 1, 26, 80, 72, 75, 78, 65, 370, 280, 2, 10, 17, 0, 0, 653, 0, 1, "戟", null, "青", "破", "def", "SwordHitYellowCritical", "yujin");
            config[102012] = new HeroConfig(102012, "曹仁", 1, 30, 90, 62, 86, 82, 84, 404, 300, 2, 10, 17, 0, 0, 1186, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caoren");
            config[102013] = new HeroConfig(102013, "曹洪", 1, 27, 82, 51, 83, 65, 60, 341, 310, 2, 10, 17, 0, 0, 393, 0, 1, "枪", null, "商", "", "atk", "SwordHitYellowCritical", "caohong");
            config[102014] = new HeroConfig(102014, "庞德", 1, 29, 89, 74, 94, 70, 83, 410, 280, 2, 10, 17, 0, 0, 1317, 0, 1, "枪", null, "坚", "", "atk", "SwordHitYellowCritical", "pangde");
            config[102015] = new HeroConfig(102015, "乐进", 1, 26, 80, 55, 84, 68, 75, 362, 280, 2, 10, 17, 0, 0, 568, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "lejin");
            config[102016] = new HeroConfig(102016, "文聘", 1, 26, 80, 65, 82, 75, 78, 380, 335, 2, 10, 17, 0, 0, 779, 0, 1, "戟", null, "透", "劫", "atk", "SwordHitYellowCritical", "wenpin");
            config[102017] = new HeroConfig(102017, "曹休", 1, 24, 73, 72, 73, 70, 72, 360, 250, 2, 10, 17, 22, 1.5f, 548, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "caoxiu");
            config[102018] = new HeroConfig(102018, "司马懿", 1, 32, 98, 98, 63, 95, 82, 436, 220, 2, 10, 17, 15, 0, 2078, 0, 3, "谋", null, "鬼", "", "inte", "ShadowExplosion", "simayi");
            config[102019] = new HeroConfig(102019, "夏侯霸", 1, 27, 82, 69, 77, 72, 75, 375, 320, 2, 10, 17, 0, 0, 713, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "xiahouba");
            config[102020] = new HeroConfig(102020, "郝昭", 1, 29, 87, 76, 79, 78, 80, 400, 300, 2, 10, 17, 0, 0, 1106, 0, 1, "枪", null, "坚", "", "def", "SwordHitYellowCritical", "haozhao");
            config[102021] = new HeroConfig(102021, "王双", 1, 22, 68, 38, 88, 50, 65, 309, 380, 2, 10, 17, 0, 0, 224, 0, 1, "枪", null, "透", "", "atk", "SwordHitYellowCritical", "wangshuang");
            config[102022] = new HeroConfig(102022, "程昱", 1, 21, 63, 90, 54, 83, 70, 360, 240, 2, 10, 17, 15, 0, 548, 0, 3, "谋", null, "识", "火", "inte", "StormExplosion", "chengyu");
            config[102023] = new HeroConfig(102023, "杨修", 1, 20, 60, 88, 57, 75, 45, 325, 260, 2, 10, 17, 18, 0, 297, 0, 3, "相", null, "虐", "", "help", "SharpExplosionGreen", "yangxiu");
            config[102024] = new HeroConfig(102024, "牛金", 1, 23, 71, 65, 77, 68, 70, 351, 330, 2, 10, 17, 0, 0, 468, 0, 1, "刀", null, "伏", "", "atk", "SwordHitYellowCritical", "niujin");
            config[102025] = new HeroConfig(102025, "文鸯", 1, 25, 76, 64, 91, 65, 75, 371, 240, 2, 10, 17, 22, 1.5f, 665, 0, 3, "弓", null, "速", "", "shoot", "BulletExplosionBlue", "wenyuan");
            config[102026] = new HeroConfig(102026, "曹真", 1, 27, 82, 70, 74, 79, 80, 385, 310, 2, 10, 17, 0, 0, 850, 0, 1, "戟", null, "境", "", "def", "SwordHitYellowCritical", "caozhen");
            config[102027] = new HeroConfig(102027, "陈群", 1, 21, 65, 84, 45, 90, 75, 359, 270, 2, 10, 17, 15, 0, 539, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "chenqun");
            config[102028] = new HeroConfig(102028, "李典", 1, 24, 74, 71, 73, 78, 80, 376, 300, 2, 10, 17, 0, 0, 726, 0, 1, "枪", null, "伏", "坚", "def", "SwordHitYellowCritical", "lidian");
            config[102029] = new HeroConfig(102029, "曹丕", 1, 26, 78, 79, 79, 85, 78, 399, 320, 2, 10, 17, 0, 0, 1086, 0, 1, "刀", null, "敏", "", "def", "SwordHitYellowCritical", "caopi");
            config[102030] = new HeroConfig(102030, "曹植", 1, 21, 64, 83, 67, 75, 85, 374, 300, 2, 10, 17, 15, 0, 701, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "caozhi");
            config[102031] = new HeroConfig(102031, "刘晔", 1, 21, 65, 88, 49, 82, 70, 354, 200, 2, 10, 17, 13, 2.5f, 493, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "liuye");
            config[102032] = new HeroConfig(102032, "朱灵", 1, 24, 73, 60, 77, 70, 68, 348, 210, 2, 10, 17, 13, 2.5f, 444, 0, 3, "炮", null, "", "", "atk", "SwordHitYellowCritical", "zhuling");
            config[102033] = new HeroConfig(102033, "曹彰", 1, 27, 82, 43, 90, 55, 75, 345, 320, 2, 10, 17, 0, 0, 421, 0, 1, "枪", null, "青", "", "atk", "SwordHitYellowCritical", "caozhang");
            config[102034] = new HeroConfig(102034, "毌丘俭", 1, 26, 78, 55, 75, 72, 70, 350, 340, 2, 10, 17, 0, 0, 460, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guanqiujian");
            config[102035] = new HeroConfig(102035, "郭淮", 1, 29, 87, 71, 78, 80, 82, 398, 320, 2, 10, 17, 0, 0, 1067, 0, 1, "士", null, "", "", "def", "SwordHitYellowCritical", "guohuai");
            config[102036] = new HeroConfig(102036, "夏侯尚", 1, 26, 79, 72, 75, 75, 72, 373, 315, 2, 10, 17, 0, 0, 689, 0, 1, "戟", null, "敏", "", "def", "SwordHitYellowCritical", "xiahoushang");
            config[102037] = new HeroConfig(102037, "钟繇", 1, 23, 70, 77, 37, 88, 80, 352, 290, 2, 10, 17, 18, 0, 476, 0, 3, "相", null, "米", "", "help", "SharpExplosionGreen", "zhongyao");
            config[102038] = new HeroConfig(102038, "满宠", 1, 28, 84, 82, 64, 85, 75, 390, 280, 2, 10, 17, 0, 0, 928, 0, 1, "戟", null, "连", "", "atk", "SwordHitYellowCritical", "manchong");
            config[102039] = new HeroConfig(102039, "曹冲", 1, 10, 31, 85, 21, 70, 85, 292, 260, 2, 10, 17, 15, 0, 166, 0, 3, "扇", null, "米", "", "inte", "FanExplosion", "caochong");
            config[102040] = new HeroConfig(102040, "蒋济", 1, 16, 48, 85, 43, 84, 75, 335, 310, 2, 10, 17, 35, 0, 354, 0, 3, "鼓", null, "米", "", "help", "SoulExplosionOrange", "jiangji");
            config[102041] = new HeroConfig(102041, "甄宓", 1, 18, 55, 74, 17, 70, 95, 311, 290, 2, 10, 17, 35, 0, 232, 0, 3, "鼓", null, "白", "", "help", "SoulExplosionOrange", "zhenshi");
            config[102042] = new HeroConfig(102042, "戏志才", 1, 22, 66, 88, 24, 75, 70, 323, 270, 2, 10, 17, 15, 0, 286, 0, 3, "谋", null, "陷", "", "inte", "StormExplosion", "xizhicai");
            config[103001] = new HeroConfig(103001, "孙坚", 1, 31, 93, 77, 90, 78, 92, 430, 290, 3, 10, 17, 0, 0, 1870, 0, 1, "枪", null, "旋", "", "atk", "SwordHitYellowCritical", "sunjian");
            config[103002] = new HeroConfig(103002, "孙策", 1, 32, 96, 74, 93, 75, 94, 432, 260, 3, 10, 17, 0, 0, 1937, 0, 1, "车", null, "虎", "", "atk", "SwordHitYellowCritical", "sunce");
            config[103003] = new HeroConfig(103003, "甘宁", 1, 31, 93, 76, 94, 70, 80, 413, 180, 3, 10, 17, 22, 1.5f, 1388, 0, 3, "弓", null, "连", "", "shoot", "BulletExplosionBlue", "ganning");
            config[103004] = new HeroConfig(103004, "太史慈", 1, 30, 90, 65, 93, 72, 88, 408, 230, 3, 10, 17, 22, 1.5f, 1272, 0, 3, "弓", null, "雨", "", "shoot", "BulletExplosionBlue", "taishici");
            config[103005] = new HeroConfig(103005, "黄盖", 1, 26, 80, 70, 83, 75, 84, 392, 320, 3, 10, 17, 0, 0, 961, 0, 1, "士", null, "奋", "", "def", "SwordHitYellowCritical", "huanggai");
            config[103006] = new HeroConfig(103006, "周泰", 1, 28, 84, 51, 91, 65, 82, 373, 350, 3, 10, 17, 0, 0, 689, 0, 1, "枪", null, "连", "", "atk", "SwordHitYellowCritical", "zhoutai");
            config[103007] = new HeroConfig(103007, "鲁肃", 1, 28, 85, 92, 61, 90, 94, 422, 250, 3, 10, 17, 18, 0, 1626, 0, 3, "相", null, "商", "雷", "help", "SharpExplosionGreen", "lusu");
            config[103008] = new HeroConfig(103008, "周瑜", 1, 32, 96, 96, 71, 88, 96, 447, 210, 3, 10, 17, 15, 0, 2519, 0, 3, "谋", null, "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu");
            config[103009] = new HeroConfig(103009, "蒋钦", 1, 25, 77, 57, 84, 68, 75, 361, 310, 3, 10, 17, 0, 0, 558, 0, 1, "戟", null, "", "", "atk", "SwordHitYellowCritical", "jiangqing");
            config[103010] = new HeroConfig(103010, "吕蒙", 1, 30, 92, 91, 80, 85, 86, 434, 250, 3, 10, 17, 0, 0, 2006, 0, 1, "马", null, "学", "羽", "def", "SwordHitYellowCritical", "lvmeng");
            config[103011] = new HeroConfig(103011, "陆逊", 1, 32, 96, 94, 69, 92, 89, 440, 220, 3, 10, 17, 15, 0, 2229, 0, 3, "谋", null, "炎", "", "inte", "GasExplosionFire", "luxun");
            config[103012] = new HeroConfig(103012, "张昭", 1, 21, 65, 89, 41, 96, 75, 366, 275, 3, 10, 17, 18, 0, 609, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "zhangzhao");
            config[103013] = new HeroConfig(103013, "诸葛瑾", 1, 25, 75, 83, 44, 88, 90, 380, 290, 3, 10, 17, 15, 0, 779, 0, 3, "扇", null, "励", "", "help", "FanExplosion", "zhugejin");
            config[103014] = new HeroConfig(103014, "孙尚香", 1, 25, 76, 70, 83, 70, 85, 384, 240, 3, 10, 17, 22, 1.5f, 835, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "sunshangxiang");
            config[103015] = new HeroConfig(103015, "朱桓", 1, 28, 84, 77, 82, 78, 75, 396, 320, 3, 10, 17, 0, 0, 1031, 0, 1, "枪", null, "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan");
            config[103016] = new HeroConfig(103016, "大乔", 1, 19, 57, 73, 14, 65, 92, 301, 280, 3, 10, 17, 15, 0, 195, 0, 3, "乐", null, "碉", "陷", "help", "StormExplosion", "daqiao");
            config[103017] = new HeroConfig(103017, "小乔", 1, 13, 40, 74, 19, 60, 90, 283, 270, 3, 10, 17, 15, 0, 142, 0, 3, "乐", null, "曲", "陷", "help", "StormExplosion", "xiaoqiao");
            config[103018] = new HeroConfig(103018, "丁奉", 1, 25, 76, 68, 82, 72, 78, 376, 205, 3, 10, 17, 13, 2.5f, 726, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "dingfeng");
            config[103019] = new HeroConfig(103019, "董袭", 1, 24, 74, 60, 80, 68, 75, 357, 325, 3, 10, 17, 0, 0, 520, 0, 1, "刀", null, "透", "", "atk", "SwordHitYellowCritical", "dongxi");
            config[103020] = new HeroConfig(103020, "凌统", 1, 25, 77, 62, 87, 70, 80, 376, 205, 3, 10, 17, 30, 1.5f, 726, 0, 3, "弩", null, "虐", "", "shoot", "BulletExplosionBlue", "lingtong");
            config[103021] = new HeroConfig(103021, "潘璋", 1, 25, 75, 74, 80, 72, 55, 356, 295, 3, 10, 17, 0, 0, 511, 0, 1, "戟", null, "刺", "虐", "def", "SwordHitYellowCritical", "panzhang");
            config[103022] = new HeroConfig(103022, "朱治", 1, 24, 73, 59, 72, 78, 80, 362, 235, 3, 10, 17, 22, 1.5f, 568, 0, 3, "弓", null, "敏", "", "shoot", "BulletExplosionBlue", "zhuzhi");
            config[103023] = new HeroConfig(103023, "徐盛", 1, 28, 86, 78, 81, 80, 82, 407, 290, 3, 10, 17, 0, 0, 1250, 0, 1, "士", null, "乱", "", "def", "SwordHitYellowCritical", "xusheng");
            config[103024] = new HeroConfig(103024, "程普", 1, 28, 84, 78, 80, 79, 84, 405, 300, 3, 10, 17, 0, 0, 1207, 0, 1, "戟", null, "实", "奋", "def", "SwordHitYellowCritical", "chengpu");
            config[103025] = new HeroConfig(103025, "张纮", 1, 22, 68, 85, 50, 86, 82, 371, 250, 3, 10, 17, 18, 0, 665, 0, 3, "相", null, "励", "", "help", "SharpExplosionGreen", "zhanghong");
            config[103026] = new HeroConfig(103026, "顾雍", 1, 21, 65, 81, 60, 90, 85, 381, 300, 3, 10, 17, 15, 0, 792, 0, 3, "扇", null, "连", "", "inte", "FanExplosion", "guyong");
            config[103027] = new HeroConfig(103027, "步骘", 1, 23, 70, 80, 65, 85, 78, 378, 315, 3, 10, 17, 35, 0, 752, 0, 3, "鼓", null, "励", "", "help", "SoulExplosionOrange", "buzhi");
            config[103028] = new HeroConfig(103028, "阚泽", 1, 20, 62, 84, 55, 82, 80, 363, 260, 3, 10, 17, 15, 0, 578, 0, 3, "谋", null, "炽", "", "inte", "StormExplosion", "kanze");
            config[103029] = new HeroConfig(103029, "韩当", 1, 25, 76, 62, 85, 70, 78, 371, 300, 3, 10, 17, 0, 0, 665, 0, 1, "戟", null, "奋", "", "atk", "SwordHitYellowCritical", "handang");
            config[103030] = new HeroConfig(103030, "陆抗", 1, 30, 91, 87, 63, 88, 86, 415, 195, 3, 10, 17, 30, 1.5f, 1438, 0, 3, "弩", null, "透", "", "shoot", "BulletExplosionBlue", "lukang");
            config[103031] = new HeroConfig(103031, "诸葛恪", 1, 24, 72, 90, 47, 80, 60, 349, 235, 3, 10, 17, 15, 0, 452, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhugege");
            config[103032] = new HeroConfig(103032, "苏飞", 1, 23, 69, 66, 63, 72, 70, 340, 330, 3, 10, 17, 0, 0, 386, 0, 1, "刀", null, "复", "", "def", "SwordHitYellowCritical", "sufei");
            config[103033] = new HeroConfig(103033, "全琮", 1, 26, 78, 75, 72, 78, 75, 378, 310, 3, 10, 17, 0, 0, 752, 0, 1, "戟", null, "实", "", "def", "SwordHitYellowCritical", "quanzong");
            config[103034] = new HeroConfig(103034, "陈武", 1, 25, 76, 47, 87, 65, 75, 350, 305, 3, 10, 17, 0, 0, 460, 0, 1, "枪", null, "劫", "", "def", "SwordHitYellowCritical", "chengwu");
            config[103035] = new HeroConfig(103035, "朱然", 1, 26, 79, 71, 69, 80, 78, 377, 315, 3, 10, 17, 0, 0, 739, 0, 1, "枪", null, "竟", "", "def", "SwordHitYellowCritical", "zhuran");
            config[103036] = new HeroConfig(103036, "孙韶", 1, 26, 80, 76, 79, 75, 78, 388, 310, 3, 10, 17, 0, 0, 896, 0, 1, "戟", null, "破", "", "def", "SwordHitYellowCritical", "sunshao");
            config[103037] = new HeroConfig(103037, "孙桓", 1, 27, 82, 76, 73, 75, 76, 382, 315, 3, 10, 17, 0, 0, 806, 0, 1, "戟", null, "竟", "", "def", "SwordHitYellowCritical", "sunhuan");
            config[103038] = new HeroConfig(103038, "严畯", 1, 20, 62, 82, 48, 84, 75, 351, 290, 3, 10, 17, 15, 0, 468, 0, 3, "扇", null, "虐", "", "inte", "FanExplosion", "yanjun");
            config[104001] = new HeroConfig(104001, "吕布", 1, 32, 97, 43, 100, 40, 50, 330, 310, 4, 10, 17, 0, 0, 324, 0, 1, "车", null, "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu");
            config[104002] = new HeroConfig(104002, "华雄", 1, 29, 88, 60, 90, 55, 65, 358, 310, 4, 10, 17, 0, 0, 529, 0, 1, "车", null, "纷", "", "atk", "SwordHitYellowCritical", "huaxiong");
            config[104003] = new HeroConfig(104003, "贾诩", 1, 28, 86, 97, 50, 85, 75, 393, 260, 4, 10, 17, 15, 0, 978, 0, 3, "谋", null, "延", "", "inte", "StormExplosion", "jiaxu");
            config[104004] = new HeroConfig(104004, "貂蝉", 1, 9, 27, 81, 65, 70, 99, 342, 280, 4, 10, 17, 15, 0, 400, 0, 3, "乐", null, "曲", "", "help", "StormExplosion", "diaochan");
            config[104005] = new HeroConfig(104005, "臧霸", 1, 26, 78, 53, 75, 70, 75, 351, 330, 4, 10, 17, 0, 0, 468, 0, 1, "马", null, "虐", "", "atk", "SwordHitYellowCritical", "zangba");
            config[104006] = new HeroConfig(104006, "高顺", 1, 28, 85, 63, 86, 75, 80, 389, 215, 4, 10, 17, 13, 2.5f, 912, 0, 3, "炮", null, "", "", "shoot", "GasShootFire", "gaoshun");
            config[104007] = new HeroConfig(104007, "李儒", 1, 21, 63, 91, 43, 80, 40, 317, 250, 4, 10, 17, 15, 0, 258, 0, 3, "谋", null, "火", "", "inte", "ShadowExplosion", "liru");
            config[104008] = new HeroConfig(104008, "李傕", 1, 23, 69, 29, 74, 45, 30, 247, 370, 4, 10, 17, 0, 0, 75, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "lijue");
            config[104009] = new HeroConfig(104009, "郭汜", 1, 21, 64, 18, 76, 40, 25, 223, 390, 4, 10, 17, 0, 0, 49, 0, 1, "刀", null, "劫", "", "atk", "SwordHitYellowCritical", "guosi");
            config[104010] = new HeroConfig(104010, "陈宫", 1, 28, 84, 89, 55, 82, 85, 395, 260, 4, 10, 17, 15, 0, 1013, 0, 3, "谋", null, "励", "溃", "inte", "ShadowExplosion", "chengong");
            config[105001] = new HeroConfig(105001, "邓艾", 1, 31, 94, 89, 87, 88, 75, 433, 295, 5, 10, 17, 0, 0, 1971, 0, 1, "枪", null, "奇", "", "def", "SwordHitYellowCritical", "dengai");
            config[105002] = new HeroConfig(105002, "司马师", 1, 26, 80, 87, 67, 82, 70, 386, 280, 5, 10, 17, 18, 0, 865, 0, 3, "相", null, "", "", "help", "SharpExplosionGreen", "simashi");
            config[105003] = new HeroConfig(105003, "司马昭", 1, 26, 78, 87, 57, 84, 65, 371, 290, 5, 10, 17, 18, 0, 665, 0, 3, "相", null, "溃", "", "help", "SharpExplosionGreen", "simazhao");
            config[105004] = new HeroConfig(105004, "羊祜", 1, 30, 90, 84, 64, 90, 92, 420, 320, 5, 10, 17, 0, 0, 1570, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "yangku");
            config[105005] = new HeroConfig(105005, "钟会", 1, 27, 82, 92, 58, 85, 65, 382, 290, 5, 10, 17, 15, 0, 806, 0, 3, "谋", null, "缓", "", "inte", "StormExplosion", "zhonghui");
            config[105006] = new HeroConfig(105006, "陈泰", 1, 28, 86, 84, 77, 86, 80, 413, 280, 5, 10, 17, 0, 0, 1388, 0, 1, "士", null, "虐", "", "def", "SwordHitYellowCritical", "chentai");
            config[105007] = new HeroConfig(105007, "杜预", 1, 28, 84, 85, 30, 89, 78, 366, 320, 5, 10, 17, 18, 0, 609, 0, 3, "相", null, "米", "", "inte", "SharpExplosionGreen", "duyu");
            config[105008] = new HeroConfig(105008, "王濬", 1, 26, 80, 79, 52, 82, 75, 368, 340, 5, 10, 17, 0, 0, 631, 0, 1, "戟", null, "敏", "", "atk", "SwordHitYellowCritical", "wangrui");
            config[105009] = new HeroConfig(105009, "辛宪英", 1, 14, 42, 84, 28, 80, 82, 316, 310, 5, 10, 17, 15, 0, 253, 0, 3, "扇", null, "缓", "", "inte", "FanExplosion", "xinxianying");
            config[106001] = new HeroConfig(106001, "颜良", 1, 29, 88, 41, 93, 50, 70, 342, 340, 6, 10, 17, 0, 0, 400, 0, 1, "车", null, "破", "", "atk", "SwordHitYellowCritical", "yanliang");
            config[106002] = new HeroConfig(106002, "文丑", 1, 29, 89, 48, 92, 52, 68, 349, 355, 6, 10, 17, 0, 0, 452, 0, 1, "车", null, "刺", "", "def", "SwordHitYellowCritical", "wenchou");
            config[106003] = new HeroConfig(106003, "田丰", 1, 24, 72, 93, 33, 88, 75, 361, 250, 6, 10, 17, 15, 0, 558, 0, 3, "谋", null, "雷", "", "inte", "StormExplosion", "tianfeng");
            config[106004] = new HeroConfig(106004, "鞠义", 1, 24, 72, 55, 78, 68, 60, 333, 250, 6, 10, 17, 22, 1.5f, 341, 0, 3, "弓", null, "", "", "shoot", "BulletExplosionBlue", "juyi");
            config[106005] = new HeroConfig(106005, "许攸", 1, 13, 39, 80, 29, 75, 30, 253, 285, 6, 10, 17, 15, 0, 84, 0, 3, "谋", null, "火", "", "inte", "StormExplosion", "xuyou");
            config[106006] = new HeroConfig(106006, "高览", 1, 25, 76, 68, 82, 70, 72, 368, 305, 6, 10, 17, 0, 0, 631, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "gaolan");
            config[106007] = new HeroConfig(106007, "沮授", 1, 26, 78, 90, 35, 89, 82, 374, 260, 6, 10, 17, 15, 0, 701, 0, 3, "谋", null, "静", "", "inte", "StormExplosion", "jushou");
            config[106008] = new HeroConfig(106008, "郭图", 1, 17, 52, 83, 50, 78, 45, 308, 290, 6, 10, 17, 15, 0, 220, 0, 3, "扇", null, "励", "米", "help", "FanExplosion", "guotu");
            config[110002] = new HeroConfig(110002, "张任", 1, 29, 88, 75, 84, 78, 82, 407, 210, 10, 10, 17, 22, 1.5f, 1250, 0, 3, "弓", null, "复", "", "shoot", "BulletExplosionBlue", "zhangren");
            config[110003] = new HeroConfig(110003, "华佗", 1, 20, 60, 77, 34, 70, 85, 326, 330, 10, 10, 17, 14, 0, 302, 0, 3, "医", null, "药", "", "help", "ShadowExplosionGreen", "huatuo");
            config[110006] = new HeroConfig(110006, "于吉", 1, 15, 47, 73, 41, 65, 70, 296, 310, 10, 10, 17, 14, 0, 178, 0, 3, "医", null, "调", "", "help", "ShadowExplosionGreen", "yuji");
            config[110007] = new HeroConfig(110007, "张角", 1, 29, 87, 86, 29, 82, 88, 372, 280, 10, 10, 17, 15, 0, 677, 0, 3, "谋", null, "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao");
            config[110008] = new HeroConfig(110008, "张宝", 1, 27, 83, 81, 71, 78, 75, 388, 280, 10, 10, 17, 0, 0, 896, 0, 1, "枪", null, "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2");
            config[110009] = new HeroConfig(110009, "张梁", 1, 26, 78, 74, 80, 75, 70, 377, 225, 10, 10, 17, 13, 2.5f, 739, 0, 3, "炮", null, "", "", "def", "SwordHitYellowCritical", "zhangliang");
            config[110010] = new HeroConfig(110010, "韩遂", 1, 27, 81, 82, 75, 78, 70, 386, 290, 10, 10, 17, 0, 0, 865, 0, 1, "马", null, "乱", "", "def", "SwordHitYellowCritical", "hansui");
            config[110011] = new HeroConfig(110011, "王异", 1, 24, 73, 82, 51, 75, 78, 359, 290, 10, 10, 17, 0, 0, 539, 0, 1, "枪", null, "", "", "atk", "SwordHitYellowCritical", "wangyi");
            config[110012] = new HeroConfig(110012, "蔡琰", 1, 20, 61, 77, 13, 75, 82, 308, 270, 10, 10, 17, 15, 0, 220, 0, 3, "乐", null, "碉", "", "help", "StormExplosion", "caiyan");

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
