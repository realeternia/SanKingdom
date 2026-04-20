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
    public float gold;
    public float food;
    public float soldier;
    public float power;
    public float wall;
    public List<DevAssignmentData> devAssignments = new List<DevAssignmentData>();    

    [NonSerialized]
    private int ownerHeroId;
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
    }

    public void RemoveDevAssignment(int heroId)
    {
        devAssignments.RemoveAll(d => d.heroId == heroId);
    }

    public void ClearDevAssignments()
    {
        devAssignments.Clear();
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
        if(seasonCfg.AddGold != 0)
            gold += (int)(level * SystemConst.City.GOLD_PER_LEVEL + seasonCfg.AddGold);
        else if(seasonCfg.AddFood != 0)
            food += (int)(level * SystemConst.City.FOOD_PER_LEVEL * seasonCfg.AddFood);
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

    public Player GetPlayer()
    {
        return GameManager.Instance.GetPlayer(forceId);
    }

    public int GetOwner()
    {
        if(ownerHeroId > 0)
            return ownerHeroId;
        foreach (var memberId in GetNormalHeroList())
        {
            var hero = GameManager.Instance.GetHero(memberId);
            if (hero == null)
                continue;
            if (hero.cityId == cityId && hero.cityOwner)
            {
                ownerHeroId = memberId;
                return memberId;
            }
        }
        return 0;
    }

    public void AddAttr(string type, int add)
    {
        switch (type.ToLower())
        {
            case "level":
                level += add;
                break;
            case "exp":
                exp += add;
                break;
            case "gold":
                gold += add;
                break;
            case "food":
                food += add;
                break;
            case "soldier":
                soldier += add;
                break;
            case "wall":
                wall += add;
                break;
            case "power":
                power += add;
                break;
            default:
                break;
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
            case "gold":
                return (int)Math.Floor(gold);
            case "food":
                return (int)Math.Floor(food);
            case "soldier":
                return (int)Math.Floor(soldier);
            case "wall":
                return (int)Math.Floor(wall);
            case "power":
                return (int)Math.Floor(power);
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
        //通过WorldConfig的x,y算距离
        var curCity = WorldConfig.GetConfig(cityId);
        var destCity = WorldConfig.GetConfig(destCityId);
        return Math.Abs(curCity.X - destCity.X) + Math.Abs(curCity.Y - destCity.Y);
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
                        int str = hero.str > 0 ? hero.str : 50;
                        int catchChance = SystemConst.Expedition.CATCH_BASE_CHANCE + (100 - str) * SystemConst.Expedition.CATCH_STR_FACTOR / 100;
                        if (UnityEngine.Random.Range(0, 100) >= catchChance)
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
            var player = GameManager.Instance.GetPlayer(forceLose);
            if (player != null)
                player.mark = -1;
            var force = GameManager.Instance.GetForce(forceLose);
            if (force != null)
                force.isEliminated = true;
            GameLog.Info($"Occupy 势力 {forceLose} 已被消灭");
        }

        foreach (var heroId in winHeroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero != null)
                hero.cityId = cityId;
        }

        RecalculateHeros();
        PanelManager.Instance.SendSignal("CityForceChange", "", cityId);

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

            float totalScore = str * SystemConst.City.OWNER_SCORE_WEIGHT_STR + inte + fair + (leadship * SystemConst.City.OWNER_SCORE_WEIGHT_LEADSHIP) + (charm * SystemConst.City.OWNER_SCORE_WEIGHT_CHARM);
            if (heroId == kingHeroId)
            {
                totalScore += SystemConst.City.KING_OWNER_BONUS_SCORE;
                GameLog.Info($"帅的分 {heroId} {totalScore}");
            }

            if (totalScore > maxScore)
            {
                maxScore = (int)totalScore;
                bestHero = hero;
            }
        }

        if (bestHero != null)
        {
            foreach (var heroId in heroList)
            {
                SaveHeroData hero = GameManager.Instance.GetHero(heroId);
                hero.cityOwner = (heroId == bestHero.heroId);
            }
        }
    }

}