using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;
using System;
using System.Linq;

public class MainPanelManager : MonoBehaviour, IPanelEvent
{
    private const float MAP_SCALE_FACTOR = 1.25f;
    
    public Button btnSystem;
    public Button btnRoundNext;
    public Button btnGm;

    public TMP_Text textRoundNext;

    public TMP_Text textYear;
    public TMP_Text textAiInfo;
    public GameObject bgPanel;
    public GameObject roadLayer;
    public GameObject textLayer;
    public GameObject areaLayer;
    public VideoPanelManager videoPanelManager;
    private MapDragHandler mapDragHandler;
    private List<WorldPieceControl> worldPieces = new List<WorldPieceControl>();
    private Dictionary<int, Vector2> cityCenterPositions = new Dictionary<int, Vector2>();
    private static Sprite whiteSprite;

    // Start is called before the first frame update
    void Start()
    {
        InitLayers();
        LoadMapPieces();
        LoadRoads();
        InitDragHandler();

        var nowRound = GameManager.Instance.SaveData.round;
        
        var seasonId = GameManager.Instance.SeasonId;
        var seasonCfg = SeasonConfig.GetConfig(seasonId);
        // 使用GameManager的常量计算年份
        int years = nowRound / SystemConst.Game.SEASONS_PER_YEAR;
        textYear.text = $"{SystemConst.Game.BASE_YEAR + years}年{seasonCfg.Name}";

        btnSystem.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowSystemPanel();
        });        
        btnRoundNext.onClick.AddListener(() =>
        {
            OnRoundNextClick();
        });
        btnGm.onClick.AddListener(() =>
        {
            PanelManager.Instance.ShowGmPanel();
        });
        btnGm.gameObject.SetActive(SysSwitch.IsDebugMode);
        
        StartCoroutine(MoveToPlayerCapitalDelayed());
    }

    void Update()
    {

    }

    private void InitLayers()
    {
        SetupLayer(areaLayer);
        SetupLayer(textLayer);
        SetupLayer(roadLayer);
    }

    private void SetupLayer(GameObject layer)
    {
        layer.transform.SetParent(bgPanel.transform, false);
        RectTransform layerRect = layer.GetComponent<RectTransform>();
        if (layerRect == null) return;

        layerRect.anchorMin = Vector2.zero;
        layerRect.anchorMax = Vector2.one;
        layerRect.offsetMin = Vector2.zero;
        layerRect.offsetMax = Vector2.zero;
    }
    
    private void OnRoundNextClick()
    {
        var currentForce = GameManager.Instance.CurrentForce;
        var phase = currentForce != null ? currentForce.phase : TurnPhase.None;
        
        if (phase == TurnPhase.None)
        {
            GameManager.Instance.NextRound();
        }
        else if (phase == TurnPhase.Planning)
        {
            if (currentForce != null && currentForce.isPlayer)
            {
                if (currentForce.GetPredictedGoldBalance() < 0)
                {
                    SystemTip.Instance.ShowTip("黄金不足，无法确认计划");
                    return;
                }
                GameManager.Instance.ConfirmPlan(currentForce.forceId);
            }
        }
    }
    
    private void UpdateRoundNextButton()
    {
        var currentForce = GameManager.Instance.CurrentForce;
        var phase = currentForce != null ? currentForce.phase : TurnPhase.None;
        bool isPlayerTurn = currentForce != null && currentForce.isPlayer;
        
        if (textRoundNext != null)
        {
            if (phase == TurnPhase.None)
            {
                textRoundNext.text = "下一回合";
            }
            else if (isPlayerTurn)
            {
                switch (phase)
                {
                    case TurnPhase.Planning:
                        textRoundNext.text = "确认计划";
                        break;
                    case TurnPhase.Execution:
                        textRoundNext.text = "执行中...";
                        break;
                    case TurnPhase.Battle:
                        textRoundNext.text = "战斗中...";
                        break;
                }
            }
            else
            {
                if (currentForce != null)
                {
                    textRoundNext.text = $"{currentForce.Name} 行动中...";
                }
                else
                {
                    textRoundNext.text = "等待中...";
                }
            }
        }
        
        btnRoundNext.interactable = (phase == TurnPhase.None) || (phase == TurnPhase.Planning && isPlayerTurn);
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
        
        var kingCity = playerForce.GetKingCity();
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
        
        string texturePath = ResPath.Texture.MapTexture(cityConfig.Name);
        Texture2D texture = ResourceCache.LoadUI<Texture2D>(texturePath);
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


    private void LoadMapPieces()
    {
        GameLog.Info($"LoadMapPieces 地图数量: {WorldConfig.ConfigList.Count}");

        // 遍历所有地图配置
        foreach (var worldConfig in WorldConfig.ConfigList)
        {
            // try
            // {
                // 构建图片路径（Resources/Textures/Maps/下的图片）
                string texturePath = ResPath.Texture.MapTexture(worldConfig.Name);
                
                // 加载图片资源
                Texture2D texture = ResourceCache.LoadUI<Texture2D>(texturePath);

                // 创建精灵
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                // 从Prefabs/WorldPiece加载预设体
                GameObject worldPiecePrefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.WorldPiece());
                
                // 实例化预设体
                GameObject mapPiece = Instantiate(worldPiecePrefab, areaLayer.transform, false);
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

                    cityCenterPositions[worldConfig.Id] = rectTransform.anchoredPosition;
                }

                pieceControl.InitForce();

                Transform textNameTransform = pieceControl.pieceName.transform;
                textNameTransform.SetParent(textLayer.transform, true);

                worldPieces.Add(pieceControl);
        }
    }

    private void LoadRoads()
    {
        HashSet<string> createdRoads = new HashSet<string>();

        foreach (var worldConfig in WorldConfig.ConfigList)
        {
            int cityId = worldConfig.Id;
            var adjacentIds = MapTool.GetAdjacentCityIds(cityId);

            foreach (int adjId in adjacentIds)
            {
                int minId = Mathf.Min(cityId, adjId);
                int maxId = Mathf.Max(cityId, adjId);
                string roadKey = minId + "_" + maxId;

                if (createdRoads.Contains(roadKey))
                    continue;
                createdRoads.Add(roadKey);

                CreateRoad(minId, maxId);
            }
        }

        GameLog.Info($"LoadRoads 道路数量: {createdRoads.Count}");
    }

    private void CreateRoad(int cityId1, int cityId2)
    {
        Vector2 pos1 = GetCityCenterPosition(cityId1);
        Vector2 pos2 = GetCityCenterPosition(cityId2);

        Vector2 midPoint = (pos1 + pos2) / 2f;
        float distance = Vector2.Distance(pos1, pos2);
        float angle = Mathf.Atan2(pos2.y - pos1.y, pos2.x - pos1.x) * Mathf.Rad2Deg;

        Color roadColor = GetRoadColor(cityId1, cityId2);

        GameObject roadObj = new GameObject("Road_" + cityId1 + "_" + cityId2);
        RectTransform rectTransform = roadObj.AddComponent<RectTransform>();
        rectTransform.SetParent(roadLayer.transform, false);

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        Image image = roadObj.AddComponent<Image>();
        image.sprite = GetWhiteSprite();
        image.color = roadColor;
        image.raycastTarget = false;

        rectTransform.anchoredPosition = midPoint + new Vector2(-1280, 1280);
        rectTransform.sizeDelta = new Vector2(distance, SystemConst.WorldMap.ROAD_WIDTH);
        rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private Color GetRoadColor(int cityId1, int cityId2)
    {
        var city1 = GameManager.Instance.GetCity(cityId1);
        var city2 = GameManager.Instance.GetCity(cityId2);

        if (city1.forceId == city2.forceId)
            return SysColor.WorldMap.RoadInternalColor;

        var relationLevel = GameManager.Instance.SaveData.forceRelation.GetRelationLevel(city1.forceId, city2.forceId);

        switch (relationLevel)
        {
            case RelationLevel.Friendly:
                return SysColor.WorldMap.RoadFriendlyColor;
            case RelationLevel.Hostile:
                return SysColor.WorldMap.RoadHostileColor;
            default:
                return SysColor.WorldMap.RoadNeutralColor;
        }
    }

    private Vector2 GetCityCenterPosition(int cityId)
    {
        if (!cityCenterPositions.TryGetValue(cityId, out Vector2 pos))
        {
            GameLog.Error("GetCityCenterPosition: 未找到城市" + cityId + "的中心位置");
            return Vector2.zero;
        }

        var cfg = WorldConfig.GetConfig(cityId);
        if (cfg.MiniMapOffsets != null && cfg.MiniMapOffsets.Length >= 2)
        {
            pos.x += cfg.MiniMapOffsets[0] * MAP_SCALE_FACTOR;
            pos.y += cfg.MiniMapOffsets[1] * MAP_SCALE_FACTOR;
        }

        return pos;
    }

    private static Sprite GetWhiteSprite()
    {
        if (whiteSprite == null)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            whiteSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        }
        return whiteSprite;
    }

    private void RefreshRoadsByForce(int forceId1, int forceId2)
    {
        foreach (Transform child in roadLayer.transform)
        {
            string name = child.name;
            if (!name.StartsWith("Road_")) continue;

            string[] parts = name.Split('_');
            if (parts.Length < 3) continue;

            if (!int.TryParse(parts[1], out int cityId1) || !int.TryParse(parts[2], out int cityId2))
                continue;

            var city1 = GameManager.Instance.GetCity(cityId1);
            var city2 = GameManager.Instance.GetCity(cityId2);
            if (city1 == null || city2 == null) continue;

            bool match = (city1.forceId == forceId1 && city2.forceId == forceId2)
                      || (city1.forceId == forceId2 && city2.forceId == forceId1);
            if (!match) continue;

            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                image.color = GetRoadColor(cityId1, cityId2);
            }
        }
    }

    public void OnPieceClick(int pieceId)
    {
        var currentForce = GameManager.Instance.CurrentForce;
        if (currentForce != null && !currentForce.isPlayer)
        {
            return;
        }

        var cityData = GameManager.Instance.GetCity(pieceId);
        bool isPlayerCity = GameManager.Instance.GetForce(cityData.forceId).isPlayer;
        
        if (!isPlayerCity && !SysSwitch.CanViewOtherForceCity)
        {
            return;
        }
        
        if (SysSwitch.CanViewOtherForceCity)
        {
            PanelManager.Instance.RefreshForceResItems(cityData.forceId);
        }
    }

    public void SendSignal(SignalData data)
    {
        if(data.Name == "PhaseChange")
        {
            UpdateRoundNextButton();
        }
        else if(data.Name == "CityForceChange")
        {
            var signal = data as CityForceChangeSignal;
            var piece = worldPieces.Find(x => x.pieceId == signal.CityId);
            if (piece != null)
                piece.RefreshCityDisplay();
        }
        else if(data.Name == "CityHeroChange")
        {
            var signal = data as CityHeroChangeSignal;
            var piece = worldPieces.Find(x => x.pieceId == signal.CityId);
            if (piece != null)
                piece.RefreshCityDisplay();
        }
        else if(data.Name == "CityLevelChange")
        {
            var signal = data as CityLevelChangeSignal;
            var piece = worldPieces.Find(x => x.pieceId == signal.CityId);
            if (piece != null)
                piece.RefreshCityDisplay();
        }
        else if(data.Name == "RoundChange")
        {
            var signal = data as RoundChangeSignal;
            var seasonId = GameManager.Instance.SeasonId;
            var seasonCfg = SeasonConfig.GetConfig(seasonId);
            int years = signal.Round / SystemConst.Game.SEASONS_PER_YEAR;
            textYear.text = $"{SystemConst.Game.BASE_YEAR + years}年{seasonCfg.Name}";

            for (int i = 0; i < worldPieces.Count; i++)
            {
                worldPieces[i].UpdateDisplay();
            }

            if(seasonCfg.Video != "")
                videoPanelManager.Play(seasonCfg.Video);

            MoveToPlayerCapital();
        }
        else if(data.Name == "AICheck")
        {
            var signal = data as AICheckSignal;
            if (string.IsNullOrEmpty(signal.AIName))
            {
                textAiInfo.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                textAiInfo.transform.parent.gameObject.SetActive(true);
                var color = ForceConfig.GetConfig(signal.ForceId).Color;
                textAiInfo.text = $"<color={color}>{signal.AIName}</color> 进行中";
            }
        }
        else if(data.Name == "RelationChange")
        {
            var signal = data as RelationChangeSignal;
            RefreshRoadsByForce(signal.ForceId1, signal.ForceId2);
        }
    }    
}
