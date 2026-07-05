using System;
using System.Collections.Generic;
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

    public void Show(string name, int forceId, List<int> cityIds = null)
    {
        IsShowing = true;

        var fairCfg = FairConfig.GetConfigByName(name);
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

        string desText = fairCfg.Des;
        if (desText.Contains("{forceName}"))
        {
            var forceCfg = ForceConfig.GetConfig(forceId);
            Color forceColor = SysColor.GetForceColor(forceId);
            string colorHex = ColorUtility.ToHtmlStringRGB(forceColor);
            string forceName = $"<color=#{colorHex}>{forceCfg.Cname}</color>";
            desText = desText.Replace("{forceName}", forceName);
        }
        if (desText.Contains("{cityList}") && cityIds != null && cityIds.Count > 0)
        {
            var cityNames = new List<string>();
            foreach (var cid in cityIds)
            {
                cityNames.Add(ConfigNameHelper.GetCityName(cid));
            }
            desText = desText.Replace("{cityList}", string.Join(",", cityNames));
        }
        fairDesText.text = desText;
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
