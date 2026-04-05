using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;

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
        int totalHeroes = city.GetNormalHeroList().Count;
        int initialAvailable = context.GetAvailableHeroes(city.cityId).Count;
        
        var cityNeeds = CityEvaluator.EvaluateCity(city);
        var availableTasks = TaskPriorityCalculator.GetAvailableTasks(city, state, cityNeeds);
        
        foreach (var task in availableTasks)
        {
            var availableHeroes = context.GetAvailableHeroes(city.cityId);
            if (availableHeroes.Count == 0)
                break;
            
            ExecuteTask(player, city, context, task, availableHeroes);
        }
        
        int finalAvailable = context.GetAvailableHeroes(city.cityId).Count;
        int actedCount = initialAvailable - finalAvailable;
        int soldier = city.GetAttr("soldier");
        
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 行动结束 已行动:{actedCount}/{totalHeroes} 黄金:{city.gold} 粮草:{city.food} 兵力:{soldier}");
    }
    
    private static bool ExecuteTask(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task, List<SaveHeroData> availableHeroes)
    {
        switch (task.config.Prefab)
        {
            case "CityDevNormal":
                return ExecuteNormalTask(player, city, context, task, availableHeroes);
            case "CityDevBattle":
                return TryExecuteAttack(player, city, context, task);
            case "CityDevMove":
                return HandleMove(player, city, context, task);
            case "CityDevUseHero":
                return HandleRecruitment(player, city, context, task);
            case "CityDevChange":
                return HandleFoodPurchase(player, city, context, task);
            case "CityDevPraiseHero":
                return HandlePraise(player, city, context, task);
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
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 内政[{GetTaskName(task.devId)}] 英雄:[{GetHeroNames(heroIds)}]");
            return true;
        }
        return false;
    }
    
    private static bool TryExecuteAttack(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
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
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击: 兵力{totalSoldier}不足500");
            return false;
        }
        
        var targetCity = GameManager.Instance.GetCity(attackTarget.Value);
        int enemySoldier = targetCity.GetAttr("soldier");
        
        if (totalSoldier < enemySoldier * 0.7f)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击: 己方{totalSoldier}少于敌方{enemySoldier}的70%");
            return false;
        }
        
        int foodNeeded = totalSoldier / 2;
        
        if (city.food < foodNeeded)
        {
            GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 跳过攻击: 粮食不足 需要{foodNeeded} 现有{city.food}");
            return false;
        }
        
        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        
        StrategicDecider.MarkTargetAttacked(player.forceId, attackTarget.Value);
        player.ExecuteCityBattleDev(city.cityId, task.devId, heroIds, foodNeeded, attackTarget.Value, true);
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(city.cityId)}] 攻击[{GetCityName(attackTarget.Value)}] 英雄:[{GetHeroNames(heroIds)}] 兵力:{totalSoldier}");
        return true;
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
    
    private static bool HandleFoodPurchase(Player player, SaveCityData city, AIStrategyContext context, TaskPriorityInfo task)
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
                (targetHero.state == HeroState.Normal && targetHero.loyalty < 80))
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
            if (hero.loyalty < 80)
            {
                lowLoyaltyHeroes.Add(hero);
            }
        }
        
        if (lowLoyaltyHeroes.Count == 0)
            return false;
        
        if (city.gold >= 100 * lowLoyaltyHeroes.Count)
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
        else if (frontlineCities.Count == 0 && rearCities.Count == 0)
        {
            return BalanceHeroesAcrossCities(player, city, task, availableHeroes);
        }
        
        return false;
    }
    
    private static bool MoveDomesticHeroesToRear(Player player, SaveCityData city, TaskPriorityInfo task, 
        List<SaveHeroData> availableHeroes, List<int> rearCities)
    {
        var domesticHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Domestic)
            .ToList();
        
        if (domesticHeroes.Count == 0)
            return false;
        
        var normalHeroes = city.GetNormalHeroList()
            .Select(h => GameManager.Instance.GetHero(h))
            .ToList();
        int combatCount = normalHeroes.Count(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat);
        
        if (combatCount < 2)
            return false;
        
        var heroToMove = domesticHeroes[0];
        var targetCityId = SelectBestRearCity(rearCities, heroToMove);
        
        if (targetCityId == 0)
            return false;
        
        return ExecuteHeroMove(player, city, heroToMove, targetCityId, task.devId, "后方城市");
    }
    
    private static bool MoveCombatHeroesToFrontline(Player player, SaveCityData city, TaskPriorityInfo task,
        List<SaveHeroData> availableHeroes, List<int> frontlineCities)
    {
        var combatHeroes = availableHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();
        
        if (combatHeroes.Count == 0)
            return false;
        
        var normalHeroes = city.GetNormalHeroList()
            .Select(h => GameManager.Instance.GetHero(h))
            .ToList();
        
        if (normalHeroes.Count <= 1)
            return false;
        
        var heroToMove = combatHeroes[0];
        var targetCityId = SelectBestFrontlineCity(frontlineCities, heroToMove);
        
        if (targetCityId == 0)
            return false;
        
        return ExecuteHeroMove(player, city, heroToMove, targetCityId, task.devId, "前线城市");
    }
    
    private static bool BalanceHeroesAcrossCities(Player player, SaveCityData city, TaskPriorityInfo task,
        List<SaveHeroData> availableHeroes)
    {
        var myCities = player.GetCityList();
        if (myCities.Count <= 1)
            return false;
        
        var normalHeroes = city.GetNormalHeroList();
        if (normalHeroes.Count <= 1)
            return false;
        
        int avgHeroes = myCities.Sum(c => c.GetNormalHeroList().Count) / myCities.Count;
        
        if (normalHeroes.Count <= avgHeroes)
            return false;
        
        var heroToMove = availableHeroes[0];
        
        var targetCity = myCities
            .Where(c => c.cityId != city.cityId)
            .OrderBy(c => c.GetNormalHeroList().Count)
            .FirstOrDefault();
        
        if (targetCity == null)
            return false;
        
        return ExecuteHeroMove(player, city, heroToMove, targetCity.cityId, task.devId, "平均分配");
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
        int soldierTotal = hero.soldier;
        int foodCost = soldierTotal * 20 / 20;
        
        if (srcCity.food < foodCost)
            return false;
        
        player.ExecuteCityMoveDev(srcCity.cityId, devId, new int[] { hero.heroId }, foodCost, targetCityId);
        GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(srcCity.cityId)}] 移动: [{GetHeroName(hero.heroId)}] -> [{GetCityName(targetCityId)}] 原因:{reason}");
        return true;
    }
}
