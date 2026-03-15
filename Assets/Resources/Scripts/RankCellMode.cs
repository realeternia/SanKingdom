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

    // Start is called before the first frame update
    void Start()
    {
        modeName.raycastTarget = false;
    }

    public void Init(string mode)
    {
        modeName.text = mode;
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
