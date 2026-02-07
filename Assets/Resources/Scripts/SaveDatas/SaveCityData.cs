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
    public int archGold; //商业
    public int archFood; //农业
    public int archPeople; //人口
    public int gold; //现有黄金
    public int food; //现有粮食
    public int soldier; //士兵
    public int secure; //安全系数
    public int power; //士气
    public int wall; //城防

    [NonSerialized]
    private int ownerHeroId;
    [NonSerialized]
    private List<int> heroIds;
    [NonSerialized]
    public Dictionary<int, int> actions = new Dictionary<int, int>();

    public void OnRound()
    {
        var seasonCfg = SeasonConfig.GetConfig(GameManager.Instance.SeasonId);
        if(seasonCfg.AddGold != 0) // 发钱
            gold += (int)(archGold + seasonCfg.AddGold);
        else if(seasonCfg.AddFood != 0) // 发粮食
            food += (int)(archFood * seasonCfg.AddFood);
        actions.Clear();
    }

    public void AddAction(int devId, int count)
    {
        if(actions.ContainsKey(devId))
            actions[devId] += count;
        else
            actions.Add(devId, count);
    }

    public List<int> GetHeroList()
    {
        if(heroIds != null)
            return heroIds;
        heroIds = new List<int>();
        foreach (var member in GameManager.Instance.SaveData.heros)
        {
            if(member.cityId == cityId)
                heroIds.Add(member.heroId);
        }
        return heroIds;
    }

    public List<BattleCardData> GetBattleHeroList(int[] filterHeroList = null)
    {
        var heroList = GetHeroList();
        List<BattleCardData> battleList = new List<BattleCardData>();
        foreach (var member in heroIds)
        {
            if (filterHeroList != null && !Array.Exists(filterHeroList, x => x == member))
                continue;

            var hero = GameManager.Instance.GetHero(member);
            var cardData = new BattleCardData();
            cardData.CardId = member;
            cardData.Level = hero.GetLevel();
            cardData.SoldierNum = Math.Max(1, hero.soldier); //临时方案，送一个兵
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
        foreach (var member in GameManager.Instance.SaveData.heros)
        {
            if (member.cityId == cityId && member.cityOwner)
            {
                ownerHeroId = member.heroId;
                return member.heroId;
            }
        }
        return 0;
    }

    public void AddAttr(string type, int add)
    {
        switch (type.ToLower())
        {
            case "archgold":
                archGold += add;
                break;
            case "archfood":
                archFood += add;
                break;
            case "archpeople":
                archPeople += add;
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
            case "secure":
                secure += add;
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
            case "archgold":
                return archGold;
            case "archfood":
                return archFood;
            case "archpeople":
                return archPeople;
            case "gold":
                return gold;
            case "food":
                return food;
            case "soldier":
                int soldierOnHero = 0;
                foreach (var heroId in GetHeroList())
                {
                    var hero = GameManager.Instance.GetHero(heroId);
                    if (hero != null)
                        soldierOnHero += hero.soldier;
                }
                return soldier + soldierOnHero;
            case "secure":
                return secure;
            case "wall":
                return wall;
            case "power":
                return power;
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

        List<SaveCityData> loseForceCities = GameManager.Instance.GetCitiesByForce(forceLose);

        UnityEngine.Debug.Log($"Occupy forceId: {forceLose} citycount: {loseForceCities.Count}");
        if (loseForceCities.Count > 0)
        {
            // 获取当前城市的相邻城市ID列表
            var currentCityConfig = WorldConfig.GetConfig(cityId);
            var nearbyCityIds = currentCityConfig.WorldNearIds;

            // 过滤出与当前城市相邻的失败方城市
            var nearbyLoseCities = new List<SaveCityData>();
            foreach (var city in loseForceCities)
            {
                if (nearbyCityIds != null && Array.Exists(nearbyCityIds, id => id == city.cityId))
                    nearbyLoseCities.Add(city);
            }

            List<int> destCityIds = new List<int>();
            if (nearbyLoseCities.Count > 0)
            {
                destCityIds.AddRange(nearbyLoseCities.Select(x => x.cityId));
            }
            else if(cityId != GameManager.Instance.GetPlayer(forceLose).GetKingCity().cityId)
            {
                destCityIds.Add(GameManager.Instance.GetPlayer(forceLose).GetKingCity().cityId);
            }
            else
            {
                destCityIds.AddRange(loseForceCities.Select(x => x.cityId));
            }

            foreach (var heroId in failHeroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero != null)
                {
                    int randomIndex = UnityEngine.Random.Range(0, destCityIds.Count);
                    hero.cityId = destCityIds[randomIndex];
                }
            }
            foreach (var destCityId in destCityIds)
            {
                GameManager.Instance.GetCity(destCityId).RecalculateHeros();
            }
        }
        else
        {
            GameManager.Instance.players.RemoveAll(x => x.forceId == forceLose);
            GameManager.Instance.SaveData.forces.RemoveAll(x => x.forceId == forceLose);
            UnityEngine.Debug.Log($"Occupy 强制数量: {GameManager.Instance.SaveData.forces.Count}");
            //最后一个城了，相当于全部投降
        }

        foreach (var heroId in winHeroIds)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero != null)
            {
                hero.cityId = cityId;
            }
        }

        RecalculateHeros();
        PanelManager.Instance.SendSignal("CityForceChange", "", cityId);

        GameManager.Instance.SaveToFile();
    }

    public void RecalculateHeros()
    {
        heroIds = null;
        ownerHeroId = 0;
        SelectOwner();
    }



    public void SelectOwner()
    {
        var heroList = GetHeroList();
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
                UnityEngine.Debug.Log($"帅的分 {heroId} {totalScore}");
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
        var heroList = GetHeroList();
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

        foreach (var heroId in heroIds)
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