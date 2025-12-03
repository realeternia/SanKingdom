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


        public WorldConfig(int Id, string Name, string Cname, int X, int Y, int Width, int Height, int ForceId)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            this.ForceId = ForceId;

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
            config[10001] = new WorldConfig(10001, "wulin", "武陵", 630, 1442, 351, 266, 1);
            config[10002] = new WorldConfig(10002, "luoyang", "洛阳", 704, 613, 328, 283, 5);
            config[10003] = new WorldConfig(10003, "pingyuan", "平原", 1228, 514, 229, 173, 4);
            config[10004] = new WorldConfig(10004, "nanpi", "南皮", 1229, 325, 273, 211, 4);
            config[10005] = new WorldConfig(10005, "chenliu", "陈留", 987, 698, 305, 247, 2);
            config[10006] = new WorldConfig(10006, "puyang", "濮阳", 1085, 618, 398, 196, 2);
            config[10007] = new WorldConfig(10007, "xinye", "新野", 789, 1022, 261, 177, 2);
            config[10008] = new WorldConfig(10008, "runan", "汝南", 1027, 940, 273, 231, 2);
            config[10009] = new WorldConfig(10009, "xiangyang", "襄阳", 640, 1080, 305, 241, 2);
            config[10010] = new WorldConfig(10010, "shangyong", "上庸", 493, 909, 241, 233, 2);
            config[10011] = new WorldConfig(10011, "yongan", "永安", 425, 1106, 311, 382, 1);
            config[10012] = new WorldConfig(10012, "lingling", "零陵", 641, 1684, 314, 358, 1);
            config[10013] = new WorldConfig(10013, "guiyang", "桂阳", 837, 1693, 432, 348, 3);
            config[10014] = new WorldConfig(10014, "xuchang", "许昌", 932, 868, 325, 208, 2);
            config[10015] = new WorldConfig(10015, "wan", "宛", 670, 851, 276, 223, 2);
            config[10016] = new WorldConfig(10016, "lujiang", "庐江", 1257, 1046, 309, 341, 3);
            config[10017] = new WorldConfig(10017, "kuaiji", "会稽", 1505, 1194, 506, 413, 3);
            config[10018] = new WorldConfig(10018, "jiangling", "江陵", 669, 1254, 438, 243, 1);
            config[10019] = new WorldConfig(10019, "jiangzhou", "江州", 269, 1295, 355, 344, 1);
            config[10020] = new WorldConfig(10020, "jianning", "建宁", 45, 1555, 499, 345, 1);
            config[10021] = new WorldConfig(10021, "chengdu", "成都", 40, 1203, 378, 437, 1);
            config[10022] = new WorldConfig(10022, "yunnan", "云南", 42, 1646, 444, 395, 1);
            config[10023] = new WorldConfig(10023, "zitong", "梓潼", 43, 1004, 506, 274, 1);
            config[10024] = new WorldConfig(10024, "hanzhong", "汉中", 145, 770, 348, 311, 1);
            config[10025] = new WorldConfig(10025, "changan", "长安", 370, 575, 415, 351, 5);
            config[10026] = new WorldConfig(10026, "anding", "安定", 155, 412, 411, 335, 5);
            config[10027] = new WorldConfig(10027, "tianshui", "天水", 42, 623, 322, 253, 5);
            config[10028] = new WorldConfig(10028, "wuwei", "武威", 42, 253, 288, 386, 5);
            config[10029] = new WorldConfig(10029, "jinyang", "晋阳", 679, 235, 498, 384, 4);
            config[10030] = new WorldConfig(10030, "beiping", "北平", 1351, 54, 288, 317, 4);
            config[10031] = new WorldConfig(10031, "beihai", "北海", 1441, 494, 341, 213, 2);
            config[10032] = new WorldConfig(10032, "xiaopei", "小沛", 1279, 701, 248, 193, 2);
            config[10033] = new WorldConfig(10033, "xiapi", "下邳", 1457, 672, 286, 297, 2);
            config[10034] = new WorldConfig(10034, "shouchun", "寿春", 1264, 876, 297, 248, 2);
            config[10035] = new WorldConfig(10035, "jiangxia", "江夏", 949, 1115, 377, 215, 3);
            config[10036] = new WorldConfig(10036, "wu", "吴", 1617, 831, 314, 404, 3);
            config[10037] = new WorldConfig(10037, "ye", "邺", 902, 356, 336, 357, 4);
            config[10038] = new WorldConfig(10038, "ji", "蓟", 980, 105, 397, 280, 4);
            config[10039] = new WorldConfig(10039, "jianye", "建业", 1469, 925, 277, 498, 3);
            config[10040] = new WorldConfig(10040, "caisang", "柴桑", 1111, 1265, 452, 503, 3);
            config[10041] = new WorldConfig(10041, "changsha", "长沙", 928, 1440, 379, 323, 3);
            config[10042] = new WorldConfig(10042, "xiangping", "襄平", 1558, 13, 317, 357, 4);

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
