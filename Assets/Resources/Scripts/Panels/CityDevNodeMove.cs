using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevNodeMove : MonoBehaviour, ICityDevNode
{
    private int cityId;
    private int devId;

    public TMP_Text attrVal1Text;
    public TMP_Text attrDesText;

    public TMP_Text foodText;
    private int foodCount = SystemConst.Expedition.DEFAULT_FOOD_DAYS;

    public Button destButton;
    public SelectHeroArmyControl heroSelect;
    public Button foodButton;
    public Button runButton;

    public TMP_Text foodCostText;

    private int selectedCityId;

    void Start()
    {
        runButton.onClick.AddListener(() =>
        {
            if (selectedCityId == 0)
            {
                SystemTip.Instance.ShowTip("请选择目标城市");
                return;
            }
            if (heroSelect.heroIds.Length <= 0)
            {
                SystemTip.Instance.ShowTip("请选择至少一个英雄");
                return;
            }
            var soldierTotal = GameManager.Instance.GetCity(cityId).GetAttr("soldier");
            var foodCost = soldierTotal * foodCount / SystemConst.Expedition.SOLDIER_FOOD_COST_DIVISOR;
            var citySrc = GameManager.Instance.GetCity(cityId);
            if(citySrc.food < foodCost)
            {
                SystemTip.Instance.ShowTip("粮食不足");
                return;
            }            

            var devConfig = CityDevConfig.GetConfig(devId);
            PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, new List<PopResultPanelManager.AttrData>(), () =>
            {
                var heroList = heroSelect.heroIds;
                if (heroList.Length <= 0)
                    return;
                OnRun(devId, heroList);
            }, devConfig.Mp4);
        });
        heroSelect.onClick = () =>
        {
            UpdateFoodInfo();
        };
        destButton.onClick.AddListener(() =>
        {
            var forceId = GameManager.Instance.GetCity(cityId).forceId;
            var cityIds = MapTool.GetOwnCityIds(forceId);
            PanelManager.Instance.ShowPopCitySelectPanel(cityIds, selectedCityId, (selectedCityId) =>
            {
                this.selectedCityId = selectedCityId;
                if(selectedCityId == 0)
                {
                    attrVal1Text.text = "-";
                    return;
                }
                var cityCfg = WorldConfig.GetConfig(selectedCityId);
                attrVal1Text.text = cityCfg.Cname;
            });
        });
        foodButton.onClick.AddListener(() =>
        {
            if(foodCount == SystemConst.Expedition.DEFAULT_FOOD_DAYS)
            {
                foodCount = SystemConst.Expedition.DEFAULT_SELECTED_FOOD_DAYS;
            }
            else if(foodCount == SystemConst.Expedition.DEFAULT_SELECTED_FOOD_DAYS)
            {
                foodCount = 30;
            }
            else if(foodCount == 30)
            {
                foodCount = SystemConst.Expedition.DEFAULT_FOOD_DAYS;
            }

            foodText.text = foodCount.ToString() + "日粮";
            UpdateFoodInfo();
        });

    }

    private void UpdateFoodInfo()
    {
        if (heroSelect.heroIds.Length <= 0)
            return;
        var heroList = heroSelect.heroIds;
        var soldierTotal = GameManager.Instance.GetCity(cityId).GetAttr("soldier");
        var foodCost = soldierTotal * foodCount / SystemConst.Expedition.SOLDIER_FOOD_COST_DIVISOR;
        var citySrc = GameManager.Instance.GetCity(cityId);
        foodCostText.text = string.Format("{0} / {1}", foodCost, (int)citySrc.food);
        foodCostText.color = foodCost <= citySrc.food ? Color.white : SysColor.Battle.FoodLossColor;
    }

    void Update()
    {
        
    } 

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        
        foodCount = SystemConst.Expedition.DEFAULT_SELECTED_FOOD_DAYS;
        foodText.text = foodCount.ToString() + "日粮";
        foodButton.gameObject.SetActive(true);
        foodCostText.transform.parent.parent.gameObject.SetActive(true);
        foodCostText.text = "待计算";

        var devCfg = CityDevConfig.GetConfig(devId);
        attrDesText.text = devCfg.Des;

        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var force = citySrc.GetForce();

        var soldierTotal = citySrc.GetAttr("soldier");
        var foodCost = soldierTotal * foodCount / SystemConst.Expedition.SOLDIER_FOOD_COST_DIVISOR;

        PanelManager.Instance.HideCityDev();    
        
        force.ExecuteCityMoveDev(cityId, devId, heroList, foodCost, selectedCityId);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
