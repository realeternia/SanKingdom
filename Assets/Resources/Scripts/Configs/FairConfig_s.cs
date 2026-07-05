using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class FairConfig
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
            {"Name", new FieldMetaInfo("名字", "string", 0, "", true)},
            {"Title", new FieldMetaInfo("标题", "string", 0)},
            {"Des", new FieldMetaInfo("描述模板", "string", 465)},
            {"Filter", new FieldMetaInfo("", "string", 0)},
            {"HappyLimit", new FieldMetaInfo("民心触发线", "int", 0)},
            {"Image", new FieldMetaInfo("图片名", "string", 0)},
            {"Bg", new FieldMetaInfo("背景音乐", "string", 0)},
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
        public string Name;
        /// <summary>
        ///标题
        /// </summary>
        public string Title;
        /// <summary>
        ///描述模板，{forceName} 替换为势力名（富文本颜色）
        /// </summary>
        public string Des;
        public string Filter;
        /// <summary>
        ///民心触发线
        /// </summary>
        public int HappyLimit;
        /// <summary>
        ///图片名，路径 Textures/Fairs/
        /// </summary>
        public string Image;
        /// <summary>
        ///背景音乐名，路径 BGMs/
        /// </summary>
        public string Bg;


        public FairConfig(int Id, string Name, string Title, string Des, string Filter, int HappyLimit, string Image, string Bg)
        {
            this.Id = Id;
            this.Name = Name;
            this.Title = Title;
            this.Des = Des;
            this.Filter = Filter;
            this.HappyLimit = HappyLimit;
            this.Image = Image;
            this.Bg = Bg;
        }

        public FairConfig() { }

        private static Dictionary<int, FairConfig> config = new Dictionary<int, FairConfig>();
        public static Dictionary<int, FairConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, FairConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new FairConfig(1, "forceover", "势力灭亡", "{forceName}势力已被消灭!", "", 0, "forceover", "forceover");
            config[2] = new FairConfig(2, "dry", "干旱", "{cityList}城市出现干旱!", "inland", 90, "dry", "");
            config[3] = new FairConfig(3, "flood", "洪水", "{cityList}城市出现洪水!", "water", 90, "flood", "");
            config[4] = new FairConfig(4, "sick", "瘟疫流行", "{cityList}城市流行瘟疫!", "", 75, "sick", "");
            config[5] = new FairConfig(5, "insect", "虫害", "{cityList}城市流行虫害!", "", 75, "insect", "");
            config[6] = new FairConfig(6, "rebel", "叛乱", "{cityList}城市出现叛乱!", "", 60, "rebel", "");

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

        public static FairConfig GetConfig(int id)
        {
            FairConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表FairConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxName = new Dictionary<string, int>();
        public static FairConfig GetConfigByName(string val)
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

        public static void Assign(int id, FairConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, FairConfig configData)
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
