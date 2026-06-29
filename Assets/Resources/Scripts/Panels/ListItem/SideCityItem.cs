using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class SideCityItem : MonoBehaviour
{
    public Image BG;
    public TMP_Text cityName;
    public TMP_Text textOwner;
    public TMP_Text text1;
    public TMP_Text text2;
    public IconLoader iconLoader1;
    public IconLoader iconLoader2;


    private bool isSelected = false;

    public Button button;
    private System.Action<SideCityItem> onClickCallback;
    private int cityId;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnItemClick);
        }
    }

    void Update()
    {

    }

    public void SetData(int cityId, string attr1Name, string attr2Name)
    {
        this.cityId = cityId;
        var cityData = GameManager.Instance.GetCity(cityId);
        var cityCfg = WorldConfig.GetConfig(cityId);

        cityName.text = cityCfg.Cname;
        textOwner.color = SysColor.GetForceColor(cityData.forceId);
        textOwner.text = ForceConfig.GetConfig(cityData.forceId).Cname;
        text1.text = GetAttrDisplayValue(cityData, attr1Name);
        text2.text = GetAttrDisplayValue(cityData, attr2Name);

        UpdateAttrIcon(iconLoader1, attr1Name);
        UpdateAttrIcon(iconLoader2, attr2Name);

        SetSelected(false);
    }

    private static void UpdateAttrIcon(IconLoader loader, string attrName)
    {
        if (loader == null) return;
        var cfg = CityAttrConfig.GetConfigByname(attrName);
        loader.sourceType = IconSourceType.CityAttr;
        loader.SetId(cfg.Id);
        loader.RefreshIcon();
    }

    private static string GetAttrDisplayValue(SaveCityData cityData, string attrName)
    {
        return ((int)cityData.GetAttr(attrName)).ToString();
    }

    public void SetOnClickCallback(System.Action<SideCityItem> callback)
    {
        onClickCallback = callback;
    }

    public void OnItemClick()
    {
        onClickCallback?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (BG != null)
        {
            BG.color = isSelected ? SysColor.UI.MatchColor : SysColor.Theme.CellNormalDark;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public int GetCityId()
    {
        return cityId;
    }
}
