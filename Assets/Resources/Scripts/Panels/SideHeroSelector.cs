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

    private const int MAX_SELECT_COUNT = 3;

    private static SideHeroSelector instance;

    private static int currentCityId;
    private static int currentForceId;
    private static int currentDayFilter = SystemConst.CityDev.CITY_DAY_MIN;
    private static System.Action<List<int>> onHeroIdsSelected;

    public static void SetContext(int cityId, int forceId, int dayFilter, System.Action<List<int>> callback)
    {
        currentCityId = cityId;
        currentForceId = forceId;
        currentDayFilter = dayFilter;
        onHeroIdsSelected = callback;
        GameLog.Info($"SideHeroSelector.SetContext: cityId={cityId}, forceId={forceId}, dayFilter={dayFilter}");
    }

    /// <summary>
    /// 更新日程过滤并刷新列表（供 dayButton 点击时调用）
    /// </summary>
    public static void UpdateDayFilter(int dayFilter)
    {
        currentDayFilter = dayFilter;
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

        // 排他性日程带：只显示精确匹配 currentDayFilter 距离的武将
        // 2日剔除1日，3日剔除1日和2日
        int loyaltyThreshold = SysFormula.Hero.GetRecruitLoyaltyThreshold(currentDayFilter);

        foreach (var hero in saveData.heros)
        {
            int distance = SysFormula.City.CalculateCityDayDistance(currentCityId, hero.cityId);
            if (distance != currentDayFilter)
                continue;

            if (hero.state == HeroState.Wild)
            {
                result.Add(hero.heroId);
            }
            else if (hero.state == HeroState.Catched && hero.cityId == currentCityId)
            {
                result.Add(hero.heroId);
            }
            else if (hero.state == HeroState.Normal && hero.forceId != currentForceId && hero.loyalty < loyaltyThreshold)
            {
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
