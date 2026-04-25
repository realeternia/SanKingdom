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
        
        foreach (var heroId in normalHeroes)
        {
            if (assignedHeroIds.Count >= maxJobCount) break;
            
            if (assignedHeroIds.Contains(heroId)) continue;
            
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero == null) continue;
            
            CityDevConfig bestDev = null;
            int bestScore = -1;
            
            foreach (var devCfg in devConfigs)
            {
                int currentCount = devAssignmentCounts.ContainsKey(devCfg.Id) ? devAssignmentCounts[devCfg.Id] : 0;
                if (currentCount >= devCfg.HeroCount) continue;
                
                int score = CalculateDevScore(hero, devCfg, city, force);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestDev = devCfg;
                }
            }
            
            if (bestDev != null)
            {
                city.SetDevAssignment(heroId, bestDev.Id);
                assignedHeroIds.Add(heroId);
                newAssignedCount++;
                
                if (!devAssignmentCounts.ContainsKey(bestDev.Id))
                    devAssignmentCounts[bestDev.Id] = 0;
                devAssignmentCounts[bestDev.Id]++;
                
                GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 派遣 {ConfigNameHelper.GetHeroName(heroId)} 进行 {bestDev.Cname}");
            }
        }
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 派遣完成，共 {assignedHeroIds.Count}/{maxJobCount} 人(新增{newAssignedCount})");
    }
    
    private static int CalculateDevScore(SaveHeroData hero, CityDevConfig devCfg, SaveCityData city, SaveForceData force)
    {
        int score = devCfg.AiPriotyDev * 10;
        
        if (devCfg.Attrs != null && devCfg.Attrs.Length > 0)
        {
            int attrScore = 0;
            foreach (var attr in devCfg.Attrs)
            {
                attrScore += hero.GetAttr(attr);
            }
            score += attrScore / devCfg.Attrs.Length;
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
                    score += deficit / 10;
                }
                else
                {
                    score -= 50;
                }
            }
            else if (!attrConfig.IsPosRes)
            {
                int currentVal = force.GetAttr(devCfg.DevAttr1);
                int maxVal = attrConfig.ValMaxForce;
                int deficit = maxVal - currentVal;
                if (deficit > 0)
                {
                    score += deficit / 10;
                }
                else
                {
                    score -= 50;
                }
            }
        }
        
        return score;
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
