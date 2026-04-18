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

    [NonSerialized]
    private int ownerHeroId;
    [NonSerialized]
    public Dictionary<int, int> actions = new Dictionary<int, int>();

    public List<DevAssignmentData> devAssignments = new List<DevAssignmentData>();

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
            gold += (int)(level * 50 + seasonCfg.AddGold);
        else if(seasonCfg.AddFood != 0)
            food += (int)(level * 40 * seasonCfg.AddFood);
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
            else if(member.state == HeroState.Normal && member.forceId != forceId && member.loyalty < 95)
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

    public List<BattleCardData> GetBattleHeroList(int[] filterHeroList = null)
    {
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
            cardData.SoldierNum = Math.Max(1, hero.soldier);
            cardData.ArmsId = hero.armsId > 0 ? hero.armsId : 601;
            battleList.Add(cardData);
        }
        return battleList;    
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
                int soldierOnHero = 0;
                foreach (var heroId in GetNormalHeroList())
                {
                    var hero = GameManager.Instance.GetHero(heroId);
                    if (hero != null)
                        soldierOnHero += hero.soldier;
                }
                return (int)Math.Floor(soldier + soldierOnHero);
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
                        var heroCfg = HeroConfig.GetConfig(heroId);
                        int str = heroCfg != null ? heroCfg.Str : 50;
                        int catchChance = 7 + (100 - str) * 8 / 100;
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
                    hero.forceId = 0;
                    hero.loyalty = 90;
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

            float totalScore = str * .75f + inte + fair + (leadship * 1.5f) + (charm * 1.2f);
            if (heroId == kingHeroId)
            {
                totalScore += 9999;
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

    public void AutoSetSoldierOnInit()
    {
        var heroList = GetNormalHeroList();
        if (heroList.Count == 0)
            return;

        SaveHeroData owner = GameManager.Instance.GetHero(GetOwner());

        int ownerSoldier = 1000;
        soldier -= ownerSoldier;
        owner.soldier = ownerSoldier;

        foreach (var heroId in heroList)
        {
            SaveHeroData hero = GameManager.Instance.GetHero(heroId);
            if (hero != owner)
            {
                hero.soldier = 100;
                soldier -= 100;
            }
        }

        for (int idx = 0; idx < 4; idx++)
        {
            List<SaveHeroData> eligibleHeroes = new List<SaveHeroData>();
            foreach (var heroId in heroList)
            {
                SaveHeroData hero = GameManager.Instance.GetHero(heroId);
                if (hero.soldier > 100)
                    continue;
                int leadship = hero.GetAttr("leadship");
                int str = hero.GetAttr("str");
                int inte = hero.GetAttr("inte");
                int x = leadship + str / 2 + inte / 2;

                if (idx == 0 && (leadship >= 90 || x >= 160))
                {
                    eligibleHeroes.Add(hero);
                }
                else if (idx == 1 && (leadship >= 80 || x >= 140))
                {
                    eligibleHeroes.Add(hero);
                }
                else if (idx == 2 && (leadship >= 65 || x >= 110))
                {
                    eligibleHeroes.Add(hero);
                }
                else if (idx == 3)
                {
                    eligibleHeroes.Add(hero);
                }
            }

            if (eligibleHeroes.Count > 0)
            {
                int elitePortion = Math.Min(900, (int)(soldier * 0.7f) / eligibleHeroes.Count);
                foreach (var hero in eligibleHeroes)
                {
                    hero.soldier += elitePortion;
                    soldier -= elitePortion;
                }
            }
        }

        foreach (var heroId in heroList)
        {
            SaveHeroData hero = GameManager.Instance.GetHero(heroId);
            if (hero != owner)
            {
                var backSoldier = hero.soldier % 50;
                if (backSoldier > 0)
                {
                    hero.soldier -= backSoldier;
                    soldier += backSoldier;
                }
            }
        }
    }
}