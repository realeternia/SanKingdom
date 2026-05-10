using UnityEngine;
using UnityEngine.UI;

public class IconLoader : MonoBehaviour
{
    public string iconPath;

    void Start()
    {
        if (!string.IsNullOrEmpty(iconPath))
        {
            string path = ResPath.Texture.AttrIcon(iconPath);
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
}
