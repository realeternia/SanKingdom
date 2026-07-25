using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;

public class CityTradePanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    public Button destButton;
    public TMP_Text cityNameText;

    public ResCheckItem resCheckItemGold;
    public ResCheckItem resCheckItemBuy;

    public Button closeButton;
    public Button okButton;
    public Button switchButton;

    private int forceId;
    private int cityId;
    private bool buySoldier = true;

    private int goldCost;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityTrade();
        });

        if (destButton != null)
        {
            destButton.onClick.AddListener(OnDestButtonClick);
        }

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnBuy);
        }

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(OnSwitch);
        }
    }

    public void Init(int forceId, int sourceCityId)
    {
        this.forceId = forceId;
        this.cityId = sourceCityId;
        this.buySoldier = true;

        var devCfg = CityDevConfig.GetConfig(CityDevConfig.GetConfigByName("Trade").Id);
        goldCost = devCfg.GoldCost;

        resCheckItemGold.Init("gold");

        UpdateBuyItemIcon();
        RefreshCityDisplay();
        CreateHeroHeadItems();
        RefreshGoldDisplay();
        RefreshBuyDisplay();
    }

    private void OnDestButtonClick()
    {
        var cityIds = MapTool.GetOwnCityIds(forceId);
        PanelManager.Instance.ShowSideBar("SideCitySelector", (panelObj) =>
        {
            var selector = panelObj.GetComponent<SideCitySelector>();
            selector.Init(cityId, cityIds, "soldier", "food", (newCityId) =>
            {
                if (newCityId == 0) return;
                cityId = newCityId;
                buySoldier = true;
                UpdateBuyItemIcon();
                RefreshCityDisplay();
                ClearAllSelections();
                CreateHeroHeadItems();
                RefreshGoldDisplay();
                RefreshBuyDisplay();
            });
        });
    }

    private void OnSwitch()
    {
        buySoldier = !buySoldier;
        UpdateBuyItemIcon();
        RefreshCityDisplay();
        RefreshBuyDisplay();
    }

    private void UpdateBuyItemIcon()
    {
        if (resCheckItemBuy == null) return;
        string attrName = buySoldier ? "soldier" : "food";
        resCheckItemBuy.Init(attrName);
    }

    private void RefreshCityDisplay()
    {
        var cityCfg = WorldConfig.GetConfig(cityId);
        if (cityNameText != null)
        {
            cityNameText.text = cityCfg != null ? cityCfg.Cname : "-";
        }
    }

    private void RefreshBuyDisplay()
    {
        if (resCheckItemBuy == null) return;

        var cityData = GameManager.Instance.GetCity(cityId);
        string attrName = buySoldier ? "soldier" : "food";
        int val = cityData != null ? (int)cityData.GetAttr(attrName) : 0;

        var selectedItems = GetSelectedItems();
        int totalGain = 0;
        foreach (var item in selectedItems)
        {
            var hero = GameManager.Instance.GetHero(item.GetHeroId());
            int inte = hero != null ? hero.inte : 0;
            totalGain += SysFormula.Economy.CalculateHeroTradeAmount(goldCost, inte);
        }

        if (selectedItems.Count > 0)
        {
            resCheckItemBuy.UpdateDisplay($"{val}(<color=green>+{totalGain}</color>)");
        }
        else
        {
            resCheckItemBuy.UpdateDisplay($"{val}");
        }
    }

    private void ClearAllSelections()
    {
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null)
            {
                itemScript.SetSelected(false);
            }
        }
    }

    private void RefreshGoldDisplay()
    {
        var force = GameManager.Instance.GetForce(forceId);
        int gold = force != null ? (int)force.gold : 0;
        int selectedCount = GetSelectedCount();
        int totalCost = selectedCount * goldCost;

        if (resCheckItemGold != null)
        {
            if (selectedCount == 0)
            {
                resCheckItemGold.UpdateDisplay(gold < goldCost ? $"<color=red>{gold}</color>" : $"{gold}");
            }
            else if (gold < totalCost)
            {
                resCheckItemGold.UpdateDisplay($"<color=red>{gold}(-{totalCost})</color>");
            }
            else
            {
                resCheckItemGold.UpdateDisplay($"{gold}(-{totalCost})");
            }
        }
    }

    private bool CanSelectHero()
    {
        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return false;
        return (GetSelectedCount() + 1) * goldCost <= force.gold;
    }

    private void OnHeroSelectionChanged()
    {
        RefreshGoldDisplay();
        RefreshBuyDisplay();
    }

    private int GetSelectedCount()
    {
        int count = 0;
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null && itemScript.IsSelected())
                count++;
        }
        return count;
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

    private void OnBuy()
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择交易武将");
            return;
        }

        int[] heroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null)
        {
            GameLog.Error($"CityTradePanelManager.OnBuy force not found forceId={forceId}");
            return;
        }

        bool success = force.ExecuteCityTrade(cityId, CityDevConfig.GetConfigByName("Trade").Id, heroIds, buySoldier, out var attrDatas);
        if (!success) return;

        var devCfg = CityDevConfig.GetConfig(CityDevConfig.GetConfigByName("Trade").Id);
        PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
        {
            RefreshCityDisplay();
            RefreshGoldDisplay();
            RefreshBuyDisplay();
            CreateHeroHeadItems();
        }, CityDevKingActionConfig.GetConfig(CityDevConfig.GetConfigByName("Trade").Id).Mp4, false);
    }

    private void CreateHeroHeadItems()
    {
        foreach (var item in heroHeadItems)
        {
            if (item != null)
                Destroy(item);
        }
        heroHeadItems.Clear();

        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null) return;

        var heroList = cityData.GetNormalHeroList();
        if (heroList == null || heroList.Count == 0) return;

        int currentRound = GameManager.Instance.SaveData.round;
        heroList = heroList.OrderBy(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null && hero.round >= currentRound ? 1 : 0;
            })
            .ThenByDescending(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null ? hero.inte : 0;
            })
            .ToList();

        var itemPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelGismo("HeroHeadItem"));

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 156f;
        float itemHeight = 185f;
        float spacing = 10f;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((containerRect.rect.width + spacing) / (itemWidth + spacing)));

        float totalWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * spacing;
        float startX = -totalWidth / 2f + itemWidth / 2f;

        for (int i = 0; i < heroList.Count; i++)
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
                string attText = GetHeroAttText(heroList[i]);
                var heroData = GameManager.Instance.GetHero(heroList[i]);
                bool hasActed = heroData != null && heroData.round >= GameManager.Instance.SaveData.round;
                itemScript.Init(heroList[i], attText, forceId, hasActed);
                itemScript.SetCallbacks(CanSelectHero, OnHeroSelectionChanged);
            }

            heroHeadItems.Add(itemObj);
        }

        int totalRows = (heroList.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }

    private string GetHeroAttText(int heroId)
    {
        var heroData = GameManager.Instance.GetHero(heroId);
        int inte = heroData != null ? heroData.inte : 0;
        int amount = SysFormula.Economy.CalculateHeroTradeAmount(goldCost, inte);
        return $"+{amount}";
    }
}
