using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class SideBar : MonoBehaviour, IPointerClickHandler
{
    public GameObject scrollItem;
    public Image scrollImg;

    private RectTransform scrollRect;
    private float fullWidth;
    private Tween currentTween;
    private Camera uiCamera;

    private const float AnimDuration = 0.4f;
    private const float GradientWidth = 100f;

    public GameObject subRegionNode;

    void Awake()
    {
        scrollRect = scrollItem.GetComponent<RectTransform>();
        fullWidth = scrollRect.sizeDelta.x;

        Canvas canvas = GetComponentInParent<Canvas>();
        uiCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            ? canvas.worldCamera
            : null;

        scrollRect.pivot = new Vector2(1f, scrollRect.pivot.y);
        scrollRect.anchoredPosition = new Vector2(
            scrollRect.anchoredPosition.x + fullWidth,
            scrollRect.anchoredPosition.y);

        var gradient = scrollImg.gameObject.AddComponent<SideBarAlphaGradient>();
        gradient.gradientWidth = GradientWidth;

        scrollItem.SetActive(false);
        scrollRect.sizeDelta = new Vector2(0, scrollRect.sizeDelta.y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(scrollRect, eventData.position, uiCamera))
        {
            return;
        }
        PanelManager.Instance.HideSideBar();
    }

    public void OnShow(string panelName, System.Action<GameObject> onCreated = null)
    {
        scrollItem.SetActive(true);

        if (currentTween != null)
            currentTween.Kill();

        scrollRect.sizeDelta = new Vector2(0, scrollRect.sizeDelta.y);

        currentTween = scrollRect.DOSizeDelta(
                new Vector2(fullWidth, scrollRect.sizeDelta.y), AnimDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        LoadSubPanel(panelName, onCreated);
    }

    private void LoadSubPanel(string panelName, System.Action<GameObject> onCreated = null)
    {
        foreach (Transform child in subRegionNode.transform)
        {
            Destroy(child.gameObject);
        }

        var prefab = ResourceCache.LoadPrefabUI(ResPath.Prefab.Panel(panelName));
        var panelObj = Instantiate(prefab, subRegionNode.transform);
        var rectTransform = panelObj.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        onCreated?.Invoke(panelObj);
    }

    public void OnHide(Action onComplete = null)
    {
        if (currentTween != null)
            currentTween.Kill();

        currentTween = scrollRect.DOSizeDelta(
                new Vector2(0, scrollRect.sizeDelta.y), AnimDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                scrollItem.SetActive(false);
                ClearSubPanel();
                onComplete?.Invoke();
            });
    }

    private void ClearSubPanel()
    {
        foreach (Transform child in subRegionNode.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
