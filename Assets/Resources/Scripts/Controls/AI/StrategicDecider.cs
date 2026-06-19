using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public class AttackCandidate
{
    public int sourceCityId;
    public int targetCityId;
    public int mySoldier;
    public int targetSoldier;
    public float advantage;
    public string sourceType;
    
    public AttackCandidate(int sourceId, int targetId, int mySold, int targetSold, string type)
    {
        sourceCityId = sourceId;
        targetCityId = targetId;
        mySoldier = mySold;
        targetSoldier = targetSold;
        advantage = SysFormula.AIStrategy.CalculateAdvantageRatio(mySold, targetSold);
        sourceType = type;
    }
}

public class StrategicDecider
{
    private const int MIN_CITY_SOLDIER_FOR_ATTACK = AIConst.AIStrategy.MIN_CITY_SOLDIER_FOR_ATTACK;
    
    private static Dictionary<int, HashSet<int>> attackedTargetsThisRound = new Dictionary<int, HashSet<int>>();
    private static Dictionary<int, int> attackTargets = new Dictionary<int, int>();
    
    public static void ClearRoundData()
    {
        attackedTargetsThisRound.Clear();
        attackTargets.Clear();
    }
    
    private static bool HasAttackedTarget(int forceId, int targetCityId)
    {
        return attackedTargetsThisRound.ContainsKey(forceId) && 
               attackedTargetsThisRound[forceId].Contains(targetCityId);
    }
    
    private static int CalculateEffectiveSoldier(SaveCityData city)
    {
        int citySoldier = (int)Math.Floor(city.GetAttr("soldier"));
        int heroCount = city.GetNormalHeroList().Count;
        return SysFormula.AIStrategy.CalculateEffectiveSoldier(citySoldier, heroCount);
    }
    
    /// <summary>
    /// 加权随机选择一个交恶势力，关系越差（分数越低）被选中的概率越大
    /// </summary>
    private static int PickHostileForceRandom(SaveForceData force, SaveForceRelation relation)
    {
        var hostileList = new List<(int forceId, int score, int weight)>();
        int totalWeight = 0;
        
        var allForces = GameManager.Instance.SaveData.forces;
        foreach (var other in allForces)
        {
            if (other.forceId == force.forceId || other.isEliminated)
                continue;
            if (relation.GetRelationLevel(force.forceId, other.forceId) != RelationLevel.Hostile)
                continue;
            
            int score = relation.GetRelation(force.forceId, other.forceId);
            int weight = SystemConst.Diplomacy.RELATION_HOSTILE_THRESHOLD - score + 1; // score越低weight越大
            totalWeight += weight;
            hostileList.Add((other.forceId, score, weight));
        }
        
        if (hostileList.Count == 0)
            return -1;
        
        int roll = SysRandom.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var hf in hostileList)
        {
            cumulative += hf.weight;
            if (roll < cumulative)
                return hf.forceId;
        }
        return hostileList[0].forceId;
    }
    
    /// <summary>
    /// 根据关系分数计算攻击发动概率，关系越差概率越高
    /// </summary>
    private static float GetAttackProbability(int relationScore)
    {
        // score ∈ [1, 35], 越接近1关系越差
        float prob = (SystemConst.Diplomacy.RELATION_HOSTILE_THRESHOLD - relationScore + 1) 
            / (float)(SystemConst.Diplomacy.RELATION_HOSTILE_THRESHOLD);
        return Math.Clamp(prob, 0.05f, 1f);
    }
    
    public static Dictionary<int, CityStrategyBase> DetermineCityStrategies(SaveForceData force, AIStrategyContext context)
    {
        var result = new Dictionary<int, CityStrategyBase>();
        var cities = force.GetCityList();
        var frontlineCities = MapTool.GetFrontlineCityIds(force.forceId);
        
        foreach (var city in cities)
        {
            result[city.cityId] = CityStrategyFactory.CreateStrategy(CityStrategyState.Dev, context, city, force);
        }
        
        int atkCount = 0;
        var relation = GameManager.Instance.SaveData.forceRelation;
        int targetForceId = PickHostileForceRandom(force, relation);
        
        if (targetForceId > 0)
        {
            int relationScore = relation.GetRelation(force.forceId, targetForceId);
            float attackProb = GetAttackProbability(relationScore);
            
            GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 选中交恶势力:{ConfigNameHelper.GetForceName(targetForceId)} 关系分:{relationScore} 攻击概率:{attackProb:F2}");
            
            if (SysRandom.Value < attackProb)
            {
                var allCandidates = new List<AttackCandidate>();
                var targetBasedCandidates = SelectAttackTargetsByEnemy(force, targetForceId, 10);
                allCandidates.AddRange(targetBasedCandidates);

                var ownCityCandidates = SelectAttackTargetsByOwnCity(force, targetForceId, 10);
                allCandidates.AddRange(ownCityCandidates);    
                
                var usedSources = new HashSet<int>();
                
                allCandidates.Sort((a, b) => b.advantage.CompareTo(a.advantage));
                
                foreach (var candidate in allCandidates)
                {
                    // 每个源城市只发起一次攻击，不限制目标城市数（允许多城围攻）
                    if (usedSources.Contains(candidate.sourceCityId))
                        continue;
                    
                    var sourceCity = cities.FirstOrDefault(c => c.cityId == candidate.sourceCityId);
                    if (sourceCity != null)
                    {
                        result[candidate.sourceCityId] = CityStrategyFactory.CreateStrategy(
                            CityStrategyState.Atk, context, sourceCity, force, candidate.targetCityId);
                    }
                    
                    attackTargets[candidate.sourceCityId] = candidate.targetCityId;
                    usedSources.Add(candidate.sourceCityId);
                    atkCount++;
                    
                    var targetCity = GameManager.Instance.GetCity(candidate.targetCityId);
                    string targetForceName = targetCity != null ? ConfigNameHelper.GetForceName(targetCity.forceId) : "未知";
                    GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(candidate.sourceCityId)}] 决定进攻[{ConfigNameHelper.GetCityName(candidate.targetCityId)}] 目标势力:{targetForceName} 优势比:{candidate.advantage:F2} 来源:{candidate.sourceType}");
                }
                
                GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 本轮发起{atkCount}路进攻");
            }
            else
            {
                GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} 随机检定未通过，本轮不进攻{ConfigNameHelper.GetForceName(targetForceId)}");
            }
        }
        
        if (atkCount == 0)
        {
            foreach (var cityId in frontlineCities)
            {               
                var city = GameManager.Instance.GetCity(cityId);
                if (HasThreat(city))
                {
                    var forceCity = cities.FirstOrDefault(c => c.cityId == cityId);
                    if (forceCity != null)
                    {
                        result[cityId] = CityStrategyFactory.CreateStrategy(CityStrategyState.Def, context, forceCity, force);
                    }
                    GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(cityId)}] 决定防御");
                }
            }
        }
        
        return result;
    }
    
    private static List<AttackCandidate> SelectAttackTargetsByEnemy(SaveForceData force, int targetForceId, int maxCount)
    {
        var result = new List<AttackCandidate>();
        var cities = force.GetCityList();
        var myCityIds = new HashSet<int>(cities.Select(c => c.cityId));
        
        var potentialTargets = new List<int>();
        
        foreach (var city in cities)
        {
            var enemyNearIds = MapTool.GetAdjacentEnemyCityIdsForCity(city.cityId, force.forceId);
            foreach (var nearId in enemyNearIds)
            {
                if (myCityIds.Contains(nearId))
                    continue;
                
                if (HasAttackedTarget(force.forceId, nearId))
                    continue;
                
                var nearCity = GameManager.Instance.GetCity(nearId);
                if (nearCity == null || nearCity.forceId != targetForceId)
                    continue;
                
                potentialTargets.Add(nearId);
            }
        }
        
        potentialTargets.Sort((a, b) => 
        {
            var cityA = GameManager.Instance.GetCity(a);
            var cityB = GameManager.Instance.GetCity(b);
            return ((int)Math.Floor(cityA.GetAttr("soldier"))).CompareTo((int)Math.Floor(cityB.GetAttr("soldier")));
        });
        
        foreach (var targetId in potentialTargets)
        {
            if (result.Count >= maxCount)
                break;
            
            var sourceId = SelectAttackSourceForTarget(force, targetId);
            if (sourceId.HasValue)
            {
                var sourceCity = GameManager.Instance.GetCity(sourceId.Value);
                var targetCity = GameManager.Instance.GetCity(targetId);
                int mySoldier = CalculateEffectiveSoldier(sourceCity);
                int targetSoldier = (int)Math.Floor(targetCity.GetAttr("soldier"));
                
                result.Add(new AttackCandidate(sourceId.Value, targetId, mySoldier, targetSoldier, "目标优先"));
            }
        }
        
        return result;
    }
    
    private static int? SelectAttackSourceForTarget(SaveForceData force, int targetCityId)
    {
        var cities = force.GetCityList();
        var targetCity = GameManager.Instance.GetCity(targetCityId);
        
        var candidateCities = new List<SaveCityData>();
        
        foreach (var city in cities)
        {
            if (MapTool.IsAdjacentCity(targetCityId, city.cityId))
            {
                candidateCities.Add(city);
            }
        }
        
        if (candidateCities.Count == 0)
            return null;
        
        candidateCities.Sort((a, b) => 
            ((int)Math.Floor(b.GetAttr("soldier"))).CompareTo((int)Math.Floor(a.GetAttr("soldier"))));
        
        var bestCity = candidateCities[0];
        int mySoldier = CalculateEffectiveSoldier(bestCity);
        int targetSoldier = (int)Math.Floor(targetCity.GetAttr("soldier"));
        
        if (mySoldier < targetSoldier * AIConst.AIStrategy.AI_ATTACK_SOURCE_ADVANTAGE_RATIO)
            return null;

        return bestCity.cityId;
    }
    
    private static List<AttackCandidate> SelectAttackTargetsByOwnCity(SaveForceData force, int targetForceId, int maxCount)
    {
        var result = new List<AttackCandidate>();
        var cities = force.GetCityList();
        var myCityIds = new HashSet<int>(cities.Select(c => c.cityId));
        
        foreach (var city in cities)
        {
            if (result.Count >= maxCount)
                break;
            
            int soldier = CalculateEffectiveSoldier(city);
            
            if (soldier > MIN_CITY_SOLDIER_FOR_ATTACK)
            {
                var enemyNearIds = MapTool.GetAdjacentEnemyCityIdsForCity(city.cityId, force.forceId);
                
                int? bestTarget = null;
                int minTargetSoldier = int.MaxValue;
                
                foreach (var nearId in enemyNearIds)
                {
                    if (myCityIds.Contains(nearId))
                        continue;
                    
                    if (HasAttackedTarget(force.forceId, nearId))
                        continue;
                    
                    var nearCity = GameManager.Instance.GetCity(nearId);
                    if (nearCity == null || nearCity.forceId != targetForceId)
                        continue;
                    
                    int targetSoldier = (int)Math.Floor(nearCity.GetAttr("soldier"));
                    
                    if (SysFormula.AIStrategy.CheckOwnCityAttackAdvantage(soldier, targetSoldier) && SysFormula.AIStrategy.CheckAttackFoodSufficient(soldier, (int)city.food))
                    {
                        if (targetSoldier < minTargetSoldier)
                        {
                            minTargetSoldier = targetSoldier;
                            bestTarget = nearId;
                        }
                    }
                }
                
                if (bestTarget.HasValue)
                {
                    result.Add(new AttackCandidate(city.cityId, bestTarget.Value, soldier, minTargetSoldier, "己方城市优先"));
                }
            }
        }
        
        return result;
    }
    
    private static bool HasThreat(SaveCityData city)
    {
        var enemyNearIds = MapTool.GetAdjacentEnemyCityIdsForCity(city.cityId, city.forceId);
        foreach (var nearId in enemyNearIds)
        {
            var nearCity = GameManager.Instance.GetCity(nearId);
            if (nearCity != null)
            {
                int enemySoldier = (int)Math.Floor(nearCity.GetAttr("soldier"));
                if (SysFormula.AIStrategy.HasThreat(enemySoldier))
                {
                    return true;
                }
            }
        }
        return false;
    }
}
