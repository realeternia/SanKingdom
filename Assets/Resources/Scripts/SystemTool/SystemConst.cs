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
        /// 登用敌方英雄忠诚度阈值（1日程）
        /// </summary>
        public const int RECRUIT_ENEMY_LOYALTY_THRESHOLD = 90;
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
    }

    public static class City
    {
        /// <summary>
        /// 城市初始民心值
        /// </summary>
        public const int INITIAL_CITY_HAPPY = 70;
        public const int RES_ADDON_BONUS = 1;
        public const int BATTLE_TIME_MAX = 5;
        public const int BATTLE_TIME_INCREMENT = 2;
        public const float WAR_PRODUCTION_MULTIPLIER = 0.7f;
        /// <summary>
        /// 城防低于此值不生成城门/城墙
        /// </summary>
        public const float GATE_MIN_WALL = 100f;
        /// <summary>
        /// 生成箭塔所需的最小城防值
        /// </summary>
        public const float TOWER_MIN_WALL = 300f;
        /// <summary>
        /// 战斗导致dev打折的起始回合数
        /// </summary>
        public const int DEFENCE_DISCOUNT_START_ROUND = 10;
        /// <summary>
        /// 民心衰减起始回合数
        /// </summary>
        public const int HAPPY_DECAY_START_ROUND = 10;
        /// <summary>
        /// 战斗每回合民心衰减值
        /// </summary>
        public const float HAPPY_DECAY_PER_ROUND = 1f;
    }

    public static class Battle
    {
        /// <summary>
        /// 网格单元格大小
        /// </summary>
        public const int GRID_CELL_SIZE = 15;
        /// <summary>
        /// 地图边界格子X最小值
        /// </summary>
        public const int GRID_MIN_GX = 14;
        /// <summary>
        /// 地图边界格子X最大值
        /// </summary>
        public const int GRID_MAX_GX = 36;
        /// <summary>
        /// 地图边界格子Z最小值
        /// </summary>
        public const int GRID_MIN_GZ = 8;
        /// <summary>
        /// 地图边界格子Z最大值
        /// </summary>
        public const int GRID_MAX_GZ = 22;
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
        /// 城墙/城门一排相对防守方布阵起始X的前移格数（朝攻方方向）
        /// </summary>
        public const int WALL_FRONT_OFFSET = 3;

        /// <summary>
        /// 城门BattleUnitId
        /// </summary>
        public const int GATE_UNIT_ID = 502001;
        /// <summary>
        /// 墙BattleUnitId
        /// </summary>
        public const int WALL_UNIT_ID = 502002;
        /// <summary>
        /// 箭塔BattleUnitId
        /// </summary>
        public const int TOWER_UNIT_ID = 502003;
        /// <summary>
        /// 默认暴击伤害倍率
        /// </summary>
        public const float DEFAULT_CRIT_DAMAGE_MULTI = 0.5f;
        /// <summary>
        /// 回血间隔帧数
        /// </summary>
        public const int REGE_INTERVAL_TICKS = 10;
        /// <summary>
        /// 攻击点阈值
        /// </summary>
        public const int ATTACK_POINT_THRESHOLD = 20;
        /// <summary>
        /// 远程攻击阈值
        /// </summary>
        public const int RANGE_ATTACK_THRESHOLD = 20;
        /// <summary>
        /// 防御方主动出击的战力倍率阈值（防御方战力 > 攻击方战力 * 此值时出击）
        /// </summary>
        public const float DEFENDER_SALLY_POWER_RATIO = 1.5f;
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
        /// 目标评分城门
        /// </summary>
        public const int TARGET_SCORE_GATE = 1;
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
        /// <summary>
        ///科技研究每次消耗的研究值
        /// </summary>
        public const int TECH_RESEARCH_SCIPOINT_COST = 10;
        /// <summary>
        /// 科技研究占用的时间周期数（英雄占用天数）
        /// </summary>
        public const int TECH_RESEARCH_TIME_PERIODS = 3;
        /// <summary>
        /// 扰乱行动每个执行人最多影响的敌方武将数
        /// </summary>
        public const int DISTURB_LOYALTY_TARGET_MAX = 5;
        /// <summary>
        /// 城市日程最大日数
        /// </summary>
        public const int CITY_DAY_MAX = 3;
    }

    public static class Economy
    {
        /// <summary>
        /// 交易基础倍率
        /// </summary>
        public const int TRADE_BASE_MULTIPLIER = 2;
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
    }

    public static class Fair
    {
        /// <summary>
        /// 灾害触发时治安减少值
        /// </summary>
        public const int HAPPY_REDUCE = 10;
        /// <summary>
        /// 灾害触发时粮食乘数（0.9 = 减少10%）
        /// </summary>
        public const float FOOD_REDUCE_RATE = 0.9f;
        /// <summary>
        /// 最近N回合内出现过的fair不再触发（1个月=3旬）
        /// </summary>
        public const int RECENT_ROUNDS = 3;
    }
}
