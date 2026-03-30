using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class ReplayCellControl : MonoBehaviour
{
    public Image force1Icon;
    public Image force2Icon;

    public TMP_Text force1Name;
    public TMP_Text force2Name;

    public TMP_Text battleName;
    public TMP_Text roundPast;

    public TMP_Text resultText;

    public Button replayBtn;

    private int currentBattleId;


    void Start()
    {
        replayBtn.onClick.AddListener(OnReplayClick);
    }

    void OnReplayClick()
    {
        if (currentBattleId > 0)
        {
            PanelManager.Instance.HideReplayPanel();
            PanelManager.Instance.HideWorld();
            BattleManager.Instance.SetMode(false, true);
            BattleManager.Instance.ReplayBattle(currentBattleId);
        }
    }

    public void SetData(BattleStatManager.BattleRecord record)
    {
        if (record == null)
            return;

        currentBattleId = record.battleId;

        var cityCfg = WorldConfig.GetConfig(record.cityId);
        battleName.text = (cityCfg != null ? cityCfg.Cname : "未知") + "之战";
        roundPast.text = record.rounds + "回合";

        var force1 = GameManager.Instance.GetPlayer(record.forceId1);
        var force2 = GameManager.Instance.GetPlayer(record.forceId2);

        string forceName1 = force1 != null ? force1.pname : ForceConfig.GetConfig(record.forceId1)?.Cname ?? "势力" + record.forceId1;
        string forceName2 = force2 != null ? force2.pname : ForceConfig.GetConfig(record.forceId2)?.Cname ?? "势力" + record.forceId2;

        force1Name.text = forceName1;
        force2Name.text = forceName2;

        if (record.result == BattleResult.Win)
        {
            resultText.text = forceName1 + " 胜利";
            resultText.color = Color.red;
        }
        else if (record.result == BattleResult.Lose)
        {
            resultText.text = forceName2 + " 胜利";
            resultText.color = Color.red;
        }
        else
        {
            resultText.text = "平局";
            resultText.color = Color.yellow;
        }
    }
}
