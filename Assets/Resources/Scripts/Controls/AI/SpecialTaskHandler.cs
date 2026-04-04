using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public class SpecialTaskHandler
{
    public static void HandleSpecialTasks(Player player, SaveCityData city, AIStrategyContext context)
    {
        HandleRecruitment(player, city, context);
        HandlePraise(player, city, context);
        HandleSearch(player, city, context);
    }
    
    public static bool HandleRecruitment(Player player, SaveCityData city, AIStrategyContext context)
    {
        var recruitableHeroes = city.GetRecruitableHeroList();
        if (recruitableHeroes.Count == 0)
            return false;
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var bestRecruiter = SelectBestRecruiter(availableHeroes);
        
        foreach (var targetHeroId in recruitableHeroes)
        {
            var targetHero = GameManager.Instance.GetHero(targetHeroId);
            if (ShouldRecruit(targetHero))
            {
                var recruitConfig = GetRecruitConfig();
                if (recruitConfig != null)
                {
                    bool success = player.ExecuteCityUseHero(city.cityId, recruitConfig.Id, 
                        bestRecruiter.heroId, targetHeroId, out _);
                    
                    if (success)
                    {
                        UnityEngine.Debug.Log($"AI登用成功: 城市{city.cityId} 英雄{bestRecruiter.heroId} 登用{targetHeroId}");
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    
    private static SaveHeroData SelectBestRecruiter(List<SaveHeroData> heroes)
    {
        var best = heroes[0];
        int bestScore = CalculateRecruitScore(best);
        
        foreach (var hero in heroes)
        {
            int score = CalculateRecruitScore(hero);
            if (score > bestScore)
            {
                bestScore = score;
                best = hero;
            }
        }
        
        return best;
    }
    
    private static int CalculateRecruitScore(SaveHeroData hero)
    {
        int charm = hero.GetAttr("charm");
        int inte = hero.GetAttr("inte");
        
        bool isKing = hero.heroId == ForceConfig.GetConfig(hero.forceId).HeroId;
        
        return charm * 2 + inte + (isKing ? 50 : 0);
    }
    
    private static bool ShouldRecruit(SaveHeroData hero)
    {
        return hero.state == HeroState.Wild || 
               hero.state == HeroState.Catched ||
               (hero.state == HeroState.Normal && hero.loyalty < 80);
    }
    
    private static CityDevConfig GetRecruitConfig()
    {
        return CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Prefab == "CityDevUseHero");
    }
    
    public static bool HandlePraise(Player player, SaveCityData city, AIStrategyContext context)
    {
        var lowLoyaltyHeroes = GetLowLoyaltyHeroes(city);
        if (lowLoyaltyHeroes.Count == 0)
            return false;
        
        var praiseConfig = GetPraiseConfig();
        if (praiseConfig == null)
            return false;
        
        int totalCost = 100 * lowLoyaltyHeroes.Count;
        if (city.gold < totalCost)
        {
            return HandlePraiseByAction(player, city, context, lowLoyaltyHeroes, praiseConfig);
        }
        
        var heroIds = lowLoyaltyHeroes.Select(h => h.heroId).ToArray();
        bool success = player.ExecuteCityPraiseHero(city.cityId, praiseConfig.Id, heroIds, 2, out _);
        
        if (success)
        {
            UnityEngine.Debug.Log($"AI褒奖(金钱): 城市{city.cityId} 褒奖{lowLoyaltyHeroes.Count}名英雄");
        }
        
        return success;
    }
    
    private static bool HandlePraiseByAction(Player player, SaveCityData city, AIStrategyContext context, 
        List<SaveHeroData> lowLoyaltyHeroes, CityDevConfig praiseConfig)
    {
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var heroToPraise = lowLoyaltyHeroes.OrderBy(h => h.loyalty).First();
        var praiser = availableHeroes[0];
        
        bool success = player.ExecuteCityPraiseHero(city.cityId, praiseConfig.Id, 
            new int[] { heroToPraise.heroId }, 1, out _);
        
        if (success)
        {
            UnityEngine.Debug.Log($"AI褒奖(行动): 城市{city.cityId} 褒奖英雄{heroToPraise.heroId}");
        }
        
        return success;
    }
    
    private static List<SaveHeroData> GetLowLoyaltyHeroes(SaveCityData city)
    {
        var result = new List<SaveHeroData>();
        
        foreach (var heroId in city.GetNormalHeroList())
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero.loyalty < 80)
            {
                result.Add(hero);
            }
        }
        
        return result;
    }
    
    private static CityDevConfig GetPraiseConfig()
    {
        return CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Prefab == "CityDevPraiseHero");
    }
    
    public static bool HandleSearch(Player player, SaveCityData city, AIStrategyContext context)
    {
        if (!ShouldSearch(city))
            return false;
        
        var availableHeroes = context.GetAvailableHeroes(city.cityId);
        if (availableHeroes.Count == 0)
            return false;
        
        var searchConfig = GetSearchConfig();
        if (searchConfig == null)
            return false;
        
        var bestSearcher = SelectBestSearcher(availableHeroes);
        
        bool success = player.ExecuteCityDev(city.cityId, searchConfig.Id, 
            new int[] { bestSearcher.heroId }, out _);
        
        if (success)
        {
            UnityEngine.Debug.Log($"AI搜索: 城市{city.cityId} 英雄{bestSearcher.heroId}");
        }
        
        return success;
    }
    
    private static bool ShouldSearch(SaveCityData city)
    {
        int heroCount = city.GetNormalHeroList().Count;
        return heroCount < 5;
    }
    
    private static SaveHeroData SelectBestSearcher(List<SaveHeroData> heroes)
    {
        return heroes
            .OrderByDescending(h => h.GetAttr("charm") + h.GetAttr("inte"))
            .First();
    }
    
    private static CityDevConfig GetSearchConfig()
    {
        return CityDevConfig.ConfigList
            .FirstOrDefault(c => c.ActionName == "find");
    }
}
