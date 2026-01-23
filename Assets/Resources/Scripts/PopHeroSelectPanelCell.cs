using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class PopHeroSelectPanelCell : MonoBehaviour, IPointerClickHandler
{
    public int heroId;
    public int heroYear;    
    public bool isSelect;
    public int attr1Val;
    private bool isAvailable; // 标记英雄是否可点击

    public PopHeroSelectPanelManager popHeroSelectPanelManager;
    public TMP_Text heroName;
    public TMP_Text textAttr1;
    public TMP_Text textAttr2;
    public Image backgroundImage;
    public Image checkImage;

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
        
        // 初始化背景色为正常状态
      //  OnSelect(false);
    }

    public void Init(SaveHeroData heroData, string[] attrs)
    {
        this.heroId = heroData.heroId;
        heroYear = heroData.round;
        var heroCfg = HeroConfig.GetConfig(heroId);
        var currentYear = GameManager.Instance.SaveData.round;

        heroName.text = heroCfg.Name;
        
        var att1Val = heroData.GetAttr(attrs[0]);
        this.attr1Val = att1Val;
        textAttr1.text = att1Val.ToString();
        textAttr1.color = att1Val >= 95 ? Color.red : (att1Val >= 90 ? new Color(0.8f, 0.5f, 0, 1) : Color.white);
        if(attrs.Length > 1)
        {
            var att2Val = heroData.GetAttr(attrs[1]);
            textAttr2.text = att2Val.ToString();
            textAttr2.color = att2Val >= 95 ? Color.red : (att2Val >= 90 ? new Color(0.8f, 0.5f, 0, 1) : Color.white);
        }
        else
            textAttr2.text = "";

        // 检查英雄是否已经在当前年份执行过任务
        isAvailable = heroData.round != currentYear;
        
        // 根据英雄是否可用设置不同的文字颜色
        if(!isAvailable)
        {
            heroName.color = Color.gray;
            textAttr1.color = Color.gray;
            textAttr2.color = Color.gray;
        }

    }

    public void OnSelect(bool isSelect)
    {
        Debug.Log($"OnSelect {heroId} {isSelect}");
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
