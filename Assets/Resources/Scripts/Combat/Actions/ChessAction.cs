[System.Serializable]
public class ChessAction
{
    public int ActionId;
    public int SourceId;
    public float Time; // 执行时刻(秒)

    [System.NonSerialized]
    public bool done; // 是否已执行（不参与序列化，回放加载后全部重置为false重新执行）

    public ChessAction(int sourceId, float time)
    {
        SourceId = sourceId;
        Time = time;
    }

    public virtual void Doing()
    {
    }
}