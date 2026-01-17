using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using CommonConfig;

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
    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopArmySetPanel();
        });
        okBtn.onClick.AddListener(() =>
        {
            Debug.Log($"okBtn {cityId} {heroId}");
            if (cityId > 0)
            {
                var total = GameManager.Instance.GetCity(cityId).soldier + GameManager.Instance.GetHero(heroId).soldier;
                var maxSoldier = Math.Min(1000, total);
                var soldier = (int)(maxSoldier * slider1.value);
                GameManager.Instance.GetHero(heroId).soldier = soldier;
                GameManager.Instance.GetCity(cityId).soldier = total - soldier;
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
                var maxSoldier = Math.Min(1000, GameManager.Instance.GetCity(cityId).soldier);
                textSoldier.text = $"{(int)(maxSoldier * slider1.value)}";
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

        heroPic.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
        textHeroName.text = heroCfg.Name;

        var maxSoldier = Math.Min(1000, GameManager.Instance.GetCity(cityId).soldier + heroData.soldier);
        slider1.value = (float)heroData.soldier / maxSoldier;
    }

    public void OnHide()
    {
    }    
}
