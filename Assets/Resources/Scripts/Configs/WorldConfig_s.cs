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
        ///等级
        /// </summary>
        public int Level;
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
        ///防御
        /// </summary>
        public int Wall;
        /// <summary>
        ///相邻
        /// </summary>
        public int[] WorldNearIds;
        /// <summary>
        ///Mini地图偏移
        /// </summary>
        public int[] MiniMapOffsets;
        /// <summary>
        ///view位置
        /// </summary>
        public string ViewPrefab;


        public WorldConfig(int Id, string Name, string Cname, int X, int Y, int Width, int Height, int ForceId, int Level, int Gold, int Food, int Soldier, int Wall, int[] WorldNearIds, int[] MiniMapOffsets, string ViewPrefab)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.ForceId = ForceId;
            this.Level = Level;
            this.Gold = Gold;
            this.Food = Food;
            this.Soldier = Soldier;
            this.Wall = Wall;
            this.WorldNearIds = WorldNearIds;
            this.MiniMapOffsets = MiniMapOffsets;
            this.ViewPrefab = ViewPrefab;

        }

        public WorldConfig() { }

        private static Dictionary<int, WorldConfig> config = new Dictionary<int, WorldConfig>();
        public static Dictionary<int, WorldConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, WorldConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[10001] = new WorldConfig(10001, "xinye", "新野", 789, 1022, 261, 177, 1, 1, 100, 100, 100, 100, new int[]{10028,10005,10038,10004,10029}, null, "xinye");
            config[10002] = new WorldConfig(10002, "chenliu", "陈留", 987, 698, 305, 247, 2, 2, 100, 100, 100, 100, new int[]{10003,10039,10004,10020,10007}, new int[]{-10,-10}, "chengdu");
            config[10003] = new WorldConfig(10003, "puyang", "濮阳", 1085, 618, 398, 196, 2, 2, 100, 100, 100, 100, new int[]{10002,10006,10007,10016,10019}, new int[]{-15,-10}, "chengdu");
            config[10004] = new WorldConfig(10004, "xuchang", "许昌", 932, 868, 325, 208, 2, 5, 100, 100, 100, 100, new int[]{10002,10038,10001,10005,10020}, new int[]{-20,0}, "changan");
            config[10005] = new WorldConfig(10005, "wan", "宛", 670, 851, 276, 223, 2, 3, 100, 100, 100, 100, new int[]{10036,10001,10028,10004,10020,10021}, new int[]{5,-5}, "chengdu");
            config[10006] = new WorldConfig(10006, "beihai", "北海", 1441, 494, 341, 213, 2, 3, 100, 100, 100, 100, new int[]{10003,10008,10016}, null, "chengdu");
            config[10007] = new WorldConfig(10007, "xiaopei", "小沛", 1279, 701, 248, 193, 2, 1, 100, 100, 100, 100, new int[]{10002,10003,10008,10039}, new int[]{8,-8}, "chengdu");
            config[10008] = new WorldConfig(10008, "xiapi", "下邳", 1457, 672, 286, 297, 2, 1, 100, 100, 100, 100, new int[]{10039,10006,10007,10012,10013}, new int[]{15,15}, "chengdu");
            config[10009] = new WorldConfig(10009, "guiyang", "桂阳", 837, 1693, 432, 348, 3, 1, 100, 100, 100, 100, new int[]{10026,10015}, null, "xinye");
            config[10010] = new WorldConfig(10010, "lujiang", "庐江", 1257, 1046, 309, 341, 3, 2, 100, 100, 100, 100, new int[]{10039,10029,10013,10014}, new int[]{17,7}, "chengdu");
            config[10011] = new WorldConfig(10011, "kuaiji", "会稽", 1505, 1194, 506, 413, 3, 3, 100, 100, 100, 100, new int[]{10012,10013,10014}, null, "jianye");
            config[10012] = new WorldConfig(10012, "wu", "吴", 1617, 831, 314, 404, 3, 4, 100, 100, 100, 100, new int[]{10008,10011,10013}, new int[]{20,0}, "xinye");
            config[10013] = new WorldConfig(10013, "jianye", "建业", 1469, 925, 277, 498, 3, 6, 100, 100, 100, 100, new int[]{10039,10010,10011,10012,10014,10008}, new int[]{12,0}, "jianye");
            config[10014] = new WorldConfig(10014, "caisang", "柴桑", 1111, 1265, 452, 503, 3, 3, 100, 100, 100, 100, new int[]{10027,10010,10029,10013,10015,10011}, new int[]{-20,40}, "jianye");
            config[10015] = new WorldConfig(10015, "changsha", "长沙", 928, 1440, 379, 323, 3, 3, 100, 100, 100, 100, new int[]{10025,10027,10009,10014}, new int[]{-15,5}, "changsha");
            config[10016] = new WorldConfig(10016, "pingyuan", "平原", 1228, 514, 229, 173, 4, 2, 100, 100, 100, 100, new int[]{10003,10006,10019,10017}, null, "xinye");
            config[10017] = new WorldConfig(10017, "nanpi", "南皮", 1229, 325, 273, 211, 4, 4, 100, 100, 100, 100, new int[]{10016,10019,10040,10041}, new int[]{-5,-5}, "chengdu");
            config[10018] = new WorldConfig(10018, "jinyang", "晋阳", 679, 235, 498, 384, 4, 1, 100, 100, 100, 100, new int[]{10019,10020,10021,10041}, new int[]{25,0}, "xinye");
            config[10019] = new WorldConfig(10019, "ye", "邺", 902, 356, 336, 357, 4, 4, 100, 100, 100, 100, new int[]{10003,10017,10018,10016,10041,10020}, new int[]{26,-20}, "beiping");
            config[10020] = new WorldConfig(10020, "luoyang", "洛阳", 704, 613, 328, 283, 5, 7, 100, 100, 100, 100, new int[]{10005,10018,10019,10021,10002,10004}, new int[]{5,0}, "changan");
            config[10021] = new WorldConfig(10021, "changan", "长安", 370, 575, 415, 351, 5, 6, 100, 100, 100, 100, new int[]{10037,10018,10020,10022,10036,10005}, new int[]{10,-15}, "changan");
            config[10022] = new WorldConfig(10022, "anding", "安定", 155, 412, 411, 335, 5, 1, 100, 100, 100, 100, new int[]{10024,10021,10023}, new int[]{0,-10}, "xiliang");
            config[10023] = new WorldConfig(10023, "tianshui", "天水", 42, 623, 322, 253, 6, 1, 100, 100, 100, 100, new int[]{10037,10022,10024}, new int[]{-15,0}, "xiliang");
            config[10024] = new WorldConfig(10024, "wuwei", "武威", 42, 253, 288, 386, 6, 1, 100, 100, 100, 100, new int[]{10023,10022}, new int[]{-10,0}, "xiliang");
            config[10025] = new WorldConfig(10025, "wulin", "武陵", 630, 1442, 351, 266, 7, 1, 100, 100, 100, 100, new int[]{10026,10030,10027,10031,10015}, new int[]{0,-10}, "changsha");
            config[10026] = new WorldConfig(10026, "lingling", "零陵", 641, 1684, 314, 358, 7, 1, 100, 100, 100, 100, new int[]{10025,10009}, new int[]{-24,0}, "changsha");
            config[10027] = new WorldConfig(10027, "jiangling", "江陵", 669, 1254, 438, 243, 7, 3, 100, 100, 100, 100, new int[]{10030,10025,10028,10029,10014,10015}, new int[]{15,0}, "chengdu");
            config[10028] = new WorldConfig(10028, "xiangyang", "襄阳", 640, 1080, 305, 241, 7, 5, 100, 100, 100, 100, new int[]{10030,10027,10001,10036,10005,10029}, new int[]{-5,0}, "jianye");
            config[10029] = new WorldConfig(10029, "jiangxia", "江夏", 949, 1115, 377, 215, 7, 3, 100, 100, 100, 100, new int[]{10027,10028,10039,10010,10014,10001,10038}, new int[]{0,-7}, "xinye");
            config[10030] = new WorldConfig(10030, "yongan", "永安", 425, 1106, 311, 382, 8, 1, 100, 100, 100, 100, new int[]{10025,10027,10031,10035,10028,10036}, new int[]{0,-15}, "changsha");
            config[10031] = new WorldConfig(10031, "jiangzhou", "江州", 269, 1295, 355, 344, 8, 2, 100, 100, 100, 100, new int[]{10030,10032,10033,10025}, new int[]{5,-15}, "xinye");
            config[10032] = new WorldConfig(10032, "jianning", "建宁", 45, 1555, 499, 345, 8, 1, 100, 100, 100, 100, new int[]{10031,10033,10034}, new int[]{25,15}, "xinye");
            config[10033] = new WorldConfig(10033, "chengdu", "成都", 40, 1203, 378, 437, 8, 5, 100, 100, 100, 100, new int[]{10031,10032,10035}, new int[]{-30,0}, "chengdu");
            config[10034] = new WorldConfig(10034, "yunnan", "云南", 42, 1646, 444, 395, 8, 1, 100, 100, 100, 100, new int[]{10032}, new int[]{-40,-23}, "xinye");
            config[10035] = new WorldConfig(10035, "zitong", "梓潼", 43, 1004, 506, 274, 8, 2, 100, 100, 100, 100, new int[]{10030,10033,10037,10036}, null, "xinye");
            config[10036] = new WorldConfig(10036, "shangyong", "上庸", 493, 909, 241, 233, 9, 1, 100, 100, 100, 100, new int[]{10030,10035,10037,10028,10005,10021}, new int[]{-10,15}, "xinye");
            config[10037] = new WorldConfig(10037, "hanzhong", "汉中", 145, 770, 348, 311, 9, 3, 100, 100, 100, 100, new int[]{10035,10036,10021,10023}, new int[]{5,0}, "chengdu");
            config[10038] = new WorldConfig(10038, "runan", "汝南", 1027, 940, 273, 231, 10, 4, 100, 100, 100, 100, new int[]{10001,10029,10004,10039,10010}, new int[]{7,-10}, "xinye");
            config[10039] = new WorldConfig(10039, "shouchun", "寿春", 1264, 876, 297, 248, 10, 4, 100, 100, 100, 100, new int[]{10038,10002,10007,10008,10010,10029,10013}, new int[]{0,5}, "chengdu");
            config[10040] = new WorldConfig(10040, "beiping", "北平", 1351, 54, 288, 317, 11, 2, 100, 100, 100, 100, new int[]{10041,10042,10017}, null, "beiping");
            config[10041] = new WorldConfig(10041, "ji", "蓟", 980, 105, 397, 280, 11, 3, 100, 100, 100, 100, new int[]{10017,10019,10018,10040}, new int[]{30,0}, "beiping");
            config[10042] = new WorldConfig(10042, "xiangping", "襄平", 1558, 13, 317, 357, 12, 2, 100, 100, 100, 100, new int[]{10040}, new int[]{20,0}, "chengdu");



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
