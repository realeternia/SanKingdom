using System;
using System.Collections.Generic;
using CommonConfig;
using UnityEngine;

public static class AI
{
    /// <summary>
    /// AI执行方法：为每个非玩家玩家的每个城市分配英雄任务
    /// </summary>
    /// <param name="player">AI玩家</param>
    public static void ExecuteAiActions(Player player)
    {
        // 获取该势力的所有城市
        List<SaveCityData> playerCities = GameManager.Instance.GetCitiesByForce(player.forceId);

        // 遍历每个城市
        foreach (var city in playerCities)
        {
            Debug.Log($"AI处理城市: {city.cityId}");

            // 获取城市的英雄列表
            List<int> heroList = city.GetHeroList(true, false); // 不包含在野英雄，因为他们无法执行任务
            if (heroList.Count == 0)
            {
                continue;
            }

            // 为城市分配英雄执行发展任务
            AssignHeroesToDevTasks(player, city.cityId, heroList.ToArray());
        }
    }

    /// <summary>
    /// 获取城市可执行的发展任务ID列表
    /// </summary>
    /// <param name="cityId">城市ID</param>
    /// <returns>可执行的发展任务ID列表</returns>
    private static List<int> GetAvailableDevIds(int cityId)
    {
        List<int> availableDevIds = new List<int>();
        var cityData = GameManager.Instance.GetCity(cityId);
        
        // 遍历所有发展配置，筛选出可执行的任务
        foreach (var devConfig in CityDevConfig.ConfigList)
        {
            if(devConfig.Prefab == "CityDevBattle")
                continue;
            
            // 检查发展任务的主要属性是否已达到最大值
            if (devConfig.Attrs.Length > 0)
            {
                string mainAttr = devConfig.DevAttr1.ToLower();
                var attrConfig = CityAttrConfig.GetConfigByname(mainAttr);
                int currentVal = cityData.GetAttr(mainAttr);
                if (currentVal >= attrConfig.ValMax)
                    continue;
            }

            if (cityData.gold < devConfig.GoldCost)
                continue;
            
            availableDevIds.Add(devConfig.Id);
        }
        
        return availableDevIds;
    }
    
    /// <summary>
    /// 为城市分配英雄执行发展任务
    /// </summary>
    /// <param name="player">玩家对象</param>
    /// <param name="cityId">城市ID</param>
    /// <param name="heroList">英雄列表</param>
    private static void AssignHeroesToDevTasks(Player player, int cityId, int[] heroList)
    {
        // 获取当前年份可用的英雄列表
        List<int> availableHeroes = player.GetAvailableHeroesThisYear(heroList);
        if (availableHeroes.Count == 0)
            return;
        
        // 获取城市数据
        var city = GameManager.Instance.GetCity(cityId);

        // 按英雄循环，确保每个英雄都分配任务
        foreach (int heroId in availableHeroes)
        {
            // 获取可执行的发展任务列表
            List<int> availableDevIds = GetAvailableDevIds(city.cityId);
            if (availableDevIds.Count == 0)
                continue;

            int devId = 0;

            // 检查城市金钱是否不足500
            // 金钱充足，随机选择一个可用的发展任务
            if (availableDevIds.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableDevIds.Count);
                devId = availableDevIds[randomIndex];
                Debug.Log($"AI城市 {cityId} 金钱充足，为英雄 {heroId} 分配随机任务 {devId}");
            }

            if (devId == 0)
                continue;
            player.ExecuteCityDev(cityId, devId, new int[] { heroId }, out _);
        }
    }
}