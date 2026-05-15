using UnityEngine;
using UnityEngine.EventSystems;

public class TenButtonUI :
    MonoBehaviour,
    IPointerClickHandler
{
    public LiquidSectionUI liquidSection;

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        // 左クリック
        if (
            eventData.button ==
            PointerEventData.InputButton.Left
        )
        {
            liquidSection.AddTen();
        }

        // 右クリック
        else if (
            eventData.button ==
            PointerEventData.InputButton.Right
        )
        {
            liquidSection.RemoveTen();
        }
    }
}