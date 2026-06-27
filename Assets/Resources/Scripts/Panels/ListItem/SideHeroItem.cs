using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class SideHeroItem : MonoBehaviour
{
    public Image BG;
    public TMP_Text heroName;
    public Image heroIcon;
    public TMP_Text textState;
    public TMP_Text textLoyal;

    private bool isSelected = false;
    private int heroId;

    public Button button;
    private System.Action<SideHeroItem> onClickCallback;

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnItemClick);
        }
    }

    public void SetData(int heroId)
    {
        this.heroId = heroId;
        var heroData = GameManager.Instance.GetHero(heroId);
        if (heroData == null)
        {
            GameLog.Warn($"SideHeroItem.SetData: heroData is null, heroId={heroId}");
            return;
        }

        var heroCfg = HeroConfig.GetConfig(heroId);
        heroName.text = heroCfg != null ? heroCfg.Name : heroId.ToString();

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

        string stateText;
        if (heroData.state == HeroState.Wild)
            stateText = "在野";
        else if (heroData.state == HeroState.Catched)
            stateText = "俘虏";
        else
            stateText = "正常";

        string forceName;
        Color forceColor;
        if (heroData.forceId == SystemConst.Hero.WILD_FORCE_ID)
        {
            forceName = "在野";
            forceColor = Color.white;
        }
        else
        {
            forceName = ForceConfig.GetConfig(heroData.forceId).Cname;
            forceColor = SysColor.GetForceColor(heroData.forceId);
        }
        string forceHex = ColorUtility.ToHtmlStringRGB(forceColor);
        textState.text = $"{stateText}/<color=#{forceHex}>{forceName}</color>";

        int displayLoyalty = heroData.state == HeroState.Wild
            ? SystemConst.Hero.WILD_HERO_LOYALTY
            : heroData.loyalty;
        textLoyal.text = $"忠:{displayLoyalty}";

        SetSelected(false);
    }

    public void SetOnClickCallback(System.Action<SideHeroItem> callback)
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

    public int GetHeroId()
    {
        return heroId;
    }
}
