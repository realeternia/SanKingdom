using System.Collections.Generic;
using CommonConfig;

public class AIStrategyContext
{
    public SaveForceData force;
    public List<SaveCityData> cities;
    public Dictionary<int, List<SaveHeroData>> cityHeroes;
    
    public AIStrategyContext(SaveForceData force)
    {
        this.force = force;
        this.cities = force.GetCityList();
        this.cityHeroes = new Dictionary<int, List<SaveHeroData>>();
        
        foreach (var city in cities)
        {
            cityHeroes[city.cityId] = new List<SaveHeroData>();
            var heroIds = city.GetNormalHeroList();
            foreach (var heroId in heroIds)
            {
                cityHeroes[city.cityId].Add(GameManager.Instance.GetHero(heroId));
            }
        }
    }
   
    
    public List<SaveHeroData> GetAvailableHeroes(int cityId)
    {
        var result = new List<SaveHeroData>();
        if (cityHeroes.ContainsKey(cityId))
        {
            foreach (var hero in cityHeroes[cityId])
            {
                result.Add(hero);
            }
        }
        return result;
    }
}
