using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevNodeNormal : MonoBehaviour, ICityDevNode
{
    private int cityId;
    private int devId;

    public TMP_Text attr1Text;
    public TMP_Text attr2Text;
    public TMP_Text attrVal1Text;
    public TMP_Text attrVal2Text;
    public TMP_Text attrDesText;

    public TMP_Text goldCostText;

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
        
        // 订阅英雄数量变化事件
        heroSelect.OnHeroCountChange += (heroCount) =>
        {
            UpdateGoldCostText(heroCount > 0 ? heroCount : 1);
        };

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

        //Debug.Log("SetDev " + devCfg.DevAttr1 + " " + devCfg.DevAttr2);
        if(!string.IsNullOrEmpty(devCfg.DevAttr2))
        {
            attr2Text.gameObject.SetActive(true);
            attrVal2Text.gameObject.SetActive(true);
            attr2Text.text = CityAttrConfig.GetConfigByname(devCfg.DevAttr2.ToLower()).Cname;
            attrVal2Text.text = cityData.GetAttr(devCfg.DevAttr2).ToString();
        }
        else
        {
            attr2Text.gameObject.SetActive(false);
            attrVal2Text.gameObject.SetActive(false);
        }

        attrDesText.text = devCfg.Des;
        
        // 初始显示单个英雄的消耗
        UpdateGoldCostText(1);

        heroSelect.SetDevId(cityId, devId);
    }
    
    // 更新黄金消耗显示
    private void UpdateGoldCostText(int heroCount)
    {
        var devCfg = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        int totalCost = devCfg.GoldCost * heroCount;
        goldCostText.text = totalCost.ToString() + "/" + cityData.gold.ToString();
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
            var heroData = GameManager.Instance.GetHero(heroList[i]);
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
        // 过滤掉当前年份已经执行过动作的英雄
        var validHeroList = new List<int>();
        var currentYear = GameManager.Instance.SaveData.year;
        
        foreach (var heroId in heroList)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            if (hero.currentYear != currentYear)
            {
                validHeroList.Add(heroId);
            }
        }
        
        if (validHeroList.Count == 0)
        {
            return; // 没有可执行动作的英雄
        }
        
        // 更新黄金消耗显示为实际执行的英雄数的消耗
        UpdateGoldCostText(validHeroList.Count);
        
        PanelManager.Instance.HideCityDev();
        var devConfig = CityDevConfig.GetConfig(devId);
        CheckDev(validHeroList.ToArray(), out var attrs, out var attrOlds, out var results);
        
        // 更新英雄的年份
        foreach (var heroId in validHeroList)
        {
            var hero = GameManager.Instance.GetHero(heroId);
            hero.currentYear = currentYear;
        }
        
        PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, attrs, attrOlds, results, null, devConfig.Mp4);
    }
    
    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
