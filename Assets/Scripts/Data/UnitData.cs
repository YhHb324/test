using UnityEngine;

[CreateAssetMenu(menuName = "Game/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Status")]
    public string unitName;

    public int hp;

    public int attack;

    public int range;

    public float attackSpeed;

    public int defense;

    public float moveSpeed;

    public Sprite sprite;

    [Header("Evolution")]
    public EvolutionData[] evolutions;

    [Header("Prefab")]
    public GameObject unitPrefab;

}