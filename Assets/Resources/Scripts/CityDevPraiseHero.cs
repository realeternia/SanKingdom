using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevPraiseHero : MonoBehaviour, ICityDevNode
{
    private int cityId;
    private int devId;

    public int methodId;
    public TMP_Text methodText;
    public TMP_Text attrDesText;

    public SelectHeroControl heroSelect;
    public Button runButton;
    public Button switchButton;

    // Start is called before the first frame update
    void Start()
    {
        runButton.onClick.AddListener(() =>
        {
            var heroList = heroSelect.heroIds;
            if(heroList.Length == 0)
            {
                SystemTip.Instance.ShowTip("请选择至少一个英雄");
                return;
            }
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

        attrDesText.text = devCfg.Des;
 
        // 设置英雄选择控件，获取在野英雄
        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        PanelManager.Instance.HideCityDev();
        
        List<PopResultPanelManager.AttrData> attrDatas;

        var cityData = GameManager.Instance.GetCity(cityId);
        // if(!cityData.GetPlayer().ExecuteCityUseHero(cityId, devId, heroList[0], wildHeroId, out attrDatas))
        // {
        //     return;
        // }
        var devConfig = CityDevConfig.GetConfig(devId);
        
       // PanelManager.Instance.ShowPopResultPanel(CityDevConfig.GetConfig(devId).Cname, attrDatas, null, devConfig.Mp4);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}