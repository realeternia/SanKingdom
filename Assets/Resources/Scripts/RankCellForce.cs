using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class RankCellForce : MonoBehaviour, IPointerDownHandler
{
    public RankPanelManager rankPanelManager;

    public TMP_Text forceName;

    // Start is called before the first frame update
    void Start()
    {
        forceName.raycastTarget = false;
    }

    public void Init(string force)
    {
        forceName.text = force;
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
