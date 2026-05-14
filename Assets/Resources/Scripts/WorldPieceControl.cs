using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;
public class WorldPieceControl : MonoBehaviour
{
    private const float MAP_SCALE_FACTOR = SystemConst.WorldMap.MAP_SCALE_FACTOR;
    
    private static WorldPieceControl currentActivePiece;
    
    public int pieceId;
    public Image pieceImage;
    public MainPanelManager worldManager;
    public TMP_Text pieceName;
    public GameObject infoNode;
    public TMP_Text extraText;
    public Button enterButton;

    private int extraMode = 1;

    private Dictionary<string, int> infos = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        if (pieceImage != null)
        {
            pieceImage.raycastTarget = true;
            pieceImage.alphaHitTestMinimumThreshold = 0.1f;
            
            Button button = pieceImage.GetComponent<Button>();
            if (button == null)
            {
                button = pieceImage.gameObject.AddComponent<Button>();
            }
            
            button.onClick.AddListener(OnPieceClicked);
        }
        
        enterButton.onClick.AddListener(OnEnterButtonClick);
        enterButton.gameObject.SetActive(false);
        
        infoNode.SetActive(false);
        extraText.gameObject.SetActive(false);
    }
    
    private void OnPieceClicked()
    {
        worldManager.OnPieceClick(pieceId);
        
        if (currentActivePiece != null && currentActivePiece != this)
        {
            currentActivePiece.enterButton.gameObject.SetActive(false);
        }
        
        var cityData = GameManager.Instance.GetCity(pieceId);
        var forceData = GameManager.Instance.GetForce(cityData.forceId);
        bool isPlayerCity = forceData.isPlayer;
        
        if (isPlayerCity)
        {
            var cityCfg = WorldConfig.GetConfig(pieceId);
            enterButton.image.color = Color.green;
            enterButton.GetComponentInChildren<TMP_Text>().text = "进入";
            enterButton.gameObject.SetActive(true);
            currentActivePiece = this;
        }
        else if (SysSwitch.CanViewOtherForceCity)
        {
            var cityCfg = WorldConfig.GetConfig(pieceId);
            enterButton.image.color = Color.yellow;
            enterButton.GetComponentInChildren<TMP_Text>().text = "查看";
            enterButton.gameObject.SetActive(true);
            currentActivePiece = this;
        }
        else
        {
            enterButton.gameObject.SetActive(false);
            if (currentActivePiece == this)
            {
                currentActivePiece = null;
            }
        }
    }
    
    private void OnEnterButtonClick()
    {
        PanelManager.Instance.ShowCity(pieceId);
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
            float offsetX = pieceCfg.MiniMapOffsets[0] * MAP_SCALE_FACTOR;
            float offsetY = pieceCfg.MiniMapOffsets[1] * MAP_SCALE_FACTOR;
            pieceName.rectTransform.anchoredPosition += new Vector2(offsetX, offsetY);
            extraText.rectTransform.anchoredPosition += new Vector2(offsetX, offsetY);
            infoNode.GetComponent<RectTransform>().anchoredPosition += new Vector2(offsetX, offsetY);
        }
    }

    public void SetColor(int forceId)
    {
        // 添加空值检查，确保代码健壮性
        if (pieceImage == null)
        {
            GameLog.Error("pieceImage is null");
            return;
        }

        // 获取force配置并检查是否为null
        var forceConfig = ForceConfig.GetConfig(forceId);
        if (forceConfig == null)
        {
            GameLog.Error($"找不到forceId为{forceId}的配置");
            return;
        }

       // GameLog.Debug($"设置颜色为{forceConfig.Color}");
        defaultColor = SysColor.GetForceColor(forceId);

        pieceImage.color = defaultColor;
        pieceName.color = SysColor.GetTextColorOnBackground(defaultColor);
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
            extraText.text = $"兵{GetTextByInt(city.GetAttr("soldier"))}";
            extraText.color = SysColor.Battle.FoodLossColor;
        }
        else if(mode == 3)
        {
            extraText.text = $"金{GetTextByInt(city.GetAttr("gold"))}";
            extraText.color = Color.yellow;
        }
    }

    private string GetTextByInt(int count)
    {
        if(count < 300)
            return " 空虚";
        else if(count < 1000)
            return " 少量";
        else if(count < 1000000)
            return $"{count / 1000.0:F1}K";
        else
            return $"{count / 1000000.0:F1}M";
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
            infoImageComp.sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.TextureByName(info.Key));
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
