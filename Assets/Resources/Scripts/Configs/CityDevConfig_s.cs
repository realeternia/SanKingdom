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
            {"Name", new FieldMetaInfo("英文名", "string", 0, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"Des", new FieldMetaInfo("描述", "string", 0)},
            {"Type", new FieldMetaInfo("类型", "string", 0)},
            {"IsSpecial", new FieldMetaInfo("特有建筑", "bool", 0)},
            {"KingAction", new FieldMetaInfo("国王行动", "bool", 0)},
            {"Action", new FieldMetaInfo("对象", "string", 97)},
            {"Icon", new FieldMetaInfo("图片", "string", 0)},
            {"GoldCost", new FieldMetaInfo("人均消耗黄金", "int", 60)},
            {"HeroCount", new FieldMetaInfo("最大参与人数", "int", 60)},
            {"DevAttr1", new FieldMetaInfo("显示属性", "string", 0)},
            {"DevAttr1Value", new FieldMetaInfo("提升值", "float[]", 0)},
            {"DevAttr2", new FieldMetaInfo("显示属性", "string", 0)},
            {"DevAttr2Value", new FieldMetaInfo("提升值", "float[]", 0)},
            {"Attrs", new FieldMetaInfo("显示属性", "string[]", 0)},
            {"ActionName", new FieldMetaInfo("行动", "string", 0)},
            {"AiWeightDev", new FieldMetaInfo("发展随机权重", "float", 60)},
            {"AiWeightAtk", new FieldMetaInfo("进攻随机权重", "float", 60)},
            {"AiWeightDef", new FieldMetaInfo("防御随机权重", "float", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///英文名
        /// </summary>
        public string Name;
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
        ///特有建筑
        /// </summary>
        public bool IsSpecial;
        /// <summary>
        ///国王行动
        /// </summary>
        public bool KingAction;
        /// <summary>
        ///对象
        /// </summary>
        public string Action;
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
        public string[] Attrs;
        /// <summary>
        ///行动
        /// </summary>
        public string ActionName;
        /// <summary>
        ///发展随机权重
        /// </summary>
        public float AiWeightDev;
        /// <summary>
        ///进攻随机权重
        /// </summary>
        public float AiWeightAtk;
        /// <summary>
        ///防御随机权重
        /// </summary>
        public float AiWeightDef;


        public CityDevConfig(int Id, string Name, string Cname, string Des, string Type, bool IsSpecial, bool KingAction, string Action, string Icon, int GoldCost, int HeroCount, string DevAttr1, float[] DevAttr1Value, string DevAttr2, float[] DevAttr2Value, string[] Attrs, string ActionName, float AiWeightDev, float AiWeightAtk, float AiWeightDef)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.Des = Des;
            this.Type = Type;
            this.IsSpecial = IsSpecial;
            this.KingAction = KingAction;
            this.Action = Action;
            this.Icon = Icon;
            this.GoldCost = GoldCost;
            this.HeroCount = HeroCount;
            this.DevAttr1 = DevAttr1;
            this.DevAttr1Value = DevAttr1Value;
            this.DevAttr2 = DevAttr2;
            this.DevAttr2Value = DevAttr2Value;
            this.Attrs = Attrs;
            this.ActionName = ActionName;
            this.AiWeightDev = AiWeightDev;
            this.AiWeightAtk = AiWeightAtk;
            this.AiWeightDef = AiWeightDef;
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
            config[21001] = new CityDevConfig(21001, "Farm", "农田", "提升粮食产量", "normal", false, false, "", "farm", 0, 0, "food", new float[]{12f,10f,8f,5f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Str"}, "dev", 11f, 5f, 10f);
            config[21002] = new CityDevConfig(21002, "Market", "市场", "提升金钱收入", "normal", false, false, "", "market", 0, 0, "gold", new float[]{6f,5f,4f,3f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "dev", 10f, 5f, 5f);
            config[21003] = new CityDevConfig(21003, "Conscript", "征兵", "提升士兵数量", "normal", false, false, "", "zhengbing", 4, 0, "soldier", new float[]{16f,12f,10f,8f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Charm"}, "sod", 5f, 8f, 5f);
            config[21004] = new CityDevConfig(21004, "Fortify", "加固城墙", "提升城防", "normal", false, false, "", "wall", 2, 0, "wall", new float[]{18f,14f,12f,10f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Str"}, "def", 0f, 3f, 5f);
            config[21005] = new CityDevConfig(21005, "Security", "治安", "提升城市治安", "normal", false, false, "", "train", 0, 0, "happy", new float[]{5f,4f,3f,2f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Str"}, "sod", 5f, 2f, 2f);
            config[21006] = new CityDevConfig(21006, "Academy", "研究院", "产出研究值", "normal", false, false, "", "research", 0, 0, "scipoint", new float[]{6f,5f,4f,3f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Inte"}, "dev", 6f, 3f, 5f);
            config[21403] = new CityDevConfig(21403, "Quarry", "采石场", "提升石料", "normal", true, false, "", "stone", 0, 0, "stone", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 0f, 0f, 0f);
            config[21404] = new CityDevConfig(21404, "Stable", "马场", "提升马场", "normal", true, false, "", "horse", 0, 0, "horse", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Str"}, "", 0f, 0f, 0f);
            config[21405] = new CityDevConfig(21405, "Lumberyard", "木材场", "提升木材", "normal", false, false, "", "wood", 0, 0, "wood", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 0f, 0f, 0f);
            config[21406] = new CityDevConfig(21406, "Smithy", "铁匠铺", "提升铁", "normal", false, false, "", "steel", 0, 0, "steel", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 0f, 0f, 0f);
            config[21407] = new CityDevConfig(21407, "ElephantPen", "象棚", "提升战象", "normal", true, false, "", "elephant", 0, 0, "elephant", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Str"}, "", 0f, 0f, 0f);
            config[21408] = new CityDevConfig(21408, "GoldMine", "金矿", "提升金钱", "normal", true, false, "", "gold", 0, 0, "gold", new float[]{8f,6f,5f,4f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 10f, 5f, 8f);
            config[21409] = new CityDevConfig(21409, "SaltMine", "盐矿", "提升盐", "normal", true, false, "", "salt", 0, 0, "gold", new float[]{3f,2f,2f,1f}, "soldier", new float[]{6f,5f,4f,3f}, new string[]{"Fair","Inte"}, "", 10f, 5f, 8f);
            config[21410] = new CityDevConfig(21410, "Fishery", "渔场", "提升鱼", "normal", true, false, "", "fish", 0, 0, "gold", new float[]{3f,2f,2f,1f}, "food", new float[]{12f,10f,8f,5f}, new string[]{"Fair","Str"}, "", 10f, 5f, 8f);
            config[21501] = new CityDevConfig(21501, "Move", "移动", "移动到其他城市", "run", false, true, "Move", "move", 0, 5, "", new float[0], "", new float[0], new string[]{"LeadShip","Charm"}, "", 0f, 0f, 0f);
            config[21502] = new CityDevConfig(21502, "Battle", "出战", "出兵攻打敌人", "run", false, true, "Battle", "battle", 0, 0, "", new float[0], "", new float[0], new string[]{"LeadShip","Str"}, "", 0f, 0f, 0f);
            config[21601] = new CityDevConfig(21601, "Search", "走访", "搜集人才和宝物", "run", false, true, "Search", "find", 5, 0, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "search", 0f, 0f, 0f);
            config[21602] = new CityDevConfig(21602, "Trade", "交易", "买粮食和士兵", "run", false, true, "Trade", "change", 10, 0, "", new float[0], "", new float[0], new string[]{"Inte"}, "", 0f, 0f, 0f);
            config[21603] = new CityDevConfig(21603, "Recruit", "登用", "提拔在野武将", "run", false, true, "UseHero", "wild", 0, 5, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "", 0f, 0f, 0f);
            config[21604] = new CityDevConfig(21604, "Praise", "褒奖", "提升武将忠心度", "run", false, true, "Praise", "praise", 0, 5, "", new float[0], "", new float[0], null, "", 0f, 0f, 0f);
            config[21605] = new CityDevConfig(21605, "Reward", "奖赏", "提升武将忠心度", "run", false, true, "Praise", "praise", 10, 0, "", new float[0], "", new float[0], null, "", 0f, 0f, 0f);
            config[21606] = new CityDevConfig(21606, "Sabotage", "破坏", "破坏敌人城防", "run", false, true, "EnemyCity", "destroy", 10, 0, "", new float[0], "", new float[0], new string[]{"Str","Inte"}, "", 0f, 0f, 0f);
            config[21607] = new CityDevConfig(21607, "Disturb", "扰乱", "扰乱敌人民心和忠诚度", "run", false, true, "EnemyCity", "destroy", 15, 0, "", new float[0], "", new float[0], new string[]{"Inte"}, "", 0f, 0f, 0f);
            config[21608] = new CityDevConfig(21608, "Diplomacy", "外交", "亲善目标势力", "run", false, true, "Relation", "peace", 30, 0, "", new float[0], "", new float[0], new string[]{"Fair","Inte"}, "", 0f, 0f, 0f);
            config[21609] = new CityDevConfig(21609, "SowDiscord", "挑拨", "降低两个目标势力间的友好度", "run", false, true, "Relation", "peace", 20, 0, "", new float[0], "", new float[0], new string[]{"Fair","Inte"}, "", 0f, 0f, 0f);
            config[21610] = new CityDevConfig(21610, "Research", "研究", "派遣武将研究科技", "run", false, true, "Tech", "tech", 0, 1, "", new float[0], "", new float[0], new string[]{"Inte"}, "", 0f, 0f, 0f);
            config[21999] = new CityDevConfig(21999, "Idle", "赋闲", "免除工作", "normal", false, false, "", "ququ", 0, 0, "", new float[0], "", new float[0], null, "", 0f, 0f, 0f);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxName.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.Name)) idxName[kv.Value.Name] = kv.Key;
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

        private static Dictionary<string, int> idxName = new Dictionary<string, int>();
        public static CityDevConfig GetConfigByName(string val)
        {
            return GetConfig(idxName[val]);
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
