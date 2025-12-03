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
        ///启动色
        /// </summary>
        public int X;
        /// <summary>
        ///结束色
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


        public WorldConfig(int Id, string Name, string Cname, int X, int Y, int Width, int Height, int ForceId, int ArchGold, int ArchFood, int ArchPeople, int Gold, int Food, int Soldier, int Secure, int Wall)
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
            config[10001] = new WorldConfig(10001, "wulin", "武陵", 630, 1442, 351, 266, 1, 175, 200, 45000, 1800, 1800, 4500, 70, 60);
            config[10002] = new WorldConfig(10002, "yongan", "永安", 425, 1106, 311, 382, 1, 200, 150, 40000, 1500, 1200, 6000, 75, 80);
            config[10003] = new WorldConfig(10003, "lingling", "零陵", 641, 1684, 314, 358, 1, 190, 210, 42000, 1700, 1900, 4000, 65, 55);
            config[10004] = new WorldConfig(10004, "jiangling", "江陵", 669, 1254, 438, 243, 1, 275, 250, 70000, 2500, 2200, 8000, 75, 70);
            config[10005] = new WorldConfig(10005, "jiangzhou", "江州", 269, 1295, 355, 344, 1, 210, 225, 50000, 1900, 2000, 5500, 70, 65);
            config[10006] = new WorldConfig(10006, "jianning", "建宁", 45, 1555, 499, 345, 1, 150, 175, 35000, 1200, 1500, 3500, 60, 50);
            config[10007] = new WorldConfig(10007, "chengdu", "成都", 40, 1203, 378, 437, 1, 350, 375, 90000, 3500, 4000, 10000, 85, 75);
            config[10008] = new WorldConfig(10008, "yunnan", "云南", 42, 1646, 444, 395, 1, 125, 150, 30000, 1000, 1300, 3000, 55, 45);
            config[10009] = new WorldConfig(10009, "zitong", "梓潼", 43, 1004, 506, 274, 1, 225, 200, 48000, 2000, 1800, 6500, 75, 75);
            config[10010] = new WorldConfig(10010, "hanzhong", "汉中", 145, 770, 348, 311, 1, 250, 225, 60000, 2200, 2000, 8500, 80, 85);
            config[10011] = new WorldConfig(10011, "chenliu", "陈留", 987, 698, 305, 247, 2, 300, 275, 75000, 2800, 2500, 9000, 80, 70);
            config[10012] = new WorldConfig(10012, "puyang", "濮阳", 1085, 618, 398, 196, 2, 275, 250, 65000, 2600, 2300, 8000, 75, 65);
            config[10013] = new WorldConfig(10013, "xinye", "新野", 789, 1022, 261, 177, 2, 225, 210, 50000, 1800, 1700, 6000, 70, 60);
            config[10014] = new WorldConfig(10014, "runan", "汝南", 1027, 940, 273, 231, 2, 260, 290, 68000, 2400, 2600, 7500, 75, 65);
            config[10015] = new WorldConfig(10015, "xiangyang", "襄阳", 640, 1080, 305, 241, 2, 325, 300, 80000, 3000, 2800, 9500, 85, 90);
            config[10016] = new WorldConfig(10016, "shangyong", "上庸", 493, 909, 241, 233, 2, 200, 190, 45000, 1600, 1600, 5000, 65, 60);
            config[10017] = new WorldConfig(10017, "xuchang", "许昌", 932, 868, 325, 208, 2, 375, 350, 85000, 3500, 3200, 11000, 90, 80);
            config[10018] = new WorldConfig(10018, "wan", "宛", 670, 851, 276, 223, 2, 290, 275, 70000, 2700, 2400, 8500, 80, 75);
            config[10019] = new WorldConfig(10019, "beihai", "北海", 1441, 494, 341, 213, 2, 250, 240, 60000, 2300, 2200, 7000, 70, 65);
            config[10020] = new WorldConfig(10020, "xiaopei", "小沛", 1279, 701, 248, 193, 2, 240, 225, 55000, 2100, 2000, 6500, 70, 60);
            config[10021] = new WorldConfig(10021, "xiapi", "下邳", 1457, 672, 286, 297, 2, 275, 260, 65000, 2500, 2300, 8000, 75, 70);
            config[10022] = new WorldConfig(10022, "shouchun", "寿春", 1264, 876, 297, 248, 2, 290, 300, 72000, 2600, 2700, 8500, 80, 75);
            config[10023] = new WorldConfig(10023, "guiyang", "桂阳", 837, 1693, 432, 348, 3, 190, 200, 43000, 1600, 1800, 4200, 65, 55);
            config[10024] = new WorldConfig(10024, "lujiang", "庐江", 1257, 1046, 309, 341, 3, 250, 275, 62000, 2200, 2500, 7000, 75, 70);
            config[10025] = new WorldConfig(10025, "kuaiji", "会稽", 1505, 1194, 506, 413, 3, 260, 250, 60000, 2400, 2200, 6500, 70, 60);
            config[10026] = new WorldConfig(10026, "jiangxia", "江夏", 949, 1115, 377, 215, 3, 275, 260, 65000, 2500, 2300, 8000, 80, 85);
            config[10027] = new WorldConfig(10027, "wu", "吴", 1617, 831, 314, 404, 3, 340, 325, 82000, 3200, 2900, 9500, 85, 80);
            config[10028] = new WorldConfig(10028, "jianye", "建业", 1469, 925, 277, 498, 3, 360, 340, 88000, 3400, 3100, 10500, 90, 85);
            config[10029] = new WorldConfig(10029, "caisang", "柴桑", 1111, 1265, 452, 503, 3, 300, 275, 70000, 2800, 2500, 9000, 85, 90);
            config[10030] = new WorldConfig(10030, "changsha", "长沙", 928, 1440, 379, 323, 3, 290, 310, 75000, 2700, 2800, 8500, 80, 75);
            config[10031] = new WorldConfig(10031, "pingyuan", "平原", 1228, 514, 229, 173, 4, 240, 260, 58000, 2200, 2300, 7000, 70, 65);
            config[10032] = new WorldConfig(10032, "nanpi", "南皮", 1229, 325, 273, 211, 4, 300, 290, 72000, 2800, 2600, 9000, 80, 75);
            config[10033] = new WorldConfig(10033, "jinyang", "晋阳", 679, 235, 498, 384, 4, 225, 200, 50000, 2000, 1800, 7500, 75, 80);
            config[10034] = new WorldConfig(10034, "beiping", "北平", 1351, 54, 288, 317, 4, 210, 225, 52000, 1900, 2000, 8000, 80, 85);
            config[10035] = new WorldConfig(10035, "ye", "邺", 902, 356, 336, 357, 4, 350, 325, 85000, 3300, 2900, 11000, 85, 90);
            config[10036] = new WorldConfig(10036, "ji", "蓟", 980, 105, 397, 280, 4, 250, 240, 60000, 2300, 2200, 8500, 80, 85);
            config[10037] = new WorldConfig(10037, "xiangping", "襄平", 1558, 13, 317, 357, 4, 175, 190, 40000, 1500, 1700, 6000, 70, 75);
            config[10038] = new WorldConfig(10038, "luoyang", "洛阳", 704, 613, 328, 283, 5, 400, 375, 95000, 4000, 3500, 12000, 95, 95);
            config[10039] = new WorldConfig(10039, "changan", "长安", 370, 575, 415, 351, 5, 390, 350, 90000, 3800, 3200, 11500, 90, 90);
            config[10040] = new WorldConfig(10040, "anding", "安定", 155, 412, 411, 335, 5, 200, 210, 48000, 1800, 1900, 7000, 75, 80);
            config[10041] = new WorldConfig(10041, "tianshui", "天水", 42, 623, 322, 253, 5, 210, 200, 50000, 1900, 1800, 7500, 75, 85);
            config[10042] = new WorldConfig(10042, "wuwei", "武威", 42, 253, 288, 386, 5, 150, 160, 35000, 1300, 1400, 6500, 70, 80);

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
