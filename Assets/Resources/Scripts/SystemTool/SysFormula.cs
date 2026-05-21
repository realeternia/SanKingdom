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

        private static float CalculateSodBonus(SaveHeroData heroData, ArmsType armsType)
        {
            var heroConfig = HeroConfig.GetConfig(heroData.heroId);
            int sodValue = armsType switch
            {
                ArmsType.SodWalk => heroConfig.SodWalk, 
                ArmsType.SodHorse => heroConfig.SodHorse,
                ArmsType.SodBow => heroConfig.SodBow,
                ArmsType.SodWater => heroConfig.SodWater,
                ArmsType.SodTank => heroConfig.SodTank,
                _ => 1
            };
            float bonus = sodValue * SystemConst.Battle.SOD_BONUS_RATE_PER_POINT;
            return Math.Clamp(bonus, SystemConst.Battle.SOD_BONUS_MIN, SystemConst.Battle.SOD_BONUS_MAX);
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

        public static (int atk, int def) CalculateCombatAttrForTroop(SaveTroopsData troop)
        {
            if (troop == null || troop.heroId1 <= 0 || troop.armsId <= 0)
                return (0, 0);
            
            var hero1 = GameManager.Instance.GetHero(troop.heroId1);
            var hero2 = troop.heroId2 > 0 ? GameManager.Instance.GetHero(troop.heroId2) : null;
            var hero3 = troop.heroId3 > 0 ? GameManager.Instance.GetHero(troop.heroId3) : null;
            
            var armsConfig = ArmsConfig.GetConfig(troop.armsId);
            int sod1 = GetSodValueFromHero(hero1, armsConfig.Type);
            int sod2 = hero2 != null ? GetSodValueFromHero(hero2, armsConfig.Type) : 0;
            int sod3 = hero3 != null ? GetSodValueFromHero(hero3, armsConfig.Type) : 0;
            int maxSod = Math.Max(Math.Max(sod1, sod2), sod3);
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
        public static int CalculateRecruitWildRate()
        {
            return SystemConst.Hero.RECRUIT_WILD_BASE_RATE;
        }

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

            if (combatScore >= SystemConst.AIHero.COMBAT_THRESHOLD && combatScore > domesticScore * SystemConst.Hero.HERO_CLASSIFY_ADVANTAGE_RATIO)
                return HeroType.Combat;
            else if (domesticScore >= SystemConst.AIHero.DOMESTIC_THRESHOLD && domesticScore > combatScore * SystemConst.Hero.HERO_CLASSIFY_ADVANTAGE_RATIO)
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

        public static int CalculateGoldProduction(int cityLevel)
        {
            return CityLevelConfig.GetConfig(cityLevel).GoldAdd;
        }

        public static int CalculateFoodProduction(int cityLevel)
        {
            return (int)(CityLevelConfig.GetConfig(cityLevel).FoodAdd);
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

        public static bool CityHasResAddon(int cityId, string attrName)
        {
            var worldCfg = WorldConfig.GetConfig(cityId);
            if (worldCfg.ResAddon == null) return false;
            var attrCfg = CityAttrConfig.GetConfigByname(attrName.ToLower());
            foreach (int addonId in worldCfg.ResAddon)
            {
                if (addonId == attrCfg.Id) return true;
            }
            return false;
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
        public static int CalculatePriority(int current, int alert)
        {
            if (current <= 0) return 100;
            int deficit = alert - current;
            return (deficit * 100) / alert;
        }

        public static int AdjustPriorityByNeeds(int basePriority, int needPriority)
        {
            return basePriority + SystemConst.AICity.NEED_WEIGHT * needPriority / 100;
        }

        public static float CalculateMatchScore(int[] attrValues)
        {
            if (attrValues == null || attrValues.Length == 0) return 50f;

            float totalScore = 0f;
            foreach (var val in attrValues)
                totalScore += val;

            return totalScore / attrValues.Length;
        }

        public static float CalculateCombinedScore(float matchScore, int adjustedPriority)
        {
            return matchScore + adjustedPriority * SystemConst.AIStrategy.TASK_PRIORITY_WEIGHT;
        }

        public static float CalculateAdvantageRatio(int mySoldier, int targetSoldier)
        {
            return mySoldier > 0 ? (float)mySoldier / Math.Max(1, targetSoldier) : 0;
        }

        public static int CalculateEffectiveSoldier(int citySoldier, int heroCount)
        {
            int maxSoldierByHeroes = (heroCount - 1) * SystemConst.AIStrategy.MAX_SOLDIER_PER_HERO;
            return Math.Min(citySoldier, maxSoldierByHeroes);
        }

        public static bool CheckAttackSourceAdvantage(int mySoldier, int targetSoldier)
        {
            return mySoldier >= targetSoldier * SystemConst.AIStrategy.AI_ATTACK_SOURCE_ADVANTAGE_RATIO;
        }

        public static bool CheckOwnCityAttackAdvantage(int mySoldier, int targetSoldier)
        {
            return mySoldier >= targetSoldier * SystemConst.AIStrategy.AI_OWN_CITY_ATTACK_ADVANTAGE_RATIO;
        }

        public static bool CheckAttackFoodSufficient(int soldier, int food)
        {
            return food >= soldier / SystemConst.AIStrategy.AI_ATTACK_FOOD_DIVISOR;
        }

        public static bool HasThreat(int enemySoldier)
        {
            return enemySoldier >= SystemConst.AIStrategy.AI_THREAT_ENEMY_SOLDIER_THRESHOLD;
        }

        public static bool CanExpand(int totalGold, int totalFood, int totalSoldier)
        {
            return totalGold >= SystemConst.AIStrategy.MIN_RESOURCE_FOR_ATTACK
                && totalFood >= SystemConst.AIStrategy.MIN_RESOURCE_FOR_ATTACK
                && totalSoldier >= SystemConst.AIStrategy.MIN_SOLDIER_FOR_ATTACK;
        }

        public static int CalculateFoodNeeded(int totalSoldier)
        {
            return totalSoldier / SystemConst.AIStrategy.AI_FOOD_NEED_DIVISOR;
        }

        public static int CalculateTroopLimit(int cityLevel, int idleHeroCount, int citySoldier)
        {
            int limitByLevel;
            if (cityLevel <= SystemConst.AIStrategy.CITY_LEVEL_LOW)
                limitByLevel = SystemConst.AIStrategy.TROOP_LIMIT_LOW;
            else if (cityLevel <= SystemConst.AIStrategy.CITY_LEVEL_HIGH)
                limitByLevel = SystemConst.AIStrategy.TROOP_LIMIT_MID;
            else
                limitByLevel = SystemConst.AIStrategy.TROOP_LIMIT_HIGH;

            int limitByHeroes = idleHeroCount / SystemConst.AIStrategy.TROOP_IDLE_HERO_THRESHOLD;
            int limitBySoldier = citySoldier / SystemConst.AIStrategy.TROOP_MIN_SOLDIER;

            return Math.Max(1, Math.Min(limitByLevel, Math.Min(limitByHeroes, limitBySoldier)));
        }

        public static int CalculateHeroesPerTroop(int idleHeroCount, int troopCount)
        {
            if (troopCount <= 0) return SystemConst.AIStrategy.TROOP_MIN_HEROES;
            int heroesPerTroop = idleHeroCount / troopCount;
            return Math.Max(SystemConst.AIStrategy.TROOP_MIN_HEROES, Math.Min(SystemConst.AIStrategy.TROOP_MAX_HEROES, heroesPerTroop));
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
        public static int CalculatePeaceDecay(bool isAdjacent)
        {
            if (isAdjacent)
                return SysRandom.Range(SystemConst.Diplomacy.PEACE_DECAY_ADJACENT_MIN, SystemConst.Diplomacy.PEACE_DECAY_ADJACENT_MAX + 1);
            return SysRandom.Range(SystemConst.Diplomacy.PEACE_DECAY_MIN, SystemConst.Diplomacy.PEACE_DECAY_MAX + 1);
        }

        public static int CalculateBattleRise()
        {
            return SysRandom.Range(SystemConst.Diplomacy.BATTLE_RISE_MIN, SystemConst.Diplomacy.BATTLE_RISE_MAX + 1);
        }
    }
}
