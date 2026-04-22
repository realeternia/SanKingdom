using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleResultCellControl : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerMark;
    public TMP_Text playerRank;

    public Image playerIcon;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetData(SaveForceData force, int rank, int killMark)
    {
        playerName.text = force.Name;

        playerRank.text = rank.ToString();
        playerMark.text = $"<color=white>{force.mark}</color> (<color=green>+{killMark}</color>)";


        playerIcon.sprite = Resources.Load<Sprite>(force.IconPath);
    }
}
