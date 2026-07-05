using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class BattleUnitConfig
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
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"Lv", new FieldMetaInfo("等级", "int", 0)},
            {"ArmsId", new FieldMetaInfo("ArmsId", "int", 0)},
            {"Hp", new FieldMetaInfo("生命", "int", 0)},
            {"Atk", new FieldMetaInfo("攻击", "int", 0)},
            {"Def", new FieldMetaInfo("防御", "int", 0)},
            {"IsShadow", new FieldMetaInfo("是否隐藏", "bool", 0)},
            {"SoldierAtkRate", new FieldMetaInfo("士兵加成攻击系数", "float", 0)},
            {"SoldierHpRate", new FieldMetaInfo("士兵加成hp系数", "float", 0)},
            {"Skills", new FieldMetaInfo("技能", "int[]", 0)},
            {"Model", new FieldMetaInfo("模型", "string", 0)},
            {"UnitType", new FieldMetaInfo("类型(0=普通 1=城门 2=墙 3=箭塔)", "int", 0)},
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
        ///等级
        /// </summary>
        public int Lv;
        /// <summary>
        ///ArmsId
        /// </summary>
        public int ArmsId;
        /// <summary>
        ///生命
        /// </summary>
        public int Hp;
        /// <summary>
        ///攻击
        /// </summary>
        public int Atk;
        /// <summary>
        ///防御
        /// </summary>
        public int Def;
        /// <summary>
        ///是否隐藏
        /// </summary>
        public bool IsShadow;
        /// <summary>
        ///士兵加成攻击系数
        /// </summary>
        public float SoldierAtkRate;
        /// <summary>
        ///士兵加成hp系数
        /// </summary>
        public float SoldierHpRate;
        /// <summary>
        ///技能
        /// </summary>
        public int[] Skills;
        /// <summary>
        ///模型
        /// </summary>
        public string Model;
        /// <summary>
        ///类型(0=普通 1=城门 2=墙)
        /// </summary>
        public int UnitType;


        public BattleUnitConfig(int Id, string Name, int Lv, int ArmsId, int Hp, int Atk, int Def, bool IsShadow, float SoldierAtkRate, float SoldierHpRate, int[] Skills, string Model, int UnitType)
        {
            this.Id = Id;
            this.Name = Name;
            this.Lv = Lv;
            this.ArmsId = ArmsId;
            this.Hp = Hp;
            this.Atk = Atk;
            this.Def = Def;
            this.IsShadow = IsShadow;
            this.SoldierAtkRate = SoldierAtkRate;
            this.SoldierHpRate = SoldierHpRate;
            this.Skills = Skills;
            this.Model = Model;
            this.UnitType = UnitType;
        }

        public BattleUnitConfig() { }

        private static Dictionary<int, BattleUnitConfig> config = new Dictionary<int, BattleUnitConfig>();
        public static Dictionary<int, BattleUnitConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, BattleUnitConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[500001] = new BattleUnitConfig(500001, "小兵", 1, 201, 130, 50, 50, false, 1f, 1f, new int[0], "UnitBing", 0);
            config[500002] = new BattleUnitConfig(500002, "远程小兵", 1, 201, 90, 50, 50, false, .8f, .65f, new int[0], "UnitBing2", 0);
            config[501001] = new BattleUnitConfig(501001, "法术场", 1, 201, 9999, 99, 99, true, 0f, 0f, new int[0], "UnitSpell", 0);
            config[501002] = new BattleUnitConfig(501002, "关羽影子", 1, 201, 2, 50, 50, false, 0f, 0f, new int[0], "UnitHero", 0);
            config[502001] = new BattleUnitConfig(502001, "城门", 1, 0, 100, 0, 0, false, 0f, 0f, new int[0], "Wall_B_gate", 1);
            config[502002] = new BattleUnitConfig(502002, "墙", 1, 0, 999999, 0, 0, false, 0f, 0f, new int[0], "Wall_B_wall", 2);
            config[502003] = new BattleUnitConfig(502003, "箭塔", 1, 901, 50, 30, 30, false, 0f, 0f, new int[0], "Tower_B", 3);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static BattleUnitConfig GetConfig(int id)
        {
            BattleUnitConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表BattleUnitConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, BattleUnitConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, BattleUnitConfig configData)
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
