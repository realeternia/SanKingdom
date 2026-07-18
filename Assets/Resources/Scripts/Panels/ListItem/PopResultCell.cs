using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class PopResultCell : MonoBehaviour
{
    public TMP_Text attrText;
    public TMP_Text attrValText;

    public void SetData(PopResultPanelManager.AttrData attrData)
    {
        if (!string.IsNullOrEmpty(attrData.attrStr))
        {
            attrText.text = attrData.attrStr;
        }
        else
        {
            var cityAttrCfg = CityAttrConfig.GetConfigByname(attrData.attr.ToLower());
            attrText.text = cityAttrCfg != null ? cityAttrCfg.Cname : attrData.attr;
        }

        if (!string.IsNullOrEmpty(attrData.valStr))
        {
            attrValText.text = attrData.valStr;
        }
        else
        {
            // a 显示扣除后的值（valOld + valAddon），.0 隐藏小数
            string newValStr = FormatNum(attrData.valOld + attrData.valAddon);
            string addonStr = FormatNum(Mathf.Abs(attrData.valAddon));
            if (attrData.valAddon > 0)
                attrValText.text = string.Format("{0}(<color=green>+{1}</color>)", newValStr, addonStr);
            else
                attrValText.text = string.Format("{0}(<color=red>-{1}</color>)", newValStr, addonStr);
        }
    }

    /// <summary>
    /// 数值格式化：整数值不显示小数，否则保留 1 位小数
    /// </summary>
    private static string FormatNum(float val)
    {
        if (val == Mathf.Floor(val))
            return ((int)val).ToString();
        return val.ToString("F1");
    }
}
