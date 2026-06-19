using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Linq;
using System;

public class CityBattlePanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject itemRegionMain;

    private int forceId;
    private int selectedCityId;
    public Button destButton;
    public TMP_Text attrVal1Text;

    public Button closeButton;
    public Button battleButton;

    private List<GameObject> cityBattleItems = new List<GameObject>();

    private static Dictionary<int, int> heroSoldierAllocations = new Dictionary<int, int>();

    // 防御模式相关字段
    private bool isDefenseMode = false;
    private int defenseTargetCityId;
    private List<int> defenseSrcCityIds;
    private List<SaveTroopsData> defenseAttackTroops;
    private Dictionary<int, int> defenseAttackSoldierMap;

    public static void SetAllocatedSoldier(int heroId, int soldier)
    {
        heroSoldierAllocations[heroId] = soldier;
    }

    public static int GetAllocatedSoldier(int heroId)
    {
        if (heroSoldierAllocations.ContainsKey(heroId))
            return heroSoldierAllocations[heroId];
        return 0;
    }

    public static void ClearAllocations()
    {
        heroSoldierAllocations.Clear();
    }

    public static Dictionary<int, int> GetAllocations()
    {
        return heroSoldierAllocations;
    }

    void Start()
    {
        closeButton.onClick.AddListener(() =>
        {
            if (isDefenseMode) return;
            PanelManager.Instance.HideCityBattle();
        });
        destButton.onClick.AddListener(() =>
        {
            var cityIds = MapTool.GetAdjacentEnemyCityIds(forceId);
            SideCitySelector.SetContext(selectedCityId, cityIds, (newCityId) =>
            {
                selectedCityId = newCityId;
                if (newCityId == 0)
                {
                    attrVal1Text.text = "-";
                }
                else
                {
                    var cityCfg = WorldConfig.GetConfig(newCityId);
                    attrVal1Text.text = cityCfg.Cname;
                }
                ClearAllSelections();
                CreateCityBattleItems(forceId);
            });
            PanelManager.Instance.ShowSideBar("SideCitySelector");
        });

        if (battleButton != null)
        {
            battleButton.onClick.AddListener(OnBattle);
        }
    }

    void Update()
    {

    }

    public void Init(int forceId)
    {
        this.forceId = forceId;
        this.isDefenseMode = false;
        heroSoldierAllocations.Clear();
        destButton.gameObject.SetActive(true);
        if (battleButton != null)
        {
            var label = battleButton.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = "出征";
        }
        CreateCityBattleItems(forceId);
    }

    public void InitDefense(int forceId, int targetCityId, List<int> srcCityIds, List<SaveTroopsData> attackTroops, Dictionary<int, int> attackSoldierMap)
    {
        this.forceId = forceId;
        this.isDefenseMode = true;
        this.defenseTargetCityId = targetCityId;
        this.defenseSrcCityIds = srcCityIds;
        this.defenseAttackTroops = attackTroops;
        this.defenseAttackSoldierMap = attackSoldierMap;
        this.selectedCityId = targetCityId;
        heroSoldierAllocations.Clear();

        // 防御模式下隐藏目标选择按钮，显示当前防守城市名
        destButton.gameObject.SetActive(false);
        var cityCfg = WorldConfig.GetConfig(targetCityId);
        attrVal1Text.text = cityCfg.Cname;

        if (battleButton != null)
        {
            var label = battleButton.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = "防御";
        }

        CreateCityBattleItems(forceId);
    }

    public bool CanSelectItem()
    {
        if (isDefenseMode)
            return true;
        return selectedCityId > 0;
    }

    private void ClearAllSelections()
    {
        foreach (var itemObj in cityBattleItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<CityBattleItem>();
            if (itemScript != null)
            {
                itemScript.SetSelected(false);
            }
        }
    }

    private List<CityBattleItem> GetSelectedItems()
    {
        List<CityBattleItem> selected = new List<CityBattleItem>();
        foreach (var itemObj in cityBattleItems)
        {
            if (itemObj == null) continue;
            var itemScript = itemObj.GetComponent<CityBattleItem>();
            if (itemScript != null && itemScript.IsSelected())
            {
                selected.Add(itemScript);
            }
        }
        return selected;
    }

    private void OnBattle()
    {
        if (isDefenseMode)
        {
            OnDefenseBattle();
            return;
        }

        if (selectedCityId <= 0)
        {
            SystemTip.Instance.ShowTip("请选择目标城市");
            return;
        }

        var selectedItems = GetSelectedItems();
        if (selectedItems.Count == 0)
        {
            SystemTip.Instance.ShowTip("请选择出战部队");
            return;
        }

        List<SaveTroopsData> attackTroops = new List<SaveTroopsData>();
        foreach (var item in selectedItems)
        {
            var troop = item.GetWarTeamData();

            if (troop.heroId1 > 0 && GetAllocatedSoldier(troop.heroId1) > 0)
            {
                attackTroops.Add(troop);
            }
        }

        if (attackTroops.Count == 0)
        {
            SystemTip.Instance.ShowTip("出战部队士兵数不能为0");
            return;
        }

        PanelManager.Instance.ShowPopResultPanel("出征", new List<PopResultPanelManager.AttrData>(), () =>
        {
            OnRun(selectedCityId, attackTroops);
        }, "atk2.mp4");
    }

    private void OnDefenseBattle()
    {
        var cityData = GameManager.Instance.GetCity(defenseTargetCityId);
        int totalSoldiers = (int)cityData.soldier;
        int totalAllocated = 0;

        List<SaveTroopsData> defenceTroops = new List<SaveTroopsData>();
        foreach (var itemObj in cityBattleItems)
        {
            if (itemObj == null) continue;
            var item = itemObj.GetComponent<CityBattleItem>();
            if (item == null) continue;
            var troop = item.GetWarTeamData();
            if (troop.heroId1 > 0)
            {
                int allocated = GetAllocatedSoldier(troop.heroId1);
                if (allocated > 0)
                {
                    defenceTroops.Add(troop);
                    totalAllocated += allocated;
                }
            }
        }

        if (defenceTroops.Count == 0)
        {
            SystemTip.Instance.ShowTip("请为部队分配兵力");
            return;
        }

        if (totalAllocated < totalSoldiers)
        {
            SystemTip.Instance.ShowTip($"防御模式必须配满兵力，当前分配{totalAllocated}，总兵力{totalSoldiers}");
            return;
        }

        // 防御模式下所有出场部队的士兵数已在分配时确定，无需再选择
        Dictionary<int, int> defenceSoldierMap = new Dictionary<int, int>();
        foreach (var troop in defenceTroops)
        {
            defenceSoldierMap[troop.heroId1] = heroSoldierAllocations[troop.heroId1];
        }

        PanelManager.Instance.HideCityBattle();
        heroSoldierAllocations.Clear();

        GameManager.Instance.OnDefenseConfirmed(defenceTroops, defenceSoldierMap);
    }

    private void OnRun(int targetCityId, List<SaveTroopsData> attackTroops)
    {
        var sourceCityIds = attackTroops
            .Where(t => t.heroId1 > 0)
            .Select(t => GameManager.Instance.GetHero(t.heroId1).cityId)
            .Distinct()
            .ToList();

        Dictionary<int, int> attackSoldierMap = new Dictionary<int, int>();
        foreach (var troop in attackTroops)
        {
            if (troop.heroId1 > 0 && heroSoldierAllocations.ContainsKey(troop.heroId1))
            {
                attackSoldierMap[troop.heroId1] = heroSoldierAllocations[troop.heroId1];
            }
        }

        var force = GameManager.Instance.GetForce(forceId);

        PanelManager.Instance.HideCityBattle();
        heroSoldierAllocations.Clear();
        force.ExecuteBattle(sourceCityIds, attackTroops, attackSoldierMap, targetCityId, false);
    }

    private void CreateCityBattleItems(int forceId)
    {
        foreach (var item in cityBattleItems)
        {
            if (item != null)
                Destroy(item);
        }
        cityBattleItems.Clear();

        List<SaveTroopsData> allTeams = new List<SaveTroopsData>();
        List<int> assignedHeroIds = new List<int>();

        if (isDefenseMode)
        {
            // 防御模式：只显示目标城市的部队
            var city = GameManager.Instance.GetCity(defenseTargetCityId);
            foreach (var troop in SaveTroopsData.GetTroopsByCity(city.cityId))
            {
                if (troop.heroId1 > 0)
                {
                    allTeams.Add(troop);
                    assignedHeroIds.Add(troop.heroId1);
                    if (troop.heroId2 > 0) assignedHeroIds.Add(troop.heroId2);
                    if (troop.heroId3 > 0) assignedHeroIds.Add(troop.heroId3);
                }
            }
        }
        else
        {
            // 进攻模式：显示相邻城市的部队
            var cities = GameManager.Instance.GetCitiesByForce(forceId);

            HashSet<int> adjacentCityIds = null;
            if (selectedCityId > 0)
            {
                adjacentCityIds = new HashSet<int>(MapTool.GetAdjacentCityIds(selectedCityId));
            }

            foreach (var city in cities)
            {
                if (adjacentCityIds != null && !adjacentCityIds.Contains(city.cityId))
                    continue;

                foreach (var troop in SaveTroopsData.GetTroopsByCity(city.cityId))
                {
                    if (troop.heroId1 > 0)
                    {
                        allTeams.Add(troop);
                        assignedHeroIds.Add(troop.heroId1);
                        if (troop.heroId2 > 0) assignedHeroIds.Add(troop.heroId2);
                        if (troop.heroId3 > 0) assignedHeroIds.Add(troop.heroId3);
                    }
                }
            }
        }

        // 添加非军团内的hero作为临时动员兵军团
        List<SaveCityData> relevantCities;
        if (isDefenseMode)
        {
            relevantCities = new List<SaveCityData> { GameManager.Instance.GetCity(defenseTargetCityId) };
        }
        else
        {
            HashSet<int> adjacentCityIds = null;
            if (selectedCityId > 0)
            {
                adjacentCityIds = new HashSet<int>(MapTool.GetAdjacentCityIds(selectedCityId));
            }
            relevantCities = GameManager.Instance.GetCitiesByForce(forceId)
                .Where(c => adjacentCityIds == null || adjacentCityIds.Contains(c.cityId))
                .ToList();
        }

        foreach (var city in relevantCities)
        {
            var heroes = city.GetNormalHeroList();
            foreach (var heroId in heroes)
            {
                if (assignedHeroIds.Contains(heroId))
                    continue;
                var hero = GameManager.Instance.GetHero(heroId);
                if (hero == null) continue;

                var tempTroop = new SaveTroopsData();
                tempTroop.heroId1 = heroId;
                tempTroop.armsId = SystemConst.Hero.DEFAULT_ARMS_ID;
                tempTroop.cityId = city.cityId;
                tempTroop.forceId = forceId;
                allTeams.Add(tempTroop);
                assignedHeroIds.Add(heroId);
            }
        }

        if (allTeams.Count == 0) return;

        heroSoldierAllocations.Clear();

        var itemPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelListItem("CityBattleItem"));

        RectTransform containerRect = itemRegionMain.GetComponent<RectTransform>();
        if (containerRect == null) return;

        float itemWidth = 750f;
        float itemHeight = 120f;
        float spacing = 10f;
        int itemsPerRow = 2;

        float totalWidth = itemsPerRow * itemWidth + (itemsPerRow - 1) * spacing;
        float startX = -totalWidth / 2f + itemWidth / 2f;

        for (int i = 0; i < allTeams.Count; i++)
        {
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;

            float posX = startX + col * (itemWidth + spacing);
            float posY = -row * (itemHeight + spacing);

            GameObject itemObj = Instantiate(itemPrefab, itemRegionMain.transform);
            itemObj.transform.localScale = Vector3.one;

            RectTransform rectTransform = itemObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(0.5f, 1f);
                rectTransform.anchorMax = new Vector2(0.5f, 1f);
                rectTransform.pivot = new Vector2(0.5f, 1f);
                rectTransform.anchoredPosition = new Vector2(posX, posY);
                rectTransform.sizeDelta = new Vector2(itemWidth, itemHeight);
            }

            CityBattleItem itemScript = itemObj.GetComponent<CityBattleItem>();
            if (itemScript != null)
            {
                itemScript.Init(allTeams[i]);
            }

            cityBattleItems.Add(itemObj);
        }

        int totalRows = (allTeams.Count + itemsPerRow - 1) / itemsPerRow;
        float contentHeight = totalRows * itemHeight + (totalRows - 1) * spacing + 20f;
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, contentHeight);
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
        heroSoldierAllocations.Clear();
    }
}
