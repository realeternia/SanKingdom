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
        ///是否force
        /// </summary>
        public bool IsForceAttr;
        /// <summary>
        ///占用类资源
        /// </summary>
        public bool IsPosRes;
        /// <summary>
        ///最大值
        /// </summary>
        public int ValMaxCity;
        /// <summary>
        ///最大值
        /// </summary>
        public int ValMaxForce;
        /// <summary>
        ///icon
        /// </summary>
        public string Icon;


        public CityAttrConfig(int Id, string name, string Cname, bool IsForceAttr, bool IsPosRes, int ValMaxCity, int ValMaxForce, string Icon)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.IsForceAttr = IsForceAttr;
            this.IsPosRes = IsPosRes;
            this.ValMaxCity = ValMaxCity;
            this.ValMaxForce = ValMaxForce;
            this.Icon = Icon;

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
            config[1] = new CityAttrConfig(1, "level", "等级", false, false, 99, 0, "");
            config[2] = new CityAttrConfig(2, "exp", "发展度", false, false, 999, 0, "citydev");
            config[5] = new CityAttrConfig(5, "food", "粮食", false, false, 999, 0, "cityfood");
            config[6] = new CityAttrConfig(6, "soldier", "士兵", false, false, 999, 0, "citysod");
            config[7] = new CityAttrConfig(7, "happy", "民心", false, false, 999, 0, "cityheart");
            config[8] = new CityAttrConfig(8, "wall", "城墙", false, false, 999, 0, "citywall");
            config[12] = new CityAttrConfig(12, "gold", "金钱", true, false, 0, 999, "citygold");
            config[13] = new CityAttrConfig(13, "steel", "铁", true, true, 0, 999, "citysteel");
            config[14] = new CityAttrConfig(14, "horse", "马", true, true, 0, 999, "cityhorse");
            config[15] = new CityAttrConfig(15, "wood", "木材", true, true, 0, 999, "citywood");
            config[16] = new CityAttrConfig(16, "stone", "石料", true, true, 0, 999, "citystone");


            idxname["level"] =  1;
            idxCname["等级"] =  1;
            idxname["exp"] =  2;
            idxCname["发展度"] =  2;
            idxname["food"] =  5;
            idxCname["粮食"] =  5;
            idxname["soldier"] =  6;
            idxCname["士兵"] =  6;
            idxname["happy"] =  7;
            idxCname["民心"] =  7;
            idxname["wall"] =  8;
            idxCname["城墙"] =  8;
            idxname["gold"] =  12;
            idxCname["金钱"] =  12;
            idxname["steel"] =  13;
            idxCname["铁"] =  13;
            idxname["horse"] =  14;
            idxCname["马"] =  14;
            idxname["wood"] =  15;
            idxCname["木材"] =  15;
            idxname["stone"] =  16;
            idxCname["石料"] =  16;

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
