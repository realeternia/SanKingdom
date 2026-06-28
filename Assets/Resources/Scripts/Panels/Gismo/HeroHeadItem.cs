using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class HeroHeadItem : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemName;
    public TMP_Text itemAttr;
    public Image BG;
    public Button itemButton;

    public delegate bool CanSelectHandler();
    public delegate void SelectionChangedHandler();

    private int heroId;
    private bool isSelected = false;
    private int forceId;
    private bool hasActed = false;
    private CanSelectHandler canSelectCallback;
    private SelectionChangedHandler selectionChangedCallback;

    public void Init(int heroId, string attText, int forceId, bool hasActed = false)
    {
        this.heroId = heroId;
        this.forceId = forceId;
        isSelected = false;
        this.hasActed = hasActed;
        UpdateBgColor();
        RefreshUI(attText);

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnItemClick);
    }

    public int GetHeroId()
    {
        return heroId;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateBgColor();
    }

    public void SetCallbacks(CanSelectHandler canSelect, SelectionChangedHandler selectionChanged)
    {
        canSelectCallback = canSelect;
        selectionChangedCallback = selectionChanged;
    }

    private void OnItemClick()
    {
        if (!isSelected && canSelectCallback != null && !canSelectCallback())
            return;

        isSelected = !isSelected;
        UpdateBgColor();

        if (selectionChangedCallback != null)
            selectionChangedCallback();
    }

    private void UpdateBgColor()
    {
        BG.color = isSelected ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormal;
    }

    private void RefreshUI(string attText)
    {
        var heroConfig = HeroConfig.GetConfig(heroId);

        if (itemName != null)
        {
            string nameText = heroConfig.Name;
            if (heroConfig.StarHero)
            {
                nameText = "★" + nameText;
            }

            if (hasActed)
            {
                string grayHex = ColorUtility.ToHtmlStringRGB(SysColor.Theme.ActedHeroTextColor);
                itemName.text = $"<color=#{grayHex}>{nameText}</color>";
            }
            else
            {
                bool isKing = false;
                if (forceId > 0)
                {
                    var forceCfg = ForceConfig.GetConfig(forceId);
                    isKing = forceCfg.HeroId == heroId;
                }

                if (isKing)
                {
                    string goldHex = "FFD700";
                    itemName.text = $"<color=#{goldHex}>{nameText}</color>";
                }
                else
                {
                    itemName.text = nameText;
                }
            }
        }

        if (itemIcon != null)
        {
            string iconPath = ResPath.Texture.HeroIcon(heroConfig.Icon);
            Sprite sprite = ResourceCache.LoadSpriteUI(iconPath);
            if (sprite != null)
            {
                itemIcon.sprite = sprite;
            }
            else
            {
                string defaultPath = ResPath.Texture.HeroDefaultIcon();
                Sprite defaultSprite = ResourceCache.LoadSpriteUI(defaultPath);
                if (defaultSprite != null)
                {
                    itemIcon.sprite = defaultSprite;
                }
            }
        }

        if (itemAttr != null)
        {
            itemAttr.text = attText ?? "";
        }
    }
}
