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
        /// <summary>
        ///工位数
        /// </summary>
        public int JobCount;


        public CityLevelConfig(int Id, int ExpNeed, int GoldAdd, int FoodAdd, int SoldierAdd, int JobCount)
        {
            this.Id = Id;
            this.ExpNeed = ExpNeed;
            this.GoldAdd = GoldAdd;
            this.FoodAdd = FoodAdd;
            this.SoldierAdd = SoldierAdd;
            this.JobCount = JobCount;

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
            config[1] = new CityLevelConfig(1, 100, 10, 10, 10, 4);
            config[2] = new CityLevelConfig(2, 110, 10, 10, 10, 4);
            config[3] = new CityLevelConfig(3, 120, 10, 10, 10, 4);
            config[4] = new CityLevelConfig(4, 130, 10, 10, 10, 4);
            config[5] = new CityLevelConfig(5, 140, 10, 10, 10, 4);
            config[6] = new CityLevelConfig(6, 150, 10, 10, 10, 4);
            config[7] = new CityLevelConfig(7, 160, 10, 10, 10, 4);
            config[8] = new CityLevelConfig(8, 170, 10, 10, 10, 4);
            config[9] = new CityLevelConfig(9, 180, 10, 10, 10, 4);
            config[10] = new CityLevelConfig(10, 190, 11, 11, 11, 5);
            config[11] = new CityLevelConfig(11, 200, 11, 11, 11, 5);
            config[12] = new CityLevelConfig(12, 210, 11, 11, 11, 5);
            config[13] = new CityLevelConfig(13, 220, 11, 11, 11, 5);
            config[14] = new CityLevelConfig(14, 230, 11, 11, 11, 5);
            config[15] = new CityLevelConfig(15, 240, 11, 11, 11, 5);
            config[16] = new CityLevelConfig(16, 250, 11, 11, 11, 5);
            config[17] = new CityLevelConfig(17, 260, 11, 11, 11, 5);
            config[18] = new CityLevelConfig(18, 270, 11, 11, 11, 5);
            config[19] = new CityLevelConfig(19, 280, 11, 11, 11, 5);
            config[20] = new CityLevelConfig(20, 290, 12, 12, 12, 6);
            config[21] = new CityLevelConfig(21, 300, 12, 12, 12, 6);
            config[22] = new CityLevelConfig(22, 310, 12, 12, 12, 6);
            config[23] = new CityLevelConfig(23, 320, 12, 12, 12, 6);
            config[24] = new CityLevelConfig(24, 330, 12, 12, 12, 6);
            config[25] = new CityLevelConfig(25, 340, 12, 12, 12, 6);
            config[26] = new CityLevelConfig(26, 350, 12, 12, 12, 6);
            config[27] = new CityLevelConfig(27, 360, 12, 12, 12, 6);
            config[28] = new CityLevelConfig(28, 370, 12, 12, 12, 6);
            config[29] = new CityLevelConfig(29, 380, 12, 12, 12, 6);
            config[30] = new CityLevelConfig(30, 390, 13, 13, 13, 7);
            config[31] = new CityLevelConfig(31, 400, 13, 13, 13, 7);
            config[32] = new CityLevelConfig(32, 410, 13, 13, 13, 7);
            config[33] = new CityLevelConfig(33, 420, 13, 13, 13, 7);
            config[34] = new CityLevelConfig(34, 430, 13, 13, 13, 7);
            config[35] = new CityLevelConfig(35, 440, 13, 13, 13, 7);
            config[36] = new CityLevelConfig(36, 450, 13, 13, 13, 7);
            config[37] = new CityLevelConfig(37, 460, 13, 13, 13, 7);
            config[38] = new CityLevelConfig(38, 470, 13, 13, 13, 7);
            config[39] = new CityLevelConfig(39, 480, 13, 13, 13, 7);
            config[40] = new CityLevelConfig(40, 490, 14, 14, 14, 7);
            config[41] = new CityLevelConfig(41, 500, 14, 14, 14, 7);
            config[42] = new CityLevelConfig(42, 510, 14, 14, 14, 7);
            config[43] = new CityLevelConfig(43, 520, 14, 14, 14, 7);
            config[44] = new CityLevelConfig(44, 530, 14, 14, 14, 7);
            config[45] = new CityLevelConfig(45, 540, 14, 14, 14, 7);
            config[46] = new CityLevelConfig(46, 550, 14, 14, 14, 7);
            config[47] = new CityLevelConfig(47, 560, 14, 14, 14, 7);
            config[48] = new CityLevelConfig(48, 570, 14, 14, 14, 7);
            config[49] = new CityLevelConfig(49, 580, 14, 14, 14, 7);
            config[50] = new CityLevelConfig(50, 590, 15, 15, 15, 8);
            config[51] = new CityLevelConfig(51, 600, 15, 15, 15, 8);
            config[52] = new CityLevelConfig(52, 610, 15, 15, 15, 8);
            config[53] = new CityLevelConfig(53, 620, 15, 15, 15, 8);
            config[54] = new CityLevelConfig(54, 630, 15, 15, 15, 8);
            config[55] = new CityLevelConfig(55, 640, 15, 15, 15, 8);
            config[56] = new CityLevelConfig(56, 650, 15, 15, 15, 8);
            config[57] = new CityLevelConfig(57, 660, 15, 15, 15, 8);
            config[58] = new CityLevelConfig(58, 670, 15, 15, 15, 8);
            config[59] = new CityLevelConfig(59, 680, 15, 15, 15, 8);
            config[60] = new CityLevelConfig(60, 690, 16, 16, 16, 8);
            config[61] = new CityLevelConfig(61, 700, 16, 16, 16, 8);
            config[62] = new CityLevelConfig(62, 710, 16, 16, 16, 8);
            config[63] = new CityLevelConfig(63, 720, 16, 16, 16, 8);
            config[64] = new CityLevelConfig(64, 730, 16, 16, 16, 8);
            config[65] = new CityLevelConfig(65, 740, 16, 16, 16, 8);
            config[66] = new CityLevelConfig(66, 750, 16, 16, 16, 8);
            config[67] = new CityLevelConfig(67, 760, 16, 16, 16, 8);
            config[68] = new CityLevelConfig(68, 770, 16, 16, 16, 8);
            config[69] = new CityLevelConfig(69, 780, 16, 16, 16, 8);
            config[70] = new CityLevelConfig(70, 790, 17, 17, 17, 8);
            config[71] = new CityLevelConfig(71, 800, 17, 17, 17, 8);
            config[72] = new CityLevelConfig(72, 810, 17, 17, 17, 8);
            config[73] = new CityLevelConfig(73, 820, 17, 17, 17, 8);
            config[74] = new CityLevelConfig(74, 830, 17, 17, 17, 8);
            config[75] = new CityLevelConfig(75, 840, 17, 17, 17, 8);
            config[76] = new CityLevelConfig(76, 850, 17, 17, 17, 8);
            config[77] = new CityLevelConfig(77, 860, 17, 17, 17, 8);
            config[78] = new CityLevelConfig(78, 870, 17, 17, 17, 8);
            config[79] = new CityLevelConfig(79, 880, 17, 17, 17, 8);
            config[80] = new CityLevelConfig(80, 890, 18, 18, 18, 8);
            config[81] = new CityLevelConfig(81, 900, 18, 18, 18, 8);
            config[82] = new CityLevelConfig(82, 910, 18, 18, 18, 8);
            config[83] = new CityLevelConfig(83, 920, 18, 18, 18, 8);
            config[84] = new CityLevelConfig(84, 930, 18, 18, 18, 8);
            config[85] = new CityLevelConfig(85, 940, 18, 18, 18, 8);
            config[86] = new CityLevelConfig(86, 950, 18, 18, 18, 8);
            config[87] = new CityLevelConfig(87, 960, 18, 18, 18, 8);
            config[88] = new CityLevelConfig(88, 970, 18, 18, 18, 8);
            config[89] = new CityLevelConfig(89, 980, 18, 18, 18, 8);
            config[90] = new CityLevelConfig(90, 990, 19, 19, 19, 10);
            config[91] = new CityLevelConfig(91, 1000, 19, 19, 19, 10);
            config[92] = new CityLevelConfig(92, 1010, 19, 19, 19, 10);
            config[93] = new CityLevelConfig(93, 1020, 19, 19, 19, 10);
            config[94] = new CityLevelConfig(94, 1030, 19, 19, 19, 10);
            config[95] = new CityLevelConfig(95, 1040, 19, 19, 19, 10);
            config[96] = new CityLevelConfig(96, 1050, 19, 19, 19, 10);
            config[97] = new CityLevelConfig(97, 1060, 19, 19, 19, 10);
            config[98] = new CityLevelConfig(98, 1070, 19, 19, 19, 10);
            config[99] = new CityLevelConfig(99, 1080, 19, 19, 19, 10);
            config[100] = new CityLevelConfig(100, 1090, 20, 20, 20, 10);



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
