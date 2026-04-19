using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class CityDevNodeNew : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int cityId;
    private int devId;

    public TMP_Text nameText;
    public Image cityImg;
    public Image heroImg;
    public Image heroImgBG;
    public Image blackMaskImg;
    public Image borderImage;
    public Image attrImg;

    private int currentHeroId = 0;
    private CityPanelManager cityPanelManager;
    private bool isSelected = false;
    private Color normalBorderColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    private Color selectedBorderColor = Color.green;
    private Color grayColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    void Start()
    {
        if (heroImg != null)
        {
            heroImg.enabled = currentHeroId > 0;
        }
        UpdateBlackMask();
        UpdateBorderColor();
    }

    void Update()
    {
        
    } 

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        var devCfg = CityDevConfig.GetConfig(devId);
        nameText.text = devCfg.Cname;
        cityImg.sprite = Resources.Load<Sprite>("Textures/Buildings/" + devCfg.Icon);

        UpdateAttrImg(devCfg);
    }

    private void UpdateAttrImg(CityDevConfig devCfg)
    {
        if (attrImg == null)
            return;

        if (string.IsNullOrEmpty(devCfg.DevAttr1))
        {
            attrImg.gameObject.SetActive(false);
            return;
        }

        string attrName = devCfg.DevAttr1.ToLower();
        try
        {
            var attrCfg = CityAttrConfig.GetConfigByname(attrName);
            if (attrCfg == null || string.IsNullOrEmpty(attrCfg.Icon))
            {
                attrImg.gameObject.SetActive(false);
                return;
            }

            attrImg.sprite = Resources.Load<Sprite>("Textures/Icons/" + attrCfg.Icon);
            attrImg.gameObject.SetActive(true);
        }
        catch
        {
            attrImg.gameObject.SetActive(false);
        }
    }

    public void SetCityPanelManager(CityPanelManager manager)
    {
        this.cityPanelManager = manager;
    }

    public void OnDrop(PointerEventData eventData)
    {
        CityCellHero draggedHero = eventData.pointerDrag?.GetComponent<CityCellHero>();
        if (draggedHero != null && cityPanelManager != null)
        {
            cityPanelManager.AssignHeroToDevNode(draggedHero.heroId, this);
            BGMPlayer.Instance.PlaySound("Sounds/equip");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CityCellHero>() != null)
        {
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void SetHero(int heroId)
    {
        this.currentHeroId = heroId;
        if (heroId > 0)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);

            heroImg.enabled = true;
            heroImg.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
        }

        UpdateBlackMask();
        UpdateHeroImgBG();
    }

    public void ClearHero()
    {
        currentHeroId = 0;
        if (heroImg != null)
        {
            heroImg.enabled = false;
        }
        UpdateBlackMask();
        UpdateHeroImgBG();
    }

    public int GetCurrentHeroId()
    {
        return currentHeroId;
    }

    public int GetDevId()
    {
        return devId;
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }

    private void UpdateBlackMask()
    {
        if (blackMaskImg != null && blackMaskImg.gameObject != null)
        {
            blackMaskImg.gameObject.SetActive(currentHeroId == 0);
        }
    }

    private void UpdateBorderColor()
    {
        if (borderImage != null)
        {
            borderImage.color = isSelected ? selectedBorderColor : normalBorderColor;
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBorderColor();
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cityPanelManager != null)
        {
            cityPanelManager.OnSelectDevNode(this);
        }
    }

    public void UpdateHeroImgBG()
    {
        if (heroImgBG == null)
            return;

        if (currentHeroId == 0)
        {
            heroImgBG.color = grayColor;
            return;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        if (devCfg == null || devCfg.Attrs == null || devCfg.Attrs.Length == 0)
        {
            heroImgBG.color = grayColor;
            return;
        }

        var heroData = GameManager.Instance.GetHero(currentHeroId);
        if (heroData == null)
        {
            heroImgBG.color = grayColor;
            return;
        }

        float weightedValue = GetWeightedAttrValue(heroData, devCfg.Attrs);

        if (weightedValue >= 90)
        {
            heroImgBG.color = Color.red;
        }
        else if (weightedValue >= 80)
        {
            heroImgBG.color = Color.yellow;
        }
        else if (weightedValue >= 70)
        {
            heroImgBG.color = Color.green;
        }
        else
        {
            heroImgBG.color = grayColor;
        }
    }

    private float GetWeightedAttrValue(SaveHeroData heroData, string[] attrs)
    {
        if (attrs.Length == 1)
        {
            return heroData.GetAttr(attrs[0]);
        }
        else
        {
            float firstAttr = heroData.GetAttr(attrs[0]);
            float secondAttr = heroData.GetAttr(attrs[1]);
            return firstAttr * (2f / 3f) + secondAttr * (1f / 3f);
        }
    }
}
