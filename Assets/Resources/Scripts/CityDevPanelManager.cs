using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

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
        runButton.onClick.AddListener(() =>
        {
            if(lastSelectedCell != null && heroSelect.heroIds.Length > 0)
            {
                var devId = lastSelectedCell.devId;
                var heroList = heroSelect.heroIds;

                OnRun(devId, heroList);
            }
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

        var devCfg = CityDevConfig.GetConfig(cellInfo.devId);
        var cityData = GameManager.Instance.GetCity(cityId);

        attr1Text.text = CityAttrConfig.GetConfigByCname(devCfg.DevAttr1).name;
        attrVal1Text.text = cityData.GetAttr(devCfg.DevAttr1).ToString();
        // if(devCfg.DevAttrs.Length > 1)
        // {
        //     attr2Text.gameObject.SetActive(true);
        //     attrVal2Text.gameObject.SetActive(true);
        //     attr2Text.text = NameTransTool.GetAttrName(devCfg.DevAttrs[1]);
        //     attrVal2Text.text = cityData.GetAttr(devCfg.DevAttrs[1]).ToString();
        // }
        // else
        // {
        //     attr2Text.gameObject.SetActive(false);
        //     attrVal2Text.gameObject.SetActive(false);
        // }

        attrDesText.text = devCfg.Des;
        goldCostText.text = devCfg.GoldCost.ToString() + "/" + cityData.gold.ToString();

        heroSelect.SetDevId(cityId, cellInfo.devId);
    }    

    public static void CheckDev(int cityId, int devId, int[] heroList, out List<string> attrs, out List<int> attrOlds, out List<int> results)
    {
        attrs = new List<string>();
        attrOlds = new List<int>();
        results = new List<int>();
        var resultTmp = new List<float>();
        var devConfig = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        if(cityData.gold < devConfig.GoldCost * heroList.Length) //每个英雄付费一次
            return;
        if(devConfig.GoldCost > 0)
            cityData.gold -= devConfig.GoldCost * heroList.Length;
        for (int i = 0; i < heroList.Length; i++)
        {
            var heroData = cityData.GetHero(heroList[i]);
            var checkAttr = devConfig.Attrs[0];
            var attrVal = heroData.GetAttr(checkAttr);
            if (devConfig.Attrs.Length > 1)
            {
                var attrVal2 = heroData.GetAttr(devConfig.Attrs[1]);
                if (attrVal2 > attrVal)
                    attrVal += (attrVal2 - attrVal) / 3;
            }

            if (resultTmp.Count <= 0)
                resultTmp.Add(0);
            resultTmp[0] += Math.Max(devConfig.DevAttr1Value[0], (float)attrVal / 100 * devConfig.DevAttr1Value[1]);

            if (devConfig.DevAttr2Value != null && devConfig.DevAttr2Value[1] != 0)
            {
                if (!string.IsNullOrEmpty(devConfig.DevAttr2) && results.Count <= 1)
                    resultTmp.Add(0);
                if (devConfig.DevAttr2Value[1] > 0)
                    resultTmp[1] += Math.Max(devConfig.DevAttr2Value[0], (float)attrVal / 100 * devConfig.DevAttr2Value[1]);
                else
                    resultTmp[1] += Math.Min(devConfig.DevAttr2Value[0], (float)attrVal / 100 * devConfig.DevAttr2Value[1]);
            }
            if (devConfig.DevAttr3Value != null && devConfig.DevAttr3Value[1] != 0)
            {
                if (!string.IsNullOrEmpty(devConfig.DevAttr3) && results.Count <= 2)
                    resultTmp.Add(0);
                if (devConfig.DevAttr3Value[1] > 0)
                    resultTmp[2] += Math.Max(devConfig.DevAttr3Value[0], (float)attrVal / 100 * devConfig.DevAttr3Value[1]);
                else
                    resultTmp[2] += Math.Min(devConfig.DevAttr3Value[0], (float)attrVal / 100 * devConfig.DevAttr3Value[1]);
            }
        }
        for (int i = 0; i < resultTmp.Count; i++)
        {
            results.Add((int)resultTmp[i]);
        }
        cityData.AddAttr(devConfig.DevAttr1, results[0]);
        attrs.Add(devConfig.DevAttr1);
        attrOlds.Add(cityData.GetAttr(devConfig.DevAttr1));
        if (!string.IsNullOrEmpty(devConfig.DevAttr2))
        {
            cityData.AddAttr(devConfig.DevAttr2, results[1]);
            attrs.Add(devConfig.DevAttr2);
            attrOlds.Add(cityData.GetAttr(devConfig.DevAttr2));
        }
        if (!string.IsNullOrEmpty(devConfig.DevAttr3))
        {
            cityData.AddAttr(devConfig.DevAttr3, results[2]);
            attrs.Add(devConfig.DevAttr3);
            attrOlds.Add(cityData.GetAttr(devConfig.DevAttr3));
        }
    }

    private void OnRun(int devId, int[] heroList)
    {
        PanelManager.Instance.HideCityBuilding();
        var devConfig = CityDevConfig.GetConfig(devId);
        CheckDev(cityId, devId, heroList, out var attrs, out var attrOlds, out var results);
        PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, attrs, attrOlds, results, devConfig.Mp4);
    }
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
