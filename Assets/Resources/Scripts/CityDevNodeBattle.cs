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

        var citySrc = GameManager.Instance.GetCity(cityId);
        var player = citySrc.GetPlayer();

        // 隐藏相关UI面板
        PanelManager.Instance.HideCityDev();
        
        // 执行城市战斗发展
        player.ExecuteCityBattleDev(cityId, devId, heroList, selectedCityId);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
