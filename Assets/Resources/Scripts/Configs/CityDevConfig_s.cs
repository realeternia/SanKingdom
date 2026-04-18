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
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///对象
        /// </summary>
        public string Prefab;
        /// <summary>
        ///图片
        /// </summary>
        public string Icon;
        /// <summary>
        ///人均消耗黄金
        /// </summary>
        public int GoldCost;
        /// <summary>
        ///最大参与人数
        /// </summary>
        public int HeroCount;
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
        ///行动
        /// </summary>
        public string ActionName;
        /// <summary>
        ///发展时ai
        /// </summary>
        public int AiPriotyDev;
        /// <summary>
        ///战时优先级
        /// </summary>
        public int AiPriotyAtk;
        /// <summary>
        ///战时优先级
        /// </summary>
        public int AiPriotyDef;
        /// <summary>
        ///动画文件
        /// </summary>
        public string Mp4;


        public CityDevConfig(int Id, string Cname, string Des, string Prefab, string Icon, int GoldCost, int HeroCount, string DevAttr1, int[] DevAttr1Value, string DevAttr2, int[] DevAttr2Value, string DevAttr3, int[] DevAttr3Value, string[] Attrs, string ActionName, int AiPriotyDev, int AiPriotyAtk, int AiPriotyDef, string Mp4)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.Des = Des;
            this.Prefab = Prefab;
            this.Icon = Icon;
            this.GoldCost = GoldCost;
            this.HeroCount = HeroCount;
            this.DevAttr1 = DevAttr1;
            this.DevAttr1Value = DevAttr1Value;
            this.DevAttr2 = DevAttr2;
            this.DevAttr2Value = DevAttr2Value;
            this.DevAttr3 = DevAttr3;
            this.DevAttr3Value = DevAttr3Value;
            this.Attrs = Attrs;
            this.ActionName = ActionName;
            this.AiPriotyDev = AiPriotyDev;
            this.AiPriotyAtk = AiPriotyAtk;
            this.AiPriotyDef = AiPriotyDef;
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
            config[21001] = new CityDevConfig(21001, "发展农业", "发展农业,提升粮食产量", "CityDevNormal", "farm", 300, 3, "Food", new int[]{5,10}, "Exp", new int[]{2,5}, "", null, new string[]{"Fair","Str"}, "dev", 11, 0, 0, "farm.mp4");
            config[21002] = new CityDevConfig(21002, "发展商业", "发展商业,提升金钱收入", "CityDevNormal", "market", 300, 3, "Gold", new int[]{5,10}, "Exp", new int[]{2,5}, "", null, new string[]{"Fair","Inte"}, "dev", 10, 0, 0, "shop3.mp4");
            config[21101] = new CityDevConfig(21101, "加固城墙", "提升城防", "CityDevNormal", "wall", 200, 3, "Wall", new int[]{5,10}, "Exp", new int[]{2,5}, "", null, new string[]{"Str"}, "def", 30, 0, 10, "fix4.mp4");
            config[21102] = new CityDevConfig(21102, "移动", "移动到其他城市", "CityDevMove", "move", 0, 10, "", null, "", null, "", null, new string[]{"LeadShip","Charm"}, "", 28, 4, 9, "move2.mp4");
            config[21103] = new CityDevConfig(21103, "出战", "出兵攻打敌人", "CityDevBattle", "battle", 0, 10, "", null, "", null, "", null, new string[]{"LeadShip","Str"}, "", 0, 1, 0, "atk2.mp4");
            config[21202] = new CityDevConfig(21202, "走访", "搜集人才和宝物", "CityDevNormal", "find", 0, 10, "Gold", new int[]{1,2}, "", null, "", null, new string[]{"Charm","Inte"}, "find", 21, 0, 11, "search2.mp4");
            config[21203] = new CityDevConfig(21203, "交易", "交易钱粮", "CityDevChange", "change", 0, 1, "Gold", null, "Food", null, "", null, new string[]{"Inte"}, "", 1, 3, 1, "change.mp4");
            config[21301] = new CityDevConfig(21301, "训练", "提升军队士气", "CityDevNormal", "train", 0, 3, "Power", new int[]{3,9}, "", null, "", null, new string[]{"LeadShip","Str"}, "sod", 0, 9, 3, "train3.mp4");
            config[21302] = new CityDevConfig(21302, "征兵", "提升士兵数量", "CityDevNormal", "zhengbing", 300, 3, "Soldier", new int[]{5,10}, "Exp", new int[]{2,5}, "", null, new string[]{"LeadShip","Charm"}, "sod", 0, 8, 2, "zhengbing2.mp4");
            config[21401] = new CityDevConfig(21401, "登用", "提拔在野武将", "CityDevUseHero", "wild", 0, 1, "", null, "", null, "", null, new string[]{"Charm","Inte"}, "", 2, 0, 0, "wild.mp4");
            config[21402] = new CityDevConfig(21402, "褒奖", "提升武将忠心度", "CityDevPraiseHero", "praise", 100, 10, "", null, "", null, "", null, new string[]{"Charm","Inte"}, "", 3, 12, 0, "wild.mp4");



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
