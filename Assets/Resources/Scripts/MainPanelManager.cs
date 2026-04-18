using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;
using Controls.Utils;
using System.Linq;

public class MainPanelManager : MonoBehaviour, IPanelEvent
{
    private const float MAP_SCALE_FACTOR = 1.25f;
    
    public GameObject topNode;      
    public CityDetail cityDetail;
    public Button btnSystem;
    public Button btnCity;
    public Button btnRoundNext;
    public Button btnMode;

    public TMP_Text textYear;
    public TMP_Text textAiInfo;
    public GameObject bgPanel;
    public VideoPanelManager videoPanelManager;
    private MapDragHandler mapDragHandler;
    private List<WorldPieceControl> worldPieces = new List<WorldPieceControl>();

    // Start is called before the first frame update
    void Start()
    {
        cityDetail.gameObject.SetActive(false);
        LoadMapPieces();
        InitDragHandler();
        InitForceControls();

        btnCity.gameObject.SetActive(false);
        var nowRound = GameManager.Instance.SaveData.round;
        
        var seasonId = GameManager.Instance.SeasonId;
        var seasonCfg = SeasonConfig.GetConfig(seasonId);
        // 使用GameManager的常量计算年份
        int years = nowRound / GameManager.SEASONS_PER_YEAR;
        textYear.text = $"{GameManager.BASE_YEAR + years}年{seasonCfg.Name}";

        btnSystem.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowSystemPanel();
        });        
        btnCity.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowCity(cityDetail.cityId);
        });
        btnRoundNext.onClick.AddListener(() =>
        {
            GameManager.Instance.NextRound();
        });
        btnMode.onClick.AddListener(() =>
        {
            SetMode();
        });
        
        StartCoroutine(MoveToPlayerCapitalDelayed());
    }

    void Update()
    {
        
    }

    private void InitDragHandler()
    {
        if (bgPanel == null)
            return;
        
        RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
        if (bgRect == null)
            return;
        
        GameLog.Info($"InitDragHandler: bgPanel pivot = {bgRect.pivot}, anchorMin = {bgRect.anchorMin}, anchorMax = {bgRect.anchorMax}");
        GameLog.Info($"InitDragHandler: bgPanel sizeDelta = {bgRect.sizeDelta}, rect.size = {bgRect.rect.size}");
        
        mapDragHandler = bgPanel.GetComponent<MapDragHandler>();
        if (mapDragHandler == null)
        {
            mapDragHandler = bgPanel.AddComponent<MapDragHandler>();
        }
        
        RectTransform viewportRect = bgRect.parent as RectTransform;
        if (viewportRect != null)
        {
            GameLog.Info($"InitDragHandler: viewport pivot = {viewportRect.pivot}, sizeDelta = {viewportRect.sizeDelta}, rect.size = {viewportRect.rect.size}");
        }
        
        mapDragHandler.Initialize(bgRect, viewportRect);
    }
    
    private IEnumerator MoveToPlayerCapitalDelayed()
    {
        yield return null;
        MoveToPlayerCapital();
    }

    private void MoveToPlayerCapital()
    {
        GameLog.Info("MoveToPlayerCapital 开始执行");
        
        if (mapDragHandler == null)
        {
            GameLog.Warn("MoveToPlayerCapital: mapDragHandler 为空");
            return;
        }
        
        var playerForce = GameManager.Instance.SaveData.forces.FirstOrDefault(f => f.isPlayer);
        if (playerForce == null)
        {
            GameLog.Warn("MoveToPlayerCapital: 未找到玩家势力");
            return;
        }
        
        var player = GameManager.Instance.GetPlayer(playerForce.forceId);
        if (player == null)
        {
            GameLog.Warn("MoveToPlayerCapital: 未找到玩家对象");
            return;
        }
        
        var kingCity = player.GetKingCity();
        if (kingCity == null)
        {
            GameLog.Warn("MoveToPlayerCapital: 未找到首都城市");
            return;
        }
        
        var cityConfig = WorldConfig.GetConfig(kingCity.cityId);
        if (cityConfig == null)
        {
            GameLog.Warn("MoveToPlayerCapital: 未找到城市配置");
            return;
        }
        GameLog.Info($"MoveToPlayerCapital: 城市名称 = {cityConfig.Cname}, X = {cityConfig.X}, Y = {cityConfig.Y}");
        
        string texturePath = "Textures/Maps/" + cityConfig.Name;
        Texture2D texture = Resources.Load<Texture2D>(texturePath);
        if (texture == null)
        {
            GameLog.Warn($"MoveToPlayerCapital: 未找到纹理, path = {texturePath}");
            return;
        }
        GameLog.Info($"MoveToPlayerCapital: 纹理尺寸 = {texture.width} x {texture.height}");

        RectTransform bgRect = bgPanel.GetComponent<RectTransform>();
        
        float bgWidth = bgRect.rect.width;
        float bgHeight = bgRect.rect.height;
        
        float cityLocalX = cityConfig.X * MAP_SCALE_FACTOR - bgWidth / 2;
        float cityLocalY = bgHeight / 2 - cityConfig.Y * MAP_SCALE_FACTOR;
        
        Vector2 targetPos = new Vector2(-cityLocalX, -cityLocalY);
        
        GameLog.Info($"MoveToPlayerCapital: bgSize = ({bgWidth}, {bgHeight}), cityLocal = ({cityLocalX}, {cityLocalY}), targetPos = ({targetPos.x}, {targetPos.y})");
        
        mapDragHandler.MoveToPositionSmooth(targetPos);
        GameLog.Info($"MoveToPlayerCapital: 平滑移动开始, bgPanel位置 = {bgPanel.GetComponent<RectTransform>().anchoredPosition}");
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
        
        var activeForces = gameManager.SaveData.forces.Where(f => !f.isEliminated).ToList();
        var totalWidth = 141 * activeForces.Count;
        var forceList = new List<int>();

        GameLog.Info($"InitForceControls 势力数量: {activeForces.Count}");
        foreach(var force in activeForces)
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
        GameLog.Info($"LoadMapPieces 地图数量: {WorldConfig.ConfigList.Count}");

        // 遍历所有地图配置
        foreach (var worldConfig in WorldConfig.ConfigList)
        {
            // try
            // {
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
                    rectTransform.anchoredPosition = new Vector2(
                        worldConfig.X * MAP_SCALE_FACTOR + texture.width * MAP_SCALE_FACTOR / 2,
                        -worldConfig.Y * MAP_SCALE_FACTOR - texture.height * MAP_SCALE_FACTOR / 2
                    );
                    
                    rectTransform.sizeDelta = new Vector2(texture.width * MAP_SCALE_FACTOR, texture.height * MAP_SCALE_FACTOR);
                }

                pieceControl.InitForce();
                worldPieces.Add(pieceControl);
        }
    }

    public void OnPieceClick(int pieceId)
    {
        if (GameManager.Instance.forbidPlayerAct)
        {
            GameLog.Warn("当前轮次玩家已操作，不能点击地图");
            return;
        }

        cityDetail.gameObject.SetActive(true);
        cityDetail.SetCityDetail(pieceId); //可以看信息

        var cityData = GameManager.Instance.GetCity(pieceId);
        if (!GameManager.Instance.GetForce(cityData.forceId).isPlayer)
        {
            btnCity.gameObject.SetActive(false);
            return;
        }

        // 高亮显示点击的地块
        var cityCfg = WorldConfig.GetConfig(pieceId);
        // foreach (var piece in worldPieces)
        // {
        //     piece.Shine(cityCfg.WorldNearIds != null && Array.Exists(cityCfg.WorldNearIds, x => x == piece.pieceId));
        // }
        btnCity.gameObject.GetComponentInChildren<TMP_Text>().text = "进入" + cityCfg.Cname;
        btnCity.gameObject.SetActive(true);
    }

    private int extraMode = 0;
    public void SetMode()
    {
        extraMode = (extraMode + 1) % 4;
        foreach (var piece in worldPieces)
        {
            piece.SetExtraMode(extraMode);
        }
    }

    public void SendSignal(string name, string parm1, int parm2)
    {
        GameLog.Debug($"WorldManager SendSignal {name} {parm1} {parm2}");
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
            var seasonId = GameManager.Instance.SeasonId;
            var seasonCfg = SeasonConfig.GetConfig(seasonId);
            // 使用GameManager的常量计算年份
            int years = nowRound / GameManager.SEASONS_PER_YEAR;
            textYear.text = $"{GameManager.BASE_YEAR + years}年{seasonCfg.Name}";

            for (int i = 0; i < worldPieces.Count; i++)
            {
                var piece = worldPieces[i];
                var cityData = GameManager.Instance.GetCity(piece.pieceId);
                var infosCount = new Dictionary<string, int>();
                foreach (var actionId in cityData.actions)
                {
                    var actionConfig = CityDevConfig.GetConfig(actionId.Key);
                    if (actionConfig.ActionName == "")
                        continue;

                    infosCount[actionConfig.ActionName] = actionId.Value;
                }
                GameLog.Debug($"CityForceChange {cityData.forceId} {cityData.actions.Count} {infosCount.Count}");
                piece.OnRound(infosCount);
            }

            if(seasonCfg.Video != "")
                videoPanelManager.Play(seasonCfg.Video);
            
            MoveToPlayerCapital();
        }
        else if(name == "AICheck")
        {
            var playerName = parm1;
            var forceId = parm2;
            if (playerName == "")
            {
                textAiInfo.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                textAiInfo.transform.parent.gameObject.SetActive(true);
                var color = ForceConfig.GetConfig(forceId).Color;
                //把color加入富文本中
                textAiInfo.text = $"<color={color}>{playerName}</color> 进行中";
            }
        }
    }    
}
