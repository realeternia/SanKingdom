using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using Controls.Utils;

public class PopHeroBattleSelectPanelCell : MonoBehaviour, IPointerClickHandler
{
    public int heroId;
    public bool isSelect;
    public int attr1Val;
    private bool isAvailable; // 标记英雄是否可点击

    public PopHeroBattleSelectPanelManager popHeroSelectPanelManager;
    public TMP_Text heroName;
    public TMP_Text textAttrLead;
    public TMP_Text textAttrStr;
    public TMP_Text textAttrIntl;
    public TMP_Text textAttrSoldier;
    public Image backgroundImage;
    public Image checkImage;

    public Button changeButton;

    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 正常状态背景色
    private Color selectedColor = new Color(0.5f, 0.5f, 0.1f, 0.8f); // 高光绿色选中状态
    private Color disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f); // 灰色不可用状态

    // Start is called before the first frame update
    void Start()
    {
        heroName.raycastTarget = false;
        textAttrLead.raycastTarget = false;
        textAttrStr.raycastTarget = false;
        textAttrIntl.raycastTarget = false;
        textAttrSoldier.raycastTarget = false;
        checkImage.raycastTarget = false;

        changeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowPopArmySetPanel(heroId);
        });
        
        // 初始化背景色为正常状态
      //  OnSelect(false);
    }

    public void Init(SaveHeroData heroData)
    {
        this.heroId = heroData.heroId;
        var heroCfg = HeroConfig.GetConfig(heroId);

        heroName.text = heroCfg.Name;
        
        attr1Val = heroData.GetAttr("leadShip");
        var attLeadVal = heroData.GetAttr("leadShip");
        textAttrLead.text = attLeadVal.ToString();
        textAttrLead.color = attLeadVal >= 95 ? Color.red : (attLeadVal >= 90 ? new Color(0.8f, 0.5f, 0, 1) : Color.white);
        var attStrVal = heroData.GetAttr("str");
        textAttrStr.text = attStrVal.ToString();
        textAttrStr.color = attStrVal >= 95 ? Color.red : (attStrVal >= 90 ? new Color(0.8f, 0.5f, 0, 1) : Color.white);
        var attIntlVal = heroData.GetAttr("inte");
        textAttrIntl.text = attIntlVal.ToString();
        textAttrIntl.color = attIntlVal >= 95 ? Color.red : (attIntlVal >= 90 ? new Color(0.8f, 0.5f, 0, 1) : Color.white);
        textAttrSoldier.text = CityBattlePanelManager.GetAllocatedSoldier(heroData.heroId).ToString();
        textAttrSoldier.color = CityBattlePanelManager.GetAllocatedSoldier(heroData.heroId) == 0 ? Color.gray : Color.white;

        isAvailable = true;
    }

    public void UpdateAttr()
    {
        var heroData = GameManager.Instance.GetHero(heroId);
        textAttrSoldier.text = CityBattlePanelManager.GetAllocatedSoldier(heroId).ToString();
        textAttrSoldier.color = CityBattlePanelManager.GetAllocatedSoldier(heroId) == 0 ? Color.gray : Color.white;
    }

    public void OnSelect(bool isSelect)
    {
        GameLog.Debug($"OnSelect {heroId} {isSelect}");
        this.isSelect = isSelect;
        checkImage.gameObject.SetActive(isSelect);
        if (backgroundImage != null)
        {
            // 只有可用的英雄才能改变选中状态的背景色
            if (isAvailable)
            {
                backgroundImage.color = isSelect ? selectedColor : normalColor;
            }
            else
            {
                backgroundImage.color = disabledColor;
            }
        }
    }
    
    // 处理点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        GameLog.Debug($"OnPointerClick {heroId} {isSelect}");
        // 只有可用的英雄才能被点击
        if (isAvailable)
        {
            // 通知面板管理器当前单元格被点击
            popHeroSelectPanelManager.OnSelectItem(this, !isSelect);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
