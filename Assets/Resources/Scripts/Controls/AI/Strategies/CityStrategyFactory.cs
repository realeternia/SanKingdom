using CommonConfig;

public static class CityStrategyFactory
{
    public static CityStrategyBase CreateStrategy(CityStrategyState state, AIStrategyContext context, SaveCityData city, SaveForceData force, int? targetCityId = null)
    {
        switch (state)
        {
            case CityStrategyState.Dev:
                return new CityStrategyDevelopment(context, city, force);
            case CityStrategyState.Def:
                return new CityStrategyDefense(context, city, force);
            case CityStrategyState.Atk:
                if (!targetCityId.HasValue)
                {
                    throw new System.ArgumentException("CityStrategyAttack requires targetCityId");
                }
                return new CityStrategyAttack(context, city, force, targetCityId.Value);
            default:
                return new CityStrategyDevelopment(context, city, force);
        }
    }
}
