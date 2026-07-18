using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityUseHeroPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    private int forceId;
    private int cityId;
    private int devId;
    private List<int> selectedHeroIds = new List<int>();
    private bool isCrossCountry = false;
    private int useHeroCount;

    public Button heroButton;
    public Button dayButton;
    public TMP_Text attrVal1Text;
    public TMP_Text dayButtonText;

    public ResCheckItem resCheckItem;    

    public Button closeButton;
    public Button okButton;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityUseHero();
        });
        heroButton.onClick.AddListener(() =>
        {
            SideHeroSelector.SetContext(cityId, forceId, isCrossCountry, (newHeroIds) =>
            {
                selectedHeroIds = newHeroIds ?? new List<int>();
                if (selectedHeroIds.Count == 0)
                {
                    attrVal1Text.text = "-";
                }
                else
                {
                    var cfg = HeroConfig.GetConfig(selectedHeroIds[0]);
                    attrVal1Text.text = cfg != null ? cfg.Name : selectedHeroIds[0].ToString();
                }
                RefreshHeroHeadItemsForRecruit();
            });
            PanelManager.Instance.ShowSideBar("SideHeroSelector");
        });

        if (dayButton != null)
        {
            dayButton.onClick.AddListener(OnDayButtonClick);
        }

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnUseHero);
        }

        UpdateDayButtonText();
    }

    private void OnDayButtonClick()
    {
        isCrossCountry = !isCrossCountry;
        UpdateDayButtonText();
        SideHeroSelector.UpdateCrossCountry(isCrossCountry);
        // 已选目标在切换模式后失效（归属不符），清空并刷新
        selectedHeroIds.Clear();
        attrVal1Text.text = "-";
        RefreshHeroHeadItemsForRecruit();
    }

    private void UpdateDayButtonText()
    {
        if (dayButtonText != null)
            dayButtonText.text = isCrossCountry ? "他国家" : "本国内";
    }

    void Update()
    {

    }

    public void Init(int forceId, int cityId, int devId)
    {
        this.forceId = forceId;
        this.cityId = cityId;
        this.devId = devId;
        this.selectedHeroIds.Clear();
        isCrossCountry = false;

        var devCfg = CityDevConfig.GetConfig(devId);
        useHeroCount = devCfg != null ? devCfg.HeroCount : 0;

        InitResCheckItem();

        attrVal1Text.text = "-";
        UpdateDayButtonText();
        CreateHeroHeadItems();
    }

    public bool CanSelectItem()
    {
        return selectedHeroIds.Count > 0;
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

    private void InitResCheckItem()
    {
        if (resCheckItem == null) return;

        var devCfg = CityDevConfig.GetConfig(devId);
        if (devCfg == null) return;

        var force = GameManager.Instance.GetForce(forceId);
        int usedCount = force != null ? force.GetKingActionCount(devId) : 0;
        resCheckItem.Init("hero");
        resCheckItem.UpdateDisplay($"{usedCount}/{useHeroCount}");
    }

    private bool CanSelectHero()
    {
        var force = GameManager.Instance.GetForce(forceId);
        int usedCount = force != null ? force.GetKingActionCount(devId) : 0;
        return usedCount + GetSelectedItems().Count < useHeroCount;
    }

    private void OnHeroSelectionChanged()
    {
        UpdateResCheckDisplay();
    }

    private void UpdateResCheckDisplay()
    {
        if (resCheckItem == null) return;

        var force = GameManager.Instance.GetForce(forceId);
        int usedCount = force != null ? force.GetKingActionCount(devId) : 0;
        int totalCount = usedCount + GetSelectedItems().Count;
        if (totalCount >= useHeroCount)
        {
            resCheckItem.UpdateDisplay("<color=red>已满</color>");
        }
        else
        {
            resCheckItem.UpdateDisplay($"{totalCount}/{useHeroCount}");
        }
    }

    private void OnUseHero()
    {
        if (selectedHeroIds.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择登用武将");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择执行武将");
            return;
        }

        int[] executorHeroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var devCfg = CityDevConfig.GetConfig(devId);
        var force = GameManager.Instance.GetForce(forceId);

        force.ExecuteCityUseHero(cityId, devId, executorHeroIds, selectedHeroIds.ToArray(), out var allAttrDatas);

        PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, allAttrDatas, () =>
        {
            PanelManager.Instance.HideCityUseHero();
        }, devCfg != null ? CityDevKingActionConfig.GetConfig(devId).Mp4 : "", false);
    }

    private void CreateHeroHeadItems()
    {
        // 保存当前选中的执行武将，便于重建后恢复选中状态
        var selectedExecutorIds = new HashSet<int>();
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null && itemScript.IsSelected())
            {
                selectedExecutorIds.Add(itemScript.GetHeroId());
            }
        }

        foreach (var item in heroHeadItems)
        {
            if (item != null)
                Destroy(item);
        }
        heroHeadItems.Clear();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;

        var cities = GameManager.Instance.GetCitiesByForce(forceId);

        int currentRound = GameManager.Instance.SaveData.round;
        int kingHeroId = ForceConfig.GetConfig(forceId).HeroId;
        bool hasTargets = selectedHeroIds.Count > 0;

        // 选定目标后，计算执行武将消耗日程（主公城市 → 目标武将所在城市）
        int recruitDay = 0;
        if (hasTargets)
        {
            var kingCity = force.GetKingCity();
            int kingCityId = kingCity != null ? kingCity.cityId : cityId;
            var targetHero = GameManager.Instance.GetHero(selectedHeroIds[0]);
            int targetCityId = targetHero != null ? targetHero.cityId : cityId;
            recruitDay = SysFormula.City.CalculateHeroDayDistance(kingCityId, targetCityId, isCrossCountry);
        }

        List<int> heroList = new List<int>();
        foreach (var city in cities)
        {
            var cityHeroes = city.GetNormalHeroList();
            heroList.AddRange(cityHeroes);
        }

        if (heroList.Count == 0) return;

        // 已行动武将排到末尾；有登庸目标时按成功率倒排，再按君主优先、魅力降序
        heroList = heroList.OrderBy(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null && hero.round >= currentRound ? 1 : 0;
            })
            .ThenByDescending(h => hasTargets ? CalculateBestRecruitRate(h) : 0)
            .ThenByDescending(h => h == kingHeroId ? 1 : 0)
            .ThenByDescending(h => GameManager.Instance.GetHero(h).GetAttr("charm"))
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
                int heroId = heroList[i];
                string attText = hasTargets ? CalculateBestRecruitRate(heroId) + "%" : GetHeroAttText(heroId);
                var heroData = GameManager.Instance.GetHero(heroId);
                bool hasActed = heroData != null && heroData.round >= currentRound;
                itemScript.Init(heroId, attText, forceId, hasActed);
                itemScript.SetCallbacks(CanSelectHero, OnHeroSelectionChanged);
                itemScript.SetDayText(recruitDay);

                if (selectedExecutorIds.Contains(heroId))
                {
                    itemScript.SetSelected(true);
                }
            }

            heroHeadItems.Add(itemObj);
        }

        int totalRows = (heroList.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    /// <summary>
    /// 当登庸目标变化时，重建 HeroHeadItem：ItemAttr 显示登庸成功率，按成功率倒排
    /// </summary>
    private void RefreshHeroHeadItemsForRecruit()
    {
        CreateHeroHeadItems();
    }

    /// <summary>
    /// 计算执行武将对当前选中目标的最佳登庸成功率
    /// </summary>
    private int CalculateBestRecruitRate(int executorHeroId)
    {
        int bestRate = 0;
        foreach (int targetId in selectedHeroIds)
        {
            int rate = SysFormula.Hero.CalculateRecruitRate(cityId, executorHeroId, targetId);
            if (rate > bestRate)
            {
                bestRate = rate;
            }
        }
        return bestRate;
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
        if (heroData == null) return "";
        return $"魅{SysColor.GetColoredText("charm", heroData.charm)}";
    }
}
