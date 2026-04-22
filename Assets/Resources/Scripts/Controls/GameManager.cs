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

    public List<Player> players = new List<Player>();
    
    public Player currentPlayer = null;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        // 初始化日志文件
        string logPath = Application.persistentDataPath + "/game_log.txt";
        logWriter = new StreamWriter(logPath, false, System.Text.Encoding.UTF8); 
        logWriter.WriteLine("Game started at: " + System.DateTime.Now);

        ConfigManager.Init();
        
        // 注册日志事件
        Application.logMessageReceived += LogMessageReceived;

        GameLog.Info("GameManager Start");
    }

    // 日志处理函数
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
            logWriter.Flush();  // 立即写入文件
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
  

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public Player GetPlayer(int forceId)
    {
        return players.Find(p => p.forceId == forceId);
    }

    private void SortPlayers()
    {
        players.Sort((a, b) =>
        {
            if (a.IsPlayer != b.IsPlayer)
                return a.IsPlayer ? -1 : 1;
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

    public List<int> GetNearbyForceCityIds(int fromCityId, int forceId)
    {
        var result = new List<int>();
        var fromCityCfg = WorldConfig.GetConfig(fromCityId);
        if (fromCityCfg == null || fromCityCfg.WorldNearIds == null)
            return result;

        foreach (var nearCityId in fromCityCfg.WorldNearIds)
        {
            var nearCity = GetCity(nearCityId);
            if (nearCity != null && nearCity.forceId == forceId)
            {
                result.Add(nearCityId);
            }
        }
        return result;
    }

    public int GetRandomForceCityId(int fromCityId, int forceId)
    {
        var nearbyCityIds = GetNearbyForceCityIds(fromCityId, forceId);
        if (nearbyCityIds.Count > 0)
        {
            return nearbyCityIds[SysRandom.Range(0, nearbyCityIds.Count)];
        }

        var kingCity = GetPlayer(forceId)?.GetKingCity();
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

  //新游戏开始数据初始化
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
            if(SystemConst.Game.BASE_YEAR - heroCfg.BornYear  < SystemConst.Game.BORN_AGE) //15岁才能登场
                continue;

            var hero = new SaveHeroData { heroId = heroCfg.Id, cityId = cityCfg.Id, state = HeroState.Normal, loyalty = heroCfg.Loyal, forceId = cityCfg.ForceId, armsId = SystemConst.Hero.DEFAULT_ARMS_ID };
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
            var forceData = new SaveForceData { forceId = force.Id, gold = force.InitGold, food = force.InitFood };
            if(force.Id == forceId)
                forceData.isPlayer = true;
            SaveData.forces.Add(forceData); 
        }
        foreach (var forceData in SaveData.forces)
        {
            players.Add(new Player(forceData.forceId));
        }

        SortPlayers();

        SaveToFile();

        foreach (var p in players)
            p.ResetRoundState();
        SaveData.currentPlayerIndex = 0;
        StartNextPlayerTurn();
    }

    public void NextRound()
    {
        SaveData.round++;
        
        PanelManager.Instance.SendSignal("RoundChange", "", SaveData.round);

        foreach(var city in SaveData.cities)
        {
            city.OnRound();
        }

        ProcessHeros();
        
        foreach (var p in players)
            p.ResetRoundState();
        SaveData.currentPlayerIndex = 0;
        SortPlayers();
        
        StartNextPlayerTurn();

        GameLog.Info("NextRound round=" + SaveData.round);
    }
    
    private void StartNextPlayerTurn()
    {
        if (SaveData.currentPlayerIndex >= players.Count)
        {
            EndRound();
            return;
        }
        
        currentPlayer = players[SaveData.currentPlayerIndex];
        SaveData.currentPlayerIndex++;
        
        StartPlayerPlanningPhase(currentPlayer);
    }
    
    private void StartPlayerPlanningPhase(Player player)
    {
        player.StartPlanningPhase();
        if (player.IsPlayer)
        {
            SaveToFile();
        }
        GameLog.Info($"StartPlayerPlanningPhase {player.pname} 计划阶段");
    }
    
    public IEnumerator AIPlayerTurnCoroutine(Player player)
    {
        yield return new WaitForSeconds(0.3f);
        GameLog.Info($"AI {player.pname} idle 回合完成");
        StartNextPlayerTurn();
    }
    
    public void ConfirmPlan(int forceId)
    {
        if (currentPlayer == null || currentPlayer.forceId != forceId)
            return;
        
        GameLog.Info($"ConfirmPlan forceId={forceId}");
        
        StartCoroutine(PlayerTurnCoroutine(currentPlayer));
    }
    
    private IEnumerator PlayerTurnCoroutine(Player player)
    {
        player.SetPhase(TurnPhase.Execution);
        PanelManager.Instance.SendSignal("PhaseChange", "Execution", player.forceId);
        
        yield return ExecutePlayerDevActions(player);
        
        if (player.warPlans.Count > 0)
        {
            player.SetPhase(TurnPhase.Battle);
            PanelManager.Instance.SendSignal("PhaseChange", "Battle", player.forceId);
            
            foreach (var warPlan in player.warPlans)
            {
                player.ExecuteCityBattleDev(
                    warPlan.sourceCityId,
                    CityDevConfig.ConfigList.FirstOrDefault(c => c.Prefab == "CityDevBattle")?.Id ?? 0,
                    warPlan.heroIds,
                    warPlan.foodCost,
                    warPlan.targetCityId,
                    true,
                    warPlan.heroSoldierDict,
                    warPlan.heroArmsDict
                );
                
                while (BattleManager.Instance.IsBattleRunning)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        
        GameLog.Info($"玩家 {player.pname} 回合完成");
        StartNextPlayerTurn();
    }
    
    private IEnumerator ExecutePlayerDevActions(Player player)
    {
        var cities = GetCitiesByForce(player.forceId);
        foreach (var city in cities)
        {
            var assignments = city.GetDevAssignments();
            foreach (var assignment in assignments)
            {
                var heroIds = new int[] { assignment.heroId };
                var devCfg = CityDevConfig.GetConfig(assignment.devId);
                
                if (devCfg == null) continue;
                
                if (devCfg.Prefab == "CityDevNormal")
                {
                    player.ExecuteCityDev(city.cityId, assignment.devId, heroIds, out _);
                }
                else if (devCfg.Prefab == "CityDevChange")
                {
                    player.ExecuteCityChange(city.cityId, assignment.devId, heroIds, true, 300, SystemConst.Economy.EXCHANGE_RATE, out _);
                }
                else if (devCfg.Prefab == "CityDevUseHero")
                {
                    var recruitableHeroes = city.GetRecruitableHeroList();
                    if (recruitableHeroes.Count > 0)
                    {
                        player.ExecuteCityUseHero(city.cityId, assignment.devId, assignment.heroId, recruitableHeroes[0], out _);
                    }
                }
                else if (devCfg.Prefab == "CityDevPraiseHero")
                {
                    var praiseableHeroes = GetPraiseableHeroList(player.forceId);
                    if (praiseableHeroes.Count > 0)
                    {
                        player.ExecuteCityPraiseHero(city.cityId, assignment.devId, praiseableHeroes.ToArray(), 1, out _);
                    }
                }
                
                yield return null;
            }
        }
    }
    
    public void EndRound()
    {
        if (currentPlayer != null)
            currentPlayer.SetPhase(TurnPhase.None);
        currentPlayer = null;
        SaveData.currentPlayerIndex = 0;
        
        PanelManager.Instance.SendSignal("AICheck", "", 0);
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
                    var cityCfg = WorldConfig.GetConfig(hero.cityId);
                    if (cityCfg != null && cityCfg.WorldNearIds != null && cityCfg.WorldNearIds.Length > 0)
                    {
                        int randomIndex = SysRandom.Range(0, cityCfg.WorldNearIds.Length);
                        hero.cityId = cityCfg.WorldNearIds[randomIndex];
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

    // 获取当前年份（包含季节的浮点数表示）
    // 例如：195年第18个季节 = 195.5
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
            foreach (var forceData in SaveData.forces)
            {
                players.Add(new Player(forceData.forceId));
            }
            SortPlayers();

            foreach (var p in players)
                p.ResetRoundState();
            StartNextPlayerTurn();

            GameLog.Info("游戏数据加载成功 year=" + SaveData.round);
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
            
            // 使用JsonUtility序列化数据
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
