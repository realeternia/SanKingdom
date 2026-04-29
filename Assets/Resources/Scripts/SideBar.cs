using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SideBar : MonoBehaviour
{
    public GameObject scrollItem;
    public Image scrollImg;

    private RectTransform scrollRect;
    private float fullWidth;
    private Tween currentTween;

    private const float AnimDuration = 0.4f;
    private const float GradientWidth = 100f;

    void Awake()
    {
        scrollRect = scrollItem.GetComponent<RectTransform>();
        fullWidth = scrollRect.sizeDelta.x;

        scrollRect.pivot = new Vector2(1f, scrollRect.pivot.y);
        scrollRect.anchoredPosition = new Vector2(
            scrollRect.anchoredPosition.x + fullWidth,
            scrollRect.anchoredPosition.y);

        var gradient = scrollImg.gameObject.AddComponent<SideBarAlphaGradient>();
        gradient.gradientWidth = GradientWidth;

        var bgButton = GetComponent<Button>();
        if (bgButton != null)
        {
            bgButton.onClick.AddListener(OnBackgroundClick);
        }

        scrollItem.SetActive(false);
        scrollRect.sizeDelta = new Vector2(0, scrollRect.sizeDelta.y);
    }

    private void OnBackgroundClick()
    {
        PanelManager.Instance.HideSideBar();
    }

    public void OnShow()
    {
        scrollItem.SetActive(true);

        if (currentTween != null)
            currentTween.Kill();

        scrollRect.sizeDelta = new Vector2(0, scrollRect.sizeDelta.y);

        currentTween = scrollRect.DOSizeDelta(
                new Vector2(fullWidth, scrollRect.sizeDelta.y), AnimDuration)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
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
                onComplete?.Invoke();
            });
    }
}
