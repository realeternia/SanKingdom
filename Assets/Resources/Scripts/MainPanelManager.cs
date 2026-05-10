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
    
    public CityDetail cityDetail;
    public Button btnSystem;
    public Button btnRoundNext;
    public Button btnMode;
    public TMP_Text textRoundNext;

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
        btnMode.onClick.AddListener(() =>
        {
            SetMode();
            PanelManager.Instance.ShowSideBar("SideArmsSelector");
        });
        
        StartCoroutine(MoveToPlayerCapitalDelayed());
    }

    void Update()
    {
        
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
                Texture2D texture = Resources.Load<Texture2D>(texturePath);

                // 创建精灵
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                
                // 从Prefabs/WorldPiece加载预设体
                GameObject worldPiecePrefab = Resources.Load<GameObject>(ResPath.Prefab.WorldPiece());
                
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
        var currentForce = GameManager.Instance.CurrentForce;
        if (currentForce != null && !currentForce.isPlayer)
        {
            return;
        }

        cityDetail.gameObject.SetActive(true);
        cityDetail.SetCityDetail(pieceId);

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

    private int extraMode = 0;
    public void SetMode()
    {
        extraMode = (extraMode + 1) % 4;
        foreach (var piece in worldPieces)
        {
            piece.SetExtraMode(extraMode);
        }
    }

    public void SendSignal(SignalData data)
    {
        GameLog.Debug($"WorldManager SendSignal {data.Name}");
        cityDetail.SendSignal(data);

        if(data.Name == "PhaseChange")
        {
            UpdateRoundNextButton();
        }
        else if(data.Name == "CityForceChange")
        {
            var signal = data as CityForceChangeSignal;
            worldPieces.Find(x => x.pieceId == signal.CityId).SetColor(GameManager.Instance.GetCity(signal.CityId).forceId);
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
    }    
}
