using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using System;

public class RankCellInfoForce : MonoBehaviour, IRankDetailInfo, IRankDetailInfoHeader
{
    public RankPanelManager rankPanelManager;

    public TMP_Text forceName;
    public TMP_Text forceCities; // 城市数量
    public TMP_Text forceHeroes; // 武将数量
    public TMP_Text forceSoldier; // Soldier 总数
    public TMP_Text forceGold; // 金钱
    public TMP_Text forceFood; // 粮食

    public Button btnSoldier;
    public Button btnCities;
    public Button btnHeroes;
    public Button btnGold;
    public Button btnFood;

    public GameObject nodeHeader;
    public GameObject nodeRow;

    public int forceId;

    // 存储数值用于排序
    private int soldierValue;
    private int citiesValue;
    private int heroesValue;
    private int goldValue;
    private int foodValue;

    // Start is called before the first frame update
    void Start()
    {
        forceName.raycastTarget = false;
        forceCities.raycastTarget = false;
        forceHeroes.raycastTarget = false;
        forceSoldier.raycastTarget = false;
        forceGold.raycastTarget = false;
        forceFood.raycastTarget = false;
    }

    public void SetManager(RankPanelManager rankPanelManager)
    {
        this.rankPanelManager = rankPanelManager;
    }    

    public void SetMode(bool isHeader)
    {
        if(isHeader)
        {
            nodeHeader.SetActive(true);
            nodeRow.SetActive(false);
            btnSoldier.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Soldier");
            });
            btnCities.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Cities");
            });
            btnHeroes.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Heroes");
            });
            btnGold.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Gold");
            });
            btnFood.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Food");
            });
        }
        else
        {
            nodeHeader.SetActive(false);
            nodeRow.SetActive(true);
        }
    }

    public int GetValInt(string key)
    {
        switch (key)
        {
            case "Soldier":
                return soldierValue;
            case "Cities":
                return citiesValue;
            case "Heroes":
                return heroesValue;
            case "Gold":
                return goldValue;
            case "Food":
                return foodValue;
            default:
                return 0;
        }
    }

    public void Init(int forceId)
    {
        this.forceId = forceId;
        var forceCfg = ForceConfig.GetConfig(forceId);
        
        forceName.text = forceCfg.Cname;
        
        // 统计城市数量
        var cities = GameManager.Instance.GetCitiesByForce(forceId);
        citiesValue = cities.Count;
        forceCities.text = citiesValue.ToString();
        
        // 统计武将数量
        int heroCount = 0;
        // 统计 Soldier 总数
        int totalSoldier = 0;        
        foreach (var heroData in GameManager.Instance.SaveData.heros)
        {
            if (heroData.state == HeroState.Normal && heroData.forceId == forceId)
            {
                heroCount++;
            }
        }
        heroesValue = heroCount;
        forceHeroes.text = heroesValue.ToString();
        
        // 统计城市的 Soldier、金钱和粮食
        foreach (var city in cities)
        {
            totalSoldier += (int)Math.Floor(city.GetAttr("soldier"));
        }
        
        int totalGold = 0;
        int totalFood = 0;
        foreach (var city in cities)
        {
            totalGold += (int)Math.Floor(city.GetAttr("gold"));
            totalFood += (int)Math.Floor(city.GetAttr("food"));
        }
        
        // 存储原始值用于排序
        soldierValue = totalSoldier;
        goldValue = totalGold;
        foodValue = totalFood;
        
        // 显示时转换为"万"单位
        forceSoldier.text = FormatToWan(totalSoldier);
        forceGold.text = FormatToWan(totalGold);
        forceFood.text = FormatToWan(totalFood);
    }

    // 将数值转换为"万"单位的字符串
    private string FormatToWan(int value)
    {
        float wanValue = value / 10000f;
        return wanValue.ToString("F1") + "万";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnSelectHero(bool isSelected)
    {
        // 对于势力信息，不需要特别的选中处理
    }
}