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
    public TMP_Text timeText;
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
        timeText.text = GetTimeString(record.year);

        var force1 = GameManager.Instance.GetForce(record.forceId1);
        var force2 = GameManager.Instance.GetForce(record.forceId2);

        string forceName1 = force1 != null ? force1.Name : ForceConfig.GetConfig(record.forceId1)?.Cname ?? "势力" + record.forceId1;
        string forceName2 = force2 != null ? force2.Name : ForceConfig.GetConfig(record.forceId2)?.Cname ?? "势力" + record.forceId2;

        force1Name.text = forceName1;
        force2Name.text = forceName2;

        var forceCfg1 = ForceConfig.GetConfig(record.forceId1);
        var forceCfg2 = ForceConfig.GetConfig(record.forceId2);
        if (forceCfg1 != null)
        {
            var heroCfg1 = HeroConfig.GetConfig(forceCfg1.HeroId);
            var sprite1 = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg1.Icon));
            if (sprite1 != null)
                force1Icon.sprite = sprite1;
            force1Name.color = SysColor.GetForceColor(record.forceId1);
        }
        if (forceCfg2 != null)
        {
            var heroCfg2 = HeroConfig.GetConfig(forceCfg2.HeroId);
            var sprite2 = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg2.Icon));
            if (sprite2 != null)
                force2Icon.sprite = sprite2;
            force2Name.color = SysColor.GetForceColor(record.forceId2);
        }

        if (record.result == BattleResult.Win)
        {
            resultText.text = "攻城成功";
            resultText.color = SysColor.Battle.AttackSuccessColor;
        }
        else
        {
            resultText.text = "攻城失败";
            resultText.color = SysColor.Battle.AttackFailColor;
        }
    }

    private string GetTimeString(int round)
    {
        int year = SystemConst.Game.BASE_YEAR + round / SystemConst.Game.SEASONS_PER_YEAR;
        var seasonCfg = SeasonConfig.GetConfig(round % SystemConst.Game.SEASONS_PER_YEAR);
        return string.Format("{0}年{1}", year, seasonCfg.Name);
    }
}
