using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class CityDevPanelManager : MonoBehaviour
{
    private int cityId;
    private int buildingId;

    public Button closeButton;
    public TMP_Text buildingText;
    public TMP_Text attr1Text;
    public TMP_Text attr2Text;
    public TMP_Text attrVal1Text;
    public TMP_Text attrVal2Text;
    public TMP_Text attrDesText;

    public TMP_Text goldCostText;
    public GameObject devPrefab;

    public GameObject devNodeParent;

    public SelectHeroControl heroSelect;
    public Button runButton;

    private CityDevPanelCell lastSelectedCell;

    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityBuilding();
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCityId(int cityId, int buildingId)
    {
        this.cityId = cityId;
        this.buildingId = buildingId;

        for(int i = 0; i < devNodeParent.transform.childCount; i++)
        {
            Destroy(devNodeParent.transform.GetChild(i).gameObject);
        }
        lastSelectedCell = null;

        Debug.Log("SetCityId: " + cityId + " " + buildingId);

        var cityData = GameManager.Instance.GetCity(cityId);
        var buildingCfg = CityBuildingConfig.GetConfig(buildingId);
                        buildingText.text = buildingCfg.Cname;
        int devIndex = 0;
        foreach(var cfg in CityDevConfig.ConfigList)
        {
            if(cfg.BuildingName == buildingCfg.Name)
            {
                var devNode = Instantiate(devPrefab, devNodeParent.transform);
                var devNodeMgr = devNode.GetComponent<CityDevPanelCell>();
                devNodeMgr.cityDevPanelManager = this;
                devNodeMgr.Init(cfg.Id);

                var rectTransform = devNode.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(40 + devIndex * 150, -22);
                devIndex++;

                if (lastSelectedCell == null)
                {
                    OnSelectItem(devNodeMgr);
                }
            }
        }
    }

    public void OnSelectItem(CityDevPanelCell cellInfo)
    {
        // 取消上次选中的城市
        if (lastSelectedCell != null && lastSelectedCell != cellInfo)
        {
            lastSelectedCell.OnSelect(false);
        }
        
        // 选中当前城市
        cellInfo.OnSelect(true);
        // 更新当前选中的单元格引用
        lastSelectedCell = cellInfo;

        var devCfg = CityDevConfig.GetConfig(cellInfo.devId);

       // attr1Text.text = devCfg.Attr1Name;
      //  attr2Text.text = devCfg.Attr2Name;
      //  attrVal1Text.text = devCfg.Attr1Val.ToString();
      //  attrVal2Text.text = devCfg.Attr2Val.ToString();
        attrDesText.text = devCfg.Des;
        goldCostText.text = devCfg.GoldCost.ToString();
    }    
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
