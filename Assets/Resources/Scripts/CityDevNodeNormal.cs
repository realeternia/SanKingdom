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
            {
                SystemTip.Instance.ShowTip("请选择英雄");
                return;
            }
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
    private void OnRun(int devId, int[] heroList)
    {
        PanelManager.Instance.HideCityDev();
        
        List<string> attrs;
        List<int> attrOlds;
        List<int> results;

        var cityData = GameManager.Instance.GetCity(cityId);
        cityData.GetPlayer().ExecuteCityDev(cityId, devId, heroList, out attrs, out attrOlds, out results);
        var devConfig = CityDevConfig.GetConfig(devId);
        PanelManager.Instance.ShowPopResultPanel(CityDevConfig.GetConfig(devId).Cname, attrs, attrOlds, results, null, devConfig.Mp4);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
