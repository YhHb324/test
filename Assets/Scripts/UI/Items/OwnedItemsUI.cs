using UnityEngine;

public class OwnedItemsUI : MonoBehaviour
{
    public ItemSlotUI[] slots;

    public ItemData[] itemDatas;

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
                Random.Range(0, ItemManager.Instance.itemCounts.Length);

            ItemManager.Instance.itemCounts[randomIndex]++;
        }

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetItem(itemDatas[i],ItemManager.Instance.itemCounts[i]);
        }
    }
}