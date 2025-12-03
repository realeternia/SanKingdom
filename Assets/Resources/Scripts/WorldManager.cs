using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;

public class WorldManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 加载地图块
        LoadMapPieces();

        var roll = UnityEngine.Random.Range(0, 3);
        BGMPlayer.Instance.PlaySound(roll == 0 ? "BGMs/chun" : (roll == 1 ? "BGMs/xia" : "BGMs/qiu"));        

        GameManager.Instance.SaveToFile();
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
                if (texture == null)
                {
                    Debug.LogWarning($"找不到地图图片: {texturePath}");
                    continue;
                }

                // 创建精灵
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                // 从Prefabs/WorldPiece加载预设体
                GameObject worldPiecePrefab = Resources.Load<GameObject>("Prefabs/WorldPiece");
                if (worldPiecePrefab == null)
                {
                    Debug.LogError("找不到预设体: Prefabs/WorldPiece");
                    continue;
                }
                
                // 实例化预设体
                GameObject mapPiece = Instantiate(worldPiecePrefab, transform, false);
                mapPiece.name = worldConfig.Name;
                
                // 获取或添加Image组件
                Image image = mapPiece.GetComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true; // 保持宽高比

                WorldPieceControl pieceControl = mapPiece.GetComponent<WorldPieceControl>();
                pieceControl.pieceId = worldConfig.Id;
                
                // 使用RectTransform设置位置和大小
                RectTransform rectTransform = mapPiece.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = new Vector2(worldConfig.X/2+texture.width/2/2, -worldConfig.Y/2-texture.height/2/2);
                    
                    // 设置大小
                    rectTransform.sizeDelta = new Vector2(texture.width/2, texture.height/2);
                    
                    // 可以根据需要设置缩放
                    // rectTransform.localScale = new Vector3(mapConfig.Width / texture.width, mapConfig.Height / texture.height, 1);
                }

                pieceControl.InitForce();
                
                Debug.Log($"成功加载UI地图: {worldConfig.Cname} ({worldConfig.Name})");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载UI地图 {worldConfig.Cname} 时出错: {e.Message}");
            }
        }
    }
}
