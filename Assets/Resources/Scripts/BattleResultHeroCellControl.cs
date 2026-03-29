using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class BattleResultHeroCellControl : MonoBehaviour
{
    public Image heroIcon;
    public TMP_Text playerName;
    public TMP_Text playerMark1;
    public TMP_Text playerMark2;
    public TMP_Text playerMark3;
    public TMP_Text playerRank;

    public Image playerIcon;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(BattleStatManager.BattleStat battleStat, int rank)
    {
        var player = GameManager.Instance.GetPlayer(battleStat.playerId);
        var heroLevel = 1;
        var heroCfg = HeroConfig.GetConfig(battleStat.heroId);
        playerName.text = heroLevel.ToString() + heroCfg.Name;

        playerRank.text = rank.ToString();
        playerMark1.text = "伤:" + battleStat.damage.ToString("F0");
        playerMark2.text = "杀:" + battleStat.killSoldier.ToString();
        playerMark3.text = "亡:" + battleStat.lostSoldier.ToString() + (battleStat.isDead ? "(亡)" : "");

        playerIcon.sprite = Resources.Load<Sprite>(player.imgPath);
        heroIcon.sprite = Resources.Load<Sprite>("Skins/" + heroCfg.Icon);
    }
}
