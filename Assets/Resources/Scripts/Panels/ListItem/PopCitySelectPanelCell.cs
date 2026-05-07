using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class PopCitySelectPanelCell : MonoBehaviour, IPointerClickHandler
{
    public int cityId;

    public PopCitySelectPanelManager popCitySelectPanelManager;
    public TMP_Text cityName;
    public TMP_Text textOwner;
    public TMP_Text textSoldier;
    public TMP_Text textHeroCount;
    public Image backgroundImage;
    
    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    private Color selectedColor = new Color(0.5f, 0.5f, 0.1f, 0.8f);
    private bool isSelected = false;

    void Start()
    {
        cityName.raycastTarget = false;
        textOwner.raycastTarget = false;
        textSoldier.raycastTarget = false;
        textHeroCount.raycastTarget = false;

        if (!isSelected)
        {
            OnSelect(false);
        }
    }

    public void Init(int cityId)
    {
        this.cityId = cityId;
        var cityData = GameManager.Instance.GetCity(cityId);
        var cityCfg = WorldConfig.GetConfig(cityId);

        cityName.text = cityCfg.Cname;
        textOwner.text = ForceConfig.GetConfig(cityData.forceId).Cname;
        textSoldier.text = cityData.GetAttr("soldier").ToString();
        textHeroCount.text = cityData.GetHeroList(true, true).Count.ToString(); // 包含在野英雄，因为城市选择面板需要显示所有英雄数量

    }

    public void OnSelect(bool isSelect)
    {
        isSelected = isSelect;
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? selectedColor : normalColor;
        }
    }
    
    // 处理点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        // 通知面板管理器当前单元格被点击
        popCitySelectPanelManager.OnSelectItem(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
