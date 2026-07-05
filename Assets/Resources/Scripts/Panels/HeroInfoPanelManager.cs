using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
public class HeroInfoPanelManager : MonoBehaviour
{
    public int heroId;
    public TMP_Text heroNameText;
    public NLDropDown typeDropdown;

    public TMP_Text ageText;
    public TMP_Text cityText;
    public TMP_Text stateText;
    public TMP_Text lvText;
    public TMP_Text leaderText;
    public TMP_Text loyalText;

    public TMP_Text gexingText;
    public TMP_Text pinzhiText;
    public TMP_Text aihaoText;
    public TMP_Text paixiText;
    public TMP_Text likesText;
    public TMP_Text hatesText;
    public TMP_Text storyText;

    public Image heroImage;

    public ScrollRect scrollRectNames;
    public GameObject rankRegionNames;
    public GameObject heroInfoCellPrefab;

    public GameObject armsPanel;
    public GameObject armsItemPrefab;

    public TMP_Text armsNameText;
    public TMP_Text armsAttr1Text;
    public TMP_Text armsAttr2Text;
    public TMP_Text troopsText;
    
    private List<ArmsItemControl> armsItems = new List<ArmsItemControl>();

    public Button buttonBasePage;
    public Button buttonEventPage;
    public TMP_Text eventDescText;

    public GameObject panelBase;
    public GameObject panelEvent;
    public GameObject panelArms;

    public Button closeBtn;

    private HeroInfoCell lastSelectedMode; // 上次选中的模式单元格
    private List<HeroInfoCell> heroInfoCells = new List<HeroInfoCell>();
    public AttrRadarChart attrRadarChart;

    private int[] currentHeroList;
    private int currentTargetHeroId;

    public ScrollRect scrollRectEvent;
    public GameObject rankRegionEvent;

    private LoopScrollRect loopScrollEvent;

    private static readonly string[] SortOptions = { "所在", "兵种", "统帅", "武力", "智力", "内政", "魅力" };
    private static readonly string[] SortAttrKeys = { "City", "Arms", "leadShip", "str", "inte", "fair", "charm" };

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideHeroInfoPanel();
        });

        buttonBasePage.onClick.AddListener(() => ShowPage(true));
        buttonEventPage.onClick.AddListener(() => ShowPage(false));

        InitTypeDropdown();

        if (scrollRectEvent != null)
        {
            loopScrollEvent = new LoopScrollRect(scrollRectEvent);
        }

        ShowPage(true);
    }

    private void ShowPage(bool isBasePage)
    {
        if (panelBase != null) panelBase.SetActive(isBasePage);
        if (panelArms != null) panelArms.SetActive(isBasePage);
        if (panelEvent != null) panelEvent.SetActive(!isBasePage);

        if (!isBasePage && currentTargetHeroId > 0)
        {
            UpdateEventList(currentTargetHeroId);
        }
    }
    
    private void InitTypeDropdown()
    {
        typeDropdown.ClearOptions();
        typeDropdown.AddOptions(SortOptions.ToList());
        typeDropdown.value = 0;
        typeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        if (typeDropdown.template != null)
        {
            var scrollbars = typeDropdown.template.GetComponentsInChildren<Scrollbar>(true);
            foreach (var scrollbar in scrollbars)
            {
                scrollbar.gameObject.SetActive(false);
            }
        }
    }
    
    private void OnDropdownValueChanged(int index)
    {
        if (currentHeroList == null || currentHeroList.Length == 0)
            return;
        
        RefreshHeroList(SortAttrKeys[index]);
    }
    
    private void RefreshHeroList(string sortKey)
    {
        foreach (Transform child in rankRegionNames.transform)
        {
            Destroy(child.gameObject);
        }
        heroInfoCells.Clear();
        lastSelectedMode = null;

        var sortedHeroIds = SortHeroes(currentHeroList, sortKey);

        HeroInfoCell targetCell = null;
        foreach (var hId in sortedHeroIds)
        {
            GameObject cell = Instantiate(heroInfoCellPrefab, rankRegionNames.transform);
            cell.transform.localScale = Vector3.one;
            HeroInfoCell cellInfo = cell.GetComponent<HeroInfoCell>();
            cellInfo.heroInfoPanelManager = this;
            var heroConfig = HeroConfig.GetConfig(hId);
            var heroData = GameManager.Instance.GetHero(hId);
            
            string displayText = GetDisplayText(hId, sortKey, heroData);
            cellInfo.Init(hId, heroConfig.Name, displayText);
            heroInfoCells.Add(cellInfo);

            if (hId == currentTargetHeroId)
            {
                targetCell = cellInfo;
                GameLog.Info($"RefreshHeroList: targetCell={targetCell != null}");
            }
        }

        RectTransform rankParentRect = rankRegionNames.GetComponent<RectTransform>();
        RectTransform cellRect = heroInfoCellPrefab.GetComponent<RectTransform>();
        if (rankParentRect != null && cellRect != null)
        {
            rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * sortedHeroIds.Length);
        }

        if (scrollRectNames != null)
        {
            scrollRectNames.normalizedPosition = new Vector2(0, 1);
        }

        if (targetCell != null)
        {
            OnSelectHero(targetCell);
        }
    }
    
    private int[] SortHeroes(int[] heroIds, string sortKey)
    {
        var heroList = heroIds.ToList();
        
        if (sortKey == "City")
        {
            heroList.Sort((a, b) =>
            {
                var heroDataA = GameManager.Instance.GetHero(a);
                var heroDataB = GameManager.Instance.GetHero(b);
                int cityIdA = heroDataA != null ? heroDataA.cityId : 0;
                int cityIdB = heroDataB != null ? heroDataB.cityId : 0;
                return cityIdB.CompareTo(cityIdA);
            });
        }
        else if (sortKey == "Arms")
        {
            heroList.Sort((a, b) =>
            {
                int levelA = GetArmsLevel(a);
                int levelB = GetArmsLevel(b);
                return levelB.CompareTo(levelA);
            });
        }
        else
        {
            heroList.Sort((a, b) =>
            {
                var heroDataA = GameManager.Instance.GetHero(a);
                var heroDataB = GameManager.Instance.GetHero(b);
                if (heroDataA != null) heroDataA.InitAttrsFromConfig();
                if (heroDataB != null) heroDataB.InitAttrsFromConfig();
                
                int valA = GetAttrValue(heroDataA, sortKey);
                int valB = GetAttrValue(heroDataB, sortKey);
                return valB.CompareTo(valA);
            });
        }
        
        return heroList.ToArray();
    }

    private int GetArmsLevel(int heroId)
    {
        var heroData = GameManager.Instance.GetHero(heroId);
        var troop = heroData.GetTroop();
        if (troop == null)
            return -1;
        var armsConfig = ArmsConfig.GetConfig(troop.armsId);
        return armsConfig.Level;
    }
    
    private int GetAttrValue(SaveHeroData heroData, string attrKey)
    {
        if (heroData == null)
            return 0;
        
        switch (attrKey)
        {
            case "leadShip": return heroData.leadShip;
            case "str": return heroData.str;
            case "inte": return heroData.inte;
            case "fair": return heroData.fair;
            case "charm": return heroData.charm;
            default: return 0;
        }
    }
    
    private string GetDisplayText(int heroId, string sortKey, SaveHeroData heroData)
    {
        var heroConfig = HeroConfig.GetConfig(heroId);
        string heroName = heroConfig.Name;
        
        if (sortKey == "City")
        {
            string cityName = "";
            if (heroData != null && heroData.cityId > 0)
            {
                var cityConfig = WorldConfig.GetConfig(heroData.cityId);
                cityName = cityConfig != null ? cityConfig.Cname : "";
            }
            return $"{heroName} <color=green>{cityName}</color>";
        }
        else if (sortKey == "Arms")
        {
            string armsName = "";
            var troop = heroData != null ? heroData.GetTroop() : null;
            if (troop != null)
            {
                var armsConfig = ArmsConfig.GetConfig(troop.armsId);
                Color color = GetColorByArmsLevel(armsConfig.Level);
                string colorHex = ColorUtility.ToHtmlStringRGB(color);
                armsName = $"<color=#{colorHex}>{armsConfig.NameS}</color>";
            }
            return $"{heroName} {armsName}";
        }
        else
        {
            if (heroData != null) heroData.InitAttrsFromConfig();
            int attrValue = GetAttrValue(heroData, sortKey);
            string coloredValue = GetColoredAttrValue(sortKey, attrValue);
            return $"{heroName} {coloredValue}";
        }
    }

    private Color GetColorByArmsLevel(int level)
    {
        return SysColor.GetArmsLevelColor(level);
    }
    
    private string GetColoredAttrValue(string attrName, int value)
    {
        return SysColor.GetColoredText(attrName, value);
    }

    public void Init(int[] heroList, int targetHeroId)
    {
        currentHeroList = heroList;
        currentTargetHeroId = targetHeroId;
        
        typeDropdown.value = 0;
        RefreshHeroList("City");
    }

    public void OnSelectHero(HeroInfoCell cellMode)
    {
        if (lastSelectedMode != null && lastSelectedMode != cellMode)
        {
            lastSelectedMode.SetSelected(false);
        }

        cellMode.SetSelected(true);

        lastSelectedMode = cellMode;

        heroId = cellMode.heroId;
        currentTargetHeroId = cellMode.heroId;

        ScrollToCell(cellMode);

        UpdateHeroInfo(heroId);
        UpdateEventList(heroId);
    }

    private void UpdateEventList(int targetHeroId)
    {
        if (loopScrollEvent == null)
        {
            GameLog.Warn("UpdateEventList: loopScrollEvent 为 null");
            return;
        }

        var eventLog = GameManager.Instance?.GameEventLog;
        if (eventLog == null)
        {
            GameLog.Warn("UpdateEventList: GameEventLog 为 null");
            if (eventDescText != null) eventDescText.text = "事件(0)";
            return;
        }

        var heroEvents = eventLog.events
            .Where(e => e.heroIds != null && e.heroIds.Contains(targetHeroId))
            .OrderBy(e => e.round)
            .ToList();

        if (eventDescText != null) eventDescText.text = "事件(" + heroEvents.Count + ")";

        List<object> dataSource;
        if (heroEvents.Count == 0)
        {
            dataSource = new List<object> { "<color=#888888>暂无事件</color>" };
        }
        else
        {
            dataSource = heroEvents.Cast<object>().ToList();
        }

        var heroEventCellPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelListItem("HeroEventCell"));
        float cellHeight = heroEventCellPrefab.GetComponent<RectTransform>().sizeDelta.y;
        loopScrollEvent.Initialize(dataSource, heroEventCellPrefab, cellHeight);

        if (scrollRectEvent != null)
        {
            scrollRectEvent.normalizedPosition = new Vector2(0, 1);
        }
    }

    private void ScrollToCell(HeroInfoCell cellMode)
    {
        if (scrollRectNames == null || cellMode == null)
        {
            GameLog.Warn($"ScrollToCell: scrollRectNames={scrollRectNames}, cellMode={cellMode}");
            return;
        }

        RectTransform contentRect = rankRegionNames.GetComponent<RectTransform>();
        RectTransform cellRect = cellMode.GetComponent<RectTransform>();
        
        if (contentRect == null || cellRect == null)
        {
            GameLog.Warn($"ScrollToCell: contentRect={contentRect}, cellRect={cellRect}");
            return;
        }

        float contentHeight = contentRect.sizeDelta.y;
        RectTransform viewportTransform = scrollRectNames.viewport;
        float viewportHeight = viewportTransform != null ? viewportTransform.rect.height : scrollRectNames.GetComponent<RectTransform>().rect.height;
        float cellY = -cellRect.anchoredPosition.y;
        float cellHeight = cellRect.sizeDelta.y;

        float targetY = cellY - viewportHeight / 2f + cellHeight / 2f;
        float maxScroll = contentHeight - viewportHeight;
        
        if (maxScroll <= 0)
        {
            scrollRectNames.normalizedPosition = new Vector2(0, 1);
            return;
        }

        float normalizedY = 1f - (targetY / maxScroll);
        normalizedY = Mathf.Clamp01(normalizedY);
        
        scrollRectNames.normalizedPosition = new Vector2(0, normalizedY);
    }

    private void UpdateHeroInfo(int hId)
    {
        var heroConfig = HeroConfig.GetConfig(hId);
        var heroData = GameManager.Instance.GetHero(hId);
        if (heroData != null) heroData.InitAttrsFromConfig();
        
        heroNameText.text = heroConfig.Name;
        string imgPath = ResPath.Texture.HeroBigIcon(heroConfig.Icon);
        Sprite sprite = ResourceCache.LoadSpriteUI(imgPath);
        heroImage.sprite = sprite;
        
        int age = (int)GameManager.Instance.GetCurrentYear() - heroConfig.BornYear;
        ageText.text = age.ToString();
        
        if (heroData != null && heroData.cityId > 0)
        {
            var cityConfig = WorldConfig.GetConfig(heroData.cityId);
            cityText.text = cityConfig != null ? cityConfig.Cname : "";
        }
        else
        {
            cityText.text = "";
        }
        
        if (heroData != null)
        {
            stateText.text = heroData.state == HeroState.Normal ? "正常" : 
                             heroData.state == HeroState.Wild ? "在野" : "俘虏";
            lvText.text = heroData.GetLevel().ToString();
            loyalText.text = SysColor.GetColoredText("loyalty", heroData.loyalty);
        }
        else
        {
            stateText.text = "在野";
            lvText.text = "1";
            loyalText.text = SysColor.GetColoredText("loyalty", 0);
        }
        
        leaderText.text = heroData.forceId == 0 ? "-" : ForceConfig.GetConfig(heroData.forceId).Cname;
        
        if (attrRadarChart != null)
        {
            attrRadarChart.SetAttrValues(
                heroData.leadShip,
                heroData.str,
                heroData.inte,
                heroData.fair,
                heroData.charm
            );
        }

        gexingText.text = string.IsNullOrEmpty(heroConfig.Xingge) ? "无" : heroConfig.Xingge;
        pinzhiText.text = heroConfig.Pinzhi != null && heroConfig.Pinzhi.Length > 0 
            ? string.Join(" ", heroConfig.Pinzhi) : "无";
        aihaoText.text = heroConfig.Aihao != null && heroConfig.Aihao.Length > 0 
            ? string.Join(" ", heroConfig.Aihao) : "无";
        paixiText.text = string.IsNullOrEmpty(heroConfig.Paixi) ? "无" : heroConfig.Paixi;
        likesText.text = heroConfig.Likes != null && heroConfig.Likes.Length > 0 
            ? string.Join(" ", heroConfig.Likes) : "无";
        hatesText.text = heroConfig.Hates != null && heroConfig.Hates.Length > 0 
            ? string.Join(" ", heroConfig.Hates) : "无";
        storyText.text = heroConfig.Story != null && heroConfig.Story.Length > 0 
            ? string.Join(" ", heroConfig.Story) : "无";

        UpdateArmsPanel(heroConfig);
    }

    private void UpdateArmsPanel(HeroConfig heroConfig)
    {
        foreach (var item in armsItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        armsItems.Clear();

        var armsAttrs = HeroAttrConfig.ConfigList.Where(c => c.IsArmsAttr).ToList();
        if (armsAttrs.Count == 0 || armsPanel == null || armsItemPrefab == null)
            return;

        var heroData = GameManager.Instance.GetHero(heroId);
        int currentArmsId = heroData != null ? heroData.GetArmsId() : 0;

        UpdateArmsInfo(currentArmsId, heroData);

        float itemWidth = 180f;
        float spacing = 10f;
        float startX = -(armsAttrs.Count - 1) * (itemWidth + spacing) / 2f;

        for (int i = 0; i < armsAttrs.Count; i++)
        {
            var attrConfig = armsAttrs[i];
            GameObject item = Instantiate(armsItemPrefab, armsPanel.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = new Vector3(startX + i * (itemWidth + spacing), 30, 0);

            ArmsItemControl control = item.GetComponent<ArmsItemControl>();
            if (control != null)
            {
                control.Init(attrConfig, heroConfig, currentArmsId);
            }
            armsItems.Add(control);
        }
    }

    private void UpdateArmsInfo(int armsId, SaveHeroData heroData)
    {
        var troop = heroData != null ? heroData.GetTroop() : null;

        if (troop == null)
        {
            armsNameText.gameObject.SetActive(false);
            troopsText.text = "无小队";
            troopsText.color = SysColor.Battle.DeadColor;
            return;
        }

        armsNameText.gameObject.SetActive(true);

        var mainHeroConfig = HeroConfig.GetConfig(troop.heroId1);
        troopsText.text = mainHeroConfig.Name + "队";
        troopsText.color = Color.white;

        var armsConfig = ArmsConfig.GetConfig(armsId);
        armsNameText.text = armsConfig.NameS;
        armsNameText.color = SysColor.GetArmsLevelColor(armsConfig.Level);
        
        var (atk, def) = SysFormula.Battle.CalculateCombatAttrForTroop(troop);
        armsAttr1Text.text = atk.ToString();
        armsAttr2Text.text = def.ToString();
    }

    private void RefreshArmsBG()
    {
        var heroData = GameManager.Instance.GetHero(heroId);
        int currentArmsId = heroData != null ? heroData.GetArmsId() : 0;

        UpdateArmsInfo(currentArmsId, heroData);

        var armsAttrs = HeroAttrConfig.ConfigList.Where(c => c.IsArmsAttr).ToList();
        for (int i = 0; i < armsItems.Count && i < armsAttrs.Count; i++)
        {
            if (armsItems[i] != null)
            {
                armsItems[i].UpdateBGColor(armsAttrs[i], currentArmsId);
            }
        }

        if (typeDropdown != null && typeDropdown.value == 1)
        {
            RefreshHeroList("Arms");
        }
    }

    public void OnHide()
    {
        if (loopScrollEvent != null && loopScrollEvent.IsInitialized)
        {
            loopScrollEvent.Clear();
        }
    }

}

