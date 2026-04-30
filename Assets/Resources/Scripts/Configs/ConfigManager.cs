using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using Controls.Utils;

public static class ConfigManager
{
    private static Dictionary<string, SkillConfig> skillDict = new Dictionary<string, SkillConfig>();

    private static bool hasInit = false;

    public static void Init()
    {
        if (hasInit)
            return;
        hasInit = true;
        
        HeroConfig.Load();
        SkillConfig.Load();
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
        CityAttrConfig.Load();
        HeroAttrConfig.Load();
        SeasonConfig.Load();
        
        ConfigManager.PostModify();      

        GameLog.Info("ConfigManager Init fin");
    }

    public static void PostModify()
    {
        foreach (var skillCfg in SkillConfig.ConfigList)
        {
            skillDict.Add(skillCfg.Sname, skillCfg);
        }

        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            if (!string.IsNullOrEmpty(heroCfg.Skill1))
            { 
                AddSkill(heroCfg, skillDict[heroCfg.Skill1].Id);
            }
            if (!string.IsNullOrEmpty(heroCfg.Skill2))
            { 
                AddSkill(heroCfg, skillDict[heroCfg.Skill2].Id);
            }
        }
    }
    
    public static int GetShowHelpSkillId(int heroId, int targetHeroId, int srcPos, int targetPos)
    {
        var heroCfg = HeroConfig.GetConfig(heroId);
        foreach(var skill in heroCfg.Skills)
        {
            var skillCfg = SkillConfig.GetConfig(skill);
            if (skillCfg.UnitHelpType <= 0)
                continue;

            var targetHeroCfg = HeroConfig.GetConfig(targetHeroId);
            if (targetHeroCfg.Skills.Contains(skill))
                continue;

            if (skillCfg.UnitHelpType == 1 && srcPos / 3 == targetPos / 3)
                return skill;
            else if (skillCfg.UnitHelpType == 2 && ((srcPos % 3) == (targetPos % 3)))
                return skill;
        }

        return 0;
    }

    private static void AddSkill(HeroConfig heroCfg, int skillId)
    {
        if (heroCfg.Skills == null)
        {
            heroCfg.Skills = new int[1] { skillId };
        }
        else
        {
            System.Array.Resize(ref heroCfg.Skills, heroCfg.Skills.Length + 1);
            heroCfg.Skills[heroCfg.Skills.Length - 1] = skillId;
        }   
    }

    public static bool IsHeroCard(int cardId)
    {
        return cardId < 200000;
    }

    public static SkillConfig GetSkillConfig(string skillName)
    {
        if (skillDict.TryGetValue(skillName, out SkillConfig value))
        {
            return value;
        }
        return null;
    }
}
