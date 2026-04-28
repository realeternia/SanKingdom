using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class SeasonConfig
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
            {"Season", new FieldMetaInfo("季节", "string")},
            {"BGM", new FieldMetaInfo("音乐", "string")},
            {"Video", new FieldMetaInfo("视频", "string")},
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
        ///季节
        /// </summary>
        public string Season;
        /// <summary>
        ///音乐
        /// </summary>
        public string BGM;
        /// <summary>
        ///视频
        /// </summary>
        public string Video;


        public SeasonConfig(int Id, string Name, string Season, string BGM, string Video)
        {
            this.Id = Id;
            this.Name = Name;
            this.Season = Season;
            this.BGM = BGM;
            this.Video = Video;

        }

        public SeasonConfig() { }

        private static Dictionary<int, SeasonConfig> config = new Dictionary<int, SeasonConfig>();
        public static Dictionary<int, SeasonConfig>.ValueCollection ConfigList
        {
            get { return config.Values; }
        }

        public static void Refresh(Dictionary<int, SeasonConfig> dict)
        {
            config.Clear();
            config = dict;
        }

        public static void Load()
        {
            config.Clear();
            config[1] = new SeasonConfig(1, "一月上", "冬", "dong", "goldin.mp4");
            config[2] = new SeasonConfig(2, "一月中", "冬", "dong", "");
            config[3] = new SeasonConfig(3, "一月下", "冬", "dong", "");
            config[4] = new SeasonConfig(4, "二月上", "冬", "dong", "goldin.mp4");
            config[5] = new SeasonConfig(5, "二月中", "冬", "dong", "");
            config[6] = new SeasonConfig(6, "二月下", "冬", "dong", "");
            config[7] = new SeasonConfig(7, "三月上", "春", "chun", "goldin.mp4");
            config[8] = new SeasonConfig(8, "三月中", "春", "chun", "");
            config[9] = new SeasonConfig(9, "三月下", "春", "chun", "");
            config[10] = new SeasonConfig(10, "四月上", "春", "chun", "goldin.mp4");
            config[11] = new SeasonConfig(11, "四月中", "春", "chun", "");
            config[12] = new SeasonConfig(12, "四月下", "春", "chun", "harve2.mp4");
            config[13] = new SeasonConfig(13, "五月上", "春", "chun", "goldin.mp4");
            config[14] = new SeasonConfig(14, "五月中", "春", "chun", "");
            config[15] = new SeasonConfig(15, "五月下", "春", "chun", "");
            config[16] = new SeasonConfig(16, "六月上", "夏", "xia", "goldin.mp4");
            config[17] = new SeasonConfig(17, "六月中", "夏", "xia", "");
            config[18] = new SeasonConfig(18, "六月下", "夏", "xia", "");
            config[19] = new SeasonConfig(19, "七月上", "夏", "xia", "goldin.mp4");
            config[20] = new SeasonConfig(20, "七月中", "夏", "xia", "");
            config[21] = new SeasonConfig(21, "七月下", "夏", "xia", "harve2.mp4");
            config[22] = new SeasonConfig(22, "八月上", "夏", "xia", "goldin.mp4");
            config[23] = new SeasonConfig(23, "八月中", "夏", "xia", "");
            config[24] = new SeasonConfig(24, "八月下", "夏", "xia", "");
            config[25] = new SeasonConfig(25, "九月上", "秋", "qiu", "goldin.mp4");
            config[26] = new SeasonConfig(26, "九月中", "秋", "qiu", "");
            config[27] = new SeasonConfig(27, "九月下", "秋", "qiu", "");
            config[28] = new SeasonConfig(28, "十月上", "秋", "qiu", "goldin.mp4");
            config[29] = new SeasonConfig(29, "十月中", "秋", "qiu", "");
            config[30] = new SeasonConfig(30, "十月下", "秋", "qiu", "harve2.mp4");
            config[31] = new SeasonConfig(31, "十一月上", "秋", "qiu", "goldin.mp4");
            config[32] = new SeasonConfig(32, "十一月中", "秋", "qiu", "");
            config[33] = new SeasonConfig(33, "十一月下", "秋", "qiu", "");
            config[34] = new SeasonConfig(34, "十二月上", "冬", "dong", "goldin.mp4");
            config[35] = new SeasonConfig(35, "十二月中", "冬", "dong", "");
            config[36] = new SeasonConfig(36, "十二月下", "冬", "dong", "");



        }

        public static SeasonConfig GetConfig(int id)
        {
            SeasonConfig data;
            if (config.TryGetValue(id, out data))
            {
                return data;
            }
            throw new NullReferenceException(string.Format("配置表SeasonConfig不存在id={0}", id));
        }



        public static bool HasConfig(int id)
        {
            if (config.ContainsKey(id))
            {
                return true;
            }
            return false;
        }

        public static void Assign(int id, SeasonConfig configData)
        {
            config[id] = configData; 
        }

        public static void Add(int id, SeasonConfig configData)
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
