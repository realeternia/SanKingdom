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
            config[30011] = new TechConfig(30011, "青铜兵器", "动员兵攻击+5", "Battle", 1, 31301, new float[]{5f,0f}, 0, 100, "sword", 10f);
            config[30012] = new TechConfig(30012, "狩猎", "动员兵防御+5", "Battle", 1, 31302, new float[]{5f,0f}, 0, 100, "sword", 9f);
            config[30021] = new TechConfig(30021, "炼铁", "动员兵攻击+5", "Battle", 2, 31301, new float[]{5f,0f}, 0, 200, "sword", 8f);
            config[30022] = new TechConfig(30022, "箭簇改良", "弓兵攻击+5", "Battle", 2, 31331, new float[]{5f,0f}, 0, 200, "sword", 7f);
            config[30031] = new TechConfig(30031, "马铁", "骑兵移动力+2", "Battle", 3, 31313, new float[]{2f,0f}, 0, 400, "sword", 6f);
            config[30032] = new TechConfig(30032, "精铁锻造", "动员兵攻击+10", "Battle", 3, 31301, new float[]{10f,0f}, 0, 400, "sword", 6f);
            config[30041] = new TechConfig(30041, "精锐骑兵", "骑兵攻击+10且+10%", "Battle", 4, 31311, new float[]{10f,0.1f}, 0, 700, "sword", 4f);
            config[30042] = new TechConfig(30042, "精锐枪兵", "动员兵攻击+10", "Battle", 4, 31301, new float[]{10f,0f}, 0, 700, "sword", 4f);
            config[30051] = new TechConfig(30051, "铁骑", "骑兵攻击+15且+15%", "Battle", 5, 31311, new float[]{15f,0.15f}, 0, 1000, "sword", 3f);
            config[30052] = new TechConfig(30052, "连弩", "弓兵攻击+15且+15%", "Battle", 5, 31331, new float[]{15f,0.15f}, 0, 1000, "sword", 3f);
            config[30111] = new TechConfig(30111, "耒耜", "农田产出+15%", "Development", 1, 31002, new float[]{0f,0.15f}, 0, 100, "sword", 10f);
            config[30112] = new TechConfig(30112, "谷仓", "农田消耗-20%", "Development", 1, 31003, new float[]{0f,0.2f}, 0, 100, "sword", 9f);
            config[30121] = new TechConfig(30121, "都江堰", "农田产出+20%", "Development", 2, 31002, new float[]{0f,0.2f}, 0, 200, "sword", 9f);
            config[30122] = new TechConfig(30122, "灌渠", "木材场产出+15%", "Development", 2, 31072, new float[]{0f,0.15f}, 0, 200, "sword", 8f);
            config[30131] = new TechConfig(30131, "占城稻", "农田产出+25%", "Development", 3, 31002, new float[]{0f,0.25f}, 0, 400, "sword", 6f);
            config[30132] = new TechConfig(30132, "曲辕犁", "铁匠铺产出+20%", "Development", 3, 31082, new float[]{0f,0.2f}, 0, 400, "sword", 6f);
            config[30141] = new TechConfig(30141, "淘金", "金矿产出+15%", "Development", 4, 31102, new float[]{0f,0.15f}, 0, 700, "sword", 4f);
            config[30142] = new TechConfig(30142, "商路", "市场消耗-20%", "Development", 4, 31013, new float[]{0f,0.2f}, 0, 700, "sword", 4f);
            config[30151] = new TechConfig(30151, "市舶司", "市场产出+30%", "Development", 5, 31012, new float[]{0f,0.3f}, 0, 1000, "sword", 3f);
            config[30152] = new TechConfig(30152, "海运", "金矿产出+25%", "Development", 5, 31102, new float[]{0f,0.25f}, 0, 1000, "sword", 3f);
            config[30211] = new TechConfig(30211, "礼制", "治安民心效果+15%", "Institution", 1, 31042, new float[]{0f,0.15f}, 0, 100, "sword", 8f);
            config[30212] = new TechConfig(30212, "井田制", "市场产出+10%", "Institution", 1, 31012, new float[]{0f,0.1f}, 0, 100, "sword", 8f);
            config[30221] = new TechConfig(30221, "察举制", "征兵产出+15%", "Institution", 2, 31022, new float[]{0f,0.15f}, 0, 200, "sword", 7f);
            config[30222] = new TechConfig(30222, "编户齐民", "征兵人数+1", "Institution", 2, 31024, new float[]{1f,0f}, 0, 200, "sword", 7f);
            config[30231] = new TechConfig(30231, "马政", "马场人数+1", "Institution", 3, 31064, new float[]{1f,0f}, 0, 400, "sword", 6f);
            config[30232] = new TechConfig(30232, "刑律", "治安民心效果+20%", "Institution", 3, 31042, new float[]{0f,0.2f}, 0, 400, "sword", 6f);
            config[30241] = new TechConfig(30241, "九品中正", "征兵产出+20%", "Institution", 4, 31022, new float[]{0f,0.2f}, 0, 700, "sword", 4f);
            config[30242] = new TechConfig(30242, "均田制", "农田产出+15%", "Institution", 4, 31002, new float[]{0f,0.15f}, 0, 700, "sword", 4f);
            config[30251] = new TechConfig(30251, "科举制", "征兵产出+25%", "Institution", 5, 31022, new float[]{0f,0.25f}, 0, 1000, "sword", 3f);
            config[30252] = new TechConfig(30252, "府兵制", "征兵人数+2", "Institution", 5, 31024, new float[]{2f,0f}, 0, 1000, "sword", 3f);
            config[30411] = new TechConfig(30411, "驿道", "英雄移动效率+15%", "Engineering", 1, 31292, new float[]{0f,0.15f}, 0, 100, "sword", 9f);
            config[30412] = new TechConfig(30412, "官道", "部队移动效率+15%", "Engineering", 1, 31293, new float[]{0f,0.15f}, 0, 100, "sword", 9f);
            config[30421] = new TechConfig(30421, "夯土墙", "城墙效果+20%", "Engineering", 2, 31032, new float[]{0f,0.2f}, 0, 200, "sword", 8f);
            config[30422] = new TechConfig(30422, "瓮城", "城墙效果+15%", "Engineering", 2, 31032, new float[]{0f,0.15f}, 0, 200, "sword", 7f);
            config[30431] = new TechConfig(30431, "地动仪", "城墙效果+30%（防灾）", "Engineering", 3, 31032, new float[]{0f,0.3f}, 0, 400, "sword", 6f);
            config[30432] = new TechConfig(30432, "抛石机", "城墙效果+25%", "Engineering", 3, 31032, new float[]{0f,0.25f}, 0, 400, "sword", 6f);
            config[30441] = new TechConfig(30441, "城砖", "城墙效果+30%", "Engineering", 4, 31032, new float[]{0f,0.3f}, 0, 700, "sword", 5f);
            config[30442] = new TechConfig(30442, "烽火台", "城墙效果+25%", "Engineering", 4, 31032, new float[]{0f,0.25f}, 0, 700, "sword", 5f);
            config[30451] = new TechConfig(30451, "重甲", "城墙效果+40%", "Engineering", 5, 31032, new float[]{0f,0.4f}, 0, 1000, "sword", 4f);
            config[30452] = new TechConfig(30452, "塔楼", "城墙效果+35%", "Engineering", 5, 31032, new float[]{0f,0.35f}, 0, 1000, "sword", 4f);

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
