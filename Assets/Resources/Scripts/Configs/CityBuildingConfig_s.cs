using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityBuildingConfig
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
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///显示属性
        /// </summary>
        public string[] Attrs;
        /// <summary>
        ///动画文件
        /// </summary>
        public string Mp4;


        public CityBuildingConfig(int Id, string Name, string Cname, string[] Attrs, string Mp4)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.Attrs = Attrs;
            this.Mp4 = Mp4;

        }

        public CityBuildingConfig() { }

        private static Dictionary<int, CityBuildingConfig> config = new Dictionary<int, CityBuildingConfig>();
        public static Dictionary<int, CityBuildingConfig>.ValueCollection ConfigList
        {
            get
            {
                return config.Values;
            }
        }

        public static void Refresh(Dictionary<int, CityBuildingConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[20001] = new CityBuildingConfig(20001, "ButtonTian", "农田", new string[]{"Fair","Str"}, "harve.mp4");
            config[20002] = new CityBuildingConfig(20002, "ButtonJishi", "集市", new string[]{"Fair","Inte"}, "shop2.mp4");

        }

        public static CityBuildingConfig GetConfig(int id)
        {
            CityBuildingConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityBuildingConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityBuildingConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityBuildingConfig configData)
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
