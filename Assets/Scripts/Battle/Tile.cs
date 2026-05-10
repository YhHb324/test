using UnityEngine;

public enum TileType
{
    Board,
    Bench
}

public class Tile : MonoBehaviour
{
    public int x;
    public int y;

    public TileType tileType;

}
