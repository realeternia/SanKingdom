using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class PickPanelControl : MonoBehaviour
{
    public GameObject pickPanelCellPrefab; // 引用PickPanelCell预制体
    public Transform cellParent; // 用于放置单元格的父容器

    public TMP_Text forceNameText;
    public TMP_Text goldText;
    public TMP_Text foodText;
    public TMP_Text soldierText;
    public TMP_Text heroNumText;
    public TMP_Text cityNumText;

    public Button okBtn;
    private int targetForceId = 0;

    private List<PickPanelCellControl> cellControls = new List<PickPanelCellControl>();

    public GameObject loadGamePanel;
    public Button loadGameBtn;
    public Button newGameBtn;


    private void Awake()
    {

    }
 
    // Start is called before the first frame update
    void Start()
    {
        okBtn.onClick.AddListener(() =>
        {
            if(targetForceId == 0)
                return;

            GameManager.Instance.NewGame(targetForceId);
            PanelManager.Instance.ShowWorld();
            PanelManager.Instance.HidePick();
        });

        okBtn.gameObject.SetActive(false);
        RefreshHeroPool();

        StartCoroutine(DelaySetMode());
        if(GameManager.Instance.IsGameSaveExist())
        {
            loadGamePanel.SetActive(true);
            loadGameBtn.onClick.AddListener(() =>
            {
                var isSuccess = GameManager.Instance.LoadFromSave();
                if(isSuccess)
                {
                    PanelManager.Instance.ShowWorld();
                    PanelManager.Instance.HidePick();
                }
                else
                {
                    loadGamePanel.SetActive(false);
                }
            });
            newGameBtn.onClick.AddListener(() =>
            {
                loadGamePanel.SetActive(false);
            });            
        }
        else
        {
            Debug.Log("No Game Save");
            loadGamePanel.SetActive(false);
        }

    }

    IEnumerator DelaySetMode()
    {
        yield return new WaitForSeconds(0.1f);
        BattleManager.Instance.isDebug = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // 取消除了指定单元格之外的所有选中状态
    public void OnSelectTarget(PickPanelCellControl exceptCell)
    {
        foreach (var cellControl in cellControls)
        {
            if (cellControl != exceptCell)
            {
                cellControl.SetSelected(false);
            }
            else
            {
                targetForceId = exceptCell.forceId;
                var forceCfg = ForceConfig.GetConfig(targetForceId);
                int foodTotal = 0;
                int goldTotal = 0;
                int soldierTotal = 0;
                int cityTotal = 0;
                
                foreach(var worldConfig in WorldConfig.ConfigList)
                {
                    if(worldConfig.ForceId != targetForceId)
                        continue;

                    foodTotal += worldConfig.Food;
                    goldTotal += worldConfig.Gold;
                    soldierTotal += worldConfig.Soldier;
                    cityTotal ++;
                }

                int heroTotal = 0;
                foreach(var heroCfg in HeroConfig.ConfigList)
                {
                    if(heroCfg.ForceId != targetForceId)
                        continue;
                    heroTotal ++;
                }

                forceNameText.text = forceCfg.Cname;
                goldText.text = goldTotal.ToString();
                foodText.text = foodTotal.ToString();
                soldierText.text = soldierTotal.ToString();
                heroNumText.text = heroTotal.ToString();
                cityNumText.text = cityTotal.ToString();
                okBtn.gameObject.SetActive(true);
            }
        }
    }

    void RefreshHeroPool()
    {
        // 销毁节点下所有子对象
        for (int i = cellParent.childCount - 1; i >= 0; i--)
        {
            Destroy(cellParent.GetChild(i).gameObject);
        }
        cellControls.Clear();


        // 获取英雄池缓存
        var forcePool = new List<int>();
        foreach (var item in ForceConfig.ConfigList)
            if(item.Id < 99)    
                forcePool.Add(item.Id);

        // 每行显示10个，共5行
        int itemsPerRow = 3;
        int rows = 5;
        int totalItems = Mathf.Min(forcePool.Count, itemsPerRow * rows);
        
        // 单元格大小和间距
        float cellWidth = 115f;
        float cellHeight = 115f;
        float spacingX = 5f;
        float spacingY = 5f;

        // 创建单元格
        for (int i = 0; i < totalItems; i++)
        {
            var forceCfg = ForceConfig.GetConfig(forcePool[i]);
            int heroId = forceCfg.HeroId;
            HeroConfig heroCfg = HeroConfig.GetConfig(heroId);

            // 实例化单元格
            GameObject cell = Instantiate(pickPanelCellPrefab, cellParent);
            cell.transform.localScale = Vector3.one;

            // 计算位置
            int row = i / itemsPerRow;
            int col = i % itemsPerRow;
            float posX = 5 + col * (cellWidth + spacingX) + 88;
            float posY = -5 -row * (cellHeight + spacingY) - 88;

            // 设置位置
            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            // 设置单元格数据
            PickPanelCellControl cellControl = cell.GetComponent<PickPanelCellControl>();
            cellControl.forceId = forceCfg.Id;
            cellControl.SetParentControl(this); // 设置父控制类引用
            cellControls.Add(cellControl);
            if (cellControl != null)
            {
                // 设置英雄图片
                cellControl.heroImg.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
                // 设置英雄名称
                cellControl.heroName.text = heroCfg.Name;

                cellControl.bgImg.GetComponent<Image>().color = ColorUtility.TryParseHtmlString(forceCfg.Color, out var wColor) ? wColor : Color.white;
            }
        }
    }

}
