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
                    return;
                }
                var cityCfg = WorldConfig.GetConfig(selectedCityId);
                attrVal1Text.text = cityCfg.Cname;
            });
        });

    }

    void Update()
    {

    }

    public void Init(int forceId)
    {
        this.forceId = forceId;
        CreateCityBattleItems(forceId);
    }

    private void CreateCityBattleItems(int forceId)
    {
        foreach (var item in cityBattleItems)
        {
            if (item != null)
                Destroy(item);
        }
        cityBattleItems.Clear();

        var forceData = GameManager.Instance.GetForce(forceId);

        List<WarTroopsData> allTeams = new List<WarTroopsData>();
        foreach (var warPlan in forceData.warPlans)
        {
            if (warPlan != null && warPlan.teams != null)
            {
                allTeams.AddRange(warPlan.teams);
            }
        }

        if (allTeams.Count == 0) return;

        var itemPrefab = Resources.Load<GameObject>("Prefabs/Panels/ListItem/CityBattleItem");

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 400f;
        float itemHeight = 200f;
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
