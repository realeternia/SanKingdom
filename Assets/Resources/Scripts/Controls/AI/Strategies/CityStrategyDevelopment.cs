using CommonConfig;
using System.Collections.Generic;
using System.Linq;

public class CityStrategyDevelopment : CityStrategyBase
{
    public CityStrategyDevelopment(AIStrategyContext context, SaveCityData city, SaveForceData force) 
        : base(CityStrategyState.Dev, context, city, force)
    {
    }

    public override void Execute()
    {
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 执行发展策略");
        
        AssignHeroesToDev();
        FormTroops();
        AssignAdvancedArms();
    }
    
    protected override List<CityDevConfig> GetSortedDevConfigs()
    {
        return CityDevConfig.ConfigList
            .Where(c => c.Type == "normal" && c.AiPriotyDev > 0 && SaveCityData.IsDevAvailableForCity(City.cityId, c))
            .OrderByDescending(c => c.AiPriotyDev)
            .ToList();
    }
}
