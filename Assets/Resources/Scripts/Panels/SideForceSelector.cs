using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using System.Linq;

public class SideForceSelector : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject subRegionMain;
    public SideForceItem itemPrefab;

    private List<SideForceItem> selectedItems = new List<SideForceItem>();
    private List<SideForceItem> allItems = new List<SideForceItem>();
    public Button confirmButton;

    private const int MAX_SELECT_COUNT = 1;

    private static SideForceSelector instance;

    private static int srcForceId;
    private static List<int> excludeForceIds = new List<int>();
    private static System.Action<List<int>> onForceIdsSelected;

    public static void SetContext(int srcForceId, System.Action<List<int>> callback, List<int> excludeForceIds = null)
    {
        SideForceSelector.srcForceId = srcForceId;
        SideForceSelector.excludeForceIds = excludeForceIds ?? new List<int>();
        onForceIdsSelected = callback;

        GameLog.Info($"SideForceSelector.SetContext: srcForceId={srcForceId} excludeCount={SideForceSelector.excludeForceIds.Count}");
    }

    void Start()
    {
        instance = this;
        LoadForceList();

        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirm);
        }
    }

    void LoadForceList()
    {
        foreach (Transform child in subRegionMain.transform)
        {
            Destroy(child.gameObject);
        }

        selectedItems.Clear();
        allItems.Clear();
        List<int> forceIds = GetAvailableForceIds();
        int count = 0;

        foreach (int forceId in forceIds)
        {
            GameObject item = Instantiate(itemPrefab.gameObject, subRegionMain.transform);
            item.transform.localScale = Vector3.one;
            SideForceItem forceItem = item.GetComponent<SideForceItem>();
            forceItem.SetData(forceId, srcForceId);
            forceItem.SetOnClickCallback(OnItemSelected);
            allItems.Add(forceItem);
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

    List<int> GetAvailableForceIds()
    {
        List<int> result = new List<int>();
        var saveData = GameManager.Instance.SaveData;
        if (saveData == null || saveData.forces == null)
        {
            GameLog.Warn("SideForceSelector.GetAvailableForceIds: saveData or forces is null");
            return result;
        }

        foreach (var force in saveData.forces)
        {
            if (force.isEliminated)
                continue;
            if (force.forceId == srcForceId)
                continue;
            if (excludeForceIds.Contains(force.forceId))
                continue;
            result.Add(force.forceId);
        }

        // 按友好度升序排列
        var forceRelation = saveData.forceRelation;
        result.Sort((a, b) =>
        {
            int relationA = forceRelation.GetRelation(srcForceId, a);
            int relationB = forceRelation.GetRelation(srcForceId, b);
            return relationA.CompareTo(relationB);
        });
        return result;
    }

    void OnItemSelected(SideForceItem item)
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
                SystemTip.Instance.ShowTip($"最多选择{MAX_SELECT_COUNT}个势力");
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
            GameLog.Warn("SideForceSelector.OnConfirm: selectedItems is empty");
            return;
        }

        List<int> selectedForceIds = selectedItems.Select(item => item.GetForceId()).ToList();
        onForceIdsSelected?.Invoke(selectedForceIds);
        PanelManager.Instance.HideSideBar();
    }
}
