using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityPraisePanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    public Button closeButton;
    public Button okButton;
    public NLMutiCheckButton checkBtn;

    private int forceId;
    private int cityId;
    private int devId;
    private int methodId = 1;
    private int praiseHeroCount;

    public ResCheckItem resCheckItem;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityPraise();
        });

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnPraise);
        }

        if (checkBtn != null)
        {
            checkBtn.Init(new string[] { "褒奖", "奖赏" });
            checkBtn.SelectIndexChange = OnCheckBtnChange;
        }
    }

    void Update()
    {

    }

    public void Init(int forceId, int cityId, int devId)
    {
        this.forceId = forceId;
        this.cityId = cityId;
        this.devId = devId;
        // 根据入口 devId 决定默认 methodId（21205→褒奖, 21206→奖赏）
        this.methodId = (devId == SystemConst.CityDev.PRAISE_PAID_DEV_ID) ? 2 : 1;

        var praiseDevCfg = CityDevConfig.GetConfig(SystemConst.CityDev.PRAISE_DEV_ID);
        praiseHeroCount = praiseDevCfg != null ? praiseDevCfg.HeroCount : 0;

        if (checkBtn != null)
        {
            checkBtn.SetSelectedIndexExternal(methodId - 1);
        }
        InitResCheckItem();
        CreateHeroHeadItems();
    }

    private void OnCheckBtnChange(int index)
    {
        methodId = index + 1;
        InitResCheckItem();
    }

    /// <summary>
    /// 根据 methodId 获取实际的 devId（褒奖=21205, 奖赏=21206）
    /// 面板集成两个行动，读取配置时需按当前 methodId 取对应 devId
    /// </summary>
    private int GetActualDevId()
    {
        return methodId == 2
            ? SystemConst.CityDev.PRAISE_PAID_DEV_ID
            : SystemConst.CityDev.PRAISE_DEV_ID;
    }

    private void InitResCheckItem()
    {
        if (resCheckItem == null) return;

        var devCfg = CityDevConfig.GetConfig(GetActualDevId());
        if (devCfg == null) return;

        if (methodId == 1)
        {
            var force = GameManager.Instance.GetForce(forceId);
            int praisedCount = force != null ? force.GetKingActionCount(SystemConst.CityDev.PRAISE_DEV_ID) : 0;
            resCheckItem.Init("hero");
            resCheckItem.UpdateDisplay($"{praisedCount}/{praiseHeroCount}");
        }
        else
        {
            resCheckItem.Init("gold");
            resCheckItem.UpdateDisplay(((int)(GameManager.Instance.GetForce(forceId)?.gold ?? 0)).ToString());
        }
    }

    private bool CanSelectHero()
    {
        if (methodId == 1)
        {
            var force = GameManager.Instance.GetForce(forceId);
            int praisedCount = force != null ? force.GetKingActionCount(SystemConst.CityDev.PRAISE_DEV_ID) : 0;
            return praisedCount + GetSelectedItems().Count < praiseHeroCount;
        }
        return true;
    }

    private void OnHeroSelectionChanged()
    {
        UpdateResCheckDisplay();
    }

    private void UpdateResCheckDisplay()
    {
        if (resCheckItem == null) return;

        if (methodId == 1)
        {
            var force = GameManager.Instance.GetForce(forceId);
            int praisedCount = force != null ? force.GetKingActionCount(SystemConst.CityDev.PRAISE_DEV_ID) : 0;
            int totalCount = praisedCount + GetSelectedItems().Count;
            if (totalCount >= praiseHeroCount)
            {
                resCheckItem.UpdateDisplay("<color=red>已满</color>");
            }
            else
            {
                resCheckItem.UpdateDisplay($"{totalCount}/{praiseHeroCount}");
            }
        }
        else
        {
            var force = GameManager.Instance.GetForce(forceId);
            var devCfg = CityDevConfig.GetConfig(GetActualDevId());
            int gold = force != null ? (int)force.gold : 0;
            int cost = GetSelectedItems().Count * (devCfg != null ? devCfg.GoldCost : 0);
            if (cost > gold)
            {
                resCheckItem.UpdateDisplay($"<color=red>{cost}</color>/{gold}");
            }
            else
            {
                resCheckItem.UpdateDisplay($"{cost}/{gold}");
            }
        }
    }

    private List<HeroHeadItem> GetSelectedItems()
    {
        List<HeroHeadItem> selected = new List<HeroHeadItem>();
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null && itemScript.IsSelected())
            {
                selected.Add(itemScript);
            }
        }
        return selected;
    }

    private void OnPraise()
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择武将");
            return;
        }

        // 面板集成 21205/21206，按 methodId 取实际 devId 的配置
        int actualDevId = GetActualDevId();
        var devCfg = CityDevConfig.GetConfig(actualDevId);

        var force = GameManager.Instance.GetForce(forceId);
        if (devCfg.GoldCost > 0)
        {
            int totalCost = selectedItems.Count * devCfg.GoldCost;
            if (force != null && force.gold < totalCost)
            {
                SystemTip.Instance.ShowTip("黄金不足");
                return;
            }
        }

        List<int> heroIds = new List<int>();
        foreach (var item in selectedItems)
        {
            heroIds.Add(item.GetHeroId());
        }

        bool success = force.ExecuteCityPraiseHero(cityId, actualDevId, heroIds.ToArray(), out var attrDatas);

        if (success)
        {
            PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
            {
                PanelManager.Instance.HideCityPraise();
            }, devCfg.Mp4, false);
        }
    }

    private void CreateHeroHeadItems()
    {
        foreach (var item in heroHeadItems)
        {
            if (item != null)
                Destroy(item);
        }
        heroHeadItems.Clear();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;

        var kingCity = force.GetKingCity();
        int kingCityId = kingCity != null ? kingCity.cityId : 0;

        var cities = GameManager.Instance.GetCitiesByForce(forceId);
        cities = cities.OrderByDescending(c => c.cityId == kingCityId).ToList();

        List<SaveHeroData> allHeroes = new List<SaveHeroData>();
        foreach (var city in cities)
        {
            var heroIds = city.GetNormalHeroList();
            foreach (var hid in heroIds)
            {
                var heroData = GameManager.Instance.GetHero(hid);
                if (heroData != null && heroData.loyalty < SystemConst.Hero.MAX_LOYALTY)
                {
                    allHeroes.Add(heroData);
                }
            }
        }

        int currentRound = GameManager.Instance.SaveData.round;
        // 已行动武将排到末尾，再按忠诚升序
        allHeroes = allHeroes.OrderBy(h => h.round >= currentRound ? 1 : 0)
                             .ThenBy(h => h.loyalty)
                             .ToList();

        if (allHeroes.Count == 0) return;

        var itemPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelGismo("HeroHeadItem"));

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 156f;
        float itemHeight = 185f;
        float spacing = 10f;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((containerRect.rect.width + spacing) / (itemWidth + spacing)));

        float totalWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * spacing;
        float startX = -totalWidth / 2f + itemWidth / 2f;

        for (int i = 0; i < allHeroes.Count; i++)
        {
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;

            float posX = startX + col * (itemWidth + spacing);
            float posY = -row * (itemHeight + spacing);

            GameObject itemObj = Instantiate(itemPrefab, itemRegionMain.transform);
            itemObj.transform.localScale = Vector3.one;

            RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(posX, posY);
                rectTransform.sizeDelta = new Vector2(itemWidth, itemHeight);
            }

            HeroHeadItem itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null)
            {
                string attText = GetHeroAttText(allHeroes[i]);
                bool hasActed = allHeroes[i].round >= GameManager.Instance.SaveData.round;
                itemScript.Init(allHeroes[i].heroId, attText, forceId, hasActed);
                itemScript.SetCallbacks(CanSelectHero, OnHeroSelectionChanged);
            }

            heroHeadItems.Add(itemObj);
        }

        int totalRows = (allHeroes.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }

    private string GetHeroAttText(SaveHeroData heroData)
    {
        return $"忠{SysColor.GetColoredText("loyalty", heroData.loyalty)}";
    }
}
