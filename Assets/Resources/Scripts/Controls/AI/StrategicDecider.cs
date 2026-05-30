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
    private const int MAX_ATK_CITIES = SystemConst.AIStrategy.MAX_ATK_CITIES;
    private const int MIN_CITY_SOLDIER_FOR_ATTACK = SystemConst.AIStrategy.MIN_CITY_SOLDIER_FOR_ATTACK;
    private const int MIN_CITY_HEROES_FOR_ATTACK = SystemConst.AIStrategy.MIN_CITY_HEROES_FOR_ATTACK;
    
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
        if (CanExpand(force))
        {
            var allCandidates = new List<AttackCandidate>();
            var targetBasedCandidates = SelectAttackTargetsByEnemy(force, 2);
            allCandidates.AddRange(targetBasedCandidates);

            var ownCityCandidates = SelectAttackTargetsByOwnCity(force, 2);
            allCandidates.AddRange(ownCityCandidates);    
            
            var usedSources = new HashSet<int>();
            var usedTargets = new HashSet<int>();

            
            allCandidates.Sort((a, b) => b.advantage.CompareTo(a.advantage));
            
            foreach (var candidate in allCandidates)
            {
                if (atkCount >= MAX_ATK_CITIES)
                    break;
                
                if (usedSources.Contains(candidate.sourceCityId))
                    continue;
                
                if (usedTargets.Contains(candidate.targetCityId))
                    continue;
                
                var sourceCity = cities.FirstOrDefault(c => c.cityId == candidate.sourceCityId);
                if (sourceCity != null)
                {
                    result[candidate.sourceCityId] = CityStrategyFactory.CreateStrategy(
                        CityStrategyState.Atk, context, sourceCity, force, candidate.targetCityId);
                }
                
                attackTargets[candidate.sourceCityId] = candidate.targetCityId;
                usedSources.Add(candidate.sourceCityId);
                usedTargets.Add(candidate.targetCityId);
                atkCount++;
                
                var targetCity = GameManager.Instance.GetCity(candidate.targetCityId);
                string targetForceName = targetCity != null ? ConfigNameHelper.GetForceName(targetCity.forceId) : "未知";
                GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(force.forceId)} - [{ConfigNameHelper.GetCityName(candidate.sourceCityId)}] 决定进攻[{ConfigNameHelper.GetCityName(candidate.targetCityId)}] 目标势力:{targetForceName} 优势比:{candidate.advantage:F2} 来源:{candidate.sourceType}");
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
    
    private static List<AttackCandidate> SelectAttackTargetsByEnemy(SaveForceData force, int maxCount)
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
        
        if (mySoldier < targetSoldier * SystemConst.AIStrategy.AI_ATTACK_SOURCE_ADVANTAGE_RATIO)
            return null;

        return bestCity.cityId;
    }
    
    private static List<AttackCandidate> SelectAttackTargetsByOwnCity(SaveForceData force, int maxCount)
    {
        var result = new List<AttackCandidate>();
        var cities = force.GetCityList();
        var myCityIds = new HashSet<int>(cities.Select(c => c.cityId));
        
        foreach (var city in cities)
        {
            if (result.Count >= maxCount)
                break;
            
            int soldier = CalculateEffectiveSoldier(city);
            int heroCount = city.GetNormalHeroList().Count;
            
            if (soldier >= MIN_CITY_SOLDIER_FOR_ATTACK && heroCount >= MIN_CITY_HEROES_FOR_ATTACK)
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
                    if (nearCity != null)
                    {
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
    
    private static bool CanExpand(SaveForceData force)
    {
        var cities = force.GetCityList();
        var forceData = GameManager.Instance.GetForce(force.forceId);
        
        int totalGold = (int)forceData.gold;
        int totalFood = 0;
        int totalSoldier = 0;
        
        foreach (var city in cities)
        {
            totalFood += (int)city.food;
            totalSoldier += (int)Math.Floor(city.GetAttr("soldier"));
        }
        
        return SysFormula.AIStrategy.CanExpand(totalGold, totalFood, totalSoldier);
    }
}
