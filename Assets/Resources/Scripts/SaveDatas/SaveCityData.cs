using System;
using System.Collections.Generic;
using System.Diagnostics;
using CommonConfig;

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

    public void OnRound(int round)
    {
        if((round % 3) == 1) // 发钱
        {
            gold += archGold;
        }
        else if((round % 12) == 7) // 发粮食
        {
            food += archFood;
        }
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
            cardData.SoliderNum = Math.Max(1, hero.soldier); //临时方案，送一个兵
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

    public void Occupy(int forceWin, List<int> winHeroIds, int forceLose, List<int> failHeroIds)
    {
        forceId = forceWin;

        List<SaveCityData> loseForceCities = GameManager.Instance.GetCitiesByForce(forceLose);

        UnityEngine.Debug.Log($"Occupy forceId: {forceLose} citycount: {loseForceCities.Count}");
        if (loseForceCities.Count > 0)
        {
            SaveCityData destCity = loseForceCities[0];
            foreach (var heroId in failHeroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero != null)
                {
                    hero.cityId = destCity.cityId;
                }
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
            if (HeroConfig.GetConfig(heroId).Job == "shuai")
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