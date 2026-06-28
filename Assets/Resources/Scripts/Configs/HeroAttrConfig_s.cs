using System;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroAttrConfig
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
            {"name", new FieldMetaInfo("名字", "string", 101, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"TextRule", new FieldMetaInfo("输出规则", "string", 345)},
            {"ColorRule", new FieldMetaInfo("颜色规则", "string", 371)},
            {"Icon", new FieldMetaInfo("icon", "string", 0)},
            {"IsArmsAttr", new FieldMetaInfo("是否兵种属性", "bool", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        public int Id;
        public string name;
        public string Cname;
        /// <summary>
        ///输出规则
        /// </summary>
        public string TextRule;
        public string ColorRule;
        public string Icon;
        public bool IsArmsAttr;


        public HeroAttrConfig(int Id, string name, string Cname, string TextRule, string ColorRule, string Icon, bool IsArmsAttr)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.TextRule = TextRule;
            this.ColorRule = ColorRule;
            this.Icon = Icon;
            this.IsArmsAttr = IsArmsAttr;
        }

        public HeroAttrConfig() { }

        private static Dictionary<int, HeroAttrConfig> config = new Dictionary<int, HeroAttrConfig>();
        public static Dictionary<int, HeroAttrConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroAttrConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new HeroAttrConfig(1, "str", "武力", "", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "herostr", false);
            config[2] = new HeroAttrConfig(2, "inte", "智力", "", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "herointe", false);
            config[3] = new HeroAttrConfig(3, "fair", "内政", "", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "herofair", false);
            config[4] = new HeroAttrConfig(4, "charm", "魅力", "", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "herocharm", false);
            config[5] = new HeroAttrConfig(5, "leadShip", "统帅", "", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "herolead", false);
            config[6] = new HeroAttrConfig(6, "loyalty", "忠诚度", "", "90:#FFFFFF,80-89:#FFFF00,70-79:#FFA500,0-69:#FF0000", "", false);
            config[7] = new HeroAttrConfig(7, "weightedAttr", "加权属性2", "", "90:#FF0000,80:#FFFF00,70:#00FF00,0:#FFFFFF", "", false);
            config[8] = new HeroAttrConfig(8, "SodWalk", "步兵驾驭", "10:S,9:A+,8:A,7:B+,6:B,4-5:C+,2-3:C,0-1:D", "10:#FF9900,8-9:#995500,6-7:#33CC33,4-5:#3333CC", "arms1", true);
            config[9] = new HeroAttrConfig(9, "SodHorse", "骑兵驾驭", "10:S,9:A+,8:A,7:B+,6:B,4-5:C+,2-3:C,0-1:D", "10:#FF9900,8-9:#995500,6-7:#33CC33,4-5:#3333CC", "arms2", true);
            config[10] = new HeroAttrConfig(10, "SodBow", "弓兵驾驭", "10:S,9:A+,8:A,7:B+,6:B,4-5:C+,2-3:C,0-1:D", "10:#FF9900,8-9:#995500,6-7:#33CC33,4-5:#3333CC", "arms3", true);
            config[11] = new HeroAttrConfig(11, "SodWater", "水军驾驭", "10:S,9:A+,8:A,7:B+,6:B,4-5:C+,2-3:C,0-1:D", "10:#FF9900,8-9:#995500,6-7:#33CC33,4-5:#3333CC", "arms4", true);
            config[12] = new HeroAttrConfig(12, "SodTank", "车炮驾驭", "10:S,9:A+,8:A,7:B+,6:B,4-5:C+,2-3:C,0-1:D", "10:#FF9900,8-9:#995500,6-7:#33CC33,4-5:#3333CC", "arms5", true);
            config[13] = new HeroAttrConfig(13, "atk", "攻击", "", "", "armsatk", false);
            config[14] = new HeroAttrConfig(14, "def", "防御", "", "", "armsdef", false);
            config[15] = new HeroAttrConfig(15, "sodnum", "士兵数", "", "", "armscount", false);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            idxname.Clear();
            foreach (var kv in config)
            {
                if (!string.IsNullOrEmpty(kv.Value.name)) idxname[kv.Value.name] = kv.Key;
            }
        }

        public static HeroAttrConfig GetConfig(int id)
        {
            HeroAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroAttrConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxname = new Dictionary<string, int>();
        public static HeroAttrConfig GetConfigByname(string val)
        {
            return GetConfig(idxname[val]);
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroAttrConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, HeroAttrConfig configData)
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
