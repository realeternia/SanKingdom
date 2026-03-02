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

    public TMP_Text foodText;
    private int foodCount = 10;

    public Button destButton;
    public SelectHeroArmyControl heroSelect;
    public Button foodButton;
    public Button runButton;

    public TMP_Text foodCostText;

    private int selectedCityId;

    // Start is called before the first frame update
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
            var soldierTotal = heroSelect.heroIds.Sum(x => GameManager.Instance.GetHero(x).soldier);
            var foodCost = soldierTotal * foodCount / 20;
            var citySrc = GameManager.Instance.GetCity(cityId);
            if(citySrc.food < foodCost)
            {
                SystemTip.Instance.ShowTip("食物不足");
                return;
            }            

            var devConfig = CityDevConfig.GetConfig(devId);
            PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, new List<string>(), new List<int>(), new List<int>(), () =>
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
            var devConfig = CityDevConfig.GetConfig(devId);
            PanelManager.Instance.ShowPopCitySelectPanel(cityId, devConfig.FindEnemy, (selectedCityId) =>
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
            // 在10,20,30日粮间切换
            if(foodCount == 10)
            {
                foodCount = 20;
            }
            else if(foodCount == 20)
            {
                foodCount = 30;
            }
            else if(foodCount == 30)
            {
                foodCount = 10;
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
        var soldierTotal = heroList.Sum(x => GameManager.Instance.GetHero(x).soldier);
        var foodCost = soldierTotal * foodCount / 20;
        var citySrc = GameManager.Instance.GetCity(cityId);
        foodCostText.text = string.Format("{0} / {1}", foodCost, citySrc.food);
        foodCostText.color = foodCost <= citySrc.food ? Color.white : Color.red;
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
        if(devCfg.FindEnemy)
        {
            foodCount = 20;
            foodText.text = foodCount.ToString() + "日粮";
            foodButton.gameObject.SetActive(true);
            foodCostText.transform.parent.parent.gameObject.SetActive(true);
            foodCostText.text = "待计算";
        }
        else
        {
            foodButton.gameObject.SetActive(false);
            foodCostText.transform.parent.parent.gameObject.SetActive(false);
        }

        attrDesText.text = devCfg.Des;

        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        var citySrc = GameManager.Instance.GetCity(cityId);
        var player = citySrc.GetPlayer();

        var soldierTotal = heroList.Sum(x => GameManager.Instance.GetHero(x).soldier);
        var foodCost = soldierTotal * foodCount / 20;

        // 隐藏相关UI面板
        PanelManager.Instance.HideCityDev();
        var devConfig = CityDevConfig.GetConfig(devId);
        if(devConfig.FindEnemy)
        {
            PanelManager.Instance.HideCity();
            PanelManager.Instance.HideWorld();
        }        
        
        // 执行城市战斗发展
        player.ExecuteCityBattleDev(cityId, devId, heroList, foodCost, selectedCityId);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
