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
    public TMP_Text textSoldier;
    public TMP_Text textHeroCount;

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

    public void SetData(int cityId)
    {
        this.cityId = cityId;
        var cityData = GameManager.Instance.GetCity(cityId);
        var cityCfg = WorldConfig.GetConfig(cityId);

        cityName.text = cityCfg.Cname;
        cityName.color = SysColor.GetForceColor(cityData.forceId);
        textOwner.text = ForceConfig.GetConfig(cityData.forceId).Cname;
        textSoldier.text = cityData.soldier.ToString();
        textHeroCount.text = cityData.GetHeroList(true, true).Count.ToString();

        SetSelected(false);
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
