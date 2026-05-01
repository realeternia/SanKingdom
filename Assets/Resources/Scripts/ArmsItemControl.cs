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

    public void Init(HeroAttrConfig attrConfig, HeroConfig heroConfig)
    {
        if (attrConfig == null || heroConfig == null)
            return;

        if (itemIcon != null && !string.IsNullOrEmpty(attrConfig.Icon))
        {
            string iconPath = "Textures/Icons/" + attrConfig.Icon;
            Sprite sprite = Resources.Load<Sprite>(iconPath);
            if (sprite != null)
            {
                itemIcon.sprite = sprite;
            }
        }

        if (itemName != null)
        {
            int attrValue = GetHeroArmsAttrValue(heroConfig, attrConfig.name);
            string displayText = HeroAttrTool.GetTextByValue(attrConfig.name, attrValue);
            Color color = HeroAttrTool.GetColorByValue(attrConfig.name, attrValue);
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            itemName.text = $"<color=#{colorHex}>{displayText}</color>";
        }
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
