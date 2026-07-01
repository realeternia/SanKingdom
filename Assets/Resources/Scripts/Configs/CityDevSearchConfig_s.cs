using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityDevSearchConfig
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
            {"Des", new FieldMetaInfo("描述", "string", 0)},
            {"ResType", new FieldMetaInfo("资源类型", "string", 97)},
            {"ResId", new FieldMetaInfo("资源id", "int", 60)},
            {"Weight", new FieldMetaInfo("加权系数", "float", 60)},
            {"Condition", new FieldMetaInfo("条件", "string", 184)},
            {"AttrValMin", new FieldMetaInfo("最小值", "int", 60)},
            {"AttrValMax", new FieldMetaInfo("最大值", "int", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///资源类型(cityattr/forceattr/findhero/findherostar)
        /// </summary>
        public string ResType;
        /// <summary>
        ///资源id(CityAttrConfig.Id)
        /// </summary>
        public int ResId;
        /// <summary>
        ///加权系数
        /// </summary>
        public float Weight;
        /// <summary>
        ///条件(如"inte>=90")
        /// </summary>
        public string Condition;
        /// <summary>
        ///最小值
        /// </summary>
        public int AttrValMin;
        /// <summary>
        ///最大值
        /// </summary>
        public int AttrValMax;


        public CityDevSearchConfig(int Id, string Des, string ResType, int ResId, float Weight, string Condition, int AttrValMin, int AttrValMax)
        {
            this.Id = Id;
            this.Des = Des;
            this.ResType = ResType;
            this.ResId = ResId;
            this.Weight = Weight;
            this.Condition = Condition;
            this.AttrValMin = AttrValMin;
            this.AttrValMax = AttrValMax;
        }

        public CityDevSearchConfig() { }

        private static Dictionary<int, CityDevSearchConfig> config = new Dictionary<int, CityDevSearchConfig>();
        public static Dictionary<int, CityDevSearchConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityDevSearchConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[22001] = new CityDevSearchConfig(22001, "发现金钱", "forceattr", 12, 1.0f, "", 5, 15);
            config[22002] = new CityDevSearchConfig(22002, "发现金钱", "forceattr", 12, 0.5f, "inte>=80", 5, 15);
            config[22003] = new CityDevSearchConfig(22003, "发现金钱", "forceattr", 12, 0.3f, "inte>=90", 5, 15);

            config[22004] = new CityDevSearchConfig(22004, "发现粮食", "cityattr", 5, 0.7f, "", 5, 15);
            config[22005] = new CityDevSearchConfig(22005, "发现粮食", "cityattr", 5, 0.4f, "inte>=80", 5, 15);

            config[22006] = new CityDevSearchConfig(22006, "发现士兵", "cityattr", 6, 0.4f, "", 5, 12);
            config[22007] = new CityDevSearchConfig(22007, "发现士兵", "cityattr", 6, 0.4f, "leadShip>=90", 5, 12);

            config[22008] = new CityDevSearchConfig(22008, "发现将领", "findhero", 0, 0.3f, "", 0, 0);
            config[22009] = new CityDevSearchConfig(22009, "发现将领", "findhero", 0, 0.3f, "inte>=80", 0, 0);

            config[22010] = new CityDevSearchConfig(22010, "发现名将", "findherostar", 0, 0.1f, "", 0, 0);
            config[22011] = new CityDevSearchConfig(22011, "发现名将", "findherostar", 0, 0.2f, "inte>=90", 0, 0);

            RebuildIndex();
        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static CityDevSearchConfig GetConfig(int id)
        {
            CityDevSearchConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityDevSearchConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityDevSearchConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityDevSearchConfig configData)
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
