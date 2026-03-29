using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class SystemPanelManager : MonoBehaviour
{
    public Button closeBtn;

    public Button rankBtn;
    public Button replayBtn;


    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() =>
        {      
            PanelManager.Instance.HideSystemPanel();
        });

        rankBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
        });

        replayBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowBattleResultPanel(1);
        });
    }


    public void OnShow()
    {

    }

    public void OnHide()
    {
    }


    // Update is called once per frame
    void Update()
    {

    }
}
