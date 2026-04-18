using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using CommonConfig;
using Controls.Utils;

public class PopArmySetManager : MonoBehaviour
{
    private int cityId;
    private int heroId;

    public Button closeBtn;
    public Button okBtn;
    public Button maxBtn;
    public TMP_Text textSoldierCity;
    public TMP_Text textSoldier;
    public Slider slider1;
    public TMP_Text textHeroName;
    public Image heroPic;
    private int maxSoldier;
    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopArmySetPanel();
        });
        okBtn.onClick.AddListener(() =>
        {
            GameLog.Debug($"okBtn {cityId} {heroId}");
            if (cityId > 0)
            {
                var soldier = (int)(maxSoldier * slider1.value);
                var oldSoldier = GameManager.Instance.GetHero(heroId).soldier;
                var change = soldier - oldSoldier;
                GameManager.Instance.GetHero(heroId).soldier = soldier;
                GameManager.Instance.GetCity(cityId).soldier -= change;
                PanelManager.Instance.SendSignal("CityAttrChange", "", 0);
                PanelManager.Instance.HidePopArmySetPanel();
            }
        });
        maxBtn.onClick.AddListener(() =>
        {
            slider1.value = 1;
        });
        slider1.onValueChanged.AddListener((value) =>
        {
            if (cityId > 0)
            {
                var soldier = (int)(maxSoldier * slider1.value);
                textSoldier.text = $"{soldier}";
                var oldSoldier = GameManager.Instance.GetHero(heroId).soldier;
                var change = soldier - oldSoldier;
                textSoldierCity.text = $"{GameManager.Instance.GetCity(cityId).soldier - change}";
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnShow(int heroId)
    {
        this.heroId = heroId;
        var heroData = GameManager.Instance.GetHero(heroId);
        this.cityId = heroData.cityId;
        var cityData = GameManager.Instance.GetCity(cityId);
        textSoldierCity.text = $"{cityData.soldier}";
        textSoldier.text = $"{heroData.soldier}";

        var heroCfg = HeroConfig.GetConfig(heroId);

        heroPic.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
        textHeroName.text = heroCfg.Name;

        maxSoldier = Math.Min(1000, (int)(GameManager.Instance.GetCity(cityId).soldier) + heroData.soldier);
        slider1.value = (float)heroData.soldier / maxSoldier;
    }

    public void OnHide()
    {
    }    
}
