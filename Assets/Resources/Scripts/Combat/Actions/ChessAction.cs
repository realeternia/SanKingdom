[System.Serializable]
public class ChessAction
{   
    public int SourceId;
    public int Tick;

    public ChessAction(int sourceId, int tick)
    {
        SourceId = sourceId;
        Tick = tick;
    }

    public virtual void Doing()
    {
    }
}