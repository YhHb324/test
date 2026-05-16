using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour
{
    public ItemData itemData;
    public Image iconImage;
    public TMP_Text countText;

    public Color ownedColor = Color.white;

    public Color unownedColor =
        new Color(0.3f, 0.3f, 0.3f, 1f);

    public void SetItem(ItemData data, int count)
    {
        itemData = data;

        iconImage.sprite = data.icon;

        // 未所持
        if (count <= 0)
        {
            iconImage.color = unownedColor;
            countText.gameObject.SetActive(false);
        }
        else
        {
            iconImage.color = ownedColor;

            // 2個以上なら表示
            if (count >= 2)
            {
                countText.gameObject.SetActive(true);
                countText.text = "×" + count;
            }
            else
            {
                countText.gameObject.SetActive(false);
            }
        }
    }

    void OnMouseDown()
    {
        if (itemData == null)
            return;

        OwnedItemsUI owner =
            FindFirstObjectByType<OwnedItemsUI>();

        int index =
            System.Array.IndexOf(
                owner.slots,
                this);

        if (owner.itemCounts[index] <= 0)
            return;

        BoardManager.Instance.draggingItem =
            itemData;

        Debug.Log(
            "Dragging " +
            itemData.itemName);
    }
}
