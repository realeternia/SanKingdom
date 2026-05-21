using UnityEngine;
using CommonConfig;

public static class SysColor
{
    private static readonly Color[] ArmsLevelColors = new Color[]
    {
        new Color(0.5f, 0.5f, 0.5f, 1f),
        Color.white,
        new Color(0.27f, 0.51f, 0.9f, 1f),
        new Color(0.2f, 0.8f, 0.2f, 1f),
        new Color(1f, 0.85f, 0f, 1f),
        new Color(1f, 0.5f, 0f, 1f)
    };

    public static Color GetArmsLevelColor(int level)
    {
        if (level >= 0 && level < ArmsLevelColors.Length)
            return ArmsLevelColors[level];
        return ArmsLevelColors[ArmsLevelColors.Length - 1];
    }

    public static Color GetForceColor(int forceId)
    {
        var forceCfg = ForceConfig.GetConfig(forceId);
        if (ColorUtility.TryParseHtmlString(forceCfg.Color, out Color color))
            return color;
        return Color.white;
    }

    public static Color GetColorByValue(string attrName, int value)
    {
        var cfg = HeroAttrConfig.GetConfigByname(attrName);
        if (cfg == null || string.IsNullOrEmpty(cfg.ColorRule))
            return Color.white;
        return ParseColorRule(cfg.ColorRule, value);
    }

    public static string GetColoredText(string attrName, int value)
    {
        Color color = GetColorByValue(attrName, value);
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorHex}>{value}</color>";
    }

    public static string GetColoredTextWithRule(string attrName, int value)
    {
        string text = HeroAttrTool.GetTextByValue(attrName, value);
        Color color = GetColorByValue(attrName, value);
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{colorHex}>{text}</color>";
    }

    public static Color GetTextColorOnBackground(Color bgColor)
    {
        float brightness = 0.299f * bgColor.r + 0.587f * bgColor.g + 0.114f * bgColor.b;
        return brightness > 0.65f ? new Color(0.4f, 0.4f, 0.4f, 1f) : Color.white;
    }

    private static Color ParseColorRule(string rule, int value)
    {
        if (string.IsNullOrEmpty(rule))
            return Color.white;

        string[] rules = rule.Split(',');
        foreach (string r in rules)
        {
            string[] parts = r.Split(':');
            if (parts.Length != 2)
                continue;

            string thresholdStr = parts[0].Trim();
            string colorStr = parts[1].Trim();

            if (TryMatchThreshold(thresholdStr, value))
            {
                if (ColorUtility.TryParseHtmlString(colorStr, out Color color))
                    return color;
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
                return value >= min && value <= max;
        }
        else if (int.TryParse(thresholdStr, out int threshold))
        {
            return value >= threshold;
        }
        return false;
    }

    public static class Theme
    {
        public static readonly Color CellNormal = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        public static readonly Color CellSelected = new Color(0.3f, 0.7f, 0.4f, 1f);
        public static readonly Color CellDisabled = new Color(0.1f, 0.1f, 0.1f, 0.5f);
    }

    public static class UI
    {
        public static readonly Color DropDownNormal = new Color(0.2f, 0.2f, 0.25f, 0.9f);
        public static readonly Color DropDownSelected = new Color(0.3f, 0.5f, 0.7f, 1f);
        public static readonly Color DropDownHover = new Color(0.35f, 0.35f, 0.4f, 0.95f);

        public static readonly Color BorderColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color BorderSelectedColor = Color.green;

        public static readonly Color MatchColor = new Color(0.3f, 0.7f, 0.3f, 1f);

        public static readonly Color DragHighlightColor = Color.green;
        public static readonly Color DragResetColor = Color.white;
    }

    public static class Battle
    {
        public static readonly Color DamageColor = new Color(1f, 0f, 0f);
        public static readonly Color FoodLossColor = Color.red;
        public static readonly Color FoodGainColor = Color.green;
        public static readonly Color DeadColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        public static readonly Color HealthLowColor = new Color(0.4f, 0.33f, 0f);
        public static readonly Color HealthNormalColor = new Color(0f, 0.4f, 0.1f);
        public static readonly Color HealthWarningColor = Color.yellow;
        public static readonly Color AttackSuccessColor = Color.red;
        public static readonly Color AttackFailColor = Color.green;
        public static readonly Color CapturedOutlineColor = Color.red;
    }

    public static class Hero
    {
        public static readonly Color StateNormalColor = Color.white;
        public static readonly Color StateWildColor = Color.yellow;
        public static readonly Color StateCapturedColor = Color.red;

        public static readonly Color TierHighColor = Color.red;
        public static readonly Color TierMediumColor = Color.yellow;
        public static readonly Color TierLowColor = Color.green;
    }

    public static class Chess
    {
        public static readonly Color GoldMain = new Color(1f, 0.843f, 0f, 1f);
        public static readonly Color GoldEmission = new Color(1f, 0.7f, 0f, 1f);
        public static readonly Color GoldOutline = new Color(0.9f, 0.7f, 0.1f, 1f);
        public static readonly Color GoldSpec = new Color(1f, 0.9f, 0.5f, 1f);

        public static readonly Color SilverMain = new Color(0.753f, 0.753f, 0.753f, 1f);
        public static readonly Color SilverEmission = new Color(0.4f, 0.4f, 0.45f, 1f);
        public static readonly Color SilverOutline = new Color(0.6f, 0.6f, 0.65f, 1f);
        public static readonly Color SilverSpec = new Color(1f, 1f, 1f, 1f);
    }

    public static class WorldMap
    {
        public static readonly Color RoadColor = new Color(0.6f, 0.5f, 0.35f, 0.4f);
        public static readonly Color RoadInternalColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
        public static readonly Color RoadFriendlyColor = new Color(0.2f, 0.8f, 0.2f, 0.4f);
        public static readonly Color RoadNeutralColor = new Color(0.9f, 0.8f, 0.2f, 0.4f);
        public static readonly Color RoadHostileColor = new Color(0.9f, 0.2f, 0.2f, 0.4f);
    }

    public static class City
    {
        public static readonly Color HeroOverlayColor = new Color(0f, 0f, 0f, 0.92f);
        public static readonly Color WildHeroBorderColor = Color.yellow;
        public static readonly Color CapturedHeroBorderColor = Color.red;
        public static readonly Color LevelColor = new Color(0.56f, 0.93f, 0.56f, 1f);
    }
}
