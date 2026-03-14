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
        int[] heroList = GameManager.Instance.GetCity(cityId).GetHeroList(true).ToArray(); // 只获取在野英雄
        int[] initialSelected = wildHeroId > 0 ? new int[] { wildHeroId } : new int[0];

        PanelManager.Instance.ShowPopHeroSelectPanel(cityId, 1, heroList, initialSelected, new string[0], (selectedIds) =>
        {
            if (selectedIds.Count > 0)
            {
                wildHeroId = selectedIds[0]; // 保持选中的heroId，只取第一个
                
                // 更新heroSelect的heroIds
                heroSelect.heroIds = new int[] { wildHeroId };
                
                // 更新英雄头像显示
                for (int i = 0; i < heroSelect.heroHeads.Length; i++)
                {
                    if (i == 0)
                    {
                        heroSelect.heroHeads[i].gameObject.SetActive(true);
                        var heroCfg = HeroConfig.GetConfig(wildHeroId);
                        heroSelect.heroHeads[i].sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
                        
                        // 更新wildHeroText显示选中的英雄名字
                        if (wildHeroText != null)
                        {
                            wildHeroText.text = heroCfg.Name;
                        }
                    }
                    else
                    {
                        heroSelect.heroHeads[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                wildHeroId = 0;
                
                // 重置heroSelect的heroIds
                heroSelect.heroIds = new int[0];
                
                // 重置英雄头像显示
                heroSelect.heroHeads[0].gameObject.SetActive(true);
                heroSelect.heroHeads[0].sprite = Resources.Load<Sprite>("Skins/moren");
                
                // 重置wildHeroText显示
                if (wildHeroText != null)
                {
                    wildHeroText.text = "请选择英雄";
                }
                
                // 隐藏其他头像
                for (int i = 1; i < heroSelect.heroHeads.Length; i++)
                {
                    heroSelect.heroHeads[i].gameObject.SetActive(false);
                }
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
        heroSelect.SetDevId(cityId, devId, true); // 第三个参数为true，表示只显示在野英雄
    }

    private void OnRun(int devId, int[] heroList)
    {
        PanelManager.Instance.HideCityDev();
        
        List<string> attrs;
        List<int> attrOlds;
        List<int> results;

        var cityData = GameManager.Instance.GetCity(cityId);
        if(!cityData.GetPlayer().ExecuteCityUseHero(cityId, devId, heroList, 123, out attrs, out attrOlds, out results))
        {
            return;
        }
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