using System;
using CommonConfig;

public static class SysFormula
{
    private static readonly System.Random _random = new System.Random();
    public static class Battle
    {
        public static int CalculateDamage(int atk, int hp, int def)
        {
            int attackPower = atk + hp / SystemConst.Battle.HP_TO_ATK_DIVISOR;
            int powerDiff = attackPower - def;
            return SystemConst.Battle.BASE_DAMAGE + powerDiff / SystemConst.Battle.DAMAGE_POWER_DIFF_DIVISOR;
        }

        private static int GetSodValueFromHero(SaveHeroData heroData, ArmsType armsType)
        {
            if (heroData == null) return 0;
            var heroConfig = HeroConfig.GetConfig(heroData.heroId);
            return armsType switch
            {
                ArmsType.SodWalk => heroConfig.SodWalk,
                ArmsType.SodHorse => heroConfig.SodHorse,
                ArmsType.SodBow => heroConfig.SodBow,
                ArmsType.SodWater => heroConfig.SodWater,
                ArmsType.SodTank => heroConfig.SodTank,
                _ => 0
            };
        }

        public static int GetTroopMaxSod(SaveTroopsData troop, ArmsType armsType)
        {
            if (troop == null || troop.heroId1 <= 0) return 0;

            var hero1 = GameManager.Instance.GetHero(troop.heroId1);
            var hero2 = troop.heroId2 > 0 ? GameManager.Instance.GetHero(troop.heroId2) : null;
            var hero3 = troop.heroId3 > 0 ? GameManager.Instance.GetHero(troop.heroId3) : null;

            int sod1 = GetSodValueFromHero(hero1, armsType);
            int sod2 = hero2 != null ? GetSodValueFromHero(hero2, armsType) : 0;
            int sod3 = hero3 != null ? GetSodValueFromHero(hero3, armsType) : 0;

            return Math.Max(Math.Max(sod1, sod2), sod3);
        }

        public static (int atk, int def) CalculateCombatAttrForTroop(SaveTroopsData troop)
        {
            if (troop == null || troop.heroId1 <= 0 || troop.armsId <= 0)
                return (0, 0);

            var hero1 = GameManager.Instance.GetHero(troop.heroId1);
            var armsConfig = ArmsConfig.GetConfig(troop.armsId);
            int maxSod = GetTroopMaxSod(troop, armsConfig.Type);
            float sodBonus = Math.Clamp(maxSod * SystemConst.Battle.SOD_BONUS_RATE_PER_POINT, SystemConst.Battle.SOD_BONUS_MIN, SystemConst.Battle.SOD_BONUS_MAX);

            int atk = (int)(hero1.str * SystemConst.Battle.HERO_ATTR_TO_COMBAT_RATE + armsConfig.Atk * (1f + sodBonus));
            int def = (int)(hero1.leadShip * SystemConst.Battle.HERO_ATTR_TO_COMBAT_RATE + armsConfig.Def * (1f + sodBonus));
            return (atk, def);
        }

        public static (int minDamage, int maxDamage) GetDamageRange(int levelDiff, bool isCrit, float critDamageMulti)
        {
            int minDamage = SystemConst.Battle.MIN_ATTACK_DAMAGE;
            int maxDamage = SystemConst.Battle.MAX_ATTACK_DAMAGE;

            if (levelDiff != 0)
            {
                minDamage = Math.Clamp(minDamage + levelDiff, SystemConst.Battle.LEVEL_DIFF_MIN_DAMAGE_MIN, SystemConst.Battle.LEVEL_DIFF_MIN_DAMAGE_MAX);
                maxDamage = Math.Clamp(maxDamage + levelDiff * SystemConst.Battle.LEVEL_DIFF_MAX_DAMAGE_FACTOR, SystemConst.Battle.LEVEL_DIFF_MAX_DAMAGE_MIN, SystemConst.Battle.LEVEL_DIFF_MAX_DAMAGE_MAX);
            }

            if (isCrit)
            {
                minDamage = (int)(minDamage * (1 + critDamageMulti));
                maxDamage = (int)(maxDamage * (1 + critDamageMulti));
            }

            return (minDamage, maxDamage);
        }

        public static float CalculateTargetScore(bool targetIsHero, float distance, float attackRange,
            int damageEstimate, int myLevel, int targetLevel, float targetHpRate)
        {
            float score = targetIsHero ? SystemConst.Battle.TARGET_SCORE_HERO : SystemConst.Battle.TARGET_SCORE_NONHERO;

            if (distance < attackRange * 2)
            {
                score += damageEstimate / 2f;
                score += (myLevel - targetLevel) * SystemConst.Battle.LEVEL_DIFF_SCORE_WEIGHT;

                if (targetHpRate < SystemConst.Battle.LOW_HP_THRESHOLD)
                    score += (SystemConst.Battle.LOW_HP_THRESHOLD - targetHpRate) * SystemConst.Battle.LOW_HP_SCORE_WEIGHT + SystemConst.Battle.LOW_HP_BONUS;
            }
            else
            {
                score += 100f / (distance + 1f);
            }

            return score;
        }

        public static float AdjustBurstRateByAttr(float baseRate, int myAttr, int defAttr, bool isEnemy)
        {
            if (!isEnemy) return baseRate;

            if (myAttr > defAttr)
                return baseRate * Math.Min(SystemConst.Battle.BURST_RATE_ATTR_CAP, 1 + (myAttr - defAttr) * SystemConst.Battle.BURST_RATE_ATTR_FACTOR);
            else if (myAttr < defAttr)
                return baseRate / Math.Min(SystemConst.Battle.BURST_RATE_ATTR_CAP, 1 + (defAttr - myAttr) * SystemConst.Battle.BURST_RATE_ATTR_FACTOR);

            return baseRate;
        }

        public static int CalculateFoodCost(int totalHeroHp)
        {
            return totalHeroHp / SystemConst.Battle.FOOD_COST_DIVISOR;
        }
    }

    public static class Hero
    {

        public static int CalculateRecruitCapturedRate(int loyalty)
        {
            int diff = 100 - loyalty;
            return diff * diff / SystemConst.Hero.RECRUIT_CAPTURED_FORMULA_A + diff / SystemConst.Hero.RECRUIT_CAPTURED_FORMULA_B;
        }

        public static int ApplyCharmBonus(int baseRate, int charm, bool isKing)
        {
            if (charm >= SystemConst.Hero.CHARM_BONUS_TIER1)
                baseRate = baseRate * SystemConst.Hero.RECRUIT_TIER1_MULTIPLIER / 100;
            else if (charm >= SystemConst.Hero.CHARM_BONUS_TIER2)
                baseRate = baseRate * SystemConst.Hero.RECRUIT_TIER2_MULTIPLIER / 100;

            if (isKing)
                baseRate = baseRate * SystemConst.Hero.KING_RECRUIT_MULTIPLIER / 100;

            return baseRate;
        }

        /// <summary>
        /// 按日程获取登庸敌方武将的忠诚度阈值：1日=90，2日=85，3日=80
        /// </summary>
        public static int GetRecruitLoyaltyThreshold(int dayFilter)
        {
            switch (dayFilter)
            {
                case 2: return SystemConst.Hero.RECRUIT_ENEMY_LOYALTY_THRESHOLD_2DAY;
                case 3: return SystemConst.Hero.RECRUIT_ENEMY_LOYALTY_THRESHOLD_3DAY;
                default: return SystemConst.Hero.RECRUIT_ENEMY_LOYALTY_THRESHOLD;
            }
        }

        /// <summary>
        /// 在野武将位于非己方势力城市时的成功率惩罚：rate * 0.5
        /// </summary>
        public static int ApplyWildNonFriendlyPenalty(int rate)
        {
            return (int)Math.Round(rate * SystemConst.Hero.RECRUIT_WILD_NON_FRIENDLY_PENALTY);
        }

        public static int CalculateCaptureChance(int str)
        {
            int effectiveStr = str > 0 ? str : 50;
            return SystemConst.Expedition.CATCH_BASE_CHANCE + (100 - effectiveStr) * SystemConst.Expedition.CATCH_STR_FACTOR / 100;
        }

        public static int CalculateAttrGrowth(int baseAttr, int level)
        {
            if (level <= 1) return 0;
            return Math.Max(SystemConst.Hero.MIN_ATTR_PER_LEVEL * (level - 1), baseAttr * (level - 1) / SystemConst.Hero.ATTR_GROWTH_DIVISOR);
        }

        public static int CalculatePraiseLoyaltyAdd()
        {
            return _random.Next(SystemConst.Hero.PRAISE_LOYALTY_ADD_MIN, SystemConst.Hero.PRAISE_LOYALTY_ADD_MAX);
        }

        public static int CalculateRewardLoyaltyAdd()
        {
            return _random.Next(SystemConst.Hero.REWARD_LOYALTY_ADD_MIN, SystemConst.Hero.REWARD_LOYALTY_ADD_MAX);
        }

        public static int CalculateCapturedLoyaltyDecay()
        {
            return _random.Next(SystemConst.Hero.CAPTURED_LOYALTY_DECAY_MIN, SystemConst.Hero.CAPTURED_LOYALTY_DECAY_MAX);
        }

        public static bool CheckEscape()
        {
            return _random.Next(0, 100) < SystemConst.Hero.CAPTURED_ESCAPE_CHANCE;
        }

        public static bool CheckWildHeroMove()
        {
            return _random.Next(0, 100) < SystemConst.Hero.WILD_HERO_MOVE_CHANCE;
        }

        public static HeroType ClassifyHero(int str, int leadship, int inte, int fair, int charm)
        {
            int combatScore = str + leadship + inte;
            int domesticScore = inte + fair + charm;

            if (combatScore >= AIConst.AIHero.COMBAT_THRESHOLD && combatScore > domesticScore * SystemConst.Hero.HERO_CLASSIFY_ADVANTAGE_RATIO)
                return HeroType.Combat;
            else if (domesticScore >= AIConst.AIHero.DOMESTIC_THRESHOLD && domesticScore > combatScore * SystemConst.Hero.HERO_CLASSIFY_ADVANTAGE_RATIO)
                return HeroType.Domestic;

            return HeroType.Balanced;
        }
    }

    public static class City
    {
        public static int GetHeroTier(float avgWeightedValue)
        {
            if (avgWeightedValue >= 90)
                return 0;
            else if (avgWeightedValue >= 80)
                return 1;
            else if (avgWeightedValue >= 70)
                return 2;
            else
                return 3;
        }

        public static float GetHeroWeightedAttrValue(SaveHeroData heroData, string[] attrs)
        {
            if (attrs == null || attrs.Length == 0)
                return 0;
            
            if (attrs.Length == 1)
            {
                return heroData.GetAttr(attrs[0]);
            }
            else
            {
                float firstAttr = heroData.GetAttr(attrs[0]);
                float secondAttr = heroData.GetAttr(attrs[1]);
                return firstAttr * (2f / 3f) + secondAttr * (1f / 3f);
            }
        }

        public static float CalculateOwnerScore(int str, int inte, int fair, int leadship, int charm, bool isKing)
        {
            float totalScore = str * SystemConst.City.OWNER_SCORE_WEIGHT_STR
                + inte + fair
                + leadship * SystemConst.City.OWNER_SCORE_WEIGHT_LEADSHIP
                + charm * SystemConst.City.OWNER_SCORE_WEIGHT_CHARM;

            if (isKing)
                totalScore += SystemConst.City.KING_OWNER_BONUS_SCORE;

            return totalScore;
        }

        public static float CalculateDevValue(int min, int max, int addon, int currentVal, int valMax)
        {
            var val = Math.Max(min, (float)addon / 100 * max);
            return Math.Min(val, valMax - currentVal);
        }

        public static int CalculateSecondaryAttrContribution(int primaryAttr, int secondaryAttr)
        {
            if (secondaryAttr > primaryAttr)
                return (secondaryAttr - primaryAttr) / SystemConst.Hero.SECONDARY_ATTR_CONTRIBUTION_DIVISOR;
            return 0;
        }

        public static int CalculateDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }

        /// <summary>
        /// 计算两城市间日程数：相邻=1日，曼哈顿距离≤阈值=2日，否则=3日
        /// </summary>
        public static int CalculateCityDayDistance(int cityId1, int cityId2)
        {
            if (cityId1 == cityId2)
                return SystemConst.CityDev.CITY_DAY_MIN;

            var cfg1 = WorldConfig.GetConfig(cityId1);
            var cfg2 = WorldConfig.GetConfig(cityId2);

            if (cfg1.WorldNearIds != null && Array.IndexOf(cfg1.WorldNearIds, cityId2) >= 0)
                return SystemConst.CityDev.CITY_DAY_MIN;

            int manhattan = Math.Abs(cfg1.X - cfg2.X) + Math.Abs(cfg1.Y - cfg2.Y);
            if (manhattan <= SystemConst.CityDev.DAY_DISTANCE_THRESHOLD_2)
                return SystemConst.CityDev.CITY_DAY_MIN + 1;

            return SystemConst.CityDev.CITY_DAY_MAX;
        }

        public static bool CityHasResAddon(int cityId, string attrName)
        {
            return SaveCityData.CityHasResAddon(cityId, attrName);
        }

        public static float GetHappyMultiplier(int happy)
        {
            if (happy >= 95)
                return 1.2f;
            if (happy >= 60)
                return 1f;
            if (happy >= 30)
                return 0.8f;
            return 0.6f;
        }
    }

    public static class Economy
    {
        public static int CalculateExchangeResult(int amount, bool isBuying)
        {
            return (int)(SystemConst.Economy.EXCHANGE_RATE * amount);
        }
    }

    public static class AIStrategy
    {
        public static float CalculateAdvantageRatio(int mySoldier, int targetSoldier)
        {
            return mySoldier > 0 ? (float)mySoldier / Math.Max(1, targetSoldier) : 0;
        }

        public static int CalculateEffectiveSoldier(int citySoldier, int heroCount)
        {
            int maxSoldierByHeroes = (heroCount - 1) * AIConst.AIStrategy.MAX_SOLDIER_PER_HERO;
            return Math.Min(citySoldier, maxSoldierByHeroes);
        }

        public static bool CheckOwnCityAttackAdvantage(int mySoldier, int targetSoldier)
        {
            return mySoldier >= targetSoldier * AIConst.AIStrategy.AI_OWN_CITY_ATTACK_ADVANTAGE_RATIO;
        }

        public static bool CheckAttackFoodSufficient(int soldier, int food)
        {
            return food >= soldier / AIConst.AIStrategy.AI_ATTACK_FOOD_DIVISOR;
        }

        public static bool HasThreat(int enemySoldier)
        {
            return enemySoldier >= AIConst.AIStrategy.AI_THREAT_ENEMY_SOLDIER_THRESHOLD;
        }

        public static int CalculateFoodNeeded(int totalSoldier)
        {
            return totalSoldier / AIConst.AIStrategy.AI_FOOD_NEED_DIVISOR;
        }

        public static int CalculateTroopLimit(int commanderCount, int heroCount, int citySoldier)
        {
            int limitByCommander = commanderCount;
            int soldierPerCorps = CalculateSoldierPerCorps(heroCount);
            int limitBySoldier = citySoldier / soldierPerCorps;
            int hardLimit = AIConst.AIStrategy.TROOP_CITY_HARD_LIMIT;

            return Math.Max(0, Math.Min(hardLimit, Math.Min(limitByCommander, limitBySoldier)));
        }

        /// <summary>
        /// 梯度计算每个军团所需士兵数：武将越多，每个军团所需士兵越少
        /// ≤6武将=50，7~11武将线性递减至30，11+武将保持30
        /// </summary>
        public static int CalculateSoldierPerCorps(int heroCount)
        {
            int baseValue = AIConst.AIStrategy.TROOP_SOLDIER_PER_CORPS;
            int minValue = AIConst.AIStrategy.TROOP_SOLDIER_PER_CORPS_RELAXED;
            int startThreshold = AIConst.AIStrategy.TROOP_HERO_RICH_THRESHOLD;
            int endThreshold = AIConst.AIStrategy.TROOP_HERO_FULL_RICH_THRESHOLD;

            if (heroCount <= startThreshold) return baseValue;
            if (heroCount >= endThreshold) return minValue;

            int range = baseValue - minValue;
            int steps = endThreshold - startThreshold;
            int progress = heroCount - startThreshold;

            return baseValue - progress * range / steps;
        }
    }

    public static class Game
    {
        public static float CalculateCurrentYear(int totalSeasons)
        {
            int years = totalSeasons / SystemConst.Game.SEASONS_PER_YEAR;
            int seasons = totalSeasons % SystemConst.Game.SEASONS_PER_YEAR;
            return SystemConst.Game.BASE_YEAR + years + (seasons / (float)SystemConst.Game.SEASONS_PER_YEAR);
        }

        public static int CalculateSeasonId(int round)
        {
            return (round % SystemConst.Game.SEASONS_PER_YEAR) + 1;
        }
    }

    public static class Diplomacy
    {
        public static int CalculateBattleRise()
        {
            return SysRandom.Range(SystemConst.Diplomacy.BATTLE_RISE_MIN, SystemConst.Diplomacy.BATTLE_RISE_MAX + 1);
        }
    }
}
