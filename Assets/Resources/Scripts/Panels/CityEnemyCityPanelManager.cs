using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;

public class CityEnemyCityPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    public Button cityButton;
    public Button typeButton;
    public TMP_Text attrVal1Text;
    public TMP_Text typeButtonText;

    public ResCheckItem resCheckItem;

    public Button closeButton;
    public Button okButton;

    private int forceId;
    private int cityId;
    private int currentDevId;
    private int selectedTargetCityId = 0;
    private int kingCityId = 0;

    private List<GameObject> heroHeadItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityEnemyCity();
        });

        cityButton.onClick.AddListener(OnCityButtonClick);

        if (typeButton != null)
        {
            typeButton.onClick.AddListener(OnTypeButtonClick);
        }

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnExecute);
        }
    }

    void Update()
    {

    }

    public void Init(int forceId, int cityId, int devId)
    {
        this.forceId = forceId;
        this.cityId = cityId;
        this.currentDevId = SystemConst.CityDev.DISTURB_DEV_ID;
        this.selectedTargetCityId = 0;

        var force = GameManager.Instance.GetForce(forceId);
        var kingCity = force != null ? force.GetKingCity() : null;
        kingCityId = kingCity != null ? kingCity.cityId : cityId;

        attrVal1Text.text = "-";
        UpdateTypeButtonText();
        InitResCheckItem();
        CreateHeroHeadItems();
    }

    private void OnCityButtonClick()
    {
        // 获取所有非我方城市，按势力排序
        var enemyCityIds = GameManager.Instance.SaveData.cities
            .Where(c => c.forceId != forceId)
            .Select(c => c.cityId)
            .OrderBy(id => GameManager.Instance.GetCity(id).forceId)
            .ThenBy(id => id)
            .ToList();

        if (enemyCityIds.Count == 0)
        {
            SystemTip.Instance.ShowTip("没有可选的敌方城市");
            return;
        }

        PanelManager.Instance.ShowSideBar("SideCitySelector", (panelObj) =>
        {
            var selector = panelObj.GetComponent<SideCitySelector>();
            selector.Init(selectedTargetCityId, enemyCityIds, "wall", "happy", (newCityId) =>
            {
                selectedTargetCityId = newCityId;
                if (newCityId == 0)
                {
                    attrVal1Text.text = "-";
                }
                else
                {
                    var cityCfg = WorldConfig.GetConfig(newCityId);
                    attrVal1Text.text = cityCfg != null ? cityCfg.Cname : "-";
                }
                UpdateAllHeroDayText();
            });
        });
    }

    private void OnTypeButtonClick()
    {
        currentDevId = (currentDevId == SystemConst.CityDev.DISTURB_DEV_ID)
            ? SystemConst.CityDev.DESTROY_DEV_ID
            : SystemConst.CityDev.DISTURB_DEV_ID;
        UpdateTypeButtonText();
        InitResCheckItem();
    }

    private void UpdateTypeButtonText()
    {
        if (typeButtonText != null)
        {
            var devCfg = CityDevConfig.GetConfig(currentDevId);
            typeButtonText.text = devCfg != null ? devCfg.Cname : "-";
        }
    }

    private void InitResCheckItem()
    {
        if (resCheckItem == null) return;

        var devCfg = CityDevConfig.GetConfig(currentDevId);
        if (devCfg == null) return;

        var force = GameManager.Instance.GetForce(forceId);
        int gold = force != null ? (int)force.gold : 0;
        resCheckItem.Init(ResPath.Texture.AttrIcon("citygold"), gold.ToString());
    }

    private bool CanSelectHero()
    {
        return true;
    }

    private void OnHeroSelectionChanged()
    {
        UpdateResCheckDisplay();
    }

    private void UpdateResCheckDisplay()
    {
        if (resCheckItem == null) return;

        var force = GameManager.Instance.GetForce(forceId);
        var devCfg = CityDevConfig.GetConfig(currentDevId);
        int gold = force != null ? (int)force.gold : 0;
        int cost = GetSelectedItems().Count * (devCfg != null ? devCfg.GoldCost : 0);
        if (cost > gold)
        {
            resCheckItem.UpdateDisplay($"<color=red>{cost}</color>/{gold}");
        }
        else
        {
            resCheckItem.UpdateDisplay($"{cost}/{gold}");
        }
    }

    private List<HeroHeadItem> GetSelectedItems()
    {
        List<HeroHeadItem> selected = new List<HeroHeadItem>();
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null && itemScript.IsSelected())
            {
                selected.Add(itemScript);
            }
        }
        return selected;
    }

    /// <summary>
    /// 计算执行人到达目标城市的日程（按主公所在城市到目标城市，目标必为敌方故 crossCountry=true）。
    /// 与登庸同规则：distance 即 SetDayText 显示值。
    /// </summary>
    private int CalculateDayDistance()
    {
        if (selectedTargetCityId <= 0 || kingCityId <= 0) return 0;
        return SysFormula.City.CalculateHeroDayDistance(kingCityId, selectedTargetCityId, true);
    }

    /// <summary>
    /// 选择/切换目标城市后，刷新所有 HeroHeadItem 的天数显示
    /// </summary>
    private void UpdateAllHeroDayText()
    {
        int day = CalculateDayDistance();
        foreach (var itemObj in heroHeadItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null)
            {
                itemScript.SetDayText(day);
            }
        }
    }

    private void OnExecute()
    {
        if (selectedTargetCityId <= 0)
        {
            SystemTip.Instance.ShowTip("请选择目标城市");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择执行武将");
            return;
        }

        int[] executorHeroIds = selectedItems.Select(item => item.GetHeroId()).ToArray();

        var devCfg = CityDevConfig.GetConfig(currentDevId);
        var force = GameManager.Instance.GetForce(forceId);

        bool success;
        List<PopResultPanelManager.AttrData> attrDatas;
        if (currentDevId == SystemConst.CityDev.DESTROY_DEV_ID)
        {
            success = force.ExecuteCityDestroy(cityId, currentDevId, executorHeroIds, selectedTargetCityId, out attrDatas);
        }
        else
        {
            success = force.ExecuteCityDisturb(cityId, currentDevId, executorHeroIds, selectedTargetCityId, out attrDatas);
        }

        if (success)
        {
            PanelManager.Instance.ShowPopResultPanel(devCfg.Cname, attrDatas, () =>
            {
                PanelManager.Instance.HideCityEnemyCity();
            }, devCfg != null ? devCfg.Mp4 : "", false);
        }
    }

    private void CreateHeroHeadItems()
    {
        foreach (var item in heroHeadItems)
        {
            if (item != null)
                Destroy(item);
        }
        heroHeadItems.Clear();

        var force = GameManager.Instance.GetForce(forceId);
        if (force == null) return;

        var cities = GameManager.Instance.GetCitiesByForce(forceId);
        cities = cities.OrderByDescending(c => c.cityId == kingCityId).ToList();

        List<int> heroList = new List<int>();
        foreach (var city in cities)
        {
            var cityHeroes = city.GetNormalHeroList();
            heroList.AddRange(cityHeroes);
        }

        if (heroList.Count == 0) return;

        int currentRound = GameManager.Instance.SaveData.round;
        int kingHeroId = ForceConfig.GetConfig(forceId).HeroId;
        heroList = heroList.OrderBy(h =>
            {
                var hero = GameManager.Instance.GetHero(h);
                return hero != null && hero.round >= currentRound ? 1 : 0;
            })
            .ThenByDescending(h => h == kingHeroId ? 1 : 0)
            .ThenByDescending(h => GameManager.Instance.GetHero(h).GetAttr("charm"))
            .ToList();

        var itemPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelGismo("HeroHeadItem"));

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 156f;
        float itemHeight = 185f;
        float spacing = 10f;
        int itemsPerRow = Mathf.Max(1, Mathf.FloorToInt((containerRect.rect.width + spacing) / (itemWidth + spacing)));

        float totalWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * spacing;
        float startX = -totalWidth / 2f + itemWidth / 2f;

        int day = CalculateDayDistance();

        for (int i = 0; i < heroList.Count; i++)
        {
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;

            float posX = startX + col * (itemWidth + spacing);
            float posY = -row * (itemHeight + spacing);

            GameObject itemObj = Instantiate(itemPrefab, itemRegionMain.transform);
            itemObj.transform.localScale = Vector3.one;

            RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(posX, posY);
                rectTransform.sizeDelta = new Vector2(itemWidth, itemHeight);
            }

            HeroHeadItem itemScript = itemObj.GetComponent<HeroHeadItem>();
            if (itemScript != null)
            {
                string attText = GetHeroAttText(heroList[i]);
                var heroData = GameManager.Instance.GetHero(heroList[i]);
                bool hasActed = heroData != null && heroData.round >= GameManager.Instance.SaveData.round;
                itemScript.Init(heroList[i], attText, forceId, hasActed);
                itemScript.SetCallbacks(CanSelectHero, OnHeroSelectionChanged);
                itemScript.SetDayText(day);
            }

            heroHeadItems.Add(itemObj);
        }

        int totalRows = (heroList.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }

    private string GetHeroAttText(int heroId)
    {
        var heroData = GameManager.Instance.GetHero(heroId);
        if (heroData == null) return "";
        var cityCfg = WorldConfig.GetConfig(heroData.cityId);
        return cityCfg != null ? cityCfg.Cname : "";
    }
}
