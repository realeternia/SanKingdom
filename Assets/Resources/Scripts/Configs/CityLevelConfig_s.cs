using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityLevelConfig
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
            {"Id", new FieldMetaInfo("序列", "int", 0)},
            {"ExpNeed", new FieldMetaInfo("名字", "int", 0)},
            {"GoldAdd", new FieldMetaInfo("黄金产量", "int", 0)},
            {"FoodAdd", new FieldMetaInfo("粮食产量", "int", 0)},
            {"SoldierAdd", new FieldMetaInfo("士兵产量", "int", 0)},
            {"JobCount", new FieldMetaInfo("工位数", "int", 0)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///名字
        /// </summary>
        public int ExpNeed;
        /// <summary>
        ///黄金产量
        /// </summary>
        public int GoldAdd;
        /// <summary>
        ///粮食产量
        /// </summary>
        public int FoodAdd;
        /// <summary>
        ///士兵产量
        /// </summary>
        public int SoldierAdd;
        /// <summary>
        ///工位数
        /// </summary>
        public int JobCount;


        public CityLevelConfig(int Id, int ExpNeed, int GoldAdd, int FoodAdd, int SoldierAdd, int JobCount)
        {
            this.Id = Id;
            this.ExpNeed = ExpNeed;
            this.GoldAdd = GoldAdd;
            this.FoodAdd = FoodAdd;
            this.SoldierAdd = SoldierAdd;
            this.JobCount = JobCount;
        }

        public CityLevelConfig() { }

        private static Dictionary<int, CityLevelConfig> config = new Dictionary<int, CityLevelConfig>();
        public static Dictionary<int, CityLevelConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityLevelConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new CityLevelConfig(1, 30, 10, 10, 10, 4);
            config[2] = new CityLevelConfig(2, 80, 10, 10, 10, 4);
            config[3] = new CityLevelConfig(3, 160, 10, 10, 10, 4);
            config[4] = new CityLevelConfig(4, 260, 10, 10, 10, 4);
            config[5] = new CityLevelConfig(5, 360, 10, 10, 10, 5);
            config[6] = new CityLevelConfig(6, 460, 10, 10, 10, 5);
            config[7] = new CityLevelConfig(7, 560, 10, 10, 10, 5);
            config[8] = new CityLevelConfig(8, 660, 10, 10, 10, 5);
            config[9] = new CityLevelConfig(9, 780, 10, 10, 10, 5);
            config[10] = new CityLevelConfig(10, 920, 11, 11, 11, 6);
            config[11] = new CityLevelConfig(11, 1080, 11, 11, 11, 6);
            config[12] = new CityLevelConfig(12, 1260, 11, 11, 11, 6);
            config[13] = new CityLevelConfig(13, 1460, 11, 11, 11, 6);
            config[14] = new CityLevelConfig(14, 1660, 11, 11, 11, 6);
            config[15] = new CityLevelConfig(15, 1860, 11, 11, 11, 7);
            config[16] = new CityLevelConfig(16, 2060, 11, 11, 11, 7);
            config[17] = new CityLevelConfig(17, 2300, 11, 11, 11, 7);
            config[18] = new CityLevelConfig(18, 2580, 11, 11, 11, 7);
            config[19] = new CityLevelConfig(19, 2980, 11, 11, 11, 7);
            config[20] = new CityLevelConfig(20, 3480, 12, 12, 12, 8);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static CityLevelConfig GetConfig(int id)
        {
            CityLevelConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityLevelConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityLevelConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityLevelConfig configData)
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
