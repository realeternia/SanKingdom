using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class ResCheckItem : MonoBehaviour
{
    public TMP_Text itemNum;
    public Image icon;

    private float num;
    private int cost;
    private CityAttrConfig config;
    private bool isSpecial;

    public void Init(string attrName)
    {
        var item = CityAttrConfig.GetConfigByname(attrName);
        if (item == null)
            return;
        this.config = item;
        this.isSpecial = false;

        GetComponent<IconLoader>().SetId(item.Id);
        if (!string.IsNullOrEmpty(item.Icon))
        {
            icon.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.AttrIcon(item.Icon));
        }
    }

    public void Init(string iconPath, string displayText)
    {
        this.isSpecial = true;
        this.config = null;

        icon.sprite = ResourceCache.LoadSpriteUI(iconPath);
        itemNum.text = displayText;
    }

    public void UpdateNum(float num)
    {
        this.num = num;
        RefreshDisplay();
    }

    public void UpdateCost(int cost)
    {
        this.cost = cost;
        RefreshDisplay();
    }

    public void UpdateDisplay(string text)
    {
        itemNum.text = text;
    }

    private void RefreshDisplay()
    {
        if (isSpecial)
            return;

        string numStr = num.ToString("F0");
        if (cost > 0)
        {
            itemNum.text = $"{numStr}(<color=red>-{cost}</color>)";
        }
        else
        {
            itemNum.text = numStr;
        }
    }
}
