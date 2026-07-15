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
        // 先按 name 查找，找不到再按 icon 查找
        var item = CityAttrConfig.GetConfigByname(attrName);
        if (item == null) item = CityAttrConfig.GetConfigByname(attrName);

        if (item != null)
        {
            this.config = item;
            this.isSpecial = false;
            GetComponent<IconLoader>().SetId(item.Id);
            if (!string.IsNullOrEmpty(item.Icon))
            {
                icon.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.AttrIcon(item.Icon));
            }
        }
        else
        {
            // 非城市属性 icon（如 devId 专用图标），无 tooltip
            this.config = null;
            this.isSpecial = true;
            icon.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.AttrIcon(attrName));
        }
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
