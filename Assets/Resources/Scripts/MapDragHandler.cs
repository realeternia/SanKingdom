using UnityEngine;
using UnityEngine.EventSystems;

public class MapDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [Header("拖动设置")]
    [SerializeField] private float dragThreshold = 5f;
    [SerializeField] private bool clampToViewport = true;
    
    private RectTransform bgPanelRect;
    private RectTransform viewportRect;
    private Vector2 panelStartPos;
    private bool isDragging = false;
    private bool dragExceededThreshold = false;
    
    public bool IsDragging => isDragging && dragExceededThreshold;
    
    public void Initialize(RectTransform bgPanel, RectTransform viewport)
    {
        bgPanelRect = bgPanel;
        viewportRect = viewport;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        panelStartPos = bgPanelRect.anchoredPosition;
        isDragging = true;
        dragExceededThreshold = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || bgPanelRect == null)
            return;
        
        if (!dragExceededThreshold && eventData.delta.magnitude > dragThreshold)
        {
            dragExceededThreshold = true;
        }
        
        Vector2 newPos = panelStartPos + eventData.delta;
        
        if (clampToViewport && viewportRect != null)
        {
            newPos = ClampPositionToViewport(newPos);
        }
        
        bgPanelRect.anchoredPosition = newPos;
        panelStartPos = bgPanelRect.anchoredPosition;
    }
    
    private Vector2 ClampPositionToViewport(Vector2 position)
    {
        if (viewportRect == null || bgPanelRect == null)
            return position;
        
        Vector2 bgSize = bgPanelRect.rect.size;
        Vector2 viewSize = viewportRect.rect.size;
        
        float halfBgWidth = bgSize.x / 2f;
        float halfBgHeight = bgSize.y / 2f;
        float halfViewWidth = viewSize.x / 2f;
        float halfViewHeight = viewSize.y / 2f;
        
        float minX, maxX, minY, maxY;
        
        if (bgSize.x > viewSize.x)
        {
            minX = halfBgWidth - halfViewWidth;
            maxX = -(halfBgWidth - halfViewWidth);
        }
        else
        {
            minX = maxX = 0f;
        }
        
        if (bgSize.y > viewSize.y)
        {
            minY = halfBgHeight - halfViewHeight;
            maxY = -(halfBgHeight - halfViewHeight);
        }
        else
        {
            minY = maxY = 0f;
        }
        
        return new Vector2(
            Mathf.Clamp(position.x, maxX, minX),
            Mathf.Clamp(position.y, maxY, minY)
        );
    }
    
    public void ResetPosition()
    {
        if (bgPanelRect != null)
        {
            bgPanelRect.anchoredPosition = Vector2.zero;
        }
    }
}
