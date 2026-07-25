using System.Collections.Generic;
using System.Linq;
using CommonConfig;

[System.Serializable]
public class SaveData
{
    public List<SaveForceData> forces = new List<SaveForceData>();
    public List<SaveCityData> cities = new List<SaveCityData>();
    public List<SaveHeroData> heros = new List<SaveHeroData>();
    public List<SaveTroopsData> troops = new List<SaveTroopsData>();
    public BattleStatManager battleStatManager = new BattleStatManager();
    public int round;
    public int currentForceIndex;
    public SaveForceRelation forceRelation = new SaveForceRelation();

    public void OnNewGame(int playerForceId)
    {
        round = 1;

        foreach (var cityCfg in WorldConfig.ConfigList)
        {
            var city = new SaveCityData();
            city.cityId = cityCfg.Id;
            city.forceId = cityCfg.ForceId;
            city.exp = SaveCityData.GetExpByLevel(cityCfg.Level);
            city.soldier = cityCfg.Soldier;
            city.happy = SystemConst.City.INITIAL_CITY_HAPPY;
            city.food = cityCfg.Food;
            city.wall = cityCfg.Wall;
            cities.Add(city);
        }

        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            if (string.IsNullOrEmpty(heroCfg.City))
                continue;
            var cityCfg = WorldConfig.ConfigList.FirstOrDefault(c => c.Cname == heroCfg.City);
            if (cityCfg == null)
                continue;
            if (SystemConst.Game.BASE_YEAR - heroCfg.BornYear < SystemConst.Game.BORN_AGE)
                continue;

            var hero = new SaveHeroData { heroId = heroCfg.Id, cityId = cityCfg.Id, state = HeroState.Normal, loyalty = heroCfg.Loyal, forceId = cityCfg.ForceId };
            hero.InitAttrsFromConfig();
            heros.Add(hero);
        }

        foreach (var city in cities)
        {
            city.SelectOwner();
        }

        foreach (var force in ForceConfig.ConfigList)
        {
            if (force.Id > SystemConst.Game.MAX_FORCE_ID)
                continue;
            var forceData = new SaveForceData { forceId = force.Id, gold = force.InitGold };
            if (force.Id == playerForceId)
                forceData.isPlayer = true;
            forceData.InitRuntimeState();
            forces.Add(forceData);
        }

        forceRelation.InitForNewGame();
        SortForces();

        foreach (var forceData in forces)
            forceData.ResetRoundState();
        currentForceIndex = 0;
    }

    public void InitLoadedData()
    {
        SortForces();
        foreach (var forceData in forces)
            forceData.InitRuntimeState();
    }

    public void BeforeSave()
    {
        CleanupTroopsWithoutCommander();
    }

    public void OnRound()
    {
        round++;
        forceRelation.OnRound();
        CleanupTroopsWithoutCommander();

        foreach (var city in cities)
        {
            city.OnRound();
        }

        ProcessHeros();

        foreach (var forceData in forces)
            forceData.ResetRoundState();

        currentForceIndex = 0;
        SortForces();
    }

    private void SortForces()
    {
        forces.Sort((a, b) =>
        {
            if (a.isPlayer != b.isPlayer)
                return a.isPlayer ? -1 : 1;
            return a.forceId - b.forceId;
        });
    }

    private void ProcessHeros()
    {
        foreach (var hero in heros)
        {
            if (hero.state == HeroState.Catched)
            {
                // 主公忠心不会下降
                if (!IsKingHero(hero))
                {
                    int loyaltyOld = hero.loyalty;
                    hero.loyalty -= SysFormula.Hero.GetSysConfigModifyResult("CapturedLoyaltyDecay", hero.forceId);
                    if (hero.loyalty < 0)
                        hero.loyalty = 0;
                    int actualReduce = loyaltyOld - hero.loyalty;

                    if (actualReduce > 0)
                    {
                        GameManager.Instance.GameEventLog?.RecordEvent(GameEventData.CreateLoyaltyChange(
                            round, hero.forceId, hero.cityId, hero.heroId, -actualReduce, 1));
                    }
                }

                hero.TryEscape(round);
            }
            else if (hero.state == HeroState.Wild)
            {
                hero.TryWildMove();
            }
        }
    }

    /// <summary>
    /// 判断武将是否是主公
    /// </summary>
    private static bool IsKingHero(SaveHeroData hero)
    {
        if (hero.forceId <= 0) return false;
        var forceCfg = ForceConfig.GetConfig(hero.forceId);
        return forceCfg != null && forceCfg.HeroId == hero.heroId;
    }

    private void CleanupTroopsWithoutCommander()
    {
        troops.RemoveAll(t => t.heroId1 <= 0);
    }
}
