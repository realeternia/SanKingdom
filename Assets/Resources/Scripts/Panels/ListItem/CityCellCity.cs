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
    public Color normalColor = Color.black;
    public Color selectedColor = Color.green;

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
            backgroundImage.color = isSelect ? selectedColor : normalColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cityPanelManager.OnSelectCity(this);
    }
}
