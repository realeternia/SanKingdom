using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;


public class HeroInfoPanelManager : MonoBehaviour
{
    public int heroId;
    public TMP_Text heroNameText;
    public TMP_Text ageText;
    public TMP_Text cityText;
    public TMP_Text stateText;
    public TMP_Text lvText;
    public TMP_Text leaderText;
    public TMP_Text loyalText;

    public Image heroImage;

    public ScrollRect scrollRectNames;
    public GameObject rankRegionNames;
    public GameObject heroInfoCellPrefab;

    public Button closeBtn;

    private HeroInfoCell lastSelectedMode; // 上次选中的模式单元格
    private List<HeroInfoCell> heroInfoCells = new List<HeroInfoCell>();

    private void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideHeroInfoPanel();
        });
    }

    public void Init(int[] heroList, int targetHeroId)
    {
        foreach (Transform child in rankRegionNames.transform)
        {
            Destroy(child.gameObject);
        }
        heroInfoCells.Clear();
        lastSelectedMode = null;

        HeroInfoCell targetCell = null;
        foreach (var hId in heroList)
        {
            GameObject cell = Instantiate(heroInfoCellPrefab, rankRegionNames.transform);
            cell.transform.localScale = Vector3.one;
            HeroInfoCell cellInfo = cell.GetComponent<HeroInfoCell>();
            cellInfo.heroInfoPanelManager = this;
            var heroConfig = HeroConfig.GetConfig(hId);
            cellInfo.Init(hId, heroConfig.Name);
            heroInfoCells.Add(cellInfo);

            if (hId == targetHeroId)
            {
                targetCell = cellInfo;
            }
        }

        RectTransform rankParentRect = rankRegionNames.GetComponent<RectTransform>();
        RectTransform cellRect = heroInfoCellPrefab.GetComponent<RectTransform>();
        if (rankParentRect != null && cellRect != null)
        {
            rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * heroList.Length);
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
        
        heroNameText.text = heroConfig.Name;
        string imgPath = "SkinsBig/" + heroConfig.Icon;
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
        
    }

    public void OnHide()
    {
    }

}

