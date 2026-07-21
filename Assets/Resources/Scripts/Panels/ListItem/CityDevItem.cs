using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;
using UnityEngine.EventSystems;
public class CityDevItem : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int cityId;
    private int devId;

    public TMP_Text nameText;
    public Image cityImg;
    public Image heroImg;
    public Image heroImgBG;
    public Image hero2Img;
    public Image hero2ImgBG;
    public Image blackMaskImg;
    public Image borderImage;
    public Image attrImg;
    public Button btnRun;

    private List<int> currentHeroIds = new List<int>();
    private CityPanelManager cityPanelManager;
    private bool isSelected = false;
    private bool isRunType = false;
    private bool isViewOnly = false;
    private int maxSlotCount = 1;

    void Start()
    {
        UpdateHeroDisplay();
        UpdateBlackMask();
        UpdateBorderColor();
    }

    void Update()
    {
        
    } 

    private string prefabName = "";

    public void SetDev(int cityId, int devId)
    {
        this.cityId = cityId;
        this.devId = devId;
        var devCfg = CityDevConfig.GetConfig(devId);
        string displayName = devCfg.Cname;
        if (devCfg.IsSpecial)
        {
            displayName = "★" + displayName;
        }
        else if (!string.IsNullOrEmpty(devCfg.DevAttr1) && SysFormula.City.CityHasResAddon(cityId, devCfg.DevAttr1))
        {
            displayName = "★" + displayName;
        }

        string attr1Lower = string.IsNullOrEmpty(devCfg.DevAttr1) ? "" : devCfg.DevAttr1.ToLower();
        string attr2Lower = string.IsNullOrEmpty(devCfg.DevAttr2) ? "" : devCfg.DevAttr2.ToLower();
        if (attr1Lower == "food" || attr1Lower == "soldier" || attr1Lower == "gold"
            || attr2Lower == "food" || attr2Lower == "soldier" || attr2Lower == "gold")
        {
            var cityData = GameManager.Instance.GetCity(cityId);
            if (cityData != null)
            {
                float multiplier = cityData.GetProductionMultiplier();
                if (multiplier < 0.999f || multiplier > 1.001f)
                {
                    string hex = ColorUtility.ToHtmlStringRGB(Color.red);
                    displayName = string.Format("{0}<color=#{1}>({2:F2})</color>", displayName, hex, multiplier);
                }
            }
        }

        nameText.text = displayName;
        cityImg.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.BuildingIcon(devCfg.Icon));
        isRunType = devCfg.Type == "run";
        prefabName = devCfg.Action;

        UpdateAttrImg(devCfg);
        UpdateBtnRun();
        UpdateBlackMask();
        UpdateSlotCount();
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

            attrImg.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.AttrIcon(attrCfg.Icon));
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
        if (manager != null)
        {
            isViewOnly = manager.IsViewOnly();
            UpdateBtnRun();
        }
    }

    public bool IsViewOnly()
    {
        return isViewOnly;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (isViewOnly)
        {
            SystemTip.Instance.ShowTip("查看模式下无法操作");
            return;
        }

        CityCellHero draggedHero = eventData.pointerDrag?.GetComponent<CityCellHero>();
        if (draggedHero != null && cityPanelManager != null)
        {
            if (cityPanelManager.AssignHeroToDevNode(draggedHero.heroId, this))
            {
                BGMPlayer.Instance.PlaySound("Sounds/equip");
            }
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
        if (!currentHeroIds.Contains(heroId))
        {
            currentHeroIds.Add(heroId);
        }
        UpdateHeroDisplay();
    }

    public void ClearHero()
    {
        currentHeroIds.Clear();
        UpdateHeroDisplay();
    }

    public void RemoveHero(int heroId)
    {
        currentHeroIds.Remove(heroId);
        UpdateHeroDisplay();
    }

    public List<int> GetHeroIds()
    {
        return new List<int>(currentHeroIds);
    }

    public int GetCurrentHeroId()
    {
        return currentHeroIds.Count > 0 ? currentHeroIds[0] : 0;
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

    private void UpdateHeroDisplay()
    {
        if (isRunType || devId == SystemConst.CityDev.IDLE_DEV_ID)
        {
            if (heroImg != null) heroImg.gameObject.SetActive(false);
            if (heroImgBG != null) heroImgBG.gameObject.SetActive(false);
            if (hero2Img != null) hero2Img.gameObject.SetActive(false);
            if (hero2ImgBG != null) hero2ImgBG.gameObject.SetActive(false);
            UpdateBlackMask();
            return;
        }

        // 第二槽位：未解锁时隐藏
        bool showSlot2 = maxSlotCount >= 2;
        if (hero2Img != null) hero2Img.gameObject.SetActive(showSlot2);
        if (hero2ImgBG != null) hero2ImgBG.gameObject.SetActive(showSlot2);

        // 第一槽位
        if (heroImg != null)
        {
            if (currentHeroIds.Count > 0)
            {
                var heroCfg = HeroConfig.GetConfig(currentHeroIds[0]);
                heroImg.enabled = true;
                heroImg.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg.Icon));
            }
            else
            {
                heroImg.enabled = false;
            }
        }

        // 第二槽位头像
        if (showSlot2 && hero2Img != null)
        {
            if (currentHeroIds.Count > 1)
            {
                var heroCfg = HeroConfig.GetConfig(currentHeroIds[1]);
                hero2Img.enabled = true;
                hero2Img.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg.Icon));
            }
            else
            {
                hero2Img.enabled = false;
            }
        }

        UpdateBlackMask();
        UpdateHeroImgBG();
    }

    private void UpdateBlackMask()
    {
        if (blackMaskImg != null && blackMaskImg.gameObject != null)
        {
            if (isRunType || devId == SystemConst.CityDev.IDLE_DEV_ID)
            {
                blackMaskImg.gameObject.SetActive(false);
            }
            else
            {
                blackMaskImg.gameObject.SetActive(currentHeroIds.Count == 0);
            }
        }
    }

    private void UpdateBtnRun()
    {
        btnRun.gameObject.SetActive(isRunType && !isViewOnly);
        btnRun.onClick.RemoveAllListeners();
        btnRun.onClick.AddListener(() =>
        {
            if (prefabName == "Battle")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityBattle(cityData.forceId);
            }
            else if (prefabName == "Move")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityMove(cityData.forceId, cityId);
            }
            else if (prefabName == "Praise")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityPraise(cityData.forceId, cityId, devId);
            }
            else if (prefabName == "UseHero")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityUseHero(cityData.forceId, cityId, devId);
            }
            else if (prefabName == "Trade")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityTrade(cityData.forceId, cityId);
            }
            else if (prefabName == "Search")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCitySearch(cityData.forceId, cityId);
            }
            else if (prefabName == "EnemyCity")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityEnemyCity(cityData.forceId, cityId, devId);
            }
            else if (prefabName == "Relation")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityRelation(cityData.forceId, cityId, devId);
            }
            else if (prefabName == "Tech")
            {
                var cityData = GameManager.Instance.GetCity(cityId);
                PanelManager.Instance.ShowCityTech(cityData.forceId, cityId, devId);
            }
        });
    }

    private void UpdateBorderColor()
    {
        if (borderImage != null)
        {
            borderImage.color = isSelected ? SysColor.UI.BorderSelectedColor : SysColor.UI.BorderColor;
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

        if (currentHeroIds.Count == 0)
        {
            heroImgBG.color = SysColor.UI.BorderColor;
            return;
        }

        var devCfg = CityDevConfig.GetConfig(devId);
        if (devCfg == null || devCfg.Attrs == null || devCfg.Attrs.Length == 0)
        {
            heroImgBG.color = SysColor.UI.BorderColor;
            return;
        }

        float totalWeightedValue = 0;
        foreach (var heroId in currentHeroIds)
        {
            var heroData = GameManager.Instance.GetHero(heroId);
            if (heroData != null)
            {
                totalWeightedValue += GetWeightedAttrValue(heroData, devCfg.Attrs);
            }
        }
        float avgWeightedValue = totalWeightedValue / currentHeroIds.Count;
        int tier = SysFormula.City.GetHeroTier(avgWeightedValue);

        if (tier == 0)
        {
            heroImgBG.color = SysColor.Hero.TierHighColor;
        }
        else if (tier == 1)
        {
            heroImgBG.color = SysColor.Hero.TierMediumColor;
        }
        else if (tier == 2)
        {
            heroImgBG.color = SysColor.Hero.TierLowColor;
        }
        else
        {
            heroImgBG.color = SysColor.UI.BorderColor;
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

    private void UpdateSlotCount()
    {
        if (devId == 0 || isRunType || devId == SystemConst.CityDev.IDLE_DEV_ID)
        {
            maxSlotCount = 1;
            return;
        }
        var cityData = GameManager.Instance.GetCity(cityId);
        int forceId = cityData != null ? cityData.forceId : 0;
        maxSlotCount = ForceTech.GetEffectiveSlotCount(forceId, devId);
    }

    public int GetMaxSlotCount()
    {
        return maxSlotCount;
    }
}
