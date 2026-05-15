using CommonConfig;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum IconSourceType
{
    Path,
    CityAttr,
    HeroAttr
}

public class IconLoader : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public IconSourceType sourceType = IconSourceType.Path;
    public string iconPath;
    public int configId;
    public Image image;

    void Start()
    {
        string path = ResolveIconPath();
        if (!string.IsNullOrEmpty(path))
        {
            Sprite sprite = ResourceCache.LoadSpriteUI(path);
            if (sprite != null)
            {
                if (image == null)
                {
                    image = GetComponent<Image>();
                }
                if (image != null)
                {
                    image.sprite = sprite;
                }
            }
        }
    }

    public void SetId(int id)
    {
        configId = id;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (sourceType == IconSourceType.Path)
        {
            return;
        }

        string tipName = ResolveConfigName();
        if (string.IsNullOrEmpty(tipName))
        {
            return;
        }

        PanelManager.Instance.ShowTip(tipName, eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (sourceType == IconSourceType.Path)
        {
            return;
        }

        PanelManager.Instance.HideTip();
    }

    private string ResolveConfigName()
    {
        switch (sourceType)
        {
            case IconSourceType.CityAttr:
            {
                if (!CityAttrConfig.HasConfig(configId))
                {
                    GameLog.Error(string.Format("IconLoader CityAttrConfig不存在id={0}", configId));
                    return null;
                }
                CityAttrConfig cfg = CityAttrConfig.GetConfig(configId);
                return cfg.Cname;
            }
            case IconSourceType.HeroAttr:
            {
                if (!HeroAttrConfig.HasConfig(configId))
                {
                    GameLog.Error(string.Format("IconLoader HeroAttrConfig不存在id={0}", configId));
                    return null;
                }
                HeroAttrConfig cfg = HeroAttrConfig.GetConfig(configId);
                return cfg.Cname;
            }
            default:
                return null;
        }
    }

    private string ResolveIconPath()
    {
        switch (sourceType)
        {
            case IconSourceType.CityAttr:
            {
                if (!CityAttrConfig.HasConfig(configId))
                {
                    GameLog.Error(string.Format("IconLoader CityAttrConfig不存在id={0}", configId));
                    return null;
                }
                CityAttrConfig cfg = CityAttrConfig.GetConfig(configId);
                if (string.IsNullOrEmpty(cfg.Icon))
                {
                    return null;
                }
                return ResPath.Texture.AttrIcon(cfg.Icon);
            }
            case IconSourceType.HeroAttr:
            {
                if (!HeroAttrConfig.HasConfig(configId))
                {
                    GameLog.Error(string.Format("IconLoader HeroAttrConfig不存在id={0}", configId));
                    return null;
                }
                HeroAttrConfig cfg = HeroAttrConfig.GetConfig(configId);
                if (string.IsNullOrEmpty(cfg.Icon))
                {
                    return null;
                }
                return ResPath.Texture.AttrIcon(cfg.Icon);
            }
            default:
            {
                if (!string.IsNullOrEmpty(iconPath))
                {
                    return ResPath.Texture.AttrIcon(iconPath);
                }
                return null;
            }
        }
    }
}
