using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SeasonConfig
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
        ///季节
        /// </summary>
        public string Season;
        /// <summary>
        ///发钱
        /// </summary>
        public float AddGold;
        /// <summary>
        ///发米
        /// </summary>
        public float AddFood;
        /// <summary>
        ///音乐
        /// </summary>
        public string BGM;
        /// <summary>
        ///视频
        /// </summary>
        public string Video;


        public SeasonConfig(int Id, string Name, string Season, float AddGold, float AddFood, string BGM, string Video)
        {
            this.Id = Id;
            this.Name = Name;
            this.Season = Season;
            this.AddGold = AddGold;
            this.AddFood = AddFood;
            this.BGM = BGM;
            this.Video = Video;

        }

        public SeasonConfig() { }

        private static Dictionary<int, SeasonConfig> config = new Dictionary<int, SeasonConfig>();
        public static Dictionary<int, SeasonConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SeasonConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new SeasonConfig(1, "一月一日", "冬", 3f, 0, "dong", "goldin.mp4");
            config[2] = new SeasonConfig(2, "一月十日", "冬", 0, 0, "dong", "");
            config[3] = new SeasonConfig(3, "一月二十日", "冬", 0, 0, "dong", "");
            config[4] = new SeasonConfig(4, "二月一日", "冬", 3f, 0, "dong", "goldin.mp4");
            config[5] = new SeasonConfig(5, "二月十日", "冬", 0, 0, "dong", "");
            config[6] = new SeasonConfig(6, "二月二十日", "冬", 0, 0, "dong", "");
            config[7] = new SeasonConfig(7, "三月一日", "春", 3f, 0, "chun", "goldin.mp4");
            config[8] = new SeasonConfig(8, "三月十日", "春", 0, 0, "chun", "");
            config[9] = new SeasonConfig(9, "三月二十日", "春", 0, 0, "chun", "");
            config[10] = new SeasonConfig(10, "四月一日", "春", 3f, 0, "chun", "goldin.mp4");
            config[11] = new SeasonConfig(11, "四月十日", "春", 0, 0, "chun", "");
            config[12] = new SeasonConfig(12, "四月二十日", "春", 0, 6f, "chun", "harve2.mp4");
            config[13] = new SeasonConfig(13, "五月一日", "春", 3f, 0, "chun", "goldin.mp4");
            config[14] = new SeasonConfig(14, "五月十日", "春", 0, 0, "chun", "");
            config[15] = new SeasonConfig(15, "五月二十日", "春", 0, 0, "chun", "");
            config[16] = new SeasonConfig(16, "六月一日", "夏", 3f, 0, "xia", "goldin.mp4");
            config[17] = new SeasonConfig(17, "六月十日", "夏", 0, 0, "xia", "");
            config[18] = new SeasonConfig(18, "六月二十日", "夏", 0, 0, "xia", "");
            config[19] = new SeasonConfig(19, "七月一日", "夏", 3f, 0, "xia", "goldin.mp4");
            config[20] = new SeasonConfig(20, "七月十日", "夏", 0, 0, "xia", "");
            config[21] = new SeasonConfig(21, "七月二十日", "夏", 0, 20f, "xia", "harve2.mp4");
            config[22] = new SeasonConfig(22, "八月一日", "夏", 3f, 0, "xia", "goldin.mp4");
            config[23] = new SeasonConfig(23, "八月十日", "夏", 0, 0, "xia", "");
            config[24] = new SeasonConfig(24, "八月二十日", "夏", 0, 0, "xia", "");
            config[25] = new SeasonConfig(25, "九月一日", "秋", 3f, 0, "qiu", "goldin.mp4");
            config[26] = new SeasonConfig(26, "九月十日", "秋", 0, 0, "qiu", "");
            config[27] = new SeasonConfig(27, "九月二十日", "秋", 0, 0, "qiu", "");
            config[28] = new SeasonConfig(28, "十月一日", "秋", 3f, 0, "qiu", "goldin.mp4");
            config[29] = new SeasonConfig(29, "十月十日", "秋", 0, 0, "qiu", "");
            config[30] = new SeasonConfig(30, "十月二十日", "秋", 0, 6f, "qiu", "harve2.mp4");
            config[31] = new SeasonConfig(31, "十一月一日", "秋", 3f, 0, "qiu", "goldin.mp4");
            config[32] = new SeasonConfig(32, "十一月十日", "秋", 0, 0, "qiu", "");
            config[33] = new SeasonConfig(33, "十一月二十日", "秋", 0, 0, "qiu", "");
            config[34] = new SeasonConfig(34, "十二月一日", "冬", 3f, 0, "dong", "goldin.mp4");
            config[35] = new SeasonConfig(35, "十二月十日", "冬", 0, 0, "dong", "");
            config[36] = new SeasonConfig(36, "十二月二十日", "冬", 0, 0, "dong", "");



        }

        public static SeasonConfig GetConfig(int id)
        {
            SeasonConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SeasonConfig不存在id={0}", id));
        }



        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SeasonConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SeasonConfig configData)
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
