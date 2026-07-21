[System.Serializable]
public class TechProgressData
{
    public int techId;
    public int progress;

    public TechProgressData()
    {
        techId = 0;
        progress = 0;
    }

    public TechProgressData(int techId, int progress)
    {
        this.techId = techId;
        this.progress = progress;
    }
}
