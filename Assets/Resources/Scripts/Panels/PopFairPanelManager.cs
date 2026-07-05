using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CommonConfig;

public class PopFairPanelManager : MonoBehaviour
{
    public int fairId;

    public TMP_Text titleText;
    public TMP_Text fairDesText;
    public Image fairImg;

    public Button okBtn;

    public static bool IsShowing { get; private set; }

    private Action onClose;

    void Start()
    {
        okBtn.onClick.AddListener(() =>
        {
            PanelManager.Instance.HidePopFairPanel();
            onClose?.Invoke();
        });
    }

    public void Show(int fairId, int forceId)
    {
        this.fairId = fairId;
        IsShowing = true;

        var fairCfg = FairConfig.GetConfig(fairId);
        titleText.text = fairCfg.Title;

        if (!string.IsNullOrEmpty(fairCfg.Image))
        {
            var sprite = ResourceCache.LoadSpriteUI(ResPath.Texture.TextureByName("Fairs/" + fairCfg.Image));
            if (sprite != null) fairImg.sprite = sprite;
        }

        if (!string.IsNullOrEmpty(fairCfg.Bg))
        {
            BGMPlayer.Instance.PlaySound("Sounds/" + fairCfg.Bg);
        }

        var forceCfg = ForceConfig.GetConfig(forceId);
        Color forceColor = SysColor.GetForceColor(forceId);
        string colorHex = ColorUtility.ToHtmlStringRGB(forceColor);
        string forceName = $"<color=#{colorHex}>{forceCfg.Cname}</color>";
        fairDesText.text = fairCfg.Des.Replace("{forceName}", forceName);
    }

    public void SetOnClose(Action callback)
    {
        onClose = callback;
    }

    public void OnHide()
    {
        IsShowing = false;
        onClose = null;
    }
}
