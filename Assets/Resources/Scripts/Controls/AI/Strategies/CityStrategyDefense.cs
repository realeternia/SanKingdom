using CommonConfig;
using System.Linq;
using System.Collections.Generic;

public class CityStrategyDefense : CityStrategyBase
{
    public CityStrategyDefense(AIStrategyContext context, SaveCityData city, SaveForceData force) 
        : base(CityStrategyState.Def, context, city, force)
    {
    }

    public override void Execute()
    {
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 执行防御策略");
        
        AssignResProduction();
        AssignHeroesToDev();
        FormTroops();
    }
    
    protected override List<CityDevConfig> GetSortedDevConfigs()
    {
        return CityDevConfig.ConfigList
            .Where(c => c.Type == "normal" && c.AiWeightDef > 0 && SaveCityData.IsDevAvailableForCity(City.cityId, c))
            .ToList();
    }
}
