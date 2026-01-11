using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;
using System.Runtime.Versioning;

public class CityDevPanelManager : MonoBehaviour
{
    private int cityId;
    private int buildingId;

    public Button closeButton;
    public TMP_Text buildingText;

    public GameObject devPrefab;
    public GameObject devNodeParent;

    public GameObject devDetailParent;

    private GameObject detailObj;

    private CityDevPanelCell lastSelectedCell;

    // Start is called before the first frame update
    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityDev();
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
                rectTransform.anchoredPosition = new Vector2(24 + devIndex * 150, -15);
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

        // 显示当前城市的详细信息
        if (detailObj != null)
        {
            Destroy(detailObj);
        }

        var devCfg = CityDevConfig.GetConfig(cellInfo.devId);

        detailObj = Instantiate(Resources.Load<GameObject>("Prefabs/Panels/" + devCfg.Prefab), devDetailParent.transform);
        detailObj.SetActive(true);
        detailObj.GetComponent<ICityDevNode>().SetDev(cityId, cellInfo.devId);
    }    
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
