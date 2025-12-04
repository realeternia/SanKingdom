using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class WorldPieceControl : MonoBehaviour
{
    public int pieceId;
    public Image pieceImage;
    public WorldManager worldManager;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitForce()
    {
        // 获取颜色字符串（格式为"R,G,B"）
        var pieceCfg = WorldConfig.GetConfig(pieceId);
        SetColor(pieceCfg.ForceId);
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
        
        string colorStr = forceConfig.Color;
        
        // 分割颜色字符串为RGB组件
        string[] rgbValues = colorStr.Split(',');
        
        // 检查是否有3个组件
        if (rgbValues.Length == 3)
        {
            // 转换为数值并除以255（Unity颜色值范围为0-1）
            float r = float.Parse(rgbValues[0]) / 255f;
            float g = float.Parse(rgbValues[1]) / 255f;
            float b = float.Parse(rgbValues[2]) / 255f;
            
            // 设置颜色，添加alpha值为1（不透明）
            pieceImage.color = new Color(r, g, b, 1f);
        }
    }
}
