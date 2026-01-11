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

    private int selectedCityId;

    // Start is called before the first frame update
    void Start()
    {
        runButton.onClick.AddListener(() =>
        {
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
            PanelManager.Instance.ShowPopCitySelectPanel(cityId, (selectedCityId) =>
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
        goldCostText.text = devCfg.GoldCost.ToString() + "/" + cityData.gold.ToString();

        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        if(selectedCityId == 0)
        {
            return;
        }
        PanelManager.Instance.HideCityDev();
        PanelManager.Instance.HideCity();
        PanelManager.Instance.HideWorld();

        var cityAttack = GameManager.Instance.GetCity(cityId);
        var cityDefense = GameManager.Instance.GetCity(selectedCityId);

        BattleManager.Instance.BattleBegin(cityAttack.GetPlayer(), cityDefense.GetPlayer(), cityAttack.GetBattleHeroList(heroList), cityDefense.GetBattleHeroList());
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
