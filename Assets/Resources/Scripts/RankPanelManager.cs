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

    public Button btnLeadShip;
    public Button btnStr;
    public Button btnInte;
    public Button btnFair;
    public Button btnCharm;

    public Button closeBtn;

    private RankCellInfo lastSelectedHero; // 缓存上次选中的英雄
    private RankCellMode lastSelectedMode; // 缓存上次选中的模式
    private RankCellForce lastSelectedForce; // 缓存上次选中的力量


    // Start is called before the first frame update
    void Start()
    {
        ConfigManager.Init();


        // 加载所有英雄配置
        LoadHeroRankings();

        btnLeadShip.onClick.AddListener(() =>
        {
            SortItems("LeadShip");
        });
        btnStr.onClick.AddListener(() =>
        {
            SortItems("Str");
        });
        btnInte.onClick.AddListener(() =>
        {
            SortItems("Inte");
        });
        btnFair.onClick.AddListener(() =>
        {
            SortItems("Fair");
        });
        btnCharm.onClick.AddListener(() =>
        {
            SortItems("Charm");
        });
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideRank();
          //  CardShopManager.Instance.OnShow();
        });

    }

    private void SortItems(string rankType)
    {
        List<RankCellInfo> cellInfos = new List<RankCellInfo>();
        foreach (Transform child in rankRegionMain.transform)
        {
            cellInfos.Add(child.GetComponent<RankCellInfo>());
        }

        cellInfos.Sort((a, b) =>
        {
            if(rankType == "LeadShip")
                return b.leadShip.CompareTo(a.leadShip);
            else if(rankType == "Str")
                return b.str.CompareTo(a.str);
            else if(rankType == "Inte")
                return b.inte.CompareTo(a.inte);
            else if(rankType == "Fair")
                return b.fair.CompareTo(a.fair);
            else if(rankType == "Charm")
                return b.charm.CompareTo(a.charm);
            return 0;
        });

        for(int i = 0; i < cellInfos.Count; i++)
        {
            cellInfos[i].gameObject.transform.SetSiblingIndex(i);
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
            string[] modeNames = {"势力武将", "势力战力"};
            for(int i = 0; i < modeNames.Length; i++)
            {
                GameObject cell = Instantiate(rankCellModePrefab, rankRegionMode.transform);
                cell.transform.localScale = Vector3.one;
                RankCellMode cellMode = cell.GetComponent<RankCellMode>();
                cellMode.rankPanelManager = this;
                cellMode.Init(modeNames[i]);
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
        }

        // 获取所有英雄配置
        var heroConfigs = HeroConfig.ConfigList;

        // 为每个英雄配置创建一个RankCell
        foreach (var heroConfig in heroConfigs)
        {
            // 实例化RankCell
            GameObject cell = Instantiate(rankCellPrefab, rankRegionMain.transform);
            cell.transform.localScale = Vector3.one;

            // 获取RankCellInfo组件
            RankCellInfo cellInfo = cell.GetComponent<RankCellInfo>();
            cellInfo.rankPanelManager = this;
            if (cellInfo != null)
                cellInfo.Init(heroConfig);
        }
        // Get the RectTransform components
         RectTransform rankParentRect = rankRegionMain.GetComponent<RectTransform>();
         RectTransform cellRect = rankCellPrefab.GetComponent<RectTransform>();
          
         if (rankParentRect != null && cellRect != null)
         {
             // Set the height of rankParent based on the number of cells
             rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * heroConfigs.Count);
         }
        // 确保scrollRect不为空，然后滚动到最前面
        if (scrollRectMain != null)
        {
            scrollRectMain.normalizedPosition = new Vector2(0, 1);
        }
    }

    public void OnSelectHero(RankCellInfo cellInfo)
    {
        // 取消上次选中的英雄
        if (lastSelectedHero != null && lastSelectedHero != cellInfo)
        {
            lastSelectedHero.heroPic.gameObject.SetActive(false);
        }
        
        // 选中当前英雄
        cellInfo.heroPic.gameObject.SetActive(true);
        
        // 更新缓存的上次选中英雄
        lastSelectedHero = cellInfo;
    }

    public void OnSelectMode(RankCellMode cellMode)
    {
        // 取消上次选中的英雄
        if (lastSelectedMode != null && lastSelectedMode != cellMode)
        {
            lastSelectedMode.modeName.gameObject.SetActive(false);
        }
        
        // 选中当前英雄
        cellMode.modeName.gameObject.SetActive(true);
        
        // 更新缓存的上次选中英雄
        lastSelectedMode = cellMode;
    }
    public void OnSelectForce(RankCellForce cellForce)
    {
        // 取消上次选中的英雄
        if (lastSelectedForce != null && lastSelectedForce != cellForce)
        {
            lastSelectedForce.forceName.gameObject.SetActive(false);
        }
        
        // 选中当前英雄
        cellForce.forceName.gameObject.SetActive(true);
        
        // 更新缓存的上次选中英雄
        lastSelectedForce = cellForce;
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
