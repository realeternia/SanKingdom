using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using Controls.Utils;

public class BattleInfoTop : MonoBehaviour
{
    public static BattleInfoTop Instance;

    public Canvas canvas;

    public BattleForceBar leftTopBar;
    public BattleForceBar rightTopBar;
    public Image leftImg;
    public Image rightImg;
    public TMP_Text roundText;

    private int leftSoldierTotal;
    private int rightSoldierTotal;

    public void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int leftForceId, int rightForceId, int leftSoldierTotal, int rightSoldierTotal)
    {
        // 设置leftImg和rightImg的图片
        SetForceImage(leftImg, leftForceId);
        SetForceImage(rightImg, rightForceId);
        
        //这里改成获得整个canvas的宽度
        float totalWidth = canvas.GetComponent<RectTransform>().sizeDelta.x;
        GameLog.Info($"totalWidth: {totalWidth}");

        // 获取leftImg和rightImg的宽度
        float imgWidth = 80;
        
        // 计算中间可用空间宽度
        float availableWidth = totalWidth - imgWidth - imgWidth;

        this.leftSoldierTotal = leftSoldierTotal;
        this.rightSoldierTotal = rightSoldierTotal;

        // 计算兵力比例
        int totalSoldiers = leftSoldierTotal + rightSoldierTotal;
        float leftRatio = (float)leftSoldierTotal / totalSoldiers;
        if(leftRatio > 0.95f)
            leftRatio = 0.95f;
        else if(leftRatio < 0.05f)
            leftRatio = 0.05f;
        float rightRatio = 1 - leftRatio;
        
        // 设置leftTopBar和rightTopBar的宽度
        float leftBarWidth = availableWidth * leftRatio;
        float rightBarWidth = availableWidth * rightRatio;

        
        // 更新leftTopBar和rightTopBar的RectTransform
        if (leftTopBar != null)
        {
            RectTransform leftBarRect = leftTopBar.GetComponent<RectTransform>();
            leftBarRect.sizeDelta = new Vector2(leftBarWidth, leftBarRect.sizeDelta.y);
            leftBarRect.anchoredPosition = new Vector2(-totalWidth / 2 + imgWidth + leftBarWidth / 2, leftBarRect.anchoredPosition.y);
        }
        
        if (rightTopBar != null)
        {
            RectTransform rightBarRect = rightTopBar.GetComponent<RectTransform>();
            rightBarRect.sizeDelta = new Vector2(rightBarWidth, rightBarRect.sizeDelta.y);
            rightBarRect.anchoredPosition = new Vector2(totalWidth / 2 - imgWidth - rightBarWidth / 2, rightBarRect.anchoredPosition.y);
        }


        // 设置leftImg和rightImg的位置（最左和最右）
        leftImg.rectTransform.anchoredPosition =  new Vector2(-availableWidth / 2 - imgWidth, leftImg.rectTransform.anchoredPosition.y);
        rightImg.rectTransform.anchoredPosition = new Vector2(availableWidth / 2, rightImg.rectTransform.anchoredPosition.y);   

        leftTopBar.forceImg.color = ColorUtility.TryParseHtmlString(ForceConfig.GetConfig(leftForceId).Color, out var wColor) ? wColor : Color.green;
        rightTopBar.forceImg.color = ColorUtility.TryParseHtmlString(ForceConfig.GetConfig(rightForceId).Color, out var wColor2) ? wColor2 : Color.green;

        leftTopBar.SetRate(1);
        rightTopBar.SetRate(1);
    }
    
    private void SetForceImage(Image image, int forceId)
    {
        // 获取force配置
        var forceConfig = ForceConfig.GetConfig(forceId);
        
        // 获取hero配置
        var heroConfig = HeroConfig.GetConfig(forceConfig.HeroId);
        
        // 加载图片
        string imgPath = "Textures/Skins/" + heroConfig.Icon;
        Sprite sprite = Resources.Load<Sprite>(imgPath);
        if (sprite != null)
        {
            image.sprite = sprite;
        }
        else
        {
            GameLog.Error($"无法加载图片: {imgPath}");
        }
    }

    public void UpdateSoldierCount( int leftSoldierTotal, int rightSoldierTotal)
    {
        leftTopBar.SetRate((float)leftSoldierTotal / this.leftSoldierTotal);
        rightTopBar.SetRate((float)rightSoldierTotal / this.rightSoldierTotal);
        // UnityEngine.Debug.Log($"UpdateSoldierCount: {leftSoldierTotal}/{this.leftSoldierTotal} vs {rightSoldierTotal}/{this.rightSoldierTotal}");
    }

    public void UpdateRound(int round, int maxRound)
    {
        if (roundText != null)
            roundText.text = $"{round}/{maxRound}合";
    }
}
