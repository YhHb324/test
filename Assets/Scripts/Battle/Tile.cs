using UnityEngine;

public enum TileType
{
    Board,
    Bench
}

public enum TileArea
{
    Player,
    Enemy,
    Bench
}

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public TileType tileType;

    public TileArea tileArea;

    [System.NonSerialized]
    public BattleUnit currentUnit;
    public BattleUnit reservedUnit;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    public void UpdateColor()
    {
        if (tileArea == TileArea.Enemy)
        {
            spriteRenderer.color =
                new Color(1f, 0.7f, 0.7f);
        }
    }
}