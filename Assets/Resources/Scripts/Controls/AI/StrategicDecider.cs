using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using Controls.Utils;

public class StrategicDecider
{
    private const int MAX_DEF_CITIES = 1;
    private const int MIN_CITY_FOR_EXPANSION = 2;
    private const int MIN_RESOURCE_FOR_ATTACK = 500;
    private const int MIN_SOLDIER_FOR_ATTACK = 1000;
    
    private static Dictionary<int, HashSet<int>> attackedTargetsThisRound = new Dictionary<int, HashSet<int>>();
    
    public static void ClearRoundData()
    {
        attackedTargetsThisRound.Clear();
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
    
    private static string GetForceName(int forceId)
    {
        var cfg = ForceConfig.GetConfig(forceId);
        return cfg != null ? cfg.Cname : forceId.ToString();
    }
    
    private static string GetCityName(int cityId)
    {
        var cfg = WorldConfig.GetConfig(cityId);
        return cfg != null ? cfg.Cname : cityId.ToString();
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
        
        bool hasAttacked = false;
        
        if (CanExpand(player))
        {
            var attackTarget = SelectAttackTarget(player);
            if (attackTarget.HasValue)
            {
                var attackSource = SelectAttackSource(player, attackTarget.Value);
                if (attackSource.HasValue)
                {
                    result[attackSource.Value] = CityStrategyState.Atk;
                    hasAttacked = true;
                    var targetCity = GameManager.Instance.GetCity(attackTarget.Value);
                    string targetForceName = targetCity != null ? GetForceName(targetCity.forceId) : "未知";
                    GameLog.SetTag("AI").Info($"{GetForceName(player.forceId)} - [{GetCityName(attackSource.Value)}] 决定攻击[{GetCityName(attackTarget.Value)}] 目标势力:{targetForceName}");
                }
            }
        }
        
        if (!hasAttacked)
        {
            int defCount = 0;
            foreach (var cityId in frontlineCities)
            {
                if (defCount >= MAX_DEF_CITIES)
                    break;
                
                var city = GameManager.Instance.GetCity(cityId);
                if (HasThreat(city))
                {
                    result[cityId] = CityStrategyState.Def;
                    defCount++;
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
                if (enemySoldier >= 500)
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
        if (cities.Count < MIN_CITY_FOR_EXPANSION)
            return false;
        
        int totalGold = 0;
        int totalFood = 0;
        int totalSoldier = 0;
        
        foreach (var city in cities)
        {
            totalGold += city.gold;
            totalFood += city.food;
            totalSoldier += city.GetAttr("soldier");
        }
        
        return totalGold >= MIN_RESOURCE_FOR_ATTACK && 
               totalFood >= MIN_RESOURCE_FOR_ATTACK &&
               totalSoldier >= MIN_SOLDIER_FOR_ATTACK;
    }
    
    private static int? SelectAttackTarget(Player player)
    {
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
        
        if (potentialTargets.Count == 0)
            return null;
        
        potentialTargets.Sort((a, b) => 
        {
            var cityA = GameManager.Instance.GetCity(a);
            var cityB = GameManager.Instance.GetCity(b);
            return cityA.GetAttr("soldier").CompareTo(cityB.GetAttr("soldier"));
        });
        
        return potentialTargets[0];
    }
    
    private static int? SelectAttackSource(Player player, int targetCityId)
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
        
        return candidateCities[0].cityId;
    }
}
