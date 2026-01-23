using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;

public class PopHeroBattleSelectPanelManager : MonoBehaviour, IPanelEvent
{
    private int mCityId;
    private int mSelectCount;
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject cellPrefab; // RankCell预制体引用
    private bool allowZeroSoldier = false;

    public Button closeBtn;
    public Button selectBtn;

    public TMP_Text textAttr1;
    public TMP_Text textAttr2;
    private List<PopHeroBattleSelectPanelCell> lastSelectedCells = new List<PopHeroBattleSelectPanelCell>();
    private List<PopHeroBattleSelectPanelCell> cacheHeroList = new List<PopHeroBattleSelectPanelCell>();
    private Action<List<int>> onSelectMethod;


    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopHeroBattleSelectPanel();
            //  CardShopManager.Instance.OnShow();
        });
        selectBtn.onClick.AddListener(() =>
        {
            if (lastSelectedCells != null && lastSelectedCells.Count > 0)
            {
                List<int> selectedHeroIds = new List<int>();
                foreach (var cell in lastSelectedCells)
                {
                    selectedHeroIds.Add(cell.heroId);
                }
                if (!allowZeroSoldier)
                {
                    foreach (var heroId in selectedHeroIds)
                    {
                        if (GameManager.Instance.GetHero(heroId).soldier <= 0)
                            return;
                    }
                }
                onSelectMethod?.Invoke(selectedHeroIds);
                PanelManager.Instance.HidePopHeroBattleSelectPanel();
            }
        });
    }


    // 加载英雄排名
    private void Init(int cityId, int selectCount, int[] heroList, int[] checkedList)
    {
        mCityId = cityId;
        mSelectCount = selectCount;
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }
        lastSelectedCells.Clear();

        int itemCount = 0;
        var sortdata = new List<PopHeroBattleSelectPanelCell>();
        foreach(var heroId in heroList)
        {
            // 实例化RankCell
            GameObject cell = Instantiate(cellPrefab, rankParent.transform);
            cell.transform.localScale = Vector3.one;
            // 获取PopHeroSelectPanelCell组件
            var cellInfo = cell.GetComponent<PopHeroBattleSelectPanelCell>();
            cellInfo.popHeroSelectPanelManager = this;

            var heroData = GameManager.Instance.GetHero(heroId);
            cellInfo.Init(heroData);
            cacheHeroList.Add(cellInfo);
            itemCount++;

            if (checkedList != null && checkedList.Length > 0)
            {
                if (Array.IndexOf(checkedList, heroId) >= 0)
                    OnSelectItem(cellInfo, true);
            }
            sortdata.Add(cellInfo);
        }

        sortdata.Sort((a, b) => {
            int yearCompare = a.heroYear.CompareTo(b.heroYear);
            if (yearCompare != 0) return yearCompare;
            return b.attr1Val.CompareTo(a.attr1Val);
        });

        for (int i = 0; i < sortdata.Count; i++)
        {
            sortdata[i].gameObject.transform.SetSiblingIndex(i);
        }

        // Get the RectTransform components
         RectTransform rankParentRect = rankParent.GetComponent<RectTransform>();
         RectTransform cellRect = cellPrefab.GetComponent<RectTransform>();
          
         if (rankParentRect != null && cellRect != null)
         {
             // Set the height of rankParent based on the number of cells
             rankParentRect.sizeDelta = new Vector2(rankParentRect.sizeDelta.x, cellRect.sizeDelta.y * itemCount);
         }
        // 确保scrollRect不为空，然后滚动到最前面
        if (scrollRect != null)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
    }

    public void OnSelectItem(PopHeroBattleSelectPanelCell selectTarget, bool isSelect)
    {
        if (!isSelect)
        {
            foreach (var cellInfo in lastSelectedCells)
            {
                if (cellInfo == selectTarget)
                {
                    cellInfo.OnSelect(false);
                    lastSelectedCells.Remove(cellInfo);
                    break;
                }
            }
        }
        else if(lastSelectedCells.Count < mSelectCount)
        {
            lastSelectedCells.Add(selectTarget);
        }

        // 选中当前城市
        foreach (var cellInfo in lastSelectedCells)
        {
            cellInfo.OnSelect(true);
        }
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        if(name == "CityAttrChange")
        {
            foreach (var cellInfo in cacheHeroList)
            {
                cellInfo.UpdateAttr();
            }
        }
    }

    public void OnShow(int cityId, int selectCount, int[] heroList, bool allowZeroSoldier, int[] checkedList, Action<List<int>> onSelectMethod)
    {
        this.allowZeroSoldier = allowZeroSoldier;
        this.onSelectMethod = onSelectMethod;
        Init(cityId, selectCount, heroList, checkedList);
    }

    public void OnHide()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}
