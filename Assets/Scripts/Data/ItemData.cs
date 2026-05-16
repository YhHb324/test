using UnityEngine;

[CreateAssetMenu(menuName = "ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;

    public Sprite icon;

    public int hpBonus;

    public int attackBonus;

    public int rangeBonus;

    public float attackSpeedBonus;

    public int defenseBonus;

    public float moveSpeedBonus;
}