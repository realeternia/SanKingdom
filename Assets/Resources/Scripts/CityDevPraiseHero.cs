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
    private int methodId = 1;

    public TMP_Text methodText;
    public TMP_Text attrDesText;
    public TMP_Text costText;
    
    public Button runButton;
    public Button switchButton;
    public SelectHeroControl heroSelect;

    void Start()
    {
        switchButton.onClick.AddListener(() =>
        {
            OnSwitchMethod();
        });

        runButton.onClick.AddListener(() =>
        {
            var heroIds = heroSelect.heroIds;
            if(heroIds.Length == 0)
            {
                SystemTip.Instance.ShowTip("请选择至少一个英雄");
                return;
            }
            OnRun(devId, heroIds);
        });

        heroSelect.OnHeroCountChange = (count) =>
        {
            UpdateCostText(count);
        };
    }

    private void OnSwitchMethod()
    {
        if(methodId == 1)
        {
            methodId = 2;
            methodText.text = "赏赐";
            attrDesText.text = "消耗每人100金,提升忠心度3-5";
        }
        else
        {
            methodId = 1;
            methodText.text = "褒奖";
            attrDesText.text = "消耗武将本回合行动力,提升忠心度1-3";
        }
        heroSelect.SetMode(methodId);
        UpdateCostText(0);
    }

    private void UpdateCostText(int heroCount)
    {
        if(costText != null)
        {
             var cityData = GameManager.Instance.GetCity(cityId);
            if(methodId == 2)
            {
                costText.text = string.Format("{0}/{1}", heroCount * SystemConst.Hero.PRAISE_GOLD_COST_PER_HERO, cityData.gold);
            }
            else
            {
                costText.text = string.Format("{0}/{1}", 0, cityData.gold);
            }
        }
    }

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        this.methodId = 1;
        
        methodText.text = "褒奖";
        attrDesText.text = "消耗武将本回合行动力，提升忠心度1-3";

        heroSelect.SetMode(methodId);
        UpdateCostText(0);

        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        var cityData = GameManager.Instance.GetCity(cityId);
        List<PopResultPanelManager.AttrData> attrDatas;

        if(!cityData.GetPlayer().ExecuteCityPraiseHero(cityId, devId, heroList, methodId, out attrDatas))
        {
            return;
        }
        
        PanelManager.Instance.HideCityDev();
        
        var devConfig = CityDevConfig.GetConfig(devId);
        string title = methodId == 1 ? "褒奖" : "赏赐";
        PanelManager.Instance.ShowPopResultPanel(title, attrDatas, null, devConfig.Mp4);
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }
}
