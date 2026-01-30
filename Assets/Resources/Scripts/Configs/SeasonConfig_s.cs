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
        ///音乐
        /// </summary>
        public string BGM;


        public SeasonConfig(int Id, string Name, string BGM)
        {
            this.Id = Id;
            this.Name = Name;
            this.BGM = BGM;

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
            config[1] = new SeasonConfig(1, "一月 冬", "dong");
            config[2] = new SeasonConfig(2, "二月 冬", "dong");
            config[3] = new SeasonConfig(3, "三月 春", "chun");
            config[4] = new SeasonConfig(4, "四月 春", "chun");
            config[5] = new SeasonConfig(5, "五月 春", "chun");
            config[6] = new SeasonConfig(6, "六月 夏", "xia");
            config[7] = new SeasonConfig(7, "七月 夏", "xia");
            config[8] = new SeasonConfig(8, "八月 夏", "xia");
            config[9] = new SeasonConfig(9, "九月 秋", "qiu");
            config[10] = new SeasonConfig(10, "十月 秋", "qiu");
            config[11] = new SeasonConfig(11, "十一月 秋", "qiu");
            config[12] = new SeasonConfig(12, "十二月 冬", "dong");



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
