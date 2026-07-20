using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class CityDevKingActionConfig
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
            {"Cname", new FieldMetaInfo("中文名", "string", 0)},
            {"Des", new FieldMetaInfo("描述", "string", 436)},
            {"Mp4", new FieldMetaInfo("动画文件", "string", 0)},
            {"BaseRate", new FieldMetaInfo("基础成功率", "float", 60)},
            {"AttrHighBound", new FieldMetaInfo("属性阈值", "int", 60)},
            {"BonusPerPoint", new FieldMetaInfo("每点溢出收益", "float", 60)},
            {"KingBonus", new FieldMetaInfo("君主收益", "float", 60)},
            {"NeedAdditiveBonus", new FieldMetaInfo("启用派系爱好加成", "bool", 0)},
            {"EffectMin", new FieldMetaInfo("效果最小值", "int", 60)},
            {"EffectMax", new FieldMetaInfo("效果最大值", "int", 60)},
            {"Effect2Min", new FieldMetaInfo("次效果最小值", "int", 60)},
            {"Effect2Max", new FieldMetaInfo("次效果最大值", "int", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列（对应 CityDevConfig 中的 KingAction devId）
        /// </summary>
        public int Id;
        /// <summary>
        ///中文名
        /// </summary>
        public string Cname;
        /// <summary>
        ///描述
        /// </summary>
        public string Des;
        /// <summary>
        ///动画文件
        /// </summary>
        public string Mp4;
        /// <summary>
        ///基础成功率(0-1)
        /// </summary>
        public float BaseRate;
        /// <summary>
        ///属性阈值
        /// </summary>
        public int AttrHighBound;
        /// <summary>
        ///每点溢出收益
        /// </summary>
        public float BonusPerPoint;
        /// <summary>
        ///君主收益
        /// </summary>
        public float KingBonus;
        /// <summary>
        ///启用派系爱好加成
        /// </summary>
        public bool NeedAdditiveBonus;
        /// <summary>
        ///效果最小值（含）
        /// </summary>
        public int EffectMin;
        /// <summary>
        ///效果最大值（含）
        /// </summary>
        public int EffectMax;
        /// <summary>
        ///次效果最小值（含，扰乱忠心专用）
        /// </summary>
        public int Effect2Min;
        /// <summary>
        ///次效果最大值（含，扰乱忠心专用）
        /// </summary>
        public int Effect2Max;


        public CityDevKingActionConfig(int Id, string Cname, string Des, string Mp4, float BaseRate, int AttrHighBound, float BonusPerPoint, float KingBonus, bool NeedAdditiveBonus, int EffectMin, int EffectMax, int Effect2Min, int Effect2Max)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.Des = Des;
            this.Mp4 = Mp4;
            this.BaseRate = BaseRate;
            this.AttrHighBound = AttrHighBound;
            this.BonusPerPoint = BonusPerPoint;
            this.KingBonus = KingBonus;
            this.NeedAdditiveBonus = NeedAdditiveBonus;
            this.EffectMin = EffectMin;
            this.EffectMax = EffectMax;
            this.Effect2Min = Effect2Min;
            this.Effect2Max = Effect2Max;
        }

        public CityDevKingActionConfig() { }

        private static Dictionary<int, CityDevKingActionConfig> config = new Dictionary<int, CityDevKingActionConfig>();
        public static Dictionary<int, CityDevKingActionConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, CityDevKingActionConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[21102] = new CityDevKingActionConfig(21102, "移动", "派遣武将移动到其他城市", "move2.mp4", 0f, 0, 0f, 0f, false, 0, 0, 0, 0);
            config[21103] = new CityDevKingActionConfig(21103, "出战", "派遣武将出兵攻打敌人", "atk2.mp4", 0f, 0, 0f, 0f, false, 0, 0, 0, 0);
            config[21202] = new CityDevKingActionConfig(21202, "走访", "派遣武将走访搜集人才和宝物", "search2.mp4", 0f, 0, 0f, 0f, false, 0, 0, 0, 0);
            config[21203] = new CityDevKingActionConfig(21203, "交易", "派遣武将购买粮食和士兵", "change.mp4", 0f, 0, 0f, 0f, false, 0, 0, 0, 0);
            config[21204] = new CityDevKingActionConfig(21204, "登用", "派遣武将登庸在野/俘虏武将", "wild.mp4", 0f, 75, 0.01f, 0.1f, true, 0, 0, 0, 0);
            config[21205] = new CityDevKingActionConfig(21205, "褒奖", "派遣武将褒奖（不消耗黄金），EffectMin/Max=提升忠心点数", "praise.mp4", 0f, 0, 0f, 0f, false, 1, 3, 0, 0);
            config[21206] = new CityDevKingActionConfig(21206, "奖赏", "派遣武将奖赏（消耗黄金），EffectMin/Max=提升忠心点数", "praise.mp4", 0f, 0, 0f, 0f, false, 3, 5, 0, 0);
            config[21207] = new CityDevKingActionConfig(21207, "破坏", "派遣武将破坏敌方城防，EffectMin/Max=城防降低点数", "destroy.mp4", 0.5f, 75, 0.01f, 0f, false, 5, 10, 0, 0);
            config[21208] = new CityDevKingActionConfig(21208, "扰乱", "派遣武将扰乱敌方，EffectMin/Max=民心降低点数，Effect2Min/Max=忠心降低点数", "destroy.mp4", 0.5f, 75, 0.01f, 0f, false, 3, 5, 3, 5);
            config[21209] = new CityDevKingActionConfig(21209, "外交", "派遣武将亲善目标势力，EffectMin/Max=友好度提升点数", "peace.mp4", 0.5f, 75, 0.01f, 0f, false, 10, 10, 0, 0);
            config[21210] = new CityDevKingActionConfig(21210, "挑拨", "派遣武将挑拨两个目标势力，EffectMin/Max=友好度降低点数", "peace.mp4", 0.5f, 75, 0.01f, 0f, false, 10, 10, 0, 0);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static CityDevKingActionConfig GetConfig(int id)
        {
            CityDevKingActionConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表CityDevKingActionConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, CityDevKingActionConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, CityDevKingActionConfig configData)
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
