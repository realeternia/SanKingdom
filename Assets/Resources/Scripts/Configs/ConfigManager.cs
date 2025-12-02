using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;

public static class ConfigManager
{
    private static Dictionary<string, JobConfig> jobDict = new Dictionary<string, JobConfig>();
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
        SoldierConfig.Load();
        ShopConfig.Load();
        PlayerConfig.Load();
        FormulaLearnAttrConfig.Load();
        JobConfig.Load();
        WorldConfig.Load();
        ForceConfig.Load();

        ConfigManager.PostModify();      

        UnityEngine.Debug.Log("ConfigManager Init fin");
    }

    public static void PostModify()
    {
        var jobNameConvDict = new Dictionary<string, string>();
        foreach (var jobCfg in JobConfig.ConfigList)
        {
            jobDict.Add(jobCfg.Name, jobCfg);
            jobDict.Add(jobCfg.NameS, jobCfg);
            jobNameConvDict[jobCfg.NameS] = jobCfg.Name;
        }      
        foreach (var skillCfg in SkillConfig.ConfigList)
        {
            skillDict.Add(skillCfg.Sname, skillCfg);
        }

        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            if (Profile.Instance.cardLoves != null && Profile.Instance.cardLoves.Contains((int)heroCfg.Id))
                heroCfg.RateAbs = 65;

            heroCfg.Job = jobNameConvDict.ContainsKey(heroCfg.Job) ? jobNameConvDict[heroCfg.Job] : heroCfg.Job;

            var jobCfg = GetJobConfig(heroCfg.Job);
            if (jobCfg != null)
                AddSkill(heroCfg, jobCfg.SkillId);

            if (!string.IsNullOrEmpty(heroCfg.Skill1))
            { 
                AddSkill(heroCfg, skillDict[heroCfg.Skill1].Id);
            }
            if (!string.IsNullOrEmpty(heroCfg.Skill2))
            { 
                AddSkill(heroCfg, skillDict[heroCfg.Skill2].Id);
            }
        }


        foreach (var heroCfg in HeroConfig.ConfigList)
        {
            if (heroCfg.Job.StartsWith("ma"))
                heroCfg.MoveSpeed = 12;
            else if (heroCfg.Job.StartsWith("gongnu"))
            {
                heroCfg.Range = 70;
                heroCfg.MoveSpeed = 7;
            }
            else if (heroCfg.Job.StartsWith("gong"))
            {
                heroCfg.Range = 50;
                heroCfg.MoveSpeed = 8;
            }          
            else if (heroCfg.Job.StartsWith("shan"))
            {
                heroCfg.Range = 35;
                heroCfg.MoveSpeed = 8;
            }     
            else if (heroCfg.Job.StartsWith("gu"))
            {
                heroCfg.Range = 35;
                heroCfg.MoveSpeed = 8;
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
            var tarJobCfg = ConfigManager.GetJobConfig(targetHeroCfg.Job);
            if (targetHeroCfg.Skills.Contains(skill) || (skillCfg.HelpSkillJob != "" && !skillCfg.HelpSkillJob.Contains(tarJobCfg.NameS)))
                continue;

            if (skillCfg.UnitHelpType == 1 && srcPos / 3 == targetPos / 3)
                return skill;
            else if (skillCfg.UnitHelpType == 2 && ((srcPos % 3) == (targetPos % 3)))
                return skill;
            // else if (skillCfg.UnitHelpType == 3)
            //     return skill;
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

    public static JobConfig GetJobConfig(string jobName)
    {
        if (jobDict.TryGetValue(jobName, out JobConfig value))
        {
            return value;
        }
        return null;
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
