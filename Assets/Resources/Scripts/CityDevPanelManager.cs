using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;

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

    private string GetAttrName(string type)
    {
        switch (type.ToLower())
        {
            case "archgold":
                return "商业";
            case "archfood":
                return "农业";
            case "archpeople":
                return "人口";
            case "gold":
                return "金钱";
            case "food":
                return "粮食";
            case "soldier":
                return "士兵";
            case "secure":
                return "治安";
            case "wall":
                return "城防";
            default:
                return "";
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
        var cityData = GameManager.Instance.GetCity(cityId);

        if(devCfg.DevAttrs.Length > 0)
        {
            attr1Text.text =  GetAttrName(devCfg.DevAttrs[0]);
            attrVal1Text.text = cityData.GetAttr(devCfg.DevAttrs[0]).ToString();
        }
        if(devCfg.DevAttrs.Length > 1)
        {
            attr2Text.gameObject.SetActive(true);
            attrVal2Text.gameObject.SetActive(true);
            attr2Text.text =  GetAttrName(devCfg.DevAttrs[1]);
            attrVal2Text.text = cityData.GetAttr(devCfg.DevAttrs[1]).ToString();
        }
        else
        {
            attr2Text.gameObject.SetActive(false);
            attrVal2Text.gameObject.SetActive(false);
        }

        attrDesText.text = devCfg.Des;
        goldCostText.text = devCfg.GoldCost.ToString() + "/" + cityData.gold.ToString();

        heroSelect.SetDevId(cityId, cellInfo.devId);
    }    
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
