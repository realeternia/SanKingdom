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

    public TMP_Text goldCostText;

    private int forceId;
    private int cityId;
    private int devId;
    private int methodId = 1;

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
        this.methodId = 1;
        UpdateMethodUI();
        CreateHeroHeadItems();
    }

    private void OnCheckBtnChange(int index)
    {
        methodId = index + 1;
        UpdateMethodUI();
    }

    private void UpdateMethodUI()
    {
        if (goldCostText != null)
        {
            if (methodId == 2)
            {
                int selectedCount = GetSelectedItems().Count;
                int cost = selectedCount * SystemConst.Hero.PRAISE_GOLD_COST_PER_HERO;
                var force = GameManager.Instance.GetForce(forceId);
                goldCostText.text = string.Format("{0} / {1}", cost, force != null ? (int)force.gold : 0);
                goldCostText.gameObject.SetActive(true);
            }
            else
            {
                goldCostText.gameObject.SetActive(false);
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

        var force = GameManager.Instance.GetForce(forceId);
        if (methodId == 2)
        {
            int totalCost = selectedItems.Count * SystemConst.Hero.PRAISE_GOLD_COST_PER_HERO;
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

        var devCfg = CityDevConfig.GetConfig(devId);
        bool success = force.ExecuteCityPraiseHero(cityId, devId, heroIds.ToArray(), methodId, out var attrDatas);

        if (success)
        {
            PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
            {
                PanelManager.Instance.HideCityPraise();
            }, devCfg != null ? devCfg.Mp4 : "", false);
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
                if (heroData != null)
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
        return $"忠{heroData.loyalty}";
    }
}
