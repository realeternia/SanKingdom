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
        var soldierPerTeam = (int)(soldier / heroIds.Count);
        UnityEngine.Debug.Log(" soldierPerTeam " + " " + soldierPerTeam + " cityId " + cityId);
        if(soldierPerTeam > 1000)
            soldierPerTeam = 1000;
        soldier -= soldierPerTeam * heroIds.Count;
        if(soldierPerTeam < 1)
        {
            soldierPerTeam = 1;
            soldier = 0;
        }
        List<BattleCardData> battleList = new List<BattleCardData>();
        foreach (var member in heroIds)
        {
            if(filterHeroList != null && !Array.Exists(filterHeroList, x => x == member))
                continue;
            var cardData = new BattleCardData();
            cardData.CardId = member;
            cardData.Level = GameManager.Instance.GetHero(member).GetLevel();
            cardData.SoliderNum = soldierPerTeam;
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
                return soldier;
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
}