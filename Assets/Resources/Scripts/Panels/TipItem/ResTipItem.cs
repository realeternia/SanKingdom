using UnityEngine;
using TMPro;

public class ResTipItem : MonoBehaviour
{
    public TMP_Text nameText;
    public RectTransform bgRect;
    public float maxWidth = 350f;

    void Awake()
    {
        if (nameText == null)
            nameText = GetComponentInChildren<TMP_Text>();
        if (bgRect == null)
            bgRect = GetComponent<RectTransform>();

        if (nameText != null)
        {
            nameText.enableWordWrapping = true;
            nameText.overflowMode = TextOverflowModes.Overflow;
        }
    }

    public void SetName(string name)
    {
        if (nameText == null)
            return;

        nameText.text = name;

        nameText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
        Canvas.ForceUpdateCanvases();

        Vector2 preferred = nameText.GetPreferredValues();
        preferred.x = Mathf.Min(preferred.x, maxWidth);

        nameText.rectTransform.sizeDelta = preferred;

        if (bgRect != null)
        {
            bgRect.sizeDelta = new Vector2(preferred.x + 20f, preferred.y + 10f);
        }
    }
}
