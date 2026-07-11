using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GmPanelManager : MonoBehaviour
{
    public Button fightBtn;
    public Button fightQuickBtn;
    public Button beAttackBtn;
    public Button fairBtn;
    public Button closeBtn;
    public Button wallBtn;

    private const float GM_WALL_FULL = 350f;
    private const float GM_WALL_MINIMAL = 30f;

    private bool hasWall = true;
    private float savedWall = -1f;
    private int savedCityId = 0;

    void Start()
    {
        fightBtn.onClick.AddListener(OnFight);
        fightQuickBtn.onClick.AddListener(OnFightQuick);
        beAttackBtn.onClick.AddListener(OnBeAttack);
        fairBtn.onClick.AddListener(OnFair);
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideGmPanel();
        });
        if (wallBtn != null)
        {
            wallBtn.onClick.AddListener(OnWallToggle);
            UpdateWallBtnLabel();
        }
    }

    private void OnWallToggle()
    {
        hasWall = !hasWall;
        UpdateWallBtnLabel();
    }

    private void UpdateWallBtnLabel()
    {
        if (wallBtn == null) return;
        var label = wallBtn.GetComponentInChildren<TMP_Text>();
        if (label != null)
            label.text = hasWall ? "城墙: 开" : "城墙: 关";
    }

    private void TryApplyWall(int cityId)
    {
        var city = GameManager.Instance.GetCity(cityId);
        if (city == null)
        {
            GameLog.Warn($"GmPanelManager.TryApplyWall 找不到城市 cityId={cityId}");
            return;
        }
        savedWall = city.wall;
        savedCityId = cityId;
        float targetWall = hasWall ? GM_WALL_FULL : GM_WALL_MINIMAL;
        city.wall = targetWall;
        GameLog.Info($"GmPanelManager wall cityId={cityId} saved={savedWall} → {targetWall} (hasWall={hasWall})");
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
        attackTroops.Add(new SaveTroopsData{ heroId1 = 101001, armsId = 201 }); // 张飞-枪兵
       // attackTroops.Add(new SaveTroopsData{ heroId1 = 101008, armsId = 101 }); // 赵云-骑兵
       // attackTroops.Add(new SaveTroopsData{ heroId1 = 106001, armsId = 101 }); // 马超-骑兵
       // attackTroops.Add(new SaveTroopsData{ heroId1 = 107002, armsId = 201 }); // 黄忠-弓兵
        // 五子良将
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 105001, armsId = 109 }); // 张辽-骑兵
        defenderTroops.Add(new SaveTroopsData{ heroId1 = 102007, armsId = 201 }); // 徐晃-戟兵
      //  defenderTroops.Add(new SaveTroopsData{ heroId1 = 102009, armsId = 601 }); // 于禁-刀兵
       // defenderTroops.Add(new SaveTroopsData{ heroId1 = 102012, armsId = 602 }); // 乐进-枪兵
      //  defenderTroops.Add(new SaveTroopsData{ heroId1 = 104001, armsId = 201 }); // 张郃-弓兵

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

        TryApplyWall(battleCityId);

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

    private void OnBeAttack()
    {
        var forces = GameManager.Instance.SaveData.forces;
        if (forces.Count < 2)
        {
            GameLog.Error("GmPanelManager.OnBeAttack: 势力数量不足");
            return;
        }

        // 找到玩家势力
        var playerForce = forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce == null)
        {
            GameLog.Error("GmPanelManager.OnBeAttack: 找不到玩家势力");
            return;
        }

        // 找到随机AI势力
        var aiForces = forces.Where(f => !f.isPlayer).ToList();
        if (aiForces.Count == 0)
        {
            GameLog.Error("GmPanelManager.OnBeAttack: 找不到AI势力");
            return;
        }
        var attackerForce = aiForces[SysRandom.Range(0, aiForces.Count)];

        // 随机选AI的一个城市作为攻击源
        var aiCities = GameManager.Instance.GetCitiesByForce(attackerForce.forceId);
        if (aiCities.Count == 0)
        {
            GameLog.Error("GmPanelManager.OnBeAttack: AI势力没有城市");
            return;
        }
        var srcCity = aiCities[SysRandom.Range(0, aiCities.Count)];

        // 获取攻击源城市的英雄
        var heroList = srcCity.GetNormalHeroList();
        if (heroList.Count == 0)
        {
            GameLog.Error($"GmPanelManager.OnBeAttack: AI城市{srcCity.cityId}没有英雄");
            return;
        }

        // 随机选择1-3个英雄作为攻击部队
        int attackHeroCount = Mathf.Min(3, heroList.Count);
        var selectedHeroes = heroList.OrderBy(_ => SysRandom.Value).Take(attackHeroCount).ToArray();

        var attackTroops = new List<SaveTroopsData>();
        var attackSoldierMap = new Dictionary<int, int>();
        foreach (var heroId in selectedHeroes)
        {
            var troop = new SaveTroopsData();
            troop.heroId1 = heroId;
            troop.armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
            attackTroops.Add(troop);
            // 给一个测试用的士兵数
            attackSoldierMap[heroId] = Mathf.Min(50, (int)srcCity.soldier / selectedHeroes.Length);
        }

        // 随机选玩家的一个城市作为目标
        var playerCities = GameManager.Instance.GetCitiesByForce(playerForce.forceId);
        if (playerCities.Count == 0)
        {
            GameLog.Error("GmPanelManager.OnBeAttack: 玩家没有城市");
            return;
        }
        var targetCity = playerCities[SysRandom.Range(0, playerCities.Count)];

        PanelManager.Instance.HideGmPanel();

        TryApplyWall(targetCity.cityId);

        GameManager.Instance.StartTestDefense(attackerForce, targetCity.cityId, new List<int> { srcCity.cityId }, attackTroops, attackSoldierMap);
    }

    private void OnFair()
    {
        PanelManager.Instance.ShowPopFairPanel("forceover", 1);
    }

    public void OnShow()
    {
    }

    public void OnHide()
    {
    }
}
