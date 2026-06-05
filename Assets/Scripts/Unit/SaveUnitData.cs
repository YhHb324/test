using UnityEngine;

[System.Serializable]
public class SavedUnitData
{
    public UnitData unitData;

    public ItemData[] items;

    public bool isOnBench;
    public bool isDamaged;

    public int x;
    public int y;

    public int benchIndex;
}