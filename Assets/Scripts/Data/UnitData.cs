using UnityEngine;

[CreateAssetMenu(menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;

    public int hp;

    public int attack;

    public int range;

    public float attackSpeed;

    public int defense;

    public float moveSpeed;

    public Sprite sprite;
}