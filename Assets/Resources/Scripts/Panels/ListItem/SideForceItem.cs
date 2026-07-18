using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class SideForceItem : MonoBehaviour
{
    public Image BG;
    public TMP_Text forceName;
    public Image heroIcon;
    public TMP_Text textCityCount;
    public TMP_Text textRelation;

    private bool isSelected = false;
    private int forceId;

    public Button button;
    private System.Action<SideForceItem> onClickCallback;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnItemClick);
        }
    }

    public void SetData(int forceId, int srcForceId)
    {
        this.forceId = forceId;
        var forceData = GameManager.Instance.GetForce(forceId);
        if (forceData == null)
        {
            GameLog.Warn($"SideForceItem.SetData: forceData is null, forceId={forceId}");
            return;
        }

        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);

        // 势力名（用势力色高亮）
        Color forceColor = SysColor.GetForceColor(forceId);
        string forceHex = ColorUtility.ToHtmlStringRGB(forceColor);
        string displayName = heroCfg != null ? heroCfg.Name : forceCfg.Cname;
        forceName.text = $"<color=#{forceHex}>{displayName}</color>";

        if (heroIcon != null && heroCfg != null)
        {
            string iconPath = ResPath.Texture.HeroIcon(heroCfg.Icon);
            Sprite sprite = ResourceCache.LoadSpriteUI(iconPath);
            if (sprite != null)
            {
                heroIcon.sprite = sprite;
            }
            else
            {
                string defaultPath = ResPath.Texture.HeroDefaultIcon();
                Sprite defaultSprite = ResourceCache.LoadSpriteUI(defaultPath);
                if (defaultSprite != null)
                {
                    heroIcon.sprite = defaultSprite;
                }
            }
        }

        // 城市数
        int cityCount = GameManager.Instance.GetCitiesByForce(forceId).Count;
        textCityCount.text = cityCount.ToString();

        // 与源势力的友好度（按级别着色）
        int relation = GameManager.Instance.SaveData.forceRelation.GetRelation(srcForceId, forceId);
        var relationLevel = GameManager.Instance.SaveData.forceRelation.GetRelationLevel(srcForceId, forceId);
        Color relationColor;
        switch (relationLevel)
        {
            case RelationLevel.Friendly:
                relationColor = new Color(0.2f, 0.8f, 0.2f);
                break;
            case RelationLevel.Hostile:
                relationColor = new Color(0.9f, 0.2f, 0.2f);
                break;
            default:
                relationColor = new Color(0.6f, 0.5f, 0.35f);
                break;
        }
        string relationHex = ColorUtility.ToHtmlStringRGB(relationColor);
        textRelation.text = $"<color=#{relationHex}>{relation}</color>";

        SetSelected(false);
    }

    public void SetOnClickCallback(System.Action<SideForceItem> callback)
    {
        onClickCallback = callback;
    }

    public void OnItemClick()
    {
        onClickCallback?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (BG != null)
        {
            BG.color = isSelected ? SysColor.UI.MatchColor : SysColor.Theme.CellNormalDark;
        }
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public int GetForceId()
    {
        return forceId;
    }
}