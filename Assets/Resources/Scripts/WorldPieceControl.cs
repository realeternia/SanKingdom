using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonConfig;
using TMPro;

public class WorldPieceControl : MonoBehaviour
{
    private const float MAP_SCALE_FACTOR = SystemConst.WorldMap.MAP_SCALE_FACTOR;

    private static WorldPieceControl currentActivePiece;

    public int pieceId;
    public Image pieceImage;
    public MainPanelManager worldManager;
    public TMP_Text pieceName;
    public Button enterButton;
    public Image[] heroImage;
    public Image[] resImage;

    private Color defaultColor;
    private Vector2[] heroOrigPositions;
    private Vector2[] resOrigPositions;
    private bool positionsRecorded;

    void Start()
    {
        pieceName.raycastTarget = false;

        foreach (var img in heroImage)
            img.raycastTarget = false;
        foreach (var img in resImage)
            img.raycastTarget = false;

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
    }

    private void OnPieceClicked()
    {
        worldManager.OnPieceClick(pieceId);

        if (currentActivePiece != null && currentActivePiece != this)
        {
            currentActivePiece.enterButton.gameObject.SetActive(false);
            currentActivePiece.ShowResImages(true);
        }

        var cityData = GameManager.Instance.GetCity(pieceId);
        var forceData = GameManager.Instance.GetForce(cityData.forceId);
        bool isPlayerCity = forceData.isPlayer;

        if (isPlayerCity)
        {
            enterButton.image.color = Color.green;
            enterButton.GetComponentInChildren<TMP_Text>().text = "进入";
            enterButton.gameObject.SetActive(true);
            ShowResImages(false);
            currentActivePiece = this;
        }
        else if (SysSwitch.CanViewOtherForceCity)
        {
            enterButton.image.color = Color.yellow;
            enterButton.GetComponentInChildren<TMP_Text>().text = "查看";
            enterButton.gameObject.SetActive(true);
            ShowResImages(false);
            currentActivePiece = this;
        }
        else
        {
            enterButton.gameObject.SetActive(false);
            ShowResImages(true);
            if (currentActivePiece == this)
            {
                currentActivePiece = null;
            }
        }
    }

    private void ShowResImages(bool show)
    {
        foreach (var img in resImage)
        {
            if (img == null) continue;
            if (show)
                img.gameObject.SetActive(img.sprite != null);
            else
                img.gameObject.SetActive(false);
        }
    }

    private void OnEnterButtonClick()
    {
        PanelManager.Instance.ShowCity(pieceId);
    }

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

    public void InitForce()
    {
        RecordPositions();

        var pieceCfg = WorldConfig.GetConfig(pieceId);
        var city = GameManager.Instance.GetCity(pieceId);
        SetColor(city.forceId);

        string levelHex = ColorUtility.ToHtmlStringRGB(SysColor.City.LevelColor);
        pieceName.text = $"<color=#{levelHex}>({city.GetLevel()})</color>{pieceCfg.Cname}";

        if (pieceCfg.MiniMapOffsets != null && pieceCfg.MiniMapOffsets.Length >= 2)
        {
            float offsetX = pieceCfg.MiniMapOffsets[0] * MAP_SCALE_FACTOR;
            float offsetY = pieceCfg.MiniMapOffsets[1] * MAP_SCALE_FACTOR;
            pieceName.rectTransform.anchoredPosition += new Vector2(offsetX, offsetY);
        }

        UpdateResImages(pieceCfg);
        UpdateHeroImages(city);
    }

    public void UpdateDisplay()
    {
        var city = GameManager.Instance.GetCity(pieceId);

        string levelHex = ColorUtility.ToHtmlStringRGB(SysColor.City.LevelColor);
        pieceName.text = $"<color=#{levelHex}>({city.GetLevel()})</color>{WorldConfig.GetConfig(pieceId).Cname}";

        UpdateHeroImages(city);
    }

    public void RefreshCityDisplay()
    {
        var city = GameManager.Instance.GetCity(pieceId);

        SetColor(city.forceId);

        string levelHex = ColorUtility.ToHtmlStringRGB(SysColor.City.LevelColor);
        pieceName.text = $"<color=#{levelHex}>({city.GetLevel()})</color>{WorldConfig.GetConfig(pieceId).Cname}";

        UpdateHeroImages(city);
    }

    private void RecordPositions()
    {
        if (positionsRecorded) return;

        heroOrigPositions = new Vector2[heroImage.Length];
        for (int i = 0; i < heroImage.Length; i++)
            heroOrigPositions[i] = heroImage[i].rectTransform.anchoredPosition;

        resOrigPositions = new Vector2[resImage.Length];
        for (int i = 0; i < resImage.Length; i++)
            resOrigPositions[i] = resImage[i].rectTransform.anchoredPosition;

        positionsRecorded = true;
    }

    private void LayoutVisibleImages(Image[] images, Vector2[] origPositions)
    {
        int visibleCount = 0;
        foreach (var img in images)
            if (img != null && img.gameObject.activeSelf)
                visibleCount++;

        if (visibleCount == 0 || origPositions.Length == 0) return;

        Vector2 center = (origPositions[0] + origPositions[origPositions.Length - 1]) / 2f;
        float spacing = origPositions.Length > 1
            ? (origPositions[origPositions.Length - 1].x - origPositions[0].x) / (origPositions.Length - 1)
            : 0f;

        float startX = center.x - (visibleCount - 1) * spacing / 2f;

        int visIdx = 0;
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] != null && images[i].gameObject.activeSelf)
            {
                var pos = origPositions[i];
                pos.x = startX + visIdx * spacing;
                images[i].rectTransform.anchoredPosition = pos;
                visIdx++;
            }
            else if (images[i] != null)
            {
                images[i].rectTransform.anchoredPosition = origPositions[i];
            }
        }
    }

    private void UpdateResImages(WorldConfig pieceCfg)
    {
        var displayIcons = new List<string>();

        if (pieceCfg.SpecialBuildings != null)
        {
            foreach (int buildingId in pieceCfg.SpecialBuildings)
            {
                var devCfg = CityDevConfig.GetConfig(buildingId);
                if(devCfg.DevAttr1 != null)
                {
                    var attrCfg = CityAttrConfig.GetConfigByname(devCfg.DevAttr1);
                    displayIcons.Add(attrCfg.Icon);
                }
            }
        }

        if (pieceCfg.ResAddon != null && displayIcons.Count < resImage.Length)
        {
            foreach (int addonId in pieceCfg.ResAddon)
            {
                if (displayIcons.Count >= resImage.Length) break;
                var attrCfg = CityAttrConfig.GetConfig(addonId);
                displayIcons.Add(attrCfg.Icon);
            }
        }
       

        for (int i = 0; i < resImage.Length; i++)
        {
            if (i < displayIcons.Count)
            {
                resImage[i].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.AttrIcon(displayIcons[i]));
                resImage[i].gameObject.SetActive(true);
            }
            else
            {
                resImage[i].sprite = null;
                resImage[i].gameObject.SetActive(false);
            }
        }
        LayoutVisibleImages(resImage, resOrigPositions);
    }

    private void UpdateHeroImages(SaveCityData city)
    {
        var heroIds = city.GetNormalHeroList();
        var displayHeroes = new List<int>();

        foreach (var hid in heroIds)
        {
            var heroCfg = HeroConfig.GetConfig(hid);
            if (hid == city.ownerHeroId || heroCfg.StarHero)
            {
                displayHeroes.Add(hid);
            }
        }

        displayHeroes.Sort((a, b) => HeroConfig.GetConfig(b).Total.CompareTo(HeroConfig.GetConfig(a).Total));

        int count = Mathf.Min(displayHeroes.Count, heroImage.Length, 3);
        for (int i = 0; i < heroImage.Length; i++)
        {
            if (i < count)
            {
                var heroCfg = HeroConfig.GetConfig(displayHeroes[i]);
                heroImage[i].sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.HeroIcon(heroCfg.Icon));
                heroImage[i].gameObject.SetActive(true);
            }
            else
            {
                heroImage[i].sprite = null;
                heroImage[i].gameObject.SetActive(false);
            }
        }
        LayoutVisibleImages(heroImage, heroOrigPositions);
    }

    public void SetColor(int forceId)
    {
        if (pieceImage == null)
        {
            GameLog.Error("pieceImage is null");
            return;
        }

        var forceConfig = ForceConfig.GetConfig(forceId);
        if (forceConfig == null)
        {
            GameLog.Error($"找不到forceId为{forceId}的配置");
            return;
        }

        defaultColor = SysColor.GetForceColor(forceId);
        pieceImage.color = defaultColor;
        pieceName.color = SysColor.GetTextColorOnBackground(defaultColor);
    }
}
