using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TroopsHeroSlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;
    public CityTroopsItem troopsItem;

    public void OnDrop(PointerEventData eventData)
    {
        ResetColor();

        CityCellHero draggedHero = eventData.pointerDrag?.GetComponent<CityCellHero>();
        if (draggedHero != null && troopsItem != null)
        {
            BGMPlayer.Instance.PlaySound("Sounds/equip");
            troopsItem.OnHeroDropped(draggedHero.heroId, slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.GetComponent<CityCellHero>() != null)
        {
            SetColor(Color.green);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetColor();
    }

    public void SetColor(Color color)
    {
        var image = GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
    }

    public void ResetColor()
    {
        SetColor(Color.white);
    }
}
