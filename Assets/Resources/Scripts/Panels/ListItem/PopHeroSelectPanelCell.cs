using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using Controls.Utils;

public class PopHeroSelectPanelCell : MonoBehaviour, IPointerClickHandler
{
    public int heroId;
    public bool isSelect;
    public int attr1Val;
    private bool isAvailable; // 标记英雄是否可点击

    public PopHeroSelectPanelManager popHeroSelectPanelManager;
    public TMP_Text heroName;
    public TMP_Text textAttr1;
    public TMP_Text textAttr2;
    public TMP_Text textLoyalty;
    public TMP_Text textState;
    public Image backgroundImage;
    public Image checkImage;

    public Button viewButton;

    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 正常状态背景色
    private Color selectedColor = new Color(0.5f, 0.5f, 0.1f, 0.8f); // 高光绿色选中状态
    private Color disabledColor = new Color(0.1f, 0.1f, 0.1f, 0.5f); // 灰色不可用状态

    // Start is called before the first frame update
    void Start()
    {
        heroName.raycastTarget = false;
        textAttr1.raycastTarget = false;
        textAttr2.raycastTarget = false;
        checkImage.raycastTarget = false;
        
        viewButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowHeroInfoPanel(popHeroSelectPanelManager.mHeroList, heroId);
        });
        
        // 初始化背景色为正常状态
      //  OnSelect(false);
    }

    public void Init(SaveHeroData heroData, string[] attrs, bool ignoreActionCheck = false)
    {
        this.heroId = heroData.heroId;
        var heroCfg = HeroConfig.GetConfig(heroId);

        heroName.text = heroCfg.Name;
        
        var att1Val = heroData.GetAttr(attrs[0]);
        this.attr1Val = att1Val;
        textAttr1.text = att1Val.ToString();
        textAttr1.color = HeroAttrTool.GetColorByValue(attrs[0], att1Val);
        if(attrs.Length > 1)
        {
            var att2Val = heroData.GetAttr(attrs[1]);
            textAttr2.text = att2Val.ToString();
            textAttr2.color = HeroAttrTool.GetColorByValue(attrs[1], att2Val);
        }
        else
            textAttr2.text = "";

        textLoyalty.text = heroData.loyalty.ToString();
        textLoyalty.color = HeroAttrTool.GetColorByValue("loyalty", heroData.loyalty);

        switch (heroData.state)
        {
            case HeroState.Normal:
                textState.text = "正常";
                textState.color = Color.white;
                break;
            case HeroState.Wild:
                textState.text = "在野";
                textState.color = Color.yellow;
                break;
            case HeroState.Catched:
                textState.text = "俘虏";
                textState.color = Color.red;
                break;
        }

        isAvailable = true;

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
        // 只有可用的英雄才能被点击
        if (isAvailable)
        {
            // 通知面板管理器当前单元格被点击
            popHeroSelectPanelManager.OnSelectItem(this, !isSelect);
        }
    }

    // Update is called once per update
    void Update()
    {
        
    }
}
