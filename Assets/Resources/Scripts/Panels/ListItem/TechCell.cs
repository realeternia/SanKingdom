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
    public Image backgroundImage;
    public TMP_Text levelText;

    private bool isUnlocked;

    public void Init(int id, bool unlocked)
    {
        techId = id;
        isUnlocked = unlocked;

        var cfg = TechConfig.GetConfig(id);
        if (techName != null)
            techName.text = cfg.Cname;
        if (levelText != null)
            levelText.text = $"L{cfg.Level}";

        UpdateBackgroundColor();
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage == null) return;

        var cfg = TechConfig.GetConfig(techId);
        if (isUnlocked)
        {
            backgroundImage.color = SysColor.Tech.GetCategoryColor(cfg.Category);
        }
        else
        {
            backgroundImage.color = SysColor.Tech.LockedColor;
        }

        // 文字颜色：解锁时用白色，未解锁时用深灰
        if (techName != null)
            techName.color = isUnlocked ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (techPanelManager != null)
            techPanelManager.OnSelectTech(this);
    }
}
