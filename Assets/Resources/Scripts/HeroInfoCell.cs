using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;

public class HeroInfoCell : MonoBehaviour, IPointerDownHandler
{
    public HeroInfoPanelManager heroInfoPanelManager;

    public int heroId;
    
    public TMP_Text heroNameText;
    public bool isSelect = false;
    public Image backgroundImage;
    public Color normalColor = Color.black;
    public Color selectedColor = Color.blue;

    // Start is called before the first frame update
    void Start()
    {
        heroNameText.raycastTarget = false;
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
        UpdateBackgroundColor();
    }

    public void Init(int heroId, string heroName)
    {
        this.heroId = heroId;
        heroNameText.text = heroName;
    }

    public void SetSelected(bool selected)
    {
        isSelect = selected;
        UpdateBackgroundColor();
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? selectedColor : normalColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        heroInfoPanelManager.OnSelectHero(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
