using System;
using System.Collections.Generic;

namespace CommonConfig
{
    public class HeroAttrConfig
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
            {"name", new FieldMetaInfo("名字", "string")},
            {"Cname", new FieldMetaInfo("中文名", "string")},
            {"ColorRule", new FieldMetaInfo("颜色规则", "string")},
            {"Icon", new FieldMetaInfo("icon", "string")},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        public int Id;
        public string name;
        public string Cname;
        public string ColorRule;
        public string Icon;

        public HeroAttrConfig(int Id, string name, string Cname, string ColorRule, string Icon)
        {
            this.Id = Id;
            this.name = name;
            this.Cname = Cname;
            this.ColorRule = ColorRule;
            this.Icon = Icon;
        }

        public HeroAttrConfig() { }

        private static Dictionary<int, HeroAttrConfig> config = new Dictionary<int, HeroAttrConfig>();
        public static Dictionary<int, HeroAttrConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, HeroAttrConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new HeroAttrConfig(1, "str", "武力", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "");
            config[2] = new HeroAttrConfig(2, "inte", "智力", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "");
            config[3] = new HeroAttrConfig(3, "fair", "内政", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "");
            config[4] = new HeroAttrConfig(4, "charm", "魅力", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "");
            config[5] = new HeroAttrConfig(5, "leadShip", "统帅", "95:#FF0000,90:#FFFF00,0:#FFFFFF", "");
            config[6] = new HeroAttrConfig(6, "loyalty", "忠诚度", "80:#FFFFFF,50:#FFA500,0:#FF0000", "");
            config[7] = new HeroAttrConfig(7, "weightedAttr", "加权属性", "90:#FF0000,80:#FFFF00,70:#00FF00,0:#FFFFFF", "");

            idxname["str"] = 1;
            idxCname["武力"] = 1;
            idxname["inte"] = 2;
            idxCname["智力"] = 2;
            idxname["fair"] = 3;
            idxCname["内政"] = 3;
            idxname["charm"] = 4;
            idxCname["魅力"] = 4;
            idxname["leadShip"] = 5;
            idxCname["统帅"] = 5;
            idxname["loyalty"] = 6;
            idxCname["忠诚度"] = 6;
            idxname["weightedattr"] = 7;
            idxCname["加权属性"] = 7;
        }

        public static HeroAttrConfig GetConfig(int id)
        {
            HeroAttrConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表HeroAttrConfig不存在id={0}", id));
        }

        private static Dictionary<string, int> idxname = new Dictionary<string, int>();
        public static HeroAttrConfig GetConfigByname(string val)
        {
            if (idxname.TryGetValue(val.ToLower(), out int id))
            {
                return GetConfig(id);
            }
            return null;
        }

        private static Dictionary<string, int> idxCname = new Dictionary<string, int>();
        public static HeroAttrConfig GetConfigByCname(string val)
        {
            if (idxCname.TryGetValue(val, out int id))
            {
                return GetConfig(id);
            }
            return null;
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, HeroAttrConfig configData)
        {
            config[id] = configData;
        }

        public static void Add(int id, HeroAttrConfig configData)
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
