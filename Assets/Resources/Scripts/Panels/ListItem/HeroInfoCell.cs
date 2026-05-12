using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using CommonConfig;
using Controls.Utils;

public class HeroInfoCell : MonoBehaviour, IPointerDownHandler
{
    public HeroInfoPanelManager heroInfoPanelManager;

    public int heroId;
    
    public TMP_Text heroNameText;
    public bool isSelect = false;
    public Image backgroundImage;

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

    public void Init(int heroId, string heroName, string displayText = null)
    {
        this.heroId = heroId;
        heroNameText.text = !string.IsNullOrEmpty(displayText) ? displayText : heroName;
    }

    public void SetSelected(bool selected)
    {
        isSelect = selected;
        UpdateBackgroundColor();
        GameLog.Info($"HeroInfoCell.SetSelected: heroId={heroId}, selected={selected}, color={(backgroundImage != null ? backgroundImage.color.ToString() : "null")}");
    }

    private void UpdateBackgroundColor()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelect ? SysColor.Theme.CellSelected : SysColor.Theme.CellNormal;
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
