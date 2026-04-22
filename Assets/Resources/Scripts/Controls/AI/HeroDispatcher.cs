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
    private const int COMBAT_THRESHOLD = SystemConst.AIHero.COMBAT_THRESHOLD;
    private const int DOMESTIC_THRESHOLD = SystemConst.AIHero.DOMESTIC_THRESHOLD;
    private const int MIN_REAR_HEROES = SystemConst.AIHero.MIN_REAR_HEROES;
    
    public static HeroType ClassifyHero(SaveHeroData hero)
    {
        return SysFormula.Hero.ClassifyHero(
            hero.GetAttr("str"), hero.GetAttr("leadship"), hero.GetAttr("inte"),
            hero.GetAttr("fair"), hero.GetAttr("charm"));
    }
    
    public static List<int> GetFrontlineCities(SaveForceData force)
    {
        var result = new List<int>();
        var cities = force.GetCityList();
        
        foreach (var city in cities)
        {
            if (CityEvaluator.IsFrontlineCity(city))
            {
                result.Add(city.cityId);
            }
        }
        return result;
    }
    
    public static List<int> GetRearCities(SaveForceData force)
    {
        var result = new List<int>();
        var cities = force.GetCityList();
        
        foreach (var city in cities)
        {
            if (!CityEvaluator.IsFrontlineCity(city))
            {
                result.Add(city.cityId);
            }
        }
        return result;
    }
    
    public static void DispatchHeroes(SaveForceData force)
    {
        var frontlineCities = GetFrontlineCities(force);
        var rearCities = GetRearCities(force);
        
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
            
            int neededCombat = SystemConst.AIStrategy.FRONTLINE_COMBAT_HEROES_TARGET - combatCount;
            
            for (int i = 0; i < neededCombat && rearCombatHeroes.Count > 0; i++)
            {
                var heroToMove = rearCombatHeroes[0];
                rearCombatHeroes.RemoveAt(0);

                int srcCityId = heroToMove.cityId;
                var srcCity = GameManager.Instance.GetCity(srcCityId);

                if (rearCityHeroMap.ContainsKey(srcCityId) && 
                    rearCityHeroMap[srcCityId].Count > MIN_REAR_HEROES)
                {
                    force.MoveHeroToCity(srcCityId, cityId, new int[] { heroToMove.heroId });
                    rearCityHeroMap[srcCityId].Remove(heroToMove);

                    GameLog.SetTag("AI").Info($"AI调度: 英雄{heroToMove.heroId}从后方城市{srcCityId}调往前线城市{cityId}");
                }
            }
        }
    }
}
