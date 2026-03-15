using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevNodeChange : MonoBehaviour, ICityDevNode
{
    private int cityId;
    private int devId;

    public TMP_Text attr1Text;
    public TMP_Text attr2Text;
    public TMP_Text attrVal1Text;
    public TMP_Text attrVal2Text;
    public TMP_Text attrDesText;

    public SelectHeroControl heroSelect;
    public Button runButton;
    public Button modeButton;
    public Button amountButton;

    public TMP_Text modeText;
    public TMP_Text amountText;

    private bool isBuying = true; // true: buy grain with money, false: sell grain for money
    private int[] amountOptions = { 300, 500, 1000, 2000, 3000 };
    private int currentAmountIndex = 0;
    private const float EXCHANGE_RATE = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        runButton.onClick.AddListener(() =>
        {
            var heroList = heroSelect.heroIds;
            if(heroList.Length <= 0)
            {
                SystemTip.Instance.ShowTip("请选择至少一个英雄");
                return;
            }            
            OnRun(heroList);
        });
        
        modeButton.onClick.AddListener(() =>
        {
            ToggleMode();
        });
        
        amountButton.onClick.AddListener(() =>
        {
            CycleAmount();
        });
        
        // 初始化UI
        UpdateModeText();
        UpdateAmountText();
        
        // 订阅英雄数量变化事件
        heroSelect.OnHeroCountChange += (heroCount) =>
        {
            
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

        attr2Text.text = CityAttrConfig.GetConfigByname(devCfg.DevAttr2.ToLower()).Cname;
        attrVal2Text.text = cityData.GetAttr(devCfg.DevAttr2).ToString();

        attrDesText.text = devCfg.Des;

        heroSelect.SetDevId(cityId, devId);
    }


    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
    
    private void ToggleMode()
    {
        isBuying = !isBuying;
        UpdateModeText();
    }
    
    private void UpdateModeText()
    {
        modeText.text = isBuying ? "买入" : "卖出";
        UpdateAmountText();
    }
    
    private void CycleAmount()
    {
        currentAmountIndex = (currentAmountIndex + 1) % amountOptions.Length;
        UpdateAmountText();
    }
    
    private void UpdateAmountText()
    {
        int amount = amountOptions[currentAmountIndex];
        amountText.text = amount.ToString();
        var devCfg = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        
        if (isBuying)
        {
            // Buy mode: money -> grain
            int getV = Mathf.CeilToInt(amount * EXCHANGE_RATE);
            attrVal1Text.text = string.Format("{0} / {1}", -amount, cityData.GetAttr("Gold").ToString());
            attrVal2Text.text = string.Format("{0} / {1}", getV, cityData.GetAttr("Food").ToString());
            attrVal1Text.color = cityData.GetAttr("Gold") >= amount ? Color.white : Color.red;
        }
        else
        {
            // Sell mode: grain -> money
            int getV = Mathf.FloorToInt(amount * EXCHANGE_RATE);
            attrVal1Text.text = string.Format("{0} / {1}", getV, cityData.GetAttr("Gold").ToString());
            attrVal2Text.text = string.Format("{0} / {1}", -amount, cityData.GetAttr("Food").ToString());
            attrVal2Text.color = cityData.GetAttr("Food") >= amount ? Color.white : Color.red;
        }
    }
    
    private void OnRun(int[] heroList)
    {
        int amount = amountOptions[currentAmountIndex];
        var cityData = GameManager.Instance.GetCity(cityId);
        var devConfig = CityDevConfig.GetConfig(devId);
    
        List<PopResultPanelManager.AttrData> attrDatas;

        if(!cityData.GetPlayer().ExecuteCityChange(cityId, devId, heroList, isBuying, amount, EXCHANGE_RATE, out attrDatas))
        {
            return;
        }
        
        PanelManager.Instance.HideCityDev();     
        PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, attrDatas, null, devConfig.Mp4);
    }
}
