using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonConfig;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject cardShopPanel;
    public GameObject rankPanel;
    private GameObject pickPanel;
    public GameObject worldPanel;
    public GameObject cityPanel;
    private GameObject cityDevPanel;
    private GameObject systemPanel;

    private GameObject popCitySelectPanel;
    private GameObject popHeroSelectPanel;
    private GameObject popHeroBattleSelectPanel;
    private GameObject popResultPanel;
    private GameObject popArmySetPanel;
    private GameObject heroInfoPanel;
    private GameObject battleResultPanel;

    public List<GameObject> openPanelList;

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

        ChangePanelCount(worldPanel, true);

        SwitchBGM();
    }

    public void HideWorld()
    {
        worldPanel.SetActive(false);

        ChangePanelCount(worldPanel, false);

        var roll = UnityEngine.Random.Range(0, 2);
        BGMPlayer.Instance.PlayBGM(roll == 0 ? "BGMs/weifeng" : "BGMs/pozhu");
    }

    public void ShowShop()
    {
        cardShopPanel.SetActive(true);
      //  cardShopTxt.SetActive(true);

        ChangePanelCount(cardShopPanel, true);
    }

    public void HideShop()
    {
        cardShopPanel.SetActive(false);
     //   cardShopTxt.SetActive(false);

        ChangePanelCount(cardShopPanel, false);
    }

    public void ShowCity(int cityId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
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
    }
    
    public void ShowCityDev(int cityId, int devId)
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (cityDevPanel == null)
        {
            var cityDevPanelPrefab = Resources.Load<GameObject>("Prefabs/Panels/CityDevPanel");
            cityDevPanel = Instantiate(cityDevPanelPrefab, transform);
        }
        cityDevPanel.SetActive(true);
        var cityDevPanelManager = cityDevPanel.GetComponent<CityDevPanelManager>();
        cityDevPanelManager.SetCityId(cityId, devId);
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
            var systemPanelPrefab = Resources.Load<GameObject>("Prefabs/Panels/SystemInfoPanel");
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
  

    public void ShowRank()
    {
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        if (rankPanel == null)
        {
            var rankPanelPrefab = Resources.Load<GameObject>("Prefabs/Panels/RankInfoPanel");
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
            var pickPanelPrefab = Resources.Load<GameObject>("Prefabs/Panels/PickPanel");
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

    public void ShowPopCitySelectPanel(int cityId, bool findEnemy, System.Action<int> callback)
    {
        if (popCitySelectPanel == null)
        {
            popCitySelectPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PopCitySelectPanel"), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popCitySelectPanel.SetActive(true);
        popCitySelectPanel.GetComponent<PopCitySelectPanelManager>().OnShow(cityId, findEnemy, callback);

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

    public void ShowPopHeroSelectPanel(int cityId, int selectCount, int[] heroList, int[] checkedList, string[] attrs, Action<List<int>> onSelectMethod)
    {
        if (popHeroSelectPanel == null)
        {
            popHeroSelectPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PopHeroSelectPanel"), transform);
        }
        BGMPlayer.Instance.PlaySound("Sounds/deck");
        popHeroSelectPanel.SetActive(true);
        popHeroSelectPanel.GetComponent<PopHeroSelectPanelManager>().OnShow(cityId, selectCount, heroList, checkedList, attrs, onSelectMethod);

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
            popHeroBattleSelectPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PopHeroBattleSelectPanel"), transform);
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
            popResultPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PopResultPanel"), transform);
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
            popArmySetPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/PopArmySetPanel"), transform);
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
            heroInfoPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/HeroInfoPanel"), transform);
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
            battleResultPanel = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/BattleResultPanel"), transform);
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

    public void SendSignal(string name, string parm1, int parm2)
    {
        Debug.Log($"PanelManager SendSignal {name} {parm1} {parm2}");
        if(worldPanel != null)
        {
            worldPanel.GetComponent<MainPanelManager>().SendSignal(name, parm1, parm2);
        }
        foreach (var panel in openPanelList)
        {
            Debug.Log($"PanelManager SendSignal {panel.name} {name} {parm1} {parm2}");
            if (panel.TryGetComponent<IPanelEvent>(out IPanelEvent p))
            {
                Debug.Log($"PanelManager SendSignal2 {panel.name} {name} {parm1} {parm2}");
                p.SendSignal(name, parm1, parm2);
            }
        }
    }

    private void ChangePanelCount(GameObject panel, bool isShow)
    {
        if(isShow)
            openPanelList.Add(panel);
        else
            openPanelList.Remove(panel);
        if(openPanelList.Count <= 0)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
