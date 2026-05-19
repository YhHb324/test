using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI :
    MonoBehaviour,
    IPointerDownHandler
{
    public ItemData itemData;

    public Image iconImage;

    public TMP_Text countText;

    public Color ownedColor = Color.white;

    public Color unownedColor =
        new Color(0.3f, 0.3f, 0.3f, 1f);

    int currentCount;

    public void SetItem(
        ItemData data,
        int count
    )
    {
        itemData = data;

        currentCount = count;

        iconImage.sprite = data.icon;

        if (count <= 0)
        {
            iconImage.color = unownedColor;
            countText.gameObject.SetActive(false);
        }
        else
        {
            iconImage.color = ownedColor;

            countText.gameObject.SetActive(true);
            countText.text = "×" + count;
        }
    }

    public void OnPointerDown(
        PointerEventData eventData
    )
    {
        // Setup時のみ
        if (
            BattleManager.Instance.state
            != GameState.Setup1
            &&
            BattleManager.Instance.state
            != GameState.Setup2
        )
        {
            return;
        }

        if (itemData == null)
            return;

        if (currentCount <= 0)
            return;

        BoardManager.Instance.draggingItem =
            itemData;

        BoardManager.Instance.dragItemIcon.sprite =
            itemData.icon;

        BoardManager.Instance.dragItemIcon.enabled =
            true;

        Debug.Log(
            "Dragging : " +
            itemData.itemName
        );
    }
}