using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class HeroTaskMatch
{
    public SaveHeroData hero;
    public int devId;
    public float matchScore;
    
    public HeroTaskMatch(SaveHeroData h, int id, float score)
    {
        hero = h;
        devId = id;
        matchScore = score;
    }
}

public class HeroTaskMatcher
{
    private static string NormalizeAttrName(string attr)
    {
        return attr.ToLower();
    }
    
    public static float CalculateMatchScore(SaveHeroData hero, CityDevConfig config)
    {
        if (config.Attrs == null || config.Attrs.Length == 0)
            return 50f;
        
        float totalScore = 0f;
        int attrCount = config.Attrs.Length;
        
        foreach (var attr in config.Attrs)
        {
            string normalizedAttr = NormalizeAttrName(attr);
            int attrValue = hero.GetAttr(normalizedAttr);
            totalScore += attrValue;
        }
        
        float avgScore = totalScore / attrCount;
        
        return avgScore;
    }
    
    public static HeroTaskMatch FindBestTask(SaveHeroData hero, List<TaskPriorityInfo> availableTasks)
    {
        HeroTaskMatch bestMatch = null;
        float bestScore = -1f;
        
        foreach (var taskInfo in availableTasks)
        {
            float matchScore = CalculateMatchScore(hero, taskInfo.config);
            float combinedScore = matchScore + taskInfo.adjustedPriority * 0.5f;
            
            if (combinedScore > bestScore)
            {
                bestScore = combinedScore;
                bestMatch = new HeroTaskMatch(hero, taskInfo.devId, combinedScore);
            }
        }
        
        return bestMatch;
    }
    
    public static List<HeroTaskMatch> AssignTasksToHeroes(List<SaveHeroData> heroes, List<TaskPriorityInfo> availableTasks)
    {
        var assignments = new List<HeroTaskMatch>();
        var usedHeroes = new HashSet<int>();
        var taskUsage = new Dictionary<int, int>();
        
        var sortedTasks = new List<TaskPriorityInfo>(availableTasks);
        sortedTasks.Sort((a, b) => b.adjustedPriority.CompareTo(a.adjustedPriority));
        
        foreach (var taskInfo in sortedTasks)
        {
            var heroScores = new List<HeroMatchScore>();
            
            foreach (var hero in heroes)
            {
                if (usedHeroes.Contains(hero.heroId))
                    continue;
                
                float score = CalculateMatchScore(hero, taskInfo.config);
                heroScores.Add(new HeroMatchScore(hero, score));
            }

            if (heroScores.Count == 0)
                continue;
            
            heroScores.Sort((a, b) => b.score.CompareTo(a.score));
            
            int maxHeroesPerTask = Mathf.Min(3, heroScores.Count);
            if(heroScores[0].score < 70)
                maxHeroesPerTask = 1;
            
            for (int i = 0; i < maxHeroesPerTask; i++)
            {
                var matchedHero = heroScores[i];
                assignments.Add(new HeroTaskMatch(matchedHero.hero, taskInfo.devId, matchedHero.score));
                usedHeroes.Add(matchedHero.hero.heroId);
            }
        }
        
        return assignments;
    }
    
    private class HeroMatchScore
    {
        public SaveHeroData hero;
        public float score;
        
        public HeroMatchScore(SaveHeroData h, float s)
        {
            hero = h;
            score = s;
        }
    }
    
    public static Dictionary<int, List<int>> AssignHeroesToTasks(
        List<SaveHeroData> availableHeroes,
        List<TaskPriorityInfo> availableTasks)
    {
        var result = new Dictionary<int, List<int>>();
        var assignedHeroes = new HashSet<int>();
        
        var sortedTasks = new List<TaskPriorityInfo>(availableTasks);
        sortedTasks.Sort((a, b) => b.adjustedPriority.CompareTo(a.adjustedPriority));
        
        foreach (var taskInfo in sortedTasks)
        {
            if (assignedHeroes.Count >= availableHeroes.Count)
                break;
            
            var heroScores = new List<HeroMatchScore>();
            
            foreach (var hero in availableHeroes)
            {
                if (assignedHeroes.Contains(hero.heroId))
                    continue;
                
                float score = CalculateMatchScore(hero, taskInfo.config);
                heroScores.Add(new HeroMatchScore(hero, score));
            }
            
            heroScores.Sort((a, b) => b.score.CompareTo(a.score));
            
            int heroCount = Mathf.Min(taskInfo.config.HeroCount, heroScores.Count);
            
            if (heroCount > 0 && heroScores.Count > 0)
            {
                var heroesForTask = new List<int>();
                
                for (int i = 0; i < heroCount && i < heroScores.Count; i++)
                {
                    heroesForTask.Add(heroScores[i].hero.heroId);
                    assignedHeroes.Add(heroScores[i].hero.heroId);
                }
                
                if (heroesForTask.Count > 0)
                {
                    result[taskInfo.devId] = heroesForTask;
                }
            }
        }
        
        return result;
    }
}
