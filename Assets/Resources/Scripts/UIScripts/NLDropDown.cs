using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NLDropDown : MonoBehaviour
{
    public TMP_Text dropdownText;
    public GameObject dropdownItemPrefab;
    public GameObject itemsNode;
    public Button triggerButton;
    public RectTransform itemsNodeRect;
    
    private List<string> options = new List<string>();
    private List<NLDropDownItem> items = new List<NLDropDownItem>();
    private int currentValue = 0;
    private bool isExpanded = false;
    
    public UnityEvent<int> onValueChanged = new UnityEvent<int>();
    
    public int value
    {
        get { return currentValue; }
        set
        {
            if (value >= 0 && value < options.Count && currentValue != value)
            {
                currentValue = value;
                UpdateDisplay();
                onValueChanged.Invoke(currentValue);
            }
            else if (value >= 0 && value < options.Count)
            {
                currentValue = value;
                UpdateDisplay();
            }
        }
    }
    
    public GameObject template
    {
        get { return itemsNode; }
        set { itemsNode = value; }
    }
    
    private void Start()
    {
        if (triggerButton != null)
        {
            triggerButton.onClick.AddListener(OnDropdownClick);
        }
        
        if (itemsNode != null)
        {
            itemsNode.SetActive(false);
        }
    }
    
    public void ClearOptions()
    {
        options.Clear();
        currentValue = 0;
        ClearItems();
        UpdateDisplay();
    }
    
    public void AddOptions(List<string> newOptions)
    {
        if (newOptions == null) return;
        
        options.AddRange(newOptions);
        UpdateDisplay();
    }
    
    public void AddOption(string option)
    {
        if (string.IsNullOrEmpty(option)) return;
        
        options.Add(option);
        UpdateDisplay();
    }
    
    private void OnDropdownClick()
    {
        if (isExpanded)
        {
            Collapse();
        }
        else
        {
            Expand();
        }
    }
    
    private void Expand()
    {
        if (itemsNode == null || dropdownItemPrefab == null) return;
        
        isExpanded = true;
        itemsNode.SetActive(true);
        
        CreateItems();
        UpdateItemsNodeHeight();
    }
    
    private void Collapse()
    {
        isExpanded = false;
        
        if (itemsNode != null)
        {
            itemsNode.SetActive(false);
        }
        
        ClearItems();
    }
    
    private void CreateItems()
    {
        ClearItems();
        
        if (dropdownItemPrefab == null || itemsNode == null) return;
        
        RectTransform prefabRect = dropdownItemPrefab.GetComponent<RectTransform>();
        float itemHeight = prefabRect != null ? prefabRect.sizeDelta.y : 30f;
        
        for (int i = 0; i < options.Count; i++)
        {
            GameObject itemObj = Instantiate(dropdownItemPrefab, itemsNode.transform);
            itemObj.transform.localScale = Vector3.one;
            itemObj.transform.localPosition = Vector3.zero;
            
            RectTransform itemRect = itemObj.GetComponent<RectTransform>();
            if (itemRect != null)
            {
                itemRect.anchoredPosition = new Vector2(0, -i * itemHeight);
                itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemHeight);
            }
            
            NLDropDownItem item = itemObj.GetComponent<NLDropDownItem>();
            if (item == null)
            {
                item = itemObj.AddComponent<NLDropDownItem>();
            }
            
            item.Init(this, i, options[i]);
            item.SetSelected(i == currentValue);
            items.Add(item);
        }
    }
    
    private void UpdateItemsNodeHeight()
    {
        if (itemsNodeRect == null || dropdownItemPrefab == null) return;
        
        RectTransform prefabRect = dropdownItemPrefab.GetComponent<RectTransform>();
        float itemHeight = prefabRect != null ? prefabRect.sizeDelta.y : 30f;
        float totalHeight = options.Count * itemHeight;
        
        itemsNodeRect.sizeDelta = new Vector2(itemsNodeRect.sizeDelta.x, totalHeight);
    }
    
    private void ClearItems()
    {
        foreach (var item in items)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        items.Clear();
    }
    
    private void UpdateDisplay()
    {
        if (dropdownText != null && currentValue >= 0 && currentValue < options.Count)
        {
            dropdownText.text = options[currentValue];
        }
    }
    
    public void OnItemSelected(int index)
    {
        if (index >= 0 && index < options.Count)
        {
            bool changed = (currentValue != index);
            currentValue = index;
            UpdateDisplay();
            
            if (changed)
            {
                onValueChanged.Invoke(currentValue);
            }
            
            Collapse();
        }
    }
    
    public List<string> GetOptions()
    {
        return new List<string>(options);
    }
    
    public void RefreshShownValue()
    {
        UpdateDisplay();
    }
}
