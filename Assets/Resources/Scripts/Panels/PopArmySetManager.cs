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
    public TMP_Text textSoldier;
    public TMP_Text textFood;
    public Slider slider1;
    public TMP_Text textHeroName;
    public Image heroPic;
    private int maxSoldier;

    private static System.Action<int> onSoldierSetCallback;

    public static void SetSoldierSetCallback(System.Action<int> callback)
    {
        onSoldierSetCallback = callback;
    }

    private int GetOtherAllocated()
    {
        int otherAllocated = 0;
        foreach (var kvp in CityBattlePanelManager.GetAllocations())
        {
            if (kvp.Key != heroId)
            {
                var h = GameManager.Instance.GetHero(kvp.Key);
                if (h != null && h.cityId == cityId)
                    otherAllocated += kvp.Value;
            }
        }
        return otherAllocated;
    }

    private void UpdateDisplay(int soldier, int remainingSoldier, int remainingFood)
    {
        textSoldier.text = $"{soldier}/{remainingSoldier}";
        textFood.text = $"{soldier}/{remainingFood}";
    }

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            onSoldierSetCallback = null;
            PanelManager.Instance.HidePopArmySetPanel();
        });
        okBtn.onClick.AddListener(() =>
        {
            GameLog.Debug($"okBtn {cityId} {heroId}");
            if (cityId > 0)
            {
                var soldier = (int)(maxSoldier * slider1.value);
                CityBattlePanelManager.SetAllocatedSoldier(heroId, soldier);
                currentAllocated = soldier;
                onSoldierSetCallback?.Invoke(soldier);
                onSoldierSetCallback = null;
                PanelManager.Instance.SendSignal(new CityAttrChangeSignal { CityId = 0 });
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
                var cityData = GameManager.Instance.GetCity(cityId);
                int otherAllocated = GetOtherAllocated();
                int remainingSoldier = (int)cityData.soldier - otherAllocated;
                int remainingFood = (int)cityData.food - otherAllocated;
                UpdateDisplay(soldier, remainingSoldier, remainingFood);
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

        currentAllocated = CityBattlePanelManager.GetAllocatedSoldier(heroId);

        var heroCfg = HeroConfig.GetConfig(heroId);

        heroPic.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);
        textHeroName.text = heroCfg.Name;

        int otherAllocated = GetOtherAllocated();
        int remainingSoldier = (int)cityData.soldier - otherAllocated;
        int remainingFood = (int)cityData.food - otherAllocated;

        maxSoldier = Math.Min(SystemConst.Hero.MAX_SOLDIER_PER_HERO, Math.Min(remainingSoldier, remainingFood));
        slider1.value = maxSoldier > 0 ? (float)currentAllocated / maxSoldier : 0;

        UpdateDisplay(currentAllocated, remainingSoldier, remainingFood);
    }

    public void OnHide()
    {
    }
}
