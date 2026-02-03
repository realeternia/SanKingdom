using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;

public class WorldPieceControl : MonoBehaviour
{
    public int pieceId;
    public Image pieceImage;
    public MainPanelManager worldManager;
    public TMP_Text pieceName;
    public GameObject infoNode;
    public TMP_Text extraText;

    private int extraMode = 1;

    private Dictionary<string, int> infos = new Dictionary<string, int>();

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
        infoNode.SetActive(false);
        extraText.gameObject.SetActive(false);
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
        if(pieceCfg.MiniMapOffsets != null && pieceCfg.MiniMapOffsets.Length >= 2)
        {
            // 修改pieceName和infoNode的坐标偏移
            pieceName.rectTransform.anchoredPosition += new Vector2(pieceCfg.MiniMapOffsets[0], pieceCfg.MiniMapOffsets[1]);
            extraText.rectTransform.anchoredPosition += new Vector2(pieceCfg.MiniMapOffsets[0], pieceCfg.MiniMapOffsets[1]);
            infoNode.GetComponent<RectTransform>().anchoredPosition += new Vector2(pieceCfg.MiniMapOffsets[0], pieceCfg.MiniMapOffsets[1]);
        }
    }

    public void SetColor(int forceId)
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
        // 使用标准亮度公式：亮度 = 0.299 * R + 0.587 * G + 0.114 * B
        float brightness = 0.299f * defaultColor.r + 0.587f * defaultColor.g + 0.114f * defaultColor.b;
        if (brightness > 0.65f)
            pieceName.color = new Color(0.4f, 0.4f, 0.4f, 1);
        else
            pieceName.color = Color.white;
    }

    public void SetExtraMode(int mode = 0)
    {
        extraMode = mode;
        if(mode == 0)
        {
            infoNode.SetActive(false);
            extraText.gameObject.SetActive(false);
            return;
        }

        if(mode == 1)
        {
            infoNode.SetActive(true);
            extraText.gameObject.SetActive(false);
            UpdateInfoIcons();
            return;
        }

        infoNode.SetActive(false);
        extraText.gameObject.SetActive(true);
        var city = GameManager.Instance.GetCity(pieceId);

        if(mode == 2)
        {
            extraText.text = $"兵{city.soldier}";
            extraText.color = Color.red;
        }
        else if(mode == 3)
        {
            extraText.text = $"金{city.gold}";
            extraText.color = Color.yellow;
        }
    }

    public void OnRound(Dictionary<string, int> infos)
    {
        this.infos = infos;
        SetExtraMode(extraMode); // update
    }

    private void UpdateInfoIcons()
    {
        foreach (Transform child in infoNode.transform)
        {
            Destroy(child.gameObject);
        }
        int index = 0;
        foreach (var info in infos)
        {
            //创建GameObject，并添加组件Image
            var infoImage = new GameObject($"Info_{info.Key}");
            infoImage.transform.SetParent(infoNode.transform, false);
            var infoImageComp = infoImage.AddComponent<Image>();
            infoImageComp.sprite = Resources.Load<Sprite>($"Textures/{info.Key}");
            if(info.Value > 5)
                infoImageComp.color = Color.red;
            else if(info.Value >= 3)
                infoImageComp.color = Color.yellow;

            infoImageComp.transform.localPosition = new Vector3(index * 32 + 16 - infos.Count * 16, 0, 0);   
            infoImageComp.rectTransform.sizeDelta = new Vector2(32, 32);
            index++;
        }
    }
}
