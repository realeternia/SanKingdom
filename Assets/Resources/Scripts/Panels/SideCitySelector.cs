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

    private int currentCityId;
    private List<int> cityIdList;
    private System.Action<int> onCityIdSelected;
    private string attr1Name;
    private string attr2Name;

    public void Init(int cityId, List<int> cities, string attr1Name, string attr2Name, System.Action<int> callback)
    {
        currentCityId = cityId;
        cityIdList = cities;
        onCityIdSelected = callback;
        this.attr1Name = attr1Name;
        this.attr2Name = attr2Name;
        GameLog.Info($"SideCitySelector.Init: cityId={cityId}, count={cities.Count}");

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
            cityItem.SetData(id, attr1Name, attr2Name);
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
