using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevNodeBattle : MonoBehaviour, ICityDevNode
{
    private int cityId;
    private int devId;

    public TMP_Text attr1Text;
    public TMP_Text attrVal1Text;
    public TMP_Text attrDesText;

    public TMP_Text goldCostText;

    public Button destButton;
    public SelectHeroControl heroSelect;
    public Button runButton;

    // Start is called before the first frame update
    void Start()
    {
        runButton.onClick.AddListener(() =>
        {
            var heroList = heroSelect.heroIds;
            if(heroList.Length <= 0)
                return;
            OnRun(devId, heroList);
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    } 

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        var devCfg = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);

        attr1Text.text = CityAttrConfig.GetConfigByname(devCfg.DevAttr1.ToLower()).Cname;
        attrVal1Text.text = cityData.GetAttr(devCfg.DevAttr1).ToString();


        attrDesText.text = devCfg.Des;
        goldCostText.text = devCfg.GoldCost.ToString() + "/" + cityData.gold.ToString();

        heroSelect.SetDevId(cityId, devId);
    }

    public void CheckDev(int[] heroList, out List<string> attrs, out List<int> attrOlds, out List<int> results)
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

            resultTmp.Add(0);
            resultTmp[0] += Math.Max(devConfig.DevAttr1Value[0], (float)attrVal / 100 * devConfig.DevAttr1Value[1]);

            if (devConfig.DevAttr2Value != null && devConfig.DevAttr2Value[1] != 0)
            {
                resultTmp.Add(0);
                if (devConfig.DevAttr2Value[1] > 0)
                    resultTmp[1] += Math.Max(devConfig.DevAttr2Value[0], (float)attrVal / 100 * devConfig.DevAttr2Value[1]);
                else
                    resultTmp[1] += Math.Min(devConfig.DevAttr2Value[0], (float)attrVal / 100 * devConfig.DevAttr2Value[1]);
            }
            if (devConfig.DevAttr3Value != null && devConfig.DevAttr3Value[1] != 0)
            {
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
        PanelManager.Instance.HideCityDev();
        var devConfig = CityDevConfig.GetConfig(devId);
        CheckDev(heroList, out var attrs, out var attrOlds, out var results);
        PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, attrs, attrOlds, results, devConfig.Mp4);
    }
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
