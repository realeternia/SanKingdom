using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NLDropDownItem : MonoBehaviour
{
    public TMP_Text itemText;
    public Button itemButton;
    public Image backgroundImage;
    
    private NLDropDown parentDropdown;
    private int index;
    private bool isSelected = false;
    
    private static readonly Color normalColor = new Color(0.2f, 0.2f, 0.25f, 0.9f);
    private static readonly Color selectedColor = new Color(0.3f, 0.5f, 0.7f, 1f);
    private static readonly Color hoverColor = new Color(0.35f, 0.35f, 0.4f, 0.95f);
    
    private void Start()
    {
        if (itemButton != null)
        {
            itemButton.onClick.AddListener(OnItemClick);
        }
    }
    
    public void Init(NLDropDown parent, int itemIndex, string text)
    {
        parentDropdown = parent;
        index = itemIndex;
        
        if (itemText != null)
        {
            itemText.text = text;
        }
        
        if (itemButton == null)
        {
            itemButton = GetComponent<Button>();
        }
        
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisual();
    }
    
    private void OnItemClick()
    {
        if (parentDropdown != null)
        {
            parentDropdown.OnItemSelected(index);
        }
    }
    
    private void UpdateVisual()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }
    }
    
    public void OnPointerEnter()
    {
        if (!isSelected && backgroundImage != null)
        {
            backgroundImage.color = hoverColor;
        }
    }
    
    public void OnPointerExit()
    {
        if (!isSelected && backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
}
