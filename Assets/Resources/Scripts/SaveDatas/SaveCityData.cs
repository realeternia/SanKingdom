using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonConfig;
using System.Linq;
using Controls.Utils;

[System.Serializable]
public class SaveCityData
{
    public int cityId;
    public int forceId;
    public int level;
    public int exp;
    public float soldier;
    public float happy;
    public float food;
    public float wall;
    public List<DevAssignmentData> devAssignments = new List<DevAssignmentData>();    

    public int ownerHeroId;
    [NonSerialized]
    public Dictionary<int, int> actions = new Dictionary<int, int>();

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
        var seasonCfg = SeasonConfig.GetConfig(GameManager.Instance.SeasonId);
        var forceData = GameManager.Instance.GetForce(forceId);
        if(forceData != null)
        {
            if(seasonCfg.AddGold != 0)
                forceData.AddAttr("gold", (int)SysFormula.City.CalculateGoldProduction(level, seasonCfg.AddGold));
        }
        if(seasonCfg.AddFood != 0)
            AddAttr("food", (int)SysFormula.City.CalculateFoodProduction(level, seasonCfg.AddFood));
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
        var nearCityIds = WorldConfig.GetConfig(cityId)?.WorldNearIds;
        
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
                if(nearCityIds != null && System.Array.Exists(nearCityIds, id => id == member.cityId))
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
            if(member.cityId == cityId && member.state == HeroState.Normal)
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

    public List<BattleCardData> GetBattleHeroList(int[] filterHeroList = null, Dictionary<int, int> heroSoldierDict = null, Dictionary<int, int> heroArmsDict = null)
    {
        if (heroSoldierDict == null)
            heroSoldierDict = DistributeSoldierDefault(filterHeroList ?? GetNormalHeroList().ToArray());

        var heroList = GetNormalHeroList();
        List<BattleCardData> battleList = new List<BattleCardData>();
        foreach (var member in heroList)
        {
            if (filterHeroList != null && !Array.Exists(filterHeroList, x => x == member))
                continue;

            var hero = GameManager.Instance.GetHero(member);
            var cardData = new BattleCardData();
            cardData.CardId = member;
            cardData.Level = hero.GetLevel();
            cardData.SoldierNum = Math.Max(1, heroSoldierDict.ContainsKey(member) ? heroSoldierDict[member] : 0);
            cardData.ArmsId = (heroArmsDict != null && heroArmsDict.ContainsKey(member)) ? heroArmsDict[member] : (hero.armsId > 0 ? hero.armsId : SystemConst.Hero.DEFAULT_ARMS_ID);
            battleList.Add(cardData);
        }
        return battleList;
    }

    public Dictionary<int, int> DistributeSoldierDefault(int[] heroIds, int maxPerHero = SystemConst.Hero.MAX_SOLDIER_PER_HERO)
    {
        var result = new Dictionary<int, int>();
        int citySoldier = (int)Math.Floor(soldier);

        var heroList = heroIds.Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null)
            .OrderByDescending(h => h.GetAttr("leadship"))
            .ToList();

        foreach (var hero in heroList)
        {
            if (citySoldier <= 0) break;
            int toAssign = Math.Min(maxPerHero, citySoldier);
            result[hero.heroId] = toAssign;
            citySoldier -= toAssign;
        }

        soldier = citySoldier;
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

    public void AddAttr(string type, int add)
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
        
        switch (type.ToLower())
        {
            case "level":
                level += add;
                break;
            case "exp":
                exp += add;
                break;
            case "soldier":
                soldier += add;
                break;
            case "happy":
                happy += add;
                break;
            case "food":
                food += add;
                break;
            case "wall":
                wall += add;
                break;
            default:
                break;
        }

        if (PanelManager.Instance != null)
        {
            PanelManager.Instance.SendSignal(new CityResChangeSignal { CityId = cityId, ResType = type.ToLower(), Value = GetAttr(type.ToLower()) });
        }
    }

    public int GetAttr(string type)
    {
        switch (type.ToLower())
        {
            case "level":
                return level;
            case "exp":
                return exp;
            case "soldier":
                return (int)Math.Floor(soldier);
            case "happy":
                return (int)Math.Floor(happy);
            case "food":
                return (int)Math.Floor(food);
            case "wall":
                return (int)Math.Floor(wall);
            default:
                return 0;
        }
    }

    public void MoveHeroTo(int[] heroIds, int destCityId)
    {
        foreach (var heroId in heroIds)
        {
            SaveHeroData hero = GameManager.Instance.GetHero(heroId);
            if (hero != null)
            {
                RemoveDevAssignment(heroId);
                hero.cityId = destCityId;
            }
        }
    }

    public int CalculateDistanceTo(int destCityId)
    {
        var curCity = WorldConfig.GetConfig(cityId);
        var destCity = WorldConfig.GetConfig(destCityId);
        return SysFormula.City.CalculateDistance(curCity.X, curCity.Y, destCity.X, destCity.Y);
    }

    public void Occupy(int forceWin, List<int> winHeroIds, int forceLose, List<int> failHeroIds)
    {
        forceId = forceWin;

        ClearDevAssignments();

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

        List<SaveCityData> loseForceCities = GameManager.Instance.GetCitiesByForce(forceLose);

        GameLog.Info($"Occupy cityId={cityId} winforceId: {forceWin} loseforceId: {forceLose} citycount: {loseForceCities.Count}");
        if (loseForceCities.Count > 0)
        {
            var kingHeroId = ForceConfig.GetConfig(forceLose).HeroId;
            var destCityIds = new HashSet<int>();
            foreach (var heroId in failHeroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero != null)
                {
                    if (heroId == kingHeroId)
                    {
                        hero.cityId = GameManager.Instance.GetRandomForceCityId(cityId, forceLose);
                        destCityIds.Add(hero.cityId);
                    }
                    else
                    {
                        int catchChance = SysFormula.Hero.CalculateCaptureChance(hero.str);
                        if (SysRandom.Range(0, 100) >= catchChance)
                        {
                            hero.cityId = GameManager.Instance.GetRandomForceCityId(cityId, forceLose);
                            destCityIds.Add(hero.cityId);
                        }
                        else
                        {
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
            foreach (var heroId in failHeroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero != null)
                {
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
                hero.cityId = cityId;
        }

        RecalculateHeros();
        PanelManager.Instance.SendSignal(new CityForceChangeSignal { CityId = cityId });

        GameManager.Instance.SaveToFile();
    }

    public void RecalculateHeros()
    {
        ownerHeroId = 0;
        SelectOwner();
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

    public Dictionary<string, int> CalculateDevAttrAddons()
    {
        Dictionary<string, int> attrAddons = new Dictionary<string, int>();
        var forceData = GetForce();
        
        GameLog.Debug($"CalculateDevAttrAddons cityId={cityId} devAssignments={devAssignments.Count}");
        
        foreach (var assignment in devAssignments)
        {
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

    private void CalculateDevAttrAddonsForAssignment(CityDevConfig devCfg, SaveHeroData heroData, SaveForceData forceData, Dictionary<string, int> attrAddons)
    {
        if (!string.IsNullOrEmpty(devCfg.DevAttr1))
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr1.ToLower());
            if (!attrConfig.IsForceAttr)
            {
                int attrVal = GetHeroAttrValueForDev(heroData, devCfg.Attrs);
                GameLog.Debug($"CalculateDevAttrAddonsForAssignment DevAttr1={devCfg.DevAttr1} attrs={string.Join(",", devCfg.Attrs)} attrVal={attrVal}");
                int addon = CalculateDevAddon(devCfg.DevAttr1, devCfg.DevAttr1Value[0], devCfg.DevAttr1Value[1], forceData, attrVal);
                if (addon > 0)
                {
                    string attrName = devCfg.DevAttr1.ToLower();
                    if (!attrAddons.ContainsKey(attrName))
                        attrAddons[attrName] = 0;
                    attrAddons[attrName] += addon;
                }
            }
        }
        
        if (!string.IsNullOrEmpty(devCfg.DevAttr2) && devCfg.DevAttr2Value != null && devCfg.DevAttr2Value.Length >= 2)
        {
            var attrConfig = CityAttrConfig.GetConfigByname(devCfg.DevAttr2.ToLower());
            if (!attrConfig.IsForceAttr)
            {
                int attrVal = GetHeroAttrValueForDev(heroData, devCfg.Attrs);
                GameLog.Debug($"CalculateDevAttrAddonsForAssignment DevAttr2={devCfg.DevAttr2} attrs={string.Join(",", devCfg.Attrs)} attrVal={attrVal}");
                int addon = CalculateDevAddon(devCfg.DevAttr2, devCfg.DevAttr2Value[0], devCfg.DevAttr2Value[1], forceData, attrVal);
                if (addon > 0)
                {
                    string attrName = devCfg.DevAttr2.ToLower();
                    if (!attrAddons.ContainsKey(attrName))
                        attrAddons[attrName] = 0;
                    attrAddons[attrName] += addon;
                }
            }
        }
    }

    private int GetHeroAttrValueForDev(SaveHeroData heroData, string[] attrs)
    {
        if (attrs == null || attrs.Length == 0)
            return 0;
        
        if (attrs.Length == 1)
        {
            return heroData.GetAttr(attrs[0]);
        }
        else
        {
            int firstAttr = heroData.GetAttr(attrs[0]);
            int secondAttr = heroData.GetAttr(attrs[1]);
            if (secondAttr > firstAttr)
            {
                firstAttr += SysFormula.City.CalculateSecondaryAttrContribution(firstAttr, secondAttr);
            }
            return firstAttr;
        }
    }

    private int CalculateDevAddon(string attrName, int min, int max, SaveForceData forceData, int heroAttrVal)
    {
        var attrConfig = CityAttrConfig.GetConfigByname(attrName.ToLower());
        
        int currentVal = GetAttr(attrName);
        int valMax = attrConfig.ValMaxCity;
        
        float val = SysFormula.City.CalculateDevValue(min, max, heroAttrVal, currentVal, valMax);
        GameLog.Debug($"CalculateDevAddon attrName={attrName} min={min} max={max} heroAttrVal={heroAttrVal} currentVal={currentVal} valMax={valMax} result={val}");
        return (int)val;
    }

}