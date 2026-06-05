using System.Collections.Generic;
using UnityEngine;

public class RankUpManager : MonoBehaviour
{
    public static RankUpManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void CheckRankUp()
    {
        if (
            BattleManager.Instance.state != GameState.Setup1 &&
            BattleManager.Instance.state != GameState.Setup2
        )
        {
            return;
        }

        BattleUnit[] units =
            FindObjectsByType<BattleUnit>(
                FindObjectsSortMode.None);

        Dictionary<UnitData, List<BattleUnit>> groups =
            new();

        foreach (BattleUnit unit in units)
        {
            if (unit.isEnemy)
                continue;

            if (unit.data.rankUpResult == null)
                continue;

            if (!groups.ContainsKey(unit.data))
            {
                groups.Add(
                    unit.data,
                    new List<BattleUnit>());
            }

            groups[unit.data].Add(unit);
        }

        foreach (var pair in groups)
        {
            while (pair.Value.Count >= 3)
            {
                RankUp(pair.Value);
            }
        }
    }

    void RankUp(List<BattleUnit> units)
    {
        units.Sort(CompareUnitPriority);

        BattleUnit baseUnit = units[0];

        ItemData rankUpItem = baseUnit.evolutionKeyItem;

        int guaranteeCount = 4;

        List<BattleUnit> materials =
            new()
            {
                units[0],
                units[1],
                units[2]
            };

        List<ItemData> remainItems =
            new();

        foreach (BattleUnit unit in materials)
        {
            foreach (ItemData item in unit.items)
            {
                if (item == null)
                    continue;

                if (item == unit.evolutionKeyItem)
                    continue;

                remainItems.Add(item);
            }
        }

        UnitData nextData = baseUnit.data.rankUpResult;

        GameObject obj =
            Instantiate(
                nextData.unitPrefab,
                baseUnit.transform.position,
                Quaternion.identity
            );

        BattleUnit newUnit =
            obj.GetComponent<BattleUnit>();

        newUnit.data = nextData;

        newUnit.rankUpKeyItem =
            rankUpItem;

        newUnit.rankUpKeyItemCount =
            guaranteeCount;

        // 進化保証は消す
        newUnit.evolutionKeyItem = null;

        newUnit.currentTile =
            baseUnit.currentTile;

        newUnit.isOnBench =
            baseUnit.isOnBench;

        baseUnit.currentTile.currentUnit =
            newUnit;

        for (int i = 0;i < remainItems.Count && i < newUnit.items.Length;i++)
        {
            newUnit.items[i] =
                remainItems[i];
        }

        newUnit.RefreshStats();

        foreach (BattleUnit unit in materials)
        {
            units.Remove(unit);
            Destroy(unit.gameObject);
        }
    }

    int CompareUnitPriority(BattleUnit a,BattleUnit b)
    {
        bool aBoard =
            a.currentTile.tileType
            == TileType.Board;

        bool bBoard =
            b.currentTile.tileType
            == TileType.Board;

        if (aBoard && !bBoard)
            return -1;

        if (!aBoard && bBoard)
            return 1;

        if (aBoard)
        {
            if (a.currentTile.y != b.currentTile.y)
                return a.currentTile.y
                    .CompareTo(b.currentTile.y);

            return a.currentTile.x
                .CompareTo(b.currentTile.x);
        }

        return a.currentTile.x
            .CompareTo(b.currentTile.x);
    }
}