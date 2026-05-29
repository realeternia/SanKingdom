using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using System;

public static class AIToolHeroDev
{
    public static void AssignHeroesToDev(SaveForceData force)
    {
        var context = new AIStrategyContext(force);
        foreach (var city in context.cities)
        {
            AssignHeroesToCityDev(force, city);
        }
    }

    internal static void AssignHeroesToCityDev(SaveForceData force, SaveCityData city)
    {
        var levelCfg = CityLevelConfig.GetConfig(city.GetLevel());
        
        int maxJobCount = levelCfg.JobCount;
        
        var normalHeroes = city.GetNormalHeroList();
        if (normalHeroes.Count == 0) return;
        
        var normalHeroIdSet = new HashSet<int>(normalHeroes);
        var currentAssignments = city.GetDevAssignments();
        var staleHeroIds = new List<int>();
        foreach (var assignment in currentAssignments)
        {
            if (!normalHeroIdSet.Contains(assignment.heroId))
            {
                staleHeroIds.Add(assignment.heroId);
            }
        }
        foreach (var heroId in staleHeroIds)
        {
            city.RemoveDevAssignment(heroId);
            GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(city.cityId)}] 清理无效委派 heroId={heroId}");
        }
        
        currentAssignments = city.GetDevAssignments();
        var assignedHeroIds = new HashSet<int>(currentAssignments.Select(a => a.heroId));
        var devAssignmentCounts = new Dictionary<int, int>();
        foreach (var assignment in currentAssignments)
        {
            if (!devAssignmentCounts.ContainsKey(assignment.devId))
                devAssignmentCounts[assignment.devId] = 0;
            devAssignmentCounts[assignment.devId]++;
        }
        
        var devConfigs = CityDevConfig.ConfigList
            .Where(c => c.Type == "normal" && c.AiPriotyDev > 0 && SaveCityData.IsDevAvailableForCity(city.cityId, c))
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
}
