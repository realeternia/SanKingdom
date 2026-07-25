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
            .Where(c => c.Type == "normal" && c.AiWeightAtk > 0 && SaveCityData.IsDevAvailableForCity(City.cityId, c))
            .ToList();
    }

    private void CreateWarPlan()
    {
        var normalHeroes = City.GetNormalHeroList();
        if (normalHeroes.Count == 0)
            return;

        var combatHeroes = normalHeroes
            .Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null && SysFormula.Hero.ClassifyHero(h) == HeroType.Combat)
            .ToList();

        if (combatHeroes.Count == 0)
        {
            combatHeroes = normalHeroes
                .Select(id => GameManager.Instance.GetHero(id))
                .Where(h => h != null)
                .Take(AIConst.AIStrategy.MIN_ATTACK_TROOPS)
                .ToList();
        }

        if (combatHeroes.Count == 0)
            return;

        // 补足至少MIN_ATTACK_TROOPS只部队
        if (combatHeroes.Count < AIConst.AIStrategy.MIN_ATTACK_TROOPS)
        {
            var remaining = normalHeroes
                .Select(id => GameManager.Instance.GetHero(id))
                .Where(h => h != null && !combatHeroes.Any(c => c.heroId == h.heroId))
                .ToList();

            while (combatHeroes.Count < AIConst.AIStrategy.MIN_ATTACK_TROOPS && remaining.Count > 0)
            {
                combatHeroes.Add(remaining[0]);
                remaining.RemoveAt(0);
            }
        }

        // 从已编军团获取兵种，非动员兵优先
        var cityTroops = SaveTroopsData.GetTroopsByCity(City.cityId);
        var heroArmsDict = new Dictionary<int, int>();
        foreach (var hero in combatHeroes)
        {
            var troop = cityTroops.FirstOrDefault(t => t.heroId1 == hero.heroId);
            if (troop != null)
                heroArmsDict[hero.heroId] = troop.armsId;
        }

        // 排序：非动员兵优先，其次统帅降序
        combatHeroes.Sort((a, b) =>
        {
            int armsA = heroArmsDict.ContainsKey(a.heroId) ? heroArmsDict[a.heroId] : SystemConst.Hero.DEFAULT_ARMS_ID;
            int armsB = heroArmsDict.ContainsKey(b.heroId) ? heroArmsDict[b.heroId] : SystemConst.Hero.DEFAULT_ARMS_ID;
            bool isMilitiaA = armsA == SystemConst.Hero.DEFAULT_ARMS_ID;
            bool isMilitiaB = armsB == SystemConst.Hero.DEFAULT_ARMS_ID;
            if (isMilitiaA != isMilitiaB) return isMilitiaA ? 1 : -1;
            return b.leadShip.CompareTo(a.leadShip);
        });

        var heroIds = combatHeroes.Select(h => h.heroId).ToArray();
        var heroSoldierDict = City.DistributeSoldierDefault(heroIds);

        int totalSoldier = heroSoldierDict.Values.Sum();

        if (totalSoldier < AIConst.AIStrategy.AI_MIN_ATTACK_SOLDIER)
            return;

        int foodNeeded = AIFormula.CalculateFoodNeeded(totalSoldier);

        if (City.food < foodNeeded)
            return;

        var warPlan = new WarPlanData
        {
            sourceCityId = City.cityId,
            targetCityId = _targetCityId,
            heroIds = heroIds,
            heroSoldierDict = heroSoldierDict,
            heroArmsDict = heroArmsDict
        };

        Force.AddWarPlan(warPlan);

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 计划进攻[{ConfigNameHelper.GetCityName(_targetCityId)}] 英雄:[{ConfigNameHelper.GetHeroNames(heroIds)}] 兵力:{totalSoldier}");
    }
}
