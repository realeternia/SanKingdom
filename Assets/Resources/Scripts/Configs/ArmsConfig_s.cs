using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ArmsConfig
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
            {"Name", new FieldMetaInfo("名字", "string", 0)},
            {"NameS", new FieldMetaInfo("名字", "string", 64)},
            {"Type", new FieldMetaInfo("兵种类型", "ArmsType", 168)},
            {"Level", new FieldMetaInfo("等级", "int", 60)},
            {"Atk", new FieldMetaInfo("攻击", "int", 60)},
            {"Def", new FieldMetaInfo("防御", "int", 60)},
            {"MoveSpeed", new FieldMetaInfo("移动速度", "int", 60)},
            {"Range", new FieldMetaInfo("攻击距离", "int", 60)},
            {"MissileSpeed", new FieldMetaInfo("导弹速度", "int", 60)},
            {"MissileHight", new FieldMetaInfo("导弹高度", "float", 60)},
            {"HitEffect", new FieldMetaInfo("hit", "string", 161)},
            {"Model", new FieldMetaInfo("模型", "string", 0)},
            {"HitDelay", new FieldMetaInfo("命中延迟", "float", 0)},
            {"ModelCountFactor", new FieldMetaInfo("多少个士兵显示一个模型", "int", 60)},
            {"HorseCost", new FieldMetaInfo("马消耗", "int", 60)},
            {"SteelCost", new FieldMetaInfo("铁消耗", "int", 60)},
            {"WoodCost", new FieldMetaInfo("木材消耗", "int", 60)},
            {"StoneCost", new FieldMetaInfo("石料消耗", "int", 60)},
            {"CanAssign", new FieldMetaInfo("可配给军队", "bool", 60)},
            {"AttackAnimCount", new FieldMetaInfo("攻击动画数量", "int", 60)},
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
        ///名字
        /// </summary>
        public string NameS;
        public ArmsType Type;
        /// <summary>
        ///等级
        /// </summary>
        public int Level;
        public int Atk;
        public int Def;
        public int MoveSpeed;
        /// <summary>
        ///攻击距离
        /// </summary>
        public int Range;
        /// <summary>
        ///导弹速度
        /// </summary>
        public int MissileSpeed;
        /// <summary>
        ///导弹高度
        /// </summary>
        public float MissileHight;
        /// <summary>
        ///hit
        /// </summary>
        public string HitEffect;
        /// <summary>
        ///模型
        /// </summary>
        public string Model;
        /// <summary>
        ///命中延迟
        /// </summary>
        public float HitDelay;
        /// <summary>
        ///多少个士兵显示一个模型
        /// </summary>
        public int ModelCountFactor;
        /// <summary>
        ///马消耗
        /// </summary>
        public int HorseCost;
        /// <summary>
        ///铁消耗
        /// </summary>
        public int SteelCost;
        /// <summary>
        ///木材消耗
        /// </summary>
        public int WoodCost;
        /// <summary>
        ///石料消耗
        /// </summary>
        public int StoneCost;
        /// <summary>
        ///可配给军队
        /// </summary>
        public bool CanAssign;
        /// <summary>
        ///攻击动画数量
        /// </summary>
        public int AttackAnimCount;


        public ArmsConfig(int Id, string Name, string NameS, ArmsType Type, int Level, int Atk, int Def, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, string HitEffect, string Model, float HitDelay, int ModelCountFactor, int HorseCost, int SteelCost, int WoodCost, int StoneCost, bool CanAssign, int AttackAnimCount)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.Type = Type;
            this.Level = Level;
            this.Atk = Atk;
            this.Def = Def;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.HitEffect = HitEffect;
            this.Model = Model;
            this.HitDelay = HitDelay;
            this.ModelCountFactor = ModelCountFactor;
            this.HorseCost = HorseCost;
            this.SteelCost = SteelCost;
            this.WoodCost = WoodCost;
            this.StoneCost = StoneCost;
            this.CanAssign = CanAssign;
            this.AttackAnimCount = AttackAnimCount;
        }

        public ArmsConfig() { }

        private static Dictionary<int, ArmsConfig> config = new Dictionary<int, ArmsConfig>();
        public static Dictionary<int, ArmsConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, ArmsConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new ArmsConfig(1, "dyb", "动员兵", ArmsType.SodWalk, 0, 0, 0, 10, 17, 0, 0f, "SwordHitYellowCritical", "SodStick", 1f, 4, 0, 0, 0, 0, true, 1);
            config[101] = new ArmsConfig(101, "ma", "骑兵", ArmsType.SodHorse, 1, 35, 5, 10, 17, 0, 0f, "SwordHitYellowCritical", "SodHorseSpear", 1f, 6, 2, 0, 0, 0, true, 1);
            config[201] = new ArmsConfig(201, "gong", "弓兵", ArmsType.SodBow, 1, 10, 0, 10, 40, 40, 5f, "BulletExplosionBlue", "SodBow", 1f, 4, 0, 0, 1, 0, true, 1);
            config[601] = new ArmsConfig(601, "dao", "刀", ArmsType.SodWalk, 1, 10, 8, 10, 17, 0, 0f, "SwordHitYellowCritical", "SodDao", 0f, 4, 0, 1, 0, 0, true, 2);
            config[602] = new ArmsConfig(602, "daoqiang", "枪", ArmsType.SodWalk, 1, 20, 10, 10, 17, 0, 0f, "SwordHitYellowCritical", "SodSpear", 1f, 4, 0, 1, 1, 0, true, 1);
            config[603] = new ArmsConfig(603, "daoji", "戟", ArmsType.SodWalk, 1, 10, 20, 10, 17, 0, 0f, "SwordHitYellowCritical", "SodHalberd", 1f, 4, 0, 1, 1, 0, true, 1);
            config[901] = new ArmsConfig(901, "jianta", "箭塔", ArmsType.SodBow, 1, 0, 0, 10, 45, 60, 10f, "BulletExplosionBlue", "SodBow", 1f, 4, 0, 0, 1, 0, false, 1);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static ArmsConfig GetConfig(int id)
        {
            ArmsConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表ArmsConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, ArmsConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, ArmsConfig configData)
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
