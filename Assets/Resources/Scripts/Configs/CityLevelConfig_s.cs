using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityLevelConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public int ExpNeed;
        /// <summary>
        ///黄金产量
        /// </summary>
        public int GoldAdd;
        /// <summary>
        ///粮食产量
        /// </summary>
        public int FoodAdd;
        /// <summary>
        ///士兵产量
        /// </summary>
        public int SoldierAdd;


        public CityLevelConfig(int Id, int ExpNeed, int GoldAdd, int FoodAdd, int SoldierAdd)
        {
            this.Id = Id;
            this.ExpNeed = ExpNeed;
            this.GoldAdd = GoldAdd;
            this.FoodAdd = FoodAdd;
            this.SoldierAdd = SoldierAdd;

        }

        public CityLevelConfig() { }

        private static Dictionary<int, CityLevelConfig> config = new Dictionary<int, CityLevelConfig>();
        public static Dictionary<int, CityLevelConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityLevelConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new CityLevelConfig(1, 100, 10, 10, 10);
            config[2] = new CityLevelConfig(2, 100, 10, 10, 10);
            config[3] = new CityLevelConfig(3, 100, 10, 10, 10);
            config[4] = new CityLevelConfig(4, 100, 10, 10, 10);
            config[5] = new CityLevelConfig(5, 100, 10, 10, 10);
            config[6] = new CityLevelConfig(6, 100, 10, 10, 10);
            config[7] = new CityLevelConfig(7, 100, 10, 10, 10);
            config[8] = new CityLevelConfig(8, 100, 10, 10, 10);
            config[9] = new CityLevelConfig(9, 100, 10, 10, 10);
            config[10] = new CityLevelConfig(10, 100, 11, 11, 11);
            config[11] = new CityLevelConfig(11, 100, 11, 11, 11);
            config[12] = new CityLevelConfig(12, 100, 11, 11, 11);
            config[13] = new CityLevelConfig(13, 100, 11, 11, 11);
            config[14] = new CityLevelConfig(14, 100, 11, 11, 11);
            config[15] = new CityLevelConfig(15, 100, 11, 11, 11);
            config[16] = new CityLevelConfig(16, 100, 11, 11, 11);
            config[17] = new CityLevelConfig(17, 100, 11, 11, 11);
            config[18] = new CityLevelConfig(18, 100, 11, 11, 11);
            config[19] = new CityLevelConfig(19, 100, 11, 11, 11);
            config[20] = new CityLevelConfig(20, 100, 12, 12, 12);
            config[21] = new CityLevelConfig(21, 100, 12, 12, 12);
            config[22] = new CityLevelConfig(22, 100, 12, 12, 12);
            config[23] = new CityLevelConfig(23, 100, 12, 12, 12);
            config[24] = new CityLevelConfig(24, 100, 12, 12, 12);
            config[25] = new CityLevelConfig(25, 100, 12, 12, 12);
            config[26] = new CityLevelConfig(26, 100, 12, 12, 12);
            config[27] = new CityLevelConfig(27, 100, 12, 12, 12);
            config[28] = new CityLevelConfig(28, 100, 12, 12, 12);
            config[29] = new CityLevelConfig(29, 100, 12, 12, 12);
            config[30] = new CityLevelConfig(30, 100, 13, 13, 13);
            config[31] = new CityLevelConfig(31, 100, 13, 13, 13);
            config[32] = new CityLevelConfig(32, 100, 13, 13, 13);
            config[33] = new CityLevelConfig(33, 100, 13, 13, 13);
            config[34] = new CityLevelConfig(34, 100, 13, 13, 13);
            config[35] = new CityLevelConfig(35, 100, 13, 13, 13);
            config[36] = new CityLevelConfig(36, 100, 13, 13, 13);
            config[37] = new CityLevelConfig(37, 100, 13, 13, 13);
            config[38] = new CityLevelConfig(38, 100, 13, 13, 13);
            config[39] = new CityLevelConfig(39, 100, 13, 13, 13);
            config[40] = new CityLevelConfig(40, 100, 14, 14, 14);
            config[41] = new CityLevelConfig(41, 100, 14, 14, 14);
            config[42] = new CityLevelConfig(42, 100, 14, 14, 14);
            config[43] = new CityLevelConfig(43, 100, 14, 14, 14);
            config[44] = new CityLevelConfig(44, 100, 14, 14, 14);
            config[45] = new CityLevelConfig(45, 100, 14, 14, 14);
            config[46] = new CityLevelConfig(46, 100, 14, 14, 14);
            config[47] = new CityLevelConfig(47, 100, 14, 14, 14);
            config[48] = new CityLevelConfig(48, 100, 14, 14, 14);
            config[49] = new CityLevelConfig(49, 100, 14, 14, 14);
            config[50] = new CityLevelConfig(50, 100, 15, 15, 15);
            config[51] = new CityLevelConfig(51, 100, 15, 15, 15);
            config[52] = new CityLevelConfig(52, 100, 15, 15, 15);
            config[53] = new CityLevelConfig(53, 100, 15, 15, 15);
            config[54] = new CityLevelConfig(54, 100, 15, 15, 15);
            config[55] = new CityLevelConfig(55, 100, 15, 15, 15);
            config[56] = new CityLevelConfig(56, 100, 15, 15, 15);
            config[57] = new CityLevelConfig(57, 100, 15, 15, 15);
            config[58] = new CityLevelConfig(58, 100, 15, 15, 15);
            config[59] = new CityLevelConfig(59, 100, 15, 15, 15);
            config[60] = new CityLevelConfig(60, 100, 16, 16, 16);
            config[61] = new CityLevelConfig(61, 100, 16, 16, 16);
            config[62] = new CityLevelConfig(62, 100, 16, 16, 16);
            config[63] = new CityLevelConfig(63, 100, 16, 16, 16);
            config[64] = new CityLevelConfig(64, 100, 16, 16, 16);
            config[65] = new CityLevelConfig(65, 100, 16, 16, 16);
            config[66] = new CityLevelConfig(66, 100, 16, 16, 16);
            config[67] = new CityLevelConfig(67, 100, 16, 16, 16);
            config[68] = new CityLevelConfig(68, 100, 16, 16, 16);
            config[69] = new CityLevelConfig(69, 100, 16, 16, 16);
            config[70] = new CityLevelConfig(70, 100, 17, 17, 17);
            config[71] = new CityLevelConfig(71, 100, 17, 17, 17);
            config[72] = new CityLevelConfig(72, 100, 17, 17, 17);
            config[73] = new CityLevelConfig(73, 100, 17, 17, 17);
            config[74] = new CityLevelConfig(74, 100, 17, 17, 17);
            config[75] = new CityLevelConfig(75, 100, 17, 17, 17);
            config[76] = new CityLevelConfig(76, 100, 17, 17, 17);
            config[77] = new CityLevelConfig(77, 100, 17, 17, 17);
            config[78] = new CityLevelConfig(78, 100, 17, 17, 17);
            config[79] = new CityLevelConfig(79, 100, 17, 17, 17);
            config[80] = new CityLevelConfig(80, 100, 18, 18, 18);
            config[81] = new CityLevelConfig(81, 100, 18, 18, 18);
            config[82] = new CityLevelConfig(82, 100, 18, 18, 18);
            config[83] = new CityLevelConfig(83, 100, 18, 18, 18);
            config[84] = new CityLevelConfig(84, 100, 18, 18, 18);
            config[85] = new CityLevelConfig(85, 100, 18, 18, 18);
            config[86] = new CityLevelConfig(86, 100, 18, 18, 18);
            config[87] = new CityLevelConfig(87, 100, 18, 18, 18);
            config[88] = new CityLevelConfig(88, 100, 18, 18, 18);
            config[89] = new CityLevelConfig(89, 100, 18, 18, 18);
            config[90] = new CityLevelConfig(90, 100, 19, 19, 19);
            config[91] = new CityLevelConfig(91, 100, 19, 19, 19);
            config[92] = new CityLevelConfig(92, 100, 19, 19, 19);
            config[93] = new CityLevelConfig(93, 100, 19, 19, 19);
            config[94] = new CityLevelConfig(94, 100, 19, 19, 19);
            config[95] = new CityLevelConfig(95, 100, 19, 19, 19);
            config[96] = new CityLevelConfig(96, 100, 19, 19, 19);
            config[97] = new CityLevelConfig(97, 100, 19, 19, 19);
            config[98] = new CityLevelConfig(98, 100, 19, 19, 19);
            config[99] = new CityLevelConfig(99, 100, 19, 19, 19);
            config[100] = new CityLevelConfig(100, 100, 20, 20, 20);



        }

        public static CityLevelConfig GetConfig(int id)
        {
            CityLevelConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityLevelConfig不存在id={0}", id));
        }



        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityLevelConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityLevelConfig configData)
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
