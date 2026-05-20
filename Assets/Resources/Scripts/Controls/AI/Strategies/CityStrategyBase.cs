using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public abstract class CityStrategyBase
{
    protected AIStrategyContext Context;
    protected SaveCityData City;
    protected SaveForceData Force;

    public CityStrategyState State { get; }

    protected CityStrategyBase(CityStrategyState state, AIStrategyContext context, SaveCityData city, SaveForceData force)
    {
        State = state;
        Context = context;
        City = city;
        Force = force;
    }

    public abstract void Execute();
    
    protected abstract List<CityDevConfig> GetSortedDevConfigs();

    protected void AssignHeroesToDev()
    {
        AIToolHeroDev.AssignHeroesToCityDev(Force, City);
    }

    protected void FormTroops()
    {
        var existingTroops = SaveTroopsData.GetTroopsByCity(City.cityId);

        var normalHeroes = City.GetNormalHeroList();
        if (normalHeroes.Count == 0) return;

        var assignedHeroIds = new HashSet<int>();

        foreach (var troop in existingTroops)
        {
            if (troop.heroId1 > 0) assignedHeroIds.Add(troop.heroId1);
            if (troop.heroId2 > 0) assignedHeroIds.Add(troop.heroId2);
            if (troop.heroId3 > 0) assignedHeroIds.Add(troop.heroId3);
        }

        var idleHeroes = normalHeroes
            .Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null && !assignedHeroIds.Contains(h.heroId))
            .ToList();

        if (idleHeroes.Count < SystemConst.AIStrategy.TROOP_MIN_HEROES) return;

        var commanders = idleHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) != HeroType.Domestic)
            .OrderByDescending(h => h.GetAttr("leadship"))
            .ToList();

        var viceHeroes = idleHeroes
            .Where(h => HeroDispatcher.ClassifyHero(h) != HeroType.Combat)
            .OrderByDescending(h => h.GetAttr("inte") + h.GetAttr("charm"))
            .ToList();

        if (commanders.Count == 0)
        {
            commanders = idleHeroes.OrderByDescending(h => h.GetAttr("leadship")).ToList();
            viceHeroes = new List<SaveHeroData>();
        }

        int cityLevel = City.GetLevel();
        int citySoldier = City.GetAttr("soldier");

        int troopLimit = SysFormula.AIStrategy.CalculateTroopLimit(cityLevel, idleHeroes.Count, citySoldier);

        int existingCount = existingTroops.Count;
        int newTroopCount = Math.Max(0, Math.Min(troopLimit, commanders.Count) - existingCount);

        if (newTroopCount == 0) return;

        var usedHeroIds = new HashSet<int>();
        int formedCount = 0;

        for (int i = 0; i < newTroopCount; i++)
        {
            var commander = commanders[i];
            usedHeroIds.Add(commander.heroId);

            var troop = new SaveTroopsData();
            troop.heroId1 = commander.heroId;
            troop.armsId = commander.GetArmsId() > 0 ? commander.GetArmsId() : SystemConst.Hero.DEFAULT_ARMS_ID;

            int viceCount = 0;
            foreach (var viceHero in viceHeroes)
            {
                if (viceCount >= SystemConst.AIStrategy.TROOP_MAX_HEROES - 1) break;
                if (usedHeroIds.Contains(viceHero.heroId)) continue;

                if (viceCount == 0)
                    troop.heroId2 = viceHero.heroId;
                else if (viceCount == 1)
                    troop.heroId3 = viceHero.heroId;
                usedHeroIds.Add(viceHero.heroId);
                viceCount++;
            }

            SaveTroopsData.AddTroopToCity(troop, City.cityId);
            formedCount++;
        }

        if (formedCount > 0)
        {
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 组建{formedCount}个军团(等级{cityLevel} 空闲武将{idleHeroes.Count} 士兵{citySoldier})");
        }
    }
    
    protected void AssignAdvancedArms()
    {
        var allTroops = SaveTroopsData.GetTroopsByCity(City.cityId);
        if (allTroops.Count == 0) return;
        
        int horse = City.GetAttr("horse");
        int steel = City.GetAttr("steel");
        
        if (horse <= 0 && steel <= 0) return;
        
        var scoredTroops = allTroops.Select(troop => 
        {
            float score = CalculateTroopImportance(troop);
            return new { Troop = troop, Score = score };
        })
        .OrderByDescending(x => x.Score)
        .ToList();
        
        foreach (var item in scoredTroops)
        {
            var troop = item.Troop;
            
            if (steel > 0 && troop.armsId == SystemConst.Hero.DEFAULT_ARMS_ID)
            {
                AssignBetterArmsToTroop(troop);
            }
        }
        
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 完成高级兵种分配");
    }
    
    private float CalculateTroopImportance(SaveTroopsData troop)
    {
        float score = 0;
        
        var hero1 = troop.heroId1 > 0 ? GameManager.Instance.GetHero(troop.heroId1) : null;
        if (hero1 != null)
        {
            score += hero1.GetAttr("leadship") * 2;
            score += hero1.GetAttr("str");
            score += hero1.GetAttr("inte") * 0.5f;
            if (HeroDispatcher.ClassifyHero(hero1) == HeroType.Combat)
                score += 50;
        }
        
        var hero2 = troop.heroId2 > 0 ? GameManager.Instance.GetHero(troop.heroId2) : null;
        if (hero2 != null)
        {
            score += hero2.GetAttr("leadship");
            score += hero2.GetAttr("str") * 0.5f;
        }
        
        var hero3 = troop.heroId3 > 0 ? GameManager.Instance.GetHero(troop.heroId3) : null;
        if (hero3 != null)
        {
            score += hero3.GetAttr("leadship") * 0.5f;
        }
        
        return score;
    }
    
    private void AssignBetterArmsToTroop(SaveTroopsData troop)
    {
        var armsConfigs = ArmsConfig.ConfigList
            .Where(a => a.Id > SystemConst.Hero.DEFAULT_ARMS_ID)
            .OrderByDescending(a => a.Atk + a.Def)
            .ToList();
        
        foreach (var armsCfg in armsConfigs)
        {
            int horse = City.GetAttr("horse");
            int steel = City.GetAttr("steel");
            
            troop.armsId = armsCfg.Id;
            GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 给军团装备 {armsCfg.NameS}");
            break;
        }
    }
}
