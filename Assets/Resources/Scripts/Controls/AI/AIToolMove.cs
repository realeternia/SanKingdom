using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;

/// <summary>
/// AI移动工具类
/// 前后线战斗英雄调度 → 名将保障 → 名将均衡 → 全英雄均衡
/// 迁移自 HeroDispatcher，加入 WarPlan 排除与 hero.round 可用性检查
/// </summary>
public static class AIToolMove
{
    /// <summary>
    /// 移动入口
    /// </summary>
    public static void Process(SaveForceData force, HashSet<int> excludedHeroIds)
    {
        int currentRound = GameManager.Instance.SaveData.round;

        FillHeroForSoldierCapacity(force, excludedHeroIds, currentRound);
        EnsureMinStarHeroPerCity(force, excludedHeroIds, currentRound);
        DoHeroBalance(force, onlyStarHero: true, thresholdRatio: 2f, loop: false, excludedHeroIds, currentRound);
        DoHeroBalance(force, onlyStarHero: false, thresholdRatio: 2.5f, loop: true, excludedHeroIds, currentRound);
    }

    /// <summary>
    /// 英雄是否可执行（未外出/未被占用）
    /// </summary>
    private static bool IsHeroAvailable(int heroId, int currentRound)
    {
        var hero = GameManager.Instance.GetHero(heroId);
        return hero != null && hero.round < currentRound;
    }

    /// <summary>
    /// 补充城市英雄：英雄数不足（< 士兵数/100）时，从其他城市拉非军团英雄，优先统帅高者
    /// </summary>
    private static void FillHeroForSoldierCapacity(SaveForceData force, HashSet<int> excludedHeroIds, int currentRound)
    {
        var cities = force.GetCityList();
        if (cities.Count < 2) return;

        foreach (var city in cities)
        {
            int heroCount = city.GetNormalHeroList().Count(hid => !excludedHeroIds.Contains(hid));
            int neededHeroes = (int)(city.soldier / AIConst.AIStrategy.MAX_SOLDIER_PER_HERO) - heroCount;
            if (neededHeroes <= 0) continue;

            int maxPull = Math.Min(neededHeroes, AIConst.AIHero.FILL_HERO_MAX_PULL);
            int pulled = 0;

            var candidates = new List<(int heroId, int leadShip, int srcCityId)>();
            foreach (var otherCity in cities)
            {
                if (otherCity.cityId == city.cityId) continue;

                foreach (var hid in otherCity.GetNormalHeroList())
                {
                    if (excludedHeroIds.Contains(hid)) continue;
                    if (!IsHeroAvailable(hid, currentRound)) continue;
                    if (SaveTroopsData.FindByHeroId(hid) != null) continue;

                    var hero = GameManager.Instance.GetHero(hid);
                    candidates.Add((hid, hero.leadShip, otherCity.cityId));
                }
            }

            candidates.Sort((a, b) => b.leadShip.CompareTo(a.leadShip));

            foreach (var (heroId, _, srcCityId) in candidates)
            {
                if (pulled >= maxPull) break;

                var srcCity = GameManager.Instance.GetCity(srcCityId);
                if (srcCity.GetNormalHeroList().Count <= AIConst.AIHero.MIN_REAR_HEROES) continue;

                force.MoveHeroToCity(srcCityId, city.cityId, new int[] { heroId });
                pulled++;

                GameLog.SetTag("AI").Info($"补充英雄: 英雄{heroId}从城市{ConfigNameHelper.GetCityName(srcCityId)}调往城市{ConfigNameHelper.GetCityName(city.cityId)}");
            }
        }
    }


    /// <summary>
    /// 每个城市尽量保证一个名将：从名将最多的城市调一个给无名将的城市
    /// </summary>
    private static void EnsureMinStarHeroPerCity(SaveForceData force, HashSet<int> excludedHeroIds, int currentRound)
    {
        var cities = force.GetCityList();
        if (cities.Count < 2) return;

        var cityStarCounts = new Dictionary<int, int>();
        foreach (var city in cities)
        {
            int count = city.GetNormalHeroList()
                .Count(hid => !excludedHeroIds.Contains(hid) && HeroConfig.GetConfig(hid).StarHero);
            cityStarCounts[city.cityId] = count;
        }

        var citiesWithoutStar = cities.Where(c => cityStarCounts[c.cityId] == 0).ToList();
        var citiesWithExtraStar = cities.Where(c => cityStarCounts[c.cityId] > 1)
            .OrderByDescending(c => cityStarCounts[c.cityId]).ToList();

        foreach (var poor in citiesWithoutStar)
        {
            if (citiesWithExtraStar.Count == 0) break;

            var rich = citiesWithExtraStar[0];
            // 只调可用（未外出）的名将
            var starHeroIds = rich.GetNormalHeroList()
                .Where(hid => !excludedHeroIds.Contains(hid)
                    && IsHeroAvailable(hid, currentRound)
                    && HeroConfig.GetConfig(hid).StarHero).ToList();
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
    /// 英雄均衡核心逻辑：计分→排序→取前后组→首位比较→移动
    /// </summary>
    private static void DoHeroBalance(SaveForceData force, bool onlyStarHero, float thresholdRatio, bool loop, HashSet<int> excludedHeroIds, int currentRound)
    {
        var cities = force.GetCityList();
        if (cities.Count < 2) return;

        string tag = onlyStarHero ? "名将" : "全英雄";
        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 开始{tag}均衡 (阈值{thresholdRatio}x)");

        var ranking = BuildCityScoreRanking(force, onlyStarHero, excludedHeroIds);
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

            bool moved = TryMoveHeroForBalance(force, top.cityId, bottom.cityId, onlyStarHero, excludedHeroIds, currentRound);
            if (!moved)
            {
                bool found = false;
                for (int i = 1; i < topGroup.Count; i++)
                {
                    var nextTop = topGroup[i];
                    if (TryMoveHeroForBalance(force, nextTop.cityId, bottom.cityId, onlyStarHero, excludedHeroIds, currentRound))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found) break;
            }

            ranking = BuildCityScoreRanking(force, onlyStarHero, excludedHeroIds);
            if (ranking.Count < 2) break;
        }
        while (loop);

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} {tag}均衡完成");
    }

    /// <summary>
    /// 计算城市中符合条件的英雄的HeroConfig.Total总分
    /// </summary>
    private static float CalculateCityHeroScore(SaveCityData city, bool onlyStarHero, HashSet<int> excludedHeroIds)
    {
        float total = 0;
        var heroIds = city.GetNormalHeroList();
        foreach (var heroId in heroIds)
        {
            if (excludedHeroIds.Contains(heroId)) continue;
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (onlyStarHero && !heroCfg.StarHero) continue;
            total += heroCfg.Total;
        }
        return total;
    }

    /// <summary>
    /// 按城市英雄分数降序排列
    /// </summary>
    private static List<(int cityId, float score)> BuildCityScoreRanking(SaveForceData force, bool onlyStarHero, HashSet<int> excludedHeroIds)
    {
        var cities = force.GetCityList();
        var scores = new List<(int cityId, float score)>();
        foreach (var city in cities)
        {
            float score = CalculateCityHeroScore(city, onlyStarHero, excludedHeroIds);
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
    private static bool TryMoveHeroForBalance(SaveForceData force, int fromCityId, int toCityId, bool onlyStarHero, HashSet<int> excludedHeroIds, int currentRound)
    {
        var srcCity = GameManager.Instance.GetCity(fromCityId);
        var srcHeroIds = srcCity.GetNormalHeroList();
        if (srcHeroIds.Count <= AIConst.AIHero.MIN_REAR_HEROES)
        {
            GameLog.SetTag("AI").Debug($"均衡: 源城{ConfigNameHelper.GetCityName(fromCityId)}英雄数{srcHeroIds.Count}不足，跳过");
            return false;
        }

        var candidates = srcHeroIds
            .Where(hid => !excludedHeroIds.Contains(hid))
            .Where(hid => IsHeroAvailable(hid, currentRound))
            .Where(hid => !onlyStarHero || HeroConfig.GetConfig(hid).StarHero)
            .ToList();

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
}
