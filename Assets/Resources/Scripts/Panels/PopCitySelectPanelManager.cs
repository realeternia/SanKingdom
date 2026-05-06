using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopCitySelectPanelManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject cellPrefab;

    public Button closeBtn;
    public Button selectBtn;
    private PopCitySelectPanelCell lastSelectedCell;
    private System.Action<int> onSelectCallback;

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopCitySelectPanel();
        });
        selectBtn.onClick.AddListener(() =>
        {
            if (lastSelectedCell != null && onSelectCallback != null)
            {
                onSelectCallback(lastSelectedCell.cityId);
                PanelManager.Instance.HidePopCitySelectPanel();
            }
        });
    }

    private void Init(List<int> cityIds, System.Action<int> callback)
    {
        onSelectCallback = callback;
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }

        int itemCount = 0;
        foreach (var id in cityIds)
        {
            GameObject cell = Instantiate(cellPrefab, rankParent.transform);
            cell.transform.localScale = Vector3.one;
            PopCitySelectPanelCell cellInfo = cell.GetComponent<PopCitySelectPanelCell>();
            cellInfo.popCitySelectPanelManager = this;
            cellInfo.Init(id);
            itemCount++;
        }

        RectTransform rankParentRect = rankParent.GetComponent<RectTransform>();
        RectTransform cellRect = cellPrefab.GetComponent<RectTransform>();

        if (rankParentRect != null && cellRect != null)
        {
            rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * itemCount);
        }
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    public void OnSelectItem(PopCitySelectPanelCell cellInfo)
    {
        if (lastSelectedCell != null && lastSelectedCell != cellInfo)
        {
            lastSelectedCell.OnSelect(false);
        }

        cellInfo.OnSelect(true);
        lastSelectedCell = cellInfo;
    }

    public void OnShow(List<int> cityIds, System.Action<int> callback)
    {
        Init(cityIds, callback);
    }

    public void OnHide()
    {
    }

    void Update()
    {

    }
}
