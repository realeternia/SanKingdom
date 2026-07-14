using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using System.Linq;

public class SideHeroSelector : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject subRegionMain;
    public SideHeroItem itemPrefab;

    private List<SideHeroItem> selectedItems = new List<SideHeroItem>();
    private List<SideHeroItem> allItems = new List<SideHeroItem>();
    public Button confirmButton;

    private const int MAX_SELECT_COUNT = 1;

    private static SideHeroSelector instance;

    private static int currentCityId;
    private static int currentForceId;
    private static bool currentIsCrossCountry = false;
    private static System.Action<List<int>> onHeroIdsSelected;

    public static void SetContext(int cityId, int forceId, bool isCrossCountry, System.Action<List<int>> callback)
    {
        currentForceId = forceId;
        currentIsCrossCountry = isCrossCountry;
        onHeroIdsSelected = callback;

        // 用主公所在城市作为日程计算基准
        var force = GameManager.Instance.GetForce(forceId);
        var kingCity = force != null ? force.GetKingCity() : null;
        currentCityId = kingCity != null ? kingCity.cityId : cityId;

        GameLog.Info($"SideHeroSelector.SetContext: cityId={cityId}, kingCityId={currentCityId}, forceId={forceId}, isCrossCountry={isCrossCountry}");
    }

    /// <summary>
    /// 切换本国内/他国家模式并刷新列表（供 dayButton 点击时调用）
    /// </summary>
    public static void UpdateCrossCountry(bool isCrossCountry)
    {
        currentIsCrossCountry = isCrossCountry;
        if (instance != null)
            instance.LoadHeroList();
    }

    void Start()
    {
        instance = this;
        LoadHeroList();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    void LoadHeroList()
    {
        foreach (Transform child in subRegionMain.transform)
        {
            Destroy(child.gameObject);
        }

        selectedItems.Clear();
        allItems.Clear();
        List<int> heroIds = GetAvailableHeroIds();
        int count = 0;

        foreach (int heroId in heroIds)
        {
            GameObject item = Instantiate(itemPrefab.gameObject, subRegionMain.transform);
            item.transform.localScale = Vector3.one;
            SideHeroItem heroItem = item.GetComponent<SideHeroItem>();
            heroItem.SetData(heroId);
            heroItem.SetOnClickCallback(OnItemSelected);
            allItems.Add(heroItem);
            count++;
        }

        RectTransform subRect = subRegionMain.GetComponent<RectTransform>();
        RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();

        if (subRect != null && itemRect != null)
        {
            subRect.sizeDelta = new Vector2(subRect.sizeDelta.x, itemRect.sizeDelta.y * count);
        }

        if (scrollRectMain != null)
        {
            scrollRectMain.normalizedPosition = new Vector2(0, 1);
        }
    }

    List<int> GetAvailableHeroIds()
    {
        List<int> result = new List<int>();
        var saveData = GameManager.Instance.SaveData;
        if (saveData == null || saveData.heros == null)
        {
            GameLog.Warn("SideHeroSelector.GetAvailableHeroIds: saveData or heros is null");
            return result;
        }

        // 按目标武将所在城市归属筛选：本国内=己方城市，他国家=敌方城市
        // 他国家模式下敌方在职武将需 loyalty < threshold 才显示
        int loyaltyThreshold = SystemConst.Hero.RECRUIT_ENEMY_LOYALTY_THRESHOLD;

        foreach (var hero in saveData.heros)
        {
            var heroCity = GameManager.Instance.GetCity(hero.cityId);
            bool heroInOwnCountry = heroCity != null && heroCity.forceId == currentForceId;
            // 目标城市归属需与当前模式一致
            if (currentIsCrossCountry == heroInOwnCountry)
                continue;

            if (hero.state == HeroState.Wild)
            {
                result.Add(hero.heroId);
            }
            else if (!currentIsCrossCountry && hero.state == HeroState.Catched && hero.cityId == currentCityId)
            {
                // 俘虏仅在本国内模式显示，且必须在主公城市
                result.Add(hero.heroId);
            }
            else if (currentIsCrossCountry && hero.state == HeroState.Normal && hero.forceId != currentForceId && hero.loyalty < loyaltyThreshold)
            {
                // 敌方在职武将仅在他国家模式显示
                result.Add(hero.heroId);
            }
        }

        result = result.OrderBy(h =>
        {
            var heroData = GameManager.Instance.GetHero(h);
            return heroData != null ? heroData.loyalty : 0;
        }).ToList();

        return result;
    }

    void OnItemSelected(SideHeroItem item)
    {
        if (item.IsSelected())
        {
            item.SetSelected(false);
            selectedItems.Remove(item);
        }
        else
        {
            if (selectedItems.Count >= MAX_SELECT_COUNT)
            {
                SystemTip.Instance.ShowTip($"最多选择{MAX_SELECT_COUNT}个武将");
                return;
            }
            item.SetSelected(true);
            selectedItems.Add(item);
        }
    }

    void OnConfirm()
    {
        if (selectedItems.Count == 0)
        {
            GameLog.Warn("SideHeroSelector.OnConfirm: selectedItems is empty");
            return;
        }

        List<int> selectedHeroIds = selectedItems.Select(item => item.GetHeroId()).ToList();
        onHeroIdsSelected?.Invoke(selectedHeroIds);
        PanelManager.Instance.HideSideBar();
    }
}
