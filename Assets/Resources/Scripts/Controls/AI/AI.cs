using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public static class AI
{
    public static void ExecuteAiActions(Player player)
    {
        var context = new AIStrategyContext(player);
        
        HeroDispatcher.DispatchHeroes(player);
        
        var cityStrategies = StrategicDecider.DetermineCityStrategies(player);
        
        foreach (var city in context.cities)
        {
            var state = cityStrategies.ContainsKey(city.cityId) ? 
                cityStrategies[city.cityId] : CityStrategyState.Dev;
            
            ExecuteCityActions(player, city, context, state);
        }
    }
    
    private static void ExecuteCityActions(Player player, SaveCityData city, AIStrategyContext context, CityStrategyState state)
    {
        var cityNeeds = CityEvaluator.EvaluateCity(city);
        var availableTasks = TaskPriorityCalculator.GetAvailableTasks(city, state, cityNeeds);
        
        foreach (var task in availableTasks)
        {
            var availableHeroes = context.GetAvailableHeroes(city.cityId);
            if (availableHeroes.Count == 0)
                return;
            
            if (ExecuteTask(player, city, context, task, availableHeroes))
            {
                UnityEngine.Debug.Log($"AI行动: 城市{city.cityId} 状态{state} 执行{task.config.Prefab} 任务{task.devId}");
            }
        }
    }
    
    private static bool ExecuteTask(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task, List<SaveHeroData> availableHeroes)
    {
        switch (task.config.Prefab)
        {
            case "CityDevNormal":
                return ExecuteNormalTask(player, city, context, task, availableHeroes);
            case "CityDevBattle":
                return TryExecuteAttack(player, city, context);
            case "CityDevUseHero":
                return HandleRecruitment(player, city, context);
            case "CityDevChange":
                return HandleFoodPurchase(player, city, context);
            case "CityDevPraiseHero":
                return HandlePraise(player, city, context);
            default:
                return false;
        }
    }
    
    private static bool ExecuteNormalTask(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task, List<SaveHeroData> availableHeroes)
    {
        var matchedHeroes = HeroTaskMatcher.AssignTasksToHeroes(availableHeroes, new List<TaskPriorityInfo> { task });
        if (matchedHeroes.Count == 0)
            return false;
        
        var heroIds = matchedHeroes.Select(m => m.hero.heroId).ToArray();
        if (heroIds.Length > 0)
        {
            player.ExecuteCityDev(city.cityId, task.devId, heroIds, out _);
            return true;
        }
        return false;
    }
    
    private static bool TryExecuteAttack(Player player, SaveCityData city, AIStrategyContext context)
    {
        var attackTarget = SelectBestAttackTarget(context, city);
        if (!attackTarget.HasValue)
            return false;
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var combatHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();
        
        if (combatHeroes.Count == 0)
            combatHeroes = availableHeroes.Take(3).ToList();
        
        if (combatHeroes.Count == 0)
            return false;
        
        int totalSoldier = combatHeroes.Sum(h => h.soldier);
        
        if (totalSoldier < 500)
        {
            UnityEngine.Debug.Log($"AI跳过攻击: 己方兵力{totalSoldier}不足500");
            return false;
        }
        
        var targetCity = GameManager.Instance.GetCity(attackTarget.Value);
        int enemySoldier = targetCity.GetAttr("soldier");
        
        if (totalSoldier < enemySoldier * 0.7f)
        {
            UnityEngine.Debug.Log($"AI跳过攻击: 己方兵力{totalSoldier}少于敌方{enemySoldier}的70%");
            return false;
        }
        
        int foodNeeded = totalSoldier / 2;
        
        if (city.food < foodNeeded)
        {
            UnityEngine.Debug.Log($"AI跳过攻击: 粮食不足 需要{foodNeeded} 现有{city.food}");
            return false;
        }
        
        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        
        var battleConfig = CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Prefab == "CityDevBattle" && c.FindEnemy);
        
        if (battleConfig != null)
        {
            StrategicDecider.MarkTargetAttacked(player.forceId, attackTarget.Value);
            player.ExecuteCityBattleDev(city.cityId, battleConfig.Id, heroIds, foodNeeded, attackTarget.Value, true);
            UnityEngine.Debug.Log($"AI攻击: 城市{city.cityId} 攻击{attackTarget.Value} 英雄{string.Join(",", heroIds)}");
            return true;
        }
        
        return false;
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
        
        potentialTargets.Sort((a, b) => 
        {
            var cityA = GameManager.Instance.GetCity(a);
            var cityB = GameManager.Instance.GetCity(b);
            return cityA.GetAttr("soldier").CompareTo(cityB.GetAttr("soldier"));
        });
        
        return potentialTargets[0];
    }
    
    private static bool HandleFoodPurchase(Player player, SaveCityData city, AIStrategyContext context)
    {
        int totalSoldier = city.GetNormalHeroList()
            .Select(h => GameManager.Instance.GetHero(h))
            .Sum(h => h.soldier);
        
        int foodThreshold = totalSoldier / 2;
        if (totalSoldier > 0 && city.food < foodThreshold && city.gold >= 300)
        {
            var availableHeroes = context.GetAvailableHeroes(city.cityId);
            if (availableHeroes.Count == 0)
                return false;
            
            var changeConfig = CityDevConfig.ConfigList
                .FirstOrDefault(c => c.Prefab == "CityDevChange");
            
            if (changeConfig != null)
            {
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
                    
                    const float EXCHANGE_RATE = 0.9f;
                    player.ExecuteCityChange(city.cityId, changeConfig.Id, 
                        new int[] { bestHero.heroId }, true, amount, EXCHANGE_RATE, out _);
                    UnityEngine.Debug.Log($"AI买粮: 城市{city.cityId} 花费{amount}黄金 买入{(int)(amount * EXCHANGE_RATE)}粮食");
                    return true;
                }
            }
        }
        return false;
    }
    
    private static bool HandleRecruitment(Player player, SaveCityData city, AIStrategyContext context)
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
                (targetHero.state == HeroState.Normal && targetHero.loyalty < 80))
            {
                var recruitConfig = CityDevConfig.ConfigList
                    .FirstOrDefault(c => c.Prefab == "CityDevUseHero");
                
                if (recruitConfig != null)
                {
                    player.ExecuteCityUseHero(city.cityId, recruitConfig.Id, 
                        bestRecruiter.heroId, targetHeroId, out _, false);
                    UnityEngine.Debug.Log($"AI登用: 城市{city.cityId} 英雄{bestRecruiter.heroId} 登用{targetHeroId}");
                    return true;
                }
            }
        }
        return false;
    }
    
    private static bool HandlePraise(Player player, SaveCityData city, AIStrategyContext context)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var lowLoyaltyHeroes = new List<SaveHeroData>();
        foreach (var heroId in city.GetNormalHeroList())
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero.loyalty < 80)
            {
                lowLoyaltyHeroes.Add(hero);
            }
        }
        
        if (lowLoyaltyHeroes.Count == 0)
            return false;
        
        var praiseConfig = CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Prefab == "CityDevPraiseHero");
        
        if (praiseConfig != null && city.gold >= 100 * lowLoyaltyHeroes.Count)
        {
            var heroIds = lowLoyaltyHeroes.Select(h => h.heroId).ToArray();
            player.ExecuteCityPraiseHero(city.cityId, praiseConfig.Id, heroIds, 2, out _);
            UnityEngine.Debug.Log($"AI褒奖: 城市{city.cityId} 褒奖{lowLoyaltyHeroes.Count}名英雄");
            return true;
        }
        return false;
    }
}
