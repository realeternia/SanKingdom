using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class ArmsItemControl : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemName;
    public Image BG;

    public void Init(HeroAttrConfig attrConfig, HeroConfig heroConfig, int armsId)
    {
        if (attrConfig == null || heroConfig == null)
            return;

        if (itemIcon != null && !string.IsNullOrEmpty(attrConfig.Icon))
        {
            string iconPath = ResPath.Texture.AttrIcon(attrConfig.Icon);
            Sprite sprite = ResourceCache.LoadSpriteUI(iconPath);
            if (sprite != null)
            {
                itemIcon.sprite = sprite;
            }
        }

        if (itemName != null)
        {
            int attrValue = GetHeroArmsAttrValue(heroConfig, attrConfig.name);
            string displayText = HeroAttrTool.GetTextByValue(attrConfig.name, attrValue);
            Color color = SysColor.GetColorByValue(attrConfig.name, attrValue);
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            itemName.text = $"<color=#{colorHex}>{displayText}</color>";
        }

        UpdateBGColor(attrConfig, armsId);
    }

    public void UpdateBGColor(HeroAttrConfig attrConfig, int armsId)
    {
        if (BG == null || attrConfig == null)
            return;

        bool isMatch = false;
        if (armsId > 0)
        {
            var armsConfig = ArmsConfig.GetConfig(armsId);
            isMatch = armsConfig.Type.ToString() == attrConfig.name;
        }

        BG.color = isMatch ? SysColor.UI.MatchColor : SysColor.Theme.CellNormal;
    }

    private int GetHeroArmsAttrValue(HeroConfig heroConfig, string attrName)
    {
        switch (attrName)
        {
            case "SodWalk": return heroConfig.SodWalk;
            case "SodHorse": return heroConfig.SodHorse;
            case "SodBow": return heroConfig.SodBow;
            case "SodWater": return heroConfig.SodWater;
            case "SodTank": return heroConfig.SodTank;
            default: return 0;
        }
    }
}
