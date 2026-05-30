using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using System;

public class RankCellForce : MonoBehaviour, IPointerDownHandler
{
    public RankPanelManager rankPanelManager;

    public int forceId;
    public TMP_Text forceName;
    public bool isSelect = false;
    public Image backgroundImage;

    // Start is called before the first frame update
    void Start()
    {
        forceName.raycastTarget = false;
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        UpdateBackgroundColor();
    }

    public void Init(string force)
    {
        foreach (var forceCfg in ForceConfig.ConfigList)
        {
            if (forceCfg.Cname == force)
            {
                forceId = forceCfg.Id;
                break;
            }
        }
        forceName.text = force;
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
        rankPanelManager.OnSelectForce(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
