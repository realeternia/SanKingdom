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

    public Button refreshBtn;
    public TMP_Text refreshText;
    public Button okBtn;
    private int refreshCount = 4;

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
        refreshBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.PlaySound("Sounds/page");
            RefreshBtnClick();
        });
        okBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.NewGame();
            PanelManager.Instance.ShowWorld();
            PanelManager.Instance.HidePick();
        });

        okBtn.gameObject.SetActive(false);
        refreshBtn.gameObject.SetActive(false);

        StartCoroutine(DelaySetMode());
        PanelManager.Instance.ShowPick();
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
                    RefreshBtnClick();
                }
            });
            newGameBtn.onClick.AddListener(() =>
            {
                loadGamePanel.SetActive(false);
                RefreshBtnClick();
            });            
        }
        else
        {
            Debug.Log("No Game Save");
            loadGamePanel.SetActive(false);
            RefreshBtnClick();
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

    private void RefreshBtnClick()
    {
        RefreshHeroPool();
        okBtn.gameObject.SetActive(true);
        
        refreshCount--;
        refreshText.text = "刷新(" + refreshCount + ")";
        if (refreshCount <= 0)
        {
            refreshBtn.gameObject.SetActive(false);
        }
        else
        {
            refreshBtn.gameObject.SetActive(true);
        }

    }


    // 取消除了指定单元格之外的所有选中状态
    public void ClearAllSelectionsExcept(PickPanelCellControl exceptCell)
    {
        foreach (var cellControl in cellControls)
        {
            if (cellControl != exceptCell)
            {
                cellControl.SetSelected(false);
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
            forcePool.Add(item.Id);
        
        // 每行显示10个，共5行
        int itemsPerRow = 13;
        int rows = 7;
        int totalItems = Mathf.Min(forcePool.Count, itemsPerRow * rows);
        
        // 单元格大小和间距
        float cellWidth = 200f;
        float cellHeight = 276f;
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
            float posX = 5 + col * (cellWidth + spacingX) + 105;
            float posY = -5 -row * (cellHeight + spacingY) - 140;

            // 设置位置
            RectTransform rectTransform = cell.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(posX, posY);

            // 设置单元格数据
            PickPanelCellControl cellControl = cell.GetComponent<PickPanelCellControl>();
            cellControl.heroId = heroId;
            cellControl.SetParentControl(this); // 设置父控制类引用
            cellControls.Add(cellControl);
            if (cellControl != null)
            {
                // 设置英雄图片
                cellControl.heroImg.sprite = Resources.Load<Sprite>("SkinsBig/" + heroCfg.Icon);
                // 设置英雄名称
                cellControl.heroName.text = heroCfg.Name;

                string colorStr = forceCfg.Color;
                // 分割颜色字符串为RGB组件
                string[] rgbValues = colorStr.Split(',');
                // 检查是否有3个组件
                if (rgbValues.Length == 3)
                {
                    // 转换为数值并除以255（Unity颜色值范围为0-1）
                    float r = float.Parse(rgbValues[0]) / 255f;
                    float g = float.Parse(rgbValues[1]) / 255f;
                    float b = float.Parse(rgbValues[2]) / 255f;
                    
                    // 设置颜色，添加alpha值为1（不透明）
                    cellControl.bgImg.GetComponent<Image>().color = new Color(r, g, b, 1f);
                }
            }
        }
    }

}
