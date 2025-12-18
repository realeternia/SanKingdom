using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class WorldPieceControl : MonoBehaviour
{
    public int pieceId;
    public Image pieceImage;
    public WorldManager worldManager;
    public TMP_Text pieceName;
    // Start is called before the first frame update
    void Start()
    {
        // 确保pieceImage存在并添加点击事件监听器
        if (pieceImage != null)
        {
            pieceImage.raycastTarget = true; // 确保可以接收点击事件
            pieceImage.alphaHitTestMinimumThreshold = 0.1f; // 设置点击检测的最小alpha阈值
            
            // 获取或添加Button组件
            Button button = pieceImage.GetComponent<Button>();
            if (button == null)
            {
                // 如果没有Button组件，则添加一个
                button = pieceImage.gameObject.AddComponent<Button>();
            }
            
            // 添加点击事件监听
            button.onClick.AddListener(OnPieceClicked);
        }
    }
    
    /// <summary>
    /// 处理地块点击事件
    /// </summary>
    private void OnPieceClicked()
    {
        worldManager.OnPieceClick(pieceId);
    }

    private Color defaultColor;
    public void Shine(bool isShine)
    {
        if (isShine)
        {
            pieceImage.color = Color.white;
        }
        else
        {
            pieceImage.color = defaultColor;
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitForce()
    {
        // 获取颜色字符串（格式为"R,G,B"）
        var pieceCfg = WorldConfig.GetConfig(pieceId);
        var city = GameManager.Instance.GetCity(pieceId);
        SetColor(city.forceId);

        // 设置名称
        pieceName.text = pieceCfg.Cname;
    }

    private void SetColor(int forceId)
    {
        // 添加空值检查，确保代码健壮性
        if (pieceImage == null)
        {
            Debug.LogError("pieceImage is null");
            return;
        }
        
        // 获取force配置并检查是否为null
        var forceConfig = ForceConfig.GetConfig(forceId);
        if (forceConfig == null)
        {
            Debug.LogError($"找不到forceId为{forceId}的配置");
            return;
        }

        Debug.Log($"设置颜色为{forceConfig.Color}");
        defaultColor = ColorUtility.TryParseHtmlString(forceConfig.Color, out var wColor) ? wColor : Color.white;
        pieceImage.color = defaultColor;
    }
}
