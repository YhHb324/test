using UnityEngine;
using UnityEngine.EventSystems;

public class DevelopmentSlotUI :
    MonoBehaviour,
    IPointerClickHandler
{
    public ResourceSectionUI resourceSection;

    public LiquidSectionUI liquidSection;

    public int index;

    public bool isLiquid;

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (isLiquid)
        {
            liquidSection.ClickSlot(index);
        }
        else
        {
            resourceSection.ClickSlot(index);
        }
    }
}