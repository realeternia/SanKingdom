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
        ///绑定
        /// </summary>
        public string BtnName;


        public CityBuildingConfig(int Id, string Name, string Cname, string BtnName)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.BtnName = BtnName;

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
            config[20001] = new CityBuildingConfig(20001, "farm", "发展", "ButtonTian");
            config[20002] = new CityBuildingConfig(20002, "gate", "防御", "ButtonGate");
            config[20004] = new CityBuildingConfig(20004, "market", "集市", "ButtonJishi");

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
