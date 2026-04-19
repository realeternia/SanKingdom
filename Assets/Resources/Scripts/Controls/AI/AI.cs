using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;
using UnityEngine;
using System;

public static class AI
{
    private static string GetForceName(int forceId)
    {
        var cfg = ForceConfig.GetConfig(forceId);
        return cfg != null ? cfg.Cname : forceId.ToString();
    }
    
    private static string GetHeroName(int heroId)
    {
        var cfg = HeroConfig.GetConfig(heroId);
        return cfg != null ? cfg.Name : heroId.ToString();
    }
    
    private static string GetCityName(int cityId)
    {
        var cfg = WorldConfig.GetConfig(cityId);
        return cfg != null ? cfg.Cname : cityId.ToString();
    }
    
    private static string GetTaskName(int devId)
    {
        var cfg = CityDevConfig.GetConfig(devId);
        return cfg != null ? cfg.Cname : devId.ToString();
    }
    
    private static string GetHeroNames(int[] heroIds)
    {
        return string.Join(",", heroIds.Select(GetHeroName));
    }
    
    public static IEnumerator ExecuteAiActions(Player player)
    {
        var context = new AIStrategyContext(player);
        
        HeroDispatcher.DispatchHeroes(player);
        
        var cityStrategies = StrategicDecider.DetermineCityStrategies(player);
        
        foreach (var city in context.cities)
        {
            var state = cityStrategies.ContainsKey(city.cityId) ? 
                cityStrategies[city.cityId] : CityStrategyState.Dev;
            
            yield return ExecuteCityActions(player, city, context, state);
        }
    }
    
    private static IEnumerator ExecuteCityActions(Player player, SaveCityData city, AIStrategyContext context, CityStrategyState state)
    {
        int totalHeroes = city.GetNormalHeroList().Count;
        int initialAvailable = context.GetAvailableHeroes(city.cityId).Count;
        int soldier = city.GetAttr("soldier");
        
        var nearCityIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
        var nearCityNames = nearCityIds != null 
            ? string.Join(", ", nearCityIds.Select(GetCityName)) 
            : "无";
        
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 开始决策 武将:{totalHeroes} 士兵:{soldier} 粮食:{city.food} 金钱:{city.gold} 相邻城市:[{nearCityNames}]");
        
        var cityNeeds = CityEvaluator.EvaluateCity(city);
        var availableTasks = TaskPriorityCalculator.GetAvailableTasks(city, state, cityNeeds);
        
        foreach (var task in availableTasks)
        {
            var availableHeroes = context.GetAvailableHeroes(city.cityId);
            if (availableHeroes.Count == 0)
                break;
            
            var result = ExecuteTask(player, city, context, task, availableHeroes);
            if (result is IEnumerator enumerator)
            {
                yield return enumerator;
            }
        }
        
        int finalAvailable = context.GetAvailableHeroes(city.cityId).Count;
        int actedCount = initialAvailable - finalAvailable;
        soldier = city.GetAttr("soldier");
        
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 行动结束 已行动:{actedCount}/{totalHeroes} 黄金:{city.gold} 粮草:{city.food} 兵力:{soldier}");
    }
    
    private static object ExecuteTask(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task, List<SaveHeroData> availableHeroes)
    {
        var result = true;
        int count = 0;
        while (result && count < 2)
        {
            switch (task.config.Prefab)
            {
                case "CityDevNormal":
                    result = ExecuteNormalTask(player, city, context, task);
                    break;
                case "CityDevBattle":
                    return TryExecuteAttack(player, city, context, task);
                case "CityDevMove":
                    result = HandleMove(player, city, context, task);
                    break;
                case "CityDevUseHero":
                    result = HandleRecruitment(player, city, context, task);
                    break;
                case "CityDevChange":
                    result = HandleFoodPurchase(player, city, context, task);
                    break;
                case "CityDevPraiseHero":
                    result = HandlePraise(player, city, context, task);
                    break;
                default:
                    result = false;
                    break;
            }
            count++;
        }
        return result;
    }
    
    private static bool ExecuteNormalTask(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        var matchedHeroes = HeroTaskMatcher.AssignTasksToHeroes(availableHeroes, new List<TaskPriorityInfo> { task });
        if (matchedHeroes.Count == 0)
            return false;
        
        var heroIds = matchedHeroes.Select(m => m.hero.heroId).ToArray();
        if (heroIds.Length > 0)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 内政[{GetTaskName(task.devId)}] 英雄:[{GetHeroNames(heroIds)}]");
            return player.ExecuteCityDev(city.cityId, task.devId, heroIds, out _);
        }
        return false;
    }
    
    private static IEnumerator TryExecuteAttack(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
    {
        var attackTarget = StrategicDecider.GetAttackTarget(city.cityId);
        if (!attackTarget.HasValue)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击: 没有可攻击的目标");
            yield break;
        }
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击[{GetCityName(attackTarget.Value)}]: 没有可用英雄");
            yield break;
        }
        
        int heroCountToUse = Math.Max(1, availableHeroes.Count - 1);
        
        var combatHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();
        
        if (combatHeroes.Count >= heroCountToUse)
        {
            combatHeroes = combatHeroes.Take(heroCountToUse).ToList();
        }
        else
        {
            var nonCombatHeroes = availableHeroes
                .Where(h => HeroDispatcher.ClassifyHero(h) != HeroType.Combat)
                .ToList();
            combatHeroes = combatHeroes.Concat(nonCombatHeroes).Take(heroCountToUse).ToList();
        }
        
        if (combatHeroes.Count == 0)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击[{GetCityName(attackTarget.Value)}]: 无法选择战斗英雄");
            yield break;
        }
        
        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        var heroSoldierDict = DistributeSoldierToHeroes(city, heroIds);
        
        int totalSoldier = heroSoldierDict.Values.Sum();
        
        if (totalSoldier < SystemConst.AIStrategy.AI_MIN_ATTACK_SOLDIER)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击[{GetCityName(attackTarget.Value)}]: 兵力{totalSoldier}不足500");
            yield break;
        }
        
        var targetCity = GameManager.Instance.GetCity(attackTarget.Value);
        int enemySoldier = targetCity.GetAttr("soldier");
        
        if (totalSoldier < enemySoldier * SystemConst.AIStrategy.AI_ATTACK_ADVANTAGE_RATIO)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击[{GetCityName(attackTarget.Value)}]: 己方{totalSoldier}少于敌方{enemySoldier}的70%");
            yield break;
        }
        
        int foodNeeded = totalSoldier / SystemConst.AIStrategy.AI_FOOD_NEED_DIVISOR;
        
        if (city.food < foodNeeded)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击[{GetCityName(attackTarget.Value)}]: 粮食不足 需要{foodNeeded} 现有{city.food}");
            yield break;
        }
        
        StrategicDecider.MarkTargetAttacked(player.forceId, attackTarget.Value);
        player.ExecuteCityBattleDev(city.cityId, task.devId, heroIds, foodNeeded, attackTarget.Value, true, heroSoldierDict);
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 攻击[{GetCityName(attackTarget.Value)}] 英雄:[{GetHeroNames(heroIds)}] 兵力:{totalSoldier}");
        
        while (BattleManager.Instance.IsBattleRunning)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private static Dictionary<int, int> DistributeSoldierToHeroes(SaveCityData city, int[] heroIds)
    {
        return city.DistributeSoldierDefault(heroIds);
    }
    
    private static int? SelectBestAttackTarget(AIStrategyContext context, SaveCityData sourceCity)
    {
        var myCityIds = new HashSet<int>(context.cities.Select(c => c.cityId));
        var nearCityIds = WorldConfig.GetConfig(sourceCity.cityId)?.WorldNearIds;
        
        if (nearCityIds == null)
            return null;
        
        var potentialTargets = new List<int>();
        
        foreach (var nearId in nearCityIds)
        {
            if (myCityIds.Contains(nearId))
                continue;
            
            var nearCity = GameManager.Instance.GetCity(nearId);
            if (nearCity != null && nearCity.forceId != context.player.forceId)
            {
                potentialTargets.Add(nearId);
            }
        }
        
        if (potentialTargets.Count == 0)
            return null;
        
        var connectedCountCache = new Dictionary<int, int>();
        foreach (var targetId in potentialTargets)
        {
            connectedCountCache[targetId] = CountConnectedMyCities(targetId, myCityIds);
        }
        
        potentialTargets.Sort((a, b) => 
        {
            var cityA = GameManager.Instance.GetCity(a);
            var cityB = GameManager.Instance.GetCity(b);
            
            int connectedMyCitiesA = connectedCountCache[a];
            int connectedMyCitiesB = connectedCountCache[b];

            var factorA = cityA.GetAttr("soldier") / Math.Max(0.2, connectedMyCitiesA);
            var factorB =  cityB.GetAttr("soldier") / Math.Max(0.2, connectedMyCitiesB);
            
            return factorA.CompareTo(factorB);
        });
        
        return potentialTargets[0];
    }
    
    private static int CountConnectedMyCities(int targetCityId, HashSet<int> myCityIds)
    {
        var targetNearIds = WorldConfig.GetConfig(targetCityId)?.WorldNearIds;
        if (targetNearIds == null)
            return 0;
        
        int count = 0;
        foreach (var nearId in targetNearIds)
        {
            if (myCityIds.Contains(nearId))
                count++;
        }
        return count;
    }
    
    private static bool HandleFoodPurchase(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
    {
        int totalSoldier = city.GetAttr("soldier");
        
        int foodThreshold = totalSoldier / 2;
        if (totalSoldier > 0 && city.food < foodThreshold && city.gold >= SystemConst.AIStrategy.AI_BUY_FOOD_MIN_GOLD)
        {
            var availableHeroes = context.GetAvailableHeroes(city.cityId);
            if (availableHeroes.Count == 0)
                return false;
            
            int[] amountOptions = { 300, 500, 1000, 2000, 3000 };
            int amount = 0;
            foreach (var amt in amountOptions)
            {
                if (city.gold >= amt)
                {
                    amount = amt;
                }
                else
                {
                    break;
                }
            }
            
            if (amount > 0)
            {
                var bestHero = availableHeroes
                    .OrderByDescending(h => h.GetAttr("inte"))
                    .First();
                
                const float EXCHANGE_RATE = SystemConst.Economy.EXCHANGE_RATE;
                player.ExecuteCityChange(city.cityId, task.devId, 
                    new int[] { bestHero.heroId }, true, amount, EXCHANGE_RATE, out _);
                GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 买粮: 花费{amount}黄金 买入{(int)(amount * EXCHANGE_RATE)}粮食 执行者:[{GetHeroName(bestHero.heroId)}]");
                return true;
            }
        }
        return false;
    }
    
    private static bool HandleRecruitment(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
    {
        var recruitableHeroes = city.GetRecruitableHeroList();
        if (recruitableHeroes.Count == 0)
            return false;
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var bestRecruiter = availableHeroes
            .OrderByDescending(h => h.GetAttr("charm"))
            .First();
        
        foreach (var targetHeroId in recruitableHeroes)
        {
            var targetHero = GameManager.Instance.GetHero(targetHeroId);
            if (targetHero.state == HeroState.Wild || 
                (targetHero.state == HeroState.Catched) ||
                (targetHero.state == HeroState.Normal && targetHero.loyalty < SystemConst.AIStrategy.AI_RECRUIT_ENEMY_LOYALTY_THRESHOLD))
            {
                player.ExecuteCityUseHero(city.cityId, task.devId, bestRecruiter.heroId, targetHeroId, out _);
                GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 登用: [{GetHeroName(bestRecruiter.heroId)}] 登用 [{GetHeroName(targetHeroId)}]");
                return true;
            }
        }
        return false;
    }
    
    private static bool HandlePraise(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var lowLoyaltyHeroes = new List<SaveHeroData>();
        foreach (var heroId in city.GetNormalHeroList())
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero.loyalty < SystemConst.AIStrategy.AI_PRAISE_LOYALTY_THRESHOLD)
            {
                lowLoyaltyHeroes.Add(hero);
            }
        }
        
        if (lowLoyaltyHeroes.Count == 0)
            return false;
        
        if (city.gold >= SystemConst.Hero.PRAISE_GOLD_COST_PER_HERO * lowLoyaltyHeroes.Count)
        {
            var heroIds = lowLoyaltyHeroes.Select(h => h.heroId).ToArray();
            player.ExecuteCityPraiseHero(city.cityId, task.devId, heroIds, 2, out _);
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 褒奖: [{GetHeroNames(heroIds)}] 共{lowLoyaltyHeroes.Count}人");
            return true;
        }
        return false;
    }
    
    private static bool HandleMove(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;

        if (city.GetNormalHeroList().Count <= SystemConst.AIStrategy.AI_MIN_STAY_HEROES)
            return false;
        
        var frontlineCities = HeroDispatcher.GetFrontlineCities(player);
        var rearCities = HeroDispatcher.GetRearCities(player);
        
        bool isFrontline = CityEvaluator.IsFrontlineCity(city);
        
        if (isFrontline && rearCities.Count > 0)
        {
            return MoveDomesticHeroesToRear(player, city, task, availableHeroes, rearCities);
        }
        else if (!isFrontline && frontlineCities.Count > 0)
        {
            return MoveCombatHeroesToFrontline(player, city, task, availableHeroes, frontlineCities);
        }
        else if (frontlineCities.Count > 0 && rearCities.Count == 0)
        {
            return BalanceHeroesToEmptyFrontline(player, city, task, availableHeroes, frontlineCities);
        }
        else if (frontlineCities.Count == 0 && rearCities.Count == 0)
        {
            return BalanceHeroesAcrossCities(player, city, task, availableHeroes);
        }
        
        return false;
    }
    
    private static bool MoveDomesticHeroesToRear(Player player, SaveCityData srcCity, TaskPriorityInfo task, List<SaveHeroData> availableHeroes, List<int> rearCities)
    {
        var domesticHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Domestic)
            .ToList();
        
        if (domesticHeroes.Count == 0)
            return false;
         
        var heroToMove = domesticHeroes[0];
        var targetCityId = SelectBestRearCity(rearCities, heroToMove);
        
        if (targetCityId == 0)
            return false;
        
        return ExecuteHeroMove(player, srcCity, heroToMove, targetCityId, task.devId, "后方城市");
    }
    
    private static bool MoveCombatHeroesToFrontline(Player player, SaveCityData srcCity, TaskPriorityInfo task, List<SaveHeroData> availableHeroes, List<int> frontlineCities)
    {
        var combatHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();
            
        if (combatHeroes.Count == 0)
            return false;
        
        var heroToMove = combatHeroes[0];
        var targetCityId = SelectBestFrontlineCity(frontlineCities, heroToMove);
        
        if (targetCityId == 0)
            return false;
        
        return ExecuteHeroMove(player, srcCity, heroToMove, targetCityId, task.devId, "前线城市");
    }
    
    private static bool BalanceHeroesAcrossCities(Player player, SaveCityData srcCity, TaskPriorityInfo task, List<SaveHeroData> availableHeroes)
    {
        var myCities = player.GetCityList();
        if (myCities.Count <= 1)
            return false;
        
        var normalHeroes = srcCity.GetNormalHeroList();
        if (normalHeroes.Count <= 1)
            return false;
        
        int avgHeroes = myCities.Sum(c => c.GetNormalHeroList().Count) / myCities.Count;
        
        if (normalHeroes.Count <= avgHeroes)
            return false;
        
        var heroToMove = availableHeroes[0];
        
        var targetCity = myCities
            .Where(c => c.cityId != srcCity.cityId)
            .OrderBy(c => c.GetNormalHeroList().Count)
            .FirstOrDefault();
        
        if (targetCity == null)
            return false;
        
        return ExecuteHeroMove(player, srcCity, heroToMove, targetCity.cityId, task.devId, "平均分配");
    }
    
    private static bool BalanceHeroesToEmptyFrontline(Player player, SaveCityData srcCity, TaskPriorityInfo task, List<SaveHeroData> availableHeroes, List<int> frontlineCities)
    {
        int emptyFrontlineCityId = 0;
        int minHeroCount = int.MaxValue;
        
        foreach (var cityId in frontlineCities)
        {
            if (cityId == srcCity.cityId)
                continue;
                
            var city = GameManager.Instance.GetCity(cityId);
            if (city == null) continue;
            
            int heroCount = city.GetNormalHeroList().Count;
            if (heroCount < minHeroCount)
            {
                minHeroCount = heroCount;
                emptyFrontlineCityId = cityId;
            }
        }
        
        if (emptyFrontlineCityId == 0 || minHeroCount >= srcCity.GetNormalHeroList().Count - 1)
            return false;
        
        var heroToMove = availableHeroes[0];
        return ExecuteHeroMove(player, srcCity, heroToMove, emptyFrontlineCityId, task.devId, "补充空前线");
    }
    
    private static int SelectBestRearCity(List<int> rearCities, SaveHeroData hero)
    {
        int bestCityId = 0;
        int minHeroCount = int.MaxValue;
        
        foreach (var cityId in rearCities)
        {
            var city = GameManager.Instance.GetCity(cityId);
            if (city == null) continue;
            
            int heroCount = city.GetNormalHeroList().Count;
            if (heroCount < minHeroCount)
            {
                minHeroCount = heroCount;
                bestCityId = cityId;
            }
        }
        
        return bestCityId;
    }
    
    private static int SelectBestFrontlineCity(List<int> frontlineCities, SaveHeroData hero)
    {
        int bestCityId = 0;
        int minCombatCount = int.MaxValue;
        
        foreach (var cityId in frontlineCities)
        {
            var city = GameManager.Instance.GetCity(cityId);
            if (city == null) continue;
            
            var heroes = city.GetNormalHeroList()
                .Select(h => GameManager.Instance.GetHero(h))
                .ToList();
            int combatCount = heroes.Count(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat);
            
            if (combatCount < minCombatCount)
            {
                minCombatCount = combatCount;
                bestCityId = cityId;
            }
        }
        
        return bestCityId;
    }
    
    private static bool ExecuteHeroMove(Player player, SaveCityData srcCity, SaveHeroData hero, int targetCityId, int devId, string reason)
    {
        int soldierTotal = srcCity.GetAttr("soldier");
        int foodCost = soldierTotal * SystemConst.Expedition.SOLDIER_FOOD_COST_DIVISOR / SystemConst.Expedition.SOLDIER_FOOD_COST_DIVISOR;
        
        if (srcCity.food < foodCost)
            return false;
        
        player.ExecuteCityMoveDev(srcCity.cityId, devId, new int[] { hero.heroId }, foodCost, targetCityId);
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(srcCity.cityId)}] 移动: [{GetHeroName(hero.heroId)}] -> [{GetCityName(targetCityId)}] 原因:{reason}");
        return true;
    }
}
