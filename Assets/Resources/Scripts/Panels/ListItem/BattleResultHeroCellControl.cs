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
    public TMP_Text playerRank;

    private Outline heroOutline;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetData(BattleStatManager.BattleStat battleStat, int rank)
    {
        var heroLevel = 1;
        var heroCfg = HeroConfig.GetConfig(battleStat.heroId);
        playerName.text = heroLevel.ToString() + heroCfg.Name;

        playerRank.text = rank.ToString();
        playerMark1.text = "杀:" + battleStat.damage.ToString("F0");
        playerMark2.text = "死:" + battleStat.beDamaged.ToString("F0");

        heroIcon.sprite = Resources.Load<Sprite>(ResPath.Texture.HeroIcon(heroCfg.Icon));
        if (battleStat.isDead)
            heroIcon.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        else
            heroIcon.color = Color.white;

        if (battleStat.isCatched)
        {
            if (heroOutline == null)
            {
                heroOutline = heroIcon.gameObject.AddComponent<Outline>();
                heroOutline.effectDistance = new Vector2(3, -3);
            }
            heroOutline.effectColor = Color.red;
            heroOutline.enabled = true;
        }
        else
        {
            if (heroOutline != null)
                heroOutline.enabled = false;
        }
    }
}
