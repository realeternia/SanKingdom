public static class NameTransTool
{

    public static string GetAttrName(string type)
    {
        switch (type.ToLower())
        {
            case "archgold":
                return "商业";
            case "archfood":
                return "农业";
            case "archpeople":
                return "人口";
            case "gold":
                return "金钱";
            case "food":
                return "粮食";
            case "soldier":
                return "士兵";
            case "secure":
                return "治安";
            case "power":
                return "士气";                
            case "wall":
                return "城防";
            default:
                return "";
        }
    }

    public static string GetAttrCName(string attr)
    {
        switch (attr.ToLower())
        {
            case "str":
                return "武力";
            case "inte":
                return "智力";
            case "fair":
                return "政治";
            case "leadship":
                return "统率";
            case "charm":
                return "魅力";
            default:
                return attr;
        }
    }
}