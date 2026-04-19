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
    private int currentAllocated;

    public Button closeBtn;
    public Button okBtn;
    public Button maxBtn;
    public TMP_Text textSoldierCity;
    public TMP_Text textSoldier;
    public Slider slider1;
    public TMP_Text textHeroName;
    public Image heroPic;
    private int maxSoldier;

    private static Dictionary<int, int> heroSoldierAllocations = new Dictionary<int, int>();

    public static int GetAllocatedSoldier(int heroId)
    {
        if (heroSoldierAllocations.ContainsKey(heroId))
            return heroSoldierAllocations[heroId];
        return 0;
    }

    public static void ClearAllocations()
    {
        heroSoldierAllocations.Clear();
    }

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
                var change = soldier - currentAllocated;
                heroSoldierAllocations[heroId] = soldier;
                currentAllocated = soldier;
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
                var change = soldier - currentAllocated;
                var cityData = GameManager.Instance.GetCity(cityId);
                int otherAllocated = 0;
                foreach (var kvp in heroSoldierAllocations)
                {
                    if (kvp.Key != heroId)
                    {
                        var h = GameManager.Instance.GetHero(kvp.Key);
                        if (h != null && h.cityId == cityId)
                            otherAllocated += kvp.Value;
                    }
                }
                textSoldierCity.text = $"{(int)cityData.soldier - otherAllocated - soldier}";
            }
        });
    }

    void Update()
    {
    }

    public void OnShow(int heroId)
    {
        this.heroId = heroId;
        var heroData = GameManager.Instance.GetHero(heroId);
        this.cityId = heroData.cityId;
        var cityData = GameManager.Instance.GetCity(cityId);

        currentAllocated = GetAllocatedSoldier(heroId);
        textSoldierCity.text = $"{cityData.soldier}";
        textSoldier.text = $"{currentAllocated}";

        var heroCfg = HeroConfig.GetConfig(heroId);

        heroPic.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
        textHeroName.text = heroCfg.Name;

        maxSoldier = Math.Min(1000, (int)(cityData.soldier) + currentAllocated);
        slider1.value = maxSoldier > 0 ? (float)currentAllocated / maxSoldier : 0;
    }

    public void OnHide()
    {
    }
}
