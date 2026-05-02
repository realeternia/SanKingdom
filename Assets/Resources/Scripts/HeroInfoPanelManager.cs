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
    public TMP_Dropdown typeDropdown;

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
    public TMP_Text armsAttrText;
    public Button armsChangeBtn;
    
    private List<ArmsItemControl> armsItems = new List<ArmsItemControl>();

    public Button closeBtn;

    private HeroInfoCell lastSelectedMode; // 上次选中的模式单元格
    private List<HeroInfoCell> heroInfoCells = new List<HeroInfoCell>();
    public AttrRadarChart attrRadarChart;

    private int[] currentHeroList;
    private int currentTargetHeroId;

    private static readonly string[] SortOptions = { "所在", "统帅", "武力", "智力", "内政", "魅力" };
    private static readonly string[] SortAttrKeys = { "City", "LeadShip", "Str", "Inte", "Fair", "Charm" };

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideHeroInfoPanel();
        });
        
        InitTypeDropdown();
    }
    
    private void InitTypeDropdown()
    {
        typeDropdown.ClearOptions();
        typeDropdown.AddOptions(SortOptions.ToList());
        typeDropdown.value = 0;
        typeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
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
    
    private int GetAttrValue(SaveHeroData heroData, string attrKey)
    {
        if (heroData == null)
            return 0;
        
        switch (attrKey)
        {
            case "LeadShip": return heroData.leadShip;
            case "Str": return heroData.str;
            case "Inte": return heroData.inte;
            case "Fair": return heroData.fair;
            case "Charm": return heroData.charm;
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
        else
        {
            if (heroData != null) heroData.InitAttrsFromConfig();
            int attrValue = GetAttrValue(heroData, sortKey);
            string coloredValue = GetColoredAttrValue(sortKey, attrValue);
            return $"{heroName} {coloredValue}";
        }
    }
    
    private string GetColoredAttrValue(string attrName, int value)
    {
        return HeroAttrTool.GetColoredText(attrName, value);
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
        UpdateHeroInfo(heroId);
    }

    private void UpdateHeroInfo(int hId)
    {
        var heroConfig = HeroConfig.GetConfig(hId);
        var heroData = GameManager.Instance.GetHero(hId);
        if (heroData != null) heroData.InitAttrsFromConfig();
        
        heroNameText.text = heroConfig.Name;
        string imgPath = "Textures/SkinsBig/" + heroConfig.Icon;
        Sprite sprite = Resources.Load<Sprite>(imgPath);
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
            loyalText.text = heroData.loyalty.ToString();
        }
        else
        {
            stateText.text = "在野";
            lvText.text = "1";
            loyalText.text = "0";
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
                control.Init(attrConfig, heroConfig);
            }
            armsItems.Add(control);
        }
    }

    public void OnHide()
    {
    }

}

