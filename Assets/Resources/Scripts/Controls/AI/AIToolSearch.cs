using System.Collections.Generic;
using System.Linq;
using CommonConfig;

/// <summary>
/// AI走访工具类
/// 随机选取 0~SEARCH_HERO_MAX_COUNT 名空闲武将执行走访，每人获得随机金钱
/// 优先级高于褒奖，确保空闲武将优先产出金钱
/// </summary>
public static class AIToolSearch
{
    public static void Process(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        int devId = CityDevConfig.GetConfigByName("Search").Id;
        int currentRound = GameManager.Instance.SaveData.round;

        // 1. 收集所有空闲武将（按城市分组）：排除 WarPlan、已委派、已行动
        var idleByCity = new List<KeyValuePair<int, int>>();
        foreach (var city in force.GetCityList())
        {
            var normalHeroes = city.GetNormalHeroList();
            var assignedHeroIds = new HashSet<int>(city.GetDevAssignments().Select(a => a.heroId));

            foreach (var heroId in normalHeroes)
            {
                if (excludedHeroIds.Contains(heroId)) continue;
                if (assignedHeroIds.Contains(heroId)) continue;

                var hero = GameManager.Instance.GetHero(heroId);
                if (hero == null) continue;
                if (hero.round >= currentRound) continue;

                idleByCity.Add(new KeyValuePair<int, int>(city.cityId, heroId));
            }
        }

        if (idleByCity.Count == 0) return;

        // 2. 随机决定本次走访人数 [0, maxCount]
        int maxCount = System.Math.Min(AIConst.AIKingAction.SEARCH_HERO_MAX_COUNT, idleByCity.Count);
        int searchCount = SysRandom.Range(0, maxCount + 1);
        if (searchCount == 0) return;

        // 3. 随机选取 searchCount 名武将
        var shuffled = idleByCity.OrderBy(_ => SysRandom.Value).Take(searchCount).ToList();

        // 4. 按城市分组执行走访
        var groupedByCity = shuffled.GroupBy(kv => kv.Key, kv => kv.Value);
        int totalHeroes = 0;
        foreach (var group in groupedByCity)
        {
            force.ExecuteCitySearch(group.Key, devId, group.ToArray(), out var attrDatas);
            totalHeroes += group.Count();
        }

        if (totalHeroes > 0)
        {
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 走访{totalHeroes}名武将");
        }
    }
}
