using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public List<SavedUnitData> savedUnits =
        new List<SavedUnitData>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SaveUnits()
    {
        savedUnits.Clear();

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        foreach (BattleUnit unit in units)
        {
            if (unit == null)
                continue;

            if (unit.isEnemy)
                continue;

            if (unit.isDead)
                continue;

            if (unit.currentTile == null)
                continue;

            SavedUnitData save =
                new SavedUnitData();

            save.unitData = unit.data;

            save.items =
                (ItemData[])unit.items.Clone();

            save.isOnBench =
                unit.currentTile.tileType
                == TileType.Bench;

            if (save.isOnBench)
            {
                save.benchIndex = unit.currentTile.y;
            }
            else
            {
                save.x = unit.currentTile.x;
                save.y = unit.currentTile.y;
            }

            savedUnits.Add(save);
        }

        Debug.Log("Saved Units : " + savedUnits.Count
        );
    }
}
