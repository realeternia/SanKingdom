using System;
using System.Collections.Generic;
using CommonConfig;
using Unity.VisualScripting;

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

            int baseAtk = (int)(hero1.str * 0.7f + armsConfig.Atk * (1f + sodBonus));
            int baseDef = (int)(hero1.leadShip * 0.7f + armsConfig.Def * (1f + sodBonus));

            // 科技加成：兵种属性加算
            int techAtk = ForceTech.GetArmsAttrAdd(hero1.forceId, troop.armsId, "Atk");
            int techDef = ForceTech.GetArmsAttrAdd(hero1.forceId, troop.armsId, "Def");
            int techMoveSpeed = ForceTech.GetArmsAttrAdd(hero1.forceId, troop.armsId, "MoveSpeed");

            return (baseAtk + techAtk, baseDef + techDef);
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
        /// 计算登庸在野武将的成功率
        /// 基础 SysConfigModify.RecruitWildBaseRate，非己方城市 ×0.5，加上 Recruit 的 KingAction 加成与科技 SuccessMul，封顶 100
        /// 调用方需先判断 hero.state == HeroState.Wild
        /// </summary>
        public static int CalculateRecruitWildRate(int cityId, int myHeroId, int targetHeroId)
        {
            var cityData = GameManager.Instance.GetCity(cityId);
            var hero = GameManager.Instance.GetHero(targetHeroId);

            int baseSuccessRate = GetSysConfigModifyResult("RecruitWildBaseRate", cityData.forceId);
            var heroCity = GameManager.Instance.GetCity(hero.cityId);
            if (heroCity == null || heroCity.forceId != cityData.forceId)
            {
                baseSuccessRate = (int)Math.Round(baseSuccessRate * 0.5f);
            }

            return ApplyRecruitBonus(baseSuccessRate, cityData.forceId, myHeroId, hero, targetHeroId);
        }

        /// <summary>
        /// 计算登庸俘虏/敌方在职武将的成功率
        /// 公式：RecruitEnemyOffset - 忠诚 * 3/4，最低 0，加上 Recruit 的 KingAction 加成与科技 SuccessMul，封顶 100
        /// 调用方需先判断 hero.state == HeroState.Catched 或（敌方在职）
        /// </summary>
        public static int CalculateRecruitEnemyRate(int cityId, int myHeroId, int targetHeroId)
        {
            var cityData = GameManager.Instance.GetCity(cityId);
            var hero = GameManager.Instance.GetHero(targetHeroId);

            int baseSuccessRate = GetSysConfigModifyResult("RecruitEnemyOffset", cityData.forceId) - hero.loyalty * 3 / 4;
            if (baseSuccessRate < 0)
                baseSuccessRate = 0;

            return ApplyRecruitBonus(baseSuccessRate, cityData.forceId, myHeroId, hero, targetHeroId);
        }

        /// <summary>
        /// 登庸成功率公共加成：KingAction 加成 + 科技 SuccessMul，封顶 100
        /// </summary>
        private static int ApplyRecruitBonus(int baseRate, int forceId, int myHeroId, SaveHeroData hero, int targetHeroId)
        {
            if (myHeroId > 0)
            {
                var executorHero = GameManager.Instance.GetHero(myHeroId);
                if (executorHero != null)
                {
                    var targetConfig = HeroConfig.GetConfig(targetHeroId);
                    int targetForceId = hero.forceId;
                    baseRate += CalcKingActionBonus(myHeroId, targetForceId, CityDevConfig.GetConfigByName("Recruit").Id, targetConfig);
                }
            }

            if (baseRate > 100) baseRate = 100;

            // 科技加成：登用成功率提升
            float techSuccessMul = ForceTech.GetKingActionSuccessMul(forceId, CityDevConfig.GetConfigByName("Recruit").Id);
            baseRate = ForceTech.ApplySuccessMul(baseRate, techSuccessMul);

            return baseRate;
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

            // 科技加成：KingAction 成功率提升
            float techSuccessMul = ForceTech.GetKingActionSuccessMul(executorHero.forceId, devId);
            rate = ForceTech.ApplySuccessMul(rate, techSuccessMul);

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

        public static int CalculateCaptureChance(int str, int forceId)
        {
            int addon = 0;
            if(str > 70)
                addon = str - 70;
            return GetSysConfigModifyResult("CaptureBaseChance", forceId) - addon / 6;
        }

        /// <summary>
        /// 计算常量修正结果：BaseVal + Random(RandomMin, RandomMax+1) + 科技AmountAdd，再乘科技AmountMul
        /// </summary>
        public static int GetSysConfigModifyResult(string name, int forceId)
        {
            var cfg = SysConfigModify.GetConfigByName(name);
            int result = cfg.BaseVal;
            if (cfg.RandomMax > cfg.RandomMin)
                result += SysRandom.Range(cfg.RandomMin, cfg.RandomMax + 1);
            else
                result += cfg.RandomMin;

            // 科技加成：AmountAdd + AmountMul
            float amountAdd = ForceTech.GetSysConfigAmountAdd(forceId, cfg.Id);
            result += (int)amountAdd;
            float amountMul = ForceTech.GetSysConfigAmountMul(forceId, cfg.Id);
            result = (int)ForceTech.ApplyAmountMul(result, amountMul);

            return result;
        }

        public static int CalculateAttrGrowth(int baseAttr, int level)
        {
            if (level <= 1) return 0;
            return Math.Max(8 * (level - 1), baseAttr * (level - 1) / 10);
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
            return ClassifyHero(hero.GetAttr("str"), hero.GetAttr("leadship"), hero.GetAttr("inte"), hero.GetAttr("fair"), hero.GetAttr("charm"));
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
        /// 计算两城市间曼哈顿距离
        /// </summary>
        public static int CalculateManhattanDist(int cityId1, int cityId2)
        {
            if (cityId1 == cityId2) return 0;
            var cfg1 = WorldConfig.GetConfig(cityId1);
            var cfg2 = WorldConfig.GetConfig(cityId2);
            return Math.Abs(cfg1.X - cfg2.X) + Math.Abs(cfg1.Y - cfg2.Y);
        }

        /// <summary>
        /// 武将移动消耗日程：ceil(曼哈顿距离 / MoveBaseDist)，敌方城市距离×1.5且至少+1天
        /// </summary>
        public static int CalculateMoveDayDistance(int srcCityId, int destCityId, int forceId)
        {
            int manhattan = CalculateManhattanDist(srcCityId, destCityId);
            if (manhattan == 0) return 0;
            int baseDist = SysFormula.Hero.GetSysConfigModifyResult("MoveBaseDist", forceId);
            bool isCross = IsCrossCountry(destCityId, forceId);
            float effectiveDist = isCross ? manhattan * 1.5f : manhattan;
            int days = (int)Math.Ceiling(effectiveDist / baseDist);
            if (isCross && days <= 1) days = 2;
            return days;
        }

        /// <summary>
        /// 登庸消耗日程：ceil(曼哈顿距离 / RecruitBaseDist)，敌方城市距离×1.5且至少+1天
        /// </summary>
        public static int CalculateRecruitDayDistance(int srcCityId, int destCityId, int forceId)
        {
            int manhattan = CalculateManhattanDist(srcCityId, destCityId);
            if (manhattan == 0) return 0;
            int baseDist = SysFormula.Hero.GetSysConfigModifyResult("RecruitBaseDist", forceId);
            bool isCross = IsCrossCountry(destCityId, forceId);
            float effectiveDist = isCross ? manhattan * 1.5f : manhattan;
            int days = (int)Math.Ceiling(effectiveDist / baseDist);
            if (isCross && days <= 1) days = 2;
            return days;
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

        /// <summary>
        /// 城市产出倍率：民心分级（≥95=1.2, ≥60=1.0, ≥30=0.8, 否则0.6），
        /// 战争状态额外叠加 WAR_PRODUCTION_MULTIPLIER-1（即-0.3），
        /// 最终 × 防御打折
        /// </summary>
        public static float CalculateProductionMultiplier(int happy, bool isInWar, float defenceDevDiscount)
        {
            float happyMult;
            if (happy >= 95)
                happyMult = 1.2f;
            else if (happy >= 60)
                happyMult = 1f;
            else if (happy >= 30)
                happyMult = 0.8f;
            else
                happyMult = 0.6f;

            float result = isInWar
                ? SystemConst.City.WAR_PRODUCTION_MULTIPLIER + happyMult - 1f
                : happyMult;
            result *= defenceDevDiscount;
            return result;
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
        /// <summary>
        /// 计算单个武将的交易量：基数为 goldCost × TradeBaseMultiplier，智力＞70时每点加 2%
        /// </summary>
        public static int CalculateHeroTradeAmount(int goldCost, int intelligence)
        {
            int baseAmount = goldCost * SystemConst.Economy.TRADE_BASE_MULTIPLIER;
            int overThreshold = Math.Max(0, intelligence - 70);
            float bonus = overThreshold * 0.02f;
            return (int)(baseAmount * (1f + bonus));
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

}
