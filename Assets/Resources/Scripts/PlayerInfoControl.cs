using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using CommonConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerInfoControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Image targetImage;
    public float blinkDuration = 1f;
    public Color startColor = Color.white;
    public Color endColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    private float timer = 0f;
    
    public bool isOnTurn;
    public TMP_Text playerNameText;
    public Image playerImage;
    public string imgPath;
    public TMP_Text goldText;
    public TMP_Text resultText;
    public Image playerBgImg;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
    }

    public void Init(int id, int forceId)
    {
        player = new Player();
        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);
        imgPath = "Skins/" + heroCfg.Icon;
        player.Init(id, forceId, imgPath);

        playerNameText.text = player.pname;

        playerImage.sprite = Resources.Load<Sprite>(player.imgPath);
        goldText.text = player.gold.ToString();
        resultText.text = player.mark.ToString();
        playerBgImg.color = player.lineColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        PanelManager.Instance.SendSignal("SelectPlayer", "", player.pid);
    }

    public void AddGold(int g)
    {
        player.AddGold(g);
        goldText.text = player.gold.ToString();
    }

    public void AddFood(int f)
    {
        player.AddFood(f);
    }

    public void SubGold(int g, bool isHero)
    {
        player.SubGold(g, isHero);
        goldText.text = player.gold.ToString();
    }
    
    public int SubFood(int f)
    {
        return player.SubFood(f);
    }

    public void OnBattleBegin()
    {
        player.OnBattleBegin();
    }

    public void RoundFoodCost()
    {
        player.RoundFoodCost();
    }
    
    // Update is called once per frame
    void Update()
    {
        // 现有的闪烁逻辑
        if (isOnTurn)
        {
            if (targetImage != null)
            {
                timer += Time.deltaTime;
                // 使用正弦函数计算插值因子，范围在 0 到 1 之间
                float t = (Mathf.Sin((timer / blinkDuration) * Mathf.PI * 2f) + 1f) / 2f;
                // 根据插值因子在 startColor 和 endColor 之间做差值
                targetImage.color = Color.Lerp(startColor, endColor, t);
                // 重置计时器，让其循环
                timer %= blinkDuration;
            }
        }
        else
        {
            if(targetImage != null)
            {
                if(targetImage.color != new Color(0.1f, 0.1f, 0.1f, 0.8f))
                {
                    targetImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
                }
            }
        }
    }

    public void onBattleResult(bool isWin, int add)
    {
        player.onBattleResult(isWin, add);
        resultText.text = player.mark.ToString();
    }

}