using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityDevUseHero : MonoBehaviour, ICityDevNode
{
    private int cityId;
    private int devId;
    private int wildHeroId; // 保持选中的heroId，每次只能选一个

    public TMP_Text wildHeroText;
    public TMP_Text attrDesText;

    public SelectHeroControl heroSelect;
    public Button runButton;
    public Button wildHeroSelectButton;

    // Start is called before the first frame update
    void Start()
    {
        wildHeroSelectButton.onClick.AddListener(() =>
        {
            OnWildHeroSelectButtonClick();
        });

        runButton.onClick.AddListener(() =>
        {
            if(wildHeroId <= 0)
            {
                SystemTip.Instance.ShowTip("请选择至少一个英雄");
                return;
            }
            OnRun(devId, new int[] { wildHeroId });
        });

    }

    private void OnWildHeroSelectButtonClick()
    {
        int[] heroList = GameManager.Instance.GetCity(cityId).GetHeroList(false, true).ToArray(); // 只获取在野英雄
        int[] initialSelected = wildHeroId > 0 ? new int[] { wildHeroId } : new int[0];

        PanelManager.Instance.ShowPopHeroSelectPanel(cityId, 1, heroList, initialSelected, new string[]{"Str", "Inte"}, (selectedIds) =>
        {
            if (selectedIds.Count > 0)
            {
                wildHeroId = selectedIds[0]; // 保持选中的heroId，只取第一个
                var heroCfg = HeroConfig.GetConfig(wildHeroId);
                wildHeroText.text = heroCfg.Name;
            }
            else
            {
                wildHeroId = 0;
                wildHeroText.text = "请选择英雄";
            }
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
        wildHeroId = 0; // 重置选中的heroId
        var devCfg = CityDevConfig.GetConfig(devId);

        attrDesText.text = devCfg.Des;
        
        // 重置wildHeroText显示
        if (wildHeroText != null)
        {
            wildHeroText.text = "请选择英雄";
        }

        // 设置英雄选择控件，获取在野英雄
        heroSelect.SetDevId(cityId, devId);
    }

    private void OnRun(int devId, int[] heroList)
    {
        PanelManager.Instance.HideCityDev();
        
        List<PopResultPanelManager.AttrData> attrDatas;

        var cityData = GameManager.Instance.GetCity(cityId);
        if(!cityData.GetPlayer().ExecuteCityUseHero(cityId, devId, heroList, wildHeroId, out attrDatas))
        {
            return;
        }
        var devConfig = CityDevConfig.GetConfig(devId);
        
        PanelManager.Instance.ShowPopResultPanel(CityDevConfig.GetConfig(devId).Cname, attrDatas, null, devConfig.Mp4);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}