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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(string name, int num)
    {
        var item = CityAttrConfig.GetConfigByname(name);
        if (item == null)
            return;
        this.attrName = name;
        this.itemImg.sprite = Resources.Load<Sprite>("Textures/Icons/" + item.Icon);
        this.itemNum.text = num.ToString();
    }
}
