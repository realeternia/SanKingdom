using System;
using System.Collections.Generic;
using CommonConfig;

public enum HeroType
{
    Combat,
    Domestic,
    Balanced
}

public static class SysFormula
{
    private static readonly System.Random _random = new System.Random();
    public static class Battle
    {
        public static int CalculateDamage(int atk, int hp, int def)
        {
            int attackPower = atk + hp / 5;
            int powerDiff = attackPower - def;
            return 8 + powerDiff / 5;
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
            float sodBonus = Math.Clamp(maxSod * 0.03f, 0.01f, 0.30f);

            int atk = (int)(hero1.str * 0.7f + armsConfig.Atk * (1f + sodBonus));
            int def = (int)(hero1.leadShip * 0.7f + armsConfig.Def * (1f + sodBonus));
            return (atk, def);
        }

        /// <summary>
        /// 估算部队列表总战力 = 各部队 士兵数 * (atk + def) / 2 累加。
        /// 仅计算实际参战的前 MAX_BATTLE_HEROES_PER_SIDE 个部队，与 InitSummon 一致。
        /// </summary>
        public static long CalculateForcePower(List<SaveTroopsData> troops, Dictionary<int, int> soldierMap)
        {
            if (troops == null) return 0;
            long power = 0;
            int count = Math.Min(troops.Count, 15);
            for (int i = 0; i < count; i++)
            {
                var troop = troops[i];
                if (troop.heroId1 <= 0) continue;
                int soldiers = (soldierMap != null && soldierMap.ContainsKey(troop.heroId1)) ? soldierMap[troop.heroId1] : 0;
                if (soldiers <= 0) continue;
                var (atk, def) = CalculateCombatAttrForTroop(troop);
                power += (long)soldiers * (atk + def) / 2;
            }
            return power;
        }

        public static (int minDamage, int maxDamage) GetDamageRange(int levelDiff, bool isCrit, float critDamageMulti)
        {
            int minDamage = 3;
            int maxDamage = 30;

            if (levelDiff != 0)
            {
                minDamage = Math.Clamp(minDamage + levelDiff, 2, 8);
                maxDamage = Math.Clamp(maxDamage + levelDiff * 2, 15, 35);
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
            float score = targetIsHero ? 10 : 30;

            if (distance < attackRange * 2)
            {
                score += damageEstimate / 2f;
                score += (myLevel - targetLevel) * 7f;

                if (targetHpRate < 0.5f)
                    score += (0.5f - targetHpRate) * 100f + 10f;
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
                return baseRate * Math.Min(2f, 1 + (myAttr - defAttr) * 0.02f);
            else if (myAttr < defAttr)
                return baseRate / Math.Min(2f, 1 + (defAttr - myAttr) * 0.02f);

            return baseRate;
        }

        public static int CalculateFoodCost(int totalHeroHp)
        {
            return totalHeroHp / 20;
        }
    }

    public static class Hero
    {

        /// <summary>
        /// 解析条件字符串(如"inte>=90")并检查英雄属性是否满足
        /// 支持运算符: >=, <=, ==, !=, >, <
        /// 空条件返回true
        /// </summary>
        public static bool CheckHeroCondition(string condition, SaveHeroData hero)
        {
            if (string.IsNullOrEmpty(condition))
                return true;

            string[] operators = { ">=", "<=", "==", "!=", ">", "<" };
            string foundOp = null;
            string attrName = null;
            string valueStr = null;

            foreach (var op in operators)
            {
                int idx = condition.IndexOf(op);
                if (idx > 0)
                {
                    foundOp = op;
                    attrName = condition.Substring(0, idx).Trim();
                    valueStr = condition.Substring(idx + op.Length).Trim();
                    break;
                }
            }

            if (foundOp == null)
                return true;

            int threshold;
            if (!int.TryParse(valueStr, out threshold))
                return true;

            int heroValue = hero.GetAttr(attrName);

            switch (foundOp)
            {
                case ">=": return heroValue >= threshold;
                case "<=": return heroValue <= threshold;
                case ">": return heroValue > threshold;
                case "<": return heroValue < threshold;
                case "==": return heroValue == threshold;
                case "!=": return heroValue != threshold;
                default: return true;
            }
        }

        /// <summary>
        /// 计算登庸成功率
        /// 在野：基础RECRUIT_WILD_BASE_RATE，非己方城市×RECRUIT_WILD_NON_OWN_CITY_MULTIPLIER
        /// 俘虏/敌方在职：rate = diff*3/4 - RECRUIT_ENEMY_BASE_OFFSET（diff=100-忠诚）
        /// 额外加成（由 USE_HERO_DEV_ID 配置驱动）
        /// </summary>
        public static int CalculateRecruitRate(int cityId, int myHeroId, int targetHeroId)
        {
            var cityData = GameManager.Instance.GetCity(cityId);
            var hero = GameManager.Instance.GetHero(targetHeroId);

            if (hero.state == HeroState.Normal && hero.forceId == cityData.forceId)
                return 0;

            int baseSuccessRate = 0;

            if (hero.state == HeroState.Wild)
            {
                baseSuccessRate = 30;
                var heroCity = GameManager.Instance.GetCity(hero.cityId);
                if (heroCity == null || heroCity.forceId != cityData.forceId)
                {
                    baseSuccessRate = (int)Math.Round(baseSuccessRate * 0.5f);
                }
            }
            else if (hero.state == HeroState.Catched || (hero.state == HeroState.Normal && hero.forceId != cityData.forceId))
            {
                int diff = 100 - hero.loyalty;
                baseSuccessRate = diff * 3 / 4 - 5;
                if (baseSuccessRate < 0) baseSuccessRate = 0;
            }

            if (myHeroId > 0)
            {
                var executorHero = GameManager.Instance.GetHero(myHeroId);
                if (executorHero != null)
                {
                    var targetConfig = HeroConfig.GetConfig(targetHeroId);
                    int targetForceId = hero.forceId;
                    baseSuccessRate += CalcKingActionBonus(myHeroId, targetForceId, SystemConst.CityDev.USE_HERO_DEV_ID, targetConfig);
                }
            }

            if (baseSuccessRate > 100) baseSuccessRate = 100;
            return baseSuccessRate;
        }

        /// <summary>
        /// 基于 CityDevKingActionConfig 计算 KingAction 成功率（全加法公式）
        /// devId：KingAction 配置 ID（对应 CityDevConfig 中的 devId）
        /// recruitTargetConfig：登庸时为被登庸英雄配置（用于派系/爱好匹配），其他行动传 null（用目标势力主公）
        /// </summary>
        public static int CalcKingActionBonus(int executorHeroId, int targetForceId, int devId, HeroConfig recruitTargetConfig)
        {
            var executorHero = GameManager.Instance.GetHero(executorHeroId);
            if (executorHero == null) return 0;

            var devCfg = CityDevConfig.GetConfig(devId);
            var kingCfg = CityDevKingActionConfig.GetConfig(devId);

            int rate = (int)(kingCfg.BaseRate * 100);

            var executorConfig = HeroConfig.GetConfig(executorHeroId);
            int executorForceId = executorHero.forceId;
            int kingHeroId = ForceConfig.GetConfig(executorForceId).HeroId;
            bool isKing = executorHero.heroId == kingHeroId;

            // 加法加成：派系/爱好匹配
            if (kingCfg.NeedAdditiveBonus)
            {
                HeroConfig matchConfig = recruitTargetConfig;
                if (matchConfig == null)
                {
                    var targetForceCfg = ForceConfig.GetConfig(targetForceId);
                    if (targetForceCfg != null)
                        matchConfig = HeroConfig.GetConfig(targetForceCfg.HeroId);
                }
                if (matchConfig != null)
                    rate += GetAdditiveBonus(matchConfig, executorConfig);
            }

            // 属性溢出收益：取 Attrs[0] 作为主属性
            string attrName = devCfg.Attrs != null && devCfg.Attrs.Length > 0 ? devCfg.Attrs[0] : "charm";
            int attr = executorHero.GetAttr(attrName);
            if (attr > kingCfg.AttrHighBound)
                rate += (int)((attr - kingCfg.AttrHighBound) * kingCfg.BonusPerPoint * 100);

            // 君主收益
            if (isKing)
                rate += (int)(kingCfg.KingBonus * 100);

            if (rate < 0) rate = 0;
            if (rate > 100) rate = 100;
            return rate;
        }

        /// <summary>
        /// 加法加成：派系相同、爱好相同，直接返回加到基础率的数值
        /// </summary>
        private static int GetAdditiveBonus(HeroConfig targetConfig, HeroConfig executorConfig)
        {
            int bonus = 0;

            // 派系相同（非空、非"无"）+5
            if (!string.IsNullOrEmpty(targetConfig.Paixi) && targetConfig.Paixi != "无"
                && targetConfig.Paixi == executorConfig.Paixi)
            {
                bonus += 5;
            }
            // 每个相同爱好 +1
            if (targetConfig.Aihao != null && executorConfig.Aihao != null)
            {
                foreach (string hobby in targetConfig.Aihao)
                {
                    if (Array.IndexOf(executorConfig.Aihao, hobby) >= 0)
                    {
                        bonus += 1;
                    }
                }
            }

            return bonus;
        }

        /// <summary>
        /// 乘算关系加成百分比：LikeForces/HateForces（按程度分级计算）
        /// </summary>
        private static int GetRelationBonusPercent(HeroConfig targetConfig, int executorForceId)
        {
            int bonus = 0;

            int likeDegree = GetForceDegree(targetConfig.LikeForces, executorForceId);
            if (likeDegree > 0)
                bonus += likeDegree * 5;

            int hateDegree = GetForceDegree(targetConfig.HateForces, executorForceId);
            if (hateDegree > 0)
                bonus += hateDegree * -8;

            return bonus;
        }

        private static int GetForceDegree(string[] forceEntries, int forceId)
        {
            if (forceEntries == null) return 0;
            string prefix = forceId + ";";
            for (int i = 0; i < forceEntries.Length; i++)
            {
                if (forceEntries[i] != null && forceEntries[i].StartsWith(prefix))
                {
                    string degreeStr = forceEntries[i].Substring(prefix.Length);
                    if (int.TryParse(degreeStr, out int degree))
                        return degree;
                }
            }
            return 0;
        }

        /// <summary>
        /// 按日程获取登庸敌方武将的忠诚度阈值：1日=90，2日=85，3日=80
        /// </summary>
        public static int GetRecruitLoyaltyThreshold(int dayFilter)
        {
            switch (dayFilter)
            {
                case 2: return 85;
                case 3: return 80;
                default: return 90;
            }
        }

        public static int CalculateCaptureChance(int str)
        {
            int effectiveStr = str > 0 ? str : 50;
            return 7 + (100 - effectiveStr) * 8 / 100;
        }

        public static int CalculateAttrGrowth(int baseAttr, int level)
        {
            if (level <= 1) return 0;
            return Math.Max(8 * (level - 1), baseAttr * (level - 1) / 10);
        }

        public static int CalculateCapturedLoyaltyDecay()
        {
            return _random.Next(1, 4);
        }

        public static bool CheckEscape()
        {
            return _random.Next(0, 100) < 20;
        }

        public static bool CheckWildHeroMove()
        {
            return _random.Next(0, 100) < 20;
        }

        public static HeroType ClassifyHero(int str, int leadship, int inte, int fair, int charm)
        {
            int combatScore = str + leadship + inte;
            int domesticScore = inte + fair + charm;

            if (combatScore >= AIConst.AIHero.COMBAT_THRESHOLD && combatScore > domesticScore * 1.3f)
                return HeroType.Combat;
            else if (domesticScore >= AIConst.AIHero.DOMESTIC_THRESHOLD && domesticScore > combatScore * 1.3f)
                return HeroType.Domestic;

            return HeroType.Balanced;
        }

        public static HeroType ClassifyHero(SaveHeroData hero)
        {
            return ClassifyHero(hero.GetAttr("str"), hero.GetAttr("leadship"), hero.GetAttr("inte"),
                hero.GetAttr("fair"), hero.GetAttr("charm"));
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
            float totalScore = str * 0.75f
                + inte + fair
                + leadship * 1.5f
                + charm * 1.2f;

            if (isKing)
                totalScore += 9999;

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
                return (secondaryAttr - primaryAttr) / 3;
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
                return 0;

            var cfg1 = WorldConfig.GetConfig(cityId1);
            var cfg2 = WorldConfig.GetConfig(cityId2);

            int manhattan = Math.Abs(cfg1.X - cfg2.X) + Math.Abs(cfg1.Y - cfg2.Y);
            if (manhattan <= 800)
                return 1;
            if (manhattan <= 2000)
                return 2;

            return 3;
        }

        /// <summary>
        /// 武将移动/登庸消耗的统一日程折算。
        /// 本国内：基础城市间日程（1~3日）。
        /// 他国家：基础日程 + 1（2~4日）。
        /// 所有"距离→天数"场景必须走此方法，禁止各处自行折算。
        /// </summary>
        public static int CalculateHeroDayDistance(int srcCityId, int destCityId, bool isCrossCountry)
        {
            int baseDays = CalculateCityDayDistance(srcCityId, destCityId);
            return isCrossCountry
                ? baseDays + 1
                : baseDays;
        }

        /// <summary>
        /// 按目标城市归属判断是否跨国（目标城市 forceId != 当前势力 forceId）。
        /// 目标城市不存在视为跨国（在野无主城市按他国家处理）。
        /// </summary>
        public static bool IsCrossCountry(int destCityId, int currentForceId)
        {
            var destCity = GameManager.Instance.GetCity(destCityId);
            return destCity == null || destCity.forceId != currentForceId;
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

        /// <summary>
        /// 计算战斗导致的防御方dev收入打折倍率
        /// 10回合: 0.95 (减少5%), 30回合: 0 (无收入), 线性插值
        /// </summary>
        public static float GetDefenceDevDiscount(int battleRounds)
        {
            if (battleRounds <= 10)
                return 1f;
            int maxRound = 30;
            if (battleRounds >= maxRound)
                return 0f;
            float t = (float)(battleRounds - 10)
                / (maxRound - 10);
            return 1f - (0.05f + t * (1f - 0.05f));
        }
    }

    public static class Economy
    {
        public static int CalculateTradeAmount(int goldCost)
        {
            return (int)(goldCost * 2f);
        }

        /// <summary>
        /// 计算单个武将的交易量：基数为 goldCost × 2，智力＞70时每点加 2%
        /// </summary>
        public static int CalculateHeroTradeAmount(int goldCost, int intelligence)
        {
            int baseAmount = CalculateTradeAmount(goldCost);
            int overThreshold = Math.Max(0, intelligence - 70);
            float bonus = overThreshold * 0.02f;
            return (int)(baseAmount * (1f + bonus));
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

        /// <summary>
        /// 计算登庸目标优先级分数
        /// 优先级：1日名将 > 2日名将 > 1日普通 > 2日普通
        /// 忠诚越低优先级系数越高
        /// </summary>
        public static int CalculateRecruitPriority(int dayDistance, bool isStarHero, int loyalty)
        {
            // 组别基础分：1日名将 > 2日名将 > 1日普通 > 2日普通（间距10000确保组别优先于忠诚差异）
            int groupBase;
            if (dayDistance <= 1 && isStarHero)
                groupBase = 40000;
            else if (dayDistance <= 2 && isStarHero)
                groupBase = 30000;
            else if (dayDistance <= 1)
                groupBase = 20000;
            else
                groupBase = 10000;

            // 忠诚越低系数越高（0~100）
            int loyaltyBonus = (100 - loyalty) * 10;

            return groupBase + loyaltyBonus;
        }
    }

    public static class Game
    {
        public static float CalculateCurrentYear(int totalSeasons)
        {
            int years = totalSeasons / 36;
            int seasons = totalSeasons % 36;
            return 194 + years + (seasons / (float)36);
        }

        public static int CalculateSeasonId(int round)
        {
            return (round % 36) + 1;
        }
    }

    public static class Diplomacy
    {
        public static int CalculateBattleRise()
        {
            return SysRandom.Range(3, 8 + 1);
        }
    }
}
