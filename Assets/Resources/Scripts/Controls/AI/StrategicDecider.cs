using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;
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
    private const int MIN_RESOURCE_FOR_ATTACK = SystemConst.AIStrategy.MIN_RESOURCE_FOR_ATTACK;
    private const int MIN_SOLDIER_FOR_ATTACK = SystemConst.AIStrategy.MIN_SOLDIER_FOR_ATTACK;
    private const int MIN_CITY_SOLDIER_FOR_ATTACK = SystemConst.AIStrategy.MIN_CITY_SOLDIER_FOR_ATTACK;
    private const int MIN_CITY_HEROES_FOR_ATTACK = SystemConst.AIStrategy.MIN_CITY_HEROES_FOR_ATTACK;
    private const int MAX_SOLDIER_PER_HERO = SystemConst.AIStrategy.MAX_SOLDIER_PER_HERO;
    
    private static Dictionary<int, HashSet<int>> attackedTargetsThisRound = new Dictionary<int, HashSet<int>>();
    private static Dictionary<int, int> attackTargets = new Dictionary<int, int>();
    
    public static void ClearRoundData()
    {
        attackedTargetsThisRound.Clear();
        attackTargets.Clear();
    }
    
    public static int? GetAttackTarget(int sourceCityId)
    {
        if (attackTargets.ContainsKey(sourceCityId))
            return attackTargets[sourceCityId];
        return null;
    }
    
    public static void MarkTargetAttacked(int forceId, int targetCityId)
    {
        if (!attackedTargetsThisRound.ContainsKey(forceId))
        {
            attackedTargetsThisRound[forceId] = new HashSet<int>();
        }
        attackedTargetsThisRound[forceId].Add(targetCityId);
    }
    
    public static bool HasAttackedTarget(int forceId, int targetCityId)
    {
        return attackedTargetsThisRound.ContainsKey(forceId) && 
               attackedTargetsThisRound[forceId].Contains(targetCityId);
    }
    
    private static int CalculateEffectiveSoldier(SaveCityData city)
    {
        int citySoldier = city.GetAttr("soldier");
        int heroCount = city.GetNormalHeroList().Count;
        return SysFormula.AIStrategy.CalculateEffectiveSoldier(citySoldier, heroCount);
    }
    
    public static Dictionary<int, CityStrategyState> DetermineCityStrategies(Player player)
    {
        var result = new Dictionary<int, CityStrategyState>();
        var cities = player.GetCityList();
        var frontlineCities = HeroDispatcher.GetFrontlineCities(player);
        
        foreach (var city in cities)
        {
            result[city.cityId] = CityStrategyState.Dev;
        }
        
        int atkCount = 0;
        if (CanExpand(player))
        {
            var allCandidates = new List<AttackCandidate>();
            var targetBasedCandidates = SelectAttackTargetsByEnemy(player, 2);
            allCandidates.AddRange(targetBasedCandidates);

            var ownCityCandidates = SelectAttackTargetsByOwnCity(player, 2);
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
                
                result[candidate.sourceCityId] = CityStrategyState.Atk;
                attackTargets[candidate.sourceCityId] = candidate.targetCityId;
                usedSources.Add(candidate.sourceCityId);
                usedTargets.Add(candidate.targetCityId);
                atkCount++;
                
                var targetCity = GameManager.Instance.GetCity(candidate.targetCityId);
                string targetForceName = targetCity != null ? ConfigNameHelper.GetForceName(targetCity.forceId) : "未知";
                GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(player.forceId)} - [{ConfigNameHelper.GetCityName(candidate.sourceCityId)}] 决定攻击[{ConfigNameHelper.GetCityName(candidate.targetCityId)}] 目标势力:{targetForceName} 优势比:{candidate.advantage:F2} 来源:{candidate.sourceType}");
            }
        }
        
        if (atkCount == 0)
        {
            foreach (var cityId in frontlineCities)
            {               
                var city = GameManager.Instance.GetCity(cityId);
                if (HasThreat(city))
                {
                    result[cityId] = CityStrategyState.Def;
                    GameLog.SetTag("AI").Info($"{ConfigNameHelper.GetForceName(player.forceId)} - [{ConfigNameHelper.GetCityName(cityId)}] 决定防御");
                }
            }
        }
        
        return result;
    }
    
    private static List<AttackCandidate> SelectAttackTargetsByEnemy(Player player, int maxCount)
    {
        var result = new List<AttackCandidate>();
        var cities = player.GetCityList();
        var myCityIds = new HashSet<int>(cities.Select(c => c.cityId));
        
        var potentialTargets = new List<int>();
        
        foreach (var city in cities)
        {
            var nearCityIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
            if (nearCityIds == null) continue;
            
            foreach (var nearId in nearCityIds)
            {
                if (myCityIds.Contains(nearId))
                    continue;
                
                if (HasAttackedTarget(player.forceId, nearId))
                    continue;
                
                var nearCity = GameManager.Instance.GetCity(nearId);
                if (nearCity != null && nearCity.forceId != player.forceId)
                {
                    potentialTargets.Add(nearId);
                }
            }
        }
        
        potentialTargets.Sort((a, b) => 
        {
            var cityA = GameManager.Instance.GetCity(a);
            var cityB = GameManager.Instance.GetCity(b);
            return cityA.GetAttr("soldier").CompareTo(cityB.GetAttr("soldier"));
        });
        
        foreach (var targetId in potentialTargets)
        {
            if (result.Count >= maxCount)
                break;
            
            var sourceId = SelectAttackSourceForTarget(player, targetId);
            if (sourceId.HasValue)
            {
                var sourceCity = GameManager.Instance.GetCity(sourceId.Value);
                var targetCity = GameManager.Instance.GetCity(targetId);
                int mySoldier = CalculateEffectiveSoldier(sourceCity);
                int targetSoldier = targetCity.GetAttr("soldier");
                
                result.Add(new AttackCandidate(sourceId.Value, targetId, mySoldier, targetSoldier, "目标优先"));
            }
        }
        
        return result;
    }
    
    private static int? SelectAttackSourceForTarget(Player player, int targetCityId)
    {
        var cities = player.GetCityList();
        var targetCity = GameManager.Instance.GetCity(targetCityId);
        
        var nearCityIds = WorldConfig.GetConfig(targetCityId)?.WorldNearIds;
        if (nearCityIds == null) return null;
        
        var candidateCities = new List<SaveCityData>();
        
        foreach (var city in cities)
        {
            if (System.Array.Exists(nearCityIds, id => id == city.cityId))
            {
                candidateCities.Add(city);
            }
        }
        
        if (candidateCities.Count == 0)
            return null;
        
        candidateCities.Sort((a, b) => 
            b.GetAttr("soldier").CompareTo(a.GetAttr("soldier")));
        
        var bestCity = candidateCities[0];
        int mySoldier = CalculateEffectiveSoldier(bestCity);
        int targetSoldier = targetCity.GetAttr("soldier");
        
        if (mySoldier < targetSoldier * SystemConst.AIStrategy.AI_ATTACK_SOURCE_ADVANTAGE_RATIO)
            return null;

        return bestCity.cityId;
    }
    
    private static List<AttackCandidate> SelectAttackTargetsByOwnCity(Player player, int maxCount)
    {
        var result = new List<AttackCandidate>();
        var cities = player.GetCityList();
        var myCityIds = new HashSet<int>(cities.Select(c => c.cityId));
        
        foreach (var city in cities)
        {
            if (result.Count >= maxCount)
                break;
            
            int soldier = CalculateEffectiveSoldier(city);
            int heroCount = city.GetNormalHeroList().Count;
            
            if (soldier >= MIN_CITY_SOLDIER_FOR_ATTACK && heroCount >= MIN_CITY_HEROES_FOR_ATTACK)
            {
                var nearCityIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
                if (nearCityIds == null) continue;
                
                int? bestTarget = null;
                int minTargetSoldier = int.MaxValue;
                
                foreach (var nearId in nearCityIds)
                {
                    if (myCityIds.Contains(nearId))
                        continue;
                    
                    if (HasAttackedTarget(player.forceId, nearId))
                        continue;
                    
                    var nearCity = GameManager.Instance.GetCity(nearId);
                    if (nearCity != null && nearCity.forceId != player.forceId)
                    {
                        int targetSoldier = nearCity.GetAttr("soldier");
                        
                        if (SysFormula.AIStrategy.CheckOwnCityAttackAdvantage(soldier, targetSoldier) && SysFormula.AIStrategy.CheckAttackFoodSufficient(soldier, (int)GameManager.Instance.GetForce(city.forceId).food))
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
        var nearCityIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
        if (nearCityIds == null) return false;
        
        foreach (var nearId in nearCityIds)
        {
            var nearCity = GameManager.Instance.GetCity(nearId);
            if (nearCity != null && nearCity.forceId != city.forceId)
            {
                int enemySoldier = nearCity.GetAttr("soldier");
                if (SysFormula.AIStrategy.HasThreat(enemySoldier))
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    private static bool CanExpand(Player player)
    {
        var cities = player.GetCityList();
        var forceData = GameManager.Instance.GetForce(player.forceId);
        
        int totalGold = (int)forceData.gold;
        int totalFood = (int)forceData.food;
        int totalSoldier = 0;
        
        foreach (var city in cities)
        {
            totalSoldier += city.GetAttr("soldier");
        }
        
        return SysFormula.AIStrategy.CanExpand(totalGold, totalFood, totalSoldier);
    }
}
