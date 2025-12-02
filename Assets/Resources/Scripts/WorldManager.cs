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
        StartCoroutine(LoadMapPieces());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadMapPieces()
    {
        UnityEngine.Debug.Log($"LoadMapPieces 地图数量: {WorldConfig.ConfigList.Count}");
        
        // 检查地图配置是否为空
        if (WorldConfig.ConfigList.Count == 0)
        {
            Debug.LogWarning("WorldConfig配置为空，等待0.2秒后重试...");
            // 等待0.2秒
            yield return new WaitForSeconds(0.2f);
            // 重新调用自身
            StartCoroutine(LoadMapPieces());
            yield break;
        }
        
        // // 确保有Canvas组件作为UI父对象
        // Canvas mapCanvas = GetComponent<Canvas>();
        // if (mapCanvas == null)
        // {
        //     mapCanvas = gameObject.AddComponent<Canvas>();
        //     mapCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
        //     // 添加CanvasScaler组件以确保UI正确缩放
        //     CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        //     scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        //     scaler.referenceResolution = new Vector2(1920, 1080);
        //     scaler.matchWidthOrHeight = 0.5f;
            
        //     // 添加GraphicRaycaster以便UI可以接收事件
        //     gameObject.AddComponent<GraphicRaycaster>();
        // }
        
        // 遍历所有地图配置
        foreach (var mapConfig in WorldConfig.ConfigList)
        {
            try
            {
                // 构建图片路径（Resources/Textures/Maps/下的图片）
                string texturePath = "Textures/Maps/" + mapConfig.Name;
                
                // 加载图片资源
                Texture2D texture = Resources.Load<Texture2D>(texturePath);
                if (texture == null)
                {
                    Debug.LogWarning($"找不到地图图片: {texturePath}");
                    continue;
                }

                // 创建精灵
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                // 创建UI GameObject
                GameObject mapPiece = new GameObject(mapConfig.Cname);
                mapPiece.transform.SetParent(transform, false); // false表示不保持世界位置
                
                // 添加Image组件代替SpriteRenderer
                Image image = mapPiece.AddComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true; // 保持宽高比
                
                // 使用RectTransform设置位置和大小
                RectTransform rectTransform = mapPiece.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // 设置锚点和位置（使用配置中的X,Y值，可能需要根据UI系统调整）
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchoredPosition = new Vector2(mapConfig.X/2+texture.width/2/2, -mapConfig.Y/2-texture.height/2/2);
                    
                    // 设置大小
                    rectTransform.sizeDelta = new Vector2(texture.width/2, texture.height/2);
                    
                    // 可以根据需要设置缩放
                    // rectTransform.localScale = new Vector3(mapConfig.Width / texture.width, mapConfig.Height / texture.height, 1);
                }
                
                Debug.Log($"成功加载UI地图: {mapConfig.Cname} ({mapConfig.Name})");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"加载UI地图 {mapConfig.Cname} 时出错: {e.Message}");
            }
        }
    }
}
