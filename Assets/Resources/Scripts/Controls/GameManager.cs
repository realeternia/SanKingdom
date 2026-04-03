using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using CommonConfig;

public class GameManager : MonoBehaviour
{   
    public static GameManager Instance;
    // todo 这里还有一个city数据列表
    private StreamWriter logWriter;  // 日志写入器
    public SaveData SaveData;
    
    // 游戏时间常量
    public const int BASE_YEAR = 194; // 游戏起始年份
    public const int BORN_AGE = 16;
    public const int SEASONS_PER_YEAR = 36; // 一年的季节数

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

        UnityEngine.Debug.Log("GameManager Start");
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
        // 取消注册日志事件
        Application.logMessageReceived -= LogMessageReceived;
        
        // 关闭日志文件
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
            if(member.state == HeroState.Normal && member.forceId == forceId && member.loyalty < 100)
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
            city.gold = cityCfg.Gold;
            city.food = cityCfg.Food;
            city.soldier = cityCfg.Soldier;
            city.secure = cityCfg.Secure;
            city.power = 70;
            city.wall = cityCfg.Wall;
            city.archFood = cityCfg.ArchFood;
            city.archGold = cityCfg.ArchGold;
            city.archPeople = cityCfg.ArchPeople;

            SaveData.cities.Add(city);
        }
        foreach(var heroCfg in HeroConfig.ConfigList)
        {
            if(string.IsNullOrEmpty(heroCfg.City))
                continue;
            var cityCfg = WorldConfig.ConfigList.FirstOrDefault(c => c.Cname == heroCfg.City);
            if(cityCfg == null)
                continue;
            if(BASE_YEAR - heroCfg.BornYear  < BORN_AGE) //15岁才能登场
                continue;

            var hero = new SaveHeroData { heroId = heroCfg.Id, cityOwner = false, cityId = cityCfg.Id, state = HeroState.Normal, loyalty = heroCfg.Loyal, forceId = cityCfg.ForceId };
            SaveData.heros.Add(hero);
        }
        foreach(var city in SaveData.cities)
        {
            city.SelectOwner();
            city.AutoSetSoldierOnInit();
        }
        foreach(var force in ForceConfig.ConfigList)
        {
            if(force.Id > 90)
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

        Debug.Log("NextRound round=" + SaveData.round);

        forbidPlayerAct = true;
        StartCoroutine(NextRoundCoroutine());
    }

    private void ProcessHeros()
    {
        foreach (var hero in SaveData.heros)
        {
            if (hero.state == HeroState.Catched)
            {
                hero.loyalty -= UnityEngine.Random.Range(1, 4);
                if (hero.loyalty < 0)
                    hero.loyalty = 0;

                if (UnityEngine.Random.Range(0, 100) < 15)
                {
                    var destCityId = GetRandomForceCityId(hero.cityId, hero.forceId);
                    if (destCityId > 0)
                    {
                        hero.state = HeroState.Normal;
                        hero.cityId = destCityId;
                    }
                }
            }
            else if (hero.state == HeroState.Wild)
            {
                if (UnityEngine.Random.Range(0, 100) < 20)
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
            return (SaveData.round % SEASONS_PER_YEAR) + 1;
        }
    }

    // 获取当前年份（包含季节的浮点数表示）
    // 例如：195年第18个季节 = 195.5
    public float GetCurrentYear()
    {
        // 计算当前年份和季节
        int totalSeasons = SaveData.round;
        int years = totalSeasons / SEASONS_PER_YEAR;
        int seasons = totalSeasons % SEASONS_PER_YEAR;
        
        // 转换为浮点数表示
        return BASE_YEAR + years + (seasons / (float)SEASONS_PER_YEAR);
    }

    private IEnumerator NextRoundCoroutine()
    {
        foreach (var player in players)
        {
            PanelManager.Instance.SendSignal("AICheck", player.pname, player.forceId);

            // 跳过玩家势力
            if (player.IsPlayer)
                continue;
            AI.ExecuteAiActions(player);

            yield return new WaitForSeconds(0.23f);
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

            Debug.Log("游戏数据加载成功 year=" + SaveData.round);
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载游戏数据失败: " + e.Message);
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
            
            Debug.Log("游戏数据保存成功: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存游戏数据失败: " + e.Message);
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
