using System.Collections.Generic;

[System.Serializable]
public class DevAssignmentData
{
    public int heroId;
    public int devId;

    public DevAssignmentData()
    {
        heroId = 0;
        devId = 0;
    }

    public DevAssignmentData(int heroId, int devId)
    {
        this.heroId = heroId;
        this.devId = devId;
    }
}
