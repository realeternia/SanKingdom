using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System.Linq;

public class BattleResultPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectLeft;
    public GameObject rankRegionLeft;

    public ScrollRect scrollRectRight;
    public GameObject rankRegionRight;

    public Button closeBtn;
    public Button retryBtn;

    public TMP_Text textTitle;
    public TMP_Text textForceLeft;
    public TMP_Text textSoldierLeft;
    public TMP_Text textFoodLeft;
    public TMP_Text textForceRight;
    public TMP_Text textSoldierRight;
    public TMP_Text textFoodRight;    

    public GameObject cellPrefab;
    
    private int currentBattleId;


    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideBattleResultPanel();
        });
        retryBtn.onClick.AddListener(OnRetry);
    }


    public void OnShow(int battleId)
    {
        gameObject.SetActive(true);
        currentBattleId = battleId;
        
        var record = GameManager.Instance.SaveData.battleStatManager.GetBattleRecord(battleId);
        if (record == null)
            return;

        var cityCfg = WorldConfig.GetConfig(record.cityId);
        textTitle.text = (cityCfg != null ? cityCfg.Cname : "未知") + "之战 (" + record.rounds + "回合)";

        var force1 = GameManager.Instance.GetPlayer(record.forceId1);
        var force2 = GameManager.Instance.GetPlayer(record.forceId2);
        
        string forceName1 = force1 != null ? force1.pname : "势力" + record.forceId1;
        string forceName2 = force2 != null ? force2.pname : "势力" + record.forceId2;
        
        if (record.result == BattleResult.Win)
        {
            textForceLeft.text = forceName1 + "<color=red>胜利</color>";
            textForceRight.text = forceName2 + "<color=green>战败</color>";
        }
        else if (record.result == BattleResult.Lose)
        {
            textForceLeft.text = forceName1 + "<color=green>战败</color>";
            textForceRight.text = forceName2 + "<color=red>胜利</color>";
        }
        else
        {
            textForceLeft.text = forceName1 + "<color=yellow>平局</color>";
            textForceRight.text = forceName2 + "<color=yellow>平局</color>";
        }
        
        textSoldierLeft.text = "损失: " + record.soldierLoss1;
        textSoldierRight.text = "损失: " + record.soldierLoss2;
        
        textFoodLeft.text = "粮耗: " + record.foodCost1;
        textFoodRight.text = "粮耗: " + record.foodCost2;

        ClearCells(rankRegionLeft);
        ClearCells(rankRegionRight);

        var leftStats = record.battleStats.Where(s => s.forceId == record.forceId1).OrderByDescending(s => s.damage).ToList();
        var rightStats = record.battleStats.Where(s => s.forceId == record.forceId2).OrderByDescending(s => s.damage).ToList();

        for (int i = 0; i < leftStats.Count; i++)
        {
            var cell = Instantiate(cellPrefab, rankRegionLeft.transform);
            var control = cell.GetComponent<BattleResultHeroCellControl>();
            if (control != null)
                control.SetData(leftStats[i], i + 1);
        }

        for (int i = 0; i < rightStats.Count; i++)
        {
            var cell = Instantiate(cellPrefab, rankRegionRight.transform);
            var control = cell.GetComponent<BattleResultHeroCellControl>();
            if (control != null)
                control.SetData(rightStats[i], i + 1);
        }

        UpdateScrollRectSize(rankRegionLeft, scrollRectLeft, leftStats.Count);
        UpdateScrollRectSize(rankRegionRight, scrollRectRight, rightStats.Count);
    }

    private void UpdateScrollRectSize(GameObject rankRegion, ScrollRect scrollRect, int cellCount)
    {
        RectTransform rankRect = rankRegion.GetComponent<RectTransform>();
        RectTransform cellRect = cellPrefab.GetComponent<RectTransform>();

        if (rankRect != null && cellRect != null)
        {
            rankRect.sizeDelta = new Vector2(rankRect.sizeDelta.x, cellRect.sizeDelta.y * cellCount);
        }
        
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    private void ClearCells(GameObject region)
    {
        foreach (Transform child in region.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnHide()
    {
    }

    public void OnRetry()
    {
        PanelManager.Instance.HideBattleResultPanel();
        PanelManager.Instance.HideWorld();
        BattleManager.Instance.SetMode(false, true);
        BattleManager.Instance.ReplayBattle(currentBattleId);
    }
}
