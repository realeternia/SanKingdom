using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;

public class MainPanelManager : MonoBehaviour, IPanelEvent
{
    public CityDetail cityDetail;
    public Button btnRank;
    public Button btnCity;
    public GameObject bgPanel;
    private List<WorldPieceControl> worldPieces = new List<WorldPieceControl>();

    // Start is called before the first frame update
    void Start()
    {
        cityDetail.gameObject.SetActive(false);
        // 加载地图块
        LoadMapPieces();

        GameManager.Instance.SaveToFile();

        btnCity.gameObject.SetActive(false);

        btnRank.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
        });        
        btnCity.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowCity(cityDetail.cityId);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadMapPieces()
    {
        UnityEngine.Debug.Log($"LoadMapPieces 地图数量: {WorldConfig.ConfigList.Count}");
        
        // // 检查地图配置是否为空
        // if (WorldConfig.ConfigList.Count == 0)
        // {
        //     Debug.LogWarning("WorldConfig配置为空，等待0.2秒后重试...");
        //     // 等待0.2秒
        //     yield return new WaitForSeconds(0.2f);
        //     // 重新调用自身
        //     StartCoroutine(LoadMapPieces());
        //     yield break;
        // }

        // 遍历所有地图配置
        foreach (var worldConfig in WorldConfig.ConfigList)
        {
            try
            {
                // 构建图片路径（Resources/Textures/Maps/下的图片）
                string texturePath = "Textures/Maps/" + worldConfig.Name;
                
                // 加载图片资源
                Texture2D texture = Resources.Load<Texture2D>(texturePath);

                // 创建精灵
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                // 从Prefabs/WorldPiece加载预设体
                GameObject worldPiecePrefab = Resources.Load<GameObject>("Prefabs/WorldPiece");
                
                // 实例化预设体
                GameObject mapPiece = Instantiate(worldPiecePrefab, bgPanel.transform, false);
                mapPiece.name = worldConfig.Name;
                
                // 获取或添加Image组件
                Image image = mapPiece.GetComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true; // 保持宽高比

                WorldPieceControl pieceControl = mapPiece.GetComponent<WorldPieceControl>();
                pieceControl.worldManager = this;

                pieceControl.pieceId = worldConfig.Id;
                
                // 使用RectTransform设置位置和大小
                RectTransform rectTransform = mapPiece.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(worldConfig.X/2+texture.width/2/2, -worldConfig.Y/2-texture.height/2/2);
                    
                    // 设置大小
                    rectTransform.sizeDelta = new Vector2(texture.width/2, texture.height/2);
                }

                pieceControl.InitForce();
                worldPieces.Add(pieceControl);
                
                Debug.Log($"成功加载UI地图: {worldConfig.Cname} ({worldConfig.Name})");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载UI地图 {worldConfig.Cname} 时出错: {e.Message}");
            }
        }
    }

    public void OnPieceClick(int pieceId)
    {
        cityDetail.gameObject.SetActive(true);
        cityDetail.SetCityDetail(pieceId);
        // 高亮显示点击的地块
        var cityCfg = WorldConfig.GetConfig(pieceId);
        // foreach (var piece in worldPieces)
        // {
        //     piece.Shine(cityCfg.WorldNearIds != null && Array.Exists(cityCfg.WorldNearIds, x => x == piece.pieceId));
        // }
        btnCity.gameObject.GetComponentInChildren<TMP_Text>().text = "进入" + cityCfg.Cname;
        btnCity.gameObject.SetActive(true);
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        Debug.Log($"WorldManager SendSignal {name} {parm1} {parm2}");
        cityDetail.SendSignal(name, parm1, parm2);
    }    
}
