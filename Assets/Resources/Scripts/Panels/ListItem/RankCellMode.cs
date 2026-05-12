using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class RankCellMode : MonoBehaviour, IPointerDownHandler
{
    public RankPanelManager rankPanelManager;

    public TMP_Text modeName;
    public bool isSelect = false;
    public Image backgroundImage;

    // Start is called before the first frame update
    void Start()
    {
        modeName.raycastTarget = false;
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        UpdateBackgroundColor();
    }

    public void Init(string mode)
    {
        modeName.text = mode;
    }

    public void SetSelected(bool selected)
    {
        isSelect = selected;
        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormal;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rankPanelManager.OnSelectMode(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
