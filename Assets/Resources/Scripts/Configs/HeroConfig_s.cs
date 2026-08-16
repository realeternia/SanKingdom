﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroConfig
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
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Name", new FieldMetaInfo("名字", "string", 76)},
            {"BornYear", new FieldMetaInfo("出生", "int", 60)},
            {"DeadYear", new FieldMetaInfo("死亡", "int", 60)},
            {"LeadShip", new FieldMetaInfo("统帅", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Str", new FieldMetaInfo("武力", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Inte", new FieldMetaInfo("智力", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Fair", new FieldMetaInfo("内政", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"Charm", new FieldMetaInfo("魅力", "int", 60, "95-100:#FF9900,90-94:#995500,80-89:#33CC33")},
            {"SodSword", new FieldMetaInfo("步兵驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodHorse", new FieldMetaInfo("骑兵驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodBow", new FieldMetaInfo("弓兵驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodWater", new FieldMetaInfo("水军驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodTank", new FieldMetaInfo("车炮驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodSpear", new FieldMetaInfo("枪兵驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodHalberd", new FieldMetaInfo("戟兵驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"SodElephant", new FieldMetaInfo("象兵驾驭", "int", 60, "10:#FF9900,9:#995500,7-8:#33CC33,5-6:#3333CC")},
            {"Total", new FieldMetaInfo("总属性", "int", 60)},
            {"StarHero", new FieldMetaInfo("名将", "bool", 0)},
            {"ForceId", new FieldMetaInfo("阵营", "int", 60)},
            {"City", new FieldMetaInfo("所在", "string", 0)},
            {"BornCity", new FieldMetaInfo("出生", "string", 0)},
            {"Loyal", new FieldMetaInfo("初始中心度", "int", 60)},
            {"Xingge", new FieldMetaInfo("性格", "string", 0)},
            {"Pinzhi", new FieldMetaInfo("品质", "string[]", 0)},
            {"Aihao", new FieldMetaInfo("爱好", "string[]", 0)},
            {"Paixi", new FieldMetaInfo("派系", "string", 0)},
            {"LikeForces", new FieldMetaInfo("喜爱势力", "string[]", 191)},
            {"HateForces", new FieldMetaInfo("厌恶势力", "string[]", 282)},
            {"Story", new FieldMetaInfo("Story", "string[]", 0)},
            {"Pos", new FieldMetaInfo("站位", "int", 60)},
            {"Skills", new FieldMetaInfo("技能", "int[]", 0)},
            {"Skill1", new FieldMetaInfo("技能", "string", 0)},
            {"Skill2", new FieldMetaInfo("技能2", "string", 0)},
            {"Group", new FieldMetaInfo("团队", "string", 0)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 0)},
            {"Icon", new FieldMetaInfo("背景图", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

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
        public int SodSword;
        public int SodHorse;
        public int SodBow;
        public int SodWater;
        public int SodTank;
        public int SodSpear;
        public int SodHalberd;
        public int SodElephant;
        /// <summary>
        ///总属性
        /// </summary>
        public int Total;
        /// <summary>
        ///名将
        /// </summary>
        public bool StarHero;
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
        ///初始中心度
        /// </summary>
        public int Loyal;
        /// <summary>
        ///性格
        /// </summary>
        public string Xingge;
        /// <summary>
        ///品质
        /// </summary>
        public string[] Pinzhi;
        /// <summary>
        ///爱好
        /// </summary>
        public string[] Aihao;
        /// <summary>
        ///派系
        /// </summary>
        public string Paixi;
        /// <summary>
        ///喜爱势力
        /// </summary>
        public string[] LikeForces;
        /// <summary>
        ///厌恶势力
        /// </summary>
        public string[] HateForces;
        /// <summary>
        ///Story
        /// </summary>
        public string[] Story;
        /// <summary>
        ///站位
        /// </summary>
        public int Pos;
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


        public HeroConfig(int Id, string Name, int BornYear, int DeadYear, int LeadShip, int Str, int Inte, int Fair, int Charm, int SodSword, int SodHorse, int SodBow, int SodWater, int SodTank, int SodSpear, int SodHalberd, int SodElephant, int Total, bool StarHero, int ForceId, string City, string BornCity, int Loyal, string Xingge, string[] Pinzhi, string[] Aihao, string Paixi, string[] LikeForces, string[] HateForces, string[] Story, int Pos, int[] Skills, string Skill1, string Skill2, string Group, string HitEffect, string Icon)
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
            this.SodSword = SodSword;
            this.SodHorse = SodHorse;
            this.SodBow = SodBow;
            this.SodWater = SodWater;
            this.SodTank = SodTank;
            this.SodSpear = SodSpear;
            this.SodHalberd = SodHalberd;
            this.SodElephant = SodElephant;
            this.Total = Total;
            this.StarHero = StarHero;
            this.ForceId = ForceId;
            this.City = City;
            this.BornCity = BornCity;
            this.Loyal = Loyal;
            this.Xingge = Xingge;
            this.Pinzhi = Pinzhi;
            this.Aihao = Aihao;
            this.Paixi = Paixi;
            this.LikeForces = LikeForces;
            this.HateForces = HateForces;
            this.Story = Story;
            this.Pos = Pos;
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
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[100001] = new HeroConfig(100001, "刘备", 161, 223, 80, 77, 74, 78, 99, 8, 7, 6, 5, 6, 8, 8, 1, 440, true, 1, "新野", "蓟", 100, "豪爽", new string[]{"忠义","仁德"}, new string[]{"饮酒","读书"}, "无", new string[]{"1;5","11;3"}, new string[]{"2;3","5;3"}, new string[]{"三顾茅庐","桃园结义","携民渡江"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "liubei");
            config[100002] = new HeroConfig(100002, "曹操", 155, 220, 98, 81, 91, 94, 96, 8, 8, 7, 6, 8, 8, 8, 1, 497, true, 2, "许昌", "陈留", 100, "豪爽", new string[]{"雄才","狡诈"}, new string[]{"兵法","诗歌","美色"}, "无", new string[]{"1;3"}, new string[]{"1;3","5;3","6;3"}, new string[]{"官渡之战","煮酒论英雄","割须弃袍"}, 1, new int[0], "识", "", "core", "SwordHitYellowCritical", "caocao");
            config[100003] = new HeroConfig(100003, "孙策", 175, 200, 96, 93, 74, 75, 94, 8, 10, 7, 8, 7, 9, 8, 1, 471, true, 3, "吴", "吴", 100, "豪爽", new string[]{"果敢","雄才"}, new string[]{"武艺","结交"}, "无", new string[]{"3;5"}, new string[]{"7;3","10;3"}, new string[]{"小霸王"}, 1, new int[0], "虎", "", "core", "SwordHitYellowCritical", "sunce");
            config[100004] = new HeroConfig(100004, "袁绍", 154, 202, 86, 73, 70, 73, 90, 9, 7, 9, 5, 6, 9, 9, 1, 412, true, 4, "邺", "汝南", 100, "刚愎", new string[]{"雄才"}, new string[]{"华服","珍宝"}, "无", new string[]{"4;5"}, new string[]{"2;3","4;5"}, new string[]{"官渡之战"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "yuanshao");
            config[100005] = new HeroConfig(100005, "董卓", 139, 192, 77, 87, 67, 18, 36, 8, 7, 5, 4, 5, 8, 8, 1, 305, false, 5, "洛阳", "安定", 100, "暴戾", new string[]{"狡诈"}, new string[]{"美色","珍宝"}, "无", new string[]{"5;5"}, new string[]{"5;5"}, new string[]{"火烧洛阳"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "dongzhuo");
            config[100006] = new HeroConfig(100006, "马腾", 155, 212, 82, 80, 51, 58, 88, 8, 10, 5, 4, 6, 8, 8, 1, 382, false, 6, "武威", "武威", 100, "豪爽", new string[]{"忠义"}, new string[]{"骑马"}, "无", new string[]{"6;5"}, new string[]{"2;3"}, null, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "mateng");
            config[100007] = new HeroConfig(100007, "刘表", 142, 208, 46, 31, 68, 81, 80, 4, 3, 2, 5, 4, 4, 4, 1, 323, false, 7, "襄阳", "陈留", 100, "谦和", new string[]{"仁德","贤明"}, new string[]{"读书","清谈"}, "无", null, null, new string[]{"单骑定荆州"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "liubiao");
            config[100008] = new HeroConfig(100008, "刘璋", 165, 221, 15, 4, 8, 37, 64, 2, 1, 1, 2, 2, 2, 2, 1, 135, false, 8, "成都", "江夏", 100, "懦弱", new string[]{"仁德"}, new string[]{"清谈"}, "无", null, null, new string[]{"引刘备入川"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "liuzhang");
            config[100009] = new HeroConfig(100009, "张鲁", 155, 216, 51, 26, 74, 78, 75, 4, 2, 2, 2, 5, 4, 4, 1, 315, false, 9, "汉中", "濮阳", 100, "隐忍", new string[]{"仁德"}, new string[]{"炼丹","道教"}, "无", null, null, new string[]{"五斗米道"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "zhanglu");
            config[100010] = new HeroConfig(100010, "袁术", 155, 199, 67, 65, 65, 60, 45, 6, 5, 4, 3, 4, 6, 6, 1, 319, false, 10, "寿春", "汝南", 100, "骄横", new string[]{"雄才"}, new string[]{"珍宝","华服"}, "无", null, null, new string[]{"称帝"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "yuanshu");
            config[100011] = new HeroConfig(100011, "公孙瓒", 160, 199, 83, 81, 75, 46, 77, 8, 7, 5, 4, 6, 8, 8, 1, 386, false, 11, "北平", "汉中", 100, "刚愎", new string[]{"果敢"}, new string[]{"白马","射猎"}, "无", null, new string[]{"4;3"}, new string[]{"白马义从"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "gongsunzan");
            config[100012] = new HeroConfig(100012, "公孙度", 150, 204, 67, 71, 66, 51, 55, 5, 4, 3, 3, 4, 5, 5, 1, 327, false, 12, "襄平", "襄阳", 100, "刚愎", new string[]{"雄才"}, new string[]{"珍宝"}, "无", null, null, new string[]{"自立辽东"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "gongsundu");
            config[100020] = new HeroConfig(100020, "司马炎", 236, 290, 69, 59, 77, 85, 75, 6, 5, 3, 3, 5, 6, 6, 1, 385, false, 99, "", "吴", 0, "豪爽", new string[]{"雄才"}, new string[]{"美色","珍宝"}, "无", null, null, new string[]{"代魏称帝"}, 1, new int[0], "", "", "core", "SwordHitYellowCritical", "simayan");
            config[101001] = new HeroConfig(101001, "张飞", 165, 221, 92, 98, 30, 22, 45, 10, 9, 8, 7, 7, 10, 8, 1, 327, true, 1, "新野", "北平", 100, "急躁", new string[]{"忠义","果敢"}, new string[]{"饮酒","结交"}, "无", new string[]{"1;5"}, null, new string[]{"当阳桥喝退曹军","义释严颜"}, 1, new int[0], "威", "", "atk", "SwordHitYellowCritical", "zhangfei");
            config[101002] = new HeroConfig(101002, "关羽", 160, 220, 97, 97, 77, 62, 94, 9, 9, 10, 7, 8, 9, 9, 1, 468, true, 1, "新野", "洛阳", 100, "刚愎", new string[]{"忠义","坚韧"}, new string[]{"读书","骑马"}, "无", new string[]{"1;5"}, null, new string[]{"过五关斩六将","温酒斩华雄","水淹七军"}, 1, new int[0], "", "", "atk", "SwordHitGreenCritical", "guanyu");
            config[101003] = new HeroConfig(101003, "徐庶", 189, 234, 87, 65, 93, 82, 84, 7, 6, 6, 6, 8, 7, 7, 1, 444, true, 1, "新野", "许昌", 92, "冷静", new string[]{"忠义","思辨"}, new string[]{"读书","剑术"}, "无", null, new string[]{"2;2"}, new string[]{"走马荐诸葛"}, 3, new int[0], "火", "共", "inte", "GasExplosionFire", "xusu");
            config[101004] = new HeroConfig(101004, "周仓", 178, 220, 63, 82, 42, 33, 60, 8, 6, 5, 5, 5, 8, 8, 1, 309, false, 1, "新野", "洛阳", 100, "豪爽", new string[]{"忠义","果敢"}, new string[]{"舞剑","结交"}, "无", new string[]{"1;2"}, null, new string[]{"为关羽扛刀","水淹七军擒庞德"}, 1, new int[0], "劫", "", "atk", "SwordHitYellowCritical", "zhoucang");
            config[101005] = new HeroConfig(101005, "廖化", 184, 264, 74, 78, 64, 49, 66, 8, 6, 6, 5, 6, 8, 8, 1, 362, false, 1, "新野", "襄阳", 91, "坚韧", new string[]{"坚韧","忠义"}, new string[]{"骑马"}, "无", null, null, new string[]{"千里走单骑归蜀"}, 1, new int[0], "透", "", "atk", "SwordHitYellowCritical", "liaohua");
            config[101006] = new HeroConfig(101006, "简雍", 164, 221, 72, 65, 70, 75, 70, 6, 5, 5, 7, 6, 6, 6, 1, 381, false, 1, "新野", "北平", 94, "豪爽", new string[]{"能言"}, new string[]{"饮酒","清谈"}, "无", null, null, new string[]{"说和刘璋"}, 3, new int[0], "破", "", "shoot", "BulletExplosionBlue", "jianyong");
            config[101007] = new HeroConfig(101007, "孙乾", 163, 215, 62, 54, 80, 84, 82, 5, 4, 4, 4, 6, 5, 5, 1, 385, false, 1, "新野", "北海", 92, "谦和", new string[]{"忠义","勤勉"}, new string[]{"读书"}, "无", new string[]{"1;3"}, null, null, 3, new int[0], "白", "", "help", "SoulExplosionOrange", "sunqian");
            config[101008] = new HeroConfig(101008, "赵云", 168, 229, 91, 96, 76, 65, 81, 9, 9, 7, 5, 7, 8, 8, 1, 436, true, 1, "新野", "晋阳", 90, "冷静", new string[]{"忠义","果敢"}, new string[]{"武艺","骑射"}, "无", new string[]{"1;5"}, null, new string[]{"长坂坡救阿斗"}, 1, new int[0], "镜", "羽", "def", "SwordHitWhiteCritical", "zhaoyun");
            config[101009] = new HeroConfig(101009, "关平", 178, 220, 79, 82, 72, 71, 78, 9, 7, 6, 5, 6, 8, 9, 1, 415, false, 1, "新野", "洛阳", 95, "冷静", new string[]{"坚韧","忠义"}, new string[]{"舞剑","骑马"}, "无", new string[]{"1;5"}, null, null, 1, new int[0], "连", "", "def", "SwordHitYellowCritical", "guanping");
            config[101010] = new HeroConfig(101010, "胡班", 170, 220, 54, 54, 57, 49, 61, 6, 5, 5, 4, 5, 6, 6, 1, 300, false, 1, "新野", "陈留", 90, "刚猛", new string[]{"果敢"}, new string[]{"骑马","兵器"}, "无", null, null, new string[]{"荥阳放关羽"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "huban");
            config[101011] = new HeroConfig(101011, "糜竺", 157, 221, 33, 29, 77, 85, 85, 3, 2, 2, 2, 5, 3, 3, 1, 323, false, 1, "新野", "下邳", 93, "谦和", new string[]{"忠义","贤明"}, new string[]{"算术","书法"}, "无", new string[]{"1;3"}, null, new string[]{"散尽家财助刘备"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "mizhu");
            config[101012] = new HeroConfig(101012, "糜芳", 160, 223, 54, 61, 32, 23, 23, 5, 5, 4, 4, 3, 5, 5, 1, 214, false, 1, "新野", "下邳", 88, "多疑", new string[]{"怯懦"}, new string[]{"珍宝","华服"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "mifang");
            config[101013] = new HeroConfig(101013, "陈到", 178, 230, 76, 71, 63, 53, 69, 8, 7, 6, 5, 6, 8, 8, 1, 364, false, 1, "新野", "汝南", 96, "隐忍", new string[]{"忠义","专注"}, new string[]{"练兵"}, "无", new string[]{"1;3"}, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chendao");
            config[102001] = new HeroConfig(102001, "郭嘉", 170, 207, 72, 43, 98, 78, 82, 6, 5, 5, 5, 8, 6, 6, 1, 402, true, 2, "宛", "许昌", 100, "豪爽", new string[]{"雄才","洞察"}, new string[]{"饮酒","清谈"}, "无", new string[]{"2;5"}, null, new string[]{"遗计定辽东","十胜十败论"}, 3, new int[0], "天", "", "inte", "LightningExplosionBlue", "guojia");
            config[102002] = new HeroConfig(102002, "夏侯惇", 155, 220, 91, 91, 57, 68, 80, 9, 9, 7, 7, 7, 9, 9, 1, 426, true, 2, "许昌", "陈留", 100, "刚猛", new string[]{"果敢","忠义"}, new string[]{"舞剑","骑马"}, "无", new string[]{"2;5"}, null, new string[]{"拔矢啖睛"}, 1, new int[0], "青", "", "atk", "SwordHitYellowCritical", "xiahoudun");
            config[102003] = new HeroConfig(102003, "荀彧", 163, 212, 67, 47, 95, 100, 93, 5, 4, 4, 4, 8, 5, 5, 1, 427, true, 2, "许昌", "许昌", 95, "谦和", new string[]{"仁德","贤明"}, new string[]{"熏香","书法"}, "无", new string[]{"2;2"}, null, new string[]{"劝曹操迎天子"}, 3, new int[0], "", "", "help", "FrostExplosionBlue", "xunyu");
            config[102004] = new HeroConfig(102004, "许褚", 169, 227, 65, 96, 36, 20, 59, 9, 7, 7, 7, 7, 9, 9, 1, 313, true, 2, "许昌", "陈留", 99, "刚猛", new string[]{"忠义","果敢"}, new string[]{"舞剑"}, "无", null, null, new string[]{"裸衣斗马超"}, 1, new int[0], "斧", "", "atk", "SwordHitYellowCritical", "xuchu");
            config[102005] = new HeroConfig(102005, "夏侯渊", 155, 219, 90, 89, 54, 40, 80, 8, 9, 9, 7, 7, 8, 8, 1, 393, true, 2, "小沛", "陈留", 100, "急躁", new string[]{"果敢","专注"}, new string[]{"骑马","射猎"}, "无", new string[]{"2;5"}, null, new string[]{"定军山被斩（负面","不写）"}, 3, new int[0], "雨", "", "shoot", "BulletExplosionBlue", "xiahouyuan");
            config[102006] = new HeroConfig(102006, "典韦", 155, 197, 59, 95, 43, 38, 78, 10, 7, 7, 7, 7, 8, 10, 1, 351, true, 2, "许昌", "陈留", 100, "刚猛", new string[]{"忠义","果敢"}, new string[]{"舞戟","饮酒"}, "无", null, null, new string[]{"宛城护主战死"}, 1, new int[0], "护", "", "def", "SwordHitYellowCritical", "dianwei");
            config[102007] = new HeroConfig(102007, "徐晃", 155, 227, 90, 91, 74, 68, 71, 9, 8, 8, 7, 7, 9, 9, 1, 433, true, 2, "下邳", "洛阳", 95, "冷静", new string[]{"坚韧","清廉"}, new string[]{"兵法","读书"}, "无", null, null, new string[]{"解樊城之围"}, 3, new int[0], "连", "", "shoot", "BulletExplosionBlue", "xuhuang");
            config[102008] = new HeroConfig(102008, "荀攸", 157, 214, 63, 53, 93, 91, 73, 5, 4, 5, 5, 7, 5, 5, 1, 399, true, 2, "许昌", "许昌", 97, "隐忍", new string[]{"智识","思辨"}, new string[]{"围棋"}, "无", new string[]{"2;5"}, null, new string[]{"官渡献策"}, 3, new int[0], "百", "", "inte", "FrostExplosionBlue", "xunyou");
            config[102009] = new HeroConfig(102009, "于禁", 158, 219, 80, 75, 68, 55, 51, 8, 7, 6, 6, 6, 8, 8, 1, 362, false, 2, "下邳", "北海", 91, "刚愎", new string[]{"专注","坚韧"}, new string[]{"练兵"}, "无", null, new string[]{"6;3"}, null, 1, new int[0], "青", "破", "def", "SwordHitYellowCritical", "yujin");
            config[102010] = new HeroConfig(102010, "曹仁", 168, 223, 90, 86, 62, 46, 76, 8, 8, 7, 7, 7, 9, 8, 1, 397, true, 2, "宛", "陈留", 100, "隐忍", new string[]{"坚韧","果敢"}, new string[]{"骑马"}, "无", new string[]{"2;5"}, null, new string[]{"坚守樊城"}, 1, new int[0], "青", "", "atk", "SwordHitYellowCritical", "caoren");
            config[102011] = new HeroConfig(102011, "曹洪", 169, 232, 82, 83, 44, 35, 54, 7, 8, 6, 6, 5, 7, 7, 1, 330, false, 2, "小沛", "陈留", 100, "豪爽", new string[]{"果敢"}, new string[]{"珍宝","骑马"}, "无", null, null, new string[]{"救曹操于荥阳"}, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "caohong");
            config[102012] = new HeroConfig(102012, "乐进", 158, 218, 80, 84, 50, 49, 63, 8, 7, 6, 6, 5, 8, 9, 1, 358, false, 2, "北海", "濮阳", 94, "刚猛", new string[]{"果敢","坚韧"}, new string[]{"攻城"}, "无", null, null, null, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "lejin");
            config[102013] = new HeroConfig(102013, "文聘", 170, 226, 80, 82, 65, 75, 78, 7, 7, 7, 6, 6, 7, 7, 1, 413, false, 2, "北海", "宛", 93, "冷静", new string[]{"忠义","坚韧"}, new string[]{"射箭"}, "无", null, null, null, 1, new int[0], "透", "劫", "atk", "SwordHitYellowCritical", "wenpin");
            config[102014] = new HeroConfig(102014, "曹休", 170, 228, 73, 73, 58, 56, 67, 6, 7, 7, 5, 5, 6, 6, 1, 357, false, 2, "宛", "陈留", 94, "急躁", new string[]{"果敢"}, new string[]{"骑马","射猎"}, "无", null, null, null, 3, new int[0], "", "", "shoot", "BulletExplosionBlue", "caoxiu");
            config[102015] = new HeroConfig(102015, "郝昭", 176, 228, 87, 79, 74, 59, 69, 8, 6, 6, 6, 7, 8, 8, 1, 401, false, 2, "北海", "晋阳", 92, "冷静", new string[]{"坚韧","专注"}, new string[]{"守城"}, "无", null, null, new string[]{"陈仓拒诸葛亮"}, 1, new int[0], "坚", "", "def", "SwordHitYellowCritical", "haozhao");
            config[102016] = new HeroConfig(102016, "程昱", 141, 220, 63, 54, 87, 79, 56, 5, 4, 5, 5, 7, 5, 5, 1, 365, false, 2, "濮阳", "濮阳", 96, "多疑", new string[]{"雄才","果敢"}, new string[]{"兵法"}, "无", new string[]{"2;5"}, null, null, 3, new int[0], "识", "火", "inte", "StormExplosion", "chengyu");
            config[102017] = new HeroConfig(102017, "杨修", 175, 219, 10, 4, 83, 80, 43, 3, 2, 2, 2, 5, 3, 3, 1, 234, false, 2, "小沛", "长安", 86, "急躁", new string[]{"才思"}, new string[]{"美食","读书"}, "无", null, null, new string[]{"鸡肋事件"}, 3, new int[0], "虐", "", "help", "SharpExplosionGreen", "yangxiu");
            config[102018] = new HeroConfig(102018, "牛金", 175, 220, 71, 77, 38, 40, 45, 7, 7, 6, 6, 5, 7, 7, 1, 302, false, 2, "宛", "宛", 91, "刚猛", new string[]{"果敢"}, new string[]{"舞剑","饮酒"}, "无", null, null, null, 1, new int[0], "伏", "", "atk", "SwordHitYellowCritical", "niujin");
            config[102019] = new HeroConfig(102019, "陈群", 165, 237, 65, 45, 74, 98, 73, 4, 3, 3, 3, 6, 4, 4, 1, 374, true, 2, "许昌", "许昌", 93, "冷静", new string[]{"贤明","思辨"}, new string[]{"律法","书法"}, "无", new string[]{"2;5"}, null, new string[]{"创立九品中正制"}, 3, new int[0], "", "", "help", "FanExplosion", "chenqun");
            config[102020] = new HeroConfig(102020, "李典", 174, 215, 74, 73, 79, 74, 65, 7, 6, 6, 5, 6, 7, 7, 1, 395, false, 2, "宛", "陈留", 95, "谦和", new string[]{"仁德","思辨"}, new string[]{"读书"}, "无", new string[]{"2;5","5;3"}, null, null, 1, new int[0], "伏", "坚", "def", "SwordHitYellowCritical", "lidian");
            config[102021] = new HeroConfig(102021, "曹丕", 187, 226, 78, 79, 78, 84, 77, 7, 6, 6, 5, 6, 7, 7, 1, 426, false, 2, "许昌", "陈留", 100, "多疑", new string[]{"雄才"}, new string[]{"文学","剑术","美色"}, "无", new string[]{"2;5"}, null, new string[]{"逼禅称帝","著《典论》"}, 1, new int[0], "敏", "", "def", "SwordHitYellowCritical", "caopi");
            config[102022] = new HeroConfig(102022, "曹植", 192, 232, 64, 67, 75, 65, 74, 4, 3, 3, 3, 5, 4, 4, 1, 363, false, 2, "许昌", "陈留", 100, "豪爽", new string[]{"才思"}, new string[]{"饮酒","赋诗","清谈"}, "无", new string[]{"2;5"}, null, new string[]{"七步成诗","洛神赋"}, 3, new int[0], "虐", "", "inte", "FanExplosion", "caozhi");
            config[102023] = new HeroConfig(102023, "刘晔", 175, 234, 65, 49, 92, 75, 69, 5, 4, 5, 5, 8, 5, 5, 1, 377, true, 2, "小沛", "寿春", 92, "冷静", new string[]{"智识","洞察"}, new string[]{"发明"}, "无", null, null, new string[]{"献投石车破袁绍"}, 3, new int[0], "", "", "shoot", "GasShootFire", "liuye");
            config[102024] = new HeroConfig(102024, "朱灵", 168, 222, 73, 77, 67, 53, 42, 7, 7, 6, 5, 5, 7, 7, 1, 342, false, 2, "宛", "平原", 90, "刚猛", new string[]{"忠义","果敢"}, new string[]{"骑马"}, "无", null, null, null, 3, new int[0], "", "", "atk", "SwordHitYellowCritical", "zhuling");
            config[102025] = new HeroConfig(102025, "曹彰", 189, 223, 82, 90, 37, 32, 71, 8, 9, 7, 7, 5, 8, 8, 1, 348, true, 2, "小沛", "陈留", 96, "刚猛", new string[]{"果敢"}, new string[]{"射猎","武艺"}, "无", null, null, new string[]{"北征乌桓"}, 1, new int[0], "青", "", "atk", "SwordHitYellowCritical", "caozhang");
            config[102026] = new HeroConfig(102026, "满宠", 162, 242, 84, 64, 82, 84, 50, 7, 6, 6, 5, 7, 7, 7, 1, 395, false, 2, "下邳", "陈留", 94, "冷静", new string[]{"清廉","果敢"}, new string[]{"律法"}, "无", new string[]{"2;5"}, null, null, 1, new int[0], "连", "", "atk", "SwordHitYellowCritical", "manchong");
            config[102027] = new HeroConfig(102027, "曹冲", 196, 208, 31, 21, 79, 74, 78, 2, 2, 2, 2, 5, 2, 2, 1, 296, false, 2, "许昌", "陈留", 100, "冷静", new string[]{"仁德","智识"}, new string[]{"发明","读书"}, "无", new string[]{"2;5"}, null, new string[]{"称象"}, 3, new int[0], "", "", "inte", "FanExplosion", "caochong");
            config[102028] = new HeroConfig(102028, "蒋济", 188, 249, 48, 43, 80, 73, 53, 4, 3, 4, 4, 6, 4, 4, 1, 318, false, 2, "北海", "寿春", 93, "多疑", new string[]{"智识","思辨"}, new string[]{"兵法"}, "无", null, null, null, 3, new int[0], "", "", "help", "SoulExplosionOrange", "jiangji");
            config[102029] = new HeroConfig(102029, "甄宓", 183, 221, 14, 3, 69, 64, 94, 1, 1, 1, 1, 4, 1, 1, 1, 252, true, 2, "下邳", "北平", 91, "隐忍", new string[]{"仁德","贤明"}, new string[]{"读书","音律","女红"}, "无", new string[]{"2;3"}, null, new string[]{"洛神赋原型"}, 3, new int[0], "白", "", "help", "SoulExplosionOrange", "zhenshi");
            config[102030] = new HeroConfig(102030, "戏志才", 169, 196, 66, 24, 88, 75, 70, 5, 4, 4, 4, 7, 5, 5, 1, 347, false, 2, "下邳", "许昌", 96, "多疑", new string[]{"雄才","洞察"}, new string[]{"饮酒"}, "无", null, null, null, 3, new int[0], "陷", "", "inte", "StormExplosion", "xizhicai");
            config[102032] = new HeroConfig(102032, "曹真", 180, 231, 82, 74, 65, 69, 84, 7, 7, 6, 5, 6, 7, 7, 1, 405, false, 2, "北海", "陈留", 96, "刚猛", new string[]{"果敢","坚韧"}, new string[]{"射猎","骑马"}, "无", new string[]{"2;4"}, null, null, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "caozhen");
            config[102033] = new HeroConfig(102033, "郭淮", 187, 255, 87, 78, 76, 71, 73, 7, 7, 6, 5, 6, 7, 7, 1, 416, false, 2, "陈留", "晋阳", 93, "冷静", new string[]{"坚韧","思辨"}, new string[]{"兵法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "guohuai");
            config[102034] = new HeroConfig(102034, "夏侯尚", 185, 225, 79, 75, 72, 63, 59, 7, 7, 6, 5, 6, 7, 7, 1, 379, false, 2, "陈留", "陈留", 92, "急躁", new string[]{"果敢","忠义"}, new string[]{"骑马"}, "无", new string[]{"2;5"}, null, null, 1, new int[0], "敏", "", "def", "SwordHitYellowCritical", "xiahoushang");
            config[102035] = new HeroConfig(102035, "钟繇", 151, 230, 70, 37, 72, 91, 76, 5, 4, 4, 4, 6, 5, 5, 1, 369, true, 2, "许昌", "许昌", 94, "谦和", new string[]{"贤明","勤勉"}, new string[]{"书法","读书"}, "无", null, null, null, 3, new int[0], "", "", "help", "SharpExplosionGreen", "zhongyao");
            config[102036] = new HeroConfig(102036, "田予", 165, 227, 80, 72, 80, 78, 75, 7, 6, 6, 5, 6, 7, 7, 1, 415, false, 2, "北海", "吴", 90, "冷静", new string[]{"坚韧","忠义"}, new string[]{"射箭"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "tianyu");
            config[102037] = new HeroConfig(102037, "张燕", 165, 210, 79, 78, 51, 46, 61, 7, 7, 6, 5, 5, 7, 7, 1, 345, false, 2, "濮阳", "新野", 88, "豪爽", new string[]{"果敢","忠义"}, new string[]{"结交"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangyan");
            config[102038] = new HeroConfig(102038, "陈登", 163, 201, 79, 64, 81, 82, 61, 7, 6, 6, 5, 6, 7, 7, 1, 397, false, 2, "小沛", "宛", 92, "豪爽", new string[]{"雄才","果敢"}, new string[]{"美食"}, "无", new string[]{"1;3","2;3"}, new string[]{"5;3"}, new string[]{"助曹操破吕布"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chendeng");
            config[102039] = new HeroConfig(102039, "贾逵", 174, 228, 78, 61, 84, 85, 75, 7, 6, 6, 5, 6, 7, 7, 1, 413, false, 2, "北海", "陈留", 92, "刚直", new string[]{"忠义","果敢"}, new string[]{"水利"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "jiakui");
            config[102040] = new HeroConfig(102040, "曹纯", 170, 210, 75, 71, 60, 35, 72, 7, 8, 6, 5, 5, 7, 7, 1, 344, false, 2, "许昌", "濮阳", 94, "冷静", new string[]{"专注","果敢"}, new string[]{"练兵"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "caochun");
            config[102041] = new HeroConfig(102041, "王凌", 172, 251, 74, 64, 70, 82, 71, 6, 6, 5, 5, 6, 6, 6, 1, 389, false, 2, "下邳", "下邳", 89, "隐忍", new string[]{"雄才"}, new string[]{"兵法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangling");
            config[102042] = new HeroConfig(102042, "张既", 168, 223, 74, 35, 75, 89, 81, 5, 4, 4, 4, 6, 5, 5, 1, 377, false, 2, "宛", "小沛", 90, "谦和", new string[]{"贤明","勤勉"}, new string[]{"治理"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangji2");
            config[102043] = new HeroConfig(102043, "梁习", 160, 223, 73, 40, 73, 87, 80, 5, 4, 4, 4, 6, 5, 5, 1, 376, false, 2, "宛", "北海", 89, "冷静", new string[]{"勤勉","果敢"}, new string[]{"治理"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liangxi");
            config[102044] = new HeroConfig(102044, "李通", 165, 209, 73, 81, 57, 63, 83, 7, 7, 6, 5, 5, 7, 7, 1, 388, false, 2, "下邳", "平原", 91, "刚猛", new string[]{"忠义","果敢"}, new string[]{"骑射"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "litong");
            config[102046] = new HeroConfig(102046, "孙观", 170, 213, 72, 78, 51, 39, 66, 7, 6, 6, 5, 4, 7, 7, 1, 334, false, 2, "陈留", "南皮", 87, "豪爽", new string[]{"果敢"}, new string[]{"饮酒","结交"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "sunguan");
            config[102047] = new HeroConfig(102047, "夏侯德", 170, 219, 69, 73, 32, 40, 52, 6, 6, 5, 5, 4, 6, 6, 1, 292, false, 2, "陈留", "晋阳", 88, "急躁", new string[]{"刚愎"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xiahoude");
            config[102048] = new HeroConfig(102048, "杜畿", 163, 224, 66, 32, 74, 87, 76, 4, 3, 3, 3, 5, 4, 4, 1, 353, false, 2, "陈留", "安定", 91, "谦和", new string[]{"仁德","勤勉"}, new string[]{"农事"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "duji");
            config[102049] = new HeroConfig(102049, "刘馥", 174, 208, 64, 49, 73, 89, 83, 4, 3, 3, 3, 5, 4, 4, 1, 376, false, 2, "陈留", "天水", 90, "冷静", new string[]{"勤勉","贤明"}, new string[]{"水利","农事"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liufu");
            config[102050] = new HeroConfig(102050, "夏侯恩", 175, 219, 63, 71, 51, 44, 70, 6, 6, 5, 4, 4, 6, 6, 1, 324, false, 2, "陈留", "武威", 75, "急躁", new string[]{"怯懦"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xiahoueng");
            config[102051] = new HeroConfig(102051, "温恢", 171, 223, 62, 36, 73, 86, 76, 4, 3, 3, 3, 5, 4, 4, 1, 351, false, 2, "下邳", "武陵", 89, "谦和", new string[]{"贤明","勤勉"}, new string[]{"治理"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wenhui");
            config[102052] = new HeroConfig(102052, "毛玠", 165, 216, 62, 38, 58, 79, 60, 4, 3, 3, 3, 4, 4, 4, 1, 314, false, 2, "下邳", "零陵", 92, "清廉", new string[]{"清廉","贤明"}, new string[]{"书法"}, "无", null, null, new string[]{"提出奉天子以令不臣"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "maojie");
            config[102053] = new HeroConfig(102053, "陈矫", 169, 237, 61, 27, 76, 83, 64, 4, 3, 3, 3, 5, 4, 4, 1, 329, false, 2, "濮阳", "江陵", 91, "冷静", new string[]{"贤明","思辨"}, new string[]{"律法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenjiao");
            config[102054] = new HeroConfig(102054, "吕虔", 168, 227, 57, 70, 58, 74, 60, 6, 5, 5, 5, 5, 6, 6, 1, 340, false, 2, "濮阳", "江夏", 90, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lvqian");
            config[102055] = new HeroConfig(102055, "徐邈", 171, 249, 55, 32, 67, 82, 79, 4, 3, 3, 3, 5, 4, 4, 1, 333, false, 2, "宛", "桂阳", 89, "豪爽", new string[]{"贤明"}, new string[]{"饮酒"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xumiao");
            config[102056] = new HeroConfig(102056, "王修", 170, 217, 54, 27, 76, 79, 63, 4, 3, 3, 3, 5, 4, 4, 1, 317, false, 2, "许昌", "长沙", 90, "刚直", new string[]{"忠义","仁德"}, new string[]{"读书"}, "无", null, null, new string[]{"哭袁谭"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangxiu");
            config[102057] = new HeroConfig(102057, "国渊", 170, 215, 49, 18, 70, 85, 73, 4, 3, 3, 3, 5, 4, 4, 1, 313, false, 2, "濮阳", "庐江", 90, "谦和", new string[]{"清廉","勤勉"}, new string[]{"算术"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "guoyuan");
            config[102058] = new HeroConfig(102058, "典满", 170, 220, 49, 71, 38, 25, 50, 6, 5, 5, 4, 4, 6, 6, 1, 257, false, 2, "陈留", "会稽", 94, "刚猛", new string[]{"忠义"}, new string[]{"舞戟"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "dianman");
            config[102059] = new HeroConfig(102059, "娄圭", 165, 201, 48, 12, 81, 63, 14, 4, 3, 4, 3, 5, 4, 4, 1, 237, false, 2, "濮阳", "柴桑", 86, "多疑", new string[]{"智识"}, new string[]{"兵法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lougui");
            config[102060] = new HeroConfig(102060, "王朗", 152, 228, 46, 34, 79, 84, 51, 3, 2, 3, 3, 5, 3, 3, 1, 310, false, 2, "宛", "永安", 82, "急躁", new string[]{"才思"}, new string[]{"读书","辩论"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wanglang");
            config[102061] = new HeroConfig(102061, "卞氏", 159, 230, 35, 23, 74, 76, 87, 2, 1, 4, 1, 4, 2, 2, 1, 303, false, 2, "许昌", "江州", 93, "谦和", new string[]{"仁德","贤明"}, new string[]{"女红"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "nvbianshi2");
            config[102063] = new HeroConfig(102063, "李孚", 170, 230, 30, 35, 73, 72, 68, 4, 3, 3, 3, 5, 4, 4, 1, 296, false, 2, "下邳", "建宁", 88, "多疑", new string[]{"智识","果敢"}, new string[]{"权谋"}, "无", null, null, new string[]{"邺城突围"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lifu");
            config[102065] = new HeroConfig(102065, "孔融", 153, 208, 29, 4, 69, 76, 63, 2, 1, 2, 1, 4, 2, 2, 1, 251, false, 2, "北海", "北海", 84, "刚直", new string[]{"仁德","才思"}, new string[]{"读书","清谈"}, "无", null, null, new string[]{"孔融让梨"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "kongrong");
            config[102066] = new HeroConfig(102066, "司马朗", 171, 217, 20, 21, 71, 84, 81, 3, 2, 2, 2, 5, 3, 3, 1, 291, false, 2, "陈留", "梓潼", 90, "谦和", new string[]{"仁德","勤勉"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "simalang");
            config[102067] = new HeroConfig(102067, "郭奕", 185, 220, 19, 27, 66, 72, 44, 2, 2, 2, 2, 4, 2, 2, 1, 240, false, 2, "陈留", "上庸", 85, "冷静", new string[]{"思辨"}, new string[]{"围棋"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "guoyi");
            config[102068] = new HeroConfig(102068, "董昭", 156, 236, 18, 24, 78, 83, 57, 3, 2, 3, 3, 5, 3, 3, 1, 276, false, 2, "小沛", "汉中", 88, "多疑", new string[]{"雄才","狡诈"}, new string[]{"权谋"}, "无", null, null, new string[]{"劝曹操进魏公"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "dongzhao");
            config[102070] = new HeroConfig(102070, "崔琰", 163, 216, 17, 54, 69, 84, 74, 4, 3, 3, 3, 5, 4, 4, 1, 316, false, 2, "濮阳", "汝南", 91, "刚直", new string[]{"清廉","贤明"}, new string[]{"书法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "cuiyan");
            config[102071] = new HeroConfig(102071, "吴质", 178, 230, 16, 29, 68, 57, 37, 2, 2, 2, 2, 4, 2, 2, 1, 219, false, 2, "小沛", "寿春", 82, "狡诈", new string[]{"才思"}, new string[]{"文学","结交"}, "无", null, null, new string[]{"助曹丕争储"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wuzhi");
            config[102072] = new HeroConfig(102072, "桓阶", 162, 221, 9, 25, 65, 76, 67, 2, 2, 2, 2, 4, 2, 2, 1, 254, false, 2, "濮阳", "北平", 90, "冷静", new string[]{"贤明","思辨"}, new string[]{"兵法"}, "无", null, null, new string[]{"劝曹操立曹丕"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "huanjie");
            config[102073] = new HeroConfig(102073, "蒋干", 174, 212, 9, 6, 65, 64, 47, 2, 1, 2, 1, 5, 2, 2, 1, 200, false, 2, "濮阳", "蓟", 70, "多疑", new string[]{"能言"}, new string[]{"清谈"}, "无", null, null, new string[]{"蒋干盗书（虽中计","但算事件）"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "jianggan");
            config[103001] = new HeroConfig(103001, "甘宁", 175, 215, 93, 94, 76, 18, 58, 8, 7, 8, 9, 7, 8, 9, 1, 378, true, 3, "长沙", "江州", 97, "豪爽", new string[]{"果敢","忠义"}, new string[]{"舞剑","珍宝"}, "无", new string[]{"3;2"}, null, new string[]{"百骑劫魏营"}, 3, new int[0], "连", "", "shoot", "BulletExplosionBlue", "ganning");
            config[103002] = new HeroConfig(103002, "太史慈", 166, 206, 85, 93, 66, 58, 79, 8, 7, 9, 7, 7, 9, 9, 1, 419, true, 3, "吴", "北海", 96, "刚猛", new string[]{"忠义","果敢"}, new string[]{"射箭","舞戟"}, "无", new string[]{"3;3"}, null, new string[]{"北海救孔融"}, 3, new int[0], "雨", "", "shoot", "BulletExplosionBlue", "taishici");
            config[103003] = new HeroConfig(103003, "黄盖", 155, 210, 79, 83, 65, 65, 80, 7, 6, 6, 8, 6, 7, 7, 1, 405, false, 3, "桂阳", "零陵", 100, "刚猛", new string[]{"忠义","坚韧"}, new string[]{"舞鞭","练兵"}, "无", new string[]{"3;5"}, null, new string[]{"苦肉计"}, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "huanggai");
            config[103004] = new HeroConfig(103004, "周泰", 170, 225, 76, 91, 48, 38, 60, 9, 7, 7, 8, 7, 9, 9, 1, 351, true, 3, "长沙", "庐江", 97, "刚猛", new string[]{"忠义","果敢"}, new string[]{"舞刀","饮酒"}, "无", null, null, new string[]{"濡须救孙权"}, 1, new int[0], "连", "", "atk", "SwordHitYellowCritical", "zhoutai");
            config[103005] = new HeroConfig(103005, "鲁肃", 172, 217, 80, 56, 92, 92, 89, 6, 5, 5, 7, 7, 6, 6, 1, 439, true, 3, "柴桑", "寿春", 100, "谦和", new string[]{"仁德","雄才"}, new string[]{"读书","结交"}, "无", new string[]{"3;5"}, null, new string[]{"借荆州","单刀赴会"}, 3, new int[0], "", "雷", "help", "SharpExplosionGreen", "lusu");
            config[103006] = new HeroConfig(103006, "周瑜", 175, 210, 96, 71, 96, 86, 93, 7, 7, 6, 9, 8, 7, 7, 1, 479, true, 3, "柴桑", "庐江", 100, "豪爽", new string[]{"雄才","贤明"}, new string[]{"音律","饮酒","舞剑"}, "无", new string[]{"3;5"}, new string[]{"1;5","2;5"}, new string[]{"火烧赤壁"}, 3, new int[0], "炎", "炽", "inte", "ExplosionFireballFire", "zhouyu");
            config[103007] = new HeroConfig(103007, "蒋钦", 172, 220, 78, 84, 51, 42, 74, 7, 6, 6, 8, 6, 7, 7, 1, 362, false, 3, "桂阳", "寿春", 95, "冷静", new string[]{"忠义","果敢"}, new string[]{"射箭"}, "无", null, null, null, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "jiangqing");
            config[103008] = new HeroConfig(103008, "吕蒙", 178, 219, 91, 81, 84, 74, 82, 8, 7, 6, 8, 7, 8, 8, 1, 448, true, 3, "柴桑", "汝南", 98, "果敢", new string[]{"果敢","勤勉"}, new string[]{"读书","兵法"}, "无", new string[]{"3;4"}, null, new string[]{"白衣渡江","孙权劝学"}, 1, new int[0], "学", "羽", "def", "SwordHitYellowCritical", "lvmeng");
            config[103009] = new HeroConfig(103009, "陆逊", 183, 245, 96, 69, 92, 87, 90, 7, 6, 6, 8, 8, 7, 7, 1, 469, true, 3, "吴", "吴", 100, "冷静", new string[]{"雄才","思辨"}, new string[]{"读书","围棋"}, "无", new string[]{"3;3"}, null, new string[]{"火烧连营"}, 3, new int[0], "炎", "", "inte", "GasExplosionFire", "luxun");
            config[103010] = new HeroConfig(103010, "张昭", 156, 236, 32, 2, 83, 98, 79, 3, 2, 2, 4, 6, 3, 3, 1, 311, true, 3, "桂阳", "下邳", 100, "刚直", new string[]{"贤明","忠义"}, new string[]{"读书","书法"}, "无", new string[]{"3;3"}, null, null, 3, new int[0], "", "", "help", "SharpExplosionGreen", "zhangzhao");
            config[103011] = new HeroConfig(103011, "诸葛瑾", 174, 241, 72, 34, 81, 90, 90, 5, 4, 4, 6, 7, 5, 5, 1, 393, true, 3, "吴", "襄阳", 96, "谦和", new string[]{"仁德","思辨"}, new string[]{"读书","书法"}, "无", new string[]{"1;1|3;4"}, null, null, 3, new int[0], "", "", "help", "FanExplosion", "zhugejin");
            config[103012] = new HeroConfig(103012, "孙尚香", 191, 222, 69, 83, 64, 61, 70, 7, 7, 7, 7, 5, 7, 7, 1, 380, false, 3, "吴", "吴", 96, "豪爽", new string[]{"果敢"}, new string[]{"舞剑","射猎"}, "无", new string[]{"1;3|3;3"}, null, new string[]{"孙刘联姻"}, 3, new int[0], "", "", "shoot", "BulletExplosionBlue", "sunshangxiang");
            config[103013] = new HeroConfig(103013, "朱桓", 177, 238, 84, 82, 75, 56, 59, 7, 7, 6, 7, 6, 8, 7, 1, 389, false, 3, "桂阳", "吴", 94, "刚愎", new string[]{"果敢"}, new string[]{"舞槊"}, "无", new string[]{"3;5"}, null, null, 1, new int[0], "伏", "缓", "def", "SwordHitYellowCritical", "zhuhuan");
            config[103014] = new HeroConfig(103014, "大乔", 175, 221, 17, 11, 72, 78, 92, 2, 1, 2, 3, 4, 2, 2, 1, 280, true, 3, "吴", "庐江", 100, "隐忍", new string[]{"贤明"}, new string[]{"音律","女红"}, "无", null, null, null, 3, new int[0], "", "陷", "help", "StormExplosion", "daqiao");
            config[103015] = new HeroConfig(103015, "小乔", 176, 223, 16, 12, 73, 77, 92, 2, 1, 3, 3, 4, 2, 2, 1, 280, true, 3, "柴桑", "庐江", 100, "豪爽", new string[]{"贤明"}, new string[]{"音律","女红"}, "无", null, null, null, 3, new int[0], "", "陷", "help", "StormExplosion", "xiaoqiao");
            config[103016] = new HeroConfig(103016, "丁奉", 186, 271, 76, 95, 66, 51, 52, 8, 7, 7, 8, 7, 8, 8, 1, 377, true, 3, "长沙", "庐江", 94, "刚猛", new string[]{"果敢"}, new string[]{"舞刀"}, "无", null, null, new string[]{"雪中奋短兵"}, 3, new int[0], "", "", "shoot", "GasShootFire", "dingfeng");
            config[103017] = new HeroConfig(103017, "董袭", 170, 213, 72, 85, 50, 48, 60, 7, 6, 6, 8, 5, 7, 8, 1, 347, false, 3, "长沙", "会稽", 93, "刚猛", new string[]{"果敢","忠义"}, new string[]{"舞戟"}, "无", null, null, null, 1, new int[0], "透", "", "atk", "SwordHitYellowCritical", "dongxi");
            config[103018] = new HeroConfig(103018, "凌统", 189, 237, 72, 83, 54, 37, 66, 7, 6, 6, 7, 5, 7, 7, 1, 343, false, 3, "建业", "吴", 96, "急躁", new string[]{"果敢","忠义"}, new string[]{"舞刀"}, "无", null, null, new string[]{"与甘宁和解"}, 3, new int[0], "虐", "", "shoot", "BulletExplosionBlue", "lingtong");
            config[103019] = new HeroConfig(103019, "潘璋", 176, 234, 76, 80, 70, 28, 12, 7, 6, 6, 7, 4, 7, 7, 1, 296, false, 3, "庐江", "濮阳", 88, "贪婪", new string[]{"果敢"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "刺", "虐", "def", "SwordHitYellowCritical", "panzhang");
            config[103020] = new HeroConfig(103020, "朱治", 156, 224, 66, 78, 42, 39, 64, 6, 5, 5, 7, 5, 6, 6, 1, 317, false, 3, "柴桑", "会稽", 92, "谦和", new string[]{"忠义","勤勉"}, new string[]{"读书"}, "无", null, null, null, 3, new int[0], "敏", "", "shoot", "BulletExplosionBlue", "zhuzhi");
            config[103021] = new HeroConfig(103021, "徐盛", 177, 228, 87, 81, 78, 65, 73, 7, 6, 6, 8, 6, 7, 7, 1, 417, false, 3, "庐江", "北海", 94, "冷静", new string[]{"忠义","坚韧"}, new string[]{"射箭"}, "无", null, null, null, 1, new int[0], "乱", "", "def", "SwordHitYellowCritical", "xusheng");
            config[103022] = new HeroConfig(103022, "程普", 138, 215, 80, 75, 73, 71, 81, 7, 6, 6, 8, 6, 7, 8, 1, 413, false, 3, "庐江", "北平", 97, "刚猛", new string[]{"忠义","果敢"}, new string[]{"舞铁戟"}, "无", new string[]{"3;5"}, null, new string[]{"与周瑜和解"}, 1, new int[0], "实", "", "def", "SwordHitYellowCritical", "chengpu");
            config[103023] = new HeroConfig(103023, "张纮", 153, 212, 23, 21, 83, 94, 79, 2, 1, 2, 4, 5, 2, 2, 1, 314, true, 3, "会稽", "寿春", 100, "谦和", new string[]{"贤明","思辨"}, new string[]{"书法","读书"}, "无", null, null, new string[]{"谏孙权迁都"}, 3, new int[0], "", "", "help", "SharpExplosionGreen", "zhanghong");
            config[103024] = new HeroConfig(103024, "顾雍", 168, 243, 43, 18, 80, 92, 76, 4, 3, 3, 5, 6, 4, 4, 1, 330, true, 3, "桂阳", "吴", 95, "冷静", new string[]{"贤明","仁德"}, new string[]{"饮酒","书法"}, "无", null, null, null, 3, new int[0], "连", "", "inte", "FanExplosion", "guyong");
            config[103025] = new HeroConfig(103025, "步骘", 177, 247, 72, 51, 84, 87, 65, 5, 4, 4, 6, 6, 5, 5, 1, 384, false, 3, "长沙", "寿春", 92, "谦和", new string[]{"贤明","勤勉"}, new string[]{"读书"}, "无", null, null, null, 3, new int[0], "", "", "help", "SoulExplosionOrange", "buzhi");
            config[103026] = new HeroConfig(103026, "阚泽", 170, 243, 42, 48, 87, 85, 71, 4, 3, 3, 5, 6, 4, 4, 1, 354, false, 3, "长沙", "会稽", 93, "冷静", new string[]{"忠义","智识"}, new string[]{"读书","算术"}, "无", null, null, new string[]{"献诈降书"}, 3, new int[0], "炽", "", "inte", "StormExplosion", "kanze");
            config[103027] = new HeroConfig(103027, "韩当", 156, 223, 75, 84, 54, 49, 67, 7, 6, 6, 8, 5, 7, 7, 1, 361, false, 3, "柴桑", "北平", 100, "刚猛", new string[]{"忠义","果敢"}, new string[]{"射箭","骑马"}, "无", new string[]{"3;5"}, null, null, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "handang");
            config[103029] = new HeroConfig(103029, "苏飞", 170, 215, 69, 63, 66, 72, 70, 6, 5, 5, 7, 5, 6, 6, 1, 368, false, 3, "长沙", "庐江", 90, "豪爽", new string[]{"忠义"}, new string[]{"结交"}, "无", null, null, new string[]{"救甘宁"}, 1, new int[0], "复", "", "def", "SwordHitYellowCritical", "sufei");
            config[103031] = new HeroConfig(103031, "陈武", 178, 215, 74, 87, 43, 40, 62, 7, 6, 6, 7, 5, 7, 7, 1, 337, false, 3, "桂阳", "会稽", 92, "刚猛", new string[]{"果敢"}, new string[]{"舞刀"}, "无", null, null, null, 1, new int[0], "劫", "", "def", "SwordHitYellowCritical", "chengwu");
            config[103032] = new HeroConfig(103032, "朱然", 182, 249, 77, 67, 69, 58, 73, 6, 5, 5, 7, 5, 6, 6, 1, 372, false, 3, "吴", "柴桑", 94, "冷静", new string[]{"坚韧","果敢"}, new string[]{"守城"}, "无", null, null, new string[]{"江陵御魏"}, 1, new int[0], "竟", "", "def", "SwordHitYellowCritical", "zhuran");
            config[103033] = new HeroConfig(103033, "孙韶", 188, 241, 76, 75, 71, 65, 70, 6, 5, 5, 7, 5, 6, 6, 1, 385, false, 3, "吴", "永安", 93, "急躁", new string[]{"果敢","忠义"}, new string[]{"骑马"}, "无", null, null, null, 1, new int[0], "破", "", "def", "SwordHitYellowCritical", "sunshao");
            config[103035] = new HeroConfig(103035, "严畯", 183, 243, 13, 2, 70, 85, 72, 2, 1, 2, 3, 5, 2, 2, 1, 255, false, 3, "长沙", "江州", 89, "谦和", new string[]{"贤明","思辨"}, new string[]{"读书"}, "无", null, null, new string[]{"不任都督"}, 3, new int[0], "虐", "", "inte", "FanExplosion", "yanjun");
            config[103036] = new HeroConfig(103036, "吕范", 161, 228, 73, 63, 74, 77, 69, 5, 4, 4, 6, 5, 5, 5, 1, 380, false, 3, "庐江", "建宁", 90, "冷静", new string[]{"贤明","清廉"}, new string[]{"书法"}, "无", null, null, new string[]{"佐孙策平江东"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lvfan");
            config[103037] = new HeroConfig(103037, "马忠", 183, 249, 64, 73, 61, 34, 36, 6, 5, 6, 7, 4, 6, 6, 1, 296, false, 3, "桂阳", "云南", 87, "冷静", new string[]{"忠义","果敢"}, new string[]{"射箭"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "mazhong");
            config[103038] = new HeroConfig(103038, "贾华", 178, 234, 49, 65, 71, 29, 52, 5, 5, 5, 6, 4, 5, 5, 1, 291, false, 3, "建业", "梓潼", 86, "刚猛", new string[]{"果敢"}, new string[]{"舞刀"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "jiahua");
            config[103039] = new HeroConfig(103039, "贺齐", 170, 227, 83, 78, 42, 64, 73, 7, 6, 6, 7, 5, 7, 7, 1, 371, false, 3, "会稽", "上庸", 92, "冷静", new string[]{"果敢","勤勉"}, new string[]{"兵器","造船"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "heqi");
            config[103040] = new HeroConfig(103040, "留赞", 183, 255, 78, 75, 64, 57, 62, 7, 6, 6, 7, 5, 7, 7, 1, 367, false, 3, "会稽", "汉中", 90, "刚猛", new string[]{"果敢"}, new string[]{"舞刀","饮酒"}, "无", null, null, new string[]{"东兴之战"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liuzhan");
            config[103041] = new HeroConfig(103041, "虞翻", 164, 233, 43, 46, 86, 83, 46, 4, 3, 3, 5, 6, 4, 4, 1, 325, false, 3, "会稽", "汝南", 83, "刚直", new string[]{"智识","思辨"}, new string[]{"易学","围棋"}, "无", null, null, new string[]{"骂关羽"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lufan");
            config[103042] = new HeroConfig(103042, "陆绩", 187, 219, 19, 48, 61, 69, 41, 2, 4, 2, 3, 4, 2, 2, 1, 250, false, 3, "会稽", "寿春", 86, "刚直", new string[]{"仁德"}, new string[]{"读书","算术"}, "无", null, null, new string[]{"怀橘遗亲"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "luji");
            config[103043] = new HeroConfig(103043, "程秉", 168, 225, 16, 15, 71, 73, 65, 2, 6, 2, 3, 4, 2, 2, 1, 252, false, 3, "庐江", "北平", 89, "谦和", new string[]{"贤明","勤勉"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenbing");
            config[103044] = new HeroConfig(103044, "吕岱", 161, 256, 81, 70, 68, 74, 62, 7, 6, 5, 7, 5, 7, 7, 1, 385, false, 3, "柴桑", "蓟", 91, "冷静", new string[]{"果敢","勤勉"}, new string[]{"治理"}, "无", null, null, new string[]{"平定交州"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lvdai");
            config[103045] = new HeroConfig(103045, "孙瑜", 177, 215, 77, 70, 68, 69, 78, 6, 5, 5, 7, 5, 6, 6, 1, 390, false, 3, "柴桑", "襄平", 89, "谦和", new string[]{"仁德","勤勉"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "sunyu");
            config[103046] = new HeroConfig(103046, "吾粲", 190, 245, 66, 40, 76, 73, 70, 4, 3, 3, 5, 5, 4, 4, 1, 345, false, 3, "柴桑", "洛阳", 88, "刚直", new string[]{"忠义","仁德"}, new string[]{"治理"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wucan");
            config[103047] = new HeroConfig(103047, "李异", 180, 231, 56, 73, 18, 17, 22, 5, 5, 5, 6, 3, 5, 5, 1, 210, false, 3, "庐江", "长安", 86, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liyi");
            config[103048] = new HeroConfig(103048, "张承", 178, 244, 77, 70, 75, 74, 74, 5, 4, 4, 6, 6, 5, 5, 1, 395, false, 3, "建业", "许昌", 90, "谦和", new string[]{"忠义","贤明"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangcheng");
            config[103049] = new HeroConfig(103049, "孙皎", 183, 219, 75, 70, 64, 69, 71, 6, 5, 5, 7, 5, 6, 6, 1, 377, false, 3, "建业", "邺", 90, "豪爽", new string[]{"果敢","仁德"}, new string[]{"结交"}, "无", null, null, new string[]{"与甘宁和解"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "sunjiao");
            config[103050] = new HeroConfig(103050, "宋谦", 182, 235, 69, 54, 70, 73, 73, 5, 4, 4, 6, 5, 5, 5, 1, 363, false, 3, "建业", "襄阳", 88, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "songqian");
            config[103052] = new HeroConfig(103052, "孙匡", 175, 204, 49, 44, 45, 62, 53, 4, 3, 3, 4, 4, 4, 4, 1, 271, false, 3, "吴", "襄阳", 86, "谦和", new string[]{"贤明"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "sunkuang");
            config[103054] = new HeroConfig(103054, "薛综", 176, 243, 32, 15, 68, 77, 59, 2, 1, 2, 3, 4, 2, 2, 1, 263, false, 3, "柴桑", "建业", 90, "谦和", new string[]{"贤明","思辨"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xuezong");
            config[103055] = new HeroConfig(103055, "孙朗", 185, 223, 32, 40, 28, 38, 42, 3, 2, 2, 3, 3, 3, 3, 1, 193, false, 3, "柴桑", "吴", 85, "多疑", new string[]{"怯懦"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "sunlang");
            config[103056] = new HeroConfig(103056, "吴国太", 156, 216, 29, 20, 70, 74, 81, 1, 1, 1, 3, 4, 1, 1, 1, 284, false, 3, "吴", "新野", 98, "隐忍", new string[]{"仁德","贤明"}, new string[]{"女红"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wuguotai");
            config[104001] = new HeroConfig(104001, "张郃", 167, 231, 89, 90, 69, 56, 70, 9, 10, 7, 5, 7, 10, 8, 1, 400, true, 4, "邺", "南皮", 95, "冷静", new string[]{"坚韧","果敢"}, new string[]{"兵法","武艺"}, "无", null, null, new string[]{"街亭破马谡"}, 1, new int[0], "分", "", "def", "SwordHitYellowCritical", "zhanghe");
            config[104002] = new HeroConfig(104002, "颜良", 165, 200, 78, 93, 42, 32, 53, 9, 10, 7, 5, 7, 9, 7, 1, 318, true, 4, "南皮", "平原", 100, "刚愎", new string[]{"果敢"}, new string[]{"舞刀"}, "无", null, null, null, 1, new int[0], "破", "", "atk", "SwordHitYellowCritical", "yanliang");
            config[104003] = new HeroConfig(104003, "文丑", 163, 200, 79, 92, 48, 52, 68, 9, 9, 7, 5, 7, 7, 9, 1, 359, true, 4, "晋阳", "平原", 100, "急躁", new string[]{"果敢"}, new string[]{"舞刀"}, "无", null, null, null, 1, new int[0], "刺", "", "def", "SwordHitYellowCritical", "wenchou");
            config[104004] = new HeroConfig(104004, "田丰", 170, 200, 72, 33, 93, 89, 64, 4, 3, 3, 3, 7, 4, 4, 1, 367, true, 4, "晋阳", "南皮", 94, "刚直", new string[]{"忠义","智识"}, new string[]{"读书"}, "无", new string[]{"4;5"}, null, new string[]{"谏袁绍被下狱"}, 3, new int[0], "雷", "", "inte", "StormExplosion", "tianfeng");
            config[104005] = new HeroConfig(104005, "鞠义", 158, 191, 72, 78, 55, 18, 37, 8, 7, 7, 4, 6, 8, 8, 1, 282, false, 4, "平原", "武威", 89, "刚愎", new string[]{"果敢"}, new string[]{"练兵"}, "无", null, null, new string[]{"破公孙瓒"}, 3, new int[0], "", "", "shoot", "BulletExplosionBlue", "juyi");
            config[104006] = new HeroConfig(104006, "许攸", 155, 204, 39, 29, 80, 57, 23, 3, 2, 3, 2, 5, 3, 3, 1, 241, false, 4, "邺", "宛", 78, "贪婪", new string[]{"智识"}, new string[]{"珍宝"}, "无", null, null, new string[]{"乌巢烧粮"}, 3, new int[0], "火", "", "inte", "StormExplosion", "xuyou");
            config[104007] = new HeroConfig(104007, "高览", 168, 200, 76, 82, 66, 55, 62, 8, 7, 5, 4, 6, 8, 8, 1, 361, false, 4, "邺", "汝南", 92, "冷静", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "gaolan");
            config[104008] = new HeroConfig(104008, "沮授", 169, 200, 78, 35, 90, 91, 74, 5, 3, 3, 3, 7, 5, 5, 1, 386, true, 4, "邺", "南皮", 96, "忠义", new string[]{"忠义","智识"}, new string[]{"读书"}, "无", new string[]{"4;5"}, null, new string[]{"拒降曹"}, 3, new int[0], "", "", "inte", "StormExplosion", "jushou");
            config[104009] = new HeroConfig(104009, "郭图", 170, 205, 52, 50, 82, 70, 37, 3, 2, 3, 2, 5, 3, 3, 1, 304, false, 4, "南皮", "许昌", 82, "狡诈", new string[]{"能言"}, new string[]{"权谋"}, "无", null, null, null, 3, new int[0], "", "", "help", "FanExplosion", "guotu");
            config[104011] = new HeroConfig(104011, "焦触", 172, 206, 65, 72, 33, 32, 39, 7, 6, 5, 3, 4, 7, 7, 1, 258, false, 4, "平原", "天水", 83, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "jiaochu");
            config[104012] = new HeroConfig(104012, "吕翔", 175, 204, 54, 71, 12, 19, 28, 6, 5, 4, 3, 4, 6, 6, 1, 197, false, 4, "邺", "武威", 84, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lvxiang");
            config[104013] = new HeroConfig(104013, "吕旷", 174, 204, 56, 70, 13, 22, 29, 6, 5, 4, 3, 4, 6, 6, 1, 203, false, 4, "南皮", "武陵", 84, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lvkuang");
            config[104014] = new HeroConfig(104014, "张南", 173, 207, 55, 69, 45, 33, 33, 6, 5, 5, 3, 4, 6, 6, 1, 250, false, 4, "南皮", "零陵", 83, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangnan");
            config[104015] = new HeroConfig(104015, "眭元进", 170, 207, 52, 68, 45, 32, 49, 6, 5, 5, 3, 4, 6, 6, 1, 261, false, 4, "邺", "江陵", 80, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "kuiyuanjin");
            config[104016] = new HeroConfig(104016, "牵招", 171, 231, 70, 67, 71, 73, 71, 7, 7, 6, 4, 5, 7, 7, 1, 372, false, 4, "平原", "江夏", 88, "冷静", new string[]{"忠义","果敢"}, new string[]{"骑射"}, "无", new string[]{"2;3"}, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "qianzhao");
            config[104017] = new HeroConfig(104017, "袁尚", 176, 207, 58, 66, 39, 35, 66, 6, 5, 4, 3, 4, 6, 6, 1, 280, false, 4, "南皮", "桂阳", 85, "骄横", new string[]{"果敢"}, new string[]{"射猎"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yuanshang");
            config[104018] = new HeroConfig(104018, "袁谭", 173, 205, 57, 66, 28, 33, 56, 6, 5, 4, 3, 3, 6, 6, 1, 255, false, 4, "晋阳", "长沙", 86, "多疑", new string[]{"果敢"}, new string[]{"结交"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yuantan");
            config[104019] = new HeroConfig(104019, "周昂", 175, 201, 74, 65, 62, 50, 62, 7, 6, 5, 4, 5, 7, 7, 1, 330, false, 4, "晋阳", "庐江", 82, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhouang");
            config[104020] = new HeroConfig(104020, "田畴", 169, 215, 64, 64, 70, 75, 72, 3, 2, 2, 2, 4, 3, 3, 1, 356, false, 4, "晋阳", "会稽", 91, "冷静", new string[]{"忠义","贤明"}, new string[]{"隐居","农耕"}, "无", null, null, new string[]{"助曹操征乌桓"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "tianchou");
            config[104021] = new HeroConfig(104021, "淳于琼", 163, 200, 70, 64, 28, 28, 35, 6, 5, 4, 3, 4, 6, 6, 1, 238, false, 4, "晋阳", "柴桑", 75, "贪婪", new string[]{"果敢"}, new string[]{"饮酒"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chunyuqiong");
            config[104022] = new HeroConfig(104022, "吕威璜", 172, 200, 58, 63, 29, 39, 44, 5, 4, 4, 3, 3, 5, 5, 1, 245, false, 4, "平原", "永安", 78, "贪婪", new string[]{"怯懦"}, new string[]{"饮酒"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lvweihuang");
            config[104023] = new HeroConfig(104023, "审配", 165, 204, 84, 60, 83, 73, 70, 7, 5, 5, 4, 7, 7, 7, 1, 389, false, 4, "平原", "邺", 96, "刚直", new string[]{"忠义","坚韧"}, new string[]{"守城"}, "无", new string[]{"4;5"}, new string[]{"2;3"}, new string[]{"邺城战死"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "shengpei");
            config[104024] = new HeroConfig(104024, "韩莒子", 170, 200, 51, 59, 51, 46, 52, 5, 4, 4, 3, 3, 5, 5, 1, 271, false, 4, "邺", "建宁", 79, "平庸", new string[]{"庸常"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "hanlvzi");
            config[104025] = new HeroConfig(104025, "苏由", 173, 206, 50, 59, 49, 41, 49, 5, 4, 4, 3, 3, 5, 5, 1, 260, false, 4, "晋阳", "云南", 80, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "suyou");
            config[104026] = new HeroConfig(104026, "蒋义渠", 174, 202, 71, 58, 57, 51, 55, 6, 5, 5, 3, 4, 6, 6, 1, 308, false, 4, "邺", "梓潼", 83, "冷静", new string[]{"忠义"}, new string[]{"练兵"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "jiangyiqu");
            config[104027] = new HeroConfig(104027, "高干", 170, 206, 72, 54, 47, 57, 64, 6, 5, 5, 3, 4, 6, 6, 1, 311, false, 4, "晋阳", "上庸", 82, "刚愎", new string[]{"果敢"}, new string[]{"骑马"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gaogan");
            config[104028] = new HeroConfig(104028, "袁熙", 170, 207, 62, 47, 59, 61, 60, 5, 4, 4, 3, 3, 5, 5, 1, 302, false, 4, "平原", "汉中", 84, "谦和", new string[]{"仁德"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yuanxi");
            config[104029] = new HeroConfig(104029, "陈震", 178, 235, 44, 44, 65, 73, 70, 3, 2, 2, 2, 4, 3, 3, 1, 306, false, 4, "南皮", "汝南", 90, "谦和", new string[]{"忠义"}, new string[]{"外交"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenzhen");
            config[104030] = new HeroConfig(104030, "辛评", 175, 204, 69, 43, 76, 75, 68, 3, 2, 2, 2, 5, 3, 3, 1, 342, false, 4, "邺", "寿春", 86, "刚直", new string[]{"忠义"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xinping");
            config[104031] = new HeroConfig(104031, "高柔", 174, 263, 54, 40, 67, 75, 70, 3, 2, 2, 2, 4, 3, 3, 1, 317, false, 4, "南皮", "北平", 92, "冷静", new string[]{"贤明","勤勉"}, new string[]{"律法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gaorou");
            config[104032] = new HeroConfig(104032, "荀谌", 172, 214, 19, 25, 77, 79, 64, 4, 2, 2, 2, 5, 4, 4, 1, 275, false, 4, "平原", "蓟", 89, "冷静", new string[]{"智识"}, new string[]{"清谈"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xunshen");
            config[104033] = new HeroConfig(104033, "辛毗", 171, 235, 37, 23, 75, 77, 69, 4, 2, 2, 2, 5, 4, 4, 1, 292, false, 4, "南皮", "襄平", 90, "刚直", new string[]{"忠义","贤明"}, new string[]{"书法"}, "无", null, null, new string[]{"助曹丕争储"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xinpi");
            config[104034] = new HeroConfig(104034, "逢纪", 175, 202, 27, 21, 84, 72, 39, 5, 2, 3, 2, 5, 5, 5, 1, 256, false, 4, "南皮", "宛", 84, "多疑", new string[]{"智识"}, new string[]{"权谋"}, "无", null, null, new string[]{"谗害田丰"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "fengji");
            config[104035] = new HeroConfig(104035, "陈琳", 160, 217, 10, 9, 74, 79, 72, 2, 1, 2, 1, 4, 2, 2, 1, 252, false, 4, "平原", "长安", 87, "刚直", new string[]{"才思"}, new string[]{"文章","书法"}, "无", null, null, new string[]{"讨曹檄文"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenlin");
            config[105001] = new HeroConfig(105001, "张辽", 169, 222, 95, 92, 78, 56, 76, 9, 10, 7, 5, 7, 9, 8, 1, 425, true, 5, "长安", "邺", 97, "冷静", new string[]{"忠义","果敢"}, new string[]{"武艺","兵法"}, "无", new string[]{"5;1","2;3"}, null, new string[]{"威震逍遥津"}, 1, new int[0], "旋", "", "def", "SwordHitYellowCritical", "zhangliao");
            config[105002] = new HeroConfig(105002, "吕布", 156, 199, 91, 100, 26, 13, 40, 8, 10, 7, 5, 7, 9, 8, 1, 297, true, 5, "洛阳", "晋阳", 85, "刚愎", new string[]{"果敢"}, new string[]{"武艺","赤兔马"}, "无", null, null, new string[]{"辕门射戟"}, 1, new int[0], "魔", "羽", "atk", "SwordHitBlackRedCritical", "lvbu");
            config[105003] = new HeroConfig(105003, "华雄", 154, 191, 82, 90, 56, 40, 57, 9, 8, 7, 5, 7, 8, 9, 1, 345, true, 5, "洛阳", "长安", 95, "刚猛", new string[]{"果敢"}, new string[]{"舞刀"}, "无", null, null, null, 1, new int[0], "纷", "", "atk", "SwordHitYellowCritical", "huaxiong");
            config[105004] = new HeroConfig(105004, "贾诩", 147, 223, 86, 50, 97, 85, 57, 5, 3, 3, 3, 7, 5, 5, 1, 393, true, 5, "安定", "武威", 98, "隐忍", new string[]{"智识","洞察"}, new string[]{"谋略","养生"}, "无", new string[]{"2;3","5;3"}, null, new string[]{"宛城献策","离间马超韩遂"}, 3, new int[0], "", "", "inte", "StormExplosion", "jiaxu");
            config[105005] = new HeroConfig(105005, "貂蝉", 169, 195, 27, 65, 81, 70, 95, 2, 2, 2, 1, 1, 2, 2, 1, 346, true, 5, "洛阳", "晋阳", 92, "隐忍", new string[]{"忠义","果敢"}, new string[]{"音律","舞姿"}, "无", new string[]{"5;3"}, null, new string[]{"连环计"}, 3, new int[0], "", "", "help", "StormExplosion", "diaochan");
            config[105006] = new HeroConfig(105006, "臧霸", 165, 227, 78, 75, 53, 56, 71, 7, 6, 5, 4, 5, 7, 7, 1, 351, false, 5, "长安", "北海", 90, "豪爽", new string[]{"忠义","果敢"}, new string[]{"结交"}, "无", null, null, null, 1, new int[0], "虐", "", "atk", "SwordHitYellowCritical", "zangba");
            config[105007] = new HeroConfig(105007, "高顺", 158, 198, 85, 86, 54, 45, 68, 8, 6, 5, 4, 8, 8, 8, 1, 362, false, 5, "长安", "晋阳", 97, "冷静", new string[]{"忠义","坚韧"}, new string[]{"练兵"}, "无", new string[]{"5;5"}, null, null, 3, new int[0], "", "", "shoot", "GasShootFire", "gaoshun");
            config[105008] = new HeroConfig(105008, "李儒", 164, 192, 63, 43, 91, 78, 38, 4, 3, 3, 2, 6, 4, 4, 1, 329, true, 5, "洛阳", "长安", 94, "多疑", new string[]{"智识","狡诈"}, new string[]{"毒计"}, "无", null, null, new string[]{"献计迁都"}, 3, new int[0], "火", "", "inte", "ShadowExplosion", "liru");
            config[105009] = new HeroConfig(105009, "李傕", 160, 198, 69, 74, 24, 1, 17, 7, 6, 4, 3, 4, 7, 7, 1, 201, false, 5, "安定", "安定", 87, "暴躁", new string[]{"果敢"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "劫", "", "atk", "SwordHitYellowCritical", "lijue");
            config[105010] = new HeroConfig(105010, "郭汜", 160, 197, 64, 76, 13, 14, 13, 7, 6, 4, 3, 4, 7, 7, 1, 196, false, 5, "安定", "武威", 86, "暴躁", new string[]{"果敢"}, new string[]{"饮酒"}, "无", null, null, null, 1, new int[0], "劫", "", "atk", "SwordHitYellowCritical", "guosi");
            config[105011] = new HeroConfig(105011, "陈宫", 154, 199, 84, 55, 89, 82, 70, 5, 3, 3, 3, 7, 5, 5, 1, 398, false, 5, "洛阳", "濮阳", 93, "刚直", new string[]{"忠义","智识"}, new string[]{"围棋"}, "无", new string[]{"5;5"}, new string[]{"2;3"}, new string[]{"弃官随曹操后叛","辅吕布"}, 3, new int[0], "", "溃", "inte", "ShadowExplosion", "chengong");
            config[105012] = new HeroConfig(105012, "胡车儿", 172, 198, 25, 80, 40, 1, 29, 7, 5, 4, 3, 4, 7, 7, 1, 190, false, 5, "安定", "北海", 88, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"盗戟杀典韦"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "hucheer");
            config[105013] = new HeroConfig(105013, "魏续", 170, 198, 67, 78, 31, 32, 39, 7, 6, 4, 3, 4, 7, 7, 1, 261, false, 5, "洛阳", "平原", 82, "多疑", new string[]{"怯懦"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "weixu");
            config[105014] = new HeroConfig(105014, "宋宪", 171, 198, 68, 77, 38, 27, 31, 7, 6, 4, 3, 4, 7, 7, 1, 255, false, 5, "洛阳", "南皮", 83, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "songxian");
            config[105015] = new HeroConfig(105015, "徐荣", 158, 192, 80, 76, 57, 43, 42, 7, 6, 5, 4, 6, 7, 7, 1, 318, false, 5, "洛阳", "晋阳", 88, "冷静", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"荥阳败曹操"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xurong");
            config[105016] = new HeroConfig(105016, "侯成", 170, 198, 74, 75, 63, 56, 60, 7, 6, 4, 3, 4, 7, 7, 1, 342, false, 5, "洛阳", "安定", 84, "多疑", new string[]{"怯懦"}, new string[]{"饮酒"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "houcheng");
            config[105017] = new HeroConfig(105017, "胡轸", 168, 191, 65, 74, 12, 15, 21, 6, 5, 4, 3, 3, 6, 6, 1, 201, false, 5, "洛阳", "天水", 83, "急躁", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "huzhen");
            config[105018] = new HeroConfig(105018, "张绣", 167, 207, 80, 73, 60, 45, 59, 8, 7, 5, 4, 5, 9, 8, 1, 338, false, 5, "安定", "武威", 89, "隐忍", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"宛城反叛"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangxiu");
            config[105019] = new HeroConfig(105019, "樊稠", 165, 197, 66, 73, 31, 24, 39, 6, 5, 4, 3, 3, 6, 6, 1, 249, false, 5, "安定", "武陵", 84, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "fanchou");
            config[105020] = new HeroConfig(105020, "曹性", 170, 198, 53, 73, 37, 26, 38, 6, 4, 5, 3, 3, 6, 6, 1, 242, false, 5, "洛阳", "零陵", 85, "冷静", new string[]{"果敢"}, new string[]{"射箭"}, "无", null, null, new string[]{"射中夏侯惇"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "caoxing");
            config[105021] = new HeroConfig(105021, "李肃", 171, 192, 46, 69, 59, 15, 36, 5, 4, 3, 2, 3, 5, 5, 1, 237, false, 5, "洛阳", "江陵", 78, "多疑", new string[]{"狡诈"}, new string[]{"珍宝"}, "无", null, null, new string[]{"诱杀丁原"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lisu");
            config[105022] = new HeroConfig(105022, "张济", 176, 196, 69, 65, 51, 52, 54, 6, 5, 4, 3, 3, 6, 6, 1, 309, false, 5, "安定", "武威", 87, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangji");
            config[105023] = new HeroConfig(105023, "朱儁", 156, 195, 78, 65, 70, 74, 73, 7, 5, 5, 4, 5, 7, 7, 1, 378, false, 5, "洛阳", "桂阳", 90, "冷静", new string[]{"忠义","贤明"}, new string[]{"兵法"}, "无", new string[]{"5;5"}, null, new string[]{"平定黄巾"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhujun");
            config[105024] = new HeroConfig(105024, "杨奉", 172, 215, 66, 65, 31, 14, 58, 5, 4, 3, 2, 3, 5, 5, 1, 246, false, 5, "洛阳", "长沙", 80, "多疑", new string[]{"果敢"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yangfeng");
            config[105025] = new HeroConfig(105025, "牛辅", 165, 192, 38, 60, 21, 26, 37, 4, 3, 3, 2, 3, 4, 4, 1, 194, false, 5, "安定", "庐江", 75, "懦弱", new string[]{"庸常"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "niufu");
            config[105026] = new HeroConfig(105026, "董旻", 165, 192, 49, 60, 25, 12, 23, 4, 3, 3, 2, 3, 4, 4, 1, 181, false, 5, "长安", "会稽", 86, "暴躁", new string[]{"果敢"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "dongmin");
            config[105027] = new HeroConfig(105027, "皇甫嵩", 135, 195, 83, 58, 70, 48, 69, 7, 5, 5, 4, 6, 7, 7, 1, 349, false, 5, "安定", "柴桑", 91, "冷静", new string[]{"忠义","贤明"}, new string[]{"兵法"}, "无", null, null, new string[]{"平定黄巾"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "huangpusong");
            config[105028] = new HeroConfig(105028, "董承", 167, 200, 56, 53, 65, 63, 75, 5, 4, 3, 3, 3, 5, 5, 1, 325, false, 5, "长安", "永安", 82, "刚直", new string[]{"忠义"}, new string[]{"结交"}, "无", null, null, new string[]{"衣带诏"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "dongcheng");
            config[105029] = new HeroConfig(105029, "华歆", 157, 232, 18, 33, 82, 84, 17, 3, 2, 2, 2, 5, 3, 3, 1, 245, false, 5, "长安", "江州", 78, "多疑", new string[]{"雄才"}, new string[]{"书法"}, "无", null, null, new string[]{"逼禅称帝"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "huaxin");
            config[105030] = new HeroConfig(105030, "王允", 137, 192, 25, 5, 67, 83, 73, 3, 2, 2, 2, 4, 3, 3, 1, 264, false, 5, "洛阳", "建宁", 88, "隐忍", new string[]{"忠义"}, new string[]{"音律"}, "无", null, null, new string[]{"连环计"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangyun");
            config[106001] = new HeroConfig(106001, "马超", 176, 222, 92, 97, 42, 25, 80, 9, 10, 7, 5, 7, 9, 7, 1, 361, true, 6, "武威", "武威", 98, "急躁", new string[]{"果敢"}, new string[]{"武艺","骑射"}, "无", new string[]{"6;5"}, new string[]{"2;3","6;5"}, new string[]{"渭水之战"}, 1, new int[0], "", "", "atk", "SwordHitWhiteCritical", "machao");
            config[106002] = new HeroConfig(106002, "马岱", 183, 235, 80, 85, 54, 52, 71, 8, 9, 5, 4, 6, 8, 8, 1, 362, false, 6, "武威", "武威", 96, "冷静", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", new string[]{"6;5"}, new string[]{"7;3"}, new string[]{"斩魏延"}, 1, new int[0], "坚", "羽", "atk", "SwordHitYellowCritical", "madai");
            config[106003] = new HeroConfig(106003, "庞德", 176, 219, 89, 94, 67, 43, 67, 9, 9, 7, 5, 7, 9, 7, 1, 383, true, 6, "武威", "天水", 97, "刚猛", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", new string[]{"6;5"}, new string[]{"1;3"}, new string[]{"抬棺战关羽"}, 1, new int[0], "坚", "", "atk", "SwordHitYellowCritical", "pangde");
            config[106004] = new HeroConfig(106004, "成公英", 170, 220, 70, 68, 76, 60, 65, 6, 6, 5, 4, 5, 6, 6, 1, 357, false, 6, "天水", "武威", 92, "冷静", new string[]{"忠义","智识"}, new string[]{"骑射"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenggongying");
            config[106005] = new HeroConfig(106005, "成宜", 175, 211, 72, 70, 40, 47, 54, 6, 6, 4, 3, 4, 6, 6, 1, 301, false, 6, "天水", "寿春", 86, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chengyi");
            config[106006] = new HeroConfig(106006, "侯选", 170, 211, 59, 62, 32, 52, 49, 5, 6, 3, 2, 3, 5, 5, 1, 269, false, 6, "天水", "北平", 85, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "houxuan");
            config[106007] = new HeroConfig(106007, "马休", 178, 212, 63, 67, 44, 41, 63, 6, 8, 4, 3, 4, 6, 6, 1, 296, false, 6, "武威", "蓟", 88, "急躁", new string[]{"果敢"}, new string[]{"骑射"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "maxiu");
            config[106008] = new HeroConfig(106008, "马玩", 175, 211, 68, 71, 15, 22, 35, 6, 8, 4, 3, 4, 6, 6, 1, 226, false, 6, "天水", "蓟", 86, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "mawan");
            config[106009] = new HeroConfig(106009, "马铁", 179, 212, 65, 57, 52, 48, 57, 6, 6, 4, 3, 4, 6, 6, 1, 297, false, 6, "武威", "蓟", 87, "急躁", new string[]{"果敢"}, new string[]{"骑射"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "matie");
            config[106010] = new HeroConfig(106010, "梁兴", 172, 211, 59, 63, 18, 21, 25, 5, 5, 3, 2, 3, 5, 5, 1, 201, false, 6, "天水", "长安", 84, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liangxing");
            config[106011] = new HeroConfig(106011, "程银", 170, 211, 67, 71, 39, 35, 49, 6, 6, 4, 3, 4, 6, 6, 1, 276, false, 6, "天水", "许昌", 85, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenying");
            config[106012] = new HeroConfig(106012, "杨秋", 175, 220, 64, 61, 55, 61, 40, 5, 5, 4, 3, 3, 5, 5, 1, 298, false, 6, "天水", "邺", 84, "多疑", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yangqiu");
            config[106013] = new HeroConfig(106013, "阎行", 175, 215, 72, 84, 61, 58, 69, 8, 7, 5, 4, 5, 8, 8, 1, 365, false, 6, "天水", "天水", 91, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, new string[]{"6;5"}, new string[]{"与马超单挑"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yanxing");
            config[106014] = new HeroConfig(106014, "韩遂", 158, 215, 84, 75, 77, 61, 80, 7, 9, 5, 4, 6, 7, 7, 1, 398, false, 6, "天水", "天水", 93, "多疑", new string[]{"雄才","狡诈"}, new string[]{"权谋"}, "无", new string[]{"6;5"}, new string[]{"6;5"}, new string[]{"与马超反目"}, 1, new int[0], "乱", "", "def", "SwordHitYellowCritical", "hansui");
            config[107001] = new HeroConfig(107001, "魏延", 175, 234, 84, 91, 68, 49, 51, 9, 7, 7, 5, 7, 9, 9, 1, 365, true, 7, "武陵", "襄阳", 87, "刚愎", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"献长沙","汉中太守"}, 1, new int[0], "破", "乱", "atk", "SwordHitYellowCritical", "weiyan");
            config[107002] = new HeroConfig(107002, "黄忠", 145, 220, 88, 93, 64, 52, 75, 9, 7, 9, 5, 7, 9, 9, 1, 399, true, 7, "武陵", "宛", 97, "刚猛", new string[]{"忠义","果敢"}, new string[]{"射箭","舞刀"}, "无", null, null, new string[]{"定军山斩夏侯渊"}, 3, new int[0], "矢", "", "shoot", "BulletExplosionFire", "huangzhong");
            config[107003] = new HeroConfig(107003, "王威", 165, 208, 60, 70, 59, 52, 66, 6, 5, 5, 5, 4, 6, 6, 1, 326, false, 7, "江夏", "宛", 91, "冷静", new string[]{"忠义"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangwei");
            config[107004] = new HeroConfig(107004, "王粲", 177, 217, 5, 2, 79, 81, 52, 2, 4, 2, 2, 4, 2, 2, 1, 228, false, 7, "江夏", "陈留", 89, "豪爽", new string[]{"才思"}, new string[]{"读书","赋诗"}, "无", null, null, new string[]{"登楼赋"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangcan");
            config[107005] = new HeroConfig(107005, "黄祖", 158, 208, 73, 65, 52, 37, 31, 7, 5, 5, 6, 5, 7, 7, 1, 280, false, 7, "江夏", "江夏", 84, "急躁", new string[]{"刚愎"}, new string[]{"射猎"}, "无", null, null, new string[]{"射杀孙坚"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "huangzu");
            config[107006] = new HeroConfig(107006, "韩嵩", 159, 215, 25, 15, 70, 78, 61, 3, 2, 2, 4, 4, 3, 3, 1, 262, false, 7, "襄阳", "襄阳", 87, "谦和", new string[]{"忠义"}, new string[]{"读书"}, "无", null, null, new string[]{"劝刘表降曹"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "hansong");
            config[107007] = new HeroConfig(107007, "苏飞", 162, 209, 66, 60, 63, 59, 60, 5, 4, 4, 6, 4, 5, 5, 1, 326, false, 7, "零陵", "小沛", 90, "豪爽", new string[]{"忠义"}, new string[]{"结交"}, "无", null, null, new string[]{"救甘宁"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "sufei");
            config[107008] = new HeroConfig(107008, "吴巨", 165, 211, 49, 61, 23, 51, 54, 5, 4, 3, 4, 3, 5, 5, 1, 253, false, 7, "江陵", "北海", 82, "豪爽", new string[]{"结交"}, new string[]{"结交"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wuju");
            config[107009] = new HeroConfig(107009, "刘磐", 168, 208, 67, 74, 46, 42, 53, 6, 5, 4, 4, 4, 6, 6, 1, 300, false, 7, "江陵", "平原", 90, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liupan");
            config[107010] = new HeroConfig(107010, "伊籍", 168, 223, 29, 24, 80, 86, 84, 3, 2, 2, 3, 5, 3, 3, 1, 315, false, 7, "零陵", "襄阳", 91, "谦和", new string[]{"能言"}, new string[]{"外交"}, "无", null, null, new string[]{"投刘备"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yiji");
            config[107011] = new HeroConfig(107011, "张允", 168, 210, 72, 67, 42, 56, 48, 6, 5, 5, 8, 5, 6, 6, 1, 307, false, 7, "襄阳", "晋阳", 80, "多疑", new string[]{"怯懦"}, new string[]{"水战"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangyun");
            config[107012] = new HeroConfig(107012, "蒯良", 162, 203, 68, 33, 88, 83, 71, 4, 2, 3, 5, 6, 4, 4, 1, 360, false, 7, "襄阳", "襄阳", 93, "冷静", new string[]{"智识"}, new string[]{"兵法","地理"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "kuailiang");
            config[107013] = new HeroConfig(107013, "蒯越", 163, 214, 47, 27, 82, 89, 73, 3, 2, 3, 5, 5, 3, 3, 1, 334, false, 7, "襄阳", "襄阳", 93, "冷静", new string[]{"智识"}, new string[]{"谋略"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "kuaiyue");
            config[107015] = new HeroConfig(107015, "刘琦", 171, 209, 49, 11, 58, 68, 69, 3, 4, 2, 3, 3, 3, 3, 1, 267, false, 7, "襄阳", "武威", 86, "隐忍", new string[]{"孝道"}, new string[]{"读书"}, "无", null, null, new string[]{"求计诸葛亮"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liuqi");
            config[107016] = new HeroConfig(107016, "蔡中", 170, 208, 39, 52, 1, 21, 42, 4, 3, 3, 5, 3, 4, 4, 1, 169, false, 7, "襄阳", "武陵", 78, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "caizhong");
            config[107017] = new HeroConfig(107017, "蔡氏", 165, 208, 8, 7, 69, 58, 66, 1, 1, 1, 3, 2, 1, 1, 1, 215, false, 7, "襄阳", "零陵", 80, "多疑", new string[]{"狡诈"}, new string[]{"华服"}, "无", null, null, new string[]{"排挤刘琦"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "nvcaishi");
            config[107018] = new HeroConfig(107018, "蔡和", 172, 208, 38, 49, 1, 25, 44, 4, 3, 3, 5, 3, 4, 4, 1, 171, false, 7, "襄阳", "江陵", 77, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "caihe");
            config[107019] = new HeroConfig(107019, "蔡瑁", 155, 208, 77, 70, 77, 72, 62, 6, 5, 5, 9, 6, 6, 6, 1, 383, false, 7, "襄阳", "襄阳", 82, "多疑", new string[]{"狡诈"}, new string[]{"水战"}, "无", new string[]{"7;5"}, new string[]{"1;3"}, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "caimao");
            config[108001] = new HeroConfig(108001, "严颜", 158, 214, 79, 83, 69, 64, 79, 8, 7, 8, 4, 6, 8, 8, 1, 394, false, 8, "建宁", "长沙", 95, "刚猛", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", new string[]{"1;3"}, null, new string[]{"宁死不屈"}, 1, new int[0], "敏", "", "def", "SwordHitYellowCritical", "yanyan");
            config[108002] = new HeroConfig(108002, "李严", 171, 234, 82, 83, 72, 71, 50, 8, 7, 8, 4, 6, 8, 8, 1, 378, false, 8, "建宁", "庐江", 92, "刚愎", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "实", "", "def", "SwordHitYellowCritical", "liyan");
            config[108003] = new HeroConfig(108003, "张松", 165, 212, 15, 6, 88, 83, 19, 2, 1, 2, 1, 6, 2, 2, 1, 220, false, 8, "成都", "成都", 84, "多疑", new string[]{"才思"}, new string[]{"读书","地理"}, "无", null, null, new string[]{"献西川地图"}, 3, new int[0], "", "", "inte", "FanExplosion", "zhangsong");
            config[108004] = new HeroConfig(108004, "董允", 185, 246, 67, 65, 78, 94, 79, 3, 2, 2, 2, 5, 3, 3, 1, 394, true, 8, "江州", "柴桑", 90, "谦和", new string[]{"贤明","勤勉"}, new string[]{"读书"}, "无", null, null, null, 3, new int[0], "", "", "inte", "FanExplosion", "dongyun");
            config[108005] = new HeroConfig(108005, "孟获", 175, 225, 87, 87, 51, 55, 75, 8, 7, 5, 4, 6, 8, 8, 9, 372, false, 8, "云南", "云南", 94, "豪爽", new string[]{"果敢"}, new string[]{"饮酒","武艺"}, "无", new string[]{"8;5"}, null, new string[]{"七擒七纵"}, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "menghuo");
            config[108006] = new HeroConfig(108006, "祝融", 180, 225, 77, 85, 43, 50, 78, 8, 7, 5, 4, 5, 8, 8, 8, 349, false, 8, "云南", "云南", 95, "急躁", new string[]{"果敢"}, new string[]{"飞刀","武艺"}, "无", null, null, new string[]{"擒张嶷"}, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "zhurong");
            config[108007] = new HeroConfig(108007, "法正", 176, 220, 83, 52, 94, 79, 55, 5, 3, 3, 3, 7, 5, 5, 1, 381, true, 8, "江州", "长安", 93, "多疑", new string[]{"雄才","果敢"}, new string[]{"谋略"}, "无", null, null, new string[]{"定军山献策"}, 3, new int[0], "溃", "", "inte", "GasExplosionFire", "fazheng");
            config[108008] = new HeroConfig(108008, "黄权", 171, 240, 75, 59, 82, 81, 78, 6, 4, 4, 3, 6, 6, 6, 1, 394, false, 8, "建宁", "云南", 91, "冷静", new string[]{"忠义","智识"}, new string[]{"兵法"}, "无", null, null, new string[]{"夷陵降魏"}, 1, new int[0], "缓", "", "atk", "SwordHitYellowCritical", "huangquan");
            config[108009] = new HeroConfig(108009, "孟达", 170, 228, 75, 73, 74, 67, 72, 5, 4, 4, 3, 4, 5, 5, 1, 377, false, 8, "江州", "梓潼", 86, "多疑", new string[]{"反复"}, new string[]{"书法"}, "无", null, null, new string[]{"反复常"}, 3, new int[0], "乱", "", "shoot", "BulletExplosionBlue", "mengda");
            config[108010] = new HeroConfig(108010, "李恢", 174, 231, 79, 65, 79, 81, 78, 5, 3, 3, 3, 5, 5, 5, 1, 398, false, 8, "建宁", "上庸", 90, "冷静", new string[]{"能言"}, new string[]{"外交"}, "无", null, null, new string[]{"说降马超"}, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "lihui");
            config[108011] = new HeroConfig(108011, "张任", 169, 214, 88, 84, 78, 59, 76, 8, 7, 9, 4, 6, 8, 8, 1, 406, false, 8, "建宁", "汉中", 96, "刚猛", new string[]{"忠义","果敢"}, new string[]{"武艺","射箭"}, "无", null, new string[]{"1;3"}, new string[]{"落凤坡射庞统"}, 3, new int[0], "复", "", "shoot", "BulletExplosionBlue", "zhangren");
            config[108012] = new HeroConfig(108012, "雷铜", 165, 218, 69, 78, 51, 37, 53, 7, 6, 4, 3, 4, 7, 7, 1, 302, false, 8, "梓潼", "汝南", 88, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "leitong");
            config[108013] = new HeroConfig(108013, "高沛", 168, 214, 66, 61, 69, 57, 52, 5, 4, 3, 3, 3, 5, 5, 1, 318, false, 8, "梓潼", "寿春", 86, "急躁", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gaopei");
            config[108014] = new HeroConfig(108014, "杨怀", 166, 212, 62, 68, 68, 62, 53, 6, 5, 4, 3, 4, 6, 6, 1, 326, false, 8, "成都", "北平", 87, "急躁", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yanghuai");
            config[108015] = new HeroConfig(108015, "吴兰", 168, 218, 62, 80, 35, 36, 50, 7, 6, 4, 3, 4, 7, 7, 1, 277, false, 8, "梓潼", "蓟", 86, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wulan");
            config[108016] = new HeroConfig(108016, "庞羲", 165, 214, 58, 36, 65, 71, 55, 3, 2, 2, 2, 4, 3, 3, 1, 295, false, 8, "梓潼", "襄平", 84, "谦和", new string[]{"贤明"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "pangyi");
            config[108017] = new HeroConfig(108017, "王甫", 173, 222, 62, 41, 79, 79, 73, 4, 2, 2, 2, 5, 4, 4, 1, 345, false, 8, "江州", "洛阳", 90, "冷静", new string[]{"忠义"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangfu");
            config[108018] = new HeroConfig(108018, "董和", 162, 220, 57, 34, 74, 88, 76, 3, 2, 2, 2, 5, 3, 3, 1, 340, false, 8, "江州", "长安", 91, "谦和", new string[]{"贤明","清廉"}, new string[]{"治理"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "donghe");
            config[108019] = new HeroConfig(108019, "吴懿", 166, 237, 83, 73, 68, 70, 77, 7, 6, 5, 4, 5, 7, 7, 1, 389, false, 8, "成都", "许昌", 90, "冷静", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wuyi");
            config[108020] = new HeroConfig(108020, "吴班", 170, 231, 74, 71, 56, 45, 66, 6, 5, 4, 3, 4, 6, 6, 1, 327, false, 8, "成都", "邺", 88, "豪爽", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wuban");
            config[108021] = new HeroConfig(108021, "冷苞", 168, 214, 71, 82, 68, 37, 23, 7, 6, 5, 3, 5, 7, 7, 1, 295, false, 8, "建宁", "襄阳", 84, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lengbao");
            config[108022] = new HeroConfig(108022, "刘璝", 166, 214, 71, 73, 66, 44, 62, 6, 5, 4, 3, 4, 6, 6, 1, 329, false, 8, "永安", "襄阳", 85, "冷静", new string[]{"忠义"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liukui");
            config[108023] = new HeroConfig(108023, "刘循", 178, 220, 61, 44, 39, 48, 55, 5, 4, 3, 3, 3, 5, 5, 1, 258, false, 8, "永安", "建业", 82, "隐忍", new string[]{"忠义"}, new string[]{"守城"}, "无", null, null, new string[]{"坚守雒城"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liuxun");
            config[108024] = new HeroConfig(108024, "王累", 168, 214, 28, 30, 78, 81, 73, 3, 2, 2, 2, 5, 3, 3, 1, 300, false, 8, "成都", "吴", 88, "刚直", new string[]{"忠义"}, new string[]{"进谏"}, "无", null, null, new string[]{"倒悬谏刘璋"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wanglei");
            config[108025] = new HeroConfig(108025, "秦宓", 174, 226, 15, 6, 71, 77, 75, 2, 1, 2, 2, 4, 2, 2, 1, 252, false, 8, "成都", "新野", 90, "豪爽", new string[]{"能言"}, new string[]{"清谈","读书"}, "无", null, null, new string[]{"天辩"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "qinse");
            config[108026] = new HeroConfig(108026, "费诗", 170, 250, 15, 28, 64, 75, 66, 3, 2, 2, 2, 4, 3, 3, 1, 258, false, 8, "成都", "宛", 89, "刚直", new string[]{"忠义"}, new string[]{"外交"}, "无", null, null, new string[]{"劝关羽受封"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "feishi");
            config[108027] = new HeroConfig(108027, "许靖", 155, 222, 2, 4, 64, 77, 65, 2, 1, 2, 2, 4, 2, 2, 1, 220, false, 8, "成都", "陈留", 82, "谦和", new string[]{"贤明"}, new string[]{"清谈"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "xujing");
            config[109001] = new HeroConfig(109001, "张卫", 170, 215, 71, 63, 43, 42, 58, 6, 5, 4, 3, 4, 6, 6, 1, 290, false, 9, "汉中", "下邳", 88, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangwei");
            config[109002] = new HeroConfig(109002, "杨任", 170, 215, 67, 75, 51, 38, 54, 6, 5, 4, 3, 4, 6, 6, 1, 298, false, 9, "上庸", "小沛", 86, "刚猛", new string[]{"忠义"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yangren");
            config[109003] = new HeroConfig(109003, "杨昂", 172, 215, 65, 69, 36, 33, 40, 5, 4, 3, 3, 3, 5, 5, 1, 256, false, 9, "汉中", "北海", 85, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yangang2");
            config[109004] = new HeroConfig(109004, "杨松", 168, 215, 1, 4, 27, 34, 4, 1, 1, 1, 1, 1, 1, 1, 1, 75, false, 9, "上庸", "平原", 70, "贪婪", new string[]{"狡诈"}, new string[]{"珍宝"}, "无", null, null, new string[]{"谗害马超"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yangsong");
            config[109005] = new HeroConfig(109005, "杨柏", 170, 215, 42, 43, 18, 25, 20, 3, 2, 2, 2, 2, 3, 3, 1, 157, false, 9, "上庸", "南皮", 73, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yangbai");
            config[109006] = new HeroConfig(109006, "阎圃", 170, 215, 29, 25, 82, 80, 70, 3, 2, 2, 2, 5, 3, 3, 1, 297, false, 9, "上庸", "晋阳", 91, "冷静", new string[]{"忠义","智识"}, new string[]{"谋略"}, "无", null, null, new string[]{"劝张鲁降曹"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yanpu");
            config[110001] = new HeroConfig(110001, "李丰", 165, 199, 69, 74, 50, 22, 47, 6, 5, 4, 3, 4, 6, 6, 1, 277, false, 10, "寿春", "天水", 83, "谦和", new string[]{"忠义"}, new string[]{"读书"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lifeng");
            config[110002] = new HeroConfig(110002, "纪灵", 166, 199, 78, 83, 51, 48, 55, 8, 7, 5, 4, 6, 8, 8, 1, 334, false, 10, "寿春", "武威", 93, "刚猛", new string[]{"果敢"}, new string[]{"三尖刀"}, "无", null, null, new string[]{"与关羽战平"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "jiling");
            config[110003] = new HeroConfig(110003, "袁胤", 168, 199, 17, 14, 39, 43, 46, 2, 1, 3, 2, 3, 2, 2, 1, 165, false, 10, "寿春", "武陵", 82, "多疑", new string[]{"怯懦"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yuanyin");
            config[110004] = new HeroConfig(110004, "袁涣", 170, 215, 30, 17, 68, 79, 83, 3, 2, 2, 2, 4, 3, 3, 1, 287, false, 10, "寿春", "零陵", 88, "谦和", new string[]{"仁德","贤明"}, new string[]{"读书"}, "无", null, null, new string[]{"不为吕布写书"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yuanhuan");
            config[110005] = new HeroConfig(110005, "袁燿", 170, 230, 38, 47, 38, 48, 49, 2, 1, 1, 1, 1, 2, 2, 1, 226, false, 10, "寿春", "江陵", 80, "多疑", new string[]{"庸常"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yuanyao");
            config[110006] = new HeroConfig(110006, "张勋", 166, 199, 72, 68, 41, 39, 59, 6, 5, 4, 3, 4, 6, 6, 1, 295, false, 10, "汝南", "江夏", 84, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zhangxun");
            config[110007] = new HeroConfig(110007, "梁纲", 165, 199, 60, 69, 41, 22, 46, 5, 4, 3, 3, 3, 5, 5, 1, 251, false, 10, "寿春", "桂阳", 81, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lianggang");
            config[110008] = new HeroConfig(110008, "陈纪", 164, 199, 58, 65, 43, 48, 32, 5, 4, 3, 3, 3, 5, 5, 1, 258, false, 10, "汝南", "长沙", 82, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenji");
            config[110009] = new HeroConfig(110009, "陈兰", 167, 199, 66, 69, 40, 24, 38, 5, 4, 3, 3, 3, 5, 5, 1, 250, false, 10, "寿春", "庐江", 80, "多疑", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "chenlan");
            config[110010] = new HeroConfig(110010, "杨弘", 165, 200, 18, 15, 76, 62, 45, 2, 1, 2, 2, 5, 2, 2, 1, 224, false, 10, "寿春", "会稽", 84, "多疑", new string[]{"智识"}, new string[]{"谋略"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yanghong");
            config[110011] = new HeroConfig(110011, "雷薄", 168, 200, 62, 70, 36, 11, 15, 6, 5, 4, 3, 4, 6, 6, 1, 207, false, 10, "寿春", "柴桑", 79, "多疑", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "leibo");
            config[110012] = new HeroConfig(110012, "刘勋", 165, 200, 47, 63, 35, 16, 32, 5, 4, 3, 3, 3, 5, 5, 1, 205, false, 10, "寿春", "永安", 83, "多疑", new string[]{"果敢"}, new string[]{"珍宝"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "liuxun2");
            config[110013] = new HeroConfig(110013, "乐就", 166, 199, 53, 66, 58, 42, 53, 5, 4, 3, 3, 3, 5, 5, 1, 285, false, 10, "汝南", "江州", 82, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "lejiu");
            config[110014] = new HeroConfig(110014, "桥蕤", 164, 199, 62, 67, 37, 40, 56, 5, 4, 3, 3, 3, 5, 5, 1, 275, false, 10, "寿春", "建宁", 81, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "qiaorui");
            config[110015] = new HeroConfig(110015, "阎象", 166, 199, 30, 27, 70, 75, 51, 3, 2, 2, 2, 6, 3, 3, 1, 263, false, 10, "寿春", "寿春", 88, "冷静", new string[]{"忠义","智识"}, new string[]{"读书"}, "无", null, null, new string[]{"谏袁术称帝"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "yanxiang");
            config[110016] = new HeroConfig(110016, "韩胤", 168, 199, 26, 29, 64, 55, 44, 2, 3, 2, 2, 4, 2, 2, 1, 226, false, 10, "汝南", "梓潼", 78, "多疑", new string[]{"能言"}, new string[]{"外交"}, "无", null, null, new string[]{"出使吕布"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "hanyin");
            config[110017] = new HeroConfig(110017, "韩浩", 170, 215, 69, 72, 68, 88, 62, 6, 5, 4, 3, 4, 6, 6, 1, 375, false, 10, "寿春", "上庸", 90, "冷静", new string[]{"忠义","勤勉"}, new string[]{"屯田"}, "无", new string[]{"2;3"}, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "hanhao");
            config[111001] = new HeroConfig(111001, "公孙范", 165, 195, 73, 69, 64, 62, 61, 6, 5, 4, 3, 4, 6, 6, 1, 347, false, 11, "北平", "汝南", 100, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gongsunfan");
            config[111002] = new HeroConfig(111002, "公孙续", 170, 199, 59, 63, 50, 59, 62, 5, 4, 3, 3, 3, 5, 5, 1, 309, false, 11, "北平", "寿春", 100, "多疑", new string[]{"怯懦"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gongsunxu");
            config[111003] = new HeroConfig(111003, "王门", 168, 190, 65, 64, 31, 41, 49, 5, 4, 3, 3, 3, 5, 5, 1, 264, false, 11, "蓟", "北平", 90, "多疑", new string[]{"反复"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "wangmen");
            config[111004] = new HeroConfig(111004, "田豫", 171, 252, 77, 69, 77, 75, 72, 7, 6, 5, 4, 6, 7, 7, 1, 391, false, 11, "蓟", "蓟", 92, "冷静", new string[]{"忠义","果敢"}, new string[]{"骑射"}, "无", new string[]{"1;3","2;3"}, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "tianyu");
            config[111005] = new HeroConfig(111005, "田楷", 165, 204, 68, 65, 56, 61, 63, 5, 4, 3, 3, 3, 5, 5, 1, 329, false, 11, "蓟", "襄平", 95, "冷静", new string[]{"忠义"}, new string[]{"书法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "tiankai");
            config[111006] = new HeroConfig(111006, "单经", 166, 199, 71, 68, 43, 49, 54, 6, 5, 4, 3, 4, 6, 6, 1, 301, false, 11, "北平", "洛阳", 94, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "shanjing");
            config[111007] = new HeroConfig(111007, "邹丹", 165, 195, 60, 63, 33, 36, 38, 5, 4, 3, 3, 3, 5, 5, 1, 244, false, 11, "北平", "长安", 83, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "zoudan");
            config[111008] = new HeroConfig(111008, "关靖", 160, 199, 36, 52, 72, 65, 42, 3, 2, 2, 2, 4, 3, 3, 1, 277, false, 11, "北平", "许昌", 87, "刚直", new string[]{"忠义"}, new string[]{"读书"}, "无", null, null, new string[]{"随公孙瓒自焚"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "guanjing");
            config[112001] = new HeroConfig(112001, "公孙恭", 170, 230, 37, 16, 64, 57, 39, 3, 2, 2, 2, 4, 3, 3, 1, 223, false, 12, "襄平", "襄阳", 100, "懦弱", new string[]{"庸常"}, new string[]{"无"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gongsungong");
            config[112002] = new HeroConfig(112002, "公孙康", 170, 221, 69, 63, 58, 55, 53, 5, 4, 3, 3, 4, 5, 5, 1, 315, false, 12, "襄平", "建业", 100, "多疑", new string[]{"果敢"}, new string[]{"射猎"}, "无", null, null, new string[]{"杀袁尚袁熙"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "gongsunkang");
            config[120001] = new HeroConfig(120001, "邓艾", 197, 264, 94, 87, 89, 88, 75, 9, 9, 7, 5, 7, 9, 9, 1, 460, true, 99, "", "新野", 0, "隐忍", new string[]{"坚韧","果敢"}, new string[]{"兵法","地理"}, "无", null, null, new string[]{"阴平渡险"}, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "dengai");
            config[120002] = new HeroConfig(120002, "司马师", 208, 255, 80, 67, 87, 82, 70, 6, 4, 3, 3, 6, 6, 6, 1, 406, false, 99, "", "宛", 0, "冷静", new string[]{"雄才","果敢"}, new string[]{"权谋"}, "无", null, null, new string[]{"废曹芳"}, 3, new int[0], "", "", "help", "SharpExplosionGreen", "simashi");
            config[120003] = new HeroConfig(120003, "司马昭", 211, 265, 78, 57, 87, 84, 65, 6, 4, 3, 3, 6, 6, 6, 1, 391, false, 99, "", "陈留", 0, "隐忍", new string[]{"雄才","狡诈"}, new string[]{"权谋"}, "无", null, null, new string[]{"杀曹髦"}, 3, new int[0], "溃", "", "help", "SharpExplosionGreen", "simazhao");
            config[120004] = new HeroConfig(120004, "羊祜", 221, 278, 90, 64, 84, 90, 92, 7, 6, 5, 4, 6, 7, 7, 1, 443, true, 99, "", "濮阳", 0, "谦和", new string[]{"仁德","贤明"}, new string[]{"读书","围棋"}, "无", null, null, new string[]{"与陆抗交好"}, 1, new int[0], "敏", "", "atk", "SwordHitYellowCritical", "yangku");
            config[120005] = new HeroConfig(120005, "钟会", 225, 264, 82, 58, 92, 85, 65, 5, 3, 3, 3, 7, 5, 5, 1, 401, true, 99, "", "下邳", 0, "多疑", new string[]{"雄才"}, new string[]{"书法","兵法"}, "无", null, null, new string[]{"灭蜀后谋反"}, 3, new int[0], "缓", "", "inte", "StormExplosion", "zhonghui");
            config[120006] = new HeroConfig(120006, "陈泰", 200, 260, 86, 77, 84, 72, 71, 7, 6, 5, 4, 6, 7, 7, 1, 409, false, 99, "", "小沛", 0, "冷静", new string[]{"果敢","贤明"}, new string[]{"兵法"}, "无", null, null, null, 1, new int[0], "虐", "", "def", "SwordHitYellowCritical", "chentai");
            config[120007] = new HeroConfig(120007, "杜预", 222, 285, 84, 30, 85, 89, 78, 5, 3, 3, 3, 6, 5, 5, 1, 385, false, 99, "", "北海", 0, "谦和", new string[]{"雄才","勤勉"}, new string[]{"读书","律法"}, "无", null, null, new string[]{"注左传"}, 3, new int[0], "", "", "inte", "SharpExplosionGreen", "duyu");
            config[120008] = new HeroConfig(120008, "王濬", 206, 286, 80, 52, 79, 82, 75, 6, 4, 3, 5, 5, 6, 6, 1, 390, false, 99, "", "平原", 0, "刚猛", new string[]{"果敢"}, new string[]{"造船"}, "无", null, null, new string[]{"楼船破吴"}, 1, new int[0], "敏", "", "atk", "SwordHitYellowCritical", "wangrui");
            config[120009] = new HeroConfig(120009, "辛宪英", 191, 269, 42, 28, 84, 80, 82, 2, 1, 2, 2, 5, 2, 2, 1, 325, false, 99, "", "南皮", 0, "冷静", new string[]{"智识","洞察"}, new string[]{"读书"}, "无", null, null, null, 3, new int[0], "缓", "", "inte", "FanExplosion", "xinxianying");
            config[199001] = new HeroConfig(199001, "孙权", 182, 252, 74, 65, 78, 87, 93, 7, 6, 5, 7, 6, 7, 7, 1, 428, true, 3, "吴", "吴", 100, "隐忍", new string[]{"雄才","果敢"}, new string[]{"射猎","读书"}, "无", new string[]{"3;5"}, new string[]{"2;3"}, new string[]{"赤壁之战","夷陵之战"}, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "sunquan");
            config[199002] = new HeroConfig(199002, "诸葛亮", 181, 234, 98, 45, 100, 99, 92, 7, 5, 4, 3, 9, 7, 7, 1, 462, true, 99, "", "襄阳", 0, "冷静", new string[]{"忠义","智识"}, new string[]{"读书","发明","音律"}, "无", new string[]{"1;3","11;3"}, null, new string[]{"三顾茅庐","草船借箭","七擒孟获"}, 3, new int[0], "神", "空", "inte", "LightningExplosionYellow", "zhugeliang");
            config[199003] = new HeroConfig(199003, "姜维", 202, 264, 92, 89, 90, 66, 79, 9, 9, 7, 5, 7, 9, 9, 1, 441, true, 99, "", "天水", 0, "冷静", new string[]{"忠义","果敢"}, new string[]{"武艺","兵法"}, "无", null, null, new string[]{"九伐中原"}, 1, new int[0], "解", "", "def", "SwordHitYellowCritical", "jiangwei");
            config[199004] = new HeroConfig(199004, "关兴", 199, 220, 80, 85, 60, 56, 73, 8, 7, 5, 4, 6, 8, 8, 1, 374, false, 99, "", "武威", 0, "急躁", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", new string[]{"1;3"}, null, new string[]{"斩潘璋"}, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "guanxing");
            config[199006] = new HeroConfig(199006, "庞统", 179, 214, 85, 47, 98, 86, 65, 5, 3, 3, 3, 7, 5, 5, 1, 399, true, 99, "", "襄阳", 0, "豪爽", new string[]{"智识","果敢"}, new string[]{"清谈","饮酒"}, "无", new string[]{"1;3"}, null, new string[]{"落凤坡"}, 3, new int[0], "锁", "火", "inte", "ExplosionFireballFire", "pangtong");
            config[199007] = new HeroConfig(199007, "张苞", 199, 231, 75, 87, 47, 45, 67, 8, 7, 5, 4, 5, 9, 8, 1, 340, false, 99, "", "零陵", 0, "急躁", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"为父报仇"}, 1, new int[0], "乱", "", "atk", "SwordHitYellowCritical", "zhangbao");
            config[199008] = new HeroConfig(199008, "关索", 200, 263, 74, 81, 50, 46, 72, 7, 6, 5, 4, 5, 7, 7, 1, 343, false, 99, "", "江陵", 0, "刚猛", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"花关索"}, 3, new int[0], "", "", "shoot", "BulletExplosionBlue", "guansuo");
            config[199009] = new HeroConfig(199009, "黄月英", 183, 234, 58, 34, 86, 85, 70, 3, 2, 3, 3, 6, 3, 3, 1, 351, false, 99, "", "江夏", 0, "冷静", new string[]{"智识","贤明"}, new string[]{"发明","农事"}, "无", null, null, new string[]{"木牛流马"}, 3, new int[0], "", "", "shoot", "GasShootFire", "huangyueying");
            config[199010] = new HeroConfig(199010, "刘禅", 207, 271, 12, 15, 27, 40, 52, 4, 1, 2, 1, 6, 4, 4, 1, 151, false, 99, "", "新野", 0, "懦弱", new string[]{"庸常"}, new string[]{"玩乐","美色"}, "无", new string[]{"1;3"}, null, new string[]{"乐不思蜀"}, 3, new int[0], "", "", "help", "SoulExplosionOrange", "liushan");
            config[199011] = new HeroConfig(199011, "刘巴", 170, 222, 33, 24, 78, 85, 65, 3, 2, 2, 2, 5, 3, 3, 1, 296, false, 99, "", "长沙", 0, "刚直", new string[]{"清廉","才思"}, new string[]{"算术","书法"}, "无", null, null, new string[]{"铸直百钱"}, 3, new int[0], "纷", "", "inte", "FanExplosion", "liuba");
            config[199012] = new HeroConfig(199012, "司马懿", 179, 251, 98, 63, 98, 93, 87, 7, 5, 4, 4, 7, 7, 7, 1, 465, true, 99, "", "庐江", 0, "隐忍", new string[]{"雄才","坚韧"}, new string[]{"兵法","权谋"}, "无", null, null, new string[]{"高平陵之变"}, 3, new int[0], "", "", "inte", "ShadowExplosion", "simayi");
            config[199013] = new HeroConfig(199013, "夏侯霸", 200, 259, 82, 77, 69, 53, 68, 7, 6, 5, 4, 5, 7, 7, 1, 371, false, 99, "", "会稽", 0, "急躁", new string[]{"忠义","果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"投蜀"}, 1, new int[0], "连", "", "atk", "SwordHitYellowCritical", "xiahouba");
            config[199014] = new HeroConfig(199014, "王双", 195, 231, 68, 88, 19, 22, 27, 7, 6, 5, 3, 4, 7, 7, 1, 241, false, 99, "", "柴桑", 0, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, null, 1, new int[0], "透", "", "atk", "SwordHitYellowCritical", "wangshuang");
            config[199015] = new HeroConfig(199015, "文鸯", 238, 291, 76, 91, 64, 65, 68, 9, 8, 7, 5, 6, 10, 9, 1, 389, true, 99, "", "永安", 0, "刚猛", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, null, new string[]{"单骑退雄兵"}, 3, new int[0], "", "", "shoot", "BulletExplosionBlue", "wenyuan");
            config[199017] = new HeroConfig(199017, "毌丘俭", 195, 255, 78, 75, 50, 54, 52, 7, 6, 5, 4, 5, 7, 7, 1, 328, false, 99, "", "江州", 0, "刚直", new string[]{"忠义","果敢"}, new string[]{"兵法"}, "无", null, null, new string[]{"淮南二叛"}, 1, new int[0], "劫", "", "atk", "SwordHitYellowCritical", "guanqiujian");
            config[199021] = new HeroConfig(199021, "孙坚", 155, 191, 93, 90, 77, 78, 92, 9, 9, 7, 5, 7, 9, 9, 1, 458, true, 99, "", "吴", 0, "豪爽", new string[]{"果敢","忠义"}, new string[]{"武艺"}, "无", new string[]{"3;4"}, new string[]{"7;4"}, new string[]{"讨董卓"}, 1, new int[0], "旋", "", "atk", "SwordHitYellowCritical", "sunjian");
            config[199022] = new HeroConfig(199022, "诸葛恪", 203, 253, 72, 47, 90, 80, 57, 4, 3, 3, 4, 6, 4, 4, 1, 364, true, 99, "", "北海", 0, "骄横", new string[]{"才思"}, new string[]{"围棋","辩论"}, "无", null, null, new string[]{"东兴大捷"}, 3, new int[0], "缓", "", "inte", "StormExplosion", "zhugege");
            config[199023] = new HeroConfig(199023, "华佗", 145, 208, 60, 34, 77, 70, 85, 2, 1, 2, 2, 4, 2, 2, 1, 334, false, 99, "", "梓潼", 0, "冷静", new string[]{"仁德","专注"}, new string[]{"医术","炼丹"}, "无", null, null, new string[]{"刮骨疗毒"}, 3, new int[0], "药", "", "help", "ShadowExplosionGreen", "huatuo");
            config[199024] = new HeroConfig(199024, "于吉", 164, 200, 47, 41, 73, 65, 70, 2, 1, 2, 2, 4, 2, 2, 1, 304, false, 99, "", "上庸", 0, "隐忍", new string[]{"仁德"}, new string[]{"炼丹","符水"}, "无", null, null, new string[]{"被孙策杀"}, 3, new int[0], "", "", "help", "ShadowExplosionGreen", "yuji");
            config[199025] = new HeroConfig(199025, "张角", 156, 184, 87, 29, 86, 82, 88, 5, 3, 3, 3, 6, 5, 5, 1, 390, false, 99, "", "汉中", 0, "豪爽", new string[]{"雄才"}, new string[]{"符水","道教"}, "无", null, null, new string[]{"黄巾起义"}, 3, new int[0], "天", "陷", "inte", "LightningExplosionBlue", "zhangjiao");
            config[199026] = new HeroConfig(199026, "张宝", 155, 184, 83, 71, 81, 78, 75, 7, 6, 5, 4, 5, 7, 7, 1, 406, false, 99, "", "汝南", 0, "急躁", new string[]{"果敢"}, new string[]{"妖术"}, "无", null, null, null, 1, new int[0], "劫", "", "atk", "SwordHitYellowCritical", "zhangbao2");
            config[199027] = new HeroConfig(199027, "张梁", 156, 184, 78, 80, 74, 75, 70, 7, 6, 5, 4, 5, 7, 7, 1, 394, false, 99, "", "寿春", 0, "急躁", new string[]{"果敢"}, new string[]{"妖术"}, "无", null, null, null, 3, new int[0], "", "", "def", "SwordHitYellowCritical", "zhangliang");
            config[199029] = new HeroConfig(199029, "王异", 170, 214, 73, 51, 82, 75, 78, 6, 5, 4, 4, 5, 6, 6, 1, 375, false, 99, "", "北平", 0, "冷静", new string[]{"忠义","坚韧"}, new string[]{"兵法","女红"}, "无", null, new string[]{"6;3"}, new string[]{"守冀城"}, 1, new int[0], "", "", "atk", "SwordHitYellowCritical", "wangyi");
            config[199030] = new HeroConfig(199030, "蔡琰", 177, 249, 61, 13, 77, 75, 82, 2, 4, 2, 2, 4, 2, 2, 1, 316, false, 99, "", "蓟", 0, "隐忍", new string[]{"才思"}, new string[]{"音律","读书"}, "无", null, null, new string[]{"文姬归汉"}, 3, new int[0], "", "", "help", "StormExplosion", "caiyan");
            config[199031] = new HeroConfig(199031, "马谡", 190, 228, 70, 72, 88, 70, 65, 4, 3, 3, 3, 5, 4, 4, 1, 381, false, 99, "", "襄阳", 0, "刚愎", new string[]{"才思"}, new string[]{"兵法","清谈"}, "无", null, null, new string[]{"失街亭"}, 3, new int[0], "百", "", "inte", "StormExplosion", "masu");
            config[199032] = new HeroConfig(199032, "马良", 187, 222, 68, 60, 93, 87, 86, 3, 2, 2, 2, 7, 3, 3, 1, 405, true, 99, "", "襄阳", 0, "谦和", new string[]{"贤明","仁德"}, new string[]{"书法","读书"}, "无", null, null, new string[]{"白眉最良"}, 3, new int[0], "", "", "help", "SharpExplosionGreen", "maliang");
            config[199033] = new HeroConfig(199033, "蒋琬", 193, 246, 64, 52, 85, 97, 81, 4, 3, 2, 3, 5, 4, 4, 1, 394, true, 99, "", "零陵", 0, "谦和", new string[]{"贤明","勤勉"}, new string[]{"治理"}, "无", null, null, new string[]{"继任丞相"}, 3, new int[0], "", "", "help", "SharpExplosionGreen", "jiangwan");
            config[199034] = new HeroConfig(199034, "费祎", 185, 253, 68, 42, 83, 95, 83, 4, 3, 2, 3, 5, 4, 4, 1, 386, true, 99, "", "江夏", 0, "谦和", new string[]{"贤明","能言"}, new string[]{"围棋"}, "无", null, null, null, 3, new int[0], "", "", "help", "SharpExplosionGreen", "feiyi");
            config[199035] = new HeroConfig(199035, "郭攸之", 180, 243, 63, 48, 82, 80, 75, 3, 2, 2, 2, 6, 3, 3, 1, 359, false, 99, "", "邺", 0, "谦和", new string[]{"贤明"}, new string[]{"读书"}, "无", null, null, null, 3, new int[0], "陷", "", "help", "SoulExplosionOrange", "guoyouzhi");
            config[199036] = new HeroConfig(199036, "邓芝", 178, 251, 70, 71, 80, 89, 87, 7, 6, 5, 4, 6, 7, 7, 1, 416, false, 99, "", "新野", 0, "刚直", new string[]{"忠义","能言"}, new string[]{"外交"}, "无", null, null, new string[]{"出使东吴"}, 1, new int[0], "", "", "def", "SwordHitYellowCritical", "dengzhi");
            config[199037] = new HeroConfig(199037, "王平", 175, 248, 83, 78, 75, 58, 51, 7, 6, 5, 4, 5, 7, 7, 1, 365, false, 99, "", "汉中", 0, "冷静", new string[]{"忠义","坚韧"}, new string[]{"书法"}, "无", null, null, new string[]{"街亭劝谏"}, 1, new int[0], "伏", "坚", "def", "SwordHitYellowCritical", "wangping");
            config[199038] = new HeroConfig(199038, "陆抗", 226, 274, 91, 63, 87, 88, 86, 8, 6, 5, 7, 6, 8, 8, 1, 443, true, 99, "", "建业", 0, "冷静", new string[]{"忠义","贤明"}, new string[]{"兵法"}, "无", null, null, new string[]{"西陵之战"}, 3, new int[0], "透", "", "shoot", "BulletExplosionBlue", "lukang");
            config[199039] = new HeroConfig(199039, "刘封", 192, 220, 75, 79, 44, 55, 50, 7, 6, 5, 4, 5, 7, 7, 1, 321, false, 99, "", "吴", 0, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", new string[]{"1;3"}, null, new string[]{"不救关羽被赐死"}, 3, new int[0], "", "", "shoot", "BulletExplosionBlue", "liufeng");
            config[199040] = new HeroConfig(199040, "孙桓", 198, 222, 82, 73, 76, 75, 76, 7, 6, 5, 5, 5, 7, 7, 1, 403, false, 99, "", "新野", 0, "急躁", new string[]{"果敢"}, new string[]{"武艺"}, "无", null, new string[]{"1;3"}, new string[]{"夷陵之战"}, 1, new int[0], "竟", "", "def", "SwordHitYellowCritical", "sunhuan");
            config[199041] = new HeroConfig(199041, "太史享", 190, 256, 53, 62, 45, 55, 56, 5, 4, 4, 4, 4, 5, 5, 1, 287, false, 99, "", "宛", 0, "冷静", new string[]{"忠义"}, new string[]{"书法"}, "无", null, null, null, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "taishixiang");
            config[199042] = new HeroConfig(199042, "全琮", 198, 249, 75, 69, 68, 59, 64, 6, 5, 4, 5, 5, 6, 6, 1, 356, false, 99, "", "陈留", 0, "谦和", new string[]{"果敢","贤明"}, new string[]{"结交"}, "无", null, null, new string[]{"芍陂之战"}, 1, new int[0], "实", "", "def", "SwordHitYellowCritical", "quanzong");
            config[199043] = new HeroConfig(199043, "骆统", 193, 236, 69, 53, 69, 70, 70, 4, 3, 3, 4, 4, 4, 4, 1, 344, false, 99, "", "濮阳", 0, "谦和", new string[]{"仁德","贤明"}, new string[]{"读书"}, "无", null, null, new string[]{"谏孙权宽刑"}, 1, new int[0], "", "", "def", "SwordHitWhiteCritical", "luotong");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
