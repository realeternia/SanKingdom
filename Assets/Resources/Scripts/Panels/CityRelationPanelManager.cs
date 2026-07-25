using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;

public class CityRelationPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    public ResCheckItem resCheckItemGold;

    public Button closeButton;
    public Button okButton;

    public NLMutiCheckButton checkBtn; // 两种模式：亲善(0)和挑拨(1)

    public Button force1Button;
    public TMP_Text textForce1Name;

    public Button force2Button;
    public TMP_Text textForce2Name;

    public GameObject force2Obj; // 控制显影：亲善不需要force2，挑拨需要

    private int forceId;
    private int cityId;
    private int devId;
    private int selectedForceId1 = 0;
    private int selectedForceId2 = 0;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityRelation();
        });

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnConfirm);
        }

        force1Button.onClick.AddListener(OnForce1ButtonClick);
        force2Button.onClick.AddListener(OnForce2ButtonClick);

        checkBtn.Init(new string[] { "亲善", "挑拨" });
        checkBtn.SelectIndexChange = OnModeChanged;
    }

    public void Init(int forceId, int sourceCityId, int devId)
    {
        this.forceId = forceId;
        this.cityId = sourceCityId;
        this.devId = devId;
        this.selectedForceId1 = 0;
        this.selectedForceId2 = 0;

        resCheckItemGold.Init("gold");

        // 默认亲善模式
        checkBtn.SetSelectedIndexExternal(0);
        force2Obj.SetActive(false);

        RefreshForceDisplay();
        CreateHeroHeadItems();
        RefreshGoldDisplay();
    }

    private void OnModeChanged(int index)
    {
        selectedForceId1 = 0;
        selectedForceId2 = 0;
        force2Obj.SetActive(index == 1);
        RefreshForceDisplay();
    }

    private bool IsBefriendMode()
    {
        return checkBtn.GetSelectedIndex() == 0;
    }

    private int GetCurrentDevId()
    {
        return IsBefriendMode() ? CityDevConfig.GetConfigByName("Diplomacy").Id : CityDevConfig.GetConfigByName("SowDiscord").Id;
    }

    private void OnForce1ButtonClick()
    {
        SideForceSelector.SetContext(forceId, (selectedForceIds) =>
        {
            if (selectedForceIds.Count > 0)
            {
                selectedForceId1 = selectedForceIds[0];
                RefreshForceDisplay();
            }
        });
        PanelManager.Instance.ShowSideBar("SideForceSelector");
    }

    private void OnForce2ButtonClick()
    {
        if (selectedForceId1 == 0)
        {
            SystemTip.Instance.ShowTip("请先选择第一个势力");
            return;
        }

        // 挑拨模式下，force2的srcForceId是force1，同时排除玩家势力
        SideForceSelector.SetContext(selectedForceId1, (selectedForceIds) =>
        {
            if (selectedForceIds.Count > 0)
            {
                selectedForceId2 = selectedForceIds[0];
                RefreshForceDisplay();
            }
        }, new List<int> { forceId });
        PanelManager.Instance.ShowSideBar("SideForceSelector");
    }

    private void RefreshForceDisplay()
    {
        if (selectedForceId1 > 0)
        {
            var forceCfg1 = ForceConfig.GetConfig(selectedForceId1);
            Color forceColor1 = SysColor.GetForceColor(selectedForceId1);
            string hex1 = ColorUtility.ToHtmlStringRGB(forceColor1);
            textForce1Name.text = $"<color=#{hex1}>{forceCfg1.Cname}</color>";
        }
        else
        {
            textForce1Name.text = "点击选择";
        }

        if (selectedForceId2 > 0)
        {
            var forceCfg2 = ForceConfig.GetConfig(selectedForceId2);
            Color forceColor2 = SysColor.GetForceColor(selectedForceId2);
            string hex2 = ColorUtility.ToHtmlStringRGB(forceColor2);
            textForce2Name.text = $"<color=#{hex2}>{forceCfg2.Cname}</color>";
        }
        else
        {
            textForce2Name.text = IsBefriendMode() ? "" : "点击选择";
        }

        CreateHeroHeadItems();
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
            int currentDevId = GetCurrentDevId();
            var devCfg = CityDevConfig.GetConfig(currentDevId);
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

    private void OnConfirm()
    {
        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择执行武将");
            return;
        }

        if (selectedForceId1 == 0)
        {
            SystemTip.Instance.ShowTip("请选择目标势力");
            return;
        }

        int[] heroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null)
        {
            GameLog.Error($"CityRelationPanelManager.OnConfirm force not found forceId={forceId}");
            return;
        }

        int currentDevId = GetCurrentDevId();
        bool success;

        if (IsBefriendMode())
        {
            success = force.ExecuteCityBefriend(cityId, currentDevId, heroIds, selectedForceId1, out var attrDatas);
            if (!success) return;

            var devCfg = CityDevConfig.GetConfig(currentDevId);
            PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
            {
                RefreshGoldDisplay();
                CreateHeroHeadItems();
            }, CityDevKingActionConfig.GetConfig(currentDevId).Mp4, false);
        }
        else
        {
            if (selectedForceId2 == 0)
            {
                SystemTip.Instance.ShowTip("请选择第二个目标势力");
                return;
            }

            success = force.ExecuteCitySowDiscord(cityId, currentDevId, heroIds, selectedForceId1, selectedForceId2, out var attrDatas);
            if (!success) return;

            var devCfg = CityDevConfig.GetConfig(currentDevId);
            PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
            {
                RefreshGoldDisplay();
                CreateHeroHeadItems();
            }, CityDevKingActionConfig.GetConfig(currentDevId).Mp4, false);
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

        var heroList = new List<int>();
        foreach (var city in force.GetCityList())
        {
            heroList.AddRange(city.GetNormalHeroList());
        }
        if (heroList.Count == 0) return;

        int currentRound = GameManager.Instance.SaveData.round;
        bool hasTarget = selectedForceId1 > 0;
        heroList = heroList.OrderBy(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null && hero.round >= currentRound ? 1 : 0;
            })
            .ThenByDescending(h => hasTarget ? CalculateKingActionRate(h) : 0)
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
                string attText = hasTarget ? CalculateKingActionRate(heroList[i]) + "%" : GetHeroAttText(heroList[i]);
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

    private int CalculateKingActionRate(int executorHeroId)
    {
        if (selectedForceId1 <= 0) return 0;
        return SysFormula.Hero.CalcKingActionBonus(executorHeroId, selectedForceId1, GetCurrentDevId(), null);
    }
}
