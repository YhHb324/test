using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    public GameObject tilePrefab;
    public GameObject unitPrefab;

    Tile[,] boardTiles;
    Tile[] benchTiles;

    Unit draggingUnit;

    Vector3 dragOffset;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateBoard();
        CreateBench();
        SpawnUnit();
    }

    void Update()
    {
        HandleMouseInput();
    }

    // =========================
    // マウス入力
    // =========================

    void HandleMouseInput()
    {
        Vector3 mouseWorld =
            Camera.main.ScreenToWorldPoint(
                Input.mousePosition
            );

        mouseWorld.z = 0;

        // 押した瞬間
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit =
                Physics2D.OverlapPoint(mouseWorld);

            if (hit != null)
            {
                Unit unit =
                    hit.GetComponent<Unit>();

                if (unit != null)
                {
                    draggingUnit = unit;

                    dragOffset =
                        unit.transform.position
                        - mouseWorld;
                }
            }
        }

        // ドラッグ中
        if (
            draggingUnit != null &&
            Input.GetMouseButton(0)
        )
        {
            draggingUnit.transform.position =
                mouseWorld + dragOffset;
        }

        // 離した
        if (
            draggingUnit != null &&
            Input.GetMouseButtonUp(0)
        )
        {
            DropUnit(mouseWorld);
        }
    }

    // =========================
    // Unitを置く
    // =========================

    void DropUnit(Vector3 mouseWorld)
    {
        Collider2D[] hits =
            Physics2D.OverlapPointAll(mouseWorld);

        Tile targetTile = null;

        foreach (Collider2D hit in hits)
        {
            Tile tile =
                hit.GetComponent<Tile>();

            if (tile != null)
            {
                targetTile = tile;
                break;
            }
        }

        // タイルなし
        if (targetTile == null)
        {
            ReturnUnit();
            return;
        }

        //敵陣
        if (targetTile.tileArea == TileArea.Enemy)
        {
            ReturnUnit();
            return;
        }

        // 埋まってる
        if (
            targetTile.currentUnit != null &&
            targetTile.currentUnit != draggingUnit
        )
        {
            ReturnUnit();
            return;
        }

        // 元タイル空に
        draggingUnit.currentTile.currentUnit =
            null;

        // 新タイル
        draggingUnit.currentTile =
            targetTile;

        targetTile.currentUnit =
            draggingUnit;

        draggingUnit.transform.position =
            targetTile.transform.position;

        draggingUnit = null;
    }

    void ReturnUnit()
    {
        draggingUnit.transform.position =
            draggingUnit.currentTile.transform.position;

        draggingUnit = null;
    }

    // =========================
    // Board生成
    // =========================

    void CreateBoard()
    {
        boardTiles = new Tile[7, 8];

        float offsetY = -3.5f;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                Vector3 pos =
                    new Vector3(
                        x,
                        y + offsetY,
                        0
                    );

                GameObject obj =
                    Instantiate(
                        tilePrefab,
                        pos,
                        Quaternion.identity
                    );

                Tile tile =
                    obj.GetComponent<Tile>();

                tile.x = x;
                tile.y = y;
                tile.tileType = TileType.Board;

                // 下4列 = Player
                if (y < 4)
                {
                    tile.tileArea =
                        TileArea.Player;
                }

                // 上4列 = Enemy
                else
                {
                    tile.tileArea =
                        TileArea.Enemy;
                }

                tile.UpdateColor();

                boardTiles[x, y] = tile;
            }
        }
    }

    // =========================
    // Bench生成
    // =========================

    void CreateBench()
    {
        benchTiles = new Tile[5];

        for (int y = 0; y < 5; y++)
        {
            Vector3 pos =
                new Vector3(
                    -2,
                    y - 2,
                    0
                );

            GameObject obj =
                Instantiate(
                    tilePrefab,
                    pos,
                    Quaternion.identity
                );

            Tile tile =
                obj.GetComponent<Tile>();

            tile.tileType = TileType.Bench;
            tile.UpdateColor();

            benchTiles[y] = tile;
        }
    }

    // =========================
    // Unit生成
    // =========================

    void SpawnUnit()
    {
        Tile tile = benchTiles[0];

        GameObject obj =
            Instantiate(
                unitPrefab,
                tile.transform.position,
                Quaternion.identity
            );

        Unit unit =
            obj.GetComponent<Unit>();

        unit.currentTile = tile;

        tile.currentUnit = unit;
    }
}