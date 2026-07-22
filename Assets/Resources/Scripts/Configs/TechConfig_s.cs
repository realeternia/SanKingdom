using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class TechConfig
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
            {"Des", new FieldMetaInfo("描述", "string", 198)},
            {"Category", new FieldMetaInfo("分类", "string", 0)},
            {"Level", new FieldMetaInfo("等级", "int", 60)},
            {"SkillId", new FieldMetaInfo("强化模板ID", "int", 60)},
            {"EffectValue", new FieldMetaInfo("效果数值", "float[]", 0)},
            {"EffectId", new FieldMetaInfo("关联实体ID", "int", 60)},
            {"SciPointCost", new FieldMetaInfo("研究所需值", "int", 60)},
            {"Icon", new FieldMetaInfo("图标", "string", 0)},
            {"AiWeight", new FieldMetaInfo("AI权重", "float", 60)},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        private static List<CellMeta> cellMeta = new List<CellMeta>();
        public static List<CellMeta> CellMetas { get { return cellMeta; } }

        /// <summary>
        ///序列
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
        ///分类
        /// </summary>
        public string Category;
        /// <summary>
        ///等级
        /// </summary>
        public int Level;
        /// <summary>
        ///强化模板ID，指向TechSkillConfig
        /// </summary>
        public int SkillId;
        /// <summary>
        ///效果数值，具体数值在此配置，模板只定义类型
        /// </summary>
        public float[] EffectValue;
        /// <summary>
        ///关联实体ID（解锁兵种ID/解锁建筑ID/战法ID等）
        /// </summary>
        public int EffectId;
        /// <summary>
        ///研究所需值
        /// </summary>
        public int SciPointCost;
        /// <summary>
        ///图标
        /// </summary>
        public string Icon;
        /// <summary>
        ///AI权重
        /// </summary>
        public float AiWeight;


        public TechConfig(int Id, string Cname, string Des, string Category, int Level, int SkillId, float[] EffectValue, int EffectId, int SciPointCost, string Icon, float AiWeight)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.Des = Des;
            this.Category = Category;
            this.Level = Level;
            this.SkillId = SkillId;
            this.EffectValue = EffectValue;
            this.EffectId = EffectId;
            this.SciPointCost = SciPointCost;
            this.Icon = Icon;
            this.AiWeight = AiWeight;
        }

        public TechConfig() { }

        private static Dictionary<int, TechConfig> config = new Dictionary<int, TechConfig>();
        public static Dictionary<int, TechConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, TechConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();

            // ============================================================
            // Battle 战斗类（兵种属性强化）
            // ============================================================

            // L1
            config[30001] = new TechConfig(30001, "青铜兵器", "动员兵攻击+5", "Battle", 1, 31301, new float[]{5f,0f}, 0, 100, "bronze", 10f);
            config[30002] = new TechConfig(30002, "铜矛", "动员兵防御+3", "Battle", 1, 31302, new float[]{3f,0f}, 0, 100, "spear", 9f);
            config[30004] = new TechConfig(30004, "狩猎", "动员兵防御+5", "Battle", 1, 31302, new float[]{5f,0f}, 0, 100, "hunting", 9f);
            config[30005] = new TechConfig(30005, "动物驯养", "骑兵移速+10%", "Battle", 1, 31313, new float[]{0f,0.1f}, 0, 100, "taming", 8f);

            // L2
            config[30010] = new TechConfig(30010, "炼铁", "动员兵攻击+5", "Battle", 2, 31301, new float[]{5f,0f}, 0, 200, "iron", 8f);
            config[30011] = new TechConfig(30011, "箭簇改良", "弓兵攻击+5", "Battle", 2, 31331, new float[]{5f,0f}, 0, 200, "arrow", 7f);
            config[30013] = new TechConfig(30013, "战马", "骑兵攻击+5", "Battle", 2, 31311, new float[]{5f,0f}, 0, 200, "warhorse", 7f);

            // L3
            config[30024] = new TechConfig(30024, "马铁", "骑兵移动力+2", "Battle", 3, 31313, new float[]{2f,0f}, 0, 400, "horse_iron", 6f);

            // L4
            config[30030] = new TechConfig(30030, "精锐骑兵", "骑兵攻击+10且+10%", "Battle", 4, 31311, new float[]{10f,0.1f}, 0, 700, "elite_cavalry", 4f);
            config[30031] = new TechConfig(30031, "精锐枪兵", "动员兵攻击+10", "Battle", 4, 31301, new float[]{10f,0f}, 0, 700, "elite_spear", 4f);

            // ============================================================
            // Development 发展类（Dev行动强化）
            // ============================================================

            // L1 农田(21001)
            config[30101] = new TechConfig(30101, "耒耜", "农田产出+15%", "Development", 1, 31002, new float[]{0f,0.15f}, 0, 100, "leisi", 10f);
            config[30102] = new TechConfig(30102, "谷仓", "农田消耗-20%", "Development", 1, 31003, new float[]{0f,0.2f}, 0, 100, "granary", 9f);

            // L2 农田(21001)+木材场(21405)
            config[30110] = new TechConfig(30110, "都江堰", "农田产出+20%", "Development", 2, 31002, new float[]{0f,0.2f}, 0, 200, "dujiangyan", 9f);
            config[30111] = new TechConfig(30111, "灌渠", "木材场产出+15%", "Development", 2, 31072, new float[]{0f,0.15f}, 0, 200, "canal", 8f);

            // L3a 农田(21001)
            config[30120] = new TechConfig(30120, "占城稻", "农田产出+25%", "Development", 3, 31002, new float[]{0f,0.25f}, 0, 400, "rice", 6f);

            // L3b 铁匠铺(21406)+采石场(21403)
            config[30121] = new TechConfig(30121, "曲辕犁", "铁匠铺产出+20%", "Development", 3, 31082, new float[]{0f,0.2f}, 0, 400, "plow", 6f);
            config[30122] = new TechConfig(30122, "炼焦", "采石场产出+15%", "Development", 3, 31052, new float[]{0f,0.15f}, 0, 400, "coke", 6f);

            // L4 金矿(21408)+市场(21002)
            config[30131] = new TechConfig(30131, "淘金", "金矿产出+15%", "Development", 4, 31102, new float[]{0f,0.15f}, 0, 700, "pan_gold", 4f);
            config[30132] = new TechConfig(30132, "商路", "市场消耗-20%", "Development", 4, 31013, new float[]{0f,0.2f}, 0, 700, "trade_route", 4f);

            // L5 市场(21002)
            config[30140] = new TechConfig(30140, "市舶司", "市场产出+30%", "Development", 5, 31012, new float[]{0f,0.3f}, 0, 1000, "maritime_trade", 3f);

            // ============================================================
            // Institution 制度类（治理+招募+军事制度）
            // ============================================================

            // L1 治安(21005)+市场(21002)+城墙(21004)
            config[30201] = new TechConfig(30201, "礼制", "治安民心效果+15%", "Institution", 1, 31042, new float[]{0f,0.15f}, 0, 100, "rites", 8f);
            config[30202] = new TechConfig(30202, "井田制", "市场产出+10%", "Institution", 1, 31012, new float[]{0f,0.1f}, 0, 100, "field", 8f);
            config[30203] = new TechConfig(30203, "礼治", "治安效果+15%", "Institution", 1, 31042, new float[]{0f,0.15f}, 0, 100, "govern", 7f);
            config[30204] = new TechConfig(30204, "城制", "城墙消耗-20%", "Institution", 1, 31033, new float[]{0f,0.2f}, 0, 100, "wall_sys", 7f);

            // L2 征兵(21003)
            config[30210] = new TechConfig(30210, "察举制", "征兵产出+15%", "Institution", 2, 31022, new float[]{0f,0.15f}, 0, 200, "recommend", 7f);
            config[30211] = new TechConfig(30211, "编户齐民", "征兵人数+1", "Institution", 2, 31024, new float[]{1f,0f}, 0, 200, "census", 7f);
            config[30212] = new TechConfig(30212, "征发", "征兵产出+15%", "Institution", 2, 31022, new float[]{0f,0.15f}, 0, 200, "levy", 6f);

            // L3a 马场(21404)
            config[30221] = new TechConfig(30221, "马政", "马场人数+1", "Institution", 3, 31064, new float[]{1f,0f}, 0, 400, "horse_admin", 6f);

            // L3b 治安(21005)+城墙(21004)
            config[30222] = new TechConfig(30222, "刑律", "治安民心效果+20%", "Institution", 3, 31042, new float[]{0f,0.2f}, 0, 400, "law", 6f);

            // ============================================================
            // Engineering 工程类（交通+城防+武备）
            // ============================================================

            // L1 移动(21102)
            config[30401] = new TechConfig(30401, "驿道", "英雄移动效率+15%", "Engineering", 1, 31292, new float[]{0f,0.15f}, 0, 100, "post_road", 9f);
            config[30402] = new TechConfig(30402, "官道", "部队移动效率+15%", "Engineering", 1, 31293, new float[]{0f,0.15f}, 0, 100, "main_road", 9f);

            // L2 城墙(21004)
            config[30410] = new TechConfig(30410, "夯土墙", "城墙效果+20%", "Engineering", 2, 31032, new float[]{0f,0.2f}, 0, 200, "rammed_earth", 8f);
            config[30411] = new TechConfig(30411, "瓮城", "城墙效果+15%", "Engineering", 2, 31032, new float[]{0f,0.15f}, 0, 200, "barbican", 7f);
            config[30412] = new TechConfig(30412, "地动仪", "城墙效果+30%（防灾）", "Engineering", 2, 31032, new float[]{0f,0.3f}, 0, 200, "seismograph", 7f);

            // L4
            config[30430] = new TechConfig(30430, "城砖", "城墙效果+30%", "Engineering", 4, 31032, new float[]{0f,0.3f}, 0, 700, "brick", 5f);

            // L5
            config[30440] = new TechConfig(30440, "烽火台", "城墙效果+25%", "Engineering", 5, 31032, new float[]{0f,0.25f}, 0, 1000, "beacon", 4f);

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static TechConfig GetConfig(int id)
        {
            TechConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表TechConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, TechConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, TechConfig configData)
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
