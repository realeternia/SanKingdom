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
    
    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 正常状态背景色
    private Color selectedColor = new Color(0.5f, 0.5f, 0.1f, 0.8f); // 高光绿色选中状态

    // Start is called before the first frame update
    void Start()
    {
        cityName.raycastTarget = false;
        textOwner.raycastTarget = false;
        textSoldier.raycastTarget = false;
        textHeroCount.raycastTarget = false;
        
        // 初始化背景色为正常状态
        OnSelect(false);
    }

    public void Init(int cityId)
    {
        this.cityId = cityId;
        var cityData = GameManager.Instance.GetCity(cityId);
        var cityCfg = WorldConfig.GetConfig(cityId);

        cityName.text = cityCfg.Cname;
        textOwner.text = ForceConfig.GetConfig(cityData.forceId).Cname;
        textSoldier.text = cityData.soldier.ToString();
        textHeroCount.text = cityData.GetHeroList().Count.ToString();

    }

    public void OnSelect(bool isSelect)
    {
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
