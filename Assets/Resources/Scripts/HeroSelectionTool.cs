using System;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

// 定义一个单独的工具类
public static class HeroSelectionTool
{
    private static List<Tuple<int, int>> heroPoolCache = new List<Tuple<int, int>>();

    // 获取指定阵营的所有英雄ID
    public static List<int> GetAllHeroIdsBySide(int side)
    {
        List<int> heroIds = new List<int>();
        // 假设HeroConfig有一个方法GetAllConfigs()返回所有英雄配置
        foreach (var config in HeroConfig.ConfigList)
        {
            if (config.Side == side)
            {
                heroIds.Add((int)config.Id);
            }
        }
        return heroIds;
    }

    // 从源ID列表中随机选择指定数量的不重复ID
    public static List<int> GetRandomUniqueIds(List<int> sourceIds, int count)
    {
        List<int> result = new List<int>();
        if (sourceIds == null || sourceIds.Count == 0 || count <= 0)
        {
            return result;
        }

        // 创建源列表的副本以避免修改原列表
        List<int> tempIds = new List<int>(sourceIds);
        int actualCount = Mathf.Min(count, tempIds.Count);

        for (int i = 0; i < actualCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempIds.Count);
            result.Add(tempIds[randomIndex]);
            tempIds.RemoveAt(randomIndex);
        }

        return result;
    }


    public static void UpdateHeroPoolCache(List<int> heroIds)
    {
        heroPoolCache.Clear();
        foreach (var heroId in heroIds)
        {
            var config = HeroConfig.GetConfig(heroId);
            var rate = 1000 / Math.Max(15, GetPrice(config));
            if (config.Job == "shuai")
                rate += 15;
            heroPoolCache.Add(new Tuple<int, int>(heroId, rate));
        }

        heroPoolCache.Sort((a, b) =>
        {
            var configA = HeroConfig.GetConfig(a.Item1);
            var configB = HeroConfig.GetConfig(b.Item1);
            int sideCompare = configA.Side.CompareTo(configB.Side);
            if (sideCompare != 0)
            {
                return sideCompare;
            }

            // 检查ID是否在100100以下
            bool isBelow100100A = a.Item1 < 100100;
            bool isBelow100100B = b.Item1 < 100100;
            if (isBelow100100A != isBelow100100B)
            {
                return isBelow100100A ? -1 : 1;
            }

            // 按Total排序
            return configB.Total.CompareTo(configA.Total);
        });

    }

    public static List<int> GetHeroPoolCache()
    {
        // 返回只包含heroId的列表
        List<int> result = new List<int>();
        foreach (var hero in heroPoolCache)
        {
            result.Add(hero.Item1);
        }
        return result;
    }

    public static void SetBanList(List<int> banList)
    {
        heroPoolCache.RemoveAll(hero => banList.Contains(hero.Item1));
    }

    public static int GetRandomHeroId()
    {
        // 实现价格加权随机：价格越高，被选中的概率越低
        // 使用价格的倒数作为权重
        float totalWeight = 0;
        foreach (var hero in heroPoolCache)
        {
            // 防止价格为0的情况
            float weight = hero.Item2 > 0 ? hero.Item2 : 1f;
            totalWeight += weight;
        }
        
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float accumulatedWeight = 0;
        
        foreach (var hero in heroPoolCache)
        {
            float weight = hero.Item2 > 0 ? hero.Item2 : 1f;
            accumulatedWeight += weight;
            
            if (accumulatedWeight >= randomValue)
            {
                return hero.Item1;
            }
        }
        
        // 如果出现问题，返回第一个英雄（保底）
        return heroPoolCache[0].Item1;
    }

    public static bool HasHeroInPool(int heroId)
    {
        return heroPoolCache.Exists(hero => hero.Item1 == heroId);
    }

    public static int GetRandomItemId(int shopIdx)
    {
        var itemList = ItemConfig.ConfigList.ToList();
        // 剔除所有RateAbs非0的item
        itemList.RemoveAll(item => item.RateAbs > 0);
        // 剔除所有ShopId非0的item
        itemList.RemoveAll(item => item.ShopIdx > shopIdx);
        int randomIndex = UnityEngine.Random.Range(0, itemList.Count);
        return itemList[randomIndex].Id;
    }

    public static int GetPrice(HeroConfig heroCfg)
    {
        var baseP = Mathf.Pow((float)heroCfg.Total, 1.4f) / 125;
        float bonus = 0;
        if (heroCfg.Str >= 90) bonus += (heroCfg.Str - 89) * 0.01f;
        if (heroCfg.Inte >= 90) bonus += (heroCfg.Inte - 89) * 0.01f;
        if (heroCfg.LeadShip >= 90) bonus += (heroCfg.LeadShip - 89) * 0.01f;
        var rangeMark = (float)heroCfg.Range / 17 * 40;
        if(heroCfg.Range >= 20)
            rangeMark += 20;
        bonus += ((float)heroCfg.Hp + rangeMark - 340) / 340;

        if (heroCfg.Total >= 210)
        { //救一下偏科的人
            if (heroCfg.Str < 60)
                bonus -= (60 - heroCfg.Str) * 0.005f;
            if (heroCfg.Inte < 60)
                bonus -= (60 - heroCfg.Inte) * 0.005f;
            if (heroCfg.LeadShip < 60)
                bonus -= (60 - heroCfg.LeadShip) * 0.005f;
        }

        var skillP = 0;
        if (heroCfg.Skills != null)
            foreach (var skillId in heroCfg.Skills)
                skillP += SkillConfig.GetConfig(skillId).Price; //加上技能价格

        var finalP = baseP * (1 + bonus) + skillP;
        return Mathf.RoundToInt(finalP);
    }

    private static int[] cardHeroExp = new int[] { 1, 2, 4, 7, 11, 16, 22, 29, 37, 46, 56, 66, 76, 86, 96, 106, 116, 126, 136, 146, 156, 166, 176, 186, 196, 206, 216, 226, 236, 246, 256, 266, 276, 999 };
    private static int[] cardItemExp = new int[] { 1, 2, 4, 6, 9, 12, 15, 19, 23, 27, 31, 36, 41, 46, 51, 56, 62, 68, 74, 80, 86, 92, 98, 104, 110, 116, 122, 128, 136, 142, 148, 154, 160, 166, 172, 178, 184, 190, 196, 202, 999 }; //生成后续数据
    public static int GetCardLevel(int exp, bool isHero)
    {
        if(isHero)
        {
            for(int i = 0; i < cardHeroExp.Length; i++)
            {
                if(exp < cardHeroExp[i])
                    return i;
            }
            return cardHeroExp.Length;
        }
        else
        {
            for(int i = 0; i < cardItemExp.Length; i++)
            {
                if(exp < cardItemExp[i])
                    return i;
            }
            return cardItemExp.Length;
        }
    }

    public static float GetExpRate(int exp, bool isHero)
    {
        int level = GetCardLevel(exp, isHero);
        if(level >= cardHeroExp.Length)
            return 1f;
        if(level <= 1)
            return 0;
        if(isHero)
            return (float)(exp - cardHeroExp[level - 1]) / (cardHeroExp[level] - cardHeroExp[level - 1]);
        else
            return (float)(exp - cardItemExp[level - 1]) / (cardItemExp[level] - cardItemExp[level - 1]);
    }

    public static AttrInfo GetCardAttr(PlayerInfo player, int cardId, int lv)
    {
        var attrInfo = new AttrInfo();
        if (ConfigManager.IsHeroCard(cardId))
        {
            var heroConfig = HeroConfig.GetConfig(cardId);

            attrInfo.Hp = heroConfig.Hp + heroConfig.Hp * (lv - 1) / 10;
            attrInfo.Inte = heroConfig.Inte + System.Math.Max(8 * (lv - 1), heroConfig.Inte * (lv - 1) / 10);
            attrInfo.Str = heroConfig.Str + System.Math.Max(8 * (lv - 1), heroConfig.Str * (lv - 1) / 10);
            attrInfo.Lead = heroConfig.LeadShip + System.Math.Max(8 * (lv - 1), heroConfig.LeadShip * (lv - 1) / 10);
        }
        else
        {
            var itemConfig = ItemConfig.GetConfig(cardId);
            if (itemConfig.Attr1 == "str")
            {
                attrInfo.Str = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "inte")
            {
                attrInfo.Inte = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "lead")
            {
                attrInfo.Lead = itemConfig.Attr1Val;
            }
            else if (itemConfig.Attr1 == "shield")
            {
                attrInfo.Hp = itemConfig.Attr1Val;
            }

            if (itemConfig.Attr2 == "str")
            {
                attrInfo.Str = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "inte")
            {
                attrInfo.Inte = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "lead")
            {
                attrInfo.Lead = itemConfig.Attr2Val;
            }
            else if (itemConfig.Attr2 == "shield")
            {
                attrInfo.Hp = itemConfig.Attr2Val;
            }

            attrInfo.Hp = attrInfo.Hp * lv;
            attrInfo.Inte = attrInfo.Inte * lv;
            attrInfo.Str = attrInfo.Str * lv;
            attrInfo.Lead = attrInfo.Lead * lv;
        }
        if(player.attrAddons.ContainsKey(cardId))
            attrInfo.AddAttr(player.attrAddons[cardId]);

        return attrInfo;

    }

    public static Color GetSideColor(int side)
    {
        if (side == 1)
            return new Color(40 / 255f, 70 / 255f, 0 / 255f, 255 / 255f);
        else if (side == 2)
            return new Color(0 / 255f, 35 / 255f, 100 / 255f, 255 / 255f);
        else if (side == 3)
            return new Color(100 / 255f, 0 / 255f, 0 / 255f, 255 / 255f);
        else if (side == 4)
            return new Color(30 / 255f, 100 / 255f, 110 / 255f, 255 / 255f);
        else if (side == 5)
            return new Color(90 / 255f, 50 / 255f, 110 / 255f, 255 / 255f);
        else if (side == 6)
            return new Color(120 / 255f, 90 / 255f, 30 / 255f, 255 / 255f);                                    
        else
            return new Color(50 / 255f, 50 / 255f, 50 / 255f, 255 / 255f);
    }
 
}
