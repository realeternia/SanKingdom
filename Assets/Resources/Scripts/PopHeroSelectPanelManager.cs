using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class PopHeroSelectPanelManager : MonoBehaviour
{
    private int mCityId;
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject cellPrefab; // RankCell预制体引用

    public Button closeBtn;
    public Button selectBtn;
    private PopHeroSelectPanelCell lastSelectedCell;


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
            if (lastSelectedCell != null)
            {
                PanelManager.Instance.HidePopHeroSelectPanel();
            }
        });

    }

    // 加载英雄排名
    private void Init(int cityId)
    {
        mCityId = cityId;
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        int itemCount = 0;
        var myList = GameManager.Instance.GetCity(mCityId).GetHeroList();

        foreach(var heroId in myList)
        {
            // 实例化RankCell
            GameObject cell = Instantiate(cellPrefab, rankParent.transform);
            cell.transform.localScale = Vector3.one;
            // 获取PopHeroSelectPanelCell组件
            PopHeroSelectPanelCell cellInfo = cell.GetComponent<PopHeroSelectPanelCell>();
            cellInfo.popHeroSelectPanelManager = this;
            cellInfo.Init(heroId);
            itemCount++;
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

    public void OnSelectItem(PopHeroSelectPanelCell cellInfo)
    {
        // 取消上次选中的城市
        if (lastSelectedCell != null && lastSelectedCell != cellInfo)
        {
            lastSelectedCell.OnSelect(false);
        }
        
        // 选中当前城市
        cellInfo.OnSelect(true);
        // 更新当前选中的单元格引用
        lastSelectedCell = cellInfo;
    }

    public void OnShow(int cityId)
    {
        Init(cityId);
    }

    public void OnHide()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}
