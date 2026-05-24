using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class SideCitySelector : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject subRegionMain;
    public SideCityItem itemPrefab;

    private SideCityItem selectedItem;
    public Button confirmButton;

    private static int currentCityId;
    private static List<int> cityIdList;
    private static System.Action<int> onCityIdSelected;

    public static void SetContext(int cityId, List<int> cities, System.Action<int> callback)
    {
        currentCityId = cityId;
        cityIdList = cities;
        onCityIdSelected = callback;
        GameLog.Info($"SideCitySelector.SetContext: cityId={cityId}, count={cities.Count}");
    }

    void Start()
    {
        LoadCityList();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    void LoadCityList()
    {
        foreach (Transform child in subRegionMain.transform)
        {
            Destroy(child.gameObject);
        }

        selectedItem = null;
        int count = 0;
        foreach (int id in cityIdList)
        {
            GameObject item = Instantiate(itemPrefab.gameObject, subRegionMain.transform);
            item.transform.localScale = Vector3.one;
            SideCityItem cityItem = item.GetComponent<SideCityItem>();
            cityItem.SetData(id);
            cityItem.SetOnClickCallback(OnItemSelected);

            if (id == currentCityId)
            {
                cityItem.SetSelected(true);
                selectedItem = cityItem;
            }

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

    void OnItemSelected(SideCityItem item)
    {
        if (selectedItem != null && selectedItem != item)
        {
            selectedItem.SetSelected(false);
        }

        item.SetSelected(true);
        selectedItem = item;
    }

    void OnConfirm()
    {
        if (selectedItem == null)
        {
            GameLog.Warn("SideCitySelector.OnConfirm: selectedItem is null");
            return;
        }

        int selectedCityId = selectedItem.GetCityId();
        onCityIdSelected?.Invoke(selectedCityId);
        PanelManager.Instance.HideSideBar();
    }
}
