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
    private StreamWriter logWriter;
    public SaveData SaveData;

    private int currentForceId = 0;

    public SaveForceData CurrentForce 
    { 
        get { return GetForce(currentForceId); } 
    }

    // 防御面板相关字段
    private bool _waitingForDefenseSetup = false;
    private SaveForceData _pendingDestForce;
    private List<int> _pendingSrcCityIds;
    private List<SaveTroopsData> _pendingAttackTroops;
    private Dictionary<int, int> _pendingAttackSoldierMap;
    private int _pendingTargetCityId;

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

    public List<SaveTroopsData> GetTroopsByCity(int cityId)
    {
        return SaveData.troops.Where(t => t.cityId == cityId).ToList();
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

    public void NewGame(int forceId)
    {
        SaveData = new SaveData();
        SaveData.OnNewGame(forceId);
        StartNextForceTurn();

        SaveToFile();        
    }

    public void NextRound()
    {
        SaveData.OnRound();
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
                var citySrc = GetCity(warPlan.sourceCityId);
                var (attackTroops, attackSoldierMap) = TroopsBuilder.BuildAttackTroopsFromHeroList(citySrc, warPlan.heroIds, warPlan.heroSoldierDict, warPlan.heroArmsDict);
                var destForce = GetForce(GetCity(warPlan.targetCityId).forceId);

                if (!force.isPlayer && destForce.isPlayer)
                {
                    // AI攻击玩家，弹出防御面板
                    _waitingForDefenseSetup = true;
                    _pendingDestForce = destForce;
                    _pendingSrcCityIds = new List<int> { warPlan.sourceCityId };
                    _pendingAttackTroops = attackTroops;
                    _pendingAttackSoldierMap = attackSoldierMap;
                    _pendingTargetCityId = warPlan.targetCityId;
                    PanelManager.Instance.ShowCityBattle(destForce.forceId, warPlan.targetCityId, _pendingSrcCityIds, attackTroops, attackSoldierMap);
                    GameLog.Info($"AI势力 {force.Name} 进攻玩家城市 {warPlan.targetCityId}，等待玩家布防");
                    while (_waitingForDefenseSetup)
                    {
                        yield return null;
                    }
                }
                else
                {
                    force.ExecuteBattle(
                        new List<int> { warPlan.sourceCityId },
                        attackTroops,
                        attackSoldierMap,
                        warPlan.targetCityId,
                        !force.isPlayer
                    );
                }

                while (BattleManager.Instance.IsBattleRunning)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        
        GameLog.Info($"势力 {force.Name} 回合完成");

        SaveData.currentForceIndex++; 
        StartNextForceTurn();
    }

    public void OnDefenseConfirmed(List<SaveTroopsData> defenceTroops, Dictionary<int, int> defenceSoldierMap)
    {
        if (!_waitingForDefenseSetup)
        {
            GameLog.Warn("OnDefenseConfirmed 非等待防御状态");
            return;
        }
        GameLog.Info($"OnDefenseConfirmed 防御部队确认 targetCityId={_pendingTargetCityId} defenceCount={defenceTroops.Count}");
        var attackerForce = GetForce(GetCity(_pendingSrcCityIds[0]).forceId);
        attackerForce.ExecuteBattle(_pendingSrcCityIds, _pendingAttackTroops, _pendingAttackSoldierMap, _pendingTargetCityId, true,
            defenceTroops, defenceSoldierMap);
        _waitingForDefenseSetup = false;
    }

    public void StartTestDefense(SaveForceData attackerForce, int targetCityId, List<int> srcCityIds, List<SaveTroopsData> attackTroops, Dictionary<int, int> attackSoldierMap)
    {
        if (_waitingForDefenseSetup)
        {
            // 已在防御流程中，重新弹出面板
            PanelManager.Instance.ShowCityBattle(_pendingDestForce.forceId, _pendingTargetCityId, _pendingSrcCityIds, _pendingAttackTroops, _pendingAttackSoldierMap);
            GameLog.Info("StartTestDefense 防御面板重新弹出");
            return;
        }
        var destForce = GetForce(GetCity(targetCityId).forceId);
        _waitingForDefenseSetup = true;
        _pendingDestForce = destForce;
        _pendingSrcCityIds = srcCityIds;
        _pendingAttackTroops = attackTroops;
        _pendingAttackSoldierMap = attackSoldierMap;
        _pendingTargetCityId = targetCityId;
        PanelManager.Instance.ShowCityBattle(destForce.forceId, targetCityId, srcCityIds, attackTroops, attackSoldierMap);
        GameLog.Info($"StartTestDefense 势力{attackerForce.Name}进攻玩家城市{targetCityId}");
    }
    
    private IEnumerator ExecuteForceDevActions(SaveForceData force)
    {
        var cities = GetCitiesByForce(force.forceId);
        foreach (var city in cities)
        {
            var assignments = city.GetDevAssignments();
            var attrChanges = new Dictionary<string, float>();
            
            foreach (var assignment in assignments)
            {
                var heroIds = new int[] { assignment.heroId };
                var devCfg = CityDevConfig.GetConfig(assignment.devId);

                if (assignment.devId == SystemConst.CityDev.IDLE_DEV_ID)
                    continue;

                List<PopResultPanelManager.AttrData> attrDatas = null;

                if (devCfg.Type == "normal")
                {
                    force.ExecuteCityDev(city.cityId, assignment.devId, heroIds, out attrDatas);
                }
                else
                {
                    GameLog.Warn($"ExecuteForceDevActions 跳过非 normal 委派 devId={assignment.devId} type={devCfg.Type}");
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
                var changeStrs = attrChanges.Select(kvp => $"{CityAttrConfig.GetConfigByname(kvp.Key).Cname}{(kvp.Value >= 0 ? "+" : "")}{kvp.Value.ToString("F1")}").ToArray();
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

            SaveData.InitLoadedData();

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
        SaveData.BeforeSave();
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
