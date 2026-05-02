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

    private static int currentHeroId;
    private static System.Action onArmsChanged;

    public static void SetContext(int heroId, System.Action callback)
    {
        currentHeroId = heroId;
        onArmsChanged = callback;
        GameLog.Info($"SideArmysSelector.SetContext: currentHeroId={heroId}");
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

        int currentArmsId = 0;
        var heroData = GameManager.Instance.GetHero(currentHeroId);
        if (heroData != null)
        {
            currentArmsId = heroData.armsId;
        }

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
        GameLog.Info($"SideArmysSelector.OnConfirm: currentHeroId={currentHeroId}, selectedItem={selectedItem}");
        
        if (selectedItem == null)
        {
            GameLog.Warn("SideArmysSelector.OnConfirm: selectedItem is null");
            return;
        }

        int newArmsId = selectedItem.GetArmsId();
        GameLog.Info($"SideArmysSelector.OnConfirm: newArmsId={newArmsId}");
        
        var heroData = GameManager.Instance.GetHero(currentHeroId);
        if (heroData == null)
        {
            GameLog.Warn($"SideArmysSelector.OnConfirm: heroData is null for heroId={currentHeroId}");
            return;
        }

        GameLog.Info($"SideArmysSelector.OnConfirm: heroData.heroId={heroData.heroId}, heroData.armsId={heroData.armsId}, heroData.forceId={heroData.forceId}");

        if (heroData.armsId == newArmsId)
        {
            PanelManager.Instance.HideSideBar();
            return;
        }

        var force = GameManager.Instance.GetForce(heroData.forceId);
        if (force == null)
        {
            GameLog.Warn($"SideArmysSelector.OnConfirm: force is null for forceId={heroData.forceId}");
            return;
        }

        bool canAfford = force.CanAffordArms(newArmsId, currentHeroId);
        GameLog.Info($"SideArmysSelector.OnConfirm: CanAffordArms={canAfford}");
        
        if (!canAfford)
        {
            SystemTip.Instance.ShowTip("资源不足");
            return;
        }

        bool result = heroData.SetArmsId(newArmsId);
        GameLog.Info($"SideArmysSelector.OnConfirm: SetArmsId result={result}, new armsId={heroData.armsId}");
        
        if (result)
        {
            onArmsChanged?.Invoke();
            PanelManager.Instance.HideSideBar();
        }
    }
}
