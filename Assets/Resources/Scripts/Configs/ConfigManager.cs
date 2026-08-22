using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
public static class ConfigManager
{
    private static Dictionary<string, BattleSkillConfig> skillDict = new Dictionary<string, BattleSkillConfig>();

    private static bool hasInit = false;

    public static void Init()
    {
        if (hasInit)
            return;
        hasInit = true;
        
        HeroConfig.Load();
        BattleSkillConfig.Load();
        BuffConfig.Load();
        ItemConfig.Load();
        BattleUnitConfig.Load();
        ShopConfig.Load();
        FormulaLearnAttrConfig.Load();
        ArmsConfig.Load();
        WorldConfig.Load();
        ForceConfig.Load();
        CityLevelConfig.Load();
        CityDevConfig.Load();
        CityDevKingActionConfig.Load();
        CityAttrConfig.Load();
        HeroAttrConfig.Load();
        SystemAttrConfig.Load();
        SeasonConfig.Load();
        CityDevSearchConfig.Load();
        FairConfig.Load();
        TechConfig.Load();
        TechSkillConfig.Load();
        SysConfigModify.Load();
        
        ConfigManager.PostModify();      

        GameLog.Info("ConfigManager Init fin");
    }

    public static void PostModify()
    {
        // 按 Id 升序插入，保证兵种技能(21xxxx)在后，覆盖同名老技能（如 戟阵/旋风斩）
        var sorted = new List<BattleSkillConfig>(BattleSkillConfig.ConfigList);
        sorted.Sort((a, b) => a.Id.CompareTo(b.Id));
        foreach (var skillCfg in sorted)
        {
            if (skillDict.ContainsKey(skillCfg.Name))
                GameLog.Warn($"技能 Name 重名，后加载配置覆盖：{skillCfg.Name}({skillDict[skillCfg.Name].Id} -> {skillCfg.Id})");
            skillDict[skillCfg.Name] = skillCfg;
        }
    }
    
    public static bool IsHeroCard(int cardId)
    {
        return cardId < 200000;
    }

    public static BattleSkillConfig GetBattleSkillConfig(string skillName)
    {
        if (skillDict.TryGetValue(skillName, out BattleSkillConfig value))
        {
            return value;
        }
        return null;
    }
}
