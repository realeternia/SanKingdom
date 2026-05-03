using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityDevConfig
    {
        public class FieldMetaInfo
        {
            public string fieldName;
            public string fieldType;
            public int fieldWidth;
            public string fieldRule;
            public bool fieldIndex;
            public FieldMetaInfo(string name, string type, int width = 0, string rule = "", bool index = false)
            {
                fieldName = name;
                fieldType = type;
                fieldWidth = width;
                fieldRule = rule;
                fieldIndex = index;
            }
        }

        public class CellMeta
        {
            public int row;
            public int col;
            public int? foreColor;
            public int? backColor;
            public CellMeta(int row, int col, int? foreColor, int? backColor)
            {
                this.row = row;
                this.col = col;
                this.foreColor = foreColor;
                this.backColor = backColor;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int", 60)},
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"Des", new FieldMetaInfo("描述", "string", 0)},
            {"Type", new FieldMetaInfo("类型", "string", 0)},
            {"KingAction", new FieldMetaInfo("国王行动", "bool", 0)},
            {"Prefab", new FieldMetaInfo("对象", "string", 153)},
            {"Icon", new FieldMetaInfo("图片", "string", 0)},
            {"GoldCost", new FieldMetaInfo("人均消耗黄金", "int", 60)},
            {"HeroCount", new FieldMetaInfo("最大参与人数", "int", 60)},
            {"DevAttr1", new FieldMetaInfo("显示属性", "string", 0)},
            {"DevAttr1Value", new FieldMetaInfo("提升值", "float[]", 0)},
            {"DevAttr2", new FieldMetaInfo("显示属性", "string", 0)},
            {"DevAttr2Value", new FieldMetaInfo("提升值", "float[]", 0)},
            {"SpecialAction", new FieldMetaInfo("显示属性", "string", 0)},
            {"SpecialActionVal", new FieldMetaInfo("提升值", "int[]", 0)},
            {"Attrs", new FieldMetaInfo("显示属性", "string[]", 0)},
            {"ActionName", new FieldMetaInfo("行动", "string", 0)},
            {"AiPriotyDev", new FieldMetaInfo("发展时ai", "int", 60)},
            {"AiPriotyAtk", new FieldMetaInfo("战时优先级", "int", 60)},
            {"AiPriotyDef", new FieldMetaInfo("战时优先级", "int", 60)},
            {"Mp4", new FieldMetaInfo("动画文件", "string", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

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
        ///类型
        /// </summary>
        public string Type;
        /// <summary>
        ///国王行动
        /// </summary>
        public bool KingAction;
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
        public float[] DevAttr1Value;
        /// <summary>
        ///显示属性
        /// </summary>
        public string DevAttr2;
        /// <summary>
        ///提升值
        /// </summary>
        public float[] DevAttr2Value;
        /// <summary>
        ///显示属性
        /// </summary>
        public string SpecialAction;
        /// <summary>
        ///提升值
        /// </summary>
        public int[] SpecialActionVal;
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


        public CityDevConfig(int Id, string Cname, string Des, string Type, bool KingAction, string Prefab, string Icon, int GoldCost, int HeroCount, string DevAttr1, float[] DevAttr1Value, string DevAttr2, float[] DevAttr2Value, string SpecialAction, int[] SpecialActionVal, string[] Attrs, string ActionName, int AiPriotyDev, int AiPriotyAtk, int AiPriotyDef, string Mp4)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.Des = Des;
            this.Type = Type;
            this.KingAction = KingAction;
            this.Prefab = Prefab;
            this.Icon = Icon;
            this.GoldCost = GoldCost;
            this.HeroCount = HeroCount;
            this.DevAttr1 = DevAttr1;
            this.DevAttr1Value = DevAttr1Value;
            this.DevAttr2 = DevAttr2;
            this.DevAttr2Value = DevAttr2Value;
            this.SpecialAction = SpecialAction;
            this.SpecialActionVal = SpecialActionVal;
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
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[21001] = new CityDevConfig(21001, "农田", "提升粮食产量", "normal", false, "CityDevNormal", "farm", 0, 1, "food", new float[]{3f,2.7f,2.3f,2f}, "exp", new float[]{1f,0.8f,0.6f,0.5f}, "", new int[0], new string[]{"Fair","Str"}, "dev", 11, 0, 0, "farm.mp4");
            config[21002] = new CityDevConfig(21002, "市场", "提升金钱收入", "normal", false, "CityDevNormal", "market", 0, 1, "gold", new float[]{3f,2.7f,2.3f,2f}, "exp", new float[]{1f,0.8f,0.6f,0.5f}, "", new int[0], new string[]{"Fair","Inte"}, "dev", 10, 0, 0, "shop3.mp4");
            config[21003] = new CityDevConfig(21003, "征兵", "提升士兵数量", "normal", false, "CityDevNormal", "zhengbing", 10, 1, "soldier", new float[]{3f,2.7f,2.3f,2f}, "exp", new float[]{1f,0.8f,0.6f,0.5f}, "", new int[0], new string[]{"LeadShip","Charm"}, "sod", 0, 8, 2, "zhengbing2.mp4");
            config[21004] = new CityDevConfig(21004, "加固城墙", "提升城防", "normal", false, "CityDevNormal", "wall", 10, 1, "wall", new float[]{3f,2.7f,2.3f,2f}, "exp", new float[]{1f,0.8f,0.6f,0.5f}, "", new int[0], new string[]{"Str"}, "def", 30, 0, 10, "fix4.mp4");
            config[21005] = new CityDevConfig(21005, "训练", "提升军队士气", "normal", false, "CityDevNormal", "train", 0, 1, "happy", new float[]{3f,2.7f,2.3f,2f}, "", new float[0], "", new int[0], new string[]{"LeadShip","Str"}, "sod", 0, 9, 3, "train3.mp4");
            config[21102] = new CityDevConfig(21102, "移动", "移动到其他城市", "run", true, "CityDevMove", "move", 0, 10, "", new float[0], "", new float[0], "", new int[0], new string[]{"LeadShip","Charm"}, "", 28, 4, 9, "move2.mp4");
            config[21103] = new CityDevConfig(21103, "出战", "出兵攻打敌人", "run", true, "CityDevBattle", "battle", 0, 10, "", new float[0], "", new float[0], "", new int[0], new string[]{"LeadShip","Str"}, "", 0, 1, 0, "atk2.mp4");
            config[21202] = new CityDevConfig(21202, "走访", "搜集人才和宝物", "normal", false, "CityDevNormal", "find", 0, 1, "gold", new float[]{1f,0.8f,0.6f,0.5f}, "", new float[0], "", new int[0], new string[]{"Charm","Inte"}, "find", 21, 0, 11, "search2.mp4");
            config[21203] = new CityDevConfig(21203, "交易", "交易钱粮", "normal", false, "CityDevChange", "change", 0, 1, "", new float[0], "", new float[0], "", new int[0], new string[]{"Inte"}, "", 1, 3, 1, "change.mp4");
            config[21401] = new CityDevConfig(21401, "登用", "提拔在野武将", "run", true, "CityDevUseHero", "wild", 0, 1, "", new float[0], "", new float[0], "", new int[0], new string[]{"Charm","Inte"}, "", 2, 0, 0, "wild.mp4");
            config[21402] = new CityDevConfig(21402, "褒奖", "提升武将忠心度", "run", true, "CityDevPraiseHero", "praise", 10, 5, "", new float[0], "", new float[0], "praise", new int[]{1,5}, new string[]{"Charm","Inte"}, "", 3, 12, 0, "wild.mp4");
            config[21403] = new CityDevConfig(21403, "采石场", "提升石料", "normal", false, "", "stone", 0, 1, "stone", new float[]{3f,2.7f,2.3f,2f}, "", new float[0], "", new int[0], new string[]{"Charm","Inte"}, "", 0, 0, 0, "");
            config[21404] = new CityDevConfig(21404, "马场", "提升马场", "normal", false, "", "horse", 0, 1, "horse", new float[]{3f,2.7f,2.3f,2f}, "", new float[0], "", new int[0], new string[]{"Charm","Inte"}, "", 0, 0, 0, "");
            config[21405] = new CityDevConfig(21405, "木材场", "提升木材", "normal", false, "", "wood", 0, 1, "wood", new float[]{3f,2.7f,2.3f,2f}, "", new float[0], "", new int[0], new string[]{"Charm","Inte"}, "", 0, 0, 0, "");
            config[21406] = new CityDevConfig(21406, "铁匠铺", "提升铁", "normal", false, "", "steel", 0, 1, "steel", new float[]{3f,2.7f,2.3f,2f}, "", new float[0], "", new int[0], new string[]{"Charm","Inte"}, "", 0, 0, 0, "");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
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
