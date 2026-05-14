using CommonConfig;
using UnityEngine;
using UnityEngine.UI;
public enum IconSourceType
{
    Path,
    CityAttr,
    HeroAttr
}

public class IconLoader : MonoBehaviour
{
    public IconSourceType sourceType = IconSourceType.Path;
    public string iconPath;
    public int configId;

    void Start()
    {
        string path = ResolveIconPath();
        if (!string.IsNullOrEmpty(path))
        {
            Sprite sprite = ResourceCache.LoadSpriteUI(path);
            if (sprite != null)
            {
                Image image = GetComponent<Image>();
                if (image != null)
                {
                    image.sprite = sprite;
                }
            }
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
