using System;

[Serializable]
public class WarTroopsData
{
    public int heroId1;
    public int heroId2;
    public int heroId3;
    public int armsId;
    public int cityId;

    public WarTroopsData()
    {
        heroId1 = 0;
        heroId2 = 0;
        heroId3 = 0;
        armsId = 0;
        cityId = 0;
    }

    public WarTroopsData(int heroId1, int heroId2, int heroId3, int armsId, int cityId)
    {
        this.heroId1 = heroId1;
        this.heroId2 = heroId2;
        this.heroId3 = heroId3;
        this.armsId = armsId;
        this.cityId = cityId;
    }
}
