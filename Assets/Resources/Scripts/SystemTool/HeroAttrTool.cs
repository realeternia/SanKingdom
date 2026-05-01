using UnityEngine;
using CommonConfig;
using System.Collections.Generic;
using Controls.Utils;

public static class HeroAttrTool
{
    public static string GetCName(string attrName)
    {
        var cfg = HeroAttrConfig.GetConfigByname(attrName);
        if (cfg != null)
        {
            return cfg.Cname;
        }
        return attrName;
    }

    public static Color GetColorByValue(string attrName, int value)
    {
        var cfg = HeroAttrConfig.GetConfigByname(attrName);
        if (cfg == null || string.IsNullOrEmpty(cfg.ColorRule))
        {
            return Color.white;
        }
        return ParseColorRule(cfg.ColorRule, value);
    }

    public static string GetColoredText(string attrName, int value)
    {
        Color color = GetColorByValue(attrName, value);
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorHex}>{value}</color>";
    }

    public static string GetTextByValue(string attrName, int value)
    {
        GameLog.Info($"GetTextByValue: {attrName}, {value}");
        var cfg = HeroAttrConfig.GetConfigByname(attrName);
        if (cfg == null || string.IsNullOrEmpty(cfg.TextRule))
        {
            return value.ToString();
        }
        GameLog.Info($"GetTextByValue: {attrName}, {value}, {cfg.TextRule}");
        return ParseTextRule(cfg.TextRule, value);
    }

    public static string GetColoredTextWithRule(string attrName, int value)
    {
        string text = GetTextByValue(attrName, value);
        Color color = GetColorByValue(attrName, value);
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorHex}>{text}</color>";
    }

    private static string ParseTextRule(string rule, int value)
    {
        if (string.IsNullOrEmpty(rule))
        {
            return value.ToString();
        }

        string[] rules = rule.Split(',');
        foreach (string r in rules)
        {
            string[] parts = r.Split(':');
            if (parts.Length != 2)
            {
                continue;
            }

            string thresholdStr = parts[0].Trim();
            string text = parts[1].Trim();

            if (TryMatchThreshold(thresholdStr, value))
            {
                return text;
            }
        }

        return value.ToString();
    }

    private static Color ParseColorRule(string rule, int value)
    {
        if (string.IsNullOrEmpty(rule))
        {
            return Color.white;
        }

        string[] rules = rule.Split(',');
        foreach (string r in rules)
        {
            string[] parts = r.Split(':');
            if (parts.Length != 2)
            {
                continue;
            }

            string thresholdStr = parts[0].Trim();
            string colorStr = parts[1].Trim();

            if (TryMatchThreshold(thresholdStr, value))
            {
                if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                {
                    return color;
                }
            }
        }

        return Color.white;
    }

    private static bool TryMatchThreshold(string thresholdStr, int value)
    {
        if (thresholdStr.Contains("-"))
        {
            string[] range = thresholdStr.Split('-');
            if (range.Length == 2 && int.TryParse(range[0], out int min) && int.TryParse(range[1], out int max))
            {
                return value >= min && value <= max;
            }
        }
        else if (int.TryParse(thresholdStr, out int threshold))
        {
            return value >= threshold;
        }
        return false;
    }
}
