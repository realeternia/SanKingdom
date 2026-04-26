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
            {"NameS", new FieldMetaInfo("名字", "string")},
            {"MoveSpeed", new FieldMetaInfo("移动速度", "int")},
            {"Range", new FieldMetaInfo("攻击距离", "int")},
            {"MissileSpeed", new FieldMetaInfo("导弹速度", "int")},
            {"MissileHight", new FieldMetaInfo("导弹高度", "float")},
            {"HitEffect", new FieldMetaInfo("hit", "string")},
            {"Model", new FieldMetaInfo("模型", "string")},
            {"ModelCountFactor", new FieldMetaInfo("多少个士兵显示一个模型", "int")},
            {"OvercomeStrong", new FieldMetaInfo("强克制", "string")},
            {"OvercomeWeak", new FieldMetaInfo("弱克制", "string")},
            {"HorseCost", new FieldMetaInfo("马消耗", "int")},
            {"SteelCost", new FieldMetaInfo("铁消耗", "int")},
            {"WoodCost", new FieldMetaInfo("木材消耗", "int")},
            {"StoneCost", new FieldMetaInfo("石料消耗", "int")},
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
        ///名字
        /// </summary>
        public string NameS;
        /// <summary>
        ///移动速度
        /// </summary>
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
        ///多少个士兵显示一个模型
        /// </summary>
        public int ModelCountFactor;
        /// <summary>
        ///强克制
        /// </summary>
        public string OvercomeStrong;
        /// <summary>
        ///弱克制
        /// </summary>
        public string OvercomeWeak;
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


        public ArmsConfig(int Id, string Name, string NameS, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, string HitEffect, string Model, int ModelCountFactor, string OvercomeStrong, string OvercomeWeak, int HorseCost, int SteelCost, int WoodCost, int StoneCost)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.HitEffect = HitEffect;
            this.Model = Model;
            this.ModelCountFactor = ModelCountFactor;
            this.OvercomeStrong = OvercomeStrong;
            this.OvercomeWeak = OvercomeWeak;
            this.HorseCost = HorseCost;
            this.SteelCost = SteelCost;
            this.WoodCost = WoodCost;
            this.StoneCost = StoneCost;

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
        }        public class CellMeta
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

        private static List<CellMeta> cellMeta = new List<CellMeta>()
        {
            new CellMeta(0, 2, null, -65536),
        };
        public static List<CellMeta> CellMetas { get { return cellMeta; } }



        public static void Load()
{
config.Clear();
config[101] = new ArmsConfig(101, "ma", "马", 10, 17, 0, 0f, "SwordHitYellowCritical", "SodStick", 40, "戟弩炮车", "弓", 2, 0, 0, 0);
config[102] = new ArmsConfig(102, "che", "车", 10, 17, 0, 0f, "SwordHitGreenCritical", "SodStick", 40, "", "", 0, 1, 1, 0);
config[201] = new ArmsConfig(201, "gong", "弓", 10, 40, 40, 5f, "BulletExplosionBlue", "SodBow", 40, "枪戟", "刀", 0, 0, 1, 0);
config[202] = new ArmsConfig(202, "pao", "炮", 10, 17, 0, 0f, "SwordHitYellowCritical", "SodBow", 40, "盾", "士", 0, 2, 1, 0);
config[601] = new ArmsConfig(601, "dao", "刀", 10, 17, 0, 0f, "SwordHitYellowCritical", "SodStick", 40, "马车", "", 0, 1, 0, 0);
config[602] = new ArmsConfig(602, "daoqiang", "枪", 10, 17, 0, 0f, "SwordHitYellowCritical", "SodStick", 40, "枪", "", 0, 1, 1, 0);
config[603] = new ArmsConfig(603, "daoji", "戟", 10, 40, 30, 3f, "FanExplosion", "SodStick", 40, "", "", 0, 1, 0, 0);
config[701] = new ArmsConfig(701, "shan", "扇", 10, 40, 30, 3f, "GasExplosionFire", "SodStick", 40, "", "", 0, 0, 0, 0);
config[702] = new ArmsConfig(702, "mou", "谋", 7, 50, 26, 8f, "GasShootFire", "SodStick", 40, "", "", 0, 0, 0, 0);



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
