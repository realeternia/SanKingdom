using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;
using System.Collections.Generic;
public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    public TMP_Text[] textSkills;
    public TMP_Text textFriend;
    public RectTransform rect;
    public Image[] imageSkills;
    public int maxWidth = 300;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        // else
        //     Destroy(gameObject);

        gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    public void ShowTooltip(int[] skillIds, int heroId)
    {
        bool hasSkill = skillIds != null && skillIds.Length > 0;
        
        // 重置所有控件位置
        for(int i = 0; i < textSkills.Length; i++)
        {
            textSkills[i].gameObject.SetActive(skillIds!=null && skillIds.Length > i);
            imageSkills[i].gameObject.SetActive(skillIds!=null && skillIds.Length > i);
        }
        
        float currentY = 10f; // 起始Y位置
        
        // 调整背景大小
        float height = Mathf.Max(50f, currentY + 10f);
        rect.sizeDelta = new Vector2(400, height);
        
        // 调整位置 - 直接在屏幕坐标系下进行边界检测
        Vector2 mouseScreenPos = Input.mousePosition;
        
        // 获取屏幕尺寸
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        // 计算tooltip的宽高
        float tooltipWidth = rect.sizeDelta.x;
        float tooltipHeight = rect.sizeDelta.y;
        
        // 计算tooltip位置（鼠标右侧偏移30像素）
        
        
        // 边界判定：确保tooltip完全在屏幕内
        // X轴边界（左右边界）
        GameLog.Info("mouseScreenPos.x: " + mouseScreenPos.x + " w=" + tooltipWidth + " l=" + screenWidth);
        if (mouseScreenPos.x + tooltipWidth > screenWidth -tooltipWidth/2)
        {
            // 如果超出右边界，显示在鼠标左侧
            mouseScreenPos.x = screenWidth - tooltipWidth-tooltipWidth/2;
        }
        if (mouseScreenPos.x < 0)
        {
            // 如果超出左边界，紧贴左边缘
            mouseScreenPos.x = 10;
        }
        
        // Y轴边界（上下边界）
        if (mouseScreenPos.y < 0)
        {
            // 如果超出下边界，显示在鼠标上方
            mouseScreenPos.y = mouseScreenPos.y + 20;
        }
        if (mouseScreenPos.y + tooltipHeight > screenHeight)
        {
            // 如果超出上边界，紧贴顶部
            mouseScreenPos.y = screenHeight - tooltipHeight - 10;
        }
        Vector2 tooltipScreenPos = mouseScreenPos + new Vector2(30, -tooltipHeight/2);
        
        // 将屏幕坐标转换为Canvas局部坐标
        RectTransform canvasRect = transform.parent as RectTransform;
        if (canvasRect != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                tooltipScreenPos, 
                BattleManager.Instance.battleUIManager.uiCamera, 
                out localPoint);
            
            rect.anchoredPosition = localPoint;
        }
        
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}