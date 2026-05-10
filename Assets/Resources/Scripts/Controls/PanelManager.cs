using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CommonConfig;
using Controls.Utils;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject rankPanel;
    private GameObject pickPanel;
    public GameObject worldPanel;
    private GameObject cityPanel;
    private GameObject cityDevPanel;
    private GameObject systemPanel;
    private GameObject cityBattlePanel;

    private GameObject popCitySelectPanel;
    private GameObject popHeroSelectPanel;
    private GameObject popHeroBattleSelectPanel;
    private GameObject popResultPanel;
    private GameObject popArmySetPanel;
    private GameObject heroInfoPanel;
    private GameObject battleResultPanel;
    private GameObject replayPanel;
    public GameObject sideBarPanel;

    public GameObject topNode;
    private Dictionary<string, ResItem> forceResItemDict = new Dictionary<string, ResItem>();

    public List<GameObject> openPanelList;
    private bool isShowWorld = false;

    // Start is called before the first frame update
    void Start()
    {
        ShowPick();
    }
  
    public void SwitchBGM()
    {
        var round = GameManager.Instance.SaveData.round;
        var seasonCfg = SeasonConfig.GetConfig((round % 12) + 1);
        BGMPlayer.Instance.PlayBGM("BGMs/" + seasonCfg.BGM);
    }

    public void ShowWorld()
    {
        worldPanel.SetActive(true);
        gameObject.SetActive(true);
        isShowWorld = true;

        SwitchBGM();
        InitTopNodeResItems();
    }

    public void HideWorld()
    {
        worldPanel.SetActive(false);
        gameObject.SetActive(false);
        isShowWorld = false;

        var roll = SysRandom.Range(0, 2);
        BGMPlayer.Instance.PlayBGM(roll == 0 ? "BGMs/weifeng" : "BGMs/pozhu");
    }

    public void InitTopNodeResItems()
    {
        GameLog.Debug($"PanelManager.InitTopNodeResItems topNode={topNode}");
        foreach (Transform child in topNode.transform)
        {
            Destroy(child.gameObject);
        }
        forceResItemDict.Clear();
        
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce == null) return;
        
        playerForce.RecalculateResUsed();
        
        int index = 0;
        foreach (var attrConfig in CityAttrConfig.ConfigList)
        {
            if (!attrConfig.IsForceAttr) continue;
            
            GameLog.Debug($"PanelManager.InitTopNodeResItems creating ResItem for {attrConfig.name}");
            var resBasePrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.ResBase());
            var resObj = Instantiate(resBasePrefab, topNode.transform);
            resObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(240 * index, 0);
            var resItem = resObj.GetComponent<ResItem>();
            resItem.Init(attrConfig.name);
            resItem.UpdateNum(playerForce.GetAttr(attrConfig.name));
            if (attrConfig.IsPosRes)
            {
                resItem.UpdateUsed(playerForce.GetResUsed(attrConfig.name));
            }
            forceResItemDict[attrConfig.name] = resItem;
            index++;
        }
        
        GameLog.Debug($"PanelManager.InitTopNodeResItems forceResItemDict.Count={forceResItemDict.Count}");
        UpdateForceResItemAddons();
    }
    
    public void RefreshForceResItems(int forceId)
    {
        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;
        
        force.RecalculateResUsed();
        
        foreach (var kvp in forceResItemDict)
        {
            string attrName = kvp.Key;
            var resItem = kvp.Value;
            resItem.UpdateNum(force.GetAttr(attrName));
            var attrConfig = CityAttrConfig.GetConfigByname(attrName);
            if (attrConfig.IsPosRes)
            {
                resItem.UpdateUsed(force.GetResUsed(attrName));
            }
        }
        
        var attrAddons = force.CalculateForceAttrAddons();
        foreach (var kvp in forceResItemDict)
        {
            string attrName = kvp.Key;
            var resItem = kvp.Value;
            var attrConfig = CityAttrConfig.GetConfigByname(attrName);
            if (attrConfig.IsPosRes)
                continue;
            
            float addonFloat = 0;
            attrAddons.TryGetValue(attrName.ToLower(), out addonFloat);
            resItem.UpdateAddon((int)Math.Floor(addonFloat));
        }
    }

    public void UpdateForceResItemAddons()
    {
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce == null) return;
        
        playerForce.RecalculateResUsed();
        
        var attrAddons = playerForce.CalculateForceAttrAddons();
        
        GameLog.Debug($"UpdateForceResItemAddons forceResItemDict={forceResItemDict.Count} attrAddons={attrAddons.Count}");
        
        foreach (var kvp in forceResItemDict)
        {
            string attrName = kvp.Key;
            var resItem = kvp.Value;
            var attrConfig = CityAttrConfig.GetConfigByname(attrName);
            if (attrConfig.IsPosRes)
            {
                resItem.UpdateUsed(playerForce.GetResUsed(attrName));
                continue;
            }
            
            float addonFloat = 0;
            attrAddons.TryGetValue(attrName.ToLower(), out addonFloat);
            int addon = (int)Math.Floor(addonFloat);
            GameLog.Debug($"UpdateForceResItemAddons attrName={attrName} addon={addon}");
            resItem.UpdateAddon(addon);
        }
    }

    private void RefreshTopNodeResItem(string attrName, int value, int used = -1)
    {
        foreach (Transform child in topNode.transform)
        {
            var resItem = child.GetComponent<ResItem>();
            if (resItem != null && resItem.attrName == attrName)
            {
                resItem.UpdateNum(value);
                if (used >= 0)
                    resItem.UpdateUsed(used);
                return;
            }
        }
    }

    public void ShowCity(int cityId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityPanel == null)
        {
            var cityPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityPanel"));
            cityPanel = Instantiate(cityPanelPrefab, transform);
        }
        cityPanel.SetActive(true);
        var cityPanelManager = cityPanel.GetComponent<CityPanelManager>();
        cityPanelManager.SetCityId(cityId);
        cityPanelManager.OnShow();

        ChangePanelCount(cityPanel, true);
    }

    public void HideCity()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityPanel.SetActive(false);
        cityPanel.GetComponent<CityPanelManager>().OnHide();

        ChangePanelCount(cityPanel, false);
        Destroy(cityPanel);
        cityPanel = null;
        
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce != null)
        {
            RefreshForceResItems(playerForce.forceId);
        }
    }
    
    public void ShowCityDev(int cityId, int devId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityDevPanel == null)
        {
            var cityDevPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityDevPanel"));
            cityDevPanel = Instantiate(cityDevPanelPrefab, transform);
        }
        cityDevPanel.SetActive(true);
        var cityDevPanelManager = cityDevPanel.GetComponent<CityDevPanelManager>();
        cityDevPanelManager.SetDev(cityId, devId);
        cityDevPanelManager.OnShow();

        ChangePanelCount(cityDevPanel, true);
    }

    public void HideCityDev()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityDevPanel.SetActive(false);
        cityDevPanel.GetComponent<CityDevPanelManager>().OnHide();

        ChangePanelCount(cityDevPanel, false);
        Destroy(cityDevPanel);
        cityDevPanel = null;
    }

    public void ShowSystemPanel()
    {
        if (systemPanel == null)
        {
            var systemPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("SystemInfoPanel"));
            systemPanel = Instantiate(systemPanelPrefab, transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        systemPanel.SetActive(true);
        systemPanel.GetComponent<SystemPanelManager>().OnShow();

        ChangePanelCount(systemPanel, true);
    }

    public void HideSystemPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        systemPanel.SetActive(false);
        systemPanel.GetComponent<SystemPanelManager>().OnHide();

        ChangePanelCount(systemPanel, false);
        Destroy(systemPanel);
        systemPanel = null;
    }

    public void ShowCityBattle(int forceId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityBattlePanel == null)
        {
            var cityBattlePanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityBattlePanel"));
            cityBattlePanel = Instantiate(cityBattlePanelPrefab, transform);
        }
        cityBattlePanel.SetActive(true);
        var cityBattlePanelManager = cityBattlePanel.GetComponent<CityBattlePanelManager>();
        cityBattlePanelManager.Init(forceId);
        cityBattlePanelManager.OnShow();

        ChangePanelCount(cityBattlePanel, true);
    }

    public void HideCityBattle()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityBattlePanel.SetActive(false);
        cityBattlePanel.GetComponent<CityBattlePanelManager>().OnHide();

        ChangePanelCount(cityBattlePanel, false);
        Destroy(cityBattlePanel);
        cityBattlePanel = null;
    }
  


    public void ShowRank()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (rankPanel == null)
        {
            var rankPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("RankInfoPanel"));
            rankPanel = Instantiate(rankPanelPrefab, transform);
        }        
        rankPanel.SetActive(true);
        rankPanel.GetComponent<RankPanelManager>().OnShow();

        ChangePanelCount(rankPanel, true);        
    }

    public void HideRank()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(false);
        rankPanel.GetComponent<RankPanelManager>().OnHide();

        ChangePanelCount(rankPanel, false);
        Destroy(rankPanel);

        ChangePanelCount(rankPanel, false);        
    }

    public void ShowPick()
    {
        //  BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (pickPanel == null)
        {
            var pickPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PickPanel"));
            pickPanel = Instantiate(pickPanelPrefab, transform);
        }
        pickPanel.SetActive(true);
        ChangePanelCount(pickPanel, true);
    }

    public void HidePick()
    {
     //   BGMPlayer.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(false);

        ChangePanelCount(pickPanel, false);
        Destroy(pickPanel);
        pickPanel = null;
    }

    public void ShowPopCitySelectPanel(List<int> cityIds, int currentCityId, System.Action<int> callback)
    {
        if (popCitySelectPanel == null)
        {
            popCitySelectPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PopCitySelectPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popCitySelectPanel.SetActive(true);
        popCitySelectPanel.GetComponent<PopCitySelectPanelManager>().OnShow(cityIds, currentCityId, callback);

        ChangePanelCount(popCitySelectPanel, true);
    }

    public void HidePopCitySelectPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popCitySelectPanel.SetActive(false);
        popCitySelectPanel.GetComponent<PopCitySelectPanelManager>().OnHide();

        ChangePanelCount(popCitySelectPanel, false);
        Destroy(popCitySelectPanel);
        popCitySelectPanel = null;
    }

    public void ShowPopHeroSelectPanel(int cityId, int selectCount, int[] heroList, int[] checkedList, string[] attrs, Action<List<int>> onSelectMethod, bool ignoreActionCheck = false)
    {
        if (popHeroSelectPanel == null)
        {
            popHeroSelectPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PopHeroSelectPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popHeroSelectPanel.SetActive(true);
        popHeroSelectPanel.GetComponent<PopHeroSelectPanelManager>().OnShow(cityId, selectCount, heroList, checkedList, attrs, onSelectMethod, ignoreActionCheck);

        ChangePanelCount(popHeroSelectPanel, true);
    }

    public void HidePopHeroSelectPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popHeroSelectPanel.SetActive(false);
        popHeroSelectPanel.GetComponent<PopHeroSelectPanelManager>().OnHide();

        ChangePanelCount(popHeroSelectPanel, false);
        Destroy(popHeroSelectPanel);
        popHeroSelectPanel = null;
    }

    public void ShowPopHeroBattleSelectPanel(int cityId, int selectCount, int[] heroList, bool allowZeroSoldier, int[] checkedList, Action<List<int>> onSelectMethod)
    {
        if (popHeroBattleSelectPanel == null)
        {
            popHeroBattleSelectPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PopHeroBattleSelectPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popHeroBattleSelectPanel.SetActive(true);
        popHeroBattleSelectPanel.GetComponent<PopHeroBattleSelectPanelManager>().OnShow(cityId, selectCount, heroList, allowZeroSoldier, checkedList, onSelectMethod);

        ChangePanelCount(popHeroBattleSelectPanel, true);
    }

    public void HidePopHeroBattleSelectPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popHeroBattleSelectPanel.SetActive(false);
        popHeroBattleSelectPanel.GetComponent<PopHeroBattleSelectPanelManager>().OnHide();

        ChangePanelCount(popHeroBattleSelectPanel, false);
        Destroy(popHeroBattleSelectPanel);
        popHeroBattleSelectPanel = null;
    }    

    public void ShowPopResultPanel(string title, List<PopResultPanelManager.AttrData> attrDatas, Action afterRun, string path)
    {
        if (popResultPanel == null)
        {
            popResultPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PopResultPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popResultPanel.SetActive(true);
        popResultPanel.GetComponent<PopResultPanelManager>().OnShow(title, attrDatas, afterRun, path);

        ChangePanelCount(popResultPanel, true);
    }

    public void HidePopResultPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popResultPanel.SetActive(false);
        popResultPanel.GetComponent<PopResultPanelManager>().OnHide();

        ChangePanelCount(popResultPanel, false);
        Destroy(popResultPanel);
        popResultPanel = null;
    }
    public void ShowPopArmySetPanel(int heroId)
    {
        if (popArmySetPanel == null)
        {
            popArmySetPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PopArmySetPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popArmySetPanel.SetActive(true);
        popArmySetPanel.GetComponent<PopArmySetManager>().OnShow(heroId);
    }

    public void HidePopArmySetPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popArmySetPanel.SetActive(false);
        popArmySetPanel.GetComponent<PopArmySetManager>().OnHide();

        ChangePanelCount(popArmySetPanel, false);
        Destroy(popArmySetPanel);
        popArmySetPanel = null;
    }

    public void ShowHeroInfoPanel(int[] heroList, int targetHeroId)
    {
        if (heroInfoPanel == null)
        {
            heroInfoPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("HeroInfoPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        heroInfoPanel.SetActive(true);
        heroInfoPanel.GetComponent<HeroInfoPanelManager>().Init(heroList, targetHeroId);

        ChangePanelCount(heroInfoPanel, true);
    }

    public void HideHeroInfoPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        heroInfoPanel.SetActive(false);
        heroInfoPanel.GetComponent<HeroInfoPanelManager>().OnHide();

        ChangePanelCount(heroInfoPanel, false);
        Destroy(heroInfoPanel);
        heroInfoPanel = null;
    }


    public void ShowBattleResultPanel(int battleId)
    {
        if (battleResultPanel == null)
        {
            battleResultPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("BattleResultPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        battleResultPanel.SetActive(true);
        battleResultPanel.GetComponent<BattleResultPanelManager>().OnShow(battleId);

        ChangePanelCount(battleResultPanel, true);
    }

    public void HideBattleResultPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        battleResultPanel.SetActive(false);
        battleResultPanel.GetComponent<BattleResultPanelManager>().OnHide();

        ChangePanelCount(battleResultPanel, false);
        Destroy(battleResultPanel);
        battleResultPanel = null;
    }

    public void ShowReplayPanel()
    {
        if (replayPanel == null)
        {
            replayPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("ReplayPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        replayPanel.SetActive(true);
        replayPanel.GetComponent<ReplayPanelManager>().OnShow();

        ChangePanelCount(replayPanel, true);
    }

    public void HideReplayPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        replayPanel.SetActive(false);
        replayPanel.GetComponent<ReplayPanelManager>().OnHide();

        ChangePanelCount(replayPanel, false);
        Destroy(replayPanel);
        replayPanel = null;
    }

    public void ShowSideBar(string panelName)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        sideBarPanel.SetActive(true);
        sideBarPanel.GetComponent<SideBar>().OnShow(panelName);
    }

    public void HideSideBar()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        sideBarPanel.GetComponent<SideBar>().OnHide(() =>
        {
            sideBarPanel.SetActive(false);
        });
    }

    public void SendSignal(SignalData data)
    {
        if (data.Name == "ForceResChange")
        {
            var signal = data as ForceResChangeSignal;
            RefreshTopNodeResItem(signal.ResType, signal.Value, signal.Used);
        }

        if(worldPanel != null)
        {
            worldPanel.GetComponent<MainPanelManager>().SendSignal(data);
        }
        foreach (var panel in openPanelList)
        {
            if (panel.TryGetComponent<IPanelEvent>(out IPanelEvent p))
            {
                p.SendSignal(data);
            }
        }
    }

    private void ChangePanelCount(GameObject panel, bool isShow)
    {
        if(isShow)
            openPanelList.Add(panel);
        else
            openPanelList.Remove(panel);
        if(openPanelList.Count <= 0 && !isShowWorld)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
