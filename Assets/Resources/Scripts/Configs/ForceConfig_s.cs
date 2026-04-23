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
        ///难度
        /// </summary>
        public int Diff;
        /// <summary>
        ///影响id
        /// </summary>
        public int HeroId;
        /// <summary>
        ///初始金钱
        /// </summary>
        public int InitGold;
        /// <summary>
        ///初始木
        /// </summary>
        public int InitWood;
        /// <summary>
        ///马
        /// </summary>
        public int InitHorse;
        /// <summary>
        ///铁
        /// </summary>
        public int InitSteel;
        /// <summary>
        ///石头
        /// </summary>
        public int InitStone;
        /// <summary>
        ///颜色
        /// </summary>
        public string Color;


        public ForceConfig(int Id, string Cname, int Diff, int HeroId, int InitGold, int InitWood, int InitHorse, int InitSteel, int InitStone, string Color)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.Diff = Diff;
            this.HeroId = HeroId;
            this.InitGold = InitGold;
            this.InitWood = InitWood;
            this.InitHorse = InitHorse;
            this.InitSteel = InitSteel;
            this.InitStone = InitStone;
            this.Color = Color;

        }

        public ForceConfig() { }

        private static Dictionary<int, ForceConfig> config = new Dictionary<int, ForceConfig>();
        public static Dictionary<int, ForceConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ForceConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new ForceConfig(1, "刘备", 4, 100001, 200, 10, 10, 10, 10, "#387800");
            config[2] = new ForceConfig(2, "曹操", 1, 100002, 200, 10, 10, 10, 10, "#2828E9");
            config[3] = new ForceConfig(3, "孙策", 2, 100003, 200, 10, 10, 10, 10, "#D10028");
            config[4] = new ForceConfig(4, "袁绍", 3, 100004, 200, 10, 10, 10, 10, "#DBD33A");
            config[5] = new ForceConfig(5, "董卓", 3, 100005, 200, 10, 10, 10, 10, "#646464");
            config[6] = new ForceConfig(6, "马腾", 4, 100006, 200, 10, 10, 10, 10, "#B28500");
            config[7] = new ForceConfig(7, "刘表", 3, 100007, 200, 10, 10, 10, 10, "#20DFE0");
            config[8] = new ForceConfig(8, "刘璋", 3, 100008, 200, 10, 10, 10, 10, "#200070");
            config[9] = new ForceConfig(9, "张鲁", 5, 100009, 200, 10, 10, 10, 10, "#A1D487");
            config[10] = new ForceConfig(10, "袁术", 4, 100010, 200, 10, 10, 10, 10, "#F69CB2");
            config[11] = new ForceConfig(11, "公孙瓒", 5, 100011, 200, 10, 10, 10, 10, "#F36B20");
            config[12] = new ForceConfig(12, "公孙度", 5, 100012, 200, 10, 10, 10, 10, "#A385AD");
            config[99] = new ForceConfig(99, "在野", 0, 100020, 200, 10, 10, 10, 10, "#666666");



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
