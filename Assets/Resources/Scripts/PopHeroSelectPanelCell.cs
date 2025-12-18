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
    public bool isSelect;

    public PopHeroSelectPanelManager popHeroSelectPanelManager;
    public TMP_Text heroName;
    public TMP_Text textAttr1;
    public TMP_Text textAttr2;
    public Image backgroundImage;
    public Image checkImage;

    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 正常状态背景色
    private Color selectedColor = new Color(0.5f, 0.5f, 0.1f, 0.8f); // 高光绿色选中状态

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

    public string GetAttr(HeroConfig heroConfig, string attr)
    {
        switch (attr)
        {
            case "Str":
                return heroConfig.Str.ToString();
            case "Inte":
                return heroConfig.Inte.ToString();
            case "Fair":
                return heroConfig.Fair.ToString();
            default:
                return attr;
        }
    }

    public void Init(int heroId, string[] attrs)
    {
        this.heroId = heroId;
        var heroCfg = HeroConfig.GetConfig(heroId);

        heroName.text = heroCfg.Name;
        
        textAttr1.text = GetAttr(heroCfg, attrs[0]);
        textAttr2.text = GetAttr(heroCfg, attrs[1]);

    }

    public void OnSelect(bool isSelect)
    {
        Debug.Log($"OnSelect {heroId} {isSelect}");
        this.isSelect = isSelect;
        checkImage.gameObject.SetActive(isSelect);
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? selectedColor : normalColor;
        }
    }
    
    // 处理点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        // 通知面板管理器当前单元格被点击
        popHeroSelectPanelManager.OnSelectItem(this, !isSelect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
