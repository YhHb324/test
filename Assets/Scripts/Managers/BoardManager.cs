using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public GameObject tilePrefab;

    int boardwidth = 7;
    int boardheight = 8;

    int benchHeight = 5;

    float tileWidth = 1f;
    float tileHeight = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateBoard();
        CreateBench();
    }

    void CreateBoard()
    {
        float offsetX = 0f;
        float offsetY = -3.5f;

        for (int y = 0; y < boardheight; y++)
        {
            for (int x = 0; x < boardwidth; x++)
            {
                Vector3 pos = new Vector3(
                    x * tileWidth + offsetX,
                    y * tileHeight + offsetY,
                    0
                    );
                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);

                Tile tileScript = tile.GetComponent<Tile>();

                tileScript.x = x;
                tileScript.y = y;
                tileScript.tileType = TileType.Board;
            }
        }
    }

    void CreateBench()
    {
        float benchX = -2f;
        float offsetY = -2f;

        for (int y = 0; y < benchHeight; y++)
        {
            Vector3 pos = new Vector3(
                benchX,
                y * tileHeight + offsetY,
                0
            );

            GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);

            Tile tileScript = tile.GetComponent<Tile>();

            tileScript.x = -1;
            tileScript.y = y;
            tileScript.tileType = TileType.Bench;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPos = Input.mousePosition;

            mouseScreenPos.z = 10f;

            Vector3 mouseWorldPos =
                Camera.main.ScreenToWorldPoint(mouseScreenPos);

            Vector2 mousePos2D =
                new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            Collider2D hit =
                Physics2D.OverlapPoint(mousePos2D);

            if (hit != null)
            {
                Tile tile =
                    hit.GetComponent<Tile>();

                if (tile != null)
                {
                    Debug.Log(
                        $"Clicked : {tile.tileType} ({tile.x +1}, {tile.y +1})"
                    );
                }
            }
        }
    }
}
