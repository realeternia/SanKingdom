using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public GameObject cardShopPanel;
    public GameObject rankPanel;
    public GameObject rankPlayerPanel;
    public GameObject pickPanel;
    public GameObject worldPanel;
    public GameObject cityPanel;
    public GameObject cityBuildingPanel;

    public GameObject popCitySelectPanel;
    public GameObject popHeroSelectPanel;
    public GameObject popResultPanel;


    public GameObject bagPanel;

    public List<GameObject> openPanelList;

    // Start is called before the first frame update
    void Start()
    {
        ShowPick();
    }

    public void ShowWorld()
    {
        worldPanel.SetActive(true);

        ChangePanelCount(worldPanel, true);
    }

    public void HideWorld()
    {
        worldPanel.SetActive(false);

        ChangePanelCount(worldPanel, false);
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
    
    public void ShowBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(true);
        bagPanel.GetComponent<BagControl>().OnShow();

        ChangePanelCount(bagPanel, true);
    }

    public void HideBag()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        bagPanel.SetActive(false);
        bagPanel.GetComponent<BagControl>().OnHide();

        ChangePanelCount(bagPanel, false);
    }

    public void ShowCity(int cityId)
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        cityPanel.SetActive(true);
        var cityPanelManager = cityPanel.GetComponent<CityPanelManager>();
        cityPanelManager.SetCityId(cityId);
        cityPanelManager.OnShow();

        ChangePanelCount(cityPanel, true);
    }

    public void HideCity()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        cityPanel.SetActive(false);
        cityPanel.GetComponent<CityPanelManager>().OnHide();

        ChangePanelCount(cityPanel, false);
    }
    
    public void ShowCityBuilding(int cityId, int buildingId)
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        cityBuildingPanel.SetActive(true);
        var cityBuildingPanelManager = cityBuildingPanel.GetComponent<CityDevPanelManager>();
        cityBuildingPanelManager.SetCityId(cityId, buildingId);
        cityBuildingPanelManager.OnShow();

        ChangePanelCount(cityBuildingPanel, true);
    }

    public void HideCityBuilding()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        cityBuildingPanel.SetActive(false);
        cityBuildingPanel.GetComponent<CityDevPanelManager>().OnHide();

        ChangePanelCount(cityBuildingPanel, false);
    }
  

    public void ShowRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(true);
        rankPanel.GetComponent<RankPanelManager>().OnShow();

        ChangePanelCount(rankPanel, true);        
    }

    public void HideRank()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPanel.SetActive(false);
        rankPanel.GetComponent<RankPanelManager>().OnHide();

        ChangePanelCount(rankPanel, false);        
    }
    
    public void ShowRankPlayer()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPlayerPanel.SetActive(true);
        rankPlayerPanel.GetComponent<RankPlayerPanelManager>().OnShow();

        ChangePanelCount(rankPlayerPanel, true);        
    }

    public void HideRankPlayer()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        rankPlayerPanel.SetActive(false);
        rankPlayerPanel.GetComponent<RankPlayerPanelManager>().OnHide();

        ChangePanelCount(rankPlayerPanel, false);        
    }

    public void ShowPick()
    {
        //  GameManager.Instance.PlaySound("Sounds/deck");
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
     //   GameManager.Instance.PlaySound("Sounds/deck");
        pickPanel.SetActive(false);

        ChangePanelCount(pickPanel, false);
        GameObject.Destroy(pickPanel);
        pickPanel = null;
    }

    public void ShowPopCitySelectPanel(int cityId)
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        popCitySelectPanel.SetActive(true);
        popCitySelectPanel.GetComponent<PopCitySelectPanelManager>().OnShow(cityId);

        ChangePanelCount(popCitySelectPanel, true);
    }

    public void HidePopCitySelectPanel()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        popCitySelectPanel.SetActive(false);
        popCitySelectPanel.GetComponent<PopCitySelectPanelManager>().OnHide();

        ChangePanelCount(popCitySelectPanel, false);
    }

    public void ShowPopHeroSelectPanel(int[] heroList, int[] checkedList, string[] attrs, Action<List<int>> onSelectMethod)
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        popHeroSelectPanel.SetActive(true);
        popHeroSelectPanel.GetComponent<PopHeroSelectPanelManager>().OnShow(heroList, checkedList, attrs, onSelectMethod);

        ChangePanelCount(popHeroSelectPanel, true);
    }

    public void HidePopHeroSelectPanel()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        popHeroSelectPanel.SetActive(false);
        popHeroSelectPanel.GetComponent<PopHeroSelectPanelManager>().OnHide();

        ChangePanelCount(popHeroSelectPanel, false);
    }

    public void ShowPopResultPanel(string title, string path)
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        popResultPanel.SetActive(true);
        popResultPanel.GetComponent<PopResultPanelManager>().OnShow(title, path);

        ChangePanelCount(popResultPanel, true);
    }

    public void HidePopResultPanel()
    {
        GameManager.Instance.PlaySound("Sounds/deck");
        popResultPanel.SetActive(false);
        popResultPanel.GetComponent<PopResultPanelManager>().OnHide();

        ChangePanelCount(popResultPanel, false);
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        UnityEngine.Debug.Log($"PanelManager SendSignal {name} {parm1} {parm2}");
        foreach (var panel in openPanelList)
        {
            UnityEngine.Debug.Log($"PanelManager SendSignal {panel.name} {name} {parm1} {parm2}");
            if (panel.TryGetComponent<IPanelEvent>(out IPanelEvent p))
                p.SendSignal(name, parm1, parm2);
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
