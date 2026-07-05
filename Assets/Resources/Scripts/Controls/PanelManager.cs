using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private GameObject rankPanel;
    private GameObject pickPanel;
    public GameObject worldPanel;
    private GameObject cityPanel;
    private GameObject systemPanel;
    private GameObject cityBattlePanel;
    private GameObject cityMovePanel;
    private GameObject cityPraisePanel;
    private GameObject cityUseHeroPanel;
    private GameObject cityTradePanel;
    private GameObject citySearchPanel;

    private GameObject popResultPanel;
    private GameObject heroInfoPanel;
    private GameObject battleResultPanel;
    private GameObject replayPanel;
    private GameObject gmPanel;
    public GameObject sideBarPanel;

    public GameObject topNode; //显示顶部资源
    public TMP_Text cityName;
    public TMP_Text cityExp;
    public Image cityExpBar;
    public Image headIcon;
    public GameObject tipNode; //显示tooltip
    private GameObject currentTip;
    private Dictionary<string, ResItem> forceResItemDict = new Dictionary<string, ResItem>();

    public List<GameObject> openPanelList;
    private bool isShowWorld = false;

    // Start is called before the first frame update
    void Start()
    {
        topNode.SetActive(false);
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
        topNode.SetActive(true);
        gameObject.SetActive(true);
        isShowWorld = true;

        SwitchBGM();
        InitTopNodeResItems();
        ShowForceInfo();
    }

    public void ShowForceInfo()
    {
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce == null) return;
        ShowForceInfo(playerForce.forceId);
    }

    public void ShowForceInfo(int forceId)
    {
        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;

        if (cityName != null)
        {
            cityName.text = force.Name;
            cityName.color = SysColor.GetForceColor(forceId);
        }
        if (cityExpBar != null) cityExpBar.transform.parent.gameObject.SetActive(false);
        if (cityExp != null) cityExp.gameObject.SetActive(false);

        UpdateHeadIcon(forceId);
    }

    private void UpdateHeadIcon(int forceId)
    {
        if (headIcon == null) return;
        headIcon.gameObject.SetActive(true);
        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);
        Sprite sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg.Icon));
        if (sprite == null)
        {
            sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroDefaultIcon());
        }
        if (sprite != null)
        {
            headIcon.sprite = sprite;
        }
    }

    public void UpdateCityInfo(int cityId)
    {
        var cityCfg = WorldConfig.GetConfig(cityId);
        var cityData = GameManager.Instance.GetCity(cityId);

        if (cityCfg == null) return;

        int level = cityData != null ? cityData.GetLevel() : 1;
        if (cityName != null)
        {
            cityName.text = $"{cityCfg.Cname}{level}";
            cityName.color = Color.white;
        }
        if (cityExp != null) cityExp.gameObject.SetActive(true);
        if (cityExpBar != null) cityExpBar.transform.parent.gameObject.SetActive(true);
        if (cityData != null) UpdateHeadIcon(cityData.forceId);

        if (cityData == null) return;

        int currentExp = cityData.exp;
        int currentLevelExp = currentExp - SaveCityData.GetExpByLevel(level);
        int nextLevelTotalExp = level <= 20 && CityLevelConfig.HasConfig(level)
            ? CityLevelConfig.GetConfig(level).ExpNeed
            : -1;

        if (nextLevelTotalExp > 0)
        {
            int expNeededForNextLevel = nextLevelTotalExp - SaveCityData.GetExpByLevel(level);
            if (cityExp != null)
                cityExp.text = $"{currentLevelExp} / {expNeededForNextLevel}";
            if (cityExpBar != null)
            {
                float ratio = Mathf.Clamp01((float)currentLevelExp / expNeededForNextLevel);
                cityExpBar.rectTransform.sizeDelta = new Vector2(160f * ratio, cityExpBar.rectTransform.sizeDelta.y);
            }
        }
        else
        {
            if (cityExp != null)
                cityExp.text = $"Max / Max";
            if (cityExpBar != null)
                cityExpBar.rectTransform.sizeDelta = new Vector2(160f, cityExpBar.rectTransform.sizeDelta.y);
        }
    }

    public void HideWorld()
    {
        worldPanel.SetActive(false);
        topNode.SetActive(false);
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
            if (child.GetComponent<ResItem>() != null)
            {
                Destroy(child.gameObject);
            }
        }
        forceResItemDict.Clear();
        
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce == null) return;
        
        playerForce.RecalculateResUsed();
        
        int index = 0;
        float offsetX = 0;
        float prevWidth = 0;
        foreach (var attrConfig in CityAttrConfig.ConfigList)
        {
            if (!attrConfig.IsForceAttr) continue;
            if (attrConfig.NotShow) continue;
            
            float attrValue = playerForce.GetAttr(attrConfig.name);
            
            GameLog.Debug($"PanelManager.InitTopNodeResItems creating ResItem for {attrConfig.name}");
            var resBasePrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.ResBase());
            var resObj = Instantiate(resBasePrefab, topNode.transform);
            var resItem = resObj.GetComponent<ResItem>();
            resItem.Init(attrConfig.name);
            float currentWidth = ResItem.GetBaseWidth(attrConfig);
            if (index > 0 && currentWidth < prevWidth)
                offsetX -= (prevWidth - currentWidth) / 2;
            resObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetX, 0);
            resItem.UpdateNum(attrValue);
            if (attrConfig.IsPosRes)
            {
                resItem.UpdateUsed(playerForce.GetResUsed(attrConfig.name));
            }
            forceResItemDict[attrConfig.name] = resItem;
            offsetX += currentWidth;
            prevWidth = currentWidth;
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
            resItem.UpdateAddon(addonFloat);
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
            GameLog.Debug($"UpdateForceResItemAddons attrName={attrName} addon={addonFloat}");
            resItem.UpdateAddon(addonFloat);
        }
    }

    private void RefreshTopNodeResItem(string attrName, float value, int used = -1)
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
        ShowForceInfo();
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

    public void ShowCityBattle(int forceId, int targetCityId = 0, List<int> srcCityIds = null, List<SaveTroopsData> attackTroops = null, Dictionary<int, int> attackSoldierMap = null)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityBattlePanel == null)
        {
            var cityBattlePanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityBattlePanel"));
            cityBattlePanel = Instantiate(cityBattlePanelPrefab, transform);
        }
        cityBattlePanel.SetActive(true);
        var cityBattlePanelManager = cityBattlePanel.GetComponent<CityBattlePanelManager>();
        if (targetCityId > 0)
            cityBattlePanelManager.InitDefense(forceId, targetCityId, srcCityIds, attackTroops, attackSoldierMap);
        else
            cityBattlePanelManager.Init(forceId);
        cityBattlePanelManager.OnShow();

        ChangePanelCount(cityBattlePanel, true);
    }

    public void HideCityBattle()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityBattlePanel != null)
        {
            cityBattlePanel.SetActive(false);
            cityBattlePanel.GetComponent<CityBattlePanelManager>().OnHide();

            ChangePanelCount(cityBattlePanel, false);
            Destroy(cityBattlePanel);
            cityBattlePanel = null;
        }
    }

    public void ShowCityMove(int forceId, int sourceCityId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityMovePanel == null)
        {
            var cityMovePanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityMovePanel"));
            cityMovePanel = Instantiate(cityMovePanelPrefab, transform);
        }
        cityMovePanel.SetActive(true);
        var cityMovePanelManager = cityMovePanel.GetComponent<CityMovePanelManager>();
        cityMovePanelManager.Init(forceId, sourceCityId);
        cityMovePanelManager.OnShow();

        ChangePanelCount(cityMovePanel, true);
    }

    public void HideCityMove()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityMovePanel.SetActive(false);
        cityMovePanel.GetComponent<CityMovePanelManager>().OnHide();

        ChangePanelCount(cityMovePanel, false);
        Destroy(cityMovePanel);
        cityMovePanel = null;
    }

    public void ShowCityTrade(int forceId, int sourceCityId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityTradePanel == null)
        {
            var cityTradePanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityTradePanel"));
            cityTradePanel = Instantiate(cityTradePanelPrefab, transform);
        }
        cityTradePanel.SetActive(true);
        var cityTradePanelManager = cityTradePanel.GetComponent<CityTradePanelManager>();
        cityTradePanelManager.Init(forceId, sourceCityId);
        cityTradePanelManager.OnShow();

        ChangePanelCount(cityTradePanel, true);
    }

    public void HideCityTrade()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityTradePanel.SetActive(false);
        cityTradePanel.GetComponent<CityTradePanelManager>().OnHide();

        ChangePanelCount(cityTradePanel, false);
        Destroy(cityTradePanel);
        cityTradePanel = null;
    }

    public void ShowCitySearch(int forceId, int sourceCityId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (citySearchPanel == null)
        {
            var citySearchPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CitySearchPanel"));
            citySearchPanel = Instantiate(citySearchPanelPrefab, transform);
        }
        citySearchPanel.SetActive(true);
        var citySearchPanelManager = citySearchPanel.GetComponent<CitySearchPanelManager>();
        citySearchPanelManager.Init(forceId, sourceCityId);
        citySearchPanelManager.OnShow();

        ChangePanelCount(citySearchPanel, true);
    }

    public void HideCitySearch()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        citySearchPanel.SetActive(false);
        citySearchPanel.GetComponent<CitySearchPanelManager>().OnHide();

        ChangePanelCount(citySearchPanel, false);
        Destroy(citySearchPanel);
        citySearchPanel = null;
    }

    public void ShowCityPraise(int forceId, int cityId, int devId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityPraisePanel == null)
        {
            var cityPraisePanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityPraisePanel"));
            cityPraisePanel = Instantiate(cityPraisePanelPrefab, transform);
        }
        cityPraisePanel.SetActive(true);
        var cityPraisePanelManager = cityPraisePanel.GetComponent<CityPraisePanelManager>();
        cityPraisePanelManager.Init(forceId, cityId, devId);
        cityPraisePanelManager.OnShow();

        ChangePanelCount(cityPraisePanel, true);
    }

    public void HideCityPraise()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityPraisePanel.SetActive(false);
        cityPraisePanel.GetComponent<CityPraisePanelManager>().OnHide();

        ChangePanelCount(cityPraisePanel, false);
        Destroy(cityPraisePanel);
        cityPraisePanel = null;
    }

    public void ShowCityUseHero(int forceId, int cityId, int devId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityUseHeroPanel == null)
        {
            var cityUseHeroPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("CityHeroUsePanel"));
            cityUseHeroPanel = Instantiate(cityUseHeroPanelPrefab, transform);
        }
        cityUseHeroPanel.SetActive(true);
        var cityUseHeroPanelManager = cityUseHeroPanel.GetComponent<CityUseHeroPanelManager>();
        cityUseHeroPanelManager.Init(forceId, cityId, devId);
        cityUseHeroPanelManager.OnShow();

        ChangePanelCount(cityUseHeroPanel, true);
    }

    public void HideCityUseHero()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        cityUseHeroPanel.SetActive(false);
        cityUseHeroPanel.GetComponent<CityUseHeroPanelManager>().OnHide();

        ChangePanelCount(cityUseHeroPanel, false);
        Destroy(cityUseHeroPanel);
        cityUseHeroPanel = null;
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


    public void ShowPopResultPanel(string title, List<PopResultPanelManager.AttrData> attrDatas, Action afterRun, string path, bool autoHide = true)
    {
        if (popResultPanel == null)
        {
            popResultPanel = Instantiate(ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("PopResultPanel")), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popResultPanel.SetActive(true);
        popResultPanel.GetComponent<PopResultPanelManager>().OnShow(title, attrDatas, afterRun, path, autoHide);

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

    public void ShowGmPanel()
    {
        if (gmPanel == null)
        {
            var gmPanelPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel("GMPanel"));
            gmPanel = Instantiate(gmPanelPrefab, transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        gmPanel.SetActive(true);
        gmPanel.GetComponent<GmPanelManager>().OnShow();

        ChangePanelCount(gmPanel, true);
    }

    public void HideGmPanel()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        gmPanel.SetActive(false);
        gmPanel.GetComponent<GmPanelManager>().OnHide();

        ChangePanelCount(gmPanel, false);
        Destroy(gmPanel);
        gmPanel = null;
    }

    public void ShowSideBar(string panelName, System.Action<GameObject> onCreated = null)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        sideBarPanel.SetActive(true);
        sideBarPanel.GetComponent<SideBar>().OnShow(panelName, onCreated);
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

    public GameObject ShowTip(string tipName, Vector2 screenPosition)
    {
        HideTip();

        if (tipNode == null)
        {
            GameLog.Error("PanelManager tipNode为空");
            return null;
        }

        GameObject tipPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.TipItem());
        if (tipPrefab == null)
        {
            GameLog.Error("PanelManager ResTipItem预制体加载失败");
            return null;
        }

        currentTip = Instantiate(tipPrefab, tipNode.transform);
        ResTipItem resTipItem = currentTip.GetComponent<ResTipItem>();
        if (resTipItem != null)
        {
            resTipItem.SetName(tipName);
        }

        RectTransform tipRect = currentTip.GetComponent<RectTransform>();
        if (tipRect != null)
        {
            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(tipRect);

            Canvas canvas = tipNode.GetComponentInParent<Canvas>();
            Camera uiCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera : null;

            float tipHeight = tipRect.rect.height;
            float tipOffset = 60f;
            bool showAbove = screenPosition.y + tipOffset + tipHeight <= Screen.height;

            Vector2 adjustedScreenPos = screenPosition;
            adjustedScreenPos.y += showAbove ? tipOffset : -tipOffset;

            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tipNode.transform as RectTransform,
                adjustedScreenPos,
                uiCamera,
                out localPos);

            if (showAbove)
            {
                localPos.y += tipHeight * 0.5f;
            }
            else
            {
                localPos.y -= tipHeight * 0.5f;
            }

            tipRect.anchoredPosition = localPos;
        }

        return currentTip;
    }

    public void HideTip()
    {
        if (currentTip != null)
        {
            Destroy(currentTip);
            currentTip = null;
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
