using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using Controls.Utils;


public class RankCellInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IRankDetailInfo, IRankDetailInfoHeader
{
    public RankPanelManager rankPanelManager;

    public Image heroPic;
    public GameObject heroPicContainer;
    public Button viewButton;
    public TMP_Text heroName;
    public TMP_Text heroStr;
    public TMP_Text heroInte;
    public TMP_Text heroLeadShip;
    public TMP_Text heroFair;
    public TMP_Text heroCharm;
    public TMP_Text ownerName;

    public Button btnLeadShip;
    public Button btnStr;
    public Button btnInte;
    public Button btnFair;
    public Button btnCharm;    

    public GameObject nodeHeader;
    public GameObject nodeRow;

    public int heroId;
    public int str;
    public int inte;
    public int leadShip;
    public int fair;
    public int charm;

    // Start is called before the first frame update
    void Start()
    {
        heroName.raycastTarget = false;
        heroStr.raycastTarget = false;
        heroInte.raycastTarget = false;
        heroLeadShip.raycastTarget = false;
        heroFair.raycastTarget = false;
        heroCharm.raycastTarget = false;

        viewButton.onClick.AddListener(() =>
        {
            GameLog.Info($"RankCellInfo.viewButton: heroId={heroId}, mHeroList={string.Join(",", rankPanelManager.mHeroList ?? new int[0])}");
            PanelManager.Instance.ShowHeroInfoPanel(rankPanelManager.mHeroList, heroId);
        });
    }

    public void SetManager(RankPanelManager rankPanelManager)
    {
        this.rankPanelManager = rankPanelManager;
    }

    public void SetMode(bool isHeader)
    {
        if(isHeader)
        {
            viewButton.gameObject.SetActive(false);
            nodeHeader.SetActive(true);
            nodeRow.SetActive(false);
            btnLeadShip.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("LeadShip");
            });
            btnStr.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Str");
            });
            btnInte.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Inte");
            });
            btnFair.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Fair");
            });
            btnCharm.onClick.AddListener(() =>
            {
                rankPanelManager.SortItems("Charm");
            });
        }
        else
        {
            nodeHeader.SetActive(false);
            nodeRow.SetActive(true);
        }
    }

    public int GetValInt(string key)
    {
        switch (key)
        {
            case "Str":
                return str;
            case "Inte":
                return inte;
            case "LeadShip":
                return leadShip;
            case "Fair":
                return fair;
            case "Charm":
                return charm;
            default:
                return 0;
        }
    }

    public void Init(SaveHeroData heroData)
    {
        heroData.InitAttrsFromConfig();
        var heroConfig = HeroConfig.GetConfig(heroData.heroId);
        heroPic.sprite = Resources.Load<Sprite>(ResPath.Texture.HeroIcon(heroConfig.Icon));
        heroPicContainer = heroPic.gameObject;

        heroName.text = heroConfig.Name;
        heroId = heroData.heroId;
        str = heroData.str;
        inte = heroData.inte;
        leadShip = heroData.leadShip;
        fair = heroData.fair;
        charm = heroData.charm;

        var bg = GetComponent<Image>();
        var forceCfg = ForceConfig.GetConfig(heroData.forceId);
        bg.color = ColorUtility.TryParseHtmlString(forceCfg.Color, out var wColor) ? wColor : Color.white;

        heroStr.text = HeroAttrTool.GetColoredText("str", heroData.str);
        heroInte.text = HeroAttrTool.GetColoredText("inte", heroData.inte);
        heroLeadShip.text = HeroAttrTool.GetColoredText("leadShip", heroData.leadShip);
        heroFair.text = HeroAttrTool.GetColoredText("fair", heroData.fair);
        heroCharm.text = HeroAttrTool.GetColoredText("charm", heroData.charm);      
        heroPic.gameObject.SetActive(false);

        if (heroData.cityId > 0)
        {
            var cityData = GameManager.Instance.GetCity(heroData.cityId);
            var cityCfg = WorldConfig.GetConfig(heroData.cityId);
            ownerName.text = "<color=yellow>" + cityCfg.Cname + "</color>-<color=green>" + ForceConfig.GetConfig(cityData.forceId).Cname + "</color>";
        }
        else
        {
            ownerName.text = "";
        }

        
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (Tooltip.Instance != null)
        {
            Tooltip.Instance.HideTooltip();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameLog.Debug($"UI 元素被按下，位置：{eventData.position}");

        // 判断点击是否在heroSkill区域内
        rankPanelManager.OnSelectHero(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnSelectHero(bool isSelected)
    {
        if (heroPicContainer != null)
        {
            if (isSelected)
            {
                // 选中英雄
                heroPicContainer.SetActive(true);
            }
            else
            {
                // 取消选中英雄
                heroPicContainer.SetActive(false);
            }
        }
    }
}
