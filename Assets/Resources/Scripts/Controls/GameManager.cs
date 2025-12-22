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
    public GameObject topNode;

    public List<Player> players = new List<Player>();

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
    
    public Player GetPlayer(int idx)
    {
        return players.Find(p => p.pid == idx);
    }

    public SaveCityData GetCity(int cityId)
    {
        return SaveData.cities.FirstOrDefault(c => c.cityId == cityId);
    }

    public int GetHeroCity(int heroId, out int forceId)
    {
        forceId = -1;
        foreach (var city in SaveData.cities)
        {
            forceId = city.forceId;
            var heroList = city.GetHeroList();
            if (heroList != null && heroList.Contains(heroId))
                return city.cityId;
        }
        return -1;
    }
  //新游戏开始数据初始化
    public void NewGame(int forceId)
    {
        SaveData = new SaveData();
        SaveData.year = 0;
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
            city.heros = new List<SaveHeroData>();
            if(cityCfg.Leader > 0)
                city.heros.Add(new SaveHeroData { heroId = cityCfg.Leader, cityOwner = true });
            if(cityCfg.Members != null)
                city.heros.AddRange(cityCfg.Members.Select(m => new SaveHeroData { heroId = m }));
            SaveData.cities.Add(city);
        }
        foreach(var force in ForceConfig.ConfigList)
        {
            var forceData = new SaveForceData { forceId = force.Id };
            if(force.Id == forceId)
                forceData.isPlayer = true;
            SaveData.forces.Add(forceData); 
        }
        InitForceControls();
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
            InitForceControls();

            Debug.Log("游戏数据加载成功 year=" + SaveData.year);
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

    public void InitForceControls()
    {
        var playerForceControl = Resources.Load<GameObject>("Prefabs/Panels/PlayerInfoCell");
        int idx = 0;
        var totalWidth = 188 * SaveData.forces.Count;
        foreach(var force in SaveData.forces)
        {
            var forceControl = Instantiate(playerForceControl, topNode.transform);
            var playerInfoControl = forceControl.GetComponent<PlayerInfoControl>();
            playerInfoControl.Init(idx, force.forceId);
            forceControl.GetComponent<RectTransform>().anchoredPosition = new Vector2(-totalWidth / 2 + 188 * idx, 412);
            players.Add(playerInfoControl.player);
            idx++;
        }
    }
}
