using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonConfig;
using System.Linq;
[System.Serializable]
public class SaveCityData
{
    public int cityId;
    public int forceId;
    public int exp;
    public float soldier;
    public float happy;
    public float food;
    public float wall;
    public int battleTime;
    public List<DevAssignmentData> devAssignments = new List<DevAssignmentData>();    

    public int ownerHeroId;
    [NonSerialized]
    public Dictionary<int, int> actions = new Dictionary<int, int>();

    public bool IsInWar
    {
        get { return battleTime > 0; }
    }

    public int GetLevel()
    {
        int level = 1;
        for (int i = 1; i <= 20; i++)
        {
            if (!CityLevelConfig.HasConfig(i)) break;
            if (exp >= CityLevelConfig.GetConfig(i).ExpNeed)
                level = i + 1;
            else
                break;
        }
        return level;
    }

    public static int GetExpByLevel(int level)
    {
        if (level <= 1) return 0;
        if (CityLevelConfig.HasConfig(level - 1))
            return CityLevelConfig.GetConfig(level - 1).ExpNeed;
        return 0;
    }

    public void SetDevAssignment(int heroId, int devId)
    {
        var existing = devAssignments.FirstOrDefault(d => d.heroId == heroId);
        if (existing != null)
        {
            existing.devId = devId;
        }
        else
        {
            devAssignments.Add(new DevAssignmentData(heroId, devId));
        }
        GameManager.Instance.GetForce(forceId).RecalculatePosRes();        

    }

    public void RemoveDevAssignment(int heroId)
    {
        devAssignments.RemoveAll(d => d.heroId == heroId);
        GameManager.Instance.GetForce(forceId).RecalculatePosRes();        
    }

    public void ClearDevAssignments()
    {
        devAssignments.Clear();
        GameManager.Instance.GetForce(forceId).RecalculatePosRes();        
    }

    public List<DevAssignmentData> GetDevAssignments()
    {
        return devAssignments;
    }

    public int? GetDevIdByHeroId(int heroId)
    {
        var assignment = devAssignments.FirstOrDefault(d => d.heroId == heroId);
        return assignment?.devId;
    }

    public void OnRound()
    {
        battleTime = Math.Max(0, battleTime - 1);
        actions.Clear();
    }

    public void AddAction(int devId, int count)
    {
        if(actions.ContainsKey(devId))
            actions[devId] += count;
        else
            actions.Add(devId, count);
    }

    
    public List<int> GetHeroList(bool showNormal, bool showWild)
    {
        var heroIds = new List<int>();
        foreach (var member in GameManager.Instance.SaveData.heros)
        {
            if(member.cityId == cityId && 
               ((showNormal && member.state == HeroState.Normal) || 
                (showWild && member.state == HeroState.Wild)))
                heroIds.Add(member.heroId);
        }
        return heroIds;
    }

    public List<int> GetRecruitableHeroList()
    {
        var heroIds = new List<int>();
        
        foreach (var member in GameManager.Instance.SaveData.heros)
        {
            if(member.cityId == cityId)
            {
                if(member.state == HeroState.Wild)
                    heroIds.Add(member.heroId);
                else if(member.state == HeroState.Catched)
                    heroIds.Add(member.heroId);
            }
            else if(member.state == HeroState.Normal && member.forceId != forceId && member.loyalty < SystemConst.Hero.RECRUIT_ENEMY_LOYALTY_THRESHOLD)
            {
                if(MapTool.IsAdjacentCity(cityId, member.cityId))
                    heroIds.Add(member.heroId);
            }
        }
        return heroIds;
    }

    public List<int> GetNormalHeroList()
    {
        var heroIds = new List<int>();
        foreach (var member in GameManager.Instance.SaveData.heros)
        {
            if(member.cityId == cityId && member.state == HeroState.Normal && member.forceId == forceId)
                heroIds.Add(member.heroId);
        }
        return heroIds;
    }

    public List<int> GetCatchedHeroList()
    {
        var heroIds = new List<int>();
        foreach (var member in GameManager.Instance.SaveData.heros)
        {
            if(member.cityId == cityId && member.state == HeroState.Catched)
                heroIds.Add(member.heroId);
        }
        return heroIds;
    }

    public Dictionary<int, int> DistributeSoldierDefault(int[] heroIds, int maxPerHero = SystemConst.Hero.MAX_SOLDIER_PER_HERO)
    {
        var result = new Dictionary<int, int>();
        int totalSoldiers = (int)Math.Floor(soldier);

        var heroList = heroIds.Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null)
            .OrderByDescending(h => h.GetAttr("leadship"))
            .ToList();

        if (heroList.Count == 0) return result;

        // 按统帅权重比例分配，强者多得
        float totalWeight = heroList.Sum(h => (float)h.GetAttr("leadship"));

        int distributed = 0;
        foreach (var hero in heroList)
        {
            float weight = hero.GetAttr("leadship");
            int share = (int)(totalSoldiers * weight / totalWeight);
            share = Math.Min(share, maxPerHero);
            result[hero.heroId] = share;
            distributed += share;
        }

        // 取整余数按统帅从高到低补足
        int remaining = totalSoldiers - distributed;
        for (int i = 0; i < heroList.Count && remaining > 0; i++)
        {
            int heroId = heroList[i].heroId;
            int space = maxPerHero - result[heroId];
            if (space > 0)
            {
                int add = Math.Min(space, remaining);
                result[heroId] += add;
                distributed += add;
                remaining -= add;
            }
        }

        AddAttr("soldier", -distributed, "分配士兵扣除");
        
        PanelManager.Instance.SendSignal(new CityResChangeSignal { CityId = cityId, ResType = "soldier", Value = GetAttr("soldier") });
        
        return result;
    }

    public SaveForceData GetForce()
    {
        return GameManager.Instance.GetForce(forceId);
    }

    public int GetOwner()
    {
        return ownerHeroId;
    }

    public void AddAttr(string type, float add, string reason = "")
    {
        var attrConfig = CityAttrConfig.GetConfigByname(type.ToLower());
        if (attrConfig.IsForceAttr)
        {
            GameLog.Error($"AddAttr: {type} is force attr, not city attr");
            return;
        }
        
        if (attrConfig.IsPosRes)
        {
            GameLog.Debug($"AddAttr: {type} is IsPosRes, skip auto add");
            return;
        }
        
        float oldValFloat = GetAttr(type.ToLower());
        switch (type.ToLower())
        {
            case "level":
                GameLog.Warn($"AddAttr: level由exp推导，请使用AddAttr(\"exp\", ...)");
                break;
            case "exp":
                int oldLevel = GetLevel();
                exp = Math.Max(0, exp + (int)add);
                int newLevel = GetLevel();
                GameLog.Info($"SaveCityData.AddAttr cityId={cityId} type={type} old={oldValFloat} add={add} new={exp} reason={reason}");
                if (PanelManager.Instance != null)
                {
                    PanelManager.Instance.SendSignal(new CityResChangeSignal { CityId = cityId, ResType = type.ToLower(), Value = GetAttr(type.ToLower()) });
                    if (oldLevel != newLevel)
                        PanelManager.Instance.SendSignal(new CityLevelChangeSignal { CityId = cityId });
                }
                return;
            case "soldier":
                soldier = Math.Max(0, Math.Min(soldier + add, attrConfig.ValMaxCity));
                break;
            case "happy":
                happy = Math.Max(0, Math.Min(happy + add, attrConfig.ValMaxCity));
                break;
            case "food":
                food = Math.Max(0, Math.Min(food + add, attrConfig.ValMaxCity));
                break;
            case "wall":
                wall = Math.Max(0, Math.Min(wall + add, attrConfig.ValMaxCity));
                break;
            default:
                break;
        }

        float newValFloat = GetAttr(type.ToLower());
        GameLog.Info($"SaveCityData.AddAttr cityId={cityId} type={type} old={oldValFloat} add={add} new={newValFloat} reason={reason}");

        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.SendSignal(new CityResChangeSignal { CityId = cityId, ResType = type.ToLower(), Value = newValFloat });
        }
    }

    public void MultiplyAttr(string type, float multiplier)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(type.ToLower());
        if (attrConfig.IsForceAttr)
        {
            GameLog.Error($"MultiplyAttr: {type} is force attr, not city attr");
            return;
        }
        
        if (attrConfig.IsPosRes)
        {
            GameLog.Debug($"MultiplyAttr: {type} is IsPosRes, skip");
            return;
        }
        
        switch (type.ToLower())
        {
            case "level":
            case "exp":
                GameLog.Warn($"MultiplyAttr: {type} 不支持乘法操作");
                break;
            case "soldier":
                soldier = Math.Max(0, Math.Min(soldier * multiplier, attrConfig.ValMaxCity));
                break;
            case "happy":
                happy = Math.Max(0, Math.Min(happy * multiplier, attrConfig.ValMaxCity));
                break;
            case "food":
                food = Math.Max(0, Math.Min(food * multiplier, attrConfig.ValMaxCity));
                break;
            case "wall":
                wall = Math.Max(0, Math.Min(wall * multiplier, attrConfig.ValMaxCity));
                break;
            default:
                break;
        }

        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.SendSignal(new CityResChangeSignal { CityId = cityId, ResType = type.ToLower(), Value = GetAttr(type.ToLower()) });
        }
    }

    public float GetAttr(string type)
    {
        switch (type.ToLower())
        {
            case "level":
                return GetLevel();
            case "exp":
                return exp;
            case "soldier":
                return soldier;
            case "happy":
                return happy;
            case "food":
                return food;
            case "wall":
                return wall;
            default:
                return 0;
        }
    }

    public void OnBattle()
    {
        battleTime = Math.Min(battleTime + SystemConst.City.BATTLE_TIME_INCREMENT, SystemConst.City.BATTLE_TIME_MAX);
    }

    public float GetProductionMultiplier()
    {
        float happyMult = SysFormula.City.GetHappyMultiplier((int)Math.Floor(happy));
        if (IsInWar)
            return SystemConst.City.WAR_PRODUCTION_MULTIPLIER + happyMult - 1f;
        return happyMult;
    }

    public void MoveHeroTo(int[] heroIds, int destCityId)
    {
        var destCity = GameManager.Instance.GetCity(destCityId);
        foreach (var heroId in heroIds)
        {
            SaveHeroData hero = GameManager.Instance.GetHero(heroId);
            if (hero != null)
            {
                RemoveDevAssignment(heroId);
                SaveTroopsData.MoveHeroWithTroop(heroId, destCityId);
                hero.cityId = destCityId;
                if (destCity != null)
                    hero.forceId = destCity.forceId;
            }
        }
    }

    public int CalculateDistanceTo(int destCityId)
    {
        return MapTool.CalculateCityDistance(cityId, destCityId);
    }

    public void MoveTroopsFromSourceCities(List<int> heroIds)
    {
        var heroIdSet = new HashSet<int>(heroIds);
        var heroCityGroups = heroIds
            .Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null)
            .GroupBy(h => h.cityId)
            .Where(g => g.Key != cityId);

        foreach (var group in heroCityGroups)
        {
            var troopsToMove = SaveTroopsData.GetTroopsByCity(group.Key)
                .Where(t => heroIdSet.Contains(t.heroId1))
                .ToList();

            SaveTroopsData.MoveTroopsToCity(troopsToMove, cityId);

            var srcCity = GameManager.Instance.GetCity(group.Key);
            if (srcCity != null)
                srcCity.RecalculateHeros();
        }
    }

    public void Occupy(int forceWin, List<int> winHeroIds, int forceLose, List<int> failHeroIds)
    {
        forceId = forceWin;

        SaveTroopsData.RemoveAllTroopsByCity(cityId);
        ClearDevAssignments();

        MoveTroopsFromSourceCities(winHeroIds);

        var catchedHeroList = GetCatchedHeroList();
        foreach (var heroId in catchedHeroList)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero != null && hero.forceId == forceWin)
            {
                hero.state = HeroState.Normal;
                GameLog.Info($"Occupy 释放己方俘虏: heroId={heroId} forceId={hero.forceId}");
            }
        }

        var allDefenceHeroIds = new HashSet<int>(failHeroIds);
        foreach (var hero in GameManager.Instance.SaveData.heros)
        {
            if (hero.cityId == cityId && hero.forceId == forceLose && hero.state == HeroState.Normal)
            {
                allDefenceHeroIds.Add(hero.heroId);
            }
        }

        List<SaveCityData> loseForceCities = GameManager.Instance.GetCitiesByForce(forceLose);

        GameLog.Info($"Occupy cityId={cityId} winforceId: {forceWin} loseforceId: {forceLose} citycount: {loseForceCities.Count} defenceHeroes: {allDefenceHeroIds.Count}");
        if (loseForceCities.Count > 0)
        {
            var kingHeroId = ForceConfig.GetConfig(forceLose).HeroId;
            var destCityIds = new HashSet<int>();
            foreach (var heroId in allDefenceHeroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero != null)
                {
                    if (heroId == kingHeroId)
                    {
                        SaveTroopsData.RemoveHeroFromTroop(heroId);
                        hero.cityId = GameManager.Instance.GetRandomForceCityId(cityId, forceLose);
                        destCityIds.Add(hero.cityId);
                    }
                    else
                    {
                        int catchChance = SysFormula.Hero.CalculateCaptureChance(hero.str);
                        if (SysRandom.Range(0, 100) >= catchChance)
                        {
                            SaveTroopsData.RemoveHeroFromTroop(heroId);
                            hero.cityId = GameManager.Instance.GetRandomForceCityId(cityId, forceLose);
                            destCityIds.Add(hero.cityId);
                        }
                        else
                        {
                            SaveTroopsData.RemoveHeroFromTroop(heroId);
                            hero.state = HeroState.Catched;
                            BattleStatManager.RecordHeroCatched(hero.forceId, heroId);
                        }
                    }
                }
            }
            foreach (var cityId in destCityIds)
            {
                SaveCityData city = GameManager.Instance.GetCity(cityId);
                if (city != null)
                    city.RecalculateHeros();
            }
        }
        else
        {
            foreach (var heroId in allDefenceHeroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero != null)
                {
                    SaveTroopsData.RemoveHeroFromTroop(heroId);
                    hero.state = HeroState.Wild;
                    hero.forceId = SystemConst.Hero.WILD_FORCE_ID;
                    hero.loyalty = SystemConst.Hero.ELIMINATED_HERO_LOYALTY;
                }
            }
            var force = GameManager.Instance.GetForce(forceLose);
            if (force != null)
            {
                force.isEliminated = true;
            }
            GameLog.Info($"Occupy 势力 {forceLose} 已被消灭");
        }

        foreach (var heroId in winHeroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero != null)
            {
                SaveTroopsData.MoveHeroWithTroop(heroId, cityId);
                hero.cityId = cityId;
            }
        }

        RecalculateHeros();
        PanelManager.Instance.SendSignal(new CityForceChangeSignal { CityId = cityId });

        GameManager.Instance.SaveToFile();
    }

    public void RecalculateHeros()
    {
        ownerHeroId = 0;
        SelectOwner();

        PanelManager.Instance.SendSignal(new CityHeroChangeSignal { CityId = cityId });
    }

    public void SelectOwner()
    {
        var heroList = GetNormalHeroList();
        if (heroList.Count == 0)
            return;

        int maxScore = -1;
        SaveHeroData bestHero = null;

        var kingHeroId = ForceConfig.GetConfig(forceId).HeroId;

        foreach (var heroId in heroList)
        {
            SaveHeroData hero = GameManager.Instance.GetHero(heroId);
            if (hero == null)
                continue;

            int str = hero.GetAttr("str");
            int inte = hero.GetAttr("inte");
            int fair = hero.GetAttr("fair");
            int leadship = hero.GetAttr("leadship");
            int charm = hero.GetAttr("charm");

            float totalScore = SysFormula.City.CalculateOwnerScore(str, inte, fair, leadship, charm, heroId == kingHeroId);

            if (totalScore > maxScore)
            {
                maxScore = (int)totalScore;
                bestHero = hero;
            }
        }

        if (bestHero != null)
        {
            ownerHeroId = bestHero.heroId;
        }
    }

    public Dictionary<string, float> CalculateDevAttrAddons()
    {
        Dictionary<string, float> attrAddons = new Dictionary<string, float>();
        var forceData = GetForce();
        
        GameLog.Debug($"CalculateDevAttrAddons cityId={cityId} devAssignments={devAssignments.Count}");
        
        foreach (var assignment in devAssignments)
        {
            if (assignment.devId == SystemConst.CityDev.IDLE_DEV_ID)
                continue;

            var devCfg = CityDevConfig.GetConfig(assignment.devId);
            var heroData = GameManager.Instance.GetHero(assignment.heroId);
            GameLog.Debug($"CalculateDevAttrAddons devId={assignment.devId} DevAttr1={devCfg.DevAttr1} DevAttr2={devCfg.DevAttr2}");
            CalculateDevAttrAddonsForAssignment(devCfg, heroData, forceData, attrAddons);
        }
        
        foreach (var kvp in attrAddons)
        {
            GameLog.Debug($"CalculateDevAttrAddons result: {kvp.Key}={kvp.Value}");
        }
        
        return attrAddons;
    }

    private void CalculateDevAttrAddonsForAssignment(CityDevConfig devCfg, SaveHeroData heroData, SaveForceData forceData, Dictionary<string, float> attrAddons)
    {
        float avgWeightedValue = SysFormula.City.GetHeroWeightedAttrValue(heroData, devCfg.Attrs);
        int tier = SysFormula.City.GetHeroTier(avgWeightedValue);
        float multiplier = GetProductionMultiplier();

        if (!string.IsNullOrEmpty(devCfg.DevAttr1))
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr1.ToLower());
            if (!attrConfig.IsForceAttr)
            {
                GameLog.Debug($"CalculateDevAttrAddonsForAssignment DevAttr1={devCfg.DevAttr1} attrs={string.Join(",", devCfg.Attrs)} avgWeightedValue={avgWeightedValue} tier={tier}");
                float addon = CalculateDevAddonByTier(devCfg.DevAttr1, devCfg.DevAttr1Value[tier]);
                if (addon > 0)
                {
                    if (CityHasResAddon(cityId, devCfg.DevAttr1))
                        addon += SystemConst.City.RES_ADDON_BONUS;
                    addon = ApplyProductionMultiplierToAddon(devCfg.DevAttr1.ToLower(), addon, multiplier);
                    string attrName = devCfg.DevAttr1.ToLower();
                    if (!attrAddons.ContainsKey(attrName))
                        attrAddons[attrName] = 0;
                    attrAddons[attrName] += addon;
                }
            }
        }
        
        if (!string.IsNullOrEmpty(devCfg.DevAttr2) && devCfg.DevAttr2Value != null && devCfg.DevAttr2Value.Length >= 4)
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr2.ToLower());
            if (!attrConfig.IsForceAttr)
            {
                GameLog.Debug($"CalculateDevAttrAddonsForAssignment DevAttr2={devCfg.DevAttr2} attrs={string.Join(",", devCfg.Attrs)} avgWeightedValue={avgWeightedValue} tier={tier}");
                float addon = CalculateDevAddonByTier(devCfg.DevAttr2, devCfg.DevAttr2Value[tier]);
                if (addon > 0)
                {
                    if (CityHasResAddon(cityId, devCfg.DevAttr2))
                        addon += SystemConst.City.RES_ADDON_BONUS;
                    addon = ApplyProductionMultiplierToAddon(devCfg.DevAttr2.ToLower(), addon, multiplier);
                    string attrName = devCfg.DevAttr2.ToLower();
                    if (!attrAddons.ContainsKey(attrName))
                        attrAddons[attrName] = 0;
                    attrAddons[attrName] += addon;
                }
            }
        }
    }

    private static float ApplyProductionMultiplierToAddon(string attrName, float addon, float multiplier)
    {
        if (multiplier >= 0.999f && multiplier <= 1.001f)
            return addon;
        if (attrName == "food" || attrName == "soldier")
            return addon * multiplier;
        return addon;
    }

    private float CalculateDevAddonByTier(string attrName, float tierValue)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(attrName.ToLower());
        
        float currentVal = GetAttr(attrName);
        int valMax = attrConfig.ValMaxCity;
        
        float addon = tierValue;
        float remaining = valMax - currentVal;
        
        if (addon > remaining)
            addon = Math.Max(0, remaining);
        
        GameLog.Debug($"CalculateDevAddonByTier attrName={attrName} tierValue={tierValue} currentVal={currentVal} valMax={valMax} addon={addon}");
        return addon;
    }

    public static bool CityHasResAddon(int cityId, string attrName)
    {
        var worldCfg = WorldConfig.GetConfig(cityId);
        if (worldCfg.ResAddon == null) return false;
        var attrCfg = CityAttrConfig.GetConfigByname(attrName.ToLower());
        foreach (int addonId in worldCfg.ResAddon)
        {
            if (addonId == attrCfg.Id) return true;
        }
        return false;
    }

    public static bool IsDevAvailableForCity(int cityId, CityDevConfig devCfg)
    {
        if (!devCfg.IsSpecial) return true;
        var worldCfg = WorldConfig.GetConfig(cityId);
        if (worldCfg.SpecialBuildings == null) return false;
        foreach (int buildingId in worldCfg.SpecialBuildings)
        {
            if (buildingId == devCfg.Id) return true;
        }
        return false;
    }

}