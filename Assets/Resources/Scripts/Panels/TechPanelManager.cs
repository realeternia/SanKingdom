using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class TechPanelManager : MonoBehaviour
{
    public GameObject techCellPrefab;
    public GameObject techRegion;
    public Button closeBtn;
    public TMP_Text detailDes;
    public Button selectBtn;
    public TMP_Text selectBtnText;

    private int forceId;
    private TechCell lastSelectedCell;
    private List<TechCell> techCells = new List<TechCell>();

    // 选择模式：从 CityTechPanelManager 打开时，选择科技后回调
    private bool isSelectMode = false;
    private Action<int> onSelectTechCallback;

    // 分类显示名与排序
    private static readonly string[] Categories = { "Battle", "Development", "Institution", "Engineering" };
    private static readonly string[] CategoryNames = { "战斗", "发展", "制度", "工程" };

    private const int COLS_PER_ROW = 5;

    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {
            if (isSelectMode)
                PanelManager.Instance.HideTech();
            else
                PanelManager.Instance.HideRank();
        });

        if (selectBtn != null)
        {
            selectBtn.onClick.AddListener(OnSelectBtnClick);
        }
    }

    /// <summary>
    /// 设置选择模式：传入回调，点击确认按钮时返回 techId
    /// </summary>
    public void SetSelectMode(Action<int> callback)
    {
        isSelectMode = true;
        onSelectTechCallback = callback;

        if (selectBtn != null)
            selectBtn.gameObject.SetActive(true);

        if (selectBtnText != null)
            selectBtnText.text = "确认选择";
    }

    public void OnShow()
    {
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        forceId = playerForce != null ? playerForce.forceId : 0;
        CreateTechCells();

        // 非选择模式下隐藏确认按钮
        if (!isSelectMode && selectBtn != null)
            selectBtn.gameObject.SetActive(false);
    }

    public void OnHide()
    {
        ClearCells();
    }

    private void ClearCells()
    {
        foreach (var cell in techCells)
        {
            if (cell != null && cell.gameObject != null)
                Destroy(cell.gameObject);
        }
        techCells.Clear();
        lastSelectedCell = null;

        // 清除分类标题
        foreach (Transform child in techRegion.transform)
            Destroy(child.gameObject);
    }

    private void CreateTechCells()
    {
        ClearCells();

        var unlockedTechs = ForceTech.GetUnlockedTechs(forceId);
        RectTransform regionRect = techRegion.GetComponent<RectTransform>();

        float cellWidth = 140f;
        float cellHeight = 80f;
        float spacingX = 10f;
        float spacingY = 10f;
        float headerHeight = 30f;
        float categorySpacing = 15f;

        float posY = 0f;
        int totalRows = 0;

        for (int catIdx = 0; catIdx < Categories.Length; catIdx++)
        {
            string category = Categories[catIdx];
            var techs = TechConfig.ConfigList
                .Where(t => t.Category == category)
                .OrderBy(t => t.Level)
                .ThenBy(t => t.Id)
                .ToList();

            if (techs.Count == 0) continue;

            // 分类标题
            var headerObj = new GameObject($"Header_{category}");
            headerObj.transform.SetParent(techRegion.transform, false);
            var headerRect = headerObj.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0, 1);
            headerRect.anchorMax = new Vector2(0, 1);
            headerRect.pivot = new Vector2(0, 1);
            headerRect.anchoredPosition = new Vector2(0, posY);
            headerRect.sizeDelta = new Vector2(regionRect.rect.width, headerHeight);

            var headerText = headerObj.AddComponent<Text>();
            headerText.text = CategoryNames[catIdx];
            headerText.fontSize = 22;
            headerText.color = SysColor.Tech.GetCategoryColor(category);
            headerText.alignment = TextAnchor.MiddleLeft;
            headerText.raycastTarget = false;

            posY -= headerHeight;

            // 计算行数
            int rows = (techs.Count + COLS_PER_ROW - 1) / COLS_PER_ROW;

            // 计算起始X居中偏移
            float totalWidth = COLS_PER_ROW * cellWidth + (COLS_PER_ROW - 1) * spacingX;
            float startX = (regionRect.rect.width - totalWidth) / 2f;

            for (int i = 0; i < techs.Count; i++)
            {
                int row = i / COLS_PER_ROW;
                int col = i % COLS_PER_ROW;

                float posX = startX + col * (cellWidth + spacingX);
                float cellPosY = posY - row * (cellHeight + spacingY);

                GameObject cellObj = Instantiate(techCellPrefab, techRegion.transform);
                cellObj.transform.localScale = Vector3.one;

                RectTransform cellRect = cellObj.GetComponent<RectTransform>();
                if (cellRect != null)
                {
                    cellRect.anchorMin = new Vector2(0, 1);
                    cellRect.anchorMax = new Vector2(0, 1);
                    cellRect.pivot = new Vector2(0, 1);
                    cellRect.anchoredPosition = new Vector2(posX, cellPosY);
                    cellRect.sizeDelta = new Vector2(cellWidth, cellHeight);
                }

                TechCell cell = cellObj.GetComponent<TechCell>();
                bool unlocked = unlockedTechs.Contains(techs[i].Id);
                cell.Init(techs[i].Id, unlocked);
                cell.techPanelManager = this;
                techCells.Add(cell);
            }

            posY -= rows * (cellHeight + spacingY);
            posY -= categorySpacing;
            totalRows += rows;
        }

        // 设置内容区域高度
        float contentHeight = -posY;
        regionRect.sizeDelta = new Vector2(regionRect.sizeDelta.x, contentHeight);
    }

    public void OnSelectTech(TechCell cell)
    {
        if (lastSelectedCell != null && lastSelectedCell != cell)
        {
            // 可扩展：取消上次选中效果
        }
        lastSelectedCell = cell;

        var cfg = TechConfig.GetConfig(cell.techId);
        if (detailDes != null)
            detailDes.text = cfg.Des;
    }

    private void OnSelectBtnClick()
    {
        if (lastSelectedCell == null)
        {
            SystemTip.Instance.ShowTip("请先选择一个科技");
            return;
        }

        // 已解锁的科技不能选择研究
        var unlockedTechs = ForceTech.GetUnlockedTechs(forceId);
        if (unlockedTechs.Contains(lastSelectedCell.techId))
        {
            SystemTip.Instance.ShowTip("该科技已解锁，无需研究");
            return;
        }

        if (onSelectTechCallback != null)
        {
            onSelectTechCallback(lastSelectedCell.techId);
        }

        PanelManager.Instance.HideTech();
    }
}
