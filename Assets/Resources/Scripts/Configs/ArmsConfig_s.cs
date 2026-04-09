using System;
using System.Collections;
using System.Collections.Generic;

namespace CommonConfig
{
    public class ArmsConfig
    {
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
        ///强克制
        /// </summary>
        public string OvercomeStrong;
        /// <summary>
        ///弱克制
        /// </summary>
        public string OvercomeWeak;


        public ArmsConfig(int Id, string Name, string NameS, int MoveSpeed, int Range, int MissileSpeed, float MissileHight, string HitEffect, string OvercomeStrong, string OvercomeWeak)
        {
            this.Id = Id;
            this.Name = Name;
            this.NameS = NameS;
            this.MoveSpeed = MoveSpeed;
            this.Range = Range;
            this.MissileSpeed = MissileSpeed;
            this.MissileHight = MissileHight;
            this.HitEffect = HitEffect;
            this.OvercomeStrong = OvercomeStrong;
            this.OvercomeWeak = OvercomeWeak;

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
        }

        public static void Load()
        {
            config.Clear();
            config[101] = new ArmsConfig(101, "ma", "马", 10, 17, 0, 0, "SwordHitYellowCritical", "戟弩炮车", "弓");
            config[102] = new ArmsConfig(102, "che", "车", 10, 17, 0, 0, "SwordHitGreenCritical", "", "");
            config[201] = new ArmsConfig(201, "gong", "弓", 10, 40, 40, 5f, "BulletExplosionBlue", "枪戟", "刀");
            config[202] = new ArmsConfig(202, "pao", "炮", 10, 17, 0, 0, "SwordHitYellowCritical", "盾", "士");
            config[601] = new ArmsConfig(601, "dao", "刀", 10, 17, 0, 0, "SwordHitYellowCritical", "马车", "");
            config[602] = new ArmsConfig(602, "daoqiang", "枪", 10, 17, 0, 0, "SwordHitYellowCritical", "枪", "");
            config[603] = new ArmsConfig(603, "daoji", "戟", 10, 40, 30, 3f, "FanExplosion", "", "");
            config[701] = new ArmsConfig(701, "shan", "扇", 10, 40, 30, 3f, "GasExplosionFire", "", "");
            config[702] = new ArmsConfig(702, "mou", "谋", 7, 50, 26, 8f, "GasShootFire", "", "");



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
