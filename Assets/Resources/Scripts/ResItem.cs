using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CommonConfig;

public class ResItem : MonoBehaviour
{
    public Image itemImg;
    public TMP_Text itemNum;
    public string attrName;

    private int num;
    private int used;
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
        this.itemImg.sprite = Resources.Load<Sprite>("Textures/Icons/" + item.Icon);
    }

    public void UpdateNum(int num)
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

    private void RefreshDisplay()
    {
        if (used > 0)
        {
            itemNum.text = $"{num}(<color=red>{used}</color>)";
        }
        else
        {
            itemNum.text = num.ToString();
        }
    }

}
