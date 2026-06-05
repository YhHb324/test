using UnityEngine;

public enum TileType
{
    Board,
    Bench,
    Recovery
}

public enum TileArea
{
    Player,
    Enemy,
    Bench,
    Recovery
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

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}