using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;

public class CitySearchPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    public ResCheckItem resCheckItemGold;

    public Button closeButton;
    public Button okButton;

    private int forceId;
    private int cityId;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCitySearch();
        });

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnSearch);
        }
    }

    public void Init(int forceId, int sourceCityId)
    {
        this.forceId = forceId;
        this.cityId = sourceCityId;

        resCheckItemGold.Init("gold");

        CreateHeroHeadItems();
        RefreshGoldDisplay();
    }

    private void RefreshGoldDisplay()
    {
        var force = GameManager.Instance.GetForce(forceId);
        int gold = force != null ? (int)force.gold : 0;
        int selectedCount = GetSelectedCount();

        if (selectedCount == 0)
        {
            resCheckItemGold.UpdateDisplay($"{gold}");
        }
        else
        {
            var devCfg = CityDevConfig.GetConfig(CityDevConfig.GetConfigByName("Search").Id);
            int cost = selectedCount * (devCfg != null ? devCfg.GoldCost : 0);
            if (cost > gold)
            {
                resCheckItemGold.UpdateDisplay($"<color=red>{cost}</color>/{gold}");
            }
            else
            {
                resCheckItemGold.UpdateDisplay($"{cost}/{gold}");
            }
        }
    }

    private bool CanSelectHero()
    {
        return true;
    }

    private void OnHeroSelectionChanged()
    {
        RefreshGoldDisplay();
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

    private void OnSearch()
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择走访武将");
            return;
        }

        int[] heroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null)
        {
            GameLog.Error($"CitySearchPanelManager.OnSearch force not found forceId={forceId}");
            return;
        }

        bool success = force.ExecuteCitySearch(cityId, CityDevConfig.GetConfigByName("Search").Id, heroIds, out var attrDatas);
        if (!success) return;

        var devCfg = CityDevConfig.GetConfig(CityDevConfig.GetConfigByName("Search").Id);
        PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
        {
            RefreshGoldDisplay();
            CreateHeroHeadItems();
        }, CityDevKingActionConfig.GetConfig(CityDevConfig.GetConfigByName("Search").Id).Mp4, false);
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

        var heroList = new List<int>();
        foreach (var city in force.GetCityList())
        {
            heroList.AddRange(city.GetNormalHeroList());
        }
        if (heroList.Count == 0) return;

        int currentRound = GameManager.Instance.SaveData.round;
        heroList = heroList.OrderBy(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null && hero.round >= currentRound ? 1 : 0;
            })
            .ThenByDescending(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null ? hero.charm : 0;
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
        var hero = GameManager.Instance.GetHero(heroId);
        if (hero == null) return "";
        return $"智{SysColor.GetColoredText("inte", hero.inte)} 魅{SysColor.GetColoredText("charm", hero.charm)}";
    }
}
