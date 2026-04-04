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
        
        var citiesByStrategy = new Dictionary<CityStrategyState, List<SaveCityData>>();
        citiesByStrategy[CityStrategyState.Dev] = new List<SaveCityData>();
        citiesByStrategy[CityStrategyState.Def] = new List<SaveCityData>();
        citiesByStrategy[CityStrategyState.Atk] = new List<SaveCityData>();
        
        foreach (var city in context.cities)
        {
            var state = cityStrategies.ContainsKey(city.cityId) ? 
                cityStrategies[city.cityId] : CityStrategyState.Dev;
            citiesByStrategy[state].Add(city);
        }
        
        ExecuteSpecialTasks(player, context);
        
        foreach (var kvp in citiesByStrategy)
        {
            if (kvp.Value.Count == 0)
                continue;
            
            var strategy = AIStrategyManager.Instance.GetStrategy(kvp.Key);
            var strategyContext = CreateContextForCities(player, kvp.Value);
            
            UnityEngine.Debug.Log($"AI执行策略: {strategy.GetStrategyName()} 城市: {kvp.Value.Count}");
            strategy.Execute(strategyContext);
        }
    }
    
    private static AIStrategyContext CreateContextForCities(Player player, List<SaveCityData> cities)
    {
        return new AIStrategyContext(player, cities);
    }
    
    private static void ExecuteSpecialTasks(Player player, AIStrategyContext context)
    {
        foreach (var city in context.cities)
        {
            HandleRecruitment(player, city, context);
            HandlePraise(player, city, context);
            HandleSearch(player, city, context);
        }
    }
    
    private static void HandleRecruitment(Player player, SaveCityData city, AIStrategyContext context)
    {
        var recruitableHeroes = city.GetRecruitableHeroList();
        if (recruitableHeroes.Count == 0)
            return;
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return;
        
        var bestRecruiter = availableHeroes[0];
        int bestCharm = bestRecruiter.GetAttr("charm");
        
        foreach (var hero in availableHeroes)
        {
            int charm = hero.GetAttr("charm");
            if (charm > bestCharm)
            {
                bestCharm = charm;
                bestRecruiter = hero;
            }
        }
        
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
                    return;
                }
            }
        }
    }
    
    private static void HandlePraise(Player player, SaveCityData city, AIStrategyContext context)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return;
        
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
            return;
        
        var praiseConfig = CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Prefab == "CityDevPraiseHero");
        
        if (praiseConfig != null && city.gold >= 100 * lowLoyaltyHeroes.Count)
        {
            var heroIds = lowLoyaltyHeroes.Select(h => h.heroId).ToArray();
            player.ExecuteCityPraiseHero(city.cityId, praiseConfig.Id, heroIds, 2, out _);
            UnityEngine.Debug.Log($"AI褒奖: 城市{city.cityId} 褒奖{lowLoyaltyHeroes.Count}名英雄");
        }
    }
    
    private static void HandleSearch(Player player, SaveCityData city, AIStrategyContext context)
    {
        var heroCount = city.GetNormalHeroList().Count;
        if (heroCount >= 5)
            return;
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return;
        
        var searchConfig = CityDevConfig.ConfigList
            .FirstOrDefault(c => c.ActionName == "find");
        
        if (searchConfig != null)
        {
            var bestSearcher = availableHeroes
                .OrderByDescending(h => h.GetAttr("charm") + h.GetAttr("inte"))
                .First();
            
            player.ExecuteCityDev(city.cityId, searchConfig.Id, 
                new int[] { bestSearcher.heroId }, out _);
            UnityEngine.Debug.Log($"AI搜索: 城市{city.cityId} 英雄{bestSearcher.heroId}");
        }
    }
}
