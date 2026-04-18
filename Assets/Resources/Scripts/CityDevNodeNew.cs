using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class CityDevNodeNew : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private int cityId;
    private int devId;

    public TMP_Text nameText;
    public Image cityImg;
    public Image heroImg;

    private int currentHeroId = 0;
    private CityPanelManager cityPanelManager;

    void Start()
    {
        if (heroImg != null)
        {
            heroImg.enabled = false;
        }
    }

    void Update()
    {
        
    } 

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        var devCfg = CityDevConfig.GetConfig(devId);
        var cityData = GameManager.Instance.GetCity(cityId);
        nameText.text = devCfg.Cname;
        cityImg.sprite = Resources.Load<Sprite>("Textures/Buildings/" + devCfg.Icon);
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
        if (heroId > 0 && heroImg != null)
        {
            var heroCfg = HeroConfig.GetConfig(heroId);
            if (heroCfg != null)
            {
                heroImg.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
                heroImg.enabled = true;
            }
        }
        else if (heroImg != null)
        {
            heroImg.enabled = false;
        }
    }

    public void ClearHero()
    {
        currentHeroId = 0;
        if (heroImg != null)
        {
            heroImg.enabled = false;
        }
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
}
