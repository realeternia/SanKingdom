using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using System.Linq;
using TMPro;

public class RankPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject rankRegionMain;

    public ScrollRect scrollRectMode;
    public GameObject rankRegionMode;
    public GameObject rankCellModePrefab; // RankCellMode预制体引用

    public ScrollRect scrollRectForce;
    public GameObject rankRegionForce;
    public GameObject rankCellForcePrefab; // RankCellForce预制体引用

    public GameObject rankRegionMainHeader;

    public Button closeBtn;

    public int[] mHeroList;

    private IRankDetailInfo lastSelectedHero; // 缓存上次选中的英雄
    private IRankDetailInfoHeader rankHeader;
    private RankCellMode lastSelectedMode; // 缓存上次选中的模式
    private RankCellForce lastSelectedForce; // 缓存上次选中的力量
    private LoopScrollRect loopScrollMain; // 循环列表（仅「全武将」「全城市」模式启用）


    // Start is called before the first frame update
    void Start()
    {
        ConfigManager.Init();

        loopScrollMain = new LoopScrollRect(scrollRectMain);

        // 加载所有英雄配置
        LoadHeroRankings();

        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HideRank();
        });

    }

    public void SortItems(string rankType)
    {
        if (loopScrollMain != null && loopScrollMain.IsInitialized)
        {
            string modeName = lastSelectedMode != null ? lastSelectedMode.modeName.text : "";
            if (modeName == "势力武将" || modeName == "全武将")
            {
                loopScrollMain.SortItems((a, b) =>
                    GetHeroVal(b as SaveHeroData, rankType).CompareTo(GetHeroVal(a as SaveHeroData, rankType)));
            }
            else if (modeName == "势力城市" || modeName == "全城市")
            {
                loopScrollMain.SortItems((a, b) =>
                    GetCityVal(b as SaveCityData, rankType).CompareTo(GetCityVal(a as SaveCityData, rankType)));
            }
            if (lastSelectedHero != null)
            {
                lastSelectedHero.OnSelectHero(false);
            }
            lastSelectedHero = null;
            return;
        }

        List<IRankDetailInfo> cellInfos = new List<IRankDetailInfo>();
        foreach (Transform child in rankRegionMain.transform)
        {
            cellInfos.Add(child.GetComponent<IRankDetailInfo>());
        }

        cellInfos.Sort((a, b) =>
        {
            return b.GetValInt(rankType).CompareTo(a.GetValInt(rankType));
        });

        for(int i = 0; i < cellInfos.Count; i++)
        {
            (cellInfos[i] as MonoBehaviour).gameObject.transform.SetSiblingIndex(i);
        }
        scrollRectMain.normalizedPosition = new Vector2(0, 1);
    }

    private int GetHeroVal(SaveHeroData h, string rankType)
    {
        if (h == null) return 0;
        switch (rankType)
        {
            case "Str": return h.str;
            case "Inte": return h.inte;
            case "LeadShip": return h.leadShip;
            case "Fair": return h.fair;
            case "Charm": return h.charm;
            default: return 0;
        }
    }

    private int GetCityVal(SaveCityData c, string rankType)
    {
        if (c == null) return 0;
        switch (rankType)
        {
            case "Level": return c.GetLevel();
            case "Food": return Mathf.FloorToInt(c.food);
            case "Soldier": return Mathf.FloorToInt(c.soldier);
            case "Wall": return Mathf.FloorToInt(c.wall);
            case "Happy": return Mathf.FloorToInt(c.happy);
            default: return 0;
        }
    }

    // 加载英雄排名
    private void LoadHeroRankings()
    {
        // 清除现有的子物体
        foreach (Transform child in rankRegionMain.transform)
            Destroy(child.gameObject);
        
        if(rankRegionMode.transform.childCount == 0)
        {
            string[] modeNames = {"势力武将", "全武将", "势力战力", "势力城市", "全城市"};
            for(int i = 0; i < modeNames.Length; i++)
            {
                GameObject cell = Instantiate(rankCellModePrefab, rankRegionMode.transform);
                cell.transform.localScale = Vector3.one;
                RankCellMode cellMode = cell.GetComponent<RankCellMode>();
                cellMode.rankPanelManager = this;
                cellMode.Init(modeNames[i]);
            }
            RectTransform rankRect1 = rankRegionMode.GetComponent<RectTransform>();
            RectTransform cellRect1 = rankCellModePrefab.GetComponent<RectTransform>();

            if (rankRect1 != null && cellRect1 != null)
            {
                // Set the height of rankParent based on the number of cells
                rankRect1.sizeDelta = new Vector2(rankRect1.sizeDelta.x, cellRect1.sizeDelta.y * modeNames.Length);
            }
            // 确保scrollRect不为空，然后滚动到最前面
            if (scrollRectMode != null)
            {
                scrollRectMode.normalizedPosition = new Vector2(0, 1);
            }
            
            // 默认选中第一个模式
            if (rankRegionMode.transform.childCount > 0)
            {
                RankCellMode firstMode = rankRegionMode.transform.GetChild(0).GetComponent<RankCellMode>();
                if (firstMode != null)
                {
                    OnSelectMode(firstMode, true);
                }
            }
        }


        if(rankRegionForce.transform.childCount == 0)
        {
            var activeForces = GameManager.Instance.SaveData.forces.Where(f => !f.isEliminated).ToList();
            for(int i = 0; i < activeForces.Count; i++)
            {
                GameObject cell = Instantiate(rankCellForcePrefab, rankRegionForce.transform);
                cell.transform.localScale = Vector3.one;
                RankCellForce cellForce = cell.GetComponent<RankCellForce>();
                cellForce.rankPanelManager = this;
                var forceCfg = ForceConfig.GetConfig(activeForces[i].forceId);
                cellForce.Init(forceCfg.Cname);
            }
            RectTransform rankRect2 = rankRegionForce.GetComponent<RectTransform>();
            RectTransform cellRect2 = rankCellForcePrefab.GetComponent<RectTransform>();

            if (rankRect2 != null && cellRect2 != null)
            {
                rankRect2.sizeDelta = new Vector2(rankRect2.sizeDelta.x, cellRect2.sizeDelta.y * activeForces.Count);
            }
            // 确保scrollRect不为空，然后滚动到最前面
            if (scrollRectForce != null)
            {
                scrollRectForce.normalizedPosition = new Vector2(0, 1);
            }
            
            // 默认选中第一个势力
            if (rankRegionForce.transform.childCount > 0)
            {
                RankCellForce firstForce = rankRegionForce.transform.GetChild(0).GetComponent<RankCellForce>();
                if (firstForce != null)
                {
                    OnSelectForce(firstForce, true);
                }
            }
        }

        // 加载英雄单元格
        LoadHeroCells();
    }
    
    // 加载英雄单元格
    private void LoadHeroCells()
    {
        if(rankHeader != null)
        {
            Destroy((rankHeader as MonoBehaviour).gameObject);
        }

        // 清除现有的子物体
        foreach (Transform child in rankRegionMain.transform)
            Destroy(child.gameObject);

        // 切换前清理循环列表（若启用）
        if (loopScrollMain != null && loopScrollMain.IsInitialized)
        {
            loopScrollMain.Clear();
        }

        string modeName = lastSelectedMode.modeName.text;
        var prefabName = "RankCellMain";
        if(modeName == "势力城市" || modeName == "全城市")
            prefabName = "RankCellMainCity";
        else if(modeName == "势力战力")
            prefabName = "RankCellMainForce"; // 使用城市模板显示势力信息，但会用RankCellInfoForce组件
        else if(modeName == "势力武将" || modeName == "全武将")
            prefabName = "RankCellMain"; // 显示武将列表

        // 实例化RankCellInfoHeader
        var rankCellInfoHeaderPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelListItem(prefabName));
        var obj = Instantiate(rankCellInfoHeaderPrefab, rankRegionMainHeader.transform);
        var newHeader = obj.GetComponent<IRankDetailInfoHeader>();
        newHeader.SetManager(this);
        newHeader.SetMode(true);
        rankHeader = newHeader;
        
        var rankCellInfoPrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.PanelListItem(prefabName));
        float cellHeight = rankCellInfoPrefab.GetComponent<RectTransform>().sizeDelta.y;

        int count = 0;
        if (prefabName == "RankCellMain")
        {
            var heroes = (modeName == "全武将")
                ? GameManager.Instance.SaveData.heros
                : GameManager.Instance.SaveData.heros.Where(h => h.forceId == lastSelectedForce.forceId).ToList();

            List<int> heroList = new List<int>();
            foreach (var heroData in heroes)
            {
                heroList.Add(heroData.heroId);
            }
            mHeroList = heroList.ToArray();

            // 势力武将 / 全武将 都走循环列表
            List<object> dataSource = heroes.Cast<object>().ToList();
            loopScrollMain.Initialize(dataSource, rankCellInfoPrefab, cellHeight, (cell) =>
            {
                if (cell is RankCellInfo info) info.SetManager(this);
            });
            count = loopScrollMain.GetTotalCount();
        }
        else if (prefabName == "RankCellMainCity")
        {
            var cities = (modeName == "全城市")
                ? GameManager.Instance.SaveData.cities
                : GameManager.Instance.GetCitiesByForce(lastSelectedForce.forceId);

            // 过滤掉 WorldConfig 中不存在的城市，保留原"势力城市"模式的安全性
            var validCities = cities.Where(c => WorldConfig.GetConfig(c.cityId) != null).ToList();

            // 势力城市 / 全城市 都走循环列表
            List<object> dataSource = validCities.Cast<object>().ToList();
            loopScrollMain.Initialize(dataSource, rankCellInfoPrefab, cellHeight, (cell) =>
            {
                if (cell is RankCellInfoCity cityInfo) cityInfo.SetManager(this);
            });
            count = loopScrollMain.GetTotalCount();
        }
        // 新增势力战力模式的处理
        else if (prefabName == "RankCellMainForce")
        {
            // 显示所有势力信息（跳过灭亡或没有城市的势力）
            foreach (var forceData in GameManager.Instance.SaveData.forces)
            {
                if (forceData.isEliminated || GameManager.Instance.GetCitiesByForce(forceData.forceId).Count == 0)
                    continue;
                    
                GameObject cell = Instantiate(rankCellInfoPrefab, rankRegionMain.transform);
                cell.transform.localScale = Vector3.one;

                RankCellInfoForce cellInfo = cell.GetComponent<RankCellInfoForce>();
                cellInfo.rankPanelManager = this;
                cellInfo.SetMode(false);
                cellInfo.Init(forceData.forceId);
                count++;
            }
        }

        // 循环列表模式已自行设置 sizeDelta，跳过手动高度计算
        if (loopScrollMain == null || !loopScrollMain.IsInitialized)
        {
            RectTransform rankParentRect = rankRegionMain.GetComponent<RectTransform>();
            RectTransform cellRect = rankCellInfoPrefab.GetComponent<RectTransform>();
            if (rankParentRect != null && cellRect != null)
            {
                rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * count);
            }
        }

        if (scrollRectMain != null)
        {
            scrollRectMain.normalizedPosition = new Vector2(0, 1);
        }

    }

    public void OnSelectHero(IRankDetailInfo cellInfo)
    {
        // 取消上次选中的英雄
        if (lastSelectedHero != null && lastSelectedHero != cellInfo)
        {
            lastSelectedHero.OnSelectHero(false);
        }
        
        // 选中当前英雄
        cellInfo.OnSelectHero(true);
        
        // 更新缓存的上次选中英雄
        lastSelectedHero = cellInfo;
    }

    public void OnSelectMode(RankCellMode cellMode, bool init = false)
    {
        // 取消上次选中的模式
        if (lastSelectedMode != null && lastSelectedMode != cellMode)
        {
            lastSelectedMode.SetSelected(false);
        }
        
        // 选中当前模式
        cellMode.SetSelected(true);
        
        // 更新缓存的上次选中模式
        lastSelectedMode = cellMode;

        // "全武将" / "全城市" / "势力战力" 不需要按势力过滤，隐藏势力选择面板
        UpdateForcePanelVisibility();
        
        // 重新加载英雄单元格
        if (!init)
            LoadHeroCells();
    }

    private void UpdateForcePanelVisibility()
    {
        if (lastSelectedMode == null) return;
        string modeName = lastSelectedMode.modeName.text;
        bool showForcePanel = (modeName == "势力武将" || modeName == "势力城市");
        scrollRectForce.gameObject.SetActive(showForcePanel);
    }

    public void OnSelectForce(RankCellForce cellForce, bool init = false)
    {
        // 取消上次选中的势力
        if (lastSelectedForce != null && lastSelectedForce != cellForce)
        {
            lastSelectedForce.SetSelected(false);
        }
        
        // 选中当前势力
        cellForce.SetSelected(true);
        
        // 更新缓存的上次选中势力
        lastSelectedForce = cellForce;
        
        // 重新加载英雄单元格
        if (!init)
            LoadHeroCells();
    }

    public void OnShow()
    {

    }

    public void OnHide()
    {
        if (loopScrollMain != null && loopScrollMain.IsInitialized)
        {
            loopScrollMain.Clear();
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
