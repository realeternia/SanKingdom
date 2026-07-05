using UnityEngine;
using System;

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
        /// 登用敌方英雄忠诚度阈值（1日程）
        /// </summary>
        public const int RECRUIT_ENEMY_LOYALTY_THRESHOLD = 90;
        /// <summary>
        /// 登用敌方英雄忠诚度阈值（2日程）
        /// </summary>
        public const int RECRUIT_ENEMY_LOYALTY_THRESHOLD_2DAY = 85;
        /// <summary>
        /// 登用敌方英雄忠诚度阈值（3日程）
        /// </summary>
        public const int RECRUIT_ENEMY_LOYALTY_THRESHOLD_3DAY = 80;
        /// <summary>
        /// 默认兵种ID
        /// </summary>
        public const int DEFAULT_ARMS_ID = 1;
        /// <summary>
        /// 野外势力ID
        /// </summary>
        public const int WILD_FORCE_ID = 0;
        /// <summary>
        /// 每个英雄最大士兵数
        /// </summary>
        public const int MAX_SOLDIER_PER_HERO = 100;
        /// <summary>
        /// 在野武将基础登庸成功率
        /// </summary>
        public const int RECRUIT_WILD_BASE_RATE = 30;
        /// <summary>
        /// 在野武将显示衷心度
        /// </summary>
        public const int WILD_HERO_LOYALTY = 50;
        /// <summary>
        /// 在野武将位于非己方势力城市时的成功率惩罚系数（0.5=下降50%）
        /// </summary>
        public const float RECRUIT_WILD_NON_FRIENDLY_PENALTY = 0.5f;
        /// <summary>
        /// 登用俘虏/敌方公式斜率分子（rate = diff * SLOPE / DIVISOR - OFFSET）
        /// </summary>
        public const int RECRUIT_CAPTURED_RATE_SLOPE = 3;
        /// <summary>
        /// 登用俘虏/敌方公式斜率分母
        /// </summary>
        public const int RECRUIT_CAPTURED_RATE_DIVISOR = 4;
        /// <summary>
        /// 登用俘虏/敌方公式偏移量（loyalty=80 即 diff=20 时为 10%）
        /// </summary>
        public const int RECRUIT_CAPTURED_RATE_OFFSET = 5;
        /// <summary>
        /// 魅力加成阈值，超过此值每点魅力 +1% 成功率
        /// </summary>
        public const int CHARM_BONUS_THRESHOLD = 75;
        /// <summary>
        /// 魅力加成每点增加的成功率
        /// </summary>
        public const int CHARM_BONUS_PER_POINT = 1;
        /// <summary>
        /// 君主登用倍率
        /// </summary>
        public const int KING_RECRUIT_MULTIPLIER = 110;
        /// <summary>
        /// 登庸成功率上限
        /// </summary>
        public const int RECRUIT_RATE_MAX = 100;
        /// <summary>
        /// 目标喜欢执行人时的加成
        /// </summary>
        public const int RECRUIT_LIKE_EXECUTOR_BONUS = 10;
        /// <summary>
        /// 目标喜欢君主时的加成（执行人非君主才计算）
        /// </summary>
        public const int RECRUIT_LIKE_KING_BONUS = 10;
        /// <summary>
        /// 目标厌恶执行人时的惩罚
        /// </summary>
        public const int RECRUIT_HATE_EXECUTOR_PENALTY = -30;
        /// <summary>
        /// 目标厌恶君主时的惩罚
        /// </summary>
        public const int RECRUIT_HATE_KING_PENALTY = -50;
        /// <summary>
        /// 执行人与目标派系相同时的加成（加法，直接加到基础率）
        /// </summary>
        public const int RECRUIT_SAME_FACTION_BONUS = 5;
        /// <summary>
        /// 执行人与目标每有一个相同爱好的加成（加法，直接加到基础率）
        /// </summary>
        public const int RECRUIT_SHARED_HOBBY_BONUS_PER = 1;
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
        public const int RES_ADDON_BONUS = 1;
        public const int BATTLE_TIME_MAX = 5;
        public const int BATTLE_TIME_INCREMENT = 2;
        public const float WAR_PRODUCTION_MULTIPLIER = 0.7f;
    }

    public static class Economy
    {
        /// <summary>
        /// 交易行动兑换比率（1 金币兑换此值的士兵/粮草）
        /// </summary>
        public const float TRADE_EXCHANGE_RATIO = 2f;
        /// <summary>
        /// 交易智力加成阈值（智力＞此值才触发加成）
        /// </summary>
        public const int TRADE_INT_THRESHOLD = 70;
        /// <summary>
        /// 交易智力加成每点增量比率
        /// </summary>
        public const float TRADE_INT_BONUS_PER_POINT = 0.02f;
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
        public const int GRID_CELL_SIZE = 15;
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
        public const int MAX_BATTLE_HEROES_PER_SIDE = 15;
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
        /// 布阵区域行数
        /// </summary>
        public const int DEPLOY_GRID_ROWS = 3;
        /// <summary>
        /// 布阵区域列数
        /// </summary>
        public const int DEPLOY_GRID_COLS = 5;
        /// <summary>
        /// 攻击方布阵区域起始格子X
        /// </summary>
        public const int DEPLOY_SIDE1_BASE_GX = 20;
        /// <summary>
        /// 攻击方布阵区域起始格子Z
        /// </summary>
        public const int DEPLOY_SIDE1_BASE_GZ = 13;
        /// <summary>
        /// 防守方布阵区域起始格子X
        /// </summary>
        public const int DEPLOY_SIDE2_BASE_GX = 29;
        /// <summary>
        /// 防守方布阵区域起始格子Z
        /// </summary>
        public const int DEPLOY_SIDE2_BASE_GZ = 13;
        /// <summary>
        /// 城门血量
        /// </summary>
        public const int GATE_HP = 100;
        /// <summary>
        /// 城墙血量
        /// </summary>
        public const int WALL_HP = 999999;
        /// <summary>
        /// 城门BattleUnitId
        /// </summary>
        public const int GATE_UNIT_ID = 502001;
        /// <summary>
        /// 墙BattleUnitId
        /// </summary>
        public const int WALL_UNIT_ID = 502002;
        /// <summary>
        /// 默认暴击伤害倍率
        /// </summary>
        public const float DEFAULT_CRIT_DAMAGE_MULTI = 0.5f;
        /// <summary>
        /// 回血间隔帧数
        /// </summary>
        public const int REGE_INTERVAL_TICKS = 10;
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
        /// 最小攻击伤害
        /// </summary>
        public const int MIN_ATTACK_DAMAGE = 3;
        /// <summary>
        /// 最大攻击伤害
        /// </summary>
        public const int MAX_ATTACK_DAMAGE = 30;
        /// <summary>
        /// 等级差最小伤害下限
        /// </summary>
        public const int LEVEL_DIFF_MIN_DAMAGE_MIN = 2;
        /// <summary>
        /// 等级差最小伤害上限
        /// </summary>
        public const int LEVEL_DIFF_MIN_DAMAGE_MAX = 8;
        /// <summary>
        /// 等级差最大伤害因子
        /// </summary>
        public const int LEVEL_DIFF_MAX_DAMAGE_FACTOR = 2;
        /// <summary>
        /// 等级差最大伤害下限
        /// </summary>
        public const int LEVEL_DIFF_MAX_DAMAGE_MIN = 15;
        /// <summary>
        /// 等级差最大伤害上限
        /// </summary>
        public const int LEVEL_DIFF_MAX_DAMAGE_MAX = 35;
        /// <summary>
        /// 血量转攻击除数
        /// </summary>
        public const int HP_TO_ATK_DIVISOR = 5;
        /// <summary>
        /// 基础伤害
        /// </summary>
        public const int BASE_DAMAGE = 8;
        /// <summary>
        /// 伤害战力差除数
        /// </summary>
        public const int DAMAGE_POWER_DIFF_DIVISOR = 5;
        /// <summary>
        /// 战斗中帧数阈值
        /// </summary>
        public const int IN_FIGHT_TICK_THRESHOLD = 3;
        /// <summary>
        /// 被攻击buff最小伤害
        /// </summary>
        public const int ATTACKED_BUFF_MIN_DAMAGE = 3;
        /// <summary>
        /// Buff最小伤害阈值
        /// </summary>
        public const int BUFF_MIN_DAMAGE_THRESHOLD = 3;
        /// <summary>
        /// Buff最小伤害值
        /// </summary>
        public const int BUFF_MIN_DAMAGE_VALUE = 5;
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
        /// 目标评分城门
        /// </summary>
        public const int TARGET_SCORE_GATE = 1;
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
        public const float SOD_BONUS_RATE_PER_POINT = 0.03f;
        public const float SOD_BONUS_MIN = 0.01f;
        public const float SOD_BONUS_MAX = 0.30f;
        public const float HERO_ATTR_TO_COMBAT_RATE = 0.7f;
        /// <summary>
        /// 回合间等待时间（秒）
        /// </summary>
        public const float TURN_END_WAIT_TIME = 0.5f;
    }

    public static class WorldMap
    {
        public const float MAP_SCALE_FACTOR = 1.25f;
        public const float ROAD_WIDTH = 12f;
    }

    public static class ResourceCache
    {
        public const int UI_CACHE_MAX_COUNT = 200;
        public const int UI_CACHE_MAX_MEMORY_MB = 100;
        public const int BATTLE_CACHE_MAX_COUNT = 100;
        public const int BATTLE_CACHE_MAX_MEMORY_MB = 50;

        public static long UI_CACHE_MAX_MEMORY_BYTES => UI_CACHE_MAX_MEMORY_MB * 1024L * 1024L;
        public static long BATTLE_CACHE_MAX_MEMORY_BYTES => BATTLE_CACHE_MAX_MEMORY_MB * 1024L * 1024L;
    }

    public static class CityDev
    {
        public const int MOVE_DEV_ID = 21102;
        public const int BATTLE_DEV_ID = 21103;
        public const int IDLE_DEV_ID = 21999;
        public const int USE_HERO_DEV_ID = 21204;
        public const int PRAISE_DEV_ID = 21205;
        public const int PRAISE_PAID_DEV_ID = 21206;
        public const int TRADE_DEV_ID = 21203;
        public const int SEARCH_DEV_ID = 21202;
        /// <summary>
        /// 城市日程：相邻为1日，曼哈顿距离阈值≤此值算2日
        /// </summary>
        public const int DAY_DISTANCE_THRESHOLD_2 = 600;
        /// <summary>
        /// 城市日程最小日数
        /// </summary>
        public const int CITY_DAY_MIN = 1;
        /// <summary>
        /// 城市日程最大日数
        /// </summary>
        public const int CITY_DAY_MAX = 3;
    }

    public static class Diplomacy
    {
        public const int RELATION_MIN = 1;
        public const int RELATION_MAX = 100;
        public const int RELATION_DEFAULT = 50;
        public const int RELATION_FRIENDLY_THRESHOLD = 65;
        public const int RELATION_HOSTILE_THRESHOLD = 35;
        public const int PEACE_DECAY_ADJACENT = 1;
        public const int PEACE_IMPROVE_NON_ADJACENT = 1;
        public const int BATTLE_RISE_MIN = 3;
        public const int BATTLE_RISE_MAX = 8;
    }
}
