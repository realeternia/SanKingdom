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
            public FieldMetaInfo(string name, string type)
            {
                fieldName = name;
                fieldType = type;
            }
        }

        private static Dictionary<string, FieldMetaInfo> fieldMeta = new Dictionary<string, FieldMetaInfo>()
        {
            {"Id", new FieldMetaInfo("序列", "int")},
            {"Name", new FieldMetaInfo("名字", "string")},
            {"Lv", new FieldMetaInfo("等级", "int")},
            {"ArmsId", new FieldMetaInfo("ArmsId", "int")},
            {"Hp", new FieldMetaInfo("生命", "int")},
            {"Atk", new FieldMetaInfo("攻击", "int")},
            {"Def", new FieldMetaInfo("防御", "int")},
            {"IsShadow", new FieldMetaInfo("是否隐藏", "bool")},
            {"SoldierAtkRate", new FieldMetaInfo("士兵加成攻击系数", "float")},
            {"SoldierHpRate", new FieldMetaInfo("士兵加成hp系数", "float")},
            {"Skills", new FieldMetaInfo("技能", "int[]")},
            {"Model", new FieldMetaInfo("模型", "string")},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

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


        public BattleUnitConfig(int Id, string Name, int Lv, int ArmsId, int Hp, int Atk, int Def, bool IsShadow, float SoldierAtkRate, float SoldierHpRate, int[] Skills, string Model)
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
        }

        public static void Load()
{
config.Clear();
config[500001] = new BattleUnitConfig(500001, "小兵", 1, 201, 130, 50, 50, false, 1f, 1f, null, "UnitBing");
config[500002] = new BattleUnitConfig(500002, "远程小兵", 1, 201, 90, 50, 50, false, .8f, .65f, null, "UnitBing2");
config[501001] = new BattleUnitConfig(501001, "法术场", 1, 201, 9999, 99, 99, true, 0f, 0f, null, "UnitSpell");
config[501002] = new BattleUnitConfig(501002, "关羽影子", 1, 201, 2, 50, 50, false, 0f, 0f, null, "UnitHero");



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
