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

    public string pname;
    public int pid;  //自增id
    public int forceId;  //配置表id

    public int gold;

    public int mark;

    public bool isAI = false;
    public int food;
    public int maxFood;
    private float lastFoodDeductionTime = 0f;

    public Color lineColor;

    public int sodatk = 0; //士兵atk强化
    public int sodhp = 0; //士兵def强化

    public int lastFightMark;

    public CastleHUD castleHUD;

    // Start is called before the first frame update
    void Start()
    {
  		targetImage = GetComponent<Image>();
    }

    public void Init(int id, int pid1)
    {
        pid = id;
        forceId = pid1;
        isAI = id > 0;

        gold = 0;
        maxFood = 100;
        food = maxFood;

        var forceCfg = ForceConfig.GetConfig(forceId);
        var heroCfg = HeroConfig.GetConfig(forceCfg.HeroId);

        lineColor = ColorUtility.TryParseHtmlString(forceCfg.Color, out lineColor) ? lineColor : Color.white;
        pname = heroCfg.Name;
        playerNameText.text = heroCfg.Name;
        imgPath = "Skins/" + heroCfg.Icon;
        playerImage.sprite = Resources.Load<Sprite>(imgPath);
        goldText.text = gold.ToString();
        resultText.text = mark.ToString();
        playerBgImg.color = lineColor;

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        PanelManager.Instance.SendSignal("SelectPlayer", "", pid);
    }

    public void AddGold(int g)
    {
        if(g <= 0)
         throw new ArgumentException("Gold must be greater than 0");

        gold += g;
        goldText.text = gold.ToString();
    }

    public void AddFood(int f)
    {
        if(f <= 0)
            throw new ArgumentException("Food must be greater than 0");

        food += f;
    }

    public void SubGold(int g, bool isHero)
    {
        gold -= g;
        goldText.text = gold.ToString();
    }
    
    public int SubFood(int f)
    {
        if(food <= 0)
            return 0;
        var sub = Mathf.Min(f, food);
        food -= sub;
        return sub;
    }

    public void OnBattleBegin()
    {
        food = maxFood;
        // 重置上次扣除粮食的时间为当前时间
        lastFoodDeductionTime = Time.time;
    }

    public void RoundFoodCost()
    {
        // 粮食扣除逻辑
        if (Time.time - lastFoodDeductionTime >= 5f) // 每5秒扣除一次粮食
        {
            // 计算时间差，每5s，扣10点粮食
            if(food < 10)
            {
                var units = BattleManager.Instance.GetUnitsMySide(1); //todo
                foreach(var unit in units)
                    unit.LackFood((float)(10 - food) / 10);
            }
            food -= 10;
            if (food < 0) food = 0;

            // 更新上次扣除粮食的时间
            lastFoodDeductionTime = Time.time;
        }
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
        mark += add;
        resultText.text = mark.ToString();
    }

}

