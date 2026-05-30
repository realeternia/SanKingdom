﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityAttrConfig
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
            {"name", new FieldMetaInfo("名字", "string", 0, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"IsForceAttr", new FieldMetaInfo("是否force", "bool", 0)},
            {"IsPosRes", new FieldMetaInfo("占用类资源", "bool", 0)},
            {"NotShow", new FieldMetaInfo("不显示top ui", "bool", 0)},
            {"ValMaxCity", new FieldMetaInfo("最大值", "int", 0)},
            {"ValMaxForce", new FieldMetaInfo("最大值", "int", 0)},
            {"Icon", new FieldMetaInfo("icon", "string", 0)},
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
        public string name;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///是否force
        /// </summary>
        public bool IsForceAttr;
        /// <summary>
        ///占用类资源
        /// </summary>
        public bool IsPosRes;
        /// <summary>
        ///不显示top ui
        /// </summary>
        public bool NotShow;
        /// <summary>
        ///最大值
        /// </summary>
        public int ValMaxCity;
        /// <summary>
        ///最大值
        /// </summary>
        public int ValMaxForce;
        /// <summary>
        ///icon
        /// </summary>
        public string Icon;


        public CityAttrConfig(int Id, string name, string Cname, bool IsForceAttr, bool IsPosRes, bool NotShow, int ValMaxCity, int ValMaxForce, string Icon)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.IsForceAttr = IsForceAttr;
            this.IsPosRes = IsPosRes;
            this.NotShow = NotShow;
            this.ValMaxCity = ValMaxCity;
            this.ValMaxForce = ValMaxForce;
            this.Icon = Icon;
        }

        public CityAttrConfig() { }

        private static Dictionary<int, CityAttrConfig> config = new Dictionary<int, CityAttrConfig>();
        public static Dictionary<int, CityAttrConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityAttrConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new CityAttrConfig(1, "level", "等级", false, false, true, 99, 0, "");
            config[2] = new CityAttrConfig(2, "exp", "发展度", false, false, false, 999, 0, "citydev");
            config[5] = new CityAttrConfig(5, "food", "粮食", false, false, false, 999, 0, "cityfood");
            config[6] = new CityAttrConfig(6, "soldier", "士兵", false, false, false, 999, 0, "armscount");
            config[7] = new CityAttrConfig(7, "happy", "民心", false, false, false, 999, 0, "cityheart");
            config[8] = new CityAttrConfig(8, "wall", "城墙", false, false, false, 999, 0, "citywall");
            config[12] = new CityAttrConfig(12, "gold", "金钱", true, false, false, 0, 999, "citygold");
            config[13] = new CityAttrConfig(13, "steel", "铁", true, true, false, 0, 999, "citysteel");
            config[14] = new CityAttrConfig(14, "horse", "马", true, true, false, 0, 999, "cityhorse");
            config[15] = new CityAttrConfig(15, "wood", "木材", true, true, false, 0, 999, "citywood");
            config[16] = new CityAttrConfig(16, "stone", "石料", true, true, false, 0, 999, "citystone");
            config[17] = new CityAttrConfig(17, "elephant", "战象", true, true, true, 0, 999, "cityelephant");

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

        public static CityAttrConfig GetConfig(int id)
        {
            CityAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityAttrConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxname = new Dictionary<string, int>();
        public static CityAttrConfig GetConfigByname(string val)
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

        public static void Assign(int id, CityAttrConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityAttrConfig configData)
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
