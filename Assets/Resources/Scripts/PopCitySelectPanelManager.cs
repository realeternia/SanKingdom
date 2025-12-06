using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class PopCitySelectPanelManager : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject rankParent;
    public GameObject cellPrefab; // RankCell预制体引用

    public Button closeBtn;
    public Button selectBtn;


    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HidePopCitySelectPanel();
          //  CardShopManager.Instance.OnShow();
        });

    }

    // 加载英雄排名
    private void Init(int cityId)
    {
        // 清除现有的子物体
        foreach (Transform child in rankParent.transform)
        {
            Destroy(child.gameObject);
        }
        
        int itemCount = 0;
        var cityData = GameManager.Instance.GetCity(cityId);
        var cityCfg = WorldConfig.GetConfig(cityId);

        foreach(var connectCityId in cityCfg.WorldNearIds)
        {
            var connectCityData = GameManager.Instance.GetCity(connectCityId);
            var connectCityCfg = WorldConfig.GetConfig(connectCityId);

            if(cityData.forceId != connectCityData.forceId)
            {
                // 实例化RankCell
                GameObject cell = Instantiate(cellPrefab, rankParent.transform);
                cell.transform.localScale = Vector3.one;
                // 获取PopCitySelectPanelCell组件
                PopCitySelectPanelCell cellInfo = cell.GetComponent<PopCitySelectPanelCell>();
                cellInfo.Init(connectCityId);
                itemCount++;
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

    public void OnSelectHero(RankCellInfo cellInfo)
    {
        // 取消上次选中的英雄
        // if (lastSelectedHero != null && lastSelectedHero != cellInfo)
        // {
        //     lastSelectedHero.heroPic.gameObject.SetActive(false);
        // }
        
        // // 选中当前英雄
        // cellInfo.heroPic.gameObject.SetActive(true);
        
        // // 更新缓存的上次选中英雄
        // lastSelectedHero = cellInfo;
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
