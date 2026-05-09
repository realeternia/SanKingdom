using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityBattlePanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    private int foodCount = SystemConst.Expedition.DEFAULT_FOOD_DAYS;

    private int forceId;
    private int selectedCityId;
    public Button destButton;
    public TMP_Text attrVal1Text;

    public Button closeButton;
    public Button battleButton;

    private List<GameObject> cityBattleItems = new List<GameObject>();

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideCityBattle();
        });
        destButton.onClick.AddListener(() =>
        {
            var cityIds = MapTool.GetAdjacentEnemyCityIds(forceId);
            PanelManager.Instance.ShowPopCitySelectPanel(cityIds, selectedCityId, (selectedCityId) =>
            {
                this.selectedCityId = selectedCityId;
                if(selectedCityId == 0)
                {
                    attrVal1Text.text = "-";
                }
                else
                {
                    var cityCfg = WorldConfig.GetConfig(selectedCityId);
                    attrVal1Text.text = cityCfg.Cname;
                }
                ClearAllSelections();
                CreateCityBattleItems(forceId);
            });
        });

        if (battleButton != null)
        {
            battleButton.onClick.AddListener(OnBattle);
        }
    }

    void Update()
    {

    }

    public void Init(int forceId)
    {
        this.forceId = forceId;
        CreateCityBattleItems(forceId);
    }

    public bool CanSelectItem()
    {
        return selectedCityId > 0;
    }

    private void ClearAllSelections()
    {
        foreach (var itemObj in cityBattleItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<CityBattleItem>();
            if (itemScript != null)
            {
                itemScript.SetSelected(false);
            }
        }
    }

    private List<CityBattleItem> GetSelectedItems()
    {
        List<CityBattleItem> selected = new List<CityBattleItem>();
        foreach (var itemObj in cityBattleItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<CityBattleItem>();
            if (itemScript != null && itemScript.IsSelected())
            {
                selected.Add(itemScript);
            }
        }
        return selected;
    }

    private void OnBattle()
    {
        if (selectedCityId <= 0)
        {
            SystemTip.Instance.ShowTip("请选择目标城市");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择出战部队");
            return;
        }

        List<WarTroopsData> attackTroops = new List<WarTroopsData>();
        foreach (var item in selectedItems)
        {
            var troop = item.GetWarTeamData();
            if (troop == null) continue;
            if (troop.heroId1 > 0 && troop.soldierCount > 0)
            {
                attackTroops.Add(troop);
            }
        }

        if (attackTroops.Count == 0)
        {
            SystemTip.Instance.ShowTip("出战部队士兵数不能为0");
            return;
        }

        PanelManager.Instance.ShowPopResultPanel("出征", new List<PopResultPanelManager.AttrData>(), () =>
        {
            OnRun(selectedCityId, attackTroops);
        }, "atk2.mp4");
    }

    private void OnRun(int targetCityId, List<WarTroopsData> attackTroops)
    {
        var sourceCityIds = attackTroops
            .Where(t => t.heroId1 > 0)
            .Select(t => GameManager.Instance.GetHero(t.heroId1).cityId)
            .Distinct()
            .ToList();
        var force = GameManager.Instance.GetForce(forceId);

        PanelManager.Instance.HideCityBattle();

        force.ExecuteBattle(sourceCityIds, attackTroops, targetCityId, false);
    }

    private void CreateCityBattleItems(int forceId)
    {
        foreach (var item in cityBattleItems)
        {
            if (item != null)
                Destroy(item);
        }
        cityBattleItems.Clear();

        var cities = GameManager.Instance.GetCitiesByForce(forceId);

        HashSet<int> adjacentCityIds = null;
        if (selectedCityId > 0)
        {
            adjacentCityIds = new HashSet<int>(MapTool.GetAdjacentCityIds(selectedCityId));
        }

        List<WarTroopsData> allTeams = new List<WarTroopsData>();
        foreach (var city in cities)
        {
            if (adjacentCityIds != null && !adjacentCityIds.Contains(city.cityId))
                continue;

            foreach (var troop in city.troops)
            {
                if (troop.heroId1 > 0)
                {
                    allTeams.Add(troop);
                }
            }
        }

        if (allTeams.Count == 0) return;

        var itemPrefab = Resources.Load<GameObject>("Prefabs/Panels/ListItem/CityBattleItem");

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 750f;
        float itemHeight = 120f;
        float spacing = 10f;
        int itemsPerRow = 2;

        float totalWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * spacing;
        float startX = -totalWidth / 2f + itemWidth / 2f;

        for (int i = 0; i < allTeams.Count; i++)
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

            CityBattleItem itemScript = itemObj.GetComponent<CityBattleItem>();
            if (itemScript != null)
            {
                itemScript.Init(allTeams[i]);
            }

            cityBattleItems.Add(itemObj);
        }

        int totalRows = (allTeams.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing + 20f;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
    }
}
