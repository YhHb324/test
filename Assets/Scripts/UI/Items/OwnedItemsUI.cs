using UnityEngine;

public class OwnedItemsUI : MonoBehaviour
{
    public ItemSlotUI[] slots;

    public ItemData[] itemDatas;

    // 仮データ
    public int[] itemCounts =
    {
        0,
        0,
        0,
        0,
        0,
        0
    };

    void Start()
    {
        Refresh();
    }

    public void GenerateItems()
    {
        int generateCount =
            DevelopmentManager.Instance.resourceTeams + 1;

        for (int i = 0; i < generateCount; i++)
        {
            // 0〜5 のランダム
            int randomIndex =
                Random.Range(0, itemCounts.Length);

            itemCounts[randomIndex]++;
        }

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetItem( itemDatas[i], itemCounts[i] );
        }
    }
}