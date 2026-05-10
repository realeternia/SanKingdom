using System;
using System.Collections.Generic;
using System.Linq;

public static class TroopsBuilder
{
    public static List<SaveTroopsData> BuildAttackTroopsFromHeroList(SaveCityData city, int[] heroList, Dictionary<int, int> heroSoldierDict, Dictionary<int, int> heroArmsDict)
    {
        List<SaveTroopsData> troops = new List<SaveTroopsData>();
        
        if (heroList == null || heroList.Length == 0)
            return troops;

        if (heroSoldierDict == null)
            heroSoldierDict = city.DistributeSoldierDefault(heroList);

        foreach (var heroId in heroList)
        {
            var troop = new SaveTroopsData();
            troop.heroId1 = heroId;
            troop.soldierCount = heroSoldierDict.ContainsKey(heroId) ? heroSoldierDict[heroId] : 0;
            troop.armsId = (heroArmsDict != null && heroArmsDict.ContainsKey(heroId)) ? heroArmsDict[heroId] : SystemConst.Hero.DEFAULT_ARMS_ID;
            troops.Add(troop);
        }

        return troops;
    }

    public static List<SaveTroopsData> BuildDefenceTroops(SaveCityData city)
    {
        List<SaveTroopsData> defenceTroops = new List<SaveTroopsData>();
        
        int citySoldier = (int)Math.Floor(city.soldier);
        int cityForceId = city.forceId;
        
        foreach (var troop in SaveTroopsData.GetTroopsByCity(city.cityId))
        {
            if (troop.heroId1 <= 0)
                continue;

            var filledTroop = new SaveTroopsData(troop.heroId1, troop.heroId2, troop.heroId3, troop.armsId);
            
            int maxSoldier = SystemConst.Hero.MAX_SOLDIER_PER_HERO;
            int heroCount = 0;
            if (troop.heroId1 > 0) heroCount++;
            if (troop.heroId2 > 0) heroCount++;
            if (troop.heroId3 > 0) heroCount++;
            
            int totalMaxSoldier = maxSoldier * heroCount;
            int soldierToFill = Math.Min(totalMaxSoldier - troop.soldierCount, citySoldier);
            filledTroop.soldierCount = troop.soldierCount + soldierToFill;
            citySoldier -= soldierToFill;
            
            defenceTroops.Add(filledTroop);
        }

        if (citySoldier > 0)
        {
            var availableHeroes = city.GetNormalHeroList()
                .Select(id => GameManager.Instance.GetHero(id))
                .Where(h => h != null && h.forceId == cityForceId && !defenceTroops.Any(t => t.heroId1 == h.heroId || t.heroId2 == h.heroId || t.heroId3 == h.heroId))
                .ToList();

            var balancedHeroes = availableHeroes
                .Where(h => h.GetAttr("str") >= 80 && h.GetAttr("inte") >= 80)
                .OrderByDescending(h => h.GetAttr("str") + h.GetAttr("inte"))
                .ToList();

            foreach (var hero in balancedHeroes)
            {
                if (citySoldier <= 0) break;
                
                int soldier = Math.Min(SystemConst.Hero.MAX_SOLDIER_PER_HERO, citySoldier);
                if (soldier > 0)
                {
                    var troop = new SaveTroopsData();
                    troop.heroId1 = hero.heroId;
                    troop.soldierCount = soldier;
                    troop.armsId = hero.GetArmsId() > 0 ? hero.GetArmsId() : SystemConst.Hero.DEFAULT_ARMS_ID;
                    defenceTroops.Add(troop);
                    citySoldier -= soldier;
                }
            }

            var combatHeroes = availableHeroes
                .Where(h => h.GetAttr("str") >= 80 || h.GetAttr("leadship") >= 80)
                .OrderByDescending(h => Math.Max(h.GetAttr("str"), h.GetAttr("leadship")))
                .ToList();

            var intelHeroes = availableHeroes
                .Where(h => h.GetAttr("inte") >= 80 && h.GetAttr("str") < 80 && h.GetAttr("leadship") < 80)
                .OrderByDescending(h => h.GetAttr("inte"))
                .ToList();

            int i = 0, j = 0;
            while (citySoldier > 0 && (i < combatHeroes.Count || j < intelHeroes.Count))
            {
                var troop = new SaveTroopsData();
                int totalSoldier = 0;
                
                if (i < combatHeroes.Count)
                {
                    var combatHero = combatHeroes[i];
                    troop.heroId1 = combatHero.heroId;
                    troop.armsId = combatHero.GetArmsId() > 0 ? combatHero.GetArmsId() : SystemConst.Hero.DEFAULT_ARMS_ID;
                    int soldier = Math.Min(SystemConst.Hero.MAX_SOLDIER_PER_HERO, citySoldier);
                    totalSoldier += soldier;
                    citySoldier -= soldier;
                    i++;
                }
                
                if (j < intelHeroes.Count && citySoldier > 0)
                {
                    var intelHero = intelHeroes[j];
                    troop.heroId2 = intelHero.heroId;
                    int soldier = Math.Min(SystemConst.Hero.MAX_SOLDIER_PER_HERO, citySoldier);
                    totalSoldier += soldier;
                    citySoldier -= soldier;
                    j++;
                }
                
                if (troop.heroId1 > 0)
                {
                    troop.soldierCount = totalSoldier;
                    defenceTroops.Add(troop);
                }
            }

            foreach (var hero in availableHeroes)
            {
                if (citySoldier <= 0) break;
                if (defenceTroops.Any(t => t.heroId1 == hero.heroId || t.heroId2 == hero.heroId || t.heroId3 == hero.heroId))
                    continue;

                int soldier = Math.Min(SystemConst.Hero.MAX_SOLDIER_PER_HERO, citySoldier);
                if (soldier > 0)
                {
                    var troop = new SaveTroopsData();
                    troop.heroId1 = hero.heroId;
                    troop.soldierCount = soldier;
                    troop.armsId = hero.GetArmsId() > 0 ? hero.GetArmsId() : SystemConst.Hero.DEFAULT_ARMS_ID;
                    defenceTroops.Add(troop);
                    citySoldier -= soldier;
                }
            }
        }

        return defenceTroops;
    }
}
