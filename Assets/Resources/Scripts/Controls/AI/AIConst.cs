/// <summary>
/// AI相关常量
/// </summary>
public static class AIConst
{
    public static class AIStrategy
    {
        /// <summary>
        /// 最大攻击城市数
        /// </summary>
        public const int MAX_ATK_CITIES = 2;
        /// <summary>
        /// 攻击所需最小资源
        /// </summary>
        public const int MIN_RESOURCE_FOR_ATTACK = 1500;
        /// <summary>
        /// 攻击所需最小士兵
        /// </summary>
        public const int MIN_SOLDIER_FOR_ATTACK = 3000;
        /// <summary>
        /// 城市攻击所需最小士兵
        /// </summary>
        public const int MIN_CITY_SOLDIER_FOR_ATTACK = 200;
        /// <summary>
        /// 城市攻击所需最小英雄数
        /// </summary>
        public const int MIN_CITY_HEROES_FOR_ATTACK = 3;
        /// <summary>
        /// 每个英雄最大士兵数
        /// </summary>
        public const int MAX_SOLDIER_PER_HERO = 100;
        /// <summary>
        /// AI最小攻击士兵总数
        /// </summary>
        public const int AI_MIN_ATTACK_SOLDIER = 100;
        /// <summary>
        /// AI攻击最小部队数
        /// </summary>
        public const int MIN_ATTACK_TROOPS = 2;
        /// <summary>
        /// AI粮草需求除数
        /// </summary>
        public const int AI_FOOD_NEED_DIVISOR = 2;
        /// <summary>
        /// 前线战斗英雄目标数
        /// </summary>
        public const int FRONTLINE_COMBAT_HEROES_TARGET = 3;
        /// <summary>
        /// AI攻击源优势比率
        /// </summary>
        public const float AI_ATTACK_SOURCE_ADVANTAGE_RATIO = 0.7f;
        /// <summary>
        /// AI己城攻击优势比率
        /// </summary>
        public const float AI_OWN_CITY_ATTACK_ADVANTAGE_RATIO = 0.8f;
        /// <summary>
        /// AI攻击粮草除数
        /// </summary>
        public const int AI_ATTACK_FOOD_DIVISOR = 2;
        /// <summary>
        /// AI威胁敌方士兵阈值
        /// </summary>
        public const int AI_THREAT_ENEMY_SOLDIER_THRESHOLD = 500;
        /// <summary>
        /// 军团最大英雄数
        /// </summary>
        public const int TROOP_MAX_HEROES = 3;
        /// <summary>
        /// 军团主将高统帅阈值
        /// </summary>
        public const int TROOP_COMMANDER_LEADSHIP_THRESHOLD = 50;
        /// <summary>
        /// 军团士兵比例（士兵数/此值=军团上限）
        /// </summary>
        public const int TROOP_SOLDIER_PER_CORPS = 60;
        /// <summary>
        /// 武将充足时军团士兵比例（放宽）
        /// </summary>
        public const int TROOP_SOLDIER_PER_CORPS_RELAXED = 40;
        /// <summary>
        /// 武将充足阈值（超过此数开始放宽士兵比例）
        /// </summary>
        public const int TROOP_HERO_RICH_THRESHOLD = 6;
        /// <summary>
        /// 武将极度充足阈值（超过此数士兵比例降至最低）
        /// </summary>
        public const int TROOP_HERO_FULL_RICH_THRESHOLD = 12;
        /// <summary>
        /// 资源生产派遣随机重洗间隔（回合）
        /// </summary>
        public const int TROOP_RES_RESHUFFLE_INTERVAL = 10;
        /// <summary>
        /// 城市军团硬上限
        /// </summary>
        public const int TROOP_CITY_HARD_LIMIT = 5;
        /// <summary>
        /// 兵种适配-骑兵统帅权重
        /// </summary>
        public const float ARMS_FIT_HORSE_WEIGHT = 0.7f;
        /// <summary>
        /// 兵种适配-骑兵武力权重
        /// </summary>
        public const float ARMS_FIT_HORSE_STR_WEIGHT = 0.3f;
        /// <summary>
        /// 兵种适配-弓兵智力权重
        /// </summary>
        public const float ARMS_FIT_BOW_WEIGHT = 0.8f;
        /// <summary>
        /// 兵种适配-步兵武力权重
        /// </summary>
        public const float ARMS_FIT_WALK_WEIGHT = 0.7f;
        /// <summary>
        /// 兵种适配-步兵统帅权重
        /// </summary>
        public const float ARMS_FIT_WALK_LEADSHIP_WEIGHT = 0.2f;
        /// <summary>
        /// 兵种适配-基础分微调权重
        /// </summary>
        public const float ARMS_BASE_STAT_WEIGHT = 0.02f;
    }

    public static class AIHero
    {
        /// <summary>
        /// 战斗英雄阈值
        /// </summary>
        public const int COMBAT_THRESHOLD = 150;
        /// <summary>
        /// 内政英雄阈值
        /// </summary>
        public const int DOMESTIC_THRESHOLD = 150;
        /// <summary>
        /// 后方最小英雄数
        /// </summary>
        public const int MIN_REAR_HEROES = 1;
        /// <summary>
        /// 补充英雄最大拉人数
        /// </summary>
        public const int FILL_HERO_MAX_PULL = 3;
    }

    public static class AIKingAction
    {
        /// <summary>
        /// 褒奖忠诚阈值（＜此值需褒奖）
        /// </summary>
        public const int PRAISE_LOYALTY_THRESHOLD = 95;
        /// <summary>
        /// 登庸属性阈值（魅力或智力＞此值可登庸）
        /// </summary>
        public const int RECRUIT_ATTR_THRESHOLD = 80;
        /// <summary>
        /// 登庸最大人次
        /// </summary>
        public const int MAX_RECRUIT_COUNT = 6;
    }
}