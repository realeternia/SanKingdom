using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

public class GmPanelManager : MonoBehaviour
{
    public Button fightBtn;
    public Button fightQuickBtn;
    public Button closeBtn;

    void Start()
    {
        fightBtn.onClick.AddListener(OnFight);
        fightQuickBtn.onClick.AddListener(OnFightQuick);
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideGmPanel();
        });
    }

    private void StartTestBattle(bool quickMode, bool showUI)
    {
        var forces = GameManager.Instance.SaveData.forces;
        if (forces.Count < 2)
        {
            GameLog.Error("GmPanelManager.OnForce: 势力数量不足2，无法开始测试战斗");
            return;
        }

        var force1 = forces[0];
        var force2 = forces[1];

        var attackTroops = new List<SaveTroopsData>();
        var defenderTroops = new List<SaveTroopsData>();

        // 五虎上将
        attackTroops.Add(new SaveTroopsData{ heroId1 = 101002, armsId = 101 }); // 关羽-骑兵
        attackTroops.Add(new SaveTroopsData{ heroId1 = 101001, armsId = 602 }); // 张飞-枪兵
        attackTroops.Add(new SaveTroopsData{ heroId1 = 101008, armsId = 101 }); // 赵云-骑兵
        attackTroops.Add(new SaveTroopsData{ heroId1 = 106001, armsId = 101 }); // 马超-骑兵
        attackTroops.Add(new SaveTroopsData{ heroId1 = 107002, armsId = 201 }); // 黄忠-弓兵
        // 五子良将
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 105001, armsId = 101 }); // 张辽-骑兵
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 102007, armsId = 603 }); // 徐晃-戟兵
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 102009, armsId = 601 }); // 于禁-刀兵
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 102012, armsId = 602 }); // 乐进-枪兵
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 104001, armsId = 201 }); // 张郃-弓兵

        var attackSoldierMap = new Dictionary<int, int>();
        foreach (var troop in attackTroops)
        {
            attackSoldierMap[troop.heroId1] = SystemConst.Hero.MAX_SOLDIER_PER_HERO;
        }

        var defenderSoldierMap = new Dictionary<int, int>();
        foreach (var troop in defenderTroops)
        {
            defenderSoldierMap[troop.heroId1] = SystemConst.Hero.MAX_SOLDIER_PER_HERO;
        }

        BattleManager.Instance.SetMode(quickMode, showUI);

        var firstCity = GameManager.Instance.SaveData.cities.FirstOrDefault();
        int battleCityId = firstCity != null ? firstCity.cityId : 0;

        BattleManager.Instance.BattleBegin(force1, force2, attackTroops, defenderTroops, attackSoldierMap, defenderSoldierMap, battleCityId);

        PanelManager.Instance.HideGmPanel();
    }

    private void OnFight()
    {
        StartTestBattle(false, true);
    }

    private void OnFightQuick()
    {
        StartTestBattle(true, false);
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }
}
