using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;

public enum HeroType
{
    Combat,
    Domestic,
    Balanced
}

public class HeroDispatcher
{
    private const int COMBAT_THRESHOLD = 150;
    private const int DOMESTIC_THRESHOLD = 150;
    private const int MIN_REAR_HEROES = 1;
    
    public static HeroType ClassifyHero(SaveHeroData hero)
    {
        int str = hero.GetAttr("str");
        int leadship = hero.GetAttr("leadship");
        int inte = hero.GetAttr("inte");
        int fair = hero.GetAttr("fair");
        int charm = hero.GetAttr("charm");
        
        int combatScore = str + leadship + inte;
        int domesticScore = inte + fair + charm;
        
        if (combatScore >= COMBAT_THRESHOLD && combatScore > domesticScore * 1.3f)
        {
            return HeroType.Combat;
        }
        else if (domesticScore >= DOMESTIC_THRESHOLD && domesticScore > combatScore * 1.3f)
        {
            return HeroType.Domestic;
        }
        return HeroType.Balanced;
    }
    
    public static List<int> GetFrontlineCities(Player player)
    {
        var result = new List<int>();
        var cities = player.GetCityList();
        
        foreach (var city in cities)
        {
            if (CityEvaluator.IsFrontlineCity(city))
            {
                result.Add(city.cityId);
            }
        }
        return result;
    }
    
    public static List<int> GetRearCities(Player player)
    {
        var result = new List<int>();
        var cities = player.GetCityList();
        
        foreach (var city in cities)
        {
            if (!CityEvaluator.IsFrontlineCity(city))
            {
                result.Add(city.cityId);
            }
        }
        return result;
    }
    
    public static void DispatchHeroes(Player player)
    {
        var frontlineCities = GetFrontlineCities(player);
        var rearCities = GetRearCities(player);
        
        if (frontlineCities.Count == 0 || rearCities.Count == 0)
            return;
        
        var rearCombatHeroes = new List<SaveHeroData>();
        var rearCityHeroMap = new Dictionary<int, List<SaveHeroData>>();
        
        foreach (var cityId in rearCities)
        {
            var city = GameManager.Instance.GetCity(cityId);
            var heroIds = city.GetNormalHeroList();
            rearCityHeroMap[cityId] = new List<SaveHeroData>();
            
            foreach (var heroId in heroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                rearCityHeroMap[cityId].Add(hero);
                
                if (ClassifyHero(hero) == HeroType.Combat)
                {
                    rearCombatHeroes.Add(hero);
                }
            }
        }
        
        foreach (var cityId in frontlineCities)
        {
            var city = GameManager.Instance.GetCity(cityId);
            var heroIds = city.GetNormalHeroList();
            int combatCount = 0;
            
            foreach (var heroId in heroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (ClassifyHero(hero) == HeroType.Combat)
                    combatCount++;
            }
            
            int neededCombat = 3 - combatCount;
            
            for (int i = 0; i < neededCombat && rearCombatHeroes.Count > 0; i++)
            {
                var heroToMove = rearCombatHeroes[0];
                rearCombatHeroes.RemoveAt(0);
                
                bool canMove = player.CheckHeroRound(heroToMove.heroId);
                if (canMove)
                {
                    int srcCityId = heroToMove.cityId;
                    var srcCity = GameManager.Instance.GetCity(srcCityId);
                    
                    if (rearCityHeroMap.ContainsKey(srcCityId) && 
                        rearCityHeroMap[srcCityId].Count > MIN_REAR_HEROES)
                    {
                        player.MoveHeroToCity(srcCityId, cityId, new int[] { heroToMove.heroId });
                        rearCityHeroMap[srcCityId].Remove(heroToMove);
                        
                        GameLog.SetTag("AI").Info($"AI调度: 英雄{heroToMove.heroId}从后方城市{srcCityId}调往前线城市{cityId}");
                    }
                }
            }
        }
    }
}
