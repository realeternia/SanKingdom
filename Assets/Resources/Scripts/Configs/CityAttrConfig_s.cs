using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityAttrConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public string name;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///最大值
        /// </summary>
        public int ValMax;
        /// <summary>
        ///告警值
        /// </summary>
        public int ValLow;
        /// <summary>
        ///告警值
        /// </summary>
        public int ValLow2;


        public CityAttrConfig(int Id, string name, string Cname, int ValMax, int ValLow, int ValLow2)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.ValMax = ValMax;
            this.ValLow = ValLow;
            this.ValLow2 = ValLow2;

        }

        public CityAttrConfig() { }

        private static Dictionary<int, CityAttrConfig> config = new Dictionary<int, CityAttrConfig>();
        public static Dictionary<int, CityAttrConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityAttrConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new CityAttrConfig(1, "archgold", "商业", 999, 200, 0);
            config[2] = new CityAttrConfig(2, "archfood", "农业", 999, 200, 0);
            config[3] = new CityAttrConfig(3, "archpeople", "人口", 99999, 10000, 0);
            config[4] = new CityAttrConfig(4, "gold", "金钱", 99999, 500, 250);
            config[5] = new CityAttrConfig(5, "food", "粮食", 99999, 500, 250);
            config[6] = new CityAttrConfig(6, "soldier", "士兵", 19999, 500, 250);
            config[7] = new CityAttrConfig(7, "secure", "治安", 99, 60, 40);
            config[8] = new CityAttrConfig(8, "wall", "城墙", 999, 150, 100);
            config[9] = new CityAttrConfig(9, "power", "士气", 99, 50, 0);


            idxname["archgold"] =  1;
            idxCname["商业"] =  1;
            idxname["archfood"] =  2;
            idxCname["农业"] =  2;
            idxname["archpeople"] =  3;
            idxCname["人口"] =  3;
            idxname["gold"] =  4;
            idxCname["金钱"] =  4;
            idxname["food"] =  5;
            idxCname["粮食"] =  5;
            idxname["soldier"] =  6;
            idxCname["士兵"] =  6;
            idxname["secure"] =  7;
            idxCname["治安"] =  7;
            idxname["wall"] =  8;
            idxCname["城墙"] =  8;
            idxname["power"] =  9;
            idxCname["士气"] =  9;

        }

        public static CityAttrConfig GetConfig(int id)
        {
            CityAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityAttrConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxname = new Dictionary<string, int>();
        public static CityAttrConfig GetConfigByname(string val)        {
            return GetConfig(idxname[val]);        }
        private static Dictionary<string, int> idxCname = new Dictionary<string, int>();
        public static CityAttrConfig GetConfigByCname(string val)        {
            return GetConfig(idxCname[val]);        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityAttrConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityAttrConfig configData)
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
