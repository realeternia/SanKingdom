using System.Collections.Generic;
using CommonConfig;

[System.Serializable]
public class SaveData
{
    public List<SaveForceData> forces = new List<SaveForceData>();
    public List<SaveCityData> cities = new List<SaveCityData>();
    public List<SaveHeroData> heros = new List<SaveHeroData>();
    public BattleStatManager battleStatManager = new BattleStatManager();
    public int round;
    public int currentForceIndex;

    public void BeforeSave()
    {
        CleanupTroopsWithoutCommander();
    }

    public void OnRound()
    {
        CleanupTroopsWithoutCommander();

        foreach (var city in cities)
        {
            city.OnRound();
        }

        ProcessHeros();

        foreach (var forceData in forces)
            forceData.ResetRoundState();
    }

    private void ProcessHeros()
    {
        foreach (var hero in heros)
        {
            if (hero.state == HeroState.Catched)
            {
                hero.loyalty -= SysFormula.Hero.CalculateCapturedLoyaltyDecay();
                if (hero.loyalty < 0)
                    hero.loyalty = 0;

                var city = GameManager.Instance.GetCity(hero.cityId);
                if (SysFormula.Hero.CheckEscape())
                {
                    var destCityId = GameManager.Instance.GetRandomForceCityId(hero.cityId, hero.forceId);
                    if (destCityId > 0)
                    {
                        if (city != null)
                        {
                            city.RemoveDevAssignment(hero.heroId);
                        }
                        hero.state = HeroState.Normal;
                        hero.cityId = destCityId;
                    }
                }
            }
            else if (hero.state == HeroState.Wild)
            {
                if (SysFormula.Hero.CheckWildHeroMove())
                {
                    var randomCityId = MapTool.GetRandomAdjacentCityId(hero.cityId);
                    if (randomCityId != 0)
                    {
                        hero.cityId = randomCityId;
                    }
                }
            }
        }
    }

    private void CleanupTroopsWithoutCommander()
    {
        foreach (var city in cities)
        {
            city.troops.RemoveAll(t => t.heroId1 <= 0);
        }
    }
}
