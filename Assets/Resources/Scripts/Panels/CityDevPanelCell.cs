using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using UnityEngine.EventSystems;

public class CityDevPanelCell : MonoBehaviour, IPointerClickHandler
{
    public CityDevPanelManager cityDevPanelManager;
    public int devId;
    public Image devIcon;
    public TMP_Text devNameText;
    public Image backgroundImage;

    private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); // 正常状态背景色
    private Color selectedColor = new Color(0.5f, 0.5f, 0.1f, 0.8f); // 高光绿色选中状态

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int devId)
    {
        this.devId = devId;
        var cfg = CityDevConfig.GetConfig(devId);
        devIcon.sprite = Resources.Load<Sprite>("Textures/Buildings/" + cfg.Icon);
        devNameText.text = cfg.Cname;
    }

    public void OnSelect(bool isSelect)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? selectedColor : normalColor;
        }
    }
    
    // 处理点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
        // 通知面板管理器当前单元格被点击
        cityDevPanelManager.OnSelectItem(this);
    }    
}
