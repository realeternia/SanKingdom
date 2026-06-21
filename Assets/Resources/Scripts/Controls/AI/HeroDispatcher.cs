using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
public enum HeroType
{
    Combat,
    Domestic,
    Balanced
}

public class HeroDispatcher
{
    public static HeroType ClassifyHero(SaveHeroData hero)
    {
        return SysFormula.Hero.ClassifyHero(
            hero.GetAttr("str"), hero.GetAttr("leadship"), hero.GetAttr("inte"),
            hero.GetAttr("fair"), hero.GetAttr("charm"));
    }
    
    /// <summary>
    /// 英雄调度总入口：前后线战斗英雄调度 → 名将保障 → 名将均衡 → 全英雄均衡
    /// </summary>
    public static void DispatchHeroes(SaveForceData force)
    {
        DispatchCombatHeroesToFrontline(force);
        EnsureMinStarHeroPerCity(force);
        BalanceStarHeroes(force);
        BalanceAllHeroes(force);
    }

    /// <summary>
    /// 前后线战斗英雄调度：将后方战斗型英雄调往前线，每城目标3名
    /// </summary>
    private static void DispatchCombatHeroesToFrontline(SaveForceData force)
    {
        var frontlineCities = MapTool.GetFrontlineCityIds(force.forceId);
        var rearCities = MapTool.GetRearCityIds(force.forceId);
        
        if (frontlineCities.Count == 0 || rearCities.Count == 0)
            return;
        
        var rearCombatHeroes = new List<SaveHeroData>();
        var rearCityHeroMap = new Dictionary<int, List<SaveHeroData>>();
        
        foreach (var cityId in rearCities)
        {
            var city = GameManager.Instance.GetCity(cityId);
            var heroIds = city.GetNormalHeroList();
            rearCityHeroMap[cityId] = new List<SaveHeroData>();
            
            foreach (var heroId in heroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                rearCityHeroMap[cityId].Add(hero);
                
                if (ClassifyHero(hero) == HeroType.Combat)
                {
                    rearCombatHeroes.Add(hero);
                }
            }
        }
        
        foreach (var cityId in frontlineCities)
        {
            var city = GameManager.Instance.GetCity(cityId);
            var heroIds = city.GetNormalHeroList();
            int combatCount = 0;
            
            foreach (var heroId in heroIds)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                if (ClassifyHero(hero) == HeroType.Combat)
                    combatCount++;
            }
            
            int neededCombat = AIConst.AIStrategy.FRONTLINE_COMBAT_HEROES_TARGET - combatCount;
            
            for (int i = 0; i < neededCombat && rearCombatHeroes.Count > 0; i++)
            {
                var heroToMove = rearCombatHeroes[0];
                rearCombatHeroes.RemoveAt(0);

                int srcCityId = heroToMove.cityId;
                var srcCity = GameManager.Instance.GetCity(srcCityId);

                if (rearCityHeroMap.ContainsKey(srcCityId) && 
                    rearCityHeroMap[srcCityId].Count > AIConst.AIHero.MIN_REAR_HEROES)
                {
                    force.MoveHeroToCity(srcCityId, cityId, new int[] { heroToMove.heroId });
                    rearCityHeroMap[srcCityId].Remove(heroToMove);

                    GameLog.SetTag("AI").Info($"AI调度: 英雄{heroToMove.heroId}从后方城市{srcCityId}调往前线城市{cityId}");
                }
            }
        }
    }

    #region 英雄均衡调度

    /// <summary>
    /// 每个城市尽量保证一个名将：从名将最多的城市调一个给无名将的城市
    /// </summary>
    public static void EnsureMinStarHeroPerCity(SaveForceData force)
    {
        var cities = force.GetCityList();
        if (cities.Count < 2) return;

        var cityStarCounts = new Dictionary<int, int>();
        foreach (var city in cities)
        {
            int count = city.GetNormalHeroList()
                .Count(hid => HeroConfig.GetConfig(hid).StarHero);
            cityStarCounts[city.cityId] = count;
        }

        var citiesWithoutStar = cities.Where(c => cityStarCounts[c.cityId] == 0).ToList();
        var citiesWithExtraStar = cities.Where(c => cityStarCounts[c.cityId] > 1)
            .OrderByDescending(c => cityStarCounts[c.cityId]).ToList();

        foreach (var poor in citiesWithoutStar)
        {
            if (citiesWithExtraStar.Count == 0) break;

            var rich = citiesWithExtraStar[0];
            var starHeroIds = rich.GetNormalHeroList()
                .Where(hid => HeroConfig.GetConfig(hid).StarHero).ToList();
            if (starHeroIds.Count == 0) continue;

            int heroId = starHeroIds[0];
            force.MoveHeroToCity(rich.cityId, poor.cityId, new int[] { heroId });

            cityStarCounts[rich.cityId]--;
            cityStarCounts[poor.cityId]++;
            GameLog.SetTag("AI").Info($"均衡(名将保障): 英雄{heroId}从城市{rich.cityId}调往城市{poor.cityId}");

            if (cityStarCounts[rich.cityId] <= 1)
                citiesWithExtraStar.RemoveAt(0);
        }
    }

    /// <summary>
    /// 名将均衡：对只含名将的Total计分排序，前30% vs 后30%，首位>2倍则移动英雄
    /// </summary>
    public static void BalanceStarHeroes(SaveForceData force)
    {
        DoHeroBalance(force, onlyStarHero: true, thresholdRatio: 2f, loop: false);
    }

    /// <summary>
    /// 全英雄均衡：对所有英雄的Total计分排序，循环直到首位<2.5倍
    /// </summary>
    public static void BalanceAllHeroes(SaveForceData force)
    {
        DoHeroBalance(force, onlyStarHero: false, thresholdRatio: 2.5f, loop: true);
    }

    #endregion

    #region 公用函数

    /// <summary>
    /// 计算城市中符合条件的英雄的HeroConfig.Total总分
    /// </summary>
    private static float CalculateCityHeroScore(SaveCityData city, bool onlyStarHero)
    {
        float total = 0;
        var heroIds = city.GetNormalHeroList();
        foreach (var heroId in heroIds)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (onlyStarHero && !heroCfg.StarHero) continue;
            total += heroCfg.Total;
        }
        return total;
    }

    /// <summary>
    /// 按城市英雄分数降序排列（包含所有城市，分数为0的也在内）
    /// </summary>
    private static List<(int cityId, float score)> BuildCityScoreRanking(SaveForceData force, bool onlyStarHero)
    {
        var cities = force.GetCityList();
        var scores = new List<(int cityId, float score)>();
        foreach (var city in cities)
        {
            float score = CalculateCityHeroScore(city, onlyStarHero);
            scores.Add((city.cityId, score));
        }
        scores.Sort((a, b) => b.score.CompareTo(a.score));
        return scores;
    }

    /// <summary>
    /// 从排序列表中分离前30%和后30%城市组
    /// </summary>
    private static (List<(int cityId, float score)> topGroup, List<(int cityId, float score)> bottomGroup)
        SplitTopBottomGroups(List<(int cityId, float score)> ranking)
    {
        if (ranking.Count < 2)
            return (new List<(int, float)>(), new List<(int, float)>());

        int groupSize = Math.Max(1, (int)Math.Ceiling(ranking.Count * 0.3f));
        var topGroup = ranking.Take(groupSize).ToList();
        var bottomGroup = ranking.Skip(ranking.Count - groupSize).ToList();
        return (topGroup, bottomGroup);
    }

    /// <summary>
    /// 从源城市向目标城市移动随机英雄（含troop主将逻辑）
    /// </summary>
    private static bool TryMoveHeroForBalance(SaveForceData force, int fromCityId, int toCityId, bool onlyStarHero)
    {
        var srcCity = GameManager.Instance.GetCity(fromCityId);
        var srcHeroIds = srcCity.GetNormalHeroList();
        if (srcHeroIds.Count <= AIConst.AIHero.MIN_REAR_HEROES)
        {
            GameLog.SetTag("AI").Debug($"均衡: 源城{ConfigNameHelper.GetCityName(fromCityId)}英雄数{srcHeroIds.Count}不足，跳过");
            return false;
        }

        var candidates = onlyStarHero
            ? srcHeroIds.Where(hid => HeroConfig.GetConfig(hid).StarHero).ToList()
            : new List<int>(srcHeroIds);

        if (candidates.Count == 0)
        {
            GameLog.SetTag("AI").Debug($"均衡: 源城{ConfigNameHelper.GetCityName(fromCityId)}无符合条件的候补英雄({(onlyStarHero ? "名将" : "全部")})");
            return false;
        }

        int pickIndex = SysRandom.Range(0, candidates.Count);
        int pickedHeroId = candidates[pickIndex];

        var heroIdsToMove = new List<int> { pickedHeroId };

        int troopCount = SaveTroopsData.GetTroopsCountByCity(fromCityId);
        bool isCommander = SaveTroopsData.IsHeroCommander(pickedHeroId, fromCityId);

        if (isCommander && troopCount > 3)
        {
            var troop = SaveTroopsData.FindByHeroId(pickedHeroId);
            if (troop != null)
            {
                if (troop.heroId2 > 0) heroIdsToMove.Add(troop.heroId2);
                if (troop.heroId3 > 0) heroIdsToMove.Add(troop.heroId3);
                var troopsToMove = new List<SaveTroopsData> { troop };
                SaveTroopsData.MoveTroopsToCity(troopsToMove, toCityId);
            }
        }

        // 确保源城市保留足够英雄
        if (srcHeroIds.Count - heroIdsToMove.Count < AIConst.AIHero.MIN_REAR_HEROES)
        {
            GameLog.SetTag("AI").Debug($"均衡: 源城{ConfigNameHelper.GetCityName(fromCityId)}移动{heroIdsToMove.Count}个英雄后会不足{AIConst.AIHero.MIN_REAR_HEROES}个，跳过");
            return false;
        }

        force.MoveHeroToCity(fromCityId, toCityId, heroIdsToMove.ToArray());

        string tag = isCommander && troopCount > 3 ? "（整troop）" : "";
        GameLog.SetTag("AI").Info($"均衡调度{tag}: 英雄[{string.Join(",", heroIdsToMove)}]从城市{fromCityId}调往城市{toCityId}");
        return true;
    }

    /// <summary>
    /// 英雄均衡核心逻辑：计分→排序→取前后组→首位比较→移动
    /// </summary>
    private static void DoHeroBalance(SaveForceData force, bool onlyStarHero, float thresholdRatio, bool loop)
    {
        var cities = force.GetCityList();
        if (cities.Count < 2) return;

        string tag = onlyStarHero ? "名将" : "全英雄";
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 开始{tag}均衡 (阈值{thresholdRatio}x)");

        var ranking = BuildCityScoreRanking(force, onlyStarHero);
        if (ranking.Count < 2)
        {
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} {tag}均衡跳过: 参与城市不足2个");
            return;
        }

        do
        {
            var (topGroup, bottomGroup) = SplitTopBottomGroups(ranking);
            if (topGroup.Count == 0 || bottomGroup.Count == 0) break;

            var top = topGroup[0];
            var bottom = bottomGroup[0];

            float ratio = bottom.score > 0 ? top.score / bottom.score : float.MaxValue;
            GameLog.SetTag("AI").Debug($"{tag}均衡: 首城={ConfigNameHelper.GetCityName(top.cityId)}({top.score}) vs 尾城={ConfigNameHelper.GetCityName(bottom.cityId)}({bottom.score}), 比率={ratio:F2}");

            if (top.score <= bottom.score * thresholdRatio) break;

            bool moved = TryMoveHeroForBalance(force, top.cityId, bottom.cityId, onlyStarHero);
            if (!moved)
            {
                // 首位不能移，尝试下一个top城市
                bool found = false;
                for (int i = 1; i < topGroup.Count; i++)
                {
                    var nextTop = topGroup[i];
                    if (TryMoveHeroForBalance(force, nextTop.cityId, bottom.cityId, onlyStarHero))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) break;
            }

            ranking = BuildCityScoreRanking(force, onlyStarHero);
            if (ranking.Count < 2) break;
        }
        while (loop);

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} {tag}均衡完成");
    }

    #endregion
}
