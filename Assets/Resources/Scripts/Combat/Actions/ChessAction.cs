[System.Serializable]
public class ChessAction
{   
    public int SourceId;
    public int Tick;

    public virtual void Doing(Chess chess)
    {
    }
}