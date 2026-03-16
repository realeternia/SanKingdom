using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class RankPanelManager : MonoBehaviour
{
    public ScrollRect scrollRectMain;
    public GameObject rankRegionMain;
    public GameObject rankCellPrefab; // RankCell预制体引用

    public ScrollRect scrollRectMode;
    public GameObject rankRegionMode;
    public GameObject rankCellModePrefab; // RankCellMode预制体引用

    public ScrollRect scrollRectForce;
    public GameObject rankRegionForce;
    public GameObject rankCellForcePrefab; // RankCellForce预制体引用

    public GameObject rankRegionMainHeader;

    public Button closeBtn;

    private IRankDetailInfo lastSelectedHero; // 缓存上次选中的英雄
    private IRankDetailInfoHeader rankHeader;
    private RankCellMode lastSelectedMode; // 缓存上次选中的模式
    private RankCellForce lastSelectedForce; // 缓存上次选中的力量


    // Start is called before the first frame update
    void Start()
    {
        ConfigManager.Init();


        // 加载所有英雄配置
        LoadHeroRankings();

        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideRank();
        });

    }

    public void SortItems(string rankType)
    {
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

    // 加载英雄排名
    private void LoadHeroRankings()
    {
        // 清除现有的子物体
        foreach (Transform child in rankRegionMain.transform)
            Destroy(child.gameObject);
        
        if(rankRegionMode.transform.childCount == 0)
        {
            string[] modeNames = {"势力武将", "势力战力", "势力城市"};
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
            for(int i = 0; i < GameManager.Instance.SaveData.forces.Count; i++)
            {
                GameObject cell = Instantiate(rankCellForcePrefab, rankRegionForce.transform);
                cell.transform.localScale = Vector3.one;
                RankCellForce cellForce = cell.GetComponent<RankCellForce>();
                cellForce.rankPanelManager = this;
                var forceCfg = ForceConfig.GetConfig(GameManager.Instance.SaveData.forces[i].forceId);
                cellForce.Init(forceCfg.Cname);
            }
            RectTransform rankRect2 = rankRegionForce.GetComponent<RectTransform>();
            RectTransform cellRect2 = rankCellForcePrefab.GetComponent<RectTransform>();

            if (rankRect2 != null && cellRect2 != null)
            {
                // Set the height of rankParent based on the number of cells
                rankRect2.sizeDelta = new Vector2(rankRect2.sizeDelta.x, cellRect2.sizeDelta.y * GameManager.Instance.SaveData.forces.Count);
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

        // 实例化RankCellInfoHeader
        var rankCellInfoHeaderPrefab = Resources.Load<GameObject>("Prefabs/Panels/RankCellMainHeader");
        var obj = Instantiate(rankCellInfoHeaderPrefab, rankRegionMainHeader.transform);
        var newHeader = obj.GetComponent<RankCellInfoHeader>();
        newHeader.rankPanelManager = this;

        rankHeader = newHeader;
        
        // 获取所有英雄配置
        var heroConfigs = HeroConfig.ConfigList;

        // 为每个英雄配置创建一个RankCell
        int count = 0;
        foreach (var heroConfig in heroConfigs)
        {
            var heroData = GameManager.Instance.GetHero(heroConfig.Id);
            if (heroData == null)
                continue;
            var cityData = GameManager.Instance.GetCity(heroData.cityId);
            if (heroData.state != HeroState.Normal || cityData.forceId != lastSelectedForce.forceId)
                continue;

            // 实例化RankCell
            GameObject cell = Instantiate(rankCellPrefab, rankRegionMain.transform);
            cell.transform.localScale = Vector3.one;

            // 获取RankCellInfo组件
            RankCellInfo cellInfo = cell.GetComponent<RankCellInfo>();
            cellInfo.rankPanelManager = this;
            if (cellInfo != null)
                cellInfo.Init(heroConfig);
            count++;
        }
        // Get the RectTransform components
         RectTransform rankParentRect = rankRegionMain.GetComponent<RectTransform>();
         RectTransform cellRect = rankCellPrefab.GetComponent<RectTransform>();
           
         if (rankParentRect != null && cellRect != null)
         {
             // Set the height of rankParent based on the number of cells
             rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * count);
         }
        // 确保scrollRect不为空，然后滚动到最前面
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
        
        // 重新加载英雄单元格
        if (!init)
            LoadHeroCells();
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
    }


    // Update is called once per frame
    void Update()
    {

    }
}
