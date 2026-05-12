using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class CityCellCity : MonoBehaviour, IPointerDownHandler
{
    public CityPanelManager cityPanelManager;

    public int cityId;
    public TMP_Text cityName;
    public bool isSelect = false;
    public Image backgroundImage;
    public Image crownIcon;

    void Start()
    {
        cityName.raycastTarget = false;
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        UpdateBackgroundColor();
    }

    public void Init(string city)
    {
        foreach (var cityCfg in WorldConfig.ConfigList)
        {
            if (cityCfg.Cname == city)
            {
                cityId = cityCfg.Id;
                break;
            }
        }
        cityName.text = city;
        UpdateCrownIcon();
    }

    public void Init(int id, string displayName)
    {
        cityId = id;
        cityName.text = displayName;
        UpdateCrownIcon();
    }

    private void UpdateCrownIcon()
    {
        if (crownIcon == null) return;
        
        var cityData = GameManager.Instance.GetCity(cityId);
        if (cityData == null)
        {
            crownIcon.gameObject.SetActive(false);
            return;
        }
        
        var forceCfg = ForceConfig.GetConfig(cityData.forceId);
        int kingHeroId = forceCfg.HeroId;
        
        bool isKingCity = cityData.ownerHeroId == kingHeroId;
        crownIcon.gameObject.SetActive(isKingCity);
    }

    public void SetSelected(bool selected)
    {
        isSelect = selected;
        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormal;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cityPanelManager.OnSelectCity(this);
    }
}
