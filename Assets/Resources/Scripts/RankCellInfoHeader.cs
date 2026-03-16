using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankCellInfoHeader : MonoBehaviour, IRankDetailInfoHeader
{
    public RankPanelManager rankPanelManager;

    public Button btnLeadShip;
    public Button btnStr;
    public Button btnInte;
    public Button btnFair;
    public Button btnCharm;

    void Start()
    {
        btnLeadShip.onClick.AddListener(() =>
        {
            rankPanelManager.SortItems("LeadShip");
        });
        btnStr.onClick.AddListener(() =>
        {
            rankPanelManager.SortItems("Str");
        });
        btnInte.onClick.AddListener(() =>
        {
            rankPanelManager.SortItems("Inte");
        });
        btnFair.onClick.AddListener(() =>
        {
            rankPanelManager.SortItems("Fair");
        });
        btnCharm.onClick.AddListener(() =>
        {
            rankPanelManager.SortItems("Charm");
        });
    }
}