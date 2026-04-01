using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class BattleStatManager
{
    [Serializable]
    public class BattleStat
    {
        public int forceId;
        public int heroId;
        public float damage;
        public float beDamaged;
        public bool isDead;
    }

    [Serializable]
    public class BattleRecord
    {
        public int battleId;
        public int cityId;
        public int forceId1;
        public int forceId2;
        public BattleResult result;
        public int rounds;
        public int year;
        public int soldierLoss1;
        public int soldierLoss2;
        public int foodCost1;
        public int foodCost2;
        public List<BattleStat> battleStats = new List<BattleStat>();
    }

    private const int MaxBattleCount = 20;
    public List<BattleRecord> battleRecords = new List<BattleRecord>();
    public int nextBattleId = 1000;
    
    [NonSerialized]
    private static BattleStatManager currentInstance;
    
    [NonSerialized]
    private Dictionary<int, BattleStat> currentBattleStats;
    
    [NonSerialized]
    private int currentBattleId;

    [NonSerialized]
    private bool isReplayMode;

    public int OnNewBattle()
    {
        currentInstance = this;
        currentBattleId = nextBattleId++;
        currentBattleStats = new Dictionary<int, BattleStat>();
        isReplayMode = false;
        return currentBattleId;
    }

    public void LoadBattleForReplay(int battleId)
    {
        currentInstance = this;
        currentBattleId = battleId;
        isReplayMode = true;
        
        var record = battleRecords.FirstOrDefault(r => r.battleId == battleId);
        if (record != null)
        {
            currentBattleStats = new Dictionary<int, BattleStat>();
            foreach (var stat in record.battleStats)
            {
                var uid = stat.forceId * 1000000 + stat.heroId;
                currentBattleStats[uid] = stat;
            }
        }
        else
        {
            currentBattleStats = null;
        }
    }

    public static void AddDamage(int forceId, int heroId, float damage)
    {
        if (currentInstance == null || currentInstance.currentBattleStats == null || currentInstance.isReplayMode)
            return;
            
        var battleStats = currentInstance.currentBattleStats;
        var uid = forceId * 1000000 + heroId;
        if (battleStats.TryGetValue(uid, out var battleStat))
        {
            battleStat.damage += damage;
        }
        else
        {
            var battleStat1 = new BattleStat
            {
                forceId = forceId,
                heroId = heroId,
                damage = damage,
            };
            battleStats.Add(uid, battleStat1);
        }
    }

    public static void AddBeDamaged(int forceId, int heroId, float damage)
    {
        if (currentInstance == null || currentInstance.currentBattleStats == null || currentInstance.isReplayMode)
            return;
            
        var battleStats = currentInstance.currentBattleStats;
        var uid = forceId * 1000000 + heroId;
        if (battleStats.TryGetValue(uid, out var battleStat))
        {
            battleStat.beDamaged += damage;
        }
        else
        {
            var battleStat1 = new BattleStat
            {
                forceId = forceId,
                heroId = heroId,
                beDamaged = damage,
            };
            battleStats.Add(uid, battleStat1);
        }
    }

    public static void SetHeroDead(int forceId, int heroId)
    {
        if (currentInstance == null || currentInstance.currentBattleStats == null || currentInstance.isReplayMode)
            return;
            
        var battleStats = currentInstance.currentBattleStats;
        var uid = forceId * 1000000 + heroId;
        if (battleStats.TryGetValue(uid, out var battleStat))
        {
            battleStat.isDead = true;
        }
        else
        {
            var battleStat1 = new BattleStat
            {
                forceId = forceId,
                heroId = heroId,
                isDead = true,
            };
            battleStats.Add(uid, battleStat1);
        }
    }

    public void SaveCurrentBattle(int cityId, int forceId1, int forceId2, BattleResult result, int rounds, int soldierLoss1, int soldierLoss2, int foodCost1, int foodCost2)
    {
        if (currentBattleStats == null || currentBattleId == 0 || isReplayMode)
            return;
            
        var record = new BattleRecord
        {
            battleId = currentBattleId,
            cityId = cityId,
            forceId1 = forceId1,
            forceId2 = forceId2,
            result = result,
            rounds = rounds,
            year = GameManager.Instance.SaveData.round,
            soldierLoss1 = soldierLoss1,
            soldierLoss2 = soldierLoss2,
            foodCost1 = foodCost1,
            foodCost2 = foodCost2,
            battleStats = currentBattleStats.Values.ToList()
        };
        
        battleRecords.Add(record);
        if (battleRecords.Count > MaxBattleCount)
            battleRecords.RemoveAt(0);
    }


    public List<BattleStat> GetTop10()
    {
        if (currentBattleStats == null)
            return new List<BattleStat>();
        return currentBattleStats.Values.OrderByDescending(x => x.damage).Take(10).ToList();
    }

    public BattleRecord GetBattleRecord(int battleId)
    {
        return battleRecords.FirstOrDefault(r => r.battleId == battleId);
    }
}
