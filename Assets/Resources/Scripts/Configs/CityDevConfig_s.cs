using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityDevConfig
    {
        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///位置
        /// </summary>
        public string BuildingName;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///图片
        /// </summary>
        public string Icon;
        /// <summary>
        ///消耗黄金
        /// </summary>
        public int GoldCost;
        /// <summary>
        ///提升值
        /// </summary>
        public int AddMin;
        /// <summary>
        ///提升最大
        /// </summary>
        public int AddMax;
        /// <summary>
        ///显示属性
        /// </summary>
        public string[] DevAttrs;
        /// <summary>
        ///显示属性
        /// </summary>
        public string[] Attrs;
        /// <summary>
        ///动画文件
        /// </summary>
        public string Mp4;


        public CityDevConfig(int Id, string BuildingName, string Cname, string Des, string Icon, int GoldCost, int AddMin, int AddMax, string[] DevAttrs, string[] Attrs, string Mp4)
        {
            this.Id = Id;
            this.BuildingName = BuildingName;
            this.Cname = Cname;
            this.Des = Des;
            this.Icon = Icon;
            this.GoldCost = GoldCost;
            this.AddMin = AddMin;
            this.AddMax = AddMax;
            this.DevAttrs = DevAttrs;
            this.Attrs = Attrs;
            this.Mp4 = Mp4;

        }

        public CityDevConfig() { }

        private static Dictionary<int, CityDevConfig> config = new Dictionary<int, CityDevConfig>();
        public static Dictionary<int, CityDevConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, CityDevConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[21001] = new CityDevConfig(21001, "farm", "发展农业", "发展农业，提升粮食产量", "farm", 300, 4, 10, new string[]{"ArchFood"}, new string[]{"Fair","Str"}, "harve.mp4");
            config[21002] = new CityDevConfig(21002, "farm", "发展商业", "发展商业，提升金钱收入", "market", 200, 4, 10, new string[]{"ArchGold"}, new string[]{"Fair","Inte"}, "shop2.mp4");

        }

        public static CityDevConfig GetConfig(int id)
        {
            CityDevConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityDevConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityDevConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityDevConfig configData)
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
