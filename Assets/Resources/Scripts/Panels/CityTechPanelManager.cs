using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;

public class CityTechPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    public ResCheckItem resCheckItemGold;

    public Button destTechButton;
    public TMP_Text techNameText;
    public TMP_Text techProgressText;

    public Button closeButton;
    public Button okButton;

    private int forceId;
    private int cityId;
    private int devId;
    private int selectedTechId;

    private int techHeroCount;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityTech();
        });

        if (destTechButton != null)
        {
            destTechButton.onClick.AddListener(OnDestTechButtonClick);
        }

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnResearch);
        }
    }

    public void Init(int forceId, int sourceCityId, int devId)
    {
        this.forceId = forceId;
        this.cityId = sourceCityId;
        this.devId = devId;
        this.selectedTechId = 0;

        techHeroCount = ForceTech.GetEffectiveSlotCount(forceId, devId);

        UpdateTechDisplay();
        InitResCheckItem();
        CreateHeroHeadItems();
    }

    private void OnDestTechButtonClick()
    {
        PanelManager.Instance.ShowTechForSelect((techId) =>
        {
            selectedTechId = techId;
            UpdateTechDisplay();
            CreateHeroHeadItems();
        });
    }

    private void UpdateTechDisplay()
    {
        if (selectedTechId == 0)
        {
            if (techNameText != null)
                techNameText.text = "请选择科技";
            if (techProgressText != null)
                techProgressText.gameObject.SetActive(false);
            return;
        }

        var techCfg = TechConfig.GetConfig(selectedTechId);
        var force = GameManager.Instance.GetForce(forceId);
        int progress = force != null ? force.GetTechProgress(selectedTechId) : 0;

        if (techNameText != null)
            techNameText.text = techCfg.Cname;
        if (techProgressText != null)
        {
            techProgressText.gameObject.SetActive(true);
            techProgressText.text = $"研究值：{progress}/{techCfg.SciPointCost}";
        }
    }

    private void InitResCheckItem()
    {
        if (resCheckItemGold == null) return;

        resCheckItemGold.Init("scipoint");
        RefreshScipointDisplay();
    }

    private void RefreshScipointDisplay()
    {
        if (resCheckItemGold == null) return;

        var force = GameManager.Instance.GetForce(forceId);
        int scipoint = force != null ? (int)force.scipoint : 0;
        int cost = SystemConst.CityDev.TECH_RESEARCH_SCIPOINT_COST;

        if (cost > scipoint)
        {
            resCheckItemGold.UpdateDisplay($"<color=red>{cost}</color>/{scipoint}");
        }
        else
        {
            resCheckItemGold.UpdateDisplay($"{cost}/{scipoint}");
        }
    }

    private bool CanSelectHero()
    {
        var force = GameManager.Instance.GetForce(forceId);
        int usedCount = force != null ? force.GetKingActionCount(devId) : 0;
        return usedCount + GetSelectedCount() < techHeroCount;
    }

    private void OnHeroSelectionChanged()
    {
        RefreshScipointDisplay();
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

    private void OnResearch()
    {
        if (selectedTechId == 0)
        {
            SystemTip.Instance.ShowTip("请先选择要研究的科技");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择执行武将");
            return;
        }

        int[] heroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var devCfg = CityDevConfig.GetConfig(devId);
        var force = GameManager.Instance.GetForce(forceId);

        // 研究值消耗检查
        int scipointCost = SystemConst.CityDev.TECH_RESEARCH_SCIPOINT_COST;
        if (force != null && force.scipoint < scipointCost)
        {
            SystemTip.Instance.ShowTip("研究值不足");
            return;
        }

        bool success = force.ExecuteCityTech(cityId, devId, heroIds, selectedTechId, out var attrDatas);
        if (!success) return;

        RefreshScipointDisplay();

        PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
        {
            PanelManager.Instance.HideCityTech();
        }, CityDevKingActionConfig.GetConfig(devId).Mp4, false);
    }

    private void CreateHeroHeadItems()
    {
        // 保存当前选中状态
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

        var heroList = new List<int>();
        foreach (var city in force.GetCityList())
        {
            heroList.AddRange(city.GetNormalHeroList());
        }
        if (heroList.Count == 0) return;

        int currentRound = GameManager.Instance.SaveData.round;
        // 已行动武将排到末尾，按智力降序
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
                int heroId = heroList[i];
                string attText = GetHeroAttText(heroId);
                var heroData = GameManager.Instance.GetHero(heroId);
                bool hasActed = heroData != null && heroData.round >= currentRound;
                itemScript.Init(heroId, attText, forceId, hasActed);
                itemScript.SetCallbacks(CanSelectHero, OnHeroSelectionChanged);

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
        return $"智{SysColor.GetColoredText("inte", hero.inte)}";
    }
}
