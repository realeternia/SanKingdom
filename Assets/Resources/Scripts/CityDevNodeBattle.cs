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

    public Button destButton;
    public SelectHeroArmyControl heroSelect;
    public Button runButton;

    private int selectedCityId;

    // Start is called before the first frame update
    void Start()
    {
        runButton.onClick.AddListener(() =>
        {
            if (selectedCityId == 0 || heroSelect.heroIds.Length <= 0)
                return;

            var devConfig = CityDevConfig.GetConfig(devId);
            PanelManager.Instance.ShowPopResultPanel(devConfig.Cname, new List<string>(), new List<int>(), new List<int>(), () =>
            {
                var heroList = heroSelect.heroIds;
                if (heroList.Length <= 0)
                    return;
                OnRun(devId, heroList);
            }, devConfig.Mp4);
        });
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

        attrDesText.text = devCfg.Des;

        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        if(selectedCityId == 0)
        {
            return;
        }

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

        var citySrc = GameManager.Instance.GetCity(cityId);
        var cityDest = GameManager.Instance.GetCity(selectedCityId);

        var devConfig = CityDevConfig.GetConfig(devId);
        if (devConfig.FindEnemy)
        {
            PanelManager.Instance.HideCityDev();
            PanelManager.Instance.HideCity();
            PanelManager.Instance.HideWorld();
            BattleManager.Instance.BattleBegin(citySrc.GetPlayer(), cityDest.GetPlayer(), cityId, selectedCityId, citySrc.GetBattleHeroList(validHeroList.ToArray()), cityDest.GetBattleHeroList());
            
            // 更新英雄的年份
            foreach (var heroId in validHeroList)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                hero.currentYear = currentYear;
            }
        }
        else
        {
            PanelManager.Instance.HideCityDev();
            citySrc.MoveHeroTo(validHeroList.ToArray(), selectedCityId);
            citySrc.RecalculateHeros();
            cityDest.RecalculateHeros();

            PanelManager.Instance.SendSignal("CityAttrChange", "", 0);
            
            // 更新英雄的年份
            foreach (var heroId in validHeroList)
            {
                var hero = GameManager.Instance.GetHero(heroId);
                hero.currentYear = currentYear;
            }
        }

    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
