using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SysConfigModify
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
            {"Name", new FieldMetaInfo("英文名", "string", 184, "", true)},
            {"Cname", new FieldMetaInfo("中文名", "string", 157)},
            {"BaseVal", new FieldMetaInfo("基础值", "int", 60)},
            {"RandomMin", new FieldMetaInfo("随机下限", "int", 60)},
            {"RandomMax", new FieldMetaInfo("随机上限", "int", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
        /// </summary>
        public int Id;
        /// <summary>
        ///英文名（用于 GetConfigByName 查找）
        /// </summary>
        public string Name;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///基础值
        /// </summary>
        public int BaseVal;
        /// <summary>
        ///随机下限（含）
        /// </summary>
        public int RandomMin;
        /// <summary>
        ///随机上限（含）
        /// </summary>
        public int RandomMax;


        public SysConfigModify(int Id, string Name, string Cname, int BaseVal, int RandomMin, int RandomMax)
        {
            this.Id = Id;
            this.Name = Name;
            this.Cname = Cname;
            this.BaseVal = BaseVal;
            this.RandomMin = RandomMin;
            this.RandomMax = RandomMax;
        }

        public SysConfigModify() { }

        private static Dictionary<int, SysConfigModify> config = new Dictionary<int, SysConfigModify>();
        public static Dictionary<int, SysConfigModify>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SysConfigModify> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[22001] = new SysConfigModify(22001, "CapturedLoyaltyDecay", "俘虏忠心衰减", 0, 1, 3);
            config[22002] = new SysConfigModify(22002, "EscapeChance", "逃脱概率", 20, 0, 0);
            config[22003] = new SysConfigModify(22003, "WildHeroMoveChance", "在野迁移概率", 20, 0, 0);
            config[22004] = new SysConfigModify(22004, "RecruitWildBaseRate", "登庸在野基础成功率", 30, 0, 0);
            config[22005] = new SysConfigModify(22005, "RecruitEnemyOffset", "登庸敌方偏移值", 70, 0, 0);
            config[22006] = new SysConfigModify(22006, "MoveBaseDist", "移动基础日距", 800, 0, 0);
            config[22007] = new SysConfigModify(22007, "RecruitBaseDist", "登庸基础日距", 800, 0, 0);
            config[22008] = new SysConfigModify(22008, "CaptureBaseChance", "基础被抓概率", 10, 0, 0);
            config[22009] = new SysConfigModify(22009, "BattleRelationRise", "战斗关系变化量", 0, 3, 7);

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

        public static SysConfigModify GetConfig(int id)
        {
            SysConfigModify data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SysConfigModify不存在id={0}", id));
        }

        private static Dictionary<string, int> idxName = new Dictionary<string, int>();
        public static SysConfigModify GetConfigByName(string val)
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

        public static void Assign(int id, SysConfigModify configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SysConfigModify configData)
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
