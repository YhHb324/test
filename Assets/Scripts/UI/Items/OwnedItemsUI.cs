using UnityEngine;

public class OwnedItemsUI : MonoBehaviour
{
    public ItemSlotUI[] slots;

    public Sprite[] itemIcons;

    // 仮データ
    int[] itemCounts =
    {
        3,
        1,
        0,
        0,
        0,
        0
    };

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetItem(
                itemIcons[i],
                itemCounts[i]
            );
        }
    }
}