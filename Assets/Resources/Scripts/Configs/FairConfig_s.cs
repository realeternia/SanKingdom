using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class FairConfig
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
            {"Title", new FieldMetaInfo("标题", "string")},
            {"Des", new FieldMetaInfo("描述模板", "string")},
            {"Image", new FieldMetaInfo("图片名", "string")},
            {"Bg", new FieldMetaInfo("背景音乐", "string")},
        };

        public static Dictionary<string, FieldMetaInfo> FieldMeta { get { return fieldMeta; } }

        /// <summary>
        /// 序列
        /// </summary>
        public int Id;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title;
        /// <summary>
        /// 描述模板，{forceName} 替换为势力名（富文本颜色）
        /// </summary>
        public string Des;
        /// <summary>
        /// 图片名，路径 Textures/Fairs/
        /// </summary>
        public string Image;
        /// <summary>
        /// 背景音乐名，路径 BGMs/
        /// </summary>
        public string Bg;

        public FairConfig(int Id, string Title, string Des, string Image, string Bg)
        {
            this.Id = Id;
            this.Title = Title;
            this.Des = Des;
            this.Image = Image;
            this.Bg = Bg;
        }

        public FairConfig() { }

        private static Dictionary<int, FairConfig> config = new Dictionary<int, FairConfig>();
        public static Dictionary<int, FairConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, FairConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new FairConfig(1, "势力灭亡", "{forceName}势力已被消灭！", "forceover", "forceover");
        }

        public static FairConfig GetConfig(int id)
        {
            FairConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表FairConfig不存在id={0}", id));
        }

        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, FairConfig configData)
        {
            config[id] = configData;
        }

        public static void Add(int id, FairConfig configData)
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
