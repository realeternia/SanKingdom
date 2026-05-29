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
        
        AssignHeroesToDev();
        FormTroops();
        AssignAdvancedArms();
    }
    
    protected override List<CityDevConfig> GetSortedDevConfigs()
    {
        return CityDevConfig.ConfigList
            .Where(c => c.Type == "normal" && (c.AiPriotyDef > 0 || c.DevAttr1 == "wall" || c.DevAttr1 == "soldier" || c.DevAttr1 == "horse" || c.DevAttr1 == "steel") && SaveCityData.IsDevAvailableForCity(City.cityId, c))
            .OrderByDescending(c => 
            {
                // 防御策略下优先级：wall > soldier > horse/steel > 其他
                if (c.DevAttr1 == "wall") return 1000;
                if (c.DevAttr1 == "soldier") return 900;
                if (c.DevAttr1 == "horse" || c.DevAttr1 == "steel") return 800;
                return c.AiPriotyDef;
            })
            .ToList();
    }
}
