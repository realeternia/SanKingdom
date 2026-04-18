using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using Controls.Utils;

public class CityCellHero : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CityPanelManager cityPanelManager;

    public int heroId;
    public TMP_Text heroName;
    public TMP_Text stateText;
    public Image heroIcon;
    public Image thumbIcon;
    public bool isSelect = false;
    public Image backgroundImage;
    public Color normalColor = Color.black;
    public Color selectedColor = Color.green;

    private Canvas dragCanvas;
    private GameObject dragGhost;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private bool isRealDragging = false;
    private ScrollRect parentScrollRect;
    private bool isLeftSide = false;
    private CanvasGroup ownCanvasGroup;

    void Start()
    {
        thumbIcon.gameObject.SetActive(false);
        heroName.raycastTarget = false;
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        UpdateBackgroundColor();
        parentScrollRect = GetComponentInParent<ScrollRect>();
    }

    public void Init(int heroId)
    {
        this.heroId = heroId;
        var heroCfg = HeroConfig.GetConfig(heroId);
        heroName.text = heroCfg.Name;
        heroIcon.sprite = Resources.Load<Sprite>("Textures/Skins/" + heroCfg.Icon);

        UpdateWorkState();
    }

    public void UpdateWorkState()
    {
        if (stateText == null) return;

        if (cityPanelManager != null && cityPanelManager.GetDevNodeByHero(heroId) != null)
        {
            stateText.text = "工作中";
        }
        else
        {
            stateText.text = "";
        }
    }

    public void SetSelected(bool selected)
    {
        isSelect = selected;
        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? selectedColor : normalColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDragging)
        {
            cityPanelManager.OnSelectHero(this);
        }

        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint);
        isLeftSide = localPoint.x < 0;
        GameLog.Info($"OnPointerDown: localPoint={localPoint}, isLeftSide={isLeftSide}");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameLog.Info($"OnBeginDrag: isLeftSide={isLeftSide}");

        if (!isLeftSide)
        {
            if (parentScrollRect != null)
            {
                parentScrollRect.OnBeginDrag(eventData);
            }
            return;
        }

        isDragging = true;
        isRealDragging = true;
        originalPosition = transform.position;

        if (dragCanvas == null)
        {
            dragCanvas = GetComponentInParent<Canvas>();
        }

        GameLog.Info($"OnBeginDrag: dragCanvas={dragCanvas}, heroIcon={heroIcon}, sprite={heroIcon?.sprite}");

        dragGhost = new GameObject("DragGhost");
        RectTransform ghostRect = dragGhost.AddComponent<RectTransform>();
        dragGhost.transform.SetParent(dragCanvas.transform, false);

        ghostRect.sizeDelta = new Vector2(80, 80);
        ghostRect.anchorMin = new Vector2(0, 0);
        ghostRect.anchorMax = new Vector2(0, 0);
        ghostRect.pivot = new Vector2(0.5f, 0.5f);

        Image ghostImage = dragGhost.AddComponent<Image>();
        if (heroIcon != null && heroIcon.sprite != null)
        {
            ghostImage.sprite = heroIcon.sprite;
        }
        ghostImage.raycastTarget = false;
        ghostImage.preserveAspect = true;

        CanvasGroup canvasGroup = dragGhost.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.8f;

        dragGhost.transform.SetAsLastSibling();

        ownCanvasGroup = GetComponent<CanvasGroup>();
        if (ownCanvasGroup == null)
        {
            ownCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        ownCanvasGroup.blocksRaycasts = false;

        UpdateGhostPosition(eventData);
        GameLog.Info($"OnBeginDrag: ghost created, position={dragGhost.transform.position}");

        cityPanelManager.OnHeroDragStart(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isRealDragging)
        {
            if (parentScrollRect != null)
            {
                parentScrollRect.OnDrag(eventData);
            }
            return;
        }

        UpdateGhostPosition(eventData);
    }

    private void UpdateGhostPosition(PointerEventData eventData)
    {
        if (dragGhost == null || dragCanvas == null) return;

        Camera cam = dragCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : dragCanvas.worldCamera;
        Vector3 worldPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(dragCanvas.GetComponent<RectTransform>(), eventData.position, cam, out worldPoint);
        dragGhost.GetComponent<RectTransform>().position = worldPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameLog.Info($"OnEndDrag: isRealDragging={isRealDragging}");

        if (!isRealDragging)
        {
            if (parentScrollRect != null)
            {
                parentScrollRect.OnEndDrag(eventData);
            }
            return;
        }

        isDragging = false;
        isRealDragging = false;

        if (dragGhost != null)
        {
            Destroy(dragGhost);
            dragGhost = null;
        }

        if (ownCanvasGroup != null)
        {
            ownCanvasGroup.blocksRaycasts = true;
        }

        cityPanelManager.OnHeroDragEnd(this, eventData);
    }

    public int GetHeroId()
    {
        return heroId;
    }

    public void UpdateThumbIcon(string[] attrs)
    {
        if (thumbIcon == null || thumbIcon.gameObject == null)
            return;

        if (attrs == null || attrs.Length == 0)
        {
            thumbIcon.gameObject.SetActive(false);
            return;
        }

        var heroData = GameManager.Instance.GetHero(heroId);
        if (heroData == null)
        {
            thumbIcon.gameObject.SetActive(false);
            return;
        }

        float weightedValue = 0f;
        
        if (attrs.Length == 1)
        {
            weightedValue = heroData.GetAttr(attrs[0]);
        }
        else
        {
            float firstAttr = heroData.GetAttr(attrs[0]);
            float secondAttr = heroData.GetAttr(attrs[1]);
            weightedValue = firstAttr * (2f / 3f) + secondAttr * (1f / 3f);
        }

        if (weightedValue >= 90)
        {
            thumbIcon.color = Color.red;
            thumbIcon.gameObject.SetActive(true);
        }
        else if (weightedValue >= 80)
        {
            thumbIcon.color = Color.yellow;
            thumbIcon.gameObject.SetActive(true);
        }
        else if (weightedValue >= 70)
        {
            thumbIcon.color = Color.green;
            thumbIcon.gameObject.SetActive(true);
        }
        else
        {
            thumbIcon.gameObject.SetActive(false);
        }
    }

    public float GetWeightedAttrValue(string[] attrs)
    {
        if (attrs == null || attrs.Length == 0)
            return 0f;

        var heroData = GameManager.Instance.GetHero(heroId);
        if (heroData == null)
            return 0f;

        if (attrs.Length == 1)
        {
            return heroData.GetAttr(attrs[0]);
        }
        else
        {
            float firstAttr = heroData.GetAttr(attrs[0]);
            float secondAttr = heroData.GetAttr(attrs[1]);
            return firstAttr * (2f / 3f) + secondAttr * (1f / 3f);
        }
    }
}
