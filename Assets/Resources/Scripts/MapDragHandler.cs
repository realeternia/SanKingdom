using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Controls.Utils;

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
    private Coroutine moveCoroutine;
    
    public bool IsDragging => isDragging && dragExceededThreshold;
    
    public void Initialize(RectTransform bgPanel, RectTransform viewport)
    {
        bgPanelRect = bgPanel;
        viewportRect = viewport;
        
        Image image = bgPanel.GetComponent<Image>();
        if (image == null)
        {
            image = bgPanel.gameObject.AddComponent<Image>();
            image.color = new Color(1, 1, 1, 0.01f);
        }
        image.raycastTarget = true;
        
        GameLog.Info($"MapDragHandler Initialize: 添加/设置 Image 组件, raycastTarget = {image.raycastTarget}");
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameLog.Info($"OnBeginDrag 触发, position = {eventData.position}");

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        panelStartPos = bgPanelRect.anchoredPosition;
        isDragging = true;
        dragExceededThreshold = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || bgPanelRect == null)
            return;
        
        GameLog.Info($"OnDrag: delta = {eventData.delta}, panelStartPos = {panelStartPos}");
        
        if (!dragExceededThreshold && eventData.delta.magnitude > dragThreshold)
        {
            dragExceededThreshold = true;
        }
        
        Vector2 newPos = panelStartPos + eventData.delta;
        
        if (clampToViewport && viewportRect != null)
        {
            newPos = ClampPositionToViewport(newPos);
        }
        
        GameLog.Info($"OnDrag: newPos = {newPos}");
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

    public void MoveToPositionSmooth(Vector2 targetPos, float duration = 0.5f)
    {
        if (bgPanelRect == null)
            return;

        Vector2 clampedTarget = ClampPositionToViewport(targetPos);
        clampedTarget.y = -clampedTarget.y;
        clampedTarget.x = -clampedTarget.x; 
        GameLog.Info($"MoveToPositionSmooth: targetPos = ({targetPos.x}, {targetPos.y}), clamped = ({clampedTarget.x}, {clampedTarget.y}), anchoredPos = ({bgPanelRect.anchoredPosition.x}, {bgPanelRect.anchoredPosition.y}), duration = {duration}");

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(SmoothMoveCoroutine(bgPanelRect.anchoredPosition, clampedTarget, duration));
    }

    private IEnumerator SmoothMoveCoroutine(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            t = 1f - (1f - t) * (1f - t);

            bgPanelRect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        bgPanelRect.anchoredPosition = endPos;
        moveCoroutine = null;
    }
    
}
