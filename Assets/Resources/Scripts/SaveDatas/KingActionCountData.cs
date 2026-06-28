[System.Serializable]
public class KingActionCountData
{
    public int devId;
    public int count;

    public KingActionCountData()
    {
        devId = 0;
        count = 0;
    }

    public KingActionCountData(int devId, int count)
    {
        this.devId = devId;
        this.count = count;
    }
}
