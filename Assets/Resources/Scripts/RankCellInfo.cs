using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;


public class RankCellInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RankPanelManager rankPanelManager;

    public Image heroPic;
    public Image[] heroSkill;
    public TMP_Text heroName;
    public TMP_Text heroStr;
    public TMP_Text heroInte;
    public TMP_Text heroLeadShip;
    public TMP_Text heroFair;
    public TMP_Text heroCharm;
    public TMP_Text ownerName;

    public int heroId;
    public int str;
    public int inte;
    public int leadShip;
    public int fair;
    public int charm;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < heroSkill.Length; i++)
            heroSkill[i].raycastTarget = false;
        heroName.raycastTarget = false;
        heroStr.raycastTarget = false;
        heroInte.raycastTarget = false;
        heroLeadShip.raycastTarget = false;
        heroFair.raycastTarget = false;
        heroCharm.raycastTarget = false;

    }

    public void Init(HeroConfig heroConfig)
    {
        // 设置英雄信息
        heroPic.sprite = Resources.Load<Sprite>("Skins/" + heroConfig.Icon);

        for (int i = 0; i < heroSkill.Length; i++)
        {
            if (i < heroConfig.Skills.Length)
            {
                var skillIcon = SkillConfig.GetConfig(heroConfig.Skills[i]).Icon;
                heroSkill[i].sprite = Resources.Load<Sprite>("SkillPic/" + skillIcon);
            }
            else
            {
                heroSkill[i].sprite = null;
                if(i > 0)
                    heroSkill[i].gameObject.SetActive(false);// 第一个永远显示
            }
        }

        heroName.text = heroConfig.Name;
        heroId = heroConfig.Id;
        str = heroConfig.Str;
        inte = heroConfig.Inte;
        leadShip = heroConfig.LeadShip;
        fair = heroConfig.Fair;
        charm = heroConfig.Charm;

        var bg = GetComponent<Image>();
        bg.color = HeroSelectionTool.GetForceColor(heroConfig.ForceId);

        heroStr.text = heroConfig.Str.ToString();
        if (heroConfig.Str >= 95)
            heroStr.text = "<color=red>" + heroConfig.Str.ToString() + "</color>";
        else if (heroConfig.Str >= 90)
            heroStr.text = "<color=yellow>" + heroConfig.Str.ToString() + "</color>";

        heroInte.text = heroConfig.Inte.ToString();
        if (heroConfig.Inte >= 95)
            heroInte.text = "<color=red>" + heroConfig.Inte.ToString() + "</color>";
        else if (heroConfig.Inte >= 90)
            heroInte.text = "<color=yellow>" + heroConfig.Inte.ToString() + "</color>";

        heroLeadShip.text = heroConfig.LeadShip.ToString();
        if (heroConfig.LeadShip >= 95)
            heroLeadShip.text = "<color=red>" + heroConfig.LeadShip.ToString() + "</color>";
        else if (heroConfig.LeadShip >= 90)
            heroLeadShip.text = "<color=yellow>" + heroConfig.LeadShip.ToString() + "</color>";

        heroFair.text = heroConfig.Fair.ToString();
        if (heroConfig.Fair >= 95)
            heroFair.text = "<color=red>" + heroConfig.Fair.ToString() + "</color>";
        else if (heroConfig.Fair >= 90)
            heroFair.text = "<color=yellow>" + heroConfig.Fair.ToString() + "</color>";

        heroCharm.text = heroConfig.Charm.ToString();
        if (heroConfig.Charm >= 95)
            heroCharm.text = "<color=red>" + heroConfig.Charm.ToString() + "</color>";
        else if (heroConfig.Charm >= 90)
            heroCharm.text = "<color=yellow>" + heroConfig.Charm.ToString() + "</color>";      
        heroPic.gameObject.SetActive(false);

        var cityId = GameManager.Instance.GetHeroCity(heroId, out int forceId);
        Debug.Log($"GetHeroCity {heroId} {cityId} {forceId}");
        if (cityId >= 0)
        {
            var cityCfg = WorldConfig.GetConfig(cityId);
            ownerName.text = "<color=yellow>" + cityCfg.Cname + "</color>-<color=green>" + ForceConfig.GetConfig(forceId).Cname + "</color>";
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
        Debug.Log($"UI 元素被按下，位置：{eventData.position}");

        // 判断点击是否在heroSkill区域内
        bool isClickOnHeroSkill = false;
        for(int i = 0; i < heroSkill.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
            heroSkill[i].rectTransform, 
            eventData.position, 
            eventData.pressEventCamera))
            {
                isClickOnHeroSkill = true;
                break;
            }
        }

        if (!isClickOnHeroSkill)
        {
            rankPanelManager.OnSelectHero(this);
        }
        else
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg.Skills != null && heroCfg.Skills.Length > 0)
            {
                Tooltip.Instance.ShowTooltip(heroCfg.Skills, heroId);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
