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

    /// <summary>
    /// 每城市分配空闲英雄生产资源（木/铁/马），每种最多1人，先清理旧派遣再重新分配
    /// </summary>
    protected void AssignResProduction()
    {
        // 收集该城市可用的资源生产配置（木/铁/马），剔除不可用的
        var availableResConfigs = new List<CityDevConfig>();
        foreach (var resType in new[] { "wood", "steel", "horse" })
        {
            var cfg = FindResDevConfig(resType);
            if (cfg == null) continue;
            if (!SaveCityData.IsDevAvailableForCity(City.cityId, cfg)) continue;
            availableResConfigs.Add(cfg);
        }
        if (availableResConfigs.Count == 0) return;

        var resDevIdSet = new HashSet<int>(availableResConfigs.Select(c => c.Id));
        int currentResCount = City.GetDevAssignments().Count(a => resDevIdSet.Contains(a.devId));

        // 需要重洗的条件：1.当前无资源派遣（空数据/易手） 2.按城市ID错峰定期重洗
        int interval = AIConst.AIStrategy.TROOP_RES_RESHUFFLE_INTERVAL;
        bool needReshuffle = currentResCount == 0
            || GameManager.Instance.SaveData.round % interval == (City.cityId % interval);

        if (!needReshuffle) return;

        var normalHeroes = City.GetNormalHeroList();
        var assignedHeroIds = new HashSet<int>(City.GetDevAssignments().Select(a => a.heroId));
        var idleHeroes = normalHeroes.Where(id => !assignedHeroIds.Contains(id)).ToList();
        if (idleHeroes.Count == 0) return;

        // 先清理旧的木/铁/马派遣（防止换位置残留）
        var toRemove = City.GetDevAssignments()
            .Where(a => resDevIdSet.Contains(a.devId))
            .Select(a => a.heroId)
            .ToList();
        foreach (var heroId in toRemove)
        {
            City.RemoveDevAssignment(heroId);
        }

        // 刷新空闲英雄（清理后可能有英雄释放出来）
        idleHeroes = normalHeroes
            .Where(id => !City.GetDevAssignments().Any(a => a.heroId == id))
            .ToList();

        // 遵守城市格子上限
        var levelCfg = CityLevelConfig.GetConfig(City.GetLevel());
        int maxJobCount = levelCfg.JobCount;
        int currentAssignments = City.GetDevAssignments().Count;
        int remainingSlots = maxJobCount - currentAssignments;
        if (remainingSlots <= 0) return;

        int toAssign = Math.Min(Math.Min(idleHeroes.Count, availableResConfigs.Count), remainingSlots);
        if (toAssign == 0) return;

        // 随机打乱可用资源类型，每种最多1人
        var shuffled = availableResConfigs.OrderBy(_ => SysRandom.Range(0, 100)).ToList();
        for (int i = 0; i < toAssign; i++)
        {
            var cfg = shuffled[i];
            int heroId = idleHeroes[i];
            City.SetDevAssignment(heroId, cfg.Id);
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 资源生产派遣 {ConfigNameHelper.GetHeroName(heroId)}→{cfg.Cname}({cfg.DevAttr1})");
        }
    }

    protected void FormTroops()
    {
        var normalHeroes = City.GetNormalHeroList();
        if (normalHeroes.Count == 0) return;

        // 新回合重新计算军团，先清理旧军团
        SaveTroopsData.RemoveAllTroopsByCity(City.cityId);

        var allHeroes = normalHeroes
            .Select(id => GameManager.Instance.GetHero(id))
            .Where(h => h != null)
            .ToList();

        // 筛选高统帅主将（非内政英雄且统帅≥阈值）
        var commanders = allHeroes
            .Where(h => SysFormula.Hero.ClassifyHero(h) != HeroType.Domestic
                      && h.GetAttr("leadship") >= AIConst.AIStrategy.TROOP_COMMANDER_LEADSHIP_THRESHOLD)
            .OrderByDescending(h => h.GetAttr("leadship"))
            .ToList();

        if (commanders.Count == 0) return;

        var viceHeroes = allHeroes
            .Where(h => !commanders.Any(c => c.heroId == h.heroId))
            .OrderByDescending(h => h.GetAttr("inte") + h.GetAttr("charm"))
            .ToList();

        int citySoldier = (int)Math.Floor(City.GetAttr("soldier"));

        int troopLimit = AIFormula.CalculateTroopLimit(commanders.Count, normalHeroes.Count, citySoldier);

        int newTroopCount = Math.Min(troopLimit, commanders.Count);

        if (newTroopCount == 0) return;

        var usedHeroIds = new HashSet<int>();
        int formedCount = 0;
        var troopSummaries = new List<string>();

        // 按智力排序，弓兵优先：高智力主将先尝试弓兵，资源耗尽后切近战
        var commandersByInt = commanders.OrderByDescending(h => h.GetAttr("inte")).ToList();

        for (int i = 0; i < newTroopCount; i++)
        {
            var commander = commandersByInt[i];
            usedHeroIds.Add(commander.heroId);

            var troop = new SaveTroopsData();
            troop.heroId1 = commander.heroId;

            // 先尝试弓兵，不行则走正常近战选择
            int bowArmsId = TrySelectBow(commander);
            troop.armsId = bowArmsId > 0 ? bowArmsId : SelectBestArmsForCommander(commander);

            int viceCount = 0;
            var viceNames = new List<string>();
            foreach (var viceHero in viceHeroes)
            {
                if (viceCount >= AIConst.AIStrategy.TROOP_MAX_HEROES - 1) break;
                if (usedHeroIds.Contains(viceHero.heroId)) continue;

                if (viceCount == 0)
                    troop.heroId2 = viceHero.heroId;
                else if (viceCount == 1)
                    troop.heroId3 = viceHero.heroId;
                usedHeroIds.Add(viceHero.heroId);
                viceNames.Add(ConfigNameHelper.GetHeroName(viceHero.heroId));
                viceCount++;
            }

            SaveTroopsData.AddTroopToCity(troop, City.cityId);
            Force.RecalculateResUsed();
            formedCount++;

            var armsName = ArmsConfig.GetConfig(troop.armsId)?.NameS ?? "未知";
            troopSummaries.Add($"  {ConfigNameHelper.GetHeroName(commander.heroId)}({armsName})" + (viceNames.Count > 0 ? $" 副将[{string.Join(",", viceNames)}]" : ""));
        }

        if (formedCount > 0)
        {
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 组建{formedCount}个军团(高统帅主将{commanders.Count} 武将{normalHeroes.Count} 士兵{citySoldier})");
            
            // 打印配兵和采集派遣汇总
            var devAssignments = City.GetDevAssignments();
            var devSummary = new List<string>();
            foreach (var a in devAssignments)
            {
                var devName = CityDevConfig.GetConfig(a.devId)?.Cname ?? a.devId.ToString();
                devSummary.Add($"{ConfigNameHelper.GetHeroName(a.heroId)}→{devName}");
            }
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 配兵汇总:\n{string.Join("\n", troopSummaries)}");
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 采集派遣: {(devSummary.Count > 0 ? string.Join(", ", devSummary) : "无")}");
        }

        // 编队后检查过剩资源生产，释放去干别的
        ReassignExcessResProduction();
    }

    /// <summary>
    /// 编队后检查，若有资源生产过剩（木/铁/马未被军团消耗），释放英雄去干普通内政
    /// </summary>
    private void ReassignExcessResProduction()
    {
        var resDevIdToType = new Dictionary<int, string>();
        foreach (var resType in new[] { "wood", "steel", "horse" })
        {
            var cfg = FindResDevConfig(resType);
            if (cfg != null)
                resDevIdToType[cfg.Id] = resType;
        }
        if (resDevIdToType.Count == 0) return;

        var assignments = City.GetDevAssignments().ToList();
        bool anyRemoved = false;
        foreach (var a in assignments)
        {
            if (resDevIdToType.TryGetValue(a.devId, out var resType))
            {
                float used = Force.GetResUsed(resType);
                float available = Force.GetAttr(resType);
                if (available > used + 0.5f)
                {
                    City.RemoveDevAssignment(a.heroId);
                    anyRemoved = true;
                    GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 资源{resType}过剩(可用{available} 消耗{used})，释放{ConfigNameHelper.GetHeroName(a.heroId)}去干别的");
                }
            }
        }

        if (anyRemoved)
            AssignHeroesToDev();
    }

    /// <summary>
    /// 尝试为主将分配弓兵，按智力适配排序，选第一个能负担的
    /// </summary>
    private int TrySelectBow(SaveHeroData commander)
    {
        float str = commander.GetAttr("str");
        float leadship = commander.GetAttr("leadship");
        float inte = commander.GetAttr("inte");

        // 弓兵候选（按适配分排序）
        var bowCandidates = ArmsConfig.ConfigList
            .Where(a => a.CanAssign && a.Type == ArmsType.SodBow && a.Id > SystemConst.Hero.DEFAULT_ARMS_ID)
            .Select(a => new { Arms = a, Score = CalculateArmsFitScore(a, str, leadship, inte) })
            .OrderByDescending(x => x.Score)
            .ToList();

        if (bowCandidates.Count == 0) return 0;
        var bestBow = bowCandidates[0];

        // 近战最佳候选（骑兵/步兵），用于比较
        var meleeCandidates = ArmsConfig.ConfigList
            .Where(a => a.CanAssign && a.Type != ArmsType.SodBow && a.Id > SystemConst.Hero.DEFAULT_ARMS_ID)
            .Select(a => new { Arms = a, Score = CalculateArmsFitScore(a, str, leadship, inte) })
            .OrderByDescending(x => x.Score)
            .ToList();

        float bestMeleeScore = meleeCandidates.Count > 0 ? meleeCandidates[0].Score : 0;

        // 弓兵适配分必须高于近战最佳适配分，才给弓兵；否则留给近战
        if (bestBow.Score <= bestMeleeScore)
        {
            GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 主将{ConfigNameHelper.GetHeroName(commander.heroId)}(智{inte}) 弓兵适配{bestBow.Score:F1}≤近战{bestMeleeScore:F1}，跳过弓兵");
            return 0;
        }

        // 检查能否负担弓兵
        if (!Force.CanAffordArms(bestBow.Arms.Id))
        {
            GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 主将{ConfigNameHelper.GetHeroName(commander.heroId)}(智{inte}) 弓兵{bestBow.Arms.NameS}不可负担，跳过弓兵");
            return 0;
        }

        GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 主将{ConfigNameHelper.GetHeroName(commander.heroId)}(智{inte}) 弓兵适配{bestBow.Score:F1}>{bestMeleeScore:F1} 优先弓兵 {bestBow.Arms.NameS}(可负担)");
        return bestBow.Arms.Id;
    }

    /// <summary>
    /// 根据主将属性选择最强适配兵种，优先选势力能负担的，否则降级为动员兵
    /// </summary>
    private int SelectBestArmsForCommander(SaveHeroData commander)
    {
        float str = commander.GetAttr("str");
        float leadship = commander.GetAttr("leadship");
        float inte = commander.GetAttr("inte");

        // 按主将属性适配度评分所有非动员兵兵种
        var candidates = ArmsConfig.ConfigList
            .Where(a => a.CanAssign && a.Id > SystemConst.Hero.DEFAULT_ARMS_ID)
            .Select(a => new
            {
                Arms = a,
                Score = CalculateArmsFitScore(a, str, leadship, inte)
            })
            .OrderByDescending(x => x.Score)
            .ToList();

        GameLog.SetTag("AI").Debug($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 主将{ConfigNameHelper.GetHeroName(commander.heroId)}(武{str} 统{leadship} 智{inte}) 兵种候选: {string.Join(", ", candidates.Select(c => $"{c.Arms.NameS}(分{c.Score:F1})"))}");

        // 优先选势力能负担的
        foreach (var candidate in candidates)
        {
            if (Force.CanAffordArms(candidate.Arms.Id))
            {
                GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 主将{ConfigNameHelper.GetHeroName(commander.heroId)} 分配兵种 {candidate.Arms.NameS}(可负担)");
                return candidate.Arms.Id;
            }
        }

        // 都负担不起，留动员兵
        GameLog.SetTag("AI").Warn($"{ConfigNameHelper.GetForceName(Force.forceId)} - [{ConfigNameHelper.GetCityName(City.cityId)}] 主将{ConfigNameHelper.GetHeroName(commander.heroId)} 所有兵种资源不足，降级为动员兵");
        return SystemConst.Hero.DEFAULT_ARMS_ID;
    }

    /// <summary>
    /// 计算兵种与主将属性的适配分
    /// 骑兵：统帅主导；弓兵：智力主导；步兵(刀/枪/戟)：武力主导，枪/戟优先并随机
    /// </summary>
    private float CalculateArmsFitScore(ArmsConfig arms, float str, float leadship, float inte)
    {
        float attrScore = 0;

        switch (arms.Type)
        {
            case ArmsType.SodHorse:
                attrScore = leadship * AIConst.AIStrategy.ARMS_FIT_HORSE_WEIGHT
                          + str * AIConst.AIStrategy.ARMS_FIT_HORSE_STR_WEIGHT;
                break;
            case ArmsType.SodBow:
                attrScore = inte * AIConst.AIStrategy.ARMS_FIT_BOW_WEIGHT;
                break;
            case ArmsType.SodWalk:
                attrScore = str * AIConst.AIStrategy.ARMS_FIT_WALK_WEIGHT
                          + leadship * AIConst.AIStrategy.ARMS_FIT_WALK_LEADSHIP_WEIGHT;
                break;
        }

        // 基础分微调（枪/戟>刀），步兵类型加随机扰动避免枪戟始终同分
        float baseBonus = (arms.Atk + arms.Def) * AIConst.AIStrategy.ARMS_BASE_STAT_WEIGHT;
        float jitter = arms.Type == ArmsType.SodWalk ? SysRandom.Range(0, 100) * 0.0005f : 0f;

        return attrScore + baseBonus + jitter;
    }

    /// <summary>
    /// 查找资源生产对应的CityDevConfig
    /// </summary>
    private CityDevConfig FindResDevConfig(string resType)
    {
        return CityDevConfig.ConfigList
            .FirstOrDefault(c => c.Type == "normal"
                && !string.IsNullOrEmpty(c.DevAttr1)
                && c.DevAttr1.ToLower() == resType);
    }
}
