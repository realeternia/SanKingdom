using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System.Linq;

public class ReplayPanelManager : MonoBehaviour
{
    public ScrollRect replayRect;
    public GameObject replayRegion;

    public GameObject cellPrefab;

    public Button closeBtn;


    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideReplayPanel();
        });
    }


    public void OnShow()
    {
        ClearCells();

        var battleRecords = GameManager.Instance.SaveData.battleStatManager.battleRecords;
        var orderedRecords = battleRecords.OrderByDescending(r => r.battleId).ToList();

        for (int i = 0; i < orderedRecords.Count; i++)
        {
            var cell = Instantiate(cellPrefab, replayRegion.transform);
            var control = cell.GetComponent<ReplayCellControl>();
            if (control != null)
                control.SetData(orderedRecords[i]);
        }

        UpdateScrollRectSize(orderedRecords.Count);
    }

    private void UpdateScrollRectSize(int cellCount)
    {
        RectTransform rankRect = replayRegion.GetComponent<RectTransform>();
        RectTransform cellRect = cellPrefab.GetComponent<RectTransform>();

        if (rankRect != null && cellRect != null)
        {
            rankRect.sizeDelta = new Vector2(rankRect.sizeDelta.x, cellRect.sizeDelta.y * cellCount);
        }

        if (replayRect != null)
        {
            replayRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    private void ClearCells()
    {
        foreach (Transform child in replayRegion.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnHide()
    {
        ClearCells();
    }
}
