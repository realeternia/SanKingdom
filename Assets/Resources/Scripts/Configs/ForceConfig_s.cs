using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ForceConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///影响id
        /// </summary>
        public int HeroId;
        /// <summary>
        ///颜色
        /// </summary>
        public string Color;


        public ForceConfig(int Id, string Cname, int HeroId, string Color)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.HeroId = HeroId;
            this.Color = Color;

        }

        public ForceConfig() { }

        private static Dictionary<int, ForceConfig> config = new Dictionary<int, ForceConfig>();
        public static Dictionary<int, ForceConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, ForceConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new ForceConfig(1, "刘备", 100001, "50,255,50");
            config[2] = new ForceConfig(2, "曹操", 100002, "50,50,255");
            config[3] = new ForceConfig(3, "孙权", 100003, "255,50,50");
            config[4] = new ForceConfig(4, "袁绍", 100006, "200,200,0");
            config[5] = new ForceConfig(5, "董卓", 100004, "100,100,100");

        }

        public static ForceConfig GetConfig(int id)
        {
            ForceConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ForceConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ForceConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ForceConfig configData)
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
