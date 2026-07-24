using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class TechCell : MonoBehaviour, IPointerDownHandler
{
    public TechPanelManager techPanelManager;

    public int techId;
    public TMP_Text techName;
    public Image techIcon;
    public Image techBG;
    public Image backgroundImage;
    public TMP_Text techCost;
    public GameObject techCostNode;

    private bool isUnlocked;
    private bool isLearnable;
    private bool isSelected;

    // 选中高亮颜色
    private static readonly Color SelectedColor = new Color(1f, 0.95f, 0.4f, 1f);
    // 未选中透明
    private static readonly Color UnselectedColor = new Color(1f, 1f, 1f, 0f);
    // 未解锁文字颜色
    private static readonly Color LockedTextColor = new Color(0.6f, 0.6f, 0.6f, 1f);
    // 不可学时 techBG 暗灰色
    private static readonly Color LockedBGColor = new Color(0.35f, 0.35f, 0.35f, 1f);

    public void Init(int id, bool unlocked, bool learnable)
    {
        techId = id;
        isUnlocked = unlocked;
        isLearnable = learnable;
        isSelected = false;

        var cfg = TechConfig.GetConfig(id);

        // techBG：可学或已解锁显示分类颜色，不可学显示暗灰
        if (techBG != null)
            techBG.color = (isLearnable || isUnlocked) ? SysColor.Tech.GetCategoryColor(cfg.Category) : LockedBGColor;

        // 始终显示正常名称和图标
        if (techName != null)
            techName.text = cfg.Cname;
        if (techIcon != null)
            techIcon.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.TechIcon(cfg.Icon));

        // 研究值消耗节点：仅可学且未解锁时显示
        if (techCostNode != null)
            techCostNode.SetActive(isLearnable && !isUnlocked);
        if (techCost != null && isLearnable && !isUnlocked)
            techCost.text = cfg.SciPointCost.ToString();

        UpdateBackgroundColor();
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        if (techCostNode != null)
            techCostNode.SetActive(isLearnable && !isUnlocked);
        UpdateBackgroundColor();
    }

    /// <summary>
    /// 设置选中状态，切换 backgroundImage 高亮
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBackgroundColor();
    }

    public bool IsLearnable()
    {
        return isLearnable;
    }

    private void UpdateBackgroundColor()
    {
        // backgroundImage 用于选中高亮
        if (backgroundImage != null)
            backgroundImage.color = isSelected ? SelectedColor : UnselectedColor;

        // 文字颜色：已解锁白色，未解锁暗灰
        if (techName != null)
            techName.color = isUnlocked ? Color.white : LockedTextColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (techPanelManager != null)
            techPanelManager.OnSelectTech(this);
    }
}
