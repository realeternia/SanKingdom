using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;

public class GmPanelManager : MonoBehaviour
{
    public Button fightBtn;
    public Button closeBtn;

    void Start()
    {
        fightBtn.onClick.AddListener(OnFight);
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideGmPanel();
        });
    }

    private void OnFight()
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

        attackTroops.Add(new SaveTroopsData{ heroId1 = 100001, armsId = SystemConst.Hero.DEFAULT_ARMS_ID });
        attackTroops.Add(new SaveTroopsData{ heroId1 = 101001, armsId = SystemConst.Hero.DEFAULT_ARMS_ID });
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 100002, armsId = SystemConst.Hero.DEFAULT_ARMS_ID });
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 102002, armsId = SystemConst.Hero.DEFAULT_ARMS_ID });

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

        BattleManager.Instance.SetMode(false, true);

        var firstCity = GameManager.Instance.SaveData.cities.FirstOrDefault();
        int battleCityId = firstCity != null ? firstCity.cityId : 0;

        BattleManager.Instance.BattleBegin(force1, force2, attackTroops, defenderTroops, attackSoldierMap, defenderSoldierMap, battleCityId);

        PanelManager.Instance.HideGmPanel();
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }
}
