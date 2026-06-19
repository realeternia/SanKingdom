using System;
using System.Linq;
using System.Collections.Generic;
using CommonConfig;

public class CityStrategyAttack : CityStrategyBase
{
    private int _targetCityId;

    public CityStrategyAttack(AIStrategyContext context, SaveCityData city, SaveForceData force, int targetCityId) 
        : base(CityStrategyState.Atk, context, city, force)
    {
        _targetCityId = targetCityId;
    }

    public override void Execute()
    {
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 执行进攻策略，目标: {ConfigNameHelper.GetCityName(_targetCityId)}");
        
        AssignResProduction();
        AssignHeroesToDev();
        FormTroops();
        CreateWarPlan();
    }
    
    protected override List<CityDevConfig> GetSortedDevConfigs()
    {
        return CityDevConfig.ConfigList
            .Where(c => c.Type == "normal" && c.AiPriotyAtk > 0 && SaveCityData.IsDevAvailableForCity(City.cityId, c))
            .OrderByDescending(c => c.AiPriotyAtk)
            .ToList();
    }

    private void CreateWarPlan()
    {
        var normalHeroes = City.GetNormalHeroList();
        if (normalHeroes.Count == 0)
            return;

        var combatHeroes = normalHeroes
            .Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null && HeroDispatcher.ClassifyHero(h) == HeroType.Combat)
            .ToList();

        if (combatHeroes.Count == 0)
        {
            combatHeroes = normalHeroes
                .Select(id => GameManager.Instance.GetHero(id))
                .Where(h => h != null)
                .Take(3)
                .ToList();
        }

        if (combatHeroes.Count == 0)
            return;

        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        var heroSoldierDict = City.DistributeSoldierDefault(heroIds);

        int totalSoldier = heroSoldierDict.Values.Sum();

        if (totalSoldier < AIConst.AIStrategy.AI_MIN_ATTACK_SOLDIER)
            return;

        int foodNeeded = SysFormula.AIStrategy.CalculateFoodNeeded(totalSoldier);

        if (City.food < foodNeeded)
            return;

        var warPlan = new WarPlanData
        {
            sourceCityId = City.cityId,
            targetCityId = _targetCityId,
            heroIds = heroIds,
            heroSoldierDict = heroSoldierDict
        };

        Force.AddWarPlan(warPlan);

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 计划进攻[{ConfigNameHelper.GetCityName(_targetCityId)}] 英雄:[{ConfigNameHelper.GetHeroNames(heroIds)}] 兵力:{totalSoldier}");
    }
}
