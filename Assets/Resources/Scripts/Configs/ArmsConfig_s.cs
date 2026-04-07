using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ArmsConfig
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
        ///名字
        /// </summary>
        public string NameS;
        /// <summary>
        ///强克制
        /// </summary>
        public string OvercomeStrong;
        /// <summary>
        ///弱克制
        /// </summary>
        public string OvercomeWeak;


        public ArmsConfig(int Id, string Name, string NameS, string OvercomeStrong, string OvercomeWeak)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.OvercomeStrong = OvercomeStrong;
            this.OvercomeWeak = OvercomeWeak;

        }

        public ArmsConfig() { }

        private static Dictionary<int, ArmsConfig> config = new Dictionary<int, ArmsConfig>();
        public static Dictionary<int, ArmsConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ArmsConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[101] = new ArmsConfig(101, "ma", "马", "戟弩炮车", "弓");
            config[201] = new ArmsConfig(201, "gong", "弓", "枪戟", "刀");
            config[601] = new ArmsConfig(601, "dao", "刀", "盾", "士");
            config[602] = new ArmsConfig(602, "daoqiang", "枪", "马车", "");
            config[603] = new ArmsConfig(603, "daoji", "戟", "枪", "");



        }

        public static ArmsConfig GetConfig(int id)
        {
            ArmsConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ArmsConfig不存在id={0}", id));
        }



        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ArmsConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ArmsConfig configData)
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
