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

        AdjustForGoldBalance(force);
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
            .Where(c => c.Type == "normal" && c.AiWeightDev > 0 && SaveCityData.IsDevAvailableForCity(city.cityId, c))
            .ToList();
        
        if (devConfigs.Count == 0) return;
        
        int newAssignedCount = 0;
        
        while (assignedHeroIds.Count < maxJobCount)
        {
            var available = devConfigs
                .Where(c => (devAssignmentCounts.ContainsKey(c.Id) ? devAssignmentCounts[c.Id] : 0) < GetEffectiveHeroCount(c))
                .ToList();
            if (available.Count == 0) break;
            
            var devCfg = WeightedRandomPickDev(available);
            if (devCfg == null) break;
            
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
                float currentVal = city.GetAttr(devCfg.DevAttr1);
                int maxVal = attrConfig.ValMaxCity;
                float deficit = maxVal - currentVal;
                if (deficit > 0)
                {
                    score += deficit / 10f;
                }
            }
            else if (!attrConfig.IsPosRes)
            {
                float currentVal = force.GetAttr(devCfg.DevAttr1);
                int maxVal = attrConfig.ValMaxForce;
                float deficit = maxVal - currentVal;
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

    public static void AdjustForGoldBalance(SaveForceData force)
    {
        var cities = force.GetCityList();

        while (force.GetPredictedGoldBalance() < 0)
        {
            SaveCityData worstCity = null;
            int worstHeroId = 0;
            float worstEffect = float.MaxValue;
            bool foundAny = false;

            foreach (var city in cities)
            {
                var assignments = city.GetDevAssignments();
                foreach (var assignment in assignments)
                {
                    var devCfg = CityDevConfig.GetConfig(assignment.devId);
                    var heroData = GameManager.Instance.GetHero(assignment.heroId);
                    if (heroData == null) continue;
                    float effect = GetAssignmentGoldEffect(devCfg, heroData);
                    if (effect < worstEffect)
                    {
                        worstEffect = effect;
                        worstCity = city;
                        worstHeroId = assignment.heroId;
                        foundAny = true;
                    }
                }
            }

            if (!foundAny)
            {
                GameLog.SetTag("AI").Warn($"{ConfigNameHelper.GetForceName(force.forceId)} 无法通过调整委派避免金钱为负");
                break;
            }

            worstCity.RemoveDevAssignment(worstHeroId);
            GameLog.SetTag("AI").Warn($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(worstCity.cityId)}] 移除委派 {ConfigNameHelper.GetHeroName(worstHeroId)}，防止金钱预测为负");
        }
    }

    private static float GetAssignmentGoldEffect(CityDevConfig devCfg, SaveHeroData heroData)
    {
        float effect = 0;
        float avgWeightedValue = SysFormula.City.GetHeroWeightedAttrValue(heroData, devCfg.Attrs);
        int tier = SysFormula.City.GetHeroTier(avgWeightedValue);

        if (!string.IsNullOrEmpty(devCfg.DevAttr1) && devCfg.DevAttr1.ToLower() == "gold")
        {
            var attrConfig = CityAttrConfig.GetConfigByname("gold");
            if (attrConfig.IsForceAttr && devCfg.DevAttr1Value != null && devCfg.DevAttr1Value.Length > tier)
                effect += devCfg.DevAttr1Value[tier];
        }

        if (!string.IsNullOrEmpty(devCfg.DevAttr2) && devCfg.DevAttr2.ToLower() == "gold"
            && devCfg.DevAttr2Value != null && devCfg.DevAttr2Value.Length > tier)
        {
            var attrConfig = CityAttrConfig.GetConfigByname("gold");
            if (attrConfig.IsForceAttr)
                effect += devCfg.DevAttr2Value[tier];
        }

        effect -= devCfg.GoldCost;

        return effect;
    }

    /// <summary>
    /// 按 AiWeightDev 加权随机选择一个发展配置
    /// </summary>
    public static CityDevConfig WeightedRandomPickDev(List<CityDevConfig> candidates)
    {
        if (candidates == null || candidates.Count == 0)
            return null;
        
        float totalWeight = 0f;
        foreach (var c in candidates)
            totalWeight += c.AiWeightDev;
        
        if (totalWeight <= 0f)
            return null;
        
        float roll = (float)SysRandom.Range(0, (int)(totalWeight * 1000)) / 1000f;
        float cumulative = 0f;
        foreach (var c in candidates)
        {
            cumulative += c.AiWeightDev;
            if (roll < cumulative)
                return c;
        }
        return candidates[candidates.Count - 1];
    }

    private static int GetEffectiveHeroCount(CityDevConfig cfg)
    {
        return cfg.HeroCount > 0 ? cfg.HeroCount : 1;
    }
}
