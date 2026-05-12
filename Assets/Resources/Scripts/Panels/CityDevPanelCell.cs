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
        devIcon.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.BuildingIcon(cfg.Icon));
        devNameText.text = cfg.Cname;
    }

    public void OnSelect(bool isSelect)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormal;
        }
    }
    
    // 处理点击事件
    public void OnPointerClick(PointerEventData eventData)
    {
    }    
}
