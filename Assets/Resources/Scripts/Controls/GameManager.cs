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
    // todo 这里还有一个city数据列表
    private StreamWriter logWriter;  // 日志写入器
    public SaveData SaveData;
    


    public List<Player> players = new List<Player>();
    public bool forbidPlayerAct = false;

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
            return nearbyCityIds[UnityEngine.Random.Range(0, nearbyCityIds.Count)];
        }

        var kingCity = GetPlayer(forceId)?.GetKingCity();
        if (kingCity != null && fromCityId != kingCity.cityId)
        {
            return kingCity.cityId;
        }

        var forceCities = GetCitiesByForce(forceId);
        if (forceCities.Count > 0)
        {
            return forceCities[UnityEngine.Random.Range(0, forceCities.Count)].cityId;
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
            city.gold = cityCfg.Gold;
            city.food = cityCfg.Food;
            city.soldier = cityCfg.Soldier;
            city.power = SystemConst.City.INITIAL_CITY_POWER;
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

            var hero = new SaveHeroData { heroId = heroCfg.Id, cityOwner = false, cityId = cityCfg.Id, state = HeroState.Normal, loyalty = heroCfg.Loyal, forceId = cityCfg.ForceId, armsId = SystemConst.Hero.DEFAULT_ARMS_ID };
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
            var forceData = new SaveForceData { forceId = force.Id };
            if(force.Id == forceId)
                forceData.isPlayer = true;
            SaveData.forces.Add(forceData); 
        }
        foreach (var forceData in SaveData.forces)
        {
            players.Add(new Player(forceData.forceId));
        }

        SaveToFile();        
    }

    // 下轮游戏
    public void NextRound()
    {
        SaveData.round++;

        foreach(var city in SaveData.cities)
        {
            city.OnRound();
        }

        ProcessHeros();

        GameLog.Info("NextRound round=" + SaveData.round);

        forbidPlayerAct = true;
        StartCoroutine(NextRoundCoroutine());
    }

    private void ProcessHeros()
    {
        foreach (var hero in SaveData.heros)
        {
            if (hero.state == HeroState.Catched)
            {
                hero.loyalty -= UnityEngine.Random.Range(SystemConst.Hero.CAPTURED_LOYALTY_DECAY_MIN, SystemConst.Hero.CAPTURED_LOYALTY_DECAY_MAX);
                if (hero.loyalty < 0)
                    hero.loyalty = 0;

                var city = GetCity(hero.cityId);
                int escapeChance = SystemConst.Hero.CAPTURED_ESCAPE_CHANCE;
                if (UnityEngine.Random.Range(0, 100) < escapeChance)
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
                if (UnityEngine.Random.Range(0, 100) < SystemConst.Hero.WILD_HERO_MOVE_CHANCE)
                {
                    var cityCfg = WorldConfig.GetConfig(hero.cityId);
                    if (cityCfg != null && cityCfg.WorldNearIds != null && cityCfg.WorldNearIds.Length > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, cityCfg.WorldNearIds.Length);
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
            return (SaveData.round % SystemConst.Game.SEASONS_PER_YEAR) + 1;
        }
    }

    // 获取当前年份（包含季节的浮点数表示）
    // 例如：195年第18个季节 = 195.5
    public float GetCurrentYear()
    {
        // 计算当前年份和季节
        int totalSeasons = SaveData.round;
        int years = totalSeasons / SystemConst.Game.SEASONS_PER_YEAR;
        int seasons = totalSeasons % SystemConst.Game.SEASONS_PER_YEAR;
        
        return SystemConst.Game.BASE_YEAR + years + (seasons / (float)SystemConst.Game.SEASONS_PER_YEAR);
    }

    private IEnumerator NextRoundCoroutine()
    {
        StrategicDecider.ClearRoundData();
        
        var playersCopy = new List<Player>(players);
        foreach (var player in playersCopy)
        {
            PanelManager.Instance.SendSignal("AICheck", player.pname, player.forceId);

            // 跳过玩家势力
            if (player.IsPlayer)
                continue;
            
            yield return AI.ExecuteAiActions(player);
        }

        PanelManager.Instance.SendSignal("AICheck", "", 0);
        PanelManager.Instance.SendSignal("RoundChange", "", SaveData.round);
        SaveToFile();
        forbidPlayerAct = false;
        PanelManager.Instance.SwitchBGM();
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
