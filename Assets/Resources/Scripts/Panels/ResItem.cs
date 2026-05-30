using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;

public class ResItem : MonoBehaviour
{
    public TMP_Text itemNum;
    public string attrName;

    private float num;
    private int used;
    private float addon;
    private CityAttrConfig config;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Init(string name)
    {
        var item = CityAttrConfig.GetConfigByname(name);
        if (item == null)
            return;
        this.attrName = name;
        this.config = item;

        GetComponent<IconLoader>().SetId(item.Id);
    }

    public void UpdateNum(float num)
    {
        this.num = num;
        RefreshDisplay();
    }

    public void UpdateUsed(int used)
    {
        if (config == null || !config.IsPosRes)
            return;
        this.used = used;
        RefreshDisplay();
    }

    public void UpdateAddon(float addon)
    {
        if (config == null || config.IsPosRes)
            return;
        this.addon = addon;
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        string numStr = num.ToString("F0");
        if (config != null && config.IsPosRes)
        {
            if (used > 0)
            {
                itemNum.text = $"{numStr}(<color=red>{used}</color>)";
            }
            else
            {
                itemNum.text = numStr;
            }
        }
        else
        {
            string addonStr = addon.ToString("F0");
            if (addon > 0)
            {
                itemNum.text = $"{numStr}(<color=green>+{addonStr}</color>)";
            }
            else if (addon < 0)
            {
                itemNum.text = $"{numStr}(<color=red>{addonStr}</color>)";
            }
            else
            {
                itemNum.text = numStr;
            }
        }
    }

}
