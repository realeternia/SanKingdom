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
        ///显示属性
        /// </summary>
        public string DevAttr1;
        /// <summary>
        ///提升值
        /// </summary>
        public int[] DevAttr1Value;
        /// <summary>
        ///显示属性
        /// </summary>
        public string DevAttr2;
        /// <summary>
        ///提升值
        /// </summary>
        public int[] DevAttr2Value;
        /// <summary>
        ///显示属性
        /// </summary>
        public string DevAttr3;
        /// <summary>
        ///提升值
        /// </summary>
        public int[] DevAttr3Value;
        /// <summary>
        ///显示属性
        /// </summary>
        public string[] Attrs;
        /// <summary>
        ///动画文件
        /// </summary>
        public string Mp4;


        public CityDevConfig(int Id, string BuildingName, string Cname, string Des, string Icon, int GoldCost, string DevAttr1, int[] DevAttr1Value, string DevAttr2, int[] DevAttr2Value, string DevAttr3, int[] DevAttr3Value, string[] Attrs, string Mp4)
        {
            this.Id = Id;
            this.BuildingName = BuildingName;
            this.Cname = Cname;
            this.Des = Des;
            this.Icon = Icon;
            this.GoldCost = GoldCost;
            this.DevAttr1 = DevAttr1;
            this.DevAttr1Value = DevAttr1Value;
            this.DevAttr2 = DevAttr2;
            this.DevAttr2Value = DevAttr2Value;
            this.DevAttr3 = DevAttr3;
            this.DevAttr3Value = DevAttr3Value;
            this.Attrs = Attrs;
            this.Mp4 = Mp4;

        }

        public CityDevConfig() { }

        private static Dictionary<int, CityDevConfig> config = new Dictionary<int, CityDevConfig>();
        public static Dictionary<int, CityDevConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityDevConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[21001] = new CityDevConfig(21001, "farm", "发展农业", "发展农业,提升粮食产量", "farm", 300, "ArchFood", new int[]{4,10}, "ArchPeople", new int[]{300,800}, "", null, new string[]{"Fair","Str"}, "harve.mp4");
            config[21002] = new CityDevConfig(21002, "farm", "发展商业", "发展商业,提升金钱收入", "market", 300, "ArchGold", new int[]{4,10}, "ArchPeople", new int[]{300,800}, "", null, new string[]{"Fair","Inte"}, "shop2.mp4");
            config[21003] = new CityDevConfig(21003, "gate", "加固城墙", "提升城防", "wall", 200, "Wall", new int[]{6,15}, "Secure", new int[]{1,3}, "", null, new string[]{"Str"}, "fix2.mp4");
            config[21004] = new CityDevConfig(21004, "farm", "街道巡逻", "提升治安", "secure", 150, "Secure", new int[]{2,6}, "ArchPeople", new int[]{200,500}, "", null, new string[]{"Str"}, "secure.mp4");
            config[21005] = new CityDevConfig(21005, "market", "走访", "搜集人才和宝物", "find", 0, "Gold", new int[]{20,100}, "", null, "", null, new string[]{"Charm","Inte"}, "search.mp4");
            config[21006] = new CityDevConfig(21006, "gate", "训练", "提升军队士气", "train", 0, "Power", new int[]{3,9}, "", null, "", null, new string[]{"LeadShip","Str"}, "train2.mp4");
            config[21007] = new CityDevConfig(21007, "gate", "征兵", "提升士兵数量", "zhengbing", 300, "Soldier", new int[]{200,600}, "Secure", new int[]{-2,-5}, "ArchPeople", new int[]{-500,-1500}, new string[]{"LeadShip","Charm"}, "zhengbing.mp4");
            config[21008] = new CityDevConfig(21008, "gate", "出战", "出兵攻打敌人", "zhengbing", 300, "Soldier", new int[]{200,600}, "Secure", new int[]{-2,-5}, "ArchPeople", new int[]{-500,-1500}, new string[]{"LeadShip","Charm"}, "zhengbing.mp4");



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
