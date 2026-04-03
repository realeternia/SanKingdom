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
        if (!string.IsNullOrEmpty(attrData.valStr))
        {
            attrText.text = attrData.valStr;
        }
        else
        {
            var cityAttrCfg = CityAttrConfig.GetConfigByname(attrData.attr.ToLower());
            attrText.text = cityAttrCfg != null ? cityAttrCfg.Cname : attrData.attr;
        }
        if (attrData.valAddon > 0)
            attrValText.text = string.Format("{0}(<color=green>+{1}</color>)", attrData.valOld, attrData.valAddon);
        else
            attrValText.text = string.Format("{0}(<color=red>{1}</color>)", attrData.valOld, attrData.valAddon);        
    }
}
