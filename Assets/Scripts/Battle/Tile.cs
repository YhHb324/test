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

    public BattleUnit currentUnit;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    public void UpdateColor()
    {
        // プレイヤー陣地
        if (tileArea == TileArea.Player)
        {
            spriteRenderer.color =
                new Color(0.7f, 0.8f, 1f, 0.7f);
        }

        // 敵陣地
        else if (tileArea == TileArea.Enemy)
        {
            spriteRenderer.color =
                new Color(1f, 0.7f, 0.7f, 0.7f);
        }

        // ベンチ
        else
        {
            spriteRenderer.color =
                Color.gray;
        }
    }
}