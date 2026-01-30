using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;

public class MainPanelManager : MonoBehaviour, IPanelEvent
{
    public GameObject topNode;      
    public CityDetail cityDetail;
    public Button btnRank;
    public Button btnCity;
    public Button btnRoundNext;
    public TMP_Text textYear;
    public TMP_Text textAiInfo;
    public GameObject bgPanel;
    private List<WorldPieceControl> worldPieces = new List<WorldPieceControl>();

    // Start is called before the first frame update
    void Start()
    {
        cityDetail.gameObject.SetActive(false);
        // 加载地图块
        LoadMapPieces();
        InitForceControls();

        GameManager.Instance.SaveToFile();

        btnCity.gameObject.SetActive(false);
        var nowRound = GameManager.Instance.SaveData.round;

        var seasonCfg = SeasonConfig.GetConfig((nowRound % 12) + 1);
        textYear.text = $"{nowRound / 12 + 185}年{seasonCfg.Name}";

        btnRank.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowRank();
        });        
        btnCity.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowCity(cityDetail.cityId);
        });
        btnRoundNext.onClick.AddListener(() =>
        {
            GameManager.Instance.NextRound();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitForceControls()
    {
        // 移除topNode下所有子对象
        foreach (Transform child in topNode.transform)
        {
            Destroy(child.gameObject);
        }

        var playerForceControl = Resources.Load<GameObject>("Prefabs/Panels/PlayerInfoCell");
        int idx = 0;

        var gameManager = GameManager.Instance;
        
        var totalWidth = 141 * gameManager.SaveData.forces.Count;
        var forceList = new List<int>();
        Debug.Log($"InitForceControls 强制数量: {gameManager.SaveData.forces.Count}");
        foreach(var force in gameManager.SaveData.forces)
            forceList.Add(force.forceId);
        forceList.Sort((a, b) => gameManager.GetPlayerCityCount(b) - gameManager.GetPlayerCityCount(a));
        foreach(var forceId in forceList)
        {
            var forceControl = Instantiate(playerForceControl, topNode.transform);
            var playerInfoControl = forceControl.GetComponent<PlayerInfoControl>();
            playerInfoControl.Init(forceId);
            forceControl.GetComponent<RectTransform>().anchoredPosition = new Vector2(-totalWidth / 2 + 141 * idx, 412);
            idx++;
        }
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
        if (GameManager.Instance.forbidPlayerAct)
        {
            Debug.LogWarning("当前轮次玩家已操作，不能点击地图");
            return;
        }

        var cityData = GameManager.Instance.GetCity(pieceId);
        if (!GameManager.Instance.GetForce(cityData.forceId).isPlayer)
        {
            cityDetail.gameObject.SetActive(true);
            cityDetail.SetCityDetail(pieceId); //可以看信息
            btnCity.gameObject.SetActive(false);
            return;
        }

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

        if(name == "CityForceChange")
        {
            var cityId = parm2;
            worldPieces.Find(x => x.pieceId == cityId).SetColor(GameManager.Instance.GetCity(cityId).forceId);

            InitForceControls();
        }
        else if(name == "RoundChange")
        {
            var nowRound = parm2;
            var seasonCfg = SeasonConfig.GetConfig((nowRound % 12) + 1);
            textYear.text = $"{nowRound / 12 + 185}年{seasonCfg.Name}";

            for (int i = 0; i < worldPieces.Count; i++)
            {
                var piece = worldPieces[i];
                var cityData = GameManager.Instance.GetCity(piece.pieceId);
                List<string> infos = new List<string>();
                foreach (var actionId in cityData.actions)
                {
                    var actionConfig = CityDevConfig.GetConfig(actionId);
                    if (actionConfig.ActionName == "")
                        continue;
                    infos.Add(actionConfig.ActionName);
                }
                // 对infos计数排序，生成Dictionary<string, int>
                var infosCount = new Dictionary<string, int>();
                foreach (var info in infos)
                {
                    if (infosCount.ContainsKey(info))
                        infosCount[info]++;
                    else
                        infosCount.Add(info, 1);
                }
                piece.SetInfos(infosCount);
            }
        }
        else if(name == "AICheck")
        {
            var playerName = parm1;
            var round = parm2;
            if (playerName == "")
            {
                textAiInfo.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                textAiInfo.transform.parent.gameObject.SetActive(true);
                textAiInfo.text = $"{playerName} 进行中";
            }
        }
    }    
}
