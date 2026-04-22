public static class SystemConst
{
    public static class Game
    {
        /// <summary>
        /// 游戏开始的基准年份
        /// </summary>
        public const int BASE_YEAR = 194;
        /// <summary>
        /// 英雄出生年龄
        /// </summary>
        public const int BORN_AGE = 16;
        /// <summary>
        /// 每年的季节数
        /// </summary>
        public const int SEASONS_PER_YEAR = 36;
        /// <summary>
        /// 最大势力ID
        /// </summary>
        public const int MAX_FORCE_ID = 90;
    }

    public static class Hero
    {
        /// <summary>
        /// 最大忠诚度
        /// </summary>
        public const int MAX_LOYALTY = 100;
        /// <summary>
        /// 野外英雄默认忠诚度
        /// </summary>
        public const int WILD_HERO_DEFAULT_LOYALTY = 90;
        /// <summary>
        /// 登用成功忠诚度
        /// </summary>
        public const int RECRUIT_SUCCESS_LOYALTY = 85;
        /// <summary>
        /// 灭国英雄忠诚度
        /// </summary>
        public const int ELIMINATED_HERO_LOYALTY = 90;
        /// <summary>
        /// 低忠诚度阈值
        /// </summary>
        public const int LOW_LOYALTY_THRESHOLD = 80;
        /// <summary>
        /// 登用敌方英雄忠诚度阈值
        /// </summary>
        public const int RECRUIT_ENEMY_LOYALTY_THRESHOLD = 95;
        /// <summary>
        /// 默认兵种ID
        /// </summary>
        public const int DEFAULT_ARMS_ID = 601;
        /// <summary>
        /// 野外势力ID
        /// </summary>
        public const int WILD_FORCE_ID = 0;
        /// <summary>
        /// 每个英雄最大士兵数
        /// </summary>
        public const int MAX_SOLDIER_PER_HERO = 1000;
        /// <summary>
        /// 登用野外英雄基础概率
        /// </summary>
        public const int RECRUIT_WILD_BASE_RATE = 30;
        /// <summary>
        /// 登用俘虏公式参数A
        /// </summary>
        public const int RECRUIT_CAPTURED_FORMULA_A = 22;
        /// <summary>
        /// 登用俘虏公式参数B
        /// </summary>
        public const int RECRUIT_CAPTURED_FORMULA_B = 8;
        /// <summary>
        /// 魅力加成第一档
        /// </summary>
        public const int CHARM_BONUS_TIER1 = 90;
        /// <summary>
        /// 魅力加成第二档
        /// </summary>
        public const int CHARM_BONUS_TIER2 = 80;
        /// <summary>
        /// 登用第一档倍率
        /// </summary>
        public const int RECRUIT_TIER1_MULTIPLIER = 130;
        /// <summary>
        /// 登用第二档倍率
        /// </summary>
        public const int RECRUIT_TIER2_MULTIPLIER = 115;
        /// <summary>
        /// 君主登用倍率
        /// </summary>
        public const int KING_RECRUIT_MULTIPLIER = 110;
        /// <summary>
        /// 俘虏逃跑概率
        /// </summary>
        public const int CAPTURED_ESCAPE_CHANCE = 20;
        /// <summary>
        /// 俘虏忠诚度下降最小值
        /// </summary>
        public const int CAPTURED_LOYALTY_DECAY_MIN = 1;
        /// <summary>
        /// 俘虏忠诚度下降最大值
        /// </summary>
        public const int CAPTURED_LOYALTY_DECAY_MAX = 4;
        /// <summary>
        /// 野外英雄移动概率
        /// </summary>
        public const int WILD_HERO_MOVE_CHANCE = 20;
        /// <summary>
        /// 褒奖每个英雄花费金币
        /// </summary>
        public const int PRAISE_GOLD_COST_PER_HERO = 100;
        /// <summary>
        /// 褒奖忠诚度增加最小值
        /// </summary>
        public const int PRAISE_LOYALTY_ADD_MIN = 1;
        /// <summary>
        /// 褒奖忠诚度增加最大值
        /// </summary>
        public const int PRAISE_LOYALTY_ADD_MAX = 4;
        /// <summary>
        /// 赏赐忠诚度增加最小值
        /// </summary>
        public const int REWARD_LOYALTY_ADD_MIN = 3;
        /// <summary>
        /// 赏赐忠诚度增加最大值
        /// </summary>
        public const int REWARD_LOYALTY_ADD_MAX = 6;
        /// <summary>
        /// 每级最小属性成长
        /// </summary>
        public const int MIN_ATTR_PER_LEVEL = 8;
        /// <summary>
        /// 属性成长除数
        /// </summary>
        public const int ATTR_GROWTH_DIVISOR = 10;
        /// <summary>
        /// 英雄分类优势比率
        /// </summary>
        public const float HERO_CLASSIFY_ADVANTAGE_RATIO = 1.3f;
        /// <summary>
        /// 次要属性贡献除数
        /// </summary>
        public const int SECONDARY_ATTR_CONTRIBUTION_DIVISOR = 3;
    }

    public static class City
    {
        /// <summary>
        /// 城市初始民心值
        /// </summary>
        public const int INITIAL_CITY_HAPPY = 70;
        /// <summary>
        /// 每级金币产出
        /// </summary>
        public const int GOLD_PER_LEVEL = 50;
        /// <summary>
        /// 每级粮草产出
        /// </summary>
        public const int FOOD_PER_LEVEL = 40;
        /// <summary>
        /// 太守评分武力权重
        /// </summary>
        public const float OWNER_SCORE_WEIGHT_STR = 0.75f;
        /// <summary>
        /// 太守评分统率权重
        /// </summary>
        public const float OWNER_SCORE_WEIGHT_LEADSHIP = 1.5f;
        /// <summary>
        /// 太守评分魅力权重
        /// </summary>
        public const float OWNER_SCORE_WEIGHT_CHARM = 1.2f;
        /// <summary>
        /// 君主担任太守的评分加成
        /// </summary>
        public const int KING_OWNER_BONUS_SCORE = 9999;
    }

    public static class Economy
    {
        /// <summary>
        /// 金粮兑换比率
        /// </summary>
        public const float EXCHANGE_RATE = 0.9f;
        /// <summary>
        /// 兑换最小金币数
        /// </summary>
        public const int EXCHANGE_MIN_GOLD = 300;
    }

    public static class Expedition
    {
        /// <summary>
        /// 默认携带粮草天数
        /// </summary>
        public const int DEFAULT_FOOD_DAYS = 10;
        /// <summary>
        /// 默认选中携带粮草天数
        /// </summary>
        public const int DEFAULT_SELECTED_FOOD_DAYS = 20;
        /// <summary>
        /// 士兵粮草消耗除数
        /// </summary>
        public const int SOLDIER_FOOD_COST_DIVISOR = 20;
        /// <summary>
        /// 俘虏基础概率
        /// </summary>
        public const int CATCH_BASE_CHANCE = 7;
        /// <summary>
        /// 俘虏武力因子
        /// </summary>
        public const int CATCH_STR_FACTOR = 8;
    }

    public static class Battle
    {
        /// <summary>
        /// 网格单元格大小
        /// </summary>
        public const int GRID_CELL_SIZE = 3;
        /// <summary>
        /// 最大回合数
        /// </summary>
        public const int MAX_ROUND = 30;
        /// <summary>
        /// 等待时间
        /// </summary>
        public const float WAIT_TIME = 1f;
        /// <summary>
        /// 战斗开始时间
        /// </summary>
        public const float BATTLE_BEGIN_TIME = 3f;
        /// <summary>
        /// 最大战斗数量
        /// </summary>
        public const int MAX_BATTLE_COUNT = 20;
        /// <summary>
        /// 每方最大战斗英雄数
        /// </summary>
        public const int MAX_BATTLE_HEROES_PER_SIDE = 12;
        /// <summary>
        /// 召唤批次阈值
        /// </summary>
        public const int SUMMON_BATCH_THRESHOLD = 6;
        /// <summary>
        /// 召唤英雄延迟帧数
        /// </summary>
        public const int SUMMON_HERO_DELAY_TICKS = 3;
        /// <summary>
        /// 攻击方出生延迟帧数
        /// </summary>
        public const int ATTACKER_SPAWN_DELAY_TICKS = 10;
        /// <summary>
        /// 粮草扣除间隔
        /// </summary>
        public const int FOOD_DEDUCTION_INTERVAL = 5;
        /// <summary>
        /// 粮草消耗除数
        /// </summary>
        public const int FOOD_COST_DIVISOR = 20;
        /// <summary>
        /// 魔法辅助单位ID
        /// </summary>
        public const int MAGIC_HELPER_UNIT_ID = 501001;
        /// <summary>
        /// 棋子碰撞大小
        /// </summary>
        public const float CHESS_COLLISION_SIZE = 7.5f;
        /// <summary>
        /// 默认暴击伤害倍率
        /// </summary>
        public const float DEFAULT_CRIT_DAMAGE_MULTI = 0.5f;
        /// <summary>
        /// 回血间隔帧数
        /// </summary>
        public const int REGE_INTERVAL_TICKS = 10;
        /// <summary>
        /// 缺粮基础伤害
        /// </summary>
        public const int LACK_FOOD_BASE_DAMAGE = 15;
        /// <summary>
        /// 缺粮伤害递增值
        /// </summary>
        public const int LACK_FOOD_DAMAGE_INCREMENT = 5;
        /// <summary>
        /// 目标更新间隔帧数
        /// </summary>
        public const int TARGET_UPDATE_INTERVAL_TICKS = 30;
        /// <summary>
        /// 攻击点阈值
        /// </summary>
        public const int ATTACK_POINT_THRESHOLD = 20;
        /// <summary>
        /// 攻击点消耗
        /// </summary>
        public const int ATTACK_POINT_COST = 20;
        /// <summary>
        /// 远程攻击阈值
        /// </summary>
        public const int RANGE_ATTACK_THRESHOLD = 20;
        /// <summary>
        /// 移动点阈值
        /// </summary>
        public const int MOVE_POINT_THRESHOLD = 10;
        /// <summary>
        /// 移动点消耗
        /// </summary>
        public const int MOVE_POINT_COST = 10;
        /// <summary>
        /// 移动距离因子
        /// </summary>
        public const float MOVE_DISTANCE_FACTOR = 0.5f;
        /// <summary>
        /// 最小攻击伤害
        /// </summary>
        public const int MIN_ATTACK_DAMAGE = 10;
        /// <summary>
        /// 最大攻击伤害
        /// </summary>
        public const int MAX_ATTACK_DAMAGE = 60;
        /// <summary>
        /// 等级差最小伤害下限
        /// </summary>
        public const int LEVEL_DIFF_MIN_DAMAGE_MIN = 8;
        /// <summary>
        /// 等级差最小伤害上限
        /// </summary>
        public const int LEVEL_DIFF_MIN_DAMAGE_MAX = 20;
        /// <summary>
        /// 等级差最大伤害因子
        /// </summary>
        public const int LEVEL_DIFF_MAX_DAMAGE_FACTOR = 4;
        /// <summary>
        /// 等级差最大伤害下限
        /// </summary>
        public const int LEVEL_DIFF_MAX_DAMAGE_MIN = 40;
        /// <summary>
        /// 等级差最大伤害上限
        /// </summary>
        public const int LEVEL_DIFF_MAX_DAMAGE_MAX = 80;
        /// <summary>
        /// 血量转攻击除数
        /// </summary>
        public const int HP_TO_ATK_DIVISOR = 50;
        /// <summary>
        /// 基础伤害
        /// </summary>
        public const int BASE_DAMAGE = 30;
        /// <summary>
        /// 伤害战力差除数
        /// </summary>
        public const int DAMAGE_POWER_DIFF_DIVISOR = 2;
        /// <summary>
        /// 战斗中帧数阈值
        /// </summary>
        public const int IN_FIGHT_TICK_THRESHOLD = 3;
        /// <summary>
        /// 被攻击buff最小伤害
        /// </summary>
        public const int ATTACKED_BUFF_MIN_DAMAGE = 10;
        /// <summary>
        /// Buff最小伤害阈值
        /// </summary>
        public const int BUFF_MIN_DAMAGE_THRESHOLD = 10;
        /// <summary>
        /// Buff最小伤害值
        /// </summary>
        public const int BUFF_MIN_DAMAGE_VALUE = 13;
        /// <summary>
        /// 盾牌属性压制比率
        /// </summary>
        public const float SHIELD_ATTR_SUPPRESS_RATIO = 1.2f;
        /// <summary>
        /// 盾牌被压制因子
        /// </summary>
        public const float SHIELD_SUPPRESSED_FACTOR = 0.75f;
        /// <summary>
        /// 治疗目标血量比率
        /// </summary>
        public const float HEAL_TARGET_HP_RATE = 0.8f;
        /// <summary>
        /// 传送距离
        /// </summary>
        public const int TELEPORT_DISTANCE = 12;
        /// <summary>
        /// 初始战斗ID
        /// </summary>
        public const int INITIAL_BATTLE_ID = 1000;
        /// <summary>
        /// 战斗统计UID倍数
        /// </summary>
        public const int BATTLE_STAT_UID_MULTIPLIER = 1000000;
        /// <summary>
        /// 爆发率属性上限
        /// </summary>
        public const float BURST_RATE_ATTR_CAP = 2f;
        /// <summary>
        /// 爆发率属性因子
        /// </summary>
        public const float BURST_RATE_ATTR_FACTOR = 0.02f;
        /// <summary>
        /// 周围攻击角度阈值
        /// </summary>
        public const int AROUND_ATTACK_ANGLE_THRESHOLD = 90;
        /// <summary>
        /// 墙壁偏移距离
        /// </summary>
        public const int WALL_OFFSET_DISTANCE = 10;
        /// <summary>
        /// 墙壁远距离偏移
        /// </summary>
        public const int WALL_OFFSET_DISTANCE_FAR = 20;
        /// <summary>
        /// 墙壁伤害区域扩展
        /// </summary>
        public const float WALL_DAMAGE_AREA_EXPAND = 1.5f;
        /// <summary>
        /// 目标评分英雄
        /// </summary>
        public const int TARGET_SCORE_HERO = 10;
        /// <summary>
        /// 目标评分非英雄
        /// </summary>
        public const int TARGET_SCORE_NONHERO = 30;
        /// <summary>
        /// 等级差评分权重
        /// </summary>
        public const float LEVEL_DIFF_SCORE_WEIGHT = 7f;
        /// <summary>
        /// 低血量阈值
        /// </summary>
        public const float LOW_HP_THRESHOLD = 0.5f;
        /// <summary>
        /// 低血量评分权重
        /// </summary>
        public const float LOW_HP_SCORE_WEIGHT = 100f;
        /// <summary>
        /// 低血量加成
        /// </summary>
        public const float LOW_HP_BONUS = 10f;
        /// <summary>
        /// 初始攻击点最小值
        /// </summary>
        public const int INIT_ATTACK_POINT_MIN = 1;
        /// <summary>
        /// 初始攻击点最大值
        /// </summary>
        public const int INIT_ATTACK_POINT_MAX = 10;
        /// <summary>
        /// 目标搜索额外距离
        /// </summary>
        public const float TARGET_SEARCH_EXTRA_RANGE = 10f;
        /// <summary>
        /// 目标评分选取数量
        /// </summary>
        public const int TARGET_SCORE_SELECT_COUNT = 3;
        /// <summary>
        /// 移动短距离尝试次数
        /// </summary>
        public const int MOVE_SHORT_ATTEMPT_COUNT = 4;
        /// <summary>
        /// 移动长距离尝试次数
        /// </summary>
        public const int MOVE_LONG_ATTEMPT_COUNT = 10;
        /// <summary>
        /// 寻路偏移角度-低
        /// </summary>
        public const float PATHFIND_ANGLE_OFFSET_LOW = 45f;
        /// <summary>
        /// 寻路偏移角度-中
        /// </summary>
        public const float PATHFIND_ANGLE_OFFSET_MID = 70f;
        /// <summary>
        /// 寻路偏移角度-高
        /// </summary>
        public const float PATHFIND_ANGLE_OFFSET_HIGH = 90f;
        /// <summary>
        /// 寻路失败次数-低角度阈值
        /// </summary>
        public const int PATHFIND_FAIL_COUNT_LOW = 3;
        /// <summary>
        /// 寻路失败次数-中角度阈值
        /// </summary>
        public const int PATHFIND_FAIL_COUNT_MID = 5;
    }

    public static class WorldMap
    {
        /// <summary>
        /// 地图缩放因子
        /// </summary>
        public const float MAP_SCALE_FACTOR = 1.25f;
    }

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
        public const int MIN_CITY_SOLDIER_FOR_ATTACK = 5000;
        /// <summary>
        /// 城市攻击所需最小英雄数
        /// </summary>
        public const int MIN_CITY_HEROES_FOR_ATTACK = 3;
        /// <summary>
        /// 每个英雄最大士兵数
        /// </summary>
        public const int MAX_SOLDIER_PER_HERO = 1000;
        /// <summary>
        /// AI最小攻击士兵数
        /// </summary>
        public const int AI_MIN_ATTACK_SOLDIER = 500;
        /// <summary>
        /// AI攻击优势比率
        /// </summary>
        public const float AI_ATTACK_ADVANTAGE_RATIO = 0.7f;
        /// <summary>
        /// AI粮草需求除数
        /// </summary>
        public const int AI_FOOD_NEED_DIVISOR = 2;
        /// <summary>
        /// AI购买粮草最小金币
        /// </summary>
        public const int AI_BUY_FOOD_MIN_GOLD = 300;
        /// <summary>
        /// AI褒奖忠诚度阈值
        /// </summary>
        public const int AI_PRAISE_LOYALTY_THRESHOLD = 80;
        /// <summary>
        /// AI登用敌方忠诚度阈值
        /// </summary>
        public const int AI_RECRUIT_ENEMY_LOYALTY_THRESHOLD = 80;
        /// <summary>
        /// AI最小留守英雄数
        /// </summary>
        public const int AI_MIN_STAY_HEROES = 3;
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
        /// 任务优先级权重
        /// </summary>
        public const float TASK_PRIORITY_WEIGHT = 0.5f;
        /// <summary>
        /// 每个任务最大英雄数
        /// </summary>
        public const int MAX_HEROES_PER_TASK = 3;
        /// <summary>
        /// 低匹配评分阈值
        /// </summary>
        public const int LOW_MATCH_SCORE_THRESHOLD = 70;
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
    }

    public static class AICity
    {
        /// <summary>
        /// 金币警戒值
        /// </summary>
        public const int GOLD_ALERT = 500;
        /// <summary>
        /// 粮草警戒值
        /// </summary>
        public const int FOOD_ALERT = 500;
        /// <summary>
        /// 城墙警戒值
        /// </summary>
        public const int WALL_ALERT = 150;
        /// <summary>
        /// 士兵警戒值
        /// </summary>
        public const int SOLDIER_ALERT = 500;
        /// <summary>
        /// 民心警戒值
        /// </summary>
        public const int HAPPY_ALERT = 50;
        /// <summary>
        /// 需求权重
        /// </summary>
        public const int NEED_WEIGHT = 30;
    }
}
