using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
public class SideArmysSelector : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject subRegionMain;
    public SideArmsItem itemPrefab;

    private SideArmsItem selectedItem;
    public Button confirmButton;

    private static int currentArmsIdForTroop;
    private static SaveTroopsData currentTroop;
    private static System.Action<int> onArmsIdSelected;

    public static void SetContextForTroop(int armsId, SaveTroopsData troop, System.Action<int> callback)
    {
        currentArmsIdForTroop = armsId;
        currentTroop = troop;
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
        var force = GameManager.Instance.CurrentForce;

        selectedItem = null;
        int count = 0;
        foreach (var config in ArmsConfig.ConfigList)
        {
            if (!config.CanAssign || !HasResourceProduction(config, force))
                continue;

            GameObject item = Instantiate(itemPrefab.gameObject, subRegionMain.transform);
            item.transform.localScale = Vector3.one;
            SideArmsItem armsItem = item.GetComponent<SideArmsItem>();
            armsItem.SetData(config.Id, currentTroop);
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

    void OnItemSelected(SideArmsItem item)
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

    static bool HasResourceProduction(ArmsConfig config, SaveForceData force)
    {
        if (config.HorseCost > 0 && force.GetAttr("horse") <= 0) return false;
        if (config.SteelCost > 0 && force.GetAttr("steel") <= 0) return false;
        if (config.WoodCost > 0 && force.GetAttr("wood") <= 0) return false;
        if (config.StoneCost > 0 && force.GetAttr("stone") <= 0) return false;
        return true;
    }
}
