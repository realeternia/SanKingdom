using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GameEventLog
{
    public List<GameEventData> events = new List<GameEventData>();
    public int nextEventId = 1;

    [System.NonSerialized]
    private Dictionary<int, DevSnapshotEntry> lastDevSnapshot = new Dictionary<int, DevSnapshotEntry>();

    [System.Serializable]
    public class DevSnapshotEntry
    {
        public int devId;
        public int cityId;
    }

    public void OnNewGame()
    {
        events.Clear();
        nextEventId = 1;
        lastDevSnapshot = BuildCurrentDevSnapshot();
        GameLog.Info("GameEventLog OnNewGame 初始 dev 快照大小=" + lastDevSnapshot.Count);
    }

    public void InitLoadedData()
    {
        if (lastDevSnapshot == null)
            lastDevSnapshot = new Dictionary<int, DevSnapshotEntry>();
        lastDevSnapshot = BuildCurrentDevSnapshot();
        GameLog.Info("GameEventLog InitLoadedData events=" + events.Count + " dev 快照大小=" + lastDevSnapshot.Count);
    }

    public void BeforeSave()
    {
    }

    public void OnRoundEnd(int finishedRound)
    {
        RecordDevDiff(finishedRound);
        ExpireOldEvents(finishedRound);
    }

    public void RecordEvent(GameEventData data)
    {
        if (data == null)
        {
            GameLog.Warn("GameEventLog RecordEvent data 为 null，跳过");
            return;
        }
        data.eventId = nextEventId++;
        events.Add(data);
    }

    private Dictionary<int, DevSnapshotEntry> BuildCurrentDevSnapshot()
    {
        var snap = new Dictionary<int, DevSnapshotEntry>();
        var saveData = GameManager.Instance?.SaveData;
        if (saveData == null)
            return snap;
        foreach (var city in saveData.cities)
        {
            foreach (var assign in city.GetDevAssignments())
            {
                snap[assign.heroId] = new DevSnapshotEntry { devId = assign.devId, cityId = city.cityId };
            }
        }
        return snap;
    }

    private void RecordDevDiff(int finishedRound)
    {
        if (lastDevSnapshot == null)
            lastDevSnapshot = new Dictionary<int, DevSnapshotEntry>();

        var current = BuildCurrentDevSnapshot();
        var saveData = GameManager.Instance?.SaveData;

        foreach (var kvp in current)
        {
            int heroId = kvp.Key;
            int newDevId = kvp.Value.devId;
            int cityId = kvp.Value.cityId;
            if (lastDevSnapshot.TryGetValue(heroId, out var last))
            {
                if (last.devId != newDevId)
                {
                    int forceId = ResolveForceId(saveData, heroId);
                    RecordEvent(GameEventData.CreateDev(finishedRound, forceId, cityId, heroId, newDevId, 2));
                }
            }
            else
            {
                int forceId = ResolveForceId(saveData, heroId);
                RecordEvent(GameEventData.CreateDev(finishedRound, forceId, cityId, heroId, newDevId, 0));
            }
        }

        foreach (var kvp in lastDevSnapshot)
        {
            int heroId = kvp.Key;
            if (!current.ContainsKey(heroId))
            {
                int forceId = ResolveForceId(saveData, heroId);
                RecordEvent(GameEventData.CreateDev(finishedRound, forceId, kvp.Value.cityId, heroId, 0, 1));
            }
        }

        lastDevSnapshot = current;
    }

    private static int ResolveForceId(SaveData saveData, int heroId)
    {
        if (saveData == null)
            return 0;
        var hero = saveData.heros.FirstOrDefault(h => h.heroId == heroId);
        return hero != null ? hero.forceId : 0;
    }

    private void ExpireOldEvents(int finishedRound)
    {
        int threshold = finishedRound - SystemConst.Game.SEASONS_PER_YEAR;
        int removed = 0;
        while (events.Count > 0 && events[0].round < threshold)
        {
            events.RemoveAt(0);
            removed++;
        }
        if (removed > 0)
            GameLog.Info("GameEventLog 过期清理移除 " + removed + " 条事件 (threshold round<" + threshold + ")");
    }
}
