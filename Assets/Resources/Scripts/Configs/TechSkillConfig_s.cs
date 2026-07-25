﻿﻿﻿﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class TechSkillConfig
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
            {"Des", new FieldMetaInfo("描述", "string", 159)},
            {"Category", new FieldMetaInfo("大类", "string", 0)},
            {"Target", new FieldMetaInfo("作用目标", "int", 60)},
            {"EnhanceType", new FieldMetaInfo("强化类型", "string", 0)},
            {"EffectAttr", new FieldMetaInfo("受影响属性", "string", 0)},
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
        ///大类：Dev / KingAction / Arms / SysConfig
        /// SysConfig 的 Target 为 SysConfigModify.Id，EnhanceType 为 AmountAdd(加法) 或 AmountMul(乘法)
        /// </summary>
        public string Category;
        /// <summary>
        ///作用目标：Dev的CityDevConfig ID / KingAction的DevConfig ID / Arms的ArmsConfig ID / SysConfig的SysConfigModify.Id
        /// </summary>
        public int Target;
        /// <summary>
        ///强化类型：AmountAdd/AmountMul/CostReduce/SlotAdd/SuccessMul/ArmsAttrAdd
        /// </summary>
        public string EnhanceType;
        /// <summary>
        ///受影响属性：food/gold/soldier/happy/wall/Atk/Def/MoveSpeed等
        /// </summary>
        public string EffectAttr;


        public TechSkillConfig(int Id, string Cname, string Des, string Category, int Target, string EnhanceType, string EffectAttr)
        {
            this.Id = Id;
            this.Cname = Cname;
            this.Des = Des;
            this.Category = Category;
            this.Target = Target;
            this.EnhanceType = EnhanceType;
            this.EffectAttr = EffectAttr;
        }

        public TechSkillConfig() { }

        private static Dictionary<int, TechSkillConfig> config = new Dictionary<int, TechSkillConfig>();
        public static Dictionary<int, TechSkillConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, TechSkillConfig> dict)
        {
            config.Clear();
            config = dict;
            RebuildIndex();
        }

        public static void Load()
        {
            config.Clear();
            config[31001] = new TechSkillConfig(31001, "农田产出", "农田粮食产出+x", "Dev", 21001, "AmountAdd", "food");
            config[31002] = new TechSkillConfig(31002, "农田产出", "农田粮食产出+x%", "Dev", 21001, "AmountMul", "food");
            config[31003] = new TechSkillConfig(31003, "农田消耗", "农田金钱消耗-x%", "Dev", 21001, "CostReduce", "gold");
            config[31004] = new TechSkillConfig(31004, "农田槽位", "农田分配人数+x", "Dev", 21001, "SlotAdd", "slot");
            config[31011] = new TechSkillConfig(31011, "市场产出", "市场金钱产出+x", "Dev", 21002, "AmountAdd", "gold");
            config[31012] = new TechSkillConfig(31012, "市场产出", "市场金钱产出+x%", "Dev", 21002, "AmountMul", "gold");
            config[31013] = new TechSkillConfig(31013, "市场消耗", "市场金钱消耗-x%", "Dev", 21002, "CostReduce", "gold");
            config[31014] = new TechSkillConfig(31014, "市场槽位", "市场分配人数+x", "Dev", 21002, "SlotAdd", "slot");
            config[31021] = new TechSkillConfig(31021, "征兵产出", "征兵兵力产出+x", "Dev", 21003, "AmountAdd", "soldier");
            config[31022] = new TechSkillConfig(31022, "征兵产出", "征兵兵力产出+x%", "Dev", 21003, "AmountMul", "soldier");
            config[31023] = new TechSkillConfig(31023, "征兵消耗", "征兵金钱消耗-x%", "Dev", 21003, "CostReduce", "gold");
            config[31024] = new TechSkillConfig(31024, "征兵槽位", "征兵分配人数+x", "Dev", 21003, "SlotAdd", "slot");
            config[31031] = new TechSkillConfig(31031, "城墙效果", "加固城墙城防+x", "Dev", 21004, "AmountAdd", "wall");
            config[31032] = new TechSkillConfig(31032, "城墙效果", "加固城墙城防+x%", "Dev", 21004, "AmountMul", "wall");
            config[31033] = new TechSkillConfig(31033, "城墙消耗", "加固城墙金钱消耗-x%", "Dev", 21004, "CostReduce", "gold");
            config[31034] = new TechSkillConfig(31034, "城墙槽位", "加固城墙分配人数+x", "Dev", 21004, "SlotAdd", "slot");
            config[31041] = new TechSkillConfig(31041, "治安效果", "治安民心效果+x", "Dev", 21005, "AmountAdd", "happy");
            config[31042] = new TechSkillConfig(31042, "治安效果", "治安民心效果+x%", "Dev", 21005, "AmountMul", "happy");
            config[31043] = new TechSkillConfig(31043, "治安消耗", "治安金钱消耗-x%", "Dev", 21005, "CostReduce", "gold");
            config[31044] = new TechSkillConfig(31044, "治安槽位", "治安分配人数+x", "Dev", 21005, "SlotAdd", "slot");
            config[31045] = new TechSkillConfig(31045, "治安成功率", "治安成功率+x%", "Dev", 21005, "SuccessMul", "rate");
            config[31051] = new TechSkillConfig(31051, "采石场产出", "采石场石料产出+x", "Dev", 21403, "AmountAdd", "stone");
            config[31052] = new TechSkillConfig(31052, "采石场产出", "采石场石料产出+x%", "Dev", 21403, "AmountMul", "stone");
            config[31053] = new TechSkillConfig(31053, "采石场消耗", "采石场金钱消耗-x%", "Dev", 21403, "CostReduce", "gold");
            config[31054] = new TechSkillConfig(31054, "采石场槽位", "采石场分配人数+x", "Dev", 21403, "SlotAdd", "slot");
            config[31061] = new TechSkillConfig(31061, "马场产出", "马场马匹产出+x", "Dev", 21404, "AmountAdd", "horse");
            config[31062] = new TechSkillConfig(31062, "马场产出", "马场马匹产出+x%", "Dev", 21404, "AmountMul", "horse");
            config[31063] = new TechSkillConfig(31063, "马场消耗", "马场金钱消耗-x%", "Dev", 21404, "CostReduce", "gold");
            config[31064] = new TechSkillConfig(31064, "马场槽位", "马场分配人数+x", "Dev", 21404, "SlotAdd", "slot");
            config[31071] = new TechSkillConfig(31071, "木材场产出", "木材场木材产出+x", "Dev", 21405, "AmountAdd", "wood");
            config[31072] = new TechSkillConfig(31072, "木材场产出", "木材场木材产出+x%", "Dev", 21405, "AmountMul", "wood");
            config[31073] = new TechSkillConfig(31073, "木材场消耗", "木材场金钱消耗-x%", "Dev", 21405, "CostReduce", "gold");
            config[31074] = new TechSkillConfig(31074, "木材场槽位", "木材场分配人数+x", "Dev", 21405, "SlotAdd", "slot");
            config[31081] = new TechSkillConfig(31081, "铁匠铺产出", "铁匠铺铁矿产出+x", "Dev", 21406, "AmountAdd", "steel");
            config[31082] = new TechSkillConfig(31082, "铁匠铺产出", "铁匠铺铁矿产出+x%", "Dev", 21406, "AmountMul", "steel");
            config[31083] = new TechSkillConfig(31083, "铁匠铺消耗", "铁匠铺金钱消耗-x%", "Dev", 21406, "CostReduce", "gold");
            config[31084] = new TechSkillConfig(31084, "铁匠铺槽位", "铁匠铺分配人数+x", "Dev", 21406, "SlotAdd", "slot");
            config[31091] = new TechSkillConfig(31091, "象棚产出", "象棚战象产出+x", "Dev", 21407, "AmountAdd", "elephant");
            config[31092] = new TechSkillConfig(31092, "象棚产出", "象棚战象产出+x%", "Dev", 21407, "AmountMul", "elephant");
            config[31093] = new TechSkillConfig(31093, "象棚消耗", "象棚金钱消耗-x%", "Dev", 21407, "CostReduce", "gold");
            config[31094] = new TechSkillConfig(31094, "象棚槽位", "象棚分配人数+x", "Dev", 21407, "SlotAdd", "slot");
            config[31101] = new TechSkillConfig(31101, "金矿产出", "金矿金钱产出+x", "Dev", 21408, "AmountAdd", "gold");
            config[31102] = new TechSkillConfig(31102, "金矿产出", "金矿金钱产出+x%", "Dev", 21408, "AmountMul", "gold");
            config[31103] = new TechSkillConfig(31103, "金矿消耗", "金矿金钱消耗-x%", "Dev", 21408, "CostReduce", "gold");
            config[31104] = new TechSkillConfig(31104, "金矿槽位", "金矿分配人数+x", "Dev", 21408, "SlotAdd", "slot");
            config[31111] = new TechSkillConfig(31111, "盐矿金钱产出", "盐矿金钱产出+x", "Dev", 21409, "AmountAdd", "gold");
            config[31112] = new TechSkillConfig(31112, "盐矿金钱产出", "盐矿金钱产出+x%", "Dev", 21409, "AmountMul", "gold");
            config[31113] = new TechSkillConfig(31113, "盐矿兵力产出", "盐矿兵力产出+x", "Dev", 21409, "AmountAdd", "soldier");
            config[31114] = new TechSkillConfig(31114, "盐矿消耗", "盐矿金钱消耗-x%", "Dev", 21409, "CostReduce", "gold");
            config[31115] = new TechSkillConfig(31115, "盐矿槽位", "盐矿分配人数+x", "Dev", 21409, "SlotAdd", "slot");
            config[31121] = new TechSkillConfig(31121, "渔场金钱产出", "渔场金钱产出+x", "Dev", 21410, "AmountAdd", "gold");
            config[31122] = new TechSkillConfig(31122, "渔场金钱产出", "渔场金钱产出+x%", "Dev", 21410, "AmountMul", "gold");
            config[31123] = new TechSkillConfig(31123, "渔场粮食产出", "渔场粮食产出+x", "Dev", 21410, "AmountAdd", "food");
            config[31124] = new TechSkillConfig(31124, "渔场消耗", "渔场金钱消耗-x%", "Dev", 21410, "CostReduce", "gold");
            config[31125] = new TechSkillConfig(31125, "渔场槽位", "渔场分配人数+x", "Dev", 21410, "SlotAdd", "slot");
            config[31201] = new TechSkillConfig(31201, "走访成功率", "走访成功率+x%", "KingAction", 21601, "SuccessMul", "rate");
            config[31202] = new TechSkillConfig(31202, "走访消耗", "走访金钱消耗-x%", "KingAction", 21601, "CostReduce", "gold");
            config[31203] = new TechSkillConfig(31203, "走访槽位", "走访人数+x", "KingAction", 21601, "SlotAdd", "slot");
            config[31211] = new TechSkillConfig(31211, "交易汇率", "交易汇率+x%", "KingAction", 21602, "AmountMul", "rate");
            config[31212] = new TechSkillConfig(31212, "交易消耗", "交易金钱消耗-x%", "KingAction", 21602, "CostReduce", "gold");
            config[31213] = new TechSkillConfig(31213, "交易槽位", "交易人数+x", "KingAction", 21602, "SlotAdd", "slot");
            config[31221] = new TechSkillConfig(31221, "登用成功率", "登用成功率+x%", "KingAction", 21603, "SuccessMul", "rate");
            config[31222] = new TechSkillConfig(31222, "登用消耗", "登用金钱消耗-x%", "KingAction", 21603, "CostReduce", "gold");
            config[31223] = new TechSkillConfig(31223, "登用槽位", "登用人数+x", "KingAction", 21603, "SlotAdd", "slot");
            config[31231] = new TechSkillConfig(31231, "褒奖效果", "褒奖忠心效果+x%", "KingAction", 21604, "AmountMul", "loyalty");
            config[31232] = new TechSkillConfig(31232, "褒奖消耗", "褒奖金钱消耗-x%", "KingAction", 21604, "CostReduce", "gold");
            config[31233] = new TechSkillConfig(31233, "褒奖槽位", "褒奖人数+x", "KingAction", 21604, "SlotAdd", "slot");
            config[31241] = new TechSkillConfig(31241, "奖赏效果", "奖赏忠心效果+x%", "KingAction", 21605, "AmountMul", "loyalty");
            config[31242] = new TechSkillConfig(31242, "奖赏消耗", "奖赏金钱消耗-x%", "KingAction", 21605, "CostReduce", "gold");
            config[31243] = new TechSkillConfig(31243, "奖赏槽位", "奖赏人数+x", "KingAction", 21605, "SlotAdd", "slot");
            config[31251] = new TechSkillConfig(31251, "破坏效果", "破坏城防效果+x%", "KingAction", 21606, "AmountMul", "wall");
            config[31252] = new TechSkillConfig(31252, "破坏成功率", "破坏成功率+x%", "KingAction", 21606, "SuccessMul", "rate");
            config[31253] = new TechSkillConfig(31253, "破坏槽位", "破坏人数+x", "KingAction", 21606, "SlotAdd", "slot");
            config[31261] = new TechSkillConfig(31261, "扰乱效果", "扰乱民心效果+x%", "KingAction", 21607, "AmountMul", "happy");
            config[31262] = new TechSkillConfig(31262, "扰乱成功率", "扰乱成功率+x%", "KingAction", 21607, "SuccessMul", "rate");
            config[31263] = new TechSkillConfig(31263, "扰乱槽位", "扰乱人数+x", "KingAction", 21607, "SlotAdd", "slot");
            config[31271] = new TechSkillConfig(31271, "外交效果", "亲善效果+x%", "KingAction", 21608, "AmountMul", "relation");
            config[31272] = new TechSkillConfig(31272, "外交消耗", "外交金钱消耗-x%", "KingAction", 21608, "CostReduce", "gold");
            config[31273] = new TechSkillConfig(31273, "外交槽位", "外交人数+x", "KingAction", 21608, "SlotAdd", "slot");
            config[31281] = new TechSkillConfig(31281, "挑拨效果", "挑拨效果+x%", "KingAction", 21609, "AmountMul", "relation");
            config[31282] = new TechSkillConfig(31282, "挑拨成功率", "挑拨成功率+x%", "KingAction", 21609, "SuccessMul", "rate");
            config[31283] = new TechSkillConfig(31283, "挑拨槽位", "挑拨人数+x", "KingAction", 21609, "SlotAdd", "slot");
            config[31291] = new TechSkillConfig(31291, "移动人数", "单次移动英雄数+x", "KingAction", 21501, "SlotAdd", "slot");
            config[31292] = new TechSkillConfig(31292, "英雄移动效率", "英雄移动效率+x%", "KingAction", 21501, "AmountMul", "heroMove");
            config[31293] = new TechSkillConfig(31293, "部队移动效率", "部队移动效率+x%", "KingAction", 21501, "AmountMul", "armyMove");
            config[31301] = new TechSkillConfig(31301, "动员兵攻击", "动员兵攻击力+x", "Arms", 1, "ArmsAttrAdd", "Atk");
            config[31302] = new TechSkillConfig(31302, "动员兵防御", "动员兵防御力+x", "Arms", 1, "ArmsAttrAdd", "Def");
            config[31311] = new TechSkillConfig(31311, "骑兵攻击", "骑兵攻击力+x", "Arms", 101, "ArmsAttrAdd", "Atk");
            config[31312] = new TechSkillConfig(31312, "骑兵防御", "骑兵防御力+x", "Arms", 101, "ArmsAttrAdd", "Def");
            config[31313] = new TechSkillConfig(31313, "骑兵移速", "骑兵移动力+x", "Arms", 101, "ArmsAttrAdd", "MoveSpeed");
            config[31321] = new TechSkillConfig(31321, "象兵攻击", "象兵攻击力+x", "Arms", 109, "ArmsAttrAdd", "Atk");
            config[31322] = new TechSkillConfig(31322, "象兵防御", "象兵防御力+x", "Arms", 109, "ArmsAttrAdd", "Def");
            config[31331] = new TechSkillConfig(31331, "弓兵攻击", "弓兵攻击力+x", "Arms", 201, "ArmsAttrAdd", "Atk");
            config[31332] = new TechSkillConfig(31332, "弓兵防御", "弓兵防御力+x", "Arms", 201, "ArmsAttrAdd", "Def");
            config[31341] = new TechSkillConfig(31341, "刀兵攻击", "刀兵攻击力+x", "Arms", 601, "ArmsAttrAdd", "Atk");
            config[31342] = new TechSkillConfig(31342, "刀兵防御", "刀兵防御力+x", "Arms", 601, "ArmsAttrAdd", "Def");
            config[31351] = new TechSkillConfig(31351, "枪兵攻击", "枪兵攻击力+x", "Arms", 602, "ArmsAttrAdd", "Atk");
            config[31352] = new TechSkillConfig(31352, "枪兵防御", "枪兵防御力+x", "Arms", 602, "ArmsAttrAdd", "Def");
            config[31361] = new TechSkillConfig(31361, "戟兵攻击", "戟兵攻击力+x", "Arms", 603, "ArmsAttrAdd", "Atk");
            config[31362] = new TechSkillConfig(31362, "戟兵防御", "戟兵防御力+x", "Arms", 603, "ArmsAttrAdd", "Def");
            config[31371] = new TechSkillConfig(31371, "藤甲防御", "藤甲兵防御力+x", "Arms", 604, "ArmsAttrAdd", "Def");
            config[31401] = new TechSkillConfig(31401, "逃脱概率", "逃脱概率+x%", "SysConfig", 22002, "AmountMul", "val");

            RebuildIndex();

        }

        private static void RebuildIndex()
        {
            foreach (var kv in config)
            {
            }
        }

        public static TechSkillConfig GetConfig(int id)
        {
            TechSkillConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表TechSkillConfig不存在id={0}", id));
        }


        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, TechSkillConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, TechSkillConfig configData)
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
