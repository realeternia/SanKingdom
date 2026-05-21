using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;

public static class MapTool
{
    public static List<int> GetAdjacentCityIds(int cityId)
    {
        var cityCfg = WorldConfig.GetConfig(cityId);
        if (cityCfg == null || cityCfg.WorldNearIds == null)
            return new List<int>();
        return cityCfg.WorldNearIds.ToList();
    }

    public static List<int> GetAdjacentFriendlyCityIds(int cityId, int forceId)
    {
        var result = new List<int>();
        var nearIds = WorldConfig.GetConfig(cityId)?.WorldNearIds;
        if (nearIds == null) return result;

        foreach (var nearCityId in nearIds)
        {
            var nearCity = GameManager.Instance.GetCity(nearCityId);
            if (nearCity != null && nearCity.forceId == forceId)
            {
                result.Add(nearCityId);
            }
        }
        return result;
    }

    public static List<int> GetAdjacentEnemyCityIdsForCity(int cityId, int forceId)
    {
        var result = new List<int>();
        var nearIds = WorldConfig.GetConfig(cityId)?.WorldNearIds;
        if (nearIds == null) return result;

        foreach (var nearCityId in nearIds)
        {
            var nearCity = GameManager.Instance.GetCity(nearCityId);
            if (nearCity != null && nearCity.forceId != forceId)
            {
                result.Add(nearCityId);
            }
        }
        return result;
    }

    public static bool IsAdjacentCity(int cityId1, int cityId2)
    {
        var nearIds = WorldConfig.GetConfig(cityId1)?.WorldNearIds;
        if (nearIds == null) return false;
        return Array.Exists(nearIds, id => id == cityId2);
    }

    public static bool IsFrontlineCity(int cityId)
    {
        var city = GameManager.Instance.GetCity(cityId);
        if (city == null) return false;
        return GetAdjacentEnemyCityIdsForCity(cityId, city.forceId).Count > 0;
    }

    public static List<int> GetFrontlineCityIds(int forceId)
    {
        var result = new List<int>();
        var cities = GameManager.Instance.GetCitiesByForce(forceId);
        foreach (var city in cities)
        {
            if (IsFrontlineCity(city.cityId))
            {
                result.Add(city.cityId);
            }
        }
        return result;
    }

    public static List<int> GetRearCityIds(int forceId)
    {
        var result = new List<int>();
        var cities = GameManager.Instance.GetCitiesByForce(forceId);
        foreach (var city in cities)
        {
            if (!IsFrontlineCity(city.cityId))
            {
                result.Add(city.cityId);
            }
        }
        return result;
    }

    public static List<int> GetOwnCityIds(int forceId)
    {
        return GameManager.Instance.GetCitiesByForce(forceId)
            .Select(c => c.cityId)
            .ToList();
    }

    public static List<int> GetAdjacentEnemyCityIds(int forceId)
    {
        var result = new HashSet<int>();
        var ownCities = GameManager.Instance.GetCitiesByForce(forceId);

        foreach (var city in ownCities)
        {
            var enemyIds = GetAdjacentEnemyCityIdsForCity(city.cityId, forceId);
            foreach (var enemyId in enemyIds)
            {
                result.Add(enemyId);
            }
        }

        return result.ToList();
    }

    public static bool AreForcesAdjacent(int forceId1, int forceId2)
    {
        if (forceId1 == forceId2) return false;
        var cities1 = GameManager.Instance.GetCitiesByForce(forceId1);
        foreach (var city in cities1)
        {
            var nearIds = WorldConfig.GetConfig(city.cityId)?.WorldNearIds;
            if (nearIds == null) continue;
            foreach (var nearCityId in nearIds)
            {
                var nearCity = GameManager.Instance.GetCity(nearCityId);
                if (nearCity != null && nearCity.forceId == forceId2)
                    return true;
            }
        }
        return false;
    }

    public static int GetRandomAdjacentCityId(int cityId)
    {
        var nearIds = WorldConfig.GetConfig(cityId)?.WorldNearIds;
        if (nearIds == null || nearIds.Length == 0) return 0;
        return nearIds[SysRandom.Range(0, nearIds.Length)];
    }

    public static int CalculateCityDistance(int cityId1, int cityId2)
    {
        var cfg1 = WorldConfig.GetConfig(cityId1);
        var cfg2 = WorldConfig.GetConfig(cityId2);
        return SysFormula.City.CalculateDistance(cfg1.X, cfg1.Y, cfg2.X, cfg2.Y);
    }
}
