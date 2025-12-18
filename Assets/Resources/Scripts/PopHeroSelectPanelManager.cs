using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;

public class PopHeroSelectPanelManager : MonoBehaviour
{
    private int mCityId;
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject cellPrefab; // RankCell预制体引用

    public Button closeBtn;
    public Button selectBtn;
    public Image[] heroHeads;

    public TMP_Text textAttr1;
    public TMP_Text textAttr2;
    private List<PopHeroSelectPanelCell> lastSelectedCells = new List<PopHeroSelectPanelCell>();
    private Action<List<int>> onSelectMethod;


    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopHeroSelectPanel();
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
                onSelectMethod?.Invoke(selectedHeroIds);
                PanelManager.Instance.HidePopHeroSelectPanel();
            }
        });
    }

    private string GetAttrCName(string attr)
    {
        switch (attr)
        {
            case "Str":
                return "武力";
            case "Inte":
                return "智力";
            case "Fair":
                return "政治";
            default:
                return attr;
        }
    }

    // 加载英雄排名
    private void Init(int[] heroList, int[] checkedList, string[] attrs)
    {
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }
        lastSelectedCells.Clear();
        for (int i = 0; i < heroHeads.Length; i++)
        {
            heroHeads[i].gameObject.SetActive(false);
        }

        // 初始化属性文本
        textAttr1.text = GetAttrCName(attrs[0]);
        textAttr2.text = GetAttrCName(attrs[1]);
        
        int itemCount = 0;
        foreach(var heroId in heroList)
        {
            // 实例化RankCell
            GameObject cell = Instantiate(cellPrefab, rankParent.transform);
            cell.transform.localScale = Vector3.one;
            // 获取PopHeroSelectPanelCell组件
            PopHeroSelectPanelCell cellInfo = cell.GetComponent<PopHeroSelectPanelCell>();
            cellInfo.popHeroSelectPanelManager = this;
            cellInfo.Init(heroId, attrs);
            itemCount++;

            if (checkedList != null && checkedList.Length > 0)
            {
                if (Array.IndexOf(checkedList, heroId) >= 0)
                    OnSelectItem(cellInfo, true);
            }
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

    public void OnSelectItem(PopHeroSelectPanelCell selectTarget, bool isSelect)
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
        else if(lastSelectedCells.Count < heroHeads.Length)
        {
            lastSelectedCells.Add(selectTarget);
        }

        // 选中当前城市
        int id = 0;
        foreach (var cellInfo in lastSelectedCells)
        {
            cellInfo.OnSelect(true);
            var icon = HeroConfig.GetConfig(cellInfo.heroId).Icon;
            heroHeads[id].gameObject.SetActive(true);
            heroHeads[id].sprite = Resources.Load<Sprite>("Skins/" + icon);
            id++;
        }
        for(int i = id; i < heroHeads.Length; i++)
        {
            heroHeads[i].gameObject.SetActive(false);
        }
    }

    public void OnShow(int[] heroList, int[] checkedList, string[] attrs, Action<List<int>> onSelectMethod)
    {
        this.onSelectMethod = onSelectMethod;
        Init(heroList, checkedList, attrs);
    }

    public void OnHide()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}
