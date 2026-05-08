using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using Controls.Utils;

public class SideArmysSelector : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject subRegionMain;
    public SelectArmsItem itemPrefab;

    private SelectArmsItem selectedItem;
    public Button confirmButton;

    private static int currentArmsIdForTroop;
    private static System.Action<int> onArmsIdSelected;

    public static void SetContextForTroop(int armsId, System.Action<int> callback)
    {
        currentArmsIdForTroop = armsId;
        onArmsIdSelected = callback;
        GameLog.Info($"SideArmysSelector.SetContextForTroop: armsId={armsId}");
    }

    void Start()
    {
        LoadArmsList();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    void LoadArmsList()
    {
        foreach (Transform child in subRegionMain.transform)
        {
            Destroy(child.gameObject);
        }

        int currentArmsId = currentArmsIdForTroop;

        selectedItem = null;
        int count = 0;
        foreach (var config in ArmsConfig.ConfigList)
        {
            GameObject item = Instantiate(itemPrefab.gameObject, subRegionMain.transform);
            item.transform.localScale = Vector3.one;
            SelectArmsItem armsItem = item.GetComponent<SelectArmsItem>();
            armsItem.SetData(config.Id);
            armsItem.SetOnClickCallback(OnItemSelected);

            if (config.Id == currentArmsId)
            {
                armsItem.SetSelected(true);
                selectedItem = armsItem;
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

    void OnItemSelected(SelectArmsItem item)
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
            GameLog.Warn("SideArmysSelector.OnConfirm: selectedItem is null");
            return;
        }

        int newArmsId = selectedItem.GetArmsId();
        onArmsIdSelected?.Invoke(newArmsId);
        PanelManager.Instance.HideSideBar();
    }
}
