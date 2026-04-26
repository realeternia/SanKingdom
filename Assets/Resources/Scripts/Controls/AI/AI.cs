using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;

public static class AI
{
    public static void ExecutePlanningPhase(SaveForceData force)
    {
        StrategicDecider.ClearRoundData();
        
        var context = new AIStrategyContext(force);
        
        HeroDispatcher.DispatchHeroes(force);
        
        var cityStrategies = StrategicDecider.DetermineCityStrategies(force);
        
        AssignHeroesToDev(force, context);
        
        GenerateWarPlans(force, context, cityStrategies);
        
        GameManager.Instance.ConfirmPlan(force.forceId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 计划阶段完成");
    }
    
    public static void AssignHeroesToDev(SaveForceData force)
    {
        var context = new AIStrategyContext(force);
        foreach (var city in context.cities)
        {
            AssignHeroesToCityDev(force, city);
        }
    }
    
    private static void AssignHeroesToDev(SaveForceData force, AIStrategyContext context)
    {
        foreach (var city in context.cities)
        {
            AssignHeroesToCityDev(force, city);
        }
    }
    
    private static void AssignHeroesToCityDev(SaveForceData force, SaveCityData city)
    {
        var levelCfg = CityLevelConfig.GetConfig(city.level);
        
        int maxJobCount = levelCfg.JobCount;
        
        var normalHeroes = city.GetNormalHeroList();
        if (normalHeroes.Count == 0) return;
        
        var currentAssignments = city.GetDevAssignments();
        var assignedHeroIds = new HashSet<int>(currentAssignments.Select(a => a.heroId));
        var devAssignmentCounts = new Dictionary<int, int>();
        foreach (var assignment in currentAssignments)
        {
            if (!devAssignmentCounts.ContainsKey(assignment.devId))
                devAssignmentCounts[assignment.devId] = 0;
            devAssignmentCounts[assignment.devId]++;
        }
        
        var devConfigs = CityDevConfig.ConfigList
            .Where(c => c.Prefab == "CityDevNormal" && c.AiPriotyDev > 0)
            .OrderByDescending(c => c.AiPriotyDev)
            .ToList();
        
        if (devConfigs.Count == 0) return;
        
        int newAssignedCount = 0;
        
        foreach (var devCfg in devConfigs)
        {
            while (assignedHeroIds.Count < maxJobCount)
            {
                int currentCount = devAssignmentCounts.ContainsKey(devCfg.Id) ? devAssignmentCounts[devCfg.Id] : 0;
                if (currentCount >= devCfg.HeroCount) break;
                
                var bestHero = FindBestHeroForDev(normalHeroes, assignedHeroIds, devCfg, city, force);
                if (bestHero == null) break;
                
                city.SetDevAssignment(bestHero.heroId, devCfg.Id);
                assignedHeroIds.Add(bestHero.heroId);
                newAssignedCount++;
                
                if (!devAssignmentCounts.ContainsKey(devCfg.Id))
                    devAssignmentCounts[devCfg.Id] = 0;
                devAssignmentCounts[devCfg.Id]++;
                
                GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 派遣 {ConfigNameHelper.GetHeroName(bestHero.heroId)} 进行 {devCfg.Cname}");
            }
        }
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 派遣完成，共 {assignedHeroIds.Count}/{maxJobCount} 人(新增{newAssignedCount})");
    }
    
    private static SaveHeroData FindBestHeroForDev(List<int> normalHeroes, HashSet<int> assignedHeroIds, CityDevConfig devCfg, SaveCityData city, SaveForceData force)
    {
        SaveHeroData bestHero = null;
        float bestScore = -1;
        
        foreach (var heroId in normalHeroes)
        {
            if (assignedHeroIds.Contains(heroId)) continue;
            
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero == null) continue;
            
            float score = CalculateHeroDevScore(hero, devCfg, city, force);
            if (score > bestScore)
            {
                bestScore = score;
                bestHero = hero;
            }
        }
        
        return bestHero;
    }
    
    private static float CalculateHeroDevScore(SaveHeroData hero, CityDevConfig devCfg, SaveCityData city, SaveForceData force)
    {
        float score = 0;
        
        if (devCfg.Attrs != null && devCfg.Attrs.Length > 0)
        {
            score = GetWeightedAttrValue(hero, devCfg.Attrs);
        }
        
        if (!string.IsNullOrEmpty(devCfg.DevAttr1))
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr1.ToLower());
            if (!attrConfig.IsForceAttr)
            {
                int currentVal = city.GetAttr(devCfg.DevAttr1);
                int maxVal = attrConfig.ValMaxCity;
                int deficit = maxVal - currentVal;
                if (deficit > 0)
                {
                    score += deficit / 10f;
                }
            }
            else if (!attrConfig.IsPosRes)
            {
                int currentVal = force.GetAttr(devCfg.DevAttr1);
                int maxVal = attrConfig.ValMaxForce;
                int deficit = maxVal - currentVal;
                if (deficit > 0)
                {
                    score += deficit / 10f;
                }
            }
        }
        
        return score;
    }
    
    private static float GetWeightedAttrValue(SaveHeroData hero, string[] attrs)
    {
        if (attrs.Length == 1)
        {
            return hero.GetAttr(attrs[0]);
        }
        else
        {
            float firstAttr = hero.GetAttr(attrs[0]);
            float secondAttr = hero.GetAttr(attrs[1]);
            return firstAttr * (2f / 3f) + secondAttr * (1f / 3f);
        }
    }
    
    private static void GenerateWarPlans(SaveForceData force, AIStrategyContext context, Dictionary<int, CityStrategyState> cityStrategies)
    {
        foreach (var city in context.cities)
        {
            var state = cityStrategies.ContainsKey(city.cityId) ? 
                cityStrategies[city.cityId] : CityStrategyState.Dev;
            
            if (state == CityStrategyState.Atk)
            {
                var attackTarget = StrategicDecider.GetAttackTarget(city.cityId);
                if (attackTarget.HasValue)
                {
                    TryCreateWarPlan(force, city, attackTarget.Value);
                }
            }
        }
    }
    
    private static void TryCreateWarPlan(SaveForceData force, SaveCityData city, int targetCityId)
    {
        var normalHeroes = city.GetNormalHeroList();
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
        var heroSoldierDict = city.DistributeSoldierDefault(heroIds);
        
        int totalSoldier = heroSoldierDict.Values.Sum();
        
        if (totalSoldier < SystemConst.AIStrategy.AI_MIN_ATTACK_SOLDIER)
            return;
        
        int foodNeeded = SysFormula.AIStrategy.CalculateFoodNeeded(totalSoldier);
        
        if (city.food < foodNeeded)
            return;
        
        var warPlan = new WarPlanData
        {
            forceId = force.forceId,
            sourceCityId = city.cityId,
            targetCityId = targetCityId,
            heroIds = heroIds,
            foodCost = foodNeeded,
            heroSoldierDict = heroSoldierDict,
            heroArmsDict = new Dictionary<int, int>()
        };
        
        force.AddWarPlan(warPlan);
        
        StrategicDecider.MarkTargetAttacked(force.forceId, targetCityId);
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 计划攻击[{ConfigNameHelper.GetCityName(targetCityId)}] 英雄:[{ConfigNameHelper.GetHeroNames(heroIds)}] 兵力:{totalSoldier}");
    }
}
