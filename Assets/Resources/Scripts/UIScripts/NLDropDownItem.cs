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
            backgroundImage.color = isSelected ? SysColor.UI.DropDownSelected : SysColor.UI.DropDownNormal;
        }
    }
    
    public void OnPointerEnter()
    {
        if (!isSelected && backgroundImage != null)
        {
            backgroundImage.color = SysColor.UI.DropDownHover;
        }
    }
    
    public void OnPointerExit()
    {
        if (!isSelected && backgroundImage != null)
        {
            backgroundImage.color = SysColor.UI.DropDownNormal;
        }
    }
}
