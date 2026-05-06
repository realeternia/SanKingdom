using System;

[Serializable]
public class WarTeamData
{
    public int heroId1;
    public int heroId2;
    public int heroId3;
    public int armsId;
    public int targetCityId;

    public WarTeamData()
    {
        heroId1 = 0;
        heroId2 = 0;
        heroId3 = 0;
        armsId = 0;
        targetCityId = 0;
    }

    public WarTeamData(int heroId1, int heroId2, int heroId3, int armsId, int targetCityId)
    {
        this.heroId1 = heroId1;
        this.heroId2 = heroId2;
        this.heroId3 = heroId3;
        this.armsId = armsId;
        this.targetCityId = targetCityId;
    }
}
