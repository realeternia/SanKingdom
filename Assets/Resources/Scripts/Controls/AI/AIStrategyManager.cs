using System.Collections.Generic;

public enum CityStrategyState
{
    Dev,
    Def,
    Atk
}

public class AIStrategyManager
{
    private static AIStrategyManager _instance;
    public static AIStrategyManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new AIStrategyManager();
            return _instance;
        }
    }
    
    private Dictionary<CityStrategyState, IAIStrategy> strategies;
    
    public AIStrategyManager()
    {
        strategies = new Dictionary<CityStrategyState, IAIStrategy>
        {
            { CityStrategyState.Dev, new DevelopmentStrategy() },
            { CityStrategyState.Def, new DefenseStrategy() },
            { CityStrategyState.Atk, new ExpansionStrategy() }
        };
    }
    
    public IAIStrategy GetStrategy(CityStrategyState state)
    {
        if (strategies.ContainsKey(state))
            return strategies[state];
        return strategies[CityStrategyState.Dev];
    }
    
    public Dictionary<int, CityStrategyState> DetermineCityStrategies(Player player)
    {
        var result = new Dictionary<int, CityStrategyState>();
        var cities = player.GetCityList();
        
        int atkDefCount = 0;
        int maxAtkDef = 2;
        
        foreach (var city in cities)
        {
            result[city.cityId] = CityStrategyState.Dev;
        }
        
        return result;
    }
}
