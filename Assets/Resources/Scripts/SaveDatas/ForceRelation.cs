using System;
using System.Collections.Generic;
using CommonConfig;

public enum RelationLevel
{
    Friendly,
    Neutral,
    Hostile
}

[System.Serializable]
public class SaveForceRelationEntry
{
    public int forceId1;
    public int forceId2;
    public int score;

    public SaveForceRelationEntry() { }

    public SaveForceRelationEntry(int forceId1, int forceId2, int score)
    {
        this.forceId1 = forceId1;
        this.forceId2 = forceId2;
        this.score = score;
    }
}

[System.Serializable]
public class SaveForceRelation
{
    public List<SaveForceRelationEntry> relations = new List<SaveForceRelationEntry>();

    [NonSerialized]
    private HashSet<int> battlePairs = new HashSet<int>();

    private static readonly int[,] initialRelations = new int[,]
    {
        {1, 2, 15}, {1, 3, 75}, {1, 4, 40}, {1, 5, 30}, {1, 6, 55}, {1, 7, 70}, {1, 8, 65}, {1, 9, 50}, {1, 10, 30}, {1, 11, 45}, {1, 12, 50},
        {2, 3, 30}, {2, 4, 20}, {2, 5, 25}, {2, 6, 35}, {2, 7, 40}, {2, 8, 45}, {2, 9, 50}, {2, 10, 20}, {2, 11, 30}, {2, 12, 40},
        {3, 4, 40}, {3, 5, 30}, {3, 6, 45}, {3, 7, 25}, {3, 8, 50}, {3, 9, 50}, {3, 10, 35}, {3, 11, 40}, {3, 12, 45},
        {4, 5, 30}, {4, 6, 50}, {4, 7, 55}, {4, 8, 50}, {4, 9, 50}, {4, 10, 15}, {4, 11, 70}, {4, 12, 55},
        {5, 6, 20}, {5, 7, 30}, {5, 8, 40}, {5, 9, 45}, {5, 10, 35}, {5, 11, 25}, {5, 12, 40},
        {6, 7, 45}, {6, 8, 50}, {6, 9, 50}, {6, 10, 40}, {6, 11, 50}, {6, 12, 50},
        {7, 8, 70}, {7, 9, 55}, {7, 10, 30}, {7, 11, 45}, {7, 12, 50},
        {8, 9, 55}, {8, 10, 40}, {8, 11, 50}, {8, 12, 50},
        {9, 10, 40}, {9, 11, 50}, {9, 12, 50},
        {10, 11, 30}, {10, 12, 40},
        {11, 12, 65}
    };

    public void InitForNewGame()
    {
        relations.Clear();
        battlePairs = new HashSet<int>();
        for (int i = 0; i < initialRelations.GetLength(0); i++)
        {
            int fid1 = initialRelations[i, 0];
            int fid2 = initialRelations[i, 1];
            int score = initialRelations[i, 2];
            relations.Add(new SaveForceRelationEntry(fid1, fid2, score));
        }
    }

    private SaveForceRelationEntry FindEntry(int forceId1, int forceId2)
    {
        int minId = Math.Min(forceId1, forceId2);
        int maxId = Math.Max(forceId1, forceId2);
        foreach (var entry in relations)
        {
            if (entry.forceId1 == minId && entry.forceId2 == maxId)
                return entry;
        }
        return null;
    }

    public int GetRelation(int forceId1, int forceId2)
    {
        if (forceId1 == forceId2) return SystemConst.Diplomacy.RELATION_MAX;
        var entry = FindEntry(forceId1, forceId2);
        if (entry != null)
            return entry.score;
        return SystemConst.Diplomacy.RELATION_DEFAULT;
    }

    public RelationLevel GetRelationLevel(int forceId1, int forceId2)
    {
        int score = GetRelation(forceId1, forceId2);
        if (score >= SystemConst.Diplomacy.RELATION_FRIENDLY_THRESHOLD)
            return RelationLevel.Friendly;
        if (score <= SystemConst.Diplomacy.RELATION_HOSTILE_THRESHOLD)
            return RelationLevel.Hostile;
        return RelationLevel.Neutral;
    }

    public void AddRelation(int forceId1, int forceId2, int delta)
    {
        if (forceId1 == forceId2) return;

        RelationLevel oldLevel = GetRelationLevel(forceId1, forceId2);

        var entry = FindEntry(forceId1, forceId2);
        int newScore = (entry != null ? entry.score : SystemConst.Diplomacy.RELATION_DEFAULT) + delta;
        newScore = Math.Clamp(newScore, SystemConst.Diplomacy.RELATION_MIN, SystemConst.Diplomacy.RELATION_MAX);
        if (entry != null)
        {
            entry.score = newScore;
        }
        else
        {
            int minId = Math.Min(forceId1, forceId2);
            int maxId = Math.Max(forceId1, forceId2);
            relations.Add(new SaveForceRelationEntry(minId, maxId, newScore));
        }

        RelationLevel newLevel = GetRelationLevel(forceId1, forceId2);
        if (oldLevel != newLevel)
        {
            PanelManager.Instance.SendSignal(new RelationChangeSignal { ForceId1 = forceId1, ForceId2 = forceId2 });
        }
    }

    public void RecordBattle(int forceId1, int forceId2)
    {
        if (forceId1 == forceId2) return;
        if (battlePairs == null)
            battlePairs = new HashSet<int>();
        int key = Math.Min(forceId1, forceId2) * 100 + Math.Max(forceId1, forceId2);
        battlePairs.Add(key);
    }

    private bool HasBattle(int forceId1, int forceId2)
    {
        if (battlePairs == null) return false;
        int key = Math.Min(forceId1, forceId2) * 100 + Math.Max(forceId1, forceId2);
        return battlePairs.Contains(key);
    }

    public void OnRound()
    {
        if (battlePairs == null)
            battlePairs = new HashSet<int>();

        var forces = GameManager.Instance.SaveData.forces;
        for (int i = 0; i < forces.Count; i++)
        {
            for (int j = i + 1; j < forces.Count; j++)
            {
                var force1 = forces[i];
                var force2 = forces[j];

                if (force1.isEliminated || force2.isEliminated)
                    continue;

                if (HasBattle(force1.forceId, force2.forceId))
                {
                    int rise = SysFormula.Diplomacy.CalculateBattleRise();
                    AddRelation(force1.forceId, force2.forceId, rise);
                    GameLog.Info($"外交: {force1.Name}与{force2.Name}交战，关系+{rise}，当前{GetRelation(force1.forceId, force2.forceId)}");
                }
                else
                {
                    bool isAdjacent = MapTool.AreForcesAdjacent(force1.forceId, force2.forceId);
                    int decay = SysFormula.Diplomacy.CalculatePeaceDecay(isAdjacent);
                    AddRelation(force1.forceId, force2.forceId, -decay);
                    GameLog.Info($"外交: {force1.Name}与{force2.Name}和平{(isAdjacent ? "(相邻)" : "")}，关系-{decay}，当前{GetRelation(force1.forceId, force2.forceId)}");
                }
            }
        }

        battlePairs.Clear();
    }
}
