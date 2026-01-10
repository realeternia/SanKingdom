public static class NameTransTool
{
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