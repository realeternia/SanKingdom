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
        /// <summary>
        ///动画文件
        /// </summary>
        public string Mp4;


        public CityDevConfig(int Id, string Cname, string Des, string Type, bool IsSpecial, bool KingAction, string Action, string Icon, int GoldCost, int HeroCount, string DevAttr1, float[] DevAttr1Value, string DevAttr2, float[] DevAttr2Value, string[] Attrs, string ActionName, float AiWeightDev, float AiWeightAtk, float AiWeightDef, string Mp4)
        {
            this.Id = Id;
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
            config[21001] = new CityDevConfig(21001, "农田", "提升粮食产量", "normal", false, false, "", "farm", 0, 0, "food", new float[]{12f,10f,8f,5f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Str"}, "dev", 11f, 5f, 10f, "");
            config[21002] = new CityDevConfig(21002, "市场", "提升金钱收入", "normal", false, false, "", "market", 0, 0, "gold", new float[]{6f,5f,4f,3f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "dev", 10f, 5f, 5f, "");
            config[21003] = new CityDevConfig(21003, "征兵", "提升士兵数量", "normal", false, false, "", "zhengbing", 4, 0, "soldier", new float[]{16f,12f,10f,8f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Charm"}, "sod", 5f, 8f, 5f, "");
            config[21004] = new CityDevConfig(21004, "加固城墙", "提升城防", "normal", false, false, "", "wall", 2, 0, "wall", new float[]{18f,14f,12f,10f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Str"}, "def", 0f, 3f, 5f, "");
            config[21005] = new CityDevConfig(21005, "治安", "提升城市治安", "normal", false, false, "", "train", 0, 0, "happy", new float[]{5f,4f,3f,2f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Str"}, "sod", 5f, 2f, 2f, "");
            config[21102] = new CityDevConfig(21102, "移动", "移动到其他城市", "run", false, true, "Move", "move", 0, 5, "", new float[0], "", new float[0], new string[]{"LeadShip","Charm"}, "", 0f, 0f, 0f, "move2.mp4");
            config[21103] = new CityDevConfig(21103, "出战", "出兵攻打敌人", "run", false, true, "Battle", "battle", 0, 0, "", new float[0], "", new float[0], new string[]{"LeadShip","Str"}, "", 0f, 0f, 0f, "atk2.mp4");
            config[21202] = new CityDevConfig(21202, "走访", "搜集人才和宝物", "run", false, true, "Search", "find", 5, 0, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "search", 0f, 0f, 0f, "search2.mp4");
            config[21203] = new CityDevConfig(21203, "交易", "买粮食和士兵", "run", false, true, "Trade", "change", 10, 0, "", new float[0], "", new float[0], new string[]{"Inte"}, "", 0f, 0f, 0f, "change.mp4");
            config[21204] = new CityDevConfig(21204, "登用", "提拔在野武将", "run", false, true, "UseHero", "wild", 0, 5, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "", 0f, 0f, 0f, "wild.mp4");
            config[21205] = new CityDevConfig(21205, "褒奖", "提升武将忠心度", "run", false, true, "Praise", "praise", 0, 5, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "", 0f, 0f, 0f, "wild.mp4");
            config[21206] = new CityDevConfig(21206, "奖赏", "提升武将忠心度", "run", false, true, "Praise", "praise", 10, 0, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "", 0f, 0f, 0f, "wild.mp4");
            config[21207] = new CityDevConfig(21207, "破坏", "破坏敌人城防", "run", false, true, "EnemyCity", "destroy", 20, 0, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "", 0f, 0f, 0f, "wild.mp4");
            config[21208] = new CityDevConfig(21208, "扰乱", "扰乱敌人民心和忠诚度", "run", false, true, "EnemyCity", "destroy", 20, 0, "", new float[0], "", new float[0], new string[]{"Charm","Inte"}, "", 0f, 0f, 0f, "wild.mp4");
            config[21403] = new CityDevConfig(21403, "采石场", "提升石料", "normal", true, false, "", "stone", 0, 0, "stone", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 0f, 0f, 0f, "");
            config[21404] = new CityDevConfig(21404, "马场", "提升马场", "normal", true, false, "", "horse", 0, 0, "horse", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Str"}, "", 0f, 0f, 0f, "");
            config[21405] = new CityDevConfig(21405, "木材场", "提升木材", "normal", false, false, "", "wood", 0, 0, "wood", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 0f, 0f, 0f, "");
            config[21406] = new CityDevConfig(21406, "铁匠铺", "提升铁", "normal", false, false, "", "steel", 0, 0, "steel", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 0f, 0f, 0f, "");
            config[21407] = new CityDevConfig(21407, "象棚", "提升战象", "normal", true, false, "", "elephant", 0, 0, "elephant", new float[]{3f,2.4f,2f,1.6f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"LeadShip","Str"}, "", 0f, 0f, 0f, "");
            config[21408] = new CityDevConfig(21408, "金矿", "提升金钱", "normal", true, false, "", "gold", 0, 0, "gold", new float[]{8f,6f,5f,4f}, "exp", new float[]{1.2f,1f,1f,1f}, new string[]{"Fair","Inte"}, "", 10f, 5f, 8f, "");
            config[21409] = new CityDevConfig(21409, "盐矿", "提升盐", "normal", true, false, "", "salt", 0, 0, "gold", new float[]{3f,2f,2f,1f}, "soldier", new float[]{6f,5f,4f,3f}, new string[]{"Fair","Inte"}, "", 10f, 5f, 8f, "");
            config[21410] = new CityDevConfig(21410, "渔场", "提升鱼", "normal", true, false, "", "fish", 0, 0, "gold", new float[]{3f,2f,2f,1f}, "food", new float[]{12f,10f,8f,5f}, new string[]{"Fair","Str"}, "", 10f, 5f, 8f, "");
            config[21999] = new CityDevConfig(21999, "赋闲", "免除工作", "normal", false, false, "", "ququ", 0, 0, "", new float[0], "", new float[0], null, "", 0f, 0f, 0f, "");

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
