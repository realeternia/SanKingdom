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
    public PlayerInfo[] players; //不能new，都是配置好的
    // todo 这里还有一个city数据列表
    // todo playersavedata也可以单独放一个，然后set给player
    private StreamWriter logWriter;  // 日志写入器
    public int year;
    public List<SaveCityData> cityDatas;

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

        players[0].Init(0, PlayerBook.GetWang());
        var pls = PlayerBook.GetRandomN(7);
        for (int i = 0; i < 7; i++)
            players[i + 1].Init(i + 1, pls[i]);

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

    public void ClearTurn()
    {
        foreach(var p in players)
            p.isOnTurn = false;    
    }

    public void OnPlayerTurn(int pid)
    {
        foreach(var p in players)
            p.isOnTurn = false;
        players[pid].isOnTurn = true;
    }

    public PlayerInfo GetPlayer(int pid)
    {
        return players[pid];
    }

    public PlayerInfo GetFirstNoAiPlayer()
    {
        foreach(var p in players)
            if(p.pid > 0 && !p.isAI)
                return p;
        return null;
    }

    public SaveCityData GetCity(int cityId)
    {
        return cityDatas.FirstOrDefault(c => c.cityId == cityId);
    }

    public int GetHeroCity(int heroId, out int forceId)
    {
        forceId = -1;
        foreach (var city in cityDatas)
        {
            forceId = city.forceId;
            if (city.leader == heroId)
                return city.cityId;
            if (city.members != null && city.members.Contains(heroId))
                return city.cityId;
        }
        return -1;
    }

    // 静态变量记录上次播放路径和 clip
    string lastPath = "";
    AudioClip lastClip = null;

    private int lastSoundPriority = -1;
    private float lastSoundTime = 0f;

    public void PlaySound(string path, int prioty = 3)
    {
        float currentTime = Time.time;
        // 如果当前优先级低于上一次且时间间隔小于1秒，则跳过播放
        if (prioty < lastSoundPriority && currentTime - lastSoundTime < 1.5f)
        {
            return;
        }

        // 更新上次播放信息
        lastSoundPriority = prioty;
        lastSoundTime = currentTime;
    
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (lastPath != path)
        {
            lastPath = path;
            lastClip = Resources.Load<AudioClip>(path);
            if (lastClip != null)
            {
                audioSource.clip = lastClip;
            }
        }

        if (audioSource.clip != null)
        {
            audioSource.Stop();
            audioSource.Play();
        }
    }

    //新游戏开始数据初始化
    public void NewGame()
    {
        cityDatas = new List<SaveCityData>();
        foreach(var cityData in WorldConfig.ConfigList)
        {
            var city = new SaveCityData();
            city.cityId = cityData.Id;
            city.forceId = cityData.ForceId;
            city.gold = cityData.Gold;
            city.food = cityData.Food;
            city.soldier = cityData.Soldier;
            city.secure = cityData.Secure;
            city.wall = cityData.Wall;
            city.archFood = cityData.ArchFood;
            city.archGold = cityData.ArchGold;
            city.archPeople = cityData.ArchPeople;
            city.leader = cityData.Leader;
            if(cityData.Members != null)
                city.members = new List<int>(cityData.Members);
            cityDatas.Add(city);
        }
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
            year = saveData.year;
            cityDatas = saveData.cities;

            Debug.Log("游戏数据加载成功 year=" + year);
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
            SaveData saveData = new SaveData();
            saveData.cities = cityDatas;
            saveData.year = year;
            
            // 使用JsonUtility序列化数据
            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(savePath, json);
            
            Debug.Log("游戏数据保存成功: " + savePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存游戏数据失败: " + e.Message);
        }
    }
}
