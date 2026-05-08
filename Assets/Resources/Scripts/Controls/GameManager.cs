using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using CommonConfig;
using Controls.Utils;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance;
    private StreamWriter logWriter;
    public SaveData SaveData;

    private int currentForceId = 0;

    public SaveForceData CurrentForce 
    { 
        get { return GetForce(currentForceId); } 
    }

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        string logPath = Application.persistentDataPath + "/game_log.txt";
        logWriter = new StreamWriter(logPath, false, System.Text.Encoding.UTF8); 
        logWriter.WriteLine("Game started at: " + System.DateTime.Now);

        ConfigManager.Init();
        
        Application.logMessageReceived += LogMessageReceived;

        GameLog.Info("GameManager Start");
    }

    private void LogMessageReceived(string logString, string stackTrace, LogType type)
    {
        if (logWriter != null)
        {
            if(logString.Contains("font asset"))
                return;
            string logType = type.ToString();
            logWriter.WriteLine($"[{System.DateTime.Now}] [{logType}] {logString}");
            if (!string.IsNullOrEmpty(stackTrace))
            {
                logWriter.WriteLine($"Stack Trace: {stackTrace}");
            }
            logWriter.Flush();
        }
    }

    private void OnDestroy()
    {
        GameLog.Shutdown();
        
        Application.logMessageReceived -= LogMessageReceived;
        
        if (logWriter != null)
        {
            logWriter.WriteLine("Game ended at: " + System.DateTime.Now);
            logWriter.Close();
            logWriter = null;
        }
    }
  

    void Update()
    {
        
    }
    
    private void SortForces()
    {
        SaveData.forces.Sort((a, b) =>
        {
            if (a.isPlayer != b.isPlayer)
                return a.isPlayer ? -1 : 1;
            return a.forceId - b.forceId;
        });
    }

    public SaveForceData GetForce(int forceId)
    {
        return SaveData.forces.FirstOrDefault(f => f.forceId == forceId);
    }

    public SaveCityData GetCity(int cityId)
    {
        return SaveData.cities.FirstOrDefault(c => c.cityId == cityId);
    }

    public List<SaveCityData> GetCitiesByForce(int forceId)
    {
        return SaveData.cities.Where(c => c.forceId == forceId).ToList();
    }


    public int GetRandomForceCityId(int fromCityId, int forceId)
    {
        var nearbyCityIds = MapTool.GetAdjacentFriendlyCityIds(fromCityId, forceId);
        if (nearbyCityIds.Count > 0)
        {
            return nearbyCityIds[SysRandom.Range(0, nearbyCityIds.Count)];
        }

        var force = GetForce(forceId);
        var kingCity = force?.GetKingCity();
        if (kingCity != null && fromCityId != kingCity.cityId)
        {
            return kingCity.cityId;
        }

        var forceCities = GetCitiesByForce(forceId);
        if (forceCities.Count > 0)
        {
            return forceCities[SysRandom.Range(0, forceCities.Count)].cityId;
        }

        return 0;
    }

    public SaveHeroData GetHero(int heroId)
    {
        return SaveData.heros.FirstOrDefault(h => h.heroId == heroId);
    }

    public List<SaveHeroData> GetHerosByForce(int forceId)
    {
        return SaveData.heros.Where(h => h.forceId == forceId).ToList();
    }

    public List<int> GetPraiseableHeroList(int forceId)
    {
        var heroIds = new List<int>();
        foreach (var member in SaveData.heros)
        {
            if(member.state == HeroState.Normal && member.forceId == forceId && member.loyalty < SystemConst.Hero.MAX_LOYALTY)
                heroIds.Add(member.heroId);
        }
        return heroIds;
    }

    public void NewGame(int forceId)
    {
        SaveData = new SaveData();
        SaveData.round = 1;
        foreach(var cityCfg in WorldConfig.ConfigList)
        {
            var city = new SaveCityData();
            city.cityId = cityCfg.Id;
            city.forceId = cityCfg.ForceId;
            city.level = cityCfg.Level;
            city.exp = 0;
            city.soldier = cityCfg.Soldier;
            city.happy = SystemConst.City.INITIAL_CITY_HAPPY;
            city.food = cityCfg.Food;
            city.wall = cityCfg.Wall;

            SaveData.cities.Add(city);
        }
        foreach(var heroCfg in HeroConfig.ConfigList)
        {
            if(string.IsNullOrEmpty(heroCfg.City))
                continue;
            var cityCfg = WorldConfig.ConfigList.FirstOrDefault(c => c.Cname == heroCfg.City);
            if(cityCfg == null)
                continue;
            if(SystemConst.Game.BASE_YEAR - heroCfg.BornYear  < SystemConst.Game.BORN_AGE)
                continue;

            var hero = new SaveHeroData { heroId = heroCfg.Id, cityId = cityCfg.Id, state = HeroState.Normal, loyalty = heroCfg.Loyal, forceId = cityCfg.ForceId };
            hero.InitAttrsFromConfig();
            SaveData.heros.Add(hero);
        }
        foreach(var city in SaveData.cities)
        {
            city.SelectOwner();
        }
        foreach(var force in ForceConfig.ConfigList)
        {
            if(force.Id > SystemConst.Game.MAX_FORCE_ID)
                continue;
            var forceData = new SaveForceData { forceId = force.Id, gold = force.InitGold };
            if(force.Id == forceId)
                forceData.isPlayer = true;
            forceData.InitRuntimeState();
            SaveData.forces.Add(forceData); 
        }

        SortForces();

        foreach (var forceData in SaveData.forces)
            forceData.ResetRoundState();
        SaveData.currentForceIndex = 0;
        StartNextForceTurn();

        SaveToFile();        
    }

    public void NextRound()
    {
        SaveData.round++;
        
        foreach(var city in SaveData.cities)
        {
            city.OnRound();
        }

        ProcessHeros();
        
        foreach (var forceData in SaveData.forces)
            forceData.ResetRoundState();
        SaveData.currentForceIndex = 0;
        SortForces();
        StartNextForceTurn();
        
        PanelManager.Instance.SendSignal(new RoundChangeSignal { Round = SaveData.round });
        SaveToFile();

        GameLog.Info("NextRound round=" + SaveData.round);
    }
    
    private void StartNextForceTurn()
    {       
        if (SaveData.currentForceIndex >= SaveData.forces.Count)
        {
            EndRound();
            return;
        }
        
        var force = SaveData.forces[SaveData.currentForceIndex];

        StartForcePlanningPhase(force);
    }
    
    private void StartForcePlanningPhase(SaveForceData force)
    {
        currentForceId = force.forceId;
        force.StartPlanningPhase();
        GameLog.Info($"StartForcePlanningPhase {force.Name} 计划阶段");
    }
    
    public IEnumerator AIForceTurnCoroutine(SaveForceData force)
    {
        yield return new WaitForSeconds(0.3f);
        AI.ExecutePlanningPhase(force);
    }
    
    public void ConfirmPlan(int forceId)
    {
        var force = GetForce(forceId);
        if (force == null || currentForceId != forceId)
            return;
        
        GameLog.Info($"ConfirmPlan forceId={forceId}");
        
        StartCoroutine(ForceTurnCoroutine(force));
    }
    
    private IEnumerator ForceTurnCoroutine(SaveForceData force)
    {
        force.SetPhase(TurnPhase.Execution);
        PanelManager.Instance.SendSignal(new PhaseChangeSignal { PhaseName = "Execution", ForceId = force.forceId });
        
        yield return ExecuteForceDevActions(force);
        
        if (force.warPlans.Count > 0)
        {
            force.SetPhase(TurnPhase.Battle);
            PanelManager.Instance.SendSignal(new PhaseChangeSignal { PhaseName = "Battle", ForceId = force.forceId });
            
            foreach (var warPlan in force.warPlans)
            {
                // force.ExecuteCityBattleDev(
                //     warPlan.sourceCityId,
                //     CityDevConfig.ConfigList.FirstOrDefault(c => c.Prefab == "CityDevBattle")?.Id ?? 0,
                //     warPlan.heroIds,
                //     warPlan.foodCost,
                //     warPlan.targetCityId,
                //     true,
                //     warPlan.heroSoldierDict,
                //     warPlan.heroArmsDict
                // );
                
                // while (BattleManager.Instance.IsBattleRunning)
                // {
                //     yield return new WaitForSeconds(0.1f);
                // }
            }
        }
        
        GameLog.Info($"势力 {force.Name} 回合完成");

        SaveData.currentForceIndex++; 
        StartNextForceTurn();
    }
    
    private IEnumerator ExecuteForceDevActions(SaveForceData force)
    {
        var cities = GetCitiesByForce(force.forceId);
        foreach (var city in cities)
        {
            var assignments = city.GetDevAssignments();
            var attrChanges = new Dictionary<string, int>();
            
            foreach (var assignment in assignments)
            {
                var heroIds = new int[] { assignment.heroId };
                var devCfg = CityDevConfig.GetConfig(assignment.devId);
                
                if (devCfg == null) continue;
                
                List<PopResultPanelManager.AttrData> attrDatas = null;
                
                if (devCfg.Prefab == "CityDevNormal")
                {
                    force.ExecuteCityDev(city.cityId, assignment.devId, heroIds, out attrDatas);
                }
                else if (devCfg.Prefab == "CityDevChange")
                {
                    force.ExecuteCityChange(city.cityId, assignment.devId, heroIds, true, 300, SystemConst.Economy.EXCHANGE_RATE, out attrDatas);
                }
                else if (devCfg.Prefab == "CityDevUseHero")
                {
                    var recruitableHeroes = city.GetRecruitableHeroList();
                    if (recruitableHeroes.Count > 0)
                    {
                        force.ExecuteCityUseHero(city.cityId, assignment.devId, assignment.heroId, recruitableHeroes[0], out attrDatas);
                    }
                }
                else if (devCfg.Prefab == "CityDevPraiseHero")
                {
                    var praiseableHeroes = GetPraiseableHeroList(force.forceId);
                    if (praiseableHeroes.Count > 0)
                    {
                        force.ExecuteCityPraiseHero(city.cityId, assignment.devId, praiseableHeroes.ToArray(), 1, out attrDatas);
                    }
                }
                
                if (attrDatas != null)
                {
                    foreach (var attrData in attrDatas)
                    {
                        if (attrData == null)
                            continue;
                        if (attrData.valAddon == 0)
                            continue;
                        if (string.IsNullOrEmpty(attrData.attr))
                            continue;
                        string attrName = attrData.attr.ToLower();
                        if (!attrChanges.ContainsKey(attrName))
                            attrChanges[attrName] = 0;
                        attrChanges[attrName] += attrData.valAddon;
                    }
                }
                
                yield return null;
            }
            
            if (attrChanges.Count > 0)
            {
                var changeStrs = attrChanges.Select(kvp => $"{CityAttrConfig.GetConfigByname(kvp.Key).Cname}{(kvp.Value >= 0 ? "+" : "")}{kvp.Value}").ToArray();
                GameLog.SetTag("AI").Info($"[{ConfigNameHelper.GetForceName(force.forceId)}] [{ConfigNameHelper.GetCityName(city.cityId)}] 收入: {string.Join(", ", changeStrs)}");
            }
        }
    }
    
    public void EndRound()
    {
        if (CurrentForce != null)
            CurrentForce.SetPhase(TurnPhase.None);
        currentForceId = 0;
        SaveData.currentForceIndex = 0;
        
        PanelManager.Instance.SendSignal(new AICheckSignal { ForceId = 0 });
        PanelManager.Instance.SwitchBGM();
        
        GameLog.Info("EndRound");
        
        NextRound();
    }
    
    private void ProcessHeros()
    {
        foreach (var hero in SaveData.heros)
        {
            if (hero.state == HeroState.Catched)
            {
                hero.loyalty -= SysFormula.Hero.CalculateCapturedLoyaltyDecay();
                if (hero.loyalty < 0)
                    hero.loyalty = 0;

                var city = GetCity(hero.cityId);
                if (SysFormula.Hero.CheckEscape())
                {
                    var destCityId = GetRandomForceCityId(hero.cityId, hero.forceId);
                    if (destCityId > 0)
                    {
                        if (city != null)
                        {
                            city.RemoveDevAssignment(hero.heroId);
                        }
                        hero.state = HeroState.Normal;
                        hero.cityId = destCityId;
                    }
                }
            }
            else if (hero.state == HeroState.Wild)
            {
                if (SysFormula.Hero.CheckWildHeroMove())
                {
                    var randomCityId = MapTool.GetRandomAdjacentCityId(hero.cityId);
                    if (randomCityId != 0)
                    {
                        hero.cityId = randomCityId;
                    }
                }
            }
        }
    }

    public int SeasonId
    {
        get
        {
            return SysFormula.Game.CalculateSeasonId(SaveData.round);
        }
    }

    public float GetCurrentYear()
    {
        return SysFormula.Game.CalculateCurrentYear(SaveData.round);
    }

    public bool IsGameSaveExist()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        
        if(!File.Exists(savePath))
            return false;
        return true;
    }

    public bool LoadFromSave()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        if (!File.Exists(savePath))
            return false;
        try
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            SaveData = saveData;

            SortForces();
            foreach (var forceData in SaveData.forces)
                forceData.InitRuntimeState();

            StartNextForceTurn();            

            GameLog.Info("游戏数据加载成功 year=" + SaveData.round + ", currentForceId=" + SaveData.forces[SaveData.currentForceIndex].forceId);
        }
        catch (System.Exception e)
        {
            GameLog.Error("加载游戏数据失败: " + e.Message);
            return false;
        }
        return true;
    }

    public void SaveToFile()
    {
        string savePath = Application.persistentDataPath + "/game_save.json";
        try
        {
            string json = JsonUtility.ToJson(SaveData);
            File.WriteAllText(savePath, json);
            
            GameLog.Info("游戏数据保存成功: " + savePath);
        }
        catch (System.Exception e)
        {
            GameLog.Error("保存游戏数据失败: " + e.Message);
        }
    }

    public int GetPlayerCityCount(int forceId)
    {
        int count = 0;
        foreach(var city in SaveData.cities)
        {
            if(city.forceId == forceId)
                count++;
        }
        return count;
    }

}
