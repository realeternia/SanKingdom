using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class WorldConfig
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
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///x
        /// </summary>
        public int X;
        /// <summary>
        ///y
        /// </summary>
        public int Y;
        /// <summary>
        ///是否正面
        /// </summary>
        public int Width;
        /// <summary>
        ///hit
        /// </summary>
        public int Height;
        /// <summary>
        ///force
        /// </summary>
        public int ForceId;
        /// <summary>
        ///商业(0-999)
        /// </summary>
        public int ArchGold;
        /// <summary>
        ///农业(0-999)
        /// </summary>
        public int ArchFood;
        /// <summary>
        ///人口
        /// </summary>
        public int ArchPeople;
        /// <summary>
        ///金钱
        /// </summary>
        public int Gold;
        /// <summary>
        ///食物
        /// </summary>
        public int Food;
        /// <summary>
        ///士兵
        /// </summary>
        public int Soldier;
        /// <summary>
        ///治安
        /// </summary>
        public int Secure;
        /// <summary>
        ///防御
        /// </summary>
        public int Wall;
        /// <summary>
        ///太守
        /// </summary>
        public int Leader;
        /// <summary>
        ///成员
        /// </summary>
        public int[] Members;
        /// <summary>
        ///view位置
        /// </summary>
        public string ViewPrefab;


        public WorldConfig(int Id, string Name, string Cname, int X, int Y, int Width, int Height, int ForceId, int ArchGold, int ArchFood, int ArchPeople, int Gold, int Food, int Soldier, int Secure, int Wall, int Leader, int[] Members, string ViewPrefab)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.ForceId = ForceId;
            this.ArchGold = ArchGold;
            this.ArchFood = ArchFood;
            this.ArchPeople = ArchPeople;
            this.Gold = Gold;
            this.Food = Food;
            this.Soldier = Soldier;
            this.Secure = Secure;
            this.Wall = Wall;
            this.Leader = Leader;
            this.Members = Members;
            this.ViewPrefab = ViewPrefab;

        }

        public WorldConfig() { }

        private static Dictionary<int, WorldConfig> config = new Dictionary<int, WorldConfig>();
        public static Dictionary<int, WorldConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, WorldConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[10001] = new WorldConfig(10001, "wulin", "武陵", 630, 1442, 351, 266, 1, 175, 200, 15000, 1800, 1800, 2250, 70, 60, 101015, new int[]{101009,101022}, "chengdu");
            config[10002] = new WorldConfig(10002, "yongan", "永安", 425, 1106, 311, 382, 1, 200, 150, 13300, 1500, 1200, 3000, 75, 80, 101007, new int[]{101028,101040}, "chengdu");
            config[10003] = new WorldConfig(10003, "lingling", "零陵", 641, 1684, 314, 358, 1, 190, 210, 14000, 1700, 1900, 2000, 65, 55, 101014, new int[]{101030,101024}, "chengdu");
            config[10004] = new WorldConfig(10004, "jiangling", "江陵", 669, 1254, 438, 243, 1, 275, 250, 23300, 2500, 2200, 4000, 75, 70, 101005, new int[]{101006,101032,101037}, "chengdu");
            config[10005] = new WorldConfig(10005, "jiangzhou", "江州", 269, 1295, 355, 344, 1, 210, 225, 16600, 1900, 2000, 2750, 70, 65, 101018, new int[]{101039,101036}, "chengdu");
            config[10006] = new WorldConfig(10006, "jianning", "建宁", 45, 1555, 499, 345, 1, 150, 175, 11600, 1200, 1500, 1750, 60, 50, 101016, new int[]{101035,101039}, "chengdu");
            config[10007] = new WorldConfig(10007, "chengdu", "成都", 40, 1203, 378, 437, 1, 350, 375, 30000, 3500, 4000, 5000, 85, 75, 100001, new int[]{101028,101033,101034}, "chengdu");
            config[10008] = new WorldConfig(10008, "yunnan", "云南", 42, 1646, 444, 395, 1, 125, 150, 10000, 1000, 1300, 1500, 55, 45, 101039, new int[]{101035}, "chengdu");
            config[10009] = new WorldConfig(10009, "zitong", "梓潼", 43, 1004, 506, 274, 1, 225, 200, 16000, 2000, 1800, 3250, 75, 75, 101033, new int[]{101020,101011}, "chengdu");
            config[10010] = new WorldConfig(10010, "hanzhong", "汉中", 145, 770, 348, 311, 1, 250, 225, 20000, 2200, 2000, 4250, 80, 85, 101001, new int[]{101010,101026,101038}, "chengdu");
            config[10011] = new WorldConfig(10011, "chenliu", "陈留", 987, 698, 305, 247, 2, 300, 275, 25000, 2800, 2500, 4500, 80, 70, 102012, new int[]{102028,102024}, "chengdu");
            config[10012] = new WorldConfig(10012, "puyang", "濮阳", 1085, 618, 398, 196, 2, 275, 250, 21600, 2600, 2300, 4000, 75, 65, 102015, new int[]{102022,102032}, "chengdu");
            config[10013] = new WorldConfig(10013, "xinye", "新野", 789, 1022, 261, 177, 2, 225, 210, 16600, 1800, 1700, 3000, 70, 60, 102016, new int[]{102038}, "chengdu");
            config[10014] = new WorldConfig(10014, "runan", "汝南", 1027, 940, 273, 231, 2, 260, 290, 22600, 2400, 2600, 3750, 75, 65, 102006, new int[]{102031,102034}, "chengdu");
            config[10015] = new WorldConfig(10015, "xiangyang", "襄阳", 640, 1080, 305, 241, 2, 325, 300, 26600, 3000, 2800, 4750, 85, 90, 102008, new int[]{102010,102023,102042}, "chengdu");
            config[10016] = new WorldConfig(10016, "shangyong", "上庸", 493, 909, 241, 233, 2, 200, 190, 15000, 1600, 1600, 2500, 65, 60, 102035, new int[]{102036}, "chengdu");
            config[10017] = new WorldConfig(10017, "xuchang", "许昌", 932, 868, 325, 208, 2, 375, 350, 28300, 3500, 3200, 5500, 90, 80, 100002, new int[]{102001,102003,102029,102037,102041}, "chengdu");
            config[10018] = new WorldConfig(10018, "wan", "宛", 670, 851, 276, 223, 2, 290, 275, 23300, 2700, 2400, 4250, 80, 75, 102002, new int[]{102014,102040}, "chengdu");
            config[10019] = new WorldConfig(10019, "beihai", "北海", 1441, 494, 341, 213, 2, 250, 240, 20000, 2300, 2200, 3500, 70, 65, 102009, new int[]{102027}, "chengdu");
            config[10020] = new WorldConfig(10020, "xiaopei", "小沛", 1279, 701, 248, 193, 2, 240, 225, 18300, 2100, 2000, 3250, 70, 60, 102011, new int[]{102013}, "chengdu");
            config[10021] = new WorldConfig(10021, "xiapi", "下邳", 1457, 672, 286, 297, 2, 275, 260, 21600, 2500, 2300, 4000, 75, 70, 102004, new int[]{102005,102020}, "chengdu");
            config[10022] = new WorldConfig(10022, "shouchun", "寿春", 1264, 876, 297, 248, 2, 290, 300, 24000, 2600, 2700, 4250, 80, 75, 102007, new int[]{102017,102021}, "chengdu");
            config[10023] = new WorldConfig(10023, "guiyang", "桂阳", 837, 1693, 432, 348, 3, 190, 200, 14300, 1600, 1800, 2100, 65, 55, 103024, new int[]{103019,103038}, "chengdu");
            config[10024] = new WorldConfig(10024, "lujiang", "庐江", 1257, 1046, 309, 341, 3, 250, 275, 20600, 2200, 2500, 3500, 75, 70, 103003, new int[]{103009,103034}, "chengdu");
            config[10025] = new WorldConfig(10025, "kuaiji", "会稽", 1505, 1194, 506, 413, 3, 260, 250, 20000, 2400, 2200, 3250, 70, 60, 103026, new int[]{103027,103033}, "chengdu");
            config[10026] = new WorldConfig(10026, "jiangxia", "江夏", 949, 1115, 377, 215, 3, 275, 260, 21600, 2500, 2300, 4000, 80, 85, 103004, new int[]{103018,103032}, "chengdu");
            config[10027] = new WorldConfig(10027, "wu", "吴", 1617, 831, 314, 404, 3, 340, 325, 27300, 3200, 2900, 4750, 85, 80, 103012, new int[]{103025,103013,103016,103017}, "chengdu");
            config[10028] = new WorldConfig(10028, "jianye", "建业", 1469, 925, 277, 498, 3, 360, 340, 29300, 3400, 3100, 5250, 90, 85, 100003, new int[]{103008,103010,103011,103014,103028}, "jianye");
            config[10029] = new WorldConfig(10029, "caisang", "柴桑", 1111, 1265, 452, 503, 3, 300, 275, 23300, 2800, 2500, 4500, 85, 90, 103007, new int[]{103006,103021,103035}, "jianye");
            config[10030] = new WorldConfig(10030, "changsha", "长沙", 928, 1440, 379, 323, 3, 290, 310, 25000, 2700, 2800, 4250, 80, 75, 103005, new int[]{103020,103022,103029}, "chengdu");
            config[10031] = new WorldConfig(10031, "pingyuan", "平原", 1228, 514, 229, 173, 4, 240, 260, 19300, 2200, 2300, 3500, 70, 65, 106001, new int[]{106002,106005}, "chengdu");
            config[10032] = new WorldConfig(10032, "nanpi", "南皮", 1229, 325, 273, 211, 4, 300, 290, 24000, 2800, 2600, 4500, 80, 75, 106003, new int[]{106007,106008}, "chengdu");
            config[10033] = new WorldConfig(10033, "jinyang", "晋阳", 679, 235, 498, 384, 4, 225, 200, 16600, 2000, 1800, 3750, 75, 80, 106004, new int[]{106006}, "chengdu");
            config[10034] = new WorldConfig(10034, "beiping", "北平", 1351, 54, 288, 317, 4, 210, 225, 17300, 1900, 2000, 4000, 80, 85, 110001, null, "beiping");
            config[10035] = new WorldConfig(10035, "ye", "邺", 902, 356, 336, 357, 4, 350, 325, 28300, 3300, 2900, 5500, 85, 90, 100006, new int[]{106003,106007,106008}, "chengdu");
            config[10036] = new WorldConfig(10036, "ji", "蓟", 980, 105, 397, 280, 4, 250, 240, 20000, 2300, 2200, 4250, 80, 85, 110005, new int[]{110010}, "chengdu");
            config[10037] = new WorldConfig(10037, "xiangping", "襄平", 1558, 13, 317, 357, 4, 175, 190, 13300, 1500, 1700, 3000, 70, 75, 0, null, "chengdu");
            config[10038] = new WorldConfig(10038, "luoyang", "洛阳", 704, 613, 328, 283, 5, 400, 375, 31600, 4000, 3500, 6000, 95, 95, 100004, new int[]{104002,104007,104008,104009}, "chengdu");
            config[10039] = new WorldConfig(10039, "changan", "长安", 370, 575, 415, 351, 5, 390, 350, 30000, 3800, 3200, 5750, 90, 90, 104001, new int[]{104003,104004,104005,104006,104010}, "chengdu");
            config[10040] = new WorldConfig(10040, "anding", "安定", 155, 412, 411, 335, 5, 200, 210, 16000, 1800, 1900, 3500, 75, 80, 101003, new int[]{101011}, "chengdu");
            config[10041] = new WorldConfig(10041, "tianshui", "天水", 42, 623, 322, 253, 5, 210, 200, 16600, 1900, 1800, 3750, 75, 85, 110002, null, "chengdu");
            config[10042] = new WorldConfig(10042, "wuwei", "武威", 42, 253, 288, 386, 5, 150, 160, 11600, 1300, 1400, 3250, 70, 80, 0, null, "chengdu");

        }

        public static WorldConfig GetConfig(int id)
        {
            WorldConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表WorldConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, WorldConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, WorldConfig configData)
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
